using Masofa.BusinessLogic.Era;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.Era5;
using Masofa.Client.Era5.Models;
using Masofa.Common.Models.Era;
using Masofa.Common.Models.Satellite.Parse.Sentinel.Inspire;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using SixLabors.ImageSharp;
using System.Data;
using System.Globalization;
using System.Threading;

namespace Masofa.Cli.DevopsUtil.Commands.Weather
{
    [BaseCommand("Weather History Load", "Загрузка данных по погоде по датам")]
    public class EraWeatherHistoricalDataLoaderCommand : IBaseCommand
    {
        private DateOnly _endDate = new DateOnly(2016, 1, 1);
        private DateOnly _startDate = new DateOnly(2025, 10, 4);

        private string _resultFileName = string.Empty;
        private List<DateOnly> _resultDates = new List<DateOnly>();

        private Era5ApiUnitOfWork Era5ApiUnitOfWork { get; }
        private CountryBoundariesOptions CountryBoundaries { get; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; }
        private MasofaIdentityDbContext MasofaIdentityDbContext { get; }
        private MasofaEraDbContext EraDbContext { get; set; }
        protected ILogger Logger { get; set; }

        public EraWeatherHistoricalDataLoaderCommand(MasofaDictionariesDbContext masofaDictionariesDbContext,
            MasofaCropMonitoringDbContext masofaCropMonitoringDbContext,
            Era5ApiUnitOfWork era5ApiUnitOfWork,
            ILogger<EraWeatherHistoricalDataLoaderCommand> logger,
            MasofaEraDbContext eraDbContext,
            MasofaIdentityDbContext masofaIdentityDbContext)
        {
            CountryBoundaries = new CountryBoundariesOptions()
            {
                LatMin = 37.00,
                LatMax = 45.50,
                LonMin = 56.00,
                LonMax = 73.00,
                Step = 0.25,
            };
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            Era5ApiUnitOfWork = era5ApiUnitOfWork;
            Logger = logger;
            EraDbContext = eraDbContext;
            MasofaIdentityDbContext = masofaIdentityDbContext;
        }

        public async Task Execute()
        {
            try
            {
                if (CountryBoundaries.LatMin == 0 && CountryBoundaries.LatMax == 0 && CountryBoundaries.LonMin == 0 && CountryBoundaries.LonMax == 0)
                {
                    return;
                }

                var latitudes = GenerateRange(CountryBoundaries.LatMin, CountryBoundaries.LatMax, CountryBoundaries.Step);
                var longitudes = GenerateRange(CountryBoundaries.LonMin, CountryBoundaries.LonMax, CountryBoundaries.Step);

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

                var regions = await MasofaDictionariesDbContext.Regions.AsNoTracking().ToListAsync();
                Console.WriteLine($"Regions count: {regions.Count()}");

                var regionMaps = await MasofaDictionariesDbContext.RegionMaps.AsNoTracking().ToListAsync();
                Console.WriteLine($"Regions Map count: {regionMaps.Count()}");

                var fields = await MasofaCropMonitoringDbContext.Fields.AsNoTracking().ToListAsync();
                Console.WriteLine($"Fields count: {fields.Count()}");


                var endDateRequest = _startDate;
                var startDateRequest = endDateRequest.AddMonths(-1);
                var finalDateRequest = _endDate;

                while (startDateRequest >= finalDateRequest)
                {
                    Console.WriteLine($"Load from {startDateRequest} to {endDateRequest}");
                    foreach (var batch in batches)
                    {
                        var index = batches.IndexOf(batch);
                        Console.WriteLine($"index: {index}, count: {batch.Count}");

                        var responseList = await FetchWeatherDataAsync(batch, index, startDateRequest, endDateRequest);

                        Console.WriteLine($"Loaded count {responseList.Count()}");

                        var responseIndex = 0;
                        foreach (var response in responseList)
                        {
                            var progressLine = $"\rSave {responseIndex} of {responseList.Count()}";
                            Console.Write(progressLine.PadRight(Console.WindowWidth - 1));
                            var dot = new NetTopologySuite.Geometries.Point(response.Longitude, response.Latitude);
                            var regionMap = regionMaps.Where(m => m.Polygon != null && m.Polygon.Contains(dot)).FirstOrDefault();
                            var field = fields.Where(m => m.Polygon != null && m.Polygon.Contains(dot)).OrderBy(m => m.FieldArea ?? 0).FirstOrDefault();

                            //if ((regionMap == null) && (field == null))
                            //{
                            //    Console.WriteLine($"Field and Region not found for Lng: {response.Longitude}; Lat: {response.Latitude}");
                            //    continue;
                            //}


                            var region = regions.Where(r => r.RegionMapId == regionMap?.Id).OrderBy(r => r.RegionSquare).FirstOrDefault();

                            var stationId = await CreateWeatherStationRecord(new EraWeatherStationUpsertCommand()
                            {
                                Latitude = response.Latitude,
                                Longitude = response.Longitude,
                                RegionId = region?.Id ?? Guid.Empty,
                                FieldId = field?.Id ?? Guid.Empty,
                                Author = "Admin"
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
                                    EraWeatherStationId = stationId
                                };

                                await CreateWeatherDataRecord(weatherDataCreateRequest);
                            }
                            responseIndex++;
                        }
                        Console.WriteLine();
                        Console.WriteLine("Wait 2 minuts");
                        await Task.Delay(TimeSpan.FromMinutes(2));
                    }

                    await CalculateReports(startDateRequest, endDateRequest);
                    await Task.Delay(TimeSpan.FromMinutes(20));

                    endDateRequest = startDateRequest;
                    startDateRequest = endDateRequest.AddMonths(-1);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                Logger.LogError(ex.InnerException?.Message);
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
        private async Task<List<ERAWeatherResponse>> FetchWeatherDataAsync(List<WeatherPoint> batch, int index, DateOnly startDate, DateOnly endDate)
        {
            var latStr = string.Join(",", batch.Select(p => p.Latitude.ToString(CultureInfo.InvariantCulture)));
            var lonStr = string.Join(",", batch.Select(p => p.Longitude.ToString(CultureInfo.InvariantCulture)));

            try
            {
                var result = await Era5ApiUnitOfWork.Era5WeatherDataRepository.GetHistoricalWeatherDataAsync<List<ERAWeatherResponse>>(latStr, lonStr, startDate, endDate);
                return result;
            }
            catch
            {
                throw;
            }
        }

        private void SaveResult()
        {
            var resultJson = Newtonsoft.Json.JsonConvert.SerializeObject(_resultDates);
            if (File.Exists(_resultFileName))
            {
                File.Delete(_resultFileName);
            }
            File.WriteAllText(resultJson, _resultFileName);
        }

        private List<DateOnly> GetLastDate(DateOnly start, DateOnly end)
        {
            var result = new List<DateOnly>();
            for (DateOnly currentDate = start; currentDate >= end; currentDate = currentDate.AddDays(-1))
            {
                var modelName = "EraWeatherData";
                var tableName = $"{modelName}_{currentDate.ToString("yyyy_MM_dd")}";

                if (!EraDbContext.IsTableExists(tableName))
                {
                    continue;
                }
                result.Add(currentDate);
            }
            return result;
        }

        private async Task CreateWeatherDataRecord(EraWeatherDataUpsertCommand request)
        {
            try
            {
                var partitionDate = request.OriginalDateTimeUtc.Date;
                var modelName = "EraWeatherData";
                var tableName = $"{modelName}_{partitionDate.ToString("yyyy_MM_dd")}";
                var id = Guid.NewGuid();

                var sql = $@"
                    INSERT INTO ""{tableName}"" (
                        ""Id"", ""OriginalDateTimeUtc"", ""EraWeatherStationId"",
                        ""Temperature"", ""RelativeHumidity"", ""DewPoint"", ""Precipitation"",
                        ""CloudCover"", ""WindSpeed"", ""WindDirection"", ""GroundTemperature"",
                        ""SoilTemperature"", ""ConditionIds"", ""SoilHumidity50cm"", ""SoilHumidity2m"",
                        ""SolarRadiation""
                    )
                    VALUES (
                        @id, @originalDateTimeUtc, @eraWeatherStationId,
                        @temperature, @relativeHumidity, @dewPoint, @precipitation,
                        @cloudCover, @windSpeed, @windDirection, @groundTemperature,
                        @soilTemperature, @conditionIds, @soilHumidity50cm, @soilHumidity2m,
                        @solarRadiation
                    )
                    ON CONFLICT (""OriginalDateTimeUtc"", ""EraWeatherStationId"")
                    DO UPDATE SET
                        ""Temperature"" = EXCLUDED.""Temperature"",
                        ""RelativeHumidity"" = EXCLUDED.""RelativeHumidity"",
                        ""DewPoint"" = EXCLUDED.""DewPoint"",
                        ""Precipitation"" = EXCLUDED.""Precipitation"",
                        ""CloudCover"" = EXCLUDED.""CloudCover"",
                        ""WindSpeed"" = EXCLUDED.""WindSpeed"",
                        ""WindDirection"" = EXCLUDED.""WindDirection"",
                        ""GroundTemperature"" = EXCLUDED.""GroundTemperature"",
                        ""SoilTemperature"" = EXCLUDED.""SoilTemperature"",
                        ""ConditionIds"" = EXCLUDED.""ConditionIds"",
                        ""SoilHumidity50cm"" = EXCLUDED.""SoilHumidity50cm"",
                        ""SoilHumidity2m"" = EXCLUDED.""SoilHumidity2m"",
                        ""SolarRadiation"" = EXCLUDED.""SolarRadiation""
                    RETURNING ""Id""";

                var parameters = new[]
                {
                    new NpgsqlParameter("@id", Guid.NewGuid()),
                    new NpgsqlParameter("@originalDateTimeUtc", request.OriginalDateTimeUtc),
                    new NpgsqlParameter("@eraWeatherStationId", request.EraWeatherStationId),
                    new NpgsqlParameter("@temperature", (object)request.Temperature ?? DBNull.Value),
                    new NpgsqlParameter("@relativeHumidity", (object)request.RelativeHumidity ?? DBNull.Value),
                    new NpgsqlParameter("@dewPoint", (object)request.DewPoint ?? DBNull.Value),
                    new NpgsqlParameter("@precipitation", (object)request.Precipitation ?? DBNull.Value),
                    new NpgsqlParameter("@cloudCover", (object)request.CloudCover ?? DBNull.Value),
                    new NpgsqlParameter("@windSpeed", (object)request.WindSpeed ?? DBNull.Value),
                    new NpgsqlParameter("@windDirection", (object)request.WindDirection ?? DBNull.Value),
                    new NpgsqlParameter("@groundTemperature", (object)request.GroundTemperature ?? DBNull.Value),
                    new NpgsqlParameter("@soilTemperature", (object)request.SoilTemperature ?? DBNull.Value),
                    new NpgsqlParameter("@conditionIds", (object)request.ConditionIds ?? DBNull.Value),
                    new NpgsqlParameter("@soilHumidity50cm", (object)request.SoilHumidity50cm ?? DBNull.Value),
                    new NpgsqlParameter("@soilHumidity2m", (object)request.SoilHumidity2m ?? DBNull.Value),
                    new NpgsqlParameter("@solarRadiation", (object)request.SolarRadiation ?? DBNull.Value)
                };

                if (!EraDbContext.IsTableExists(tableName))
                {
                    EraDbContext.CreatePartitionForDateAsync(modelName, DateOnly.FromDateTime(partitionDate));
                }

                var connectionString = "Host=185.100.234.107;Port=20010;Database=agrosence-era-prod;Username=sixgrain-test;Password=xZ9vBc5nM3Qw;"; // или из конфигурации

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var command = new NpgsqlCommand(sql, connection)
                    {
                        CommandTimeout = 30
                    };
                    command.Parameters.AddRange(parameters);
                    var scalar = await command.ExecuteScalarAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
                Logger.LogCritical(ex.InnerException?.Message);
                throw ex;
            }
        }


        //private async Task CreateWeatherDataRecord(EraWeatherDataUpsertCommand request)
        //{
        //    try
        //    {
        //        var partitionDate = request.OriginalDateTimeUtc.Date;
        //        var modelName = "EraWeatherData";
        //        var tableName = $"{modelName}_{partitionDate.ToString("yyyy_MM_dd")}";
        //        var id = Guid.NewGuid();

        //        var sql = $"INSERT INTO \"{tableName}\" (\n" +
        //                  $"\"Id\"\n" +
        //                  $"\"OriginalDateTimeUtc\",\n" +
        //                  $"\"EraWeatherStationId\",\n" +
        //                  $"\"Temperature\",\n" +
        //                  $"\"RelativeHumidity\",\n" +
        //                  $"\"DewPoint\",\n" +
        //                  $"\"Precipitation\",\n" +
        //                  $"\"CloudCover\",\n" +
        //                  $"\"WindSpeed\",\n" +
        //                  $"\"WindDirection\",\n" +
        //                  $"\"GroundTemperature\",\n" +
        //                  $"\"SoilTemperature\",\n" +
        //                  $"\"ConditionIds\",\n" +
        //                  $"\"SoilHumidity50cm\",\n" +
        //                  $"\"SoilHumidity2m\",\n" +
        //                  $"\"SolarRadiation\"\n" +
        //                  $")\n" +
        //                  $"VALUES (" +
        //                  $"'{id.ToString()}'," +
        //                  $"'{request.OriginalDateTimeUtc.ToString("yyyy-MM-dd HH:mm:ss")}+03:00'," +
        //                  $"'{request.EraWeatherStationId}'," +
        //                  $"{(request.Temperature.HasValue ? request.Temperature.Value.ToString(CultureInfo.InvariantCulture) : "NULL")}," +
        //                  $"{(request.RelativeHumidity.HasValue ? request.RelativeHumidity.Value.ToString(CultureInfo.InvariantCulture) : "NULL")}," +
        //                  $"{(request.DewPoint.HasValue ? request.DewPoint.Value.ToString(CultureInfo.InvariantCulture) : "NULL")}," +
        //                  $"{(request.Precipitation.HasValue ? request.Precipitation.Value.ToString(CultureInfo.InvariantCulture) : "NULL")}," +
        //                  $"{(request.CloudCover.HasValue ? request.CloudCover.Value.ToString(CultureInfo.InvariantCulture) : "NULL")}," +
        //                  $"{(request.WindSpeed.HasValue ? request.WindSpeed.Value.ToString(CultureInfo.InvariantCulture) : "NULL")}," +
        //                  $"{(request.WindDirection.HasValue ? request.WindDirection.Value.ToString(CultureInfo.InvariantCulture) : "NULL")}," +
        //                  $"{(request.GroundTemperature.HasValue ? request.GroundTemperature.Value.ToString(CultureInfo.InvariantCulture) : "NULL")}," +
        //                  $"{(request.SoilTemperature.HasValue ? request.SoilTemperature.Value.ToString(CultureInfo.InvariantCulture) : "NULL")}," +
        //                  $"{(request.ConditionIds.HasValue ? request.ConditionIds.Value.ToString() : "NULL")}," +
        //                  $"{(request.SoilHumidity50cm.HasValue ? request.SoilHumidity50cm.Value.ToString(CultureInfo.InvariantCulture) : "NULL")}," +
        //                  $"{(request.SoilHumidity2m.HasValue ? request.SoilHumidity2m.Value.ToString(CultureInfo.InvariantCulture) : "NULL")}," +
        //                  $"{(request.SolarRadiation.HasValue ? request.SolarRadiation.Value.ToString(CultureInfo.InvariantCulture) : "NULL")}" +
        //                  $")" +
        //                  $"ON CONFLICT (\"OriginalDateTimeUtc\", \"EraWeatherStationId\")\n" +
        //                  $"DO UPDATE SET\n" +
        //                  $"\"Temperature\" = EXCLUDED.\"Temperature\",\n" +
        //                  $"\"RelativeHumidity\" = EXCLUDED.\"RelativeHumidity\",\n" +
        //                  $"\"DewPoint\" = EXCLUDED.\"DewPoint\",\n" +
        //                  $"\"Precipitation\" = EXCLUDED.\"Precipitation\",\n" +
        //                  $"\"CloudCover\" = EXCLUDED.\"CloudCover\",\n" +
        //                  $"\"WindSpeed\" = EXCLUDED.\"WindSpeed\",\n" +
        //                  $"\"WindDirection\" = EXCLUDED.\"WindDirection\",\n" +
        //                  $"\"GroundTemperature\" = EXCLUDED.\"GroundTemperature\",\n" +
        //                  $"\"SoilTemperature\" = EXCLUDED.\"SoilTemperature\",\n" +
        //                  $"\"ConditionIds\" = EXCLUDED.\"ConditionIds\",\n" +
        //                  $"\"SoilHumidity50cm\" = EXCLUDED.\"SoilHumidity50cm\",\n" +
        //                  $"\"SoilHumidity2m\" = EXCLUDED.\"SoilHumidity2m\",\n" +
        //                  $"\"SolarRadiation\" = EXCLUDED.\"SolarRadiation\"\n" +
        //                  $";";


        //        if (!EraDbContext.IsTableExists(tableName))
        //        {
        //            EraDbContext.CreatePartitionForDateAsync(modelName, DateOnly.FromDateTime(partitionDate));
        //        }
        //        EraDbContext.Database.ExecuteSqlRaw(sql);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogCritical(ex.Message);
        //        Logger.LogCritical(ex.InnerException?.Message);
        //        throw ex;
        //    }
        //}

        private async Task<Guid> CreateWeatherStationRecord(EraWeatherStationUpsertCommand request)
        {
            try
            {
                var station = await GetEraWeatherStationGetByCoordinates(new EraWeatherStationGetByCoordinatesCommand()
                {
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                });

                var lastUpdateUser = await MasofaIdentityDbContext.Set<Masofa.Common.Models.Identity.User>()
                    .FirstOrDefaultAsync(m => m.UserName.ToLower().Equals(request.Author.ToLower()));
                if (station == null)
                {
                    var p = new NetTopologySuite.Geometries.Point(request.Longitude, request.Latitude)
                    {
                        SRID = 4326
                    };
                    station = new EraWeatherStation()
                    {
                        Point = p,
                        CreateAt = DateTime.UtcNow,
                        CreateUser = lastUpdateUser != null ? lastUpdateUser.Id : Guid.Empty,
                        RegionId = request.RegionId,
                        FieldId = request.FieldId,
                        Status = request.Status,
                        LastUpdateAt = DateTime.UtcNow,
                        LastUpdateUser = lastUpdateUser != null ? lastUpdateUser.Id : Guid.Empty
                    };
                    var result = await EraDbContext.EraWeatherStations.AddAsync(station);
                    await EraDbContext.SaveChangesAsync();
                    return result.Entity.Id;
                }

                if ((station.RegionId != request.RegionId))
                {

                    station.RegionId = request.RegionId;
                    station.Status = Common.Models.StatusType.Active;
                    station.LastUpdateAt = DateTime.UtcNow;
                    station.LastUpdateUser = (lastUpdateUser != null) ? lastUpdateUser.Id : Guid.Empty;

                    EraDbContext.EraWeatherStations.Update(station);
                    await EraDbContext.SaveChangesAsync();
                }

                return station.Id;
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
                Logger.LogCritical(ex.InnerException?.Message);
                throw ex;
            }
        }
        private async Task<EraWeatherStation?> GetEraWeatherStationGetByCoordinates(EraWeatherStationGetByCoordinatesCommand request)
        {
            var station = await EraDbContext.EraWeatherStations
                .Where(s => s.Point.X == request.Longitude && s.Point.Y == request.Latitude)
                .FirstOrDefaultAsync();

            return station;
        }
        private async Task CalculateReports(DateOnly start, DateOnly end)
        {
            Console.WriteLine($"Start calculation reports from {start} to {end}");
            for (DateOnly currentDay = start; currentDay <= end; currentDay = currentDay.AddDays(1))
            {
                Console.WriteLine($"Start calculation reports for {currentDay.ToString("yyyy-MM-dd")}");
                await CalculateDaysReport(currentDay);
            }

            for (DateOnly currentDay = start; currentDay <= end; currentDay = currentDay.AddDays(7))
            {
                Console.WriteLine($"Start calculation reports from {currentDay.ToString("yyyy-MM-dd")} to {currentDay.AddDays(6).ToString("yyyy-MM-dd")}");
                await CalculateWeekReport(currentDay);
            }

            for (DateOnly currentDay = start; currentDay <= end; currentDay = currentDay.AddMonths(1))
            {
                Console.WriteLine($"Start calculation reports for {currentDay.ToString("yyyy-MM")}");
                await CalculateMonthReport(currentDay);
            }

            for (DateOnly currentDay = start; currentDay <= end; currentDay = currentDay.AddYears(1))
            {
                Console.WriteLine($"Start calculation reports for {currentDay.ToString("yyyy")}");
                await CalculateYearReport(currentDay);
            }
        }
        private async Task CalculateDaysReport(DateOnly date)
        {
            var partitionDate = date.ToString("yyyy_MM_dd");
            var modelName = "EraWeatherData";
            var tableName = $"\"{modelName}_{partitionDate}\"";

            var sqlSelect = $"SELECT\n" +
                        $"\"Id\",\n" +
                        $"\"OriginalDateTimeUtc\",\n" +
                        $"\"EraWeatherStationId\",\n" +
                        $"\"Temperature\",\n" +
                        $"\"RelativeHumidity\",\n" +
                        $"\"DewPoint\",\n" +
                        $"\"Precipitation\",\n" +
                        $"\"CloudCover\",\n" +
                        $"\"WindSpeed\",\n" +
                        $"\"WindDirection\",\n" +
                        $"\"GroundTemperature\",\n" +
                        $"\"SoilTemperature\",\n" +
                        $"\"ConditionIds\",\n" +
                        $"\"SoilHumidity50cm\",\n" +
                        $"\"SoilHumidity2m\",\n" +
                        $"\"SolarRadiation\"\n" +
                    $"FROM {tableName};";

            var connectionString = "Host=185.100.234.107;Port=20010;Database=agrosence-era-prod;Username=sixgrain-test;Password=xZ9vBc5nM3Qw;";
            List<Masofa.Common.Models.Era.EraWeatherData> scalar = new List<Masofa.Common.Models.Era.EraWeatherData>();
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand(sqlSelect, connection)
                {
                    CommandTimeout = 30
                };
                using (var reader = command.ExecuteReader()) 
                {
                    while (await reader.ReadAsync())
                    {
                        var item = new EraWeatherData
                        {
                            Id = reader.GetGuid("Id"),
                            OriginalDateTimeUtc = reader.IsDBNull("OriginalDateTimeUtc") ? null : reader.GetDateTime("OriginalDateTimeUtc"),
                            Temperature = reader.IsDBNull("Temperature") ? null : (double?)reader.GetDouble("Temperature"),
                            RelativeHumidity = reader.IsDBNull("RelativeHumidity") ? null : (double?)reader.GetDouble("RelativeHumidity"),
                            DewPoint = reader.IsDBNull("DewPoint") ? null : (double?)reader.GetDouble("DewPoint"),
                            Precipitation = reader.IsDBNull("Precipitation") ? null : (double?)reader.GetDouble("Precipitation"),
                            CloudCover = reader.IsDBNull("CloudCover") ? null : (double?)reader.GetDouble("CloudCover"),
                            WindSpeed = reader.IsDBNull("WindSpeed") ? null : (double?)reader.GetDouble("WindSpeed"),
                            WindDirection = reader.IsDBNull("WindDirection") ? null : (double?)reader.GetDouble("WindDirection"),
                            GroundTemperature = reader.IsDBNull("GroundTemperature") ? null : (double?)reader.GetDouble("GroundTemperature"),
                            SoilTemperature = reader.IsDBNull("SoilTemperature") ? null : (double?)reader.GetDouble("SoilTemperature"),
                            ConditionIds = reader.IsDBNull("ConditionIds") ? null : (int?)reader.GetInt32("ConditionIds"),
                            SoilHumidity50cm = reader.IsDBNull("SoilHumidity50cm") ? null : (double?)reader.GetDouble("SoilHumidity50cm"),
                            SoilHumidity2m = reader.IsDBNull("SoilHumidity2m") ? null : (double?)reader.GetDouble("SoilHumidity2m"),
                            SolarRadiation = reader.IsDBNull("SolarRadiation") ? null : (double?)reader.GetDouble("SolarRadiation"),
                            EraWeatherStationId = reader.GetGuid("EraWeatherStationId")
                        };
                        scalar.Add(item);
                    }
                }
            }
            var stationGroups = scalar.GroupBy(m => ((Masofa.Common.Models.Era.EraWeatherData)m).EraWeatherStationId);

            foreach (var item in stationGroups)
            {
                IGrouping<Guid, Masofa.Common.Models.Era.EraWeatherData> tempItem = (IGrouping<Guid, Masofa.Common.Models.Era.EraWeatherData>)item;
                //var sqlDelete = $"DELETE FROM \"Era5DayWeatherReports\" edwr \n" +
                //    $"WHERE \n" +
                //        $"edwr.\"WeatherStation\" = '{item.Key}'\n" +
                //        $"AND\n" +
                //        $"edwr.\"Date\" = '{date.ToString("yyyy-MM-dd")}';";
                //EraDbContext.Database.ExecuteSqlRaw(sqlDelete);

                await EraDbContext.Era5DayWeatherReports
                    .Where(m => m.WeatherStation == item.Key)
                    .Where(m => m.Date == date)
                    .ExecuteDeleteAsync();

                var newReport = new Masofa.Common.Models.Era.Era5DayWeatherReport()
                {
                    Fallout = tempItem.Sum(m => m.Precipitation) ?? 0,
                    Date = date,
                    Humidity = tempItem.Average(m => m.RelativeHumidity) ?? 0,
                    TemperatureMax = tempItem.Select(m => m.Temperature).Max() ?? 0,
                    TemperatureMin = tempItem.Select(m => m.Temperature).Min() ?? 0,
                    TemperatureMaxTotal = tempItem.Select(m => m.Temperature).Max() ?? 0,
                    TemperatureMinTotal = tempItem.Select(m => m.Temperature).Min() ?? 0,
                    SolarRadiationInfluence = tempItem.Sum(m => m.SolarRadiation) ?? 0,
                    WeatherStation = tempItem.Key,
                    WindSpeed = tempItem.Average(m => m.WindSpeed) ?? 0,
                    WindDerection = tempItem.Average(m => m.WindDirection) ?? 0
                };

                EraDbContext.Era5DayWeatherReports.Add(newReport);
            }
            EraDbContext.SaveChanges();
        }
        private async Task CalculateWeekReport(DateOnly date)
        {
            var dayReports = EraDbContext.Era5DayWeatherReports
                .Where(x => x.Date >= date)
                .Where(x => x.Date <= date.AddDays(6))
                .ToList();

            var stationDayGroups = dayReports.GroupBy(m => m.WeatherStation);

            foreach (var stationDayGroup in stationDayGroups) 
            {
                //var sqlDelete = $"DELETE FROM \"Era5WeekWeatherReports\" ewwr \n" +
                //    $"WHERE \n" +
                //        $"ewwr.\"WeatherStation\" = '{stationDayGroup.Key}'\n" +
                //        $"AND\n" +
                //        $"ewwr.\"WeekStart\" >= '{date.ToString("yyyy-MM-dd")}'\n" +
                //        $"AND\n" +
                //        $"ewwr.\"WeekEnd\" <= '{date.AddDays(6).ToString("yyyy-MM-dd")}';";

                //EraDbContext.Database.ExecuteSqlRaw(sqlDelete);


                await EraDbContext.Era5WeekWeatherReports
                    .Where(m => m.WeatherStation == stationDayGroup.Key)
                    .Where(m => m.WeekStart >= date)
                    .Where(m => m.WeekEnd <= date.AddDays(6))
                    .ExecuteDeleteAsync();

                var newReport = new Masofa.Common.Models.Era.Era5WeekWeatherReport()
                {
                    WeekStart = date,
                    WeekEnd = date.AddDays(6),
                    Fallout = stationDayGroup.Sum(m => m.Fallout),
                    Humidity = stationDayGroup.Average(m => m.Humidity),
                    TemperatureMax = stationDayGroup.Average(m => m.TemperatureMax),
                    TemperatureMin = stationDayGroup.Average(m => m.TemperatureMin),
                    TemperatureMaxTotal = stationDayGroup.Select(m => m.TemperatureMaxTotal).Max(),
                    TemperatureMinTotal = stationDayGroup.Select(m => m.TemperatureMinTotal).Min(),
                    SolarRadiationInfluence = stationDayGroup.Sum(m => m.SolarRadiationInfluence),
                    WeatherStation = stationDayGroup.Key,
                    WindSpeed = stationDayGroup.Average(m => m.WindSpeed),
                    WindDerection = stationDayGroup.Average(m => m.WindDerection)
                };

                EraDbContext.Era5WeekWeatherReports.Add(newReport);
            }
            EraDbContext.SaveChanges();


        }
        private async Task CalculateMonthReport(DateOnly date)
        {
            var dayReports = EraDbContext.Era5DayWeatherReports
                .Where(x => x.Date >= date)
                .Where(x => x.Date <= date.AddMonths(1).AddDays(-1))
                .ToList();

            var stationDayGroups = dayReports.GroupBy(m => m.WeatherStation);

            foreach (var stationDayGroup in stationDayGroups)
            {
                //var sqlDelete = $"DELETE FROM \"Era5MonthWeatherReports\" emwr \n" +
                //    $"WHERE \n" +
                //        $"emwr.\"WeatherStation\" = '{stationDayGroup.Key}'\n" +
                //        $"AND\n" +
                //        $"emwr.\"Year\" >= {date.Year}\n" +
                //        $"AND\n" +
                //        $"emwr.\"Month\" <= {date.Month};";

                //EraDbContext.Database.ExecuteSqlRaw(sqlDelete);

                await EraDbContext.Era5MonthWeatherReports
                    .Where(m => m.WeatherStation == stationDayGroup.Key)
                    .Where(m => m.Year == date.Year)
                    .Where(m => m.Month == date.Month)
                    .ExecuteDeleteAsync();

                var newReport = new Masofa.Common.Models.Era.Era5MonthWeatherReport()
                {
                    Month = date.Month,
                    Year = date.Year,
                    Fallout = stationDayGroup.Sum(m => m.Fallout),
                    Humidity = stationDayGroup.Average(m => m.Humidity),
                    TemperatureMax = stationDayGroup.Average(m => m.TemperatureMax),
                    TemperatureMin = stationDayGroup.Average(m => m.TemperatureMin),
                    TemperatureMaxTotal = stationDayGroup.Select(m => m.TemperatureMaxTotal).Max(),
                    TemperatureMinTotal = stationDayGroup.Select(m => m.TemperatureMinTotal).Min(),
                    SolarRadiationInfluence = stationDayGroup.Sum(m => m.SolarRadiationInfluence),
                    WeatherStation = stationDayGroup.Key,
                    WindSpeed = stationDayGroup.Average(m => m.WindSpeed),
                    WindDerection = stationDayGroup.Average(m => m.WindDerection)
                };

                EraDbContext.Era5MonthWeatherReports.Add(newReport);
            }
            EraDbContext.SaveChanges();
        }
        private async Task CalculateYearReport(DateOnly date)
        {
            var dayReports = EraDbContext.Era5MonthWeatherReports
                .Where(x => x.Year == date.Year)
                .ToList();

            var stationDayGroups = dayReports.GroupBy(m => m.WeatherStation);

            foreach (var stationDayGroup in stationDayGroups)
            {
                //var sqlDelete = $"DELETE FROM \"Era5YearWeatherReports\" emwr \n" +
                //    $"WHERE \n" +
                //        $"emwr.\"WeatherStation\" = '{stationDayGroup.Key}'\n" +
                //        $"AND\n" +
                //        $"emwr.\"Year\" >= {date.Year};";

                //EraDbContext.Database.ExecuteSqlRaw(sqlDelete);

                await EraDbContext.Era5YearWeatherReports
                    .Where(m => m.WeatherStation == stationDayGroup.Key)
                    .Where(m => m.Year == date.Year)
                    .ExecuteDeleteAsync();

                var newReport = new Masofa.Common.Models.Era.Era5YearWeatherReport()
                {
                    Year = date.Year,
                    Fallout = stationDayGroup.Sum(m => m.Fallout),
                    Humidity = stationDayGroup.Average(m => m.Humidity),
                    TemperatureMax = stationDayGroup.Average(m => m.TemperatureMax),
                    TemperatureMin = stationDayGroup.Average(m => m.TemperatureMin),
                    TemperatureMaxTotal = stationDayGroup.Select(m => m.TemperatureMaxTotal).Max(),
                    TemperatureMinTotal = stationDayGroup.Select(m => m.TemperatureMinTotal).Min(),
                    SolarRadiationInfluence = stationDayGroup.Sum(m => m.SolarRadiationInfluence),
                    WeatherStation = stationDayGroup.Key,
                    WindSpeed = stationDayGroup.Average(m => m.WindSpeed),
                    WindDerection = stationDayGroup.Average(m => m.WindDerection)
                };

                EraDbContext.Era5YearWeatherReports.Add(newReport);
            }
            EraDbContext.SaveChanges();
        }



        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    internal class CountryBoundariesOptions
    {
        internal double LatMin { get; set; }
        internal double LatMax { get; set; }
        internal double LonMin { get; set; }
        internal double LonMax { get; set; }
        internal double Step { get; set; }
    }

    internal class ArchiveDatesOptions
    {
        internal DateOnly ArchiveStartDate { get; set; }
        internal DateOnly ArchiveEndDate { get; set; }
    }

    internal record WeatherPoint(double Latitude, double Longitude);
}
