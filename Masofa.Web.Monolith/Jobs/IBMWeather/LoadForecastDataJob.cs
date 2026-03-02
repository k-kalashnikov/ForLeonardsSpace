using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.IBMWeather;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.IBMWeather;
using Masofa.Client.IBMWeather.Models;
using Masofa.Common.Attributes;
using Masofa.Common.Models.IBMWeather;
using Masofa.DataAccess;
using Masofa.Web.Monolith.Jobs.Weather;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Quartz;

namespace Masofa.Web.Monolith.Jobs.IBMWeather
{
    /// <summary>
    /// Джоба для загрузки прогнозных данных IBM Weather ежедневно
    /// </summary>
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "LoadForecastDataJob", "IBMWeather")]
    public class LoadForecastDataJob : BaseJob<LoadForecastDataJobResult>, IJob
    {
        private IBMWeatherApiUnitOfWork IBMWeatherApiUnitOfWork { get; }
        private CountryBoundariesOptions CountryBoundaries { get; }
        private IBMWeatherServiceOptions Options { get; set; }
        private MasofaIBMWeatherDbContext IBMWeatherDbContext { get; set; }
        private MasofaDictionariesDbContext DictionariesDbContext { get; }
        private MasofaCropMonitoringDbContext CropMonitoringDbContext { get; }
        private readonly SemaphoreSlim _rateLimit = new SemaphoreSlim(1, 1);
        private readonly int _delayMs = 700;

        public LoadForecastDataJob(
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<LoadForecastDataJob> logger,
            IBMWeatherApiUnitOfWork iBMWeatherApiUnitOfWork,
            IConfiguration configuration,
            MasofaDictionariesDbContext dictionariesDbContext,
            MasofaCropMonitoringDbContext cropMonitoringDbContext,
            MasofaIBMWeatherDbContext iBMWeatherDbContext, 
            MasofaCommonDbContext commonDbContext, 
            MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            IBMWeatherApiUnitOfWork = iBMWeatherApiUnitOfWork;
            CountryBoundaries = new CountryBoundariesOptions()
            {
                LatMin = configuration.GetValue<double>("CountryBoundaries:LatMin"),
                LatMax = configuration.GetValue<double>("CountryBoundaries:LatMax"),
                LonMin = configuration.GetValue<double>("CountryBoundaries:LonMin"),
                LonMax = configuration.GetValue<double>("CountryBoundaries:LonMax"),
                Step = configuration.GetValue<double>("IBMWeather:Step"),
            };
            if (!IBMWeatherApiUnitOfWork.IsConfigured)
            {
                Options = new IBMWeatherServiceOptions
                {
                    ApiKey = configuration.GetValue<string>("IBMWeather:ApiKey") ?? throw new InvalidOperationException("IBMWeather:ApiKey not configured"),
                    BaseUrl = configuration.GetValue<string>("IBMWeather:BaseUrl") ?? "https://api.weather.com",
                    Language = configuration.GetValue<string>("IBMWeather:Language") ?? "en-US",
                    Format = configuration.GetValue<string>("IBMWeather:Format") ?? "json",
                    Units = configuration.GetValue<string>("IBMWeather:Units") ?? "s"
                };
                IBMWeatherApiUnitOfWork.Configure(Options);
            }

            DictionariesDbContext = dictionariesDbContext;
            CropMonitoringDbContext = cropMonitoringDbContext;
            IBMWeatherDbContext = iBMWeatherDbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation($"Start LoadForecastDataJob");
            try
            {
                if (CountryBoundaries.LatMin == 0 && CountryBoundaries.LatMax == 0 && CountryBoundaries.LonMin == 0 && CountryBoundaries.LonMax == 0)
                {
                    return;
                }

                var countryMp = await Mediator.Send(new GetCountryMultiPolygonCommand());

                Geometry? enlargedPolygon = null;
                if (countryMp != null)
                {
                    enlargedPolygon = countryMp.Buffer(100.0 / 111.32);
                }
                var points = GenerateGrid(CountryBoundaries.LatMin - 1, CountryBoundaries.LatMax + 1, CountryBoundaries.LonMin - 1, CountryBoundaries.LonMax + 1, CountryBoundaries.Step, enlargedPolygon);

                var responses = await CollectResponses(points);

                await ProcessResponses(responses);
            }
            catch (Exception ex)
            {
                Result.Errors.Add(ex.Message);
                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed
                }, context);
            }
            Logger.LogInformation($"End LoadForecastDataJob");
        }

        private List<(double lat, double lon)> GenerateGrid(double latMin, double latMax, double lonMin, double lonMax, double step, Geometry? enlargedPolygon)
        {
            var list = new List<(double, double)>();
            for (double lat = latMin; lat <= latMax; lat += step)
            {
                for (double lon = lonMin; lon <= lonMax; lon += step)
                {
                    if (enlargedPolygon != null)
                    {
                        if (enlargedPolygon.Covers(new NetTopologySuite.Geometries.Point(lon, lat)))
                        {
                            list.Add((Math.Round(lat, 2), Math.Round(lon, 2)));
                        }
                        continue;
                    }

                    list.Add((Math.Round(lat, 2), Math.Round(lon, 2)));
                }
            }
            return list;
        }

        private async Task<Dictionary<(double lat, double lon, Guid stationId), ForecastDailyResponse>> CollectResponses(List<(double lat, double lon)> points)
        {
            var regions = await DictionariesDbContext.Regions.ToListAsync();
            var regionMaps = await DictionariesDbContext.RegionMaps.ToListAsync();
            var fields = await CropMonitoringDbContext.Fields.ToListAsync();

            Dictionary<(double respLat, double respLon, Guid stationId), ForecastDailyResponse> responses = [];

            foreach (var (lat, lon) in points)
            {
                await _rateLimit.WaitAsync();
                try
                {
                    var forecastPoint = new NetTopologySuite.Geometries.Point(lon, lat) { SRID = 4326 };

                    var regionMap = regionMaps.AsEnumerable().Where(m => m.Polygon != null && m.Polygon.Contains(forecastPoint)).FirstOrDefault();
                    var field = fields.AsEnumerable().Where(m => m.Polygon != null && m.Polygon.Contains(forecastPoint)).OrderBy(m => m.FieldArea ?? 0).FirstOrDefault();
                    Masofa.Common.Models.Dictionaries.Region? region = null;
                    if (regionMap != null)
                    {
                        region = regions.Where(r => r.RegionMapId == regionMap?.Id).OrderBy(r => r.RegionSquare).FirstOrDefault();
                    }

                    var stationId = await Mediator.Send(new IbmWeatherStationUpsertCommand()
                    {
                        Point = forecastPoint,
                        RegionId = region?.Id ?? Guid.Empty,
                        FieldId = field?.Id ?? Guid.Empty,
                        Author = "system"
                    });

                    var forecastResponse = await IBMWeatherApiUnitOfWork.IBMWeatherRepository.GetDailyForecastAsync(forecastPoint);
                    if (forecastResponse != null)
                    {
                        responses[(lat, lon, stationId)] = forecastResponse;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogCritical(ex.Message);
                }
                finally
                {
                    _rateLimit.Release();
                    await Task.Delay(_delayMs);
                }
            }

            return responses;
        }

        private async Task ProcessResponses(Dictionary<(double lat, double lon, Guid stationId), ForecastDailyResponse> responses)
        {
            foreach (var ((lat, lon, stationId), forecastResponse) in responses)
            {
                List<IBMWeatherData> commonData = [];
                for (int i = 0; i < forecastResponse?.ValidTimeUtc?.Count; i++)
                {
                    var validTimeUtc = GetUtcTime(forecastResponse.ValidTimeUtc[i]);

                    var weatherData = new IBMWeatherData
                    {
                        Id = Guid.NewGuid(),
                        IBMMeteoStationId = stationId,
                        ValidTimeUtc = validTimeUtc,
                        Qpf = forecastResponse.Qpf?.Count > i ? forecastResponse.Qpf[i] : null,
                        QpfSnow = forecastResponse.QpfSnow?.Count > i ? forecastResponse.QpfSnow[i] : null,
                        TemperatureMax = forecastResponse.CalendarDayTemperatureMax?.Count > i ? forecastResponse.CalendarDayTemperatureMax[i] : null,
                        TemperatureMin = forecastResponse.CalendarDayTemperatureMin?.Count > i ? forecastResponse.CalendarDayTemperatureMin[i] : null,
                        DayOfWeek = forecastResponse.DayOfWeek?.Count > i ? forecastResponse.DayOfWeek[i] : null,
                        RequestedLatitude = lat,
                        RequestedLongitude = lon,
                        CreateAt = DateTime.UtcNow,
                        LastUpdateAt = DateTime.UtcNow,
                        Status = Masofa.Common.Models.StatusType.Active
                    };

                    commonData.Add(weatherData);
                }

                foreach (var daypart in forecastResponse?.Daypart ?? [])
                {
                    for (int i = 0; i < daypart.DayOrNight?.Count; i++)
                    {
                        var commonIdx = (i - (i % 2)) / 2;
                        var validCommon = commonData[commonIdx];

                        var dayOrNight = daypart.DayOrNight[i];

                        var daypartName = daypart.DaypartName?.Count > i ? daypart.DaypartName[i] : null;
                        var temperature = daypart.Temperature?.Count > i ? daypart.Temperature[i] : null;
                        var precipChance = daypart.PrecipChance?.Count > i ? daypart.PrecipChance[i] : null;
                        var qpf = daypart.Qpf?.Count > i ? daypart.Qpf[i] : null;
                        var qpfSnow = daypart.QpfSnow?.Count > i ? daypart.QpfSnow[i] : null;
                        var windSpeed = daypart.WindSpeed?.Count > i ? daypart.WindSpeed[i] : null;
                        var relativeHumidity = daypart.RelativeHumidity?.Count > i ? daypart.RelativeHumidity[i] : null;
                        var windDirection = daypart.WindDirection?.Count > i ? daypart.WindDirection[i] : null;

                        var daypartData = new IBMWeatherData
                        {
                            Id = Guid.NewGuid(),
                            IBMMeteoStationId = stationId,
                            Temperature = temperature,
                            WindSpeed = windSpeed,
                            WindDirection = windDirection,
                            DayOrNight = dayOrNight,
                            PrecipChance = precipChance,
                            Qpf = qpf,
                            QpfSnow = qpfSnow,
                            RelativeHumidity = relativeHumidity,
                            RequestedLatitude = lat,
                            RequestedLongitude = lon,
                            ValidTimeUtc = validCommon.ValidTimeUtc,
                            TemperatureMax = validCommon.TemperatureMax,
                            TemperatureMin = validCommon.TemperatureMin,
                            DayOfWeek = validCommon.DayOfWeek,
                            CreateAt = DateTime.UtcNow,
                            LastUpdateAt = DateTime.UtcNow,
                            Status = Masofa.Common.Models.StatusType.Active
                        };

                        var newGuid = await Mediator.Send(new IbmWeatherDataUpsertCommand()
                        {
                            Data = daypartData
                        });
                    }
                }
            }
        }

        private DateTime GetUtcTime(int? unixMsLong)
        {
            if (unixMsLong == null)
            {
                return DateTime.UtcNow;
            }

            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixMsLong.Value);
        }
    }

    public class LoadForecastDataJobResult : BaseJobResult
    {
        public string Message { get; set; }
    }
}