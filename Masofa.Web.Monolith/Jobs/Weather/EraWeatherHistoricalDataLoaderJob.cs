using Masofa.BusinessLogic.Common;
using Masofa.BusinessLogic.Era;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.Era5;
using Masofa.Client.Era5.Models;
using Masofa.Common.Attributes;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Quartz;
using SixLabors.ImageSharp;
using System.Globalization;

namespace Masofa.Web.Monolith.Jobs.Weather
{
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "EraWeatherHistoricalDataLoaderJob", "Era")]
    public class EraWeatherHistoricalDataLoaderJob : BaseJob<EraWeatherDataLoaderJobResult>, IJob
    {
        private Era5ApiUnitOfWork Era5ApiUnitOfWork { get; }
        private CountryBoundariesOptions CountryBoundaries { get; }
        private ArchiveDatesOptions ArchiveDates { get; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; }

        private DateTime _startDate = new DateTime(2024, 1, 1);
        private DateTime _endDate = new DateTime(2025, 10, 01);
        private string _resultFileName = "/app/weatherReports/report.json";
        private List<DateTime> _resultDates = new List<DateTime>();

        public EraWeatherHistoricalDataLoaderJob(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger<EraWeatherHistoricalDataLoaderJob> logger, IConfiguration configuration, Era5ApiUnitOfWork era5ApiUnitOfWork, MasofaDictionariesDbContext masofaDictionariesDbContext, MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, MasofaCommonDbContext commonDbContext, MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
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
            ArchiveDates = new ArchiveDatesOptions()
            {
                ArchiveStartDate = configuration.GetValue<DateOnly>("Era5:ArchiveStartDate"),
                ArchiveEndDate = configuration.GetValue<DateOnly>("Era5:ArchiveEndDate")
            };
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            
            Logger.LogInformation($"Start EraWeatherDataLoaderJob");
            //_resultDates = GetResult();
            //SaveResult();
            //if (_resultDates.Any())
            //{
            //    _startDate = _resultDates.Last();
            //}
            //_startDate = GetLastDay();
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
                    enlargedPolygon = countryMp.Buffer(50.0 / 111.32);
                }

                var latitudes = GenerateRange(CountryBoundaries.LatMin - 2, CountryBoundaries.LatMax + 2, CountryBoundaries.Step);
                var longitudes = GenerateRange(CountryBoundaries.LonMin - 2, CountryBoundaries.LonMax + 2, CountryBoundaries.Step);

                int batchSize = 4;
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

                Console.Beep(800, 500);
                var regions = await MasofaDictionariesDbContext.Regions.ToListAsync();
                var regionMaps = await MasofaDictionariesDbContext.RegionMaps.ToListAsync();
                var fields = await MasofaCropMonitoringDbContext.Fields.ToListAsync();

                var startDate = new DateOnly(_startDate.Year, _startDate.Month, _startDate.Day);
                var endDate = startDate.AddYears(1);
                var finalDate = new DateOnly(_endDate.Year, _endDate.Month, _endDate.Day);

                List<ERAWeatherResponse>? responseLists = [];

                Console.Beep(800, 500);
                while (endDate < finalDate)
                {
                    foreach (var batch in batches)
                    {
                        var index = batches.IndexOf(batch);
                        Console.WriteLine($"index: {index + 1,3} of {batches.Count,3}, count: {batch.Count}");

                        try
                        {
                            var responseList = await FetchWeatherDataAsync(batch, index, startDate, endDate);
                            if (responseList != null)
                            {
                                responseLists.AddRange(responseList);
                            }
                            await Task.Delay(TimeSpan.FromSeconds(30));
                        }
                        catch (Exception ex)
                        {
                            Console.Beep(800, 500);
                            Console.WriteLine(ex.Message);
                        }
                    }

                    Console.Beep(800, 500);
                    startDate = endDate;
                    endDate = startDate.AddYears(1);
                    //SaveResult();
                }

                Console.Beep(800, 500);
                foreach (var response in responseLists)
                {
                    var dot = new NetTopologySuite.Geometries.Point(response.Longitude, response.Latitude);
                    var regionMap = regionMaps.AsEnumerable().Where(m => m.Polygon != null && m.Polygon.Contains(dot)).FirstOrDefault();
                    var field = fields.AsEnumerable().Where(m => m.Polygon != null && m.Polygon.Contains(dot)).OrderBy(m => m.FieldArea ?? 0).FirstOrDefault();
                    var isInEnlargderPolygon = enlargedPolygon?.Contains(dot) ?? false;

                    if (!(isInEnlargderPolygon == true || regionMap != null || field != null)) continue;
                    var region = regions.Where(r => r.RegionMapId == regionMap?.Id).OrderBy(r => r.RegionSquare).FirstOrDefault();
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

                Console.Beep(800, 500);
                Result.Message = $"Load weather data for batches {string.Join(";", batches.Select(m => string.Join(",", m.Select(w => $"{w.Latitude} {w.Longitude}"))))}";

                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success
                }, context);
            }
            catch (Exception ex)
            {
                Console.Beep(200, 1000);
                Result.Errors.Add(ex.Message);
                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed
                }, context);
            }
            Logger.LogInformation($"End EraWeatherDataLoaderJob");
        }

        //private void SaveResult()
        //{
        //    var resultJson = Newtonsoft.Json.JsonConvert.SerializeObject(_resultDates);
        //    EnsureDirectoriesExistRecursive(_resultFileName);
        //    if (File.Exists(_resultFileName))
        //    {
        //        File.Delete(_resultFileName);
        //    }
        //    File.WriteAllText(_resultFileName, resultJson);
        //}

        //private List<DateTime> GetResult()
        //{
        //    if (!File.Exists(_resultFileName))
        //    {
        //        return new List<DateTime>()
        //        {
        //            _startDate
        //        };
        //    }
        //    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateTime>>(File.ReadAllText(_resultFileName)) ?? new List<DateTime>();

        //}

        private DateTime GetLastDay()
        {
            var result = Environment.GetEnvironmentVariable("MASOFA_WEATHER_DATE_LOADED");
            if (string.IsNullOrEmpty(result))
            {
                return new DateTime(2016, 1, 1);
            }
            return DateTime.Parse(result);
        }

        private void PutLastDay(DateTime dateTime)
        {
            Environment.SetEnvironmentVariable("MASOFA_WEATHER_DATE_LOADED", dateTime.ToString("yyyy-MM-dd"));
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

        private async Task<List<ERAWeatherResponse>> FetchWeatherDataAsync(List<WeatherPoint> batch, int index, DateOnly startDate, DateOnly endDate)
        {
            var latStr = string.Join(",", batch.Select(p => p.Latitude.ToString(CultureInfo.InvariantCulture)));
            var lonStr = string.Join(",", batch.Select(p => p.Longitude.ToString(CultureInfo.InvariantCulture)));

            Console.WriteLine($"Запрос {index + 1} начат... (точек: {batch.Count})");

            try
            {
                var result = await Era5ApiUnitOfWork.Era5WeatherDataRepository.GetHistoricalWeatherDataAsync<List<ERAWeatherResponse>>(latStr, lonStr, startDate, endDate);
                Console.WriteLine($"Запрос {index + 1} result.Count: {result.Count}");
                return result;
            }
            catch
            {
                throw;
            }
        }

        private void EnsureDirectoriesExistRecursive(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("The value cannot be an empty", nameof(filePath));
            }

            string directoryPath = Path.GetDirectoryName(filePath);
            Console.WriteLine($"!!!!!!!!!!!!!! {filePath}");
            Console.WriteLine($"!!!!!!!!!!!!!! {directoryPath}");


            Logger.LogInformation($"{directoryPath}");

            if (string.IsNullOrEmpty(directoryPath))
            {
                return;
            }

            string[] pathParts = directoryPath.Split(Path.DirectorySeparatorChar);
            foreach (string pathPart in pathParts)
            {
                Console.WriteLine($"!!!!!!!!!!!!!! {pathPart}");
            }
            string currentPath = "/";

            foreach (string part in pathParts)
            {
                currentPath = Path.Combine(currentPath, part);

                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }

        public class ArchiveDatesOptions
        {
            public DateOnly ArchiveStartDate { get; set; }
            public DateOnly ArchiveEndDate { get; set; }
        }
    }
}
