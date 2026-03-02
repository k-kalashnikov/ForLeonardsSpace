using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Era;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.Era5;
using Masofa.Client.Era5.Models;
using Masofa.Common.Attributes;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Globalization;

namespace Masofa.Web.Monolith.Jobs.Weather
{
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "EraWeatherDataLoaderJob", "Era")]
    public class EraWeatherDataLoaderJob : BaseJob<EraWeatherDataLoaderJobResult>, IJob
    {
        private Era5ApiUnitOfWork Era5ApiUnitOfWork { get; }
        private CountryBoundariesOptions CountryBoundaries { get; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; }

        public EraWeatherDataLoaderJob(ILogger<EraWeatherDataLoaderJob> logger, IBusinessLogicLogger businessLogicLogger, IMediator mediator, IConfiguration configuration, Era5ApiUnitOfWork era5ApiUnitOfWork, MasofaDictionariesDbContext masofaDictionariesDbContext, MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, MasofaCommonDbContext commonDbContext, MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            Era5ApiUnitOfWork = era5ApiUnitOfWork;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            CountryBoundaries = new CountryBoundariesOptions()
            {
                LatMin = configuration.GetValue<double>("CountryBoundaries:LatMin"),
                LatMax = configuration.GetValue<double>("CountryBoundaries:LatMax"),
                LonMin = configuration.GetValue<double>("CountryBoundaries:LonMin"),
                LonMax = configuration.GetValue<double>("CountryBoundaries:LonMax"),
                Step = configuration.GetValue<double>("CountryBoundaries:Step"),
            };
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation($"Start EraWeatherDataLoaderJob");
            try
            {
                if (CountryBoundaries.LatMin == 0 && CountryBoundaries.LatMax == 0 && CountryBoundaries.LonMin == 0 && CountryBoundaries.LonMax == 0)
                {
                    return;
                }

                var countryMp = await Mediator.Send(new GetCountryMultiPolygonCommand());

                NetTopologySuite.Geometries.Geometry? enlargedPolygon = null;
                if (countryMp != null)
                {
                    enlargedPolygon = countryMp.Buffer(100.0 / 111.32);
                }

                var latitudes = GenerateRange(CountryBoundaries.LatMin - 1, CountryBoundaries.LatMax + 1, CountryBoundaries.Step);
                var longitudes = GenerateRange(CountryBoundaries.LonMin - 1, CountryBoundaries.LonMax + 1, CountryBoundaries.Step);

                int batchSize = 3;
                var batches = new List<List<WeatherPoint>>();

                for (int i = 0; i < latitudes.Count; i += batchSize)
                {
                    var batch1 = new List<WeatherPoint>();
                    var slice = latitudes.Skip(i).Take(batchSize).ToList();
                    foreach (var lat in slice)
                    {
                        foreach (var lon in longitudes)
                        {
                            batch1.Add(new WeatherPoint(lat, lon));
                        }
                    }

                    if (batch1.Count > 0)
                    {
                        batches.Add(batch1);
                    }
                }

                IQueryable<Masofa.Common.Models.Dictionaries.Region> regionsQuery = MasofaDictionariesDbContext.Regions.AsNoTracking();
                regionsQuery = regionsQuery.Where(r => r.Status == Masofa.Common.Models.StatusType.Active);

                IQueryable<Masofa.Common.Models.Dictionaries.RegionMap> regionMapsQuery = MasofaDictionariesDbContext.RegionMaps.AsNoTracking();
                regionMapsQuery = regionMapsQuery.Where(rm => rm.Status == Masofa.Common.Models.StatusType.Active);

                IQueryable<Masofa.Common.Models.CropMonitoring.Field> fieldsQuery = MasofaCropMonitoringDbContext.Fields.AsNoTracking();
                fieldsQuery = fieldsQuery.Where(f => f.Status == Masofa.Common.Models.StatusType.Active);

                List<ERAWeatherResponse>? responseLists = [];

                foreach (var batch in batches)
                {
                    var index = batches.IndexOf(batch);
                    Console.WriteLine($"index: {index + 1,3} of {batches.Count,3}, count: {batch.Count}");

                    try
                    {
                        var responseList = await FetchWeatherDataAsync(batch, index);
                        if (responseList != null)
                        {
                            responseLists.AddRange(responseList);
                        }

                        await Task.Delay(TimeSpan.FromMinutes(2));
                    }
                    catch (Exception ex)
                    {
                        Logger.LogCritical(ex.Message);
                    }
                }

                foreach (var response in responseLists)
                {
                    try
                    {
                        NetTopologySuite.Geometries.Point? dot4326 = new(response.Longitude, response.Latitude) { SRID = 4326 };
                        NetTopologySuite.Geometries.Point? dot0 = new(response.Longitude, response.Latitude) { SRID = 0 };

                        Masofa.Common.Models.Dictionaries.RegionMap? regionMap = regionMapsQuery
                            .Where(m => m.Polygon != null &&
                                 ((m.Polygon.SRID == 4326 && m.Polygon.Contains(dot4326))
                               || (m.Polygon.SRID == 0 && m.Polygon.Contains(dot0))))
                            .FirstOrDefault();

                        Masofa.Common.Models.CropMonitoring.Field? field = fieldsQuery
                            .Where(m => m.Polygon != null &&
                                      ((m.Polygon.SRID == 4326 && m.Polygon.Contains(dot4326))
                                    || (m.Polygon.SRID == 0 && m.Polygon.Contains(dot0))))
                            .OrderBy(m => m.FieldArea ?? 0)
                            .FirstOrDefault();

                        var isInEnlargderPolygon = enlargedPolygon?.Contains(dot4326) ?? false;

                        if (!(isInEnlargderPolygon == true || regionMap != null || field != null)) continue;

                        Masofa.Common.Models.Dictionaries.Region? region = null;
                        if (regionMap != null)
                        {
                            region = regionsQuery
                                .Where(r => r.RegionMapId == regionMap.Id)
                                .OrderBy(r => r.RegionSquare)
                                .FirstOrDefault() ?? null;
                        }

                        var stationId = await Mediator.Send(new EraWeatherStationUpsertCommand()
                        {
                            Latitude = response.Latitude,
                            Longitude = response.Longitude,
                            RegionId = region?.Id ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                            Author = "system"
                        });

                        var wd = response.WeatherData;
                        if (wd == null) continue;

                        foreach (var time in wd.UtcTime ?? [])
                        {
                            var idx = wd.UtcTime?.IndexOf(time);
                            if (idx == null) continue;

                            var originalTime = wd.UtcTime?[idx.Value];
                            if (originalTime == null) continue;

                            originalTime = DateTime.SpecifyKind(originalTime.Value, DateTimeKind.Utc);

                            var weatherDataCreateRequest = new EraWeatherDataUpsertCommand()
                            {
                                OriginalDateTimeUtc = originalTime.Value,
                                Temperature = wd.Temperature?[idx.Value],
                                RelativeHumidity = wd.RelativeHumidity?[idx.Value],
                                DewPoint = wd.DewPoint?[idx.Value],
                                Precipitation = wd.Precipitation?[idx.Value],
                                CloudCover = wd.CloudCover?[idx.Value],
                                WindSpeed = wd.WindSpeed?[idx.Value],
                                WindDirection = wd.WindDirection?[idx.Value],
                                GroundTemperature = wd.GroundTemperature?[idx.Value],
                                SoilTemperature = wd.SoilTemperature?[idx.Value],
                                ConditionIds = wd.ConditionIds?[idx.Value],
                                SoilHumidity50cm = wd.SoilHumidity50cm?[idx.Value],
                                SoilHumidity2m = wd.SoilHumidity2m?[idx.Value],
                                SolarRadiation = wd.SolarRadiation?[idx.Value],
                                EraWeatherStationId = stationId
                            };

                            await Mediator.Send(weatherDataCreateRequest);
                        }
                    }
                    catch (Exception ex)
                    {
                        Result.Errors.Add(ex.Message);
                        Logger.LogError(ex, ex.Message);
                    }
                }

                Result.Message = $"Load weather data for batches {string.Join(";", batches.Select(m => string.Join(",", m.Select(w => $"{w.Latitude} {w.Longitude}"))))}";

                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success
                }, context);
            }
            catch (Exception ex)
            {
                Result.Errors.Add(ex.Message);
                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed
                }, context);
            }
            Logger.LogInformation($"End EraWeatherDataLoaderJob");
        }

        private static List<double> GenerateRange(double min, double max, double step)
        {
            var values = new List<double>();
            for (double value = min; value <= max; value = Math.Round(value, 2))
            {
                values.Add(Math.Round(value, 2));
                value += step;
            }
            return values;
        }

        private async Task<List<ERAWeatherResponse>> FetchWeatherDataAsync(List<WeatherPoint> batch, int index)
        {
            var latStr = string.Join(",", batch.Select(p => p.Latitude.ToString(CultureInfo.InvariantCulture)));
            var lonStr = string.Join(",", batch.Select(p => p.Longitude.ToString(CultureInfo.InvariantCulture)));

            try
            {
                var result = await Era5ApiUnitOfWork.Era5WeatherDataRepository.GetForecastWeatherDataAsync<List<ERAWeatherResponse>>(latStr, lonStr);
                return result;
            }
            catch
            {
                throw;
            }
        }
    }

    public class CountryBoundariesOptions
    {
        public double LatMin { get; set; }
        public double LatMax { get; set; }
        public double LonMin { get; set; }
        public double LonMax { get; set; }
        public double Step { get; set; }
    }

    public record WeatherPoint(double Latitude, double Longitude);

    public class EraWeatherDataLoaderJobResult : BaseJobResult
    {
        public string Message { get; set; }
    }
}
