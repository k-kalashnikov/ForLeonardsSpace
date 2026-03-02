using CsvHelper;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.BusinessLogic.WeatherReport;
using Masofa.Common.Attributes;
using Masofa.Common.Models.Era;
using Masofa.Common.Models.IBMWeather;
using Masofa.Common.Models.Ugm;
using Masofa.Common.Resources;
using Masofa.DataAccess;
using Masofa.Web.Monolith.ViewModels.WeatherReport;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace Masofa.Web.Monolith.Controllers.WeatherReport
{
    [Route("weatherReports/[controller]")]
    [ApiExplorerSettings(GroupName = "WeatherReports")]
    public class WeatherReportExportController : BaseController
    {
        private MasofaEraDbContext EraDbContext { get; set; }
        private MasofaIBMWeatherDbContext IbmDbContext { get; set; }
        private MasofaUgmDbContext UgmDbContext { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; }

        public WeatherReportExportController(
            ILogger<WeatherReportExportController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            MasofaEraDbContext eraDbContext,
            MasofaIBMWeatherDbContext ibmWeatherDbContext,
            MasofaUgmDbContext ugmDbContext) : base(logger, configuration, mediator)
        {
            BusinessLogicLogger = businessLogicLogger;
            EraDbContext = eraDbContext;
            IbmDbContext = ibmWeatherDbContext;
            UgmDbContext = ugmDbContext;
        }

        /// <summary>
        /// Экспорт погодной информации по точке в KML, GeoJson, ESRI Shapefile форматах в ZIP архиве
        /// </summary>
        /// <param name="viewModel">Параметры экспорта точки</param>
        /// <returns>ZIP файл с данными слоев</returns>
        /// <response code="200">ZIP файл с экспортированными слоями</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ExportPoint([FromBody] PointExportViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ExportPoint)}";
            try
            {
                byte[] zip = [];

                switch (viewModel.WeatherDataSource)
                {
                    case WeatherDataSource.Era5:
                        zip = await ExportEraPoint(viewModel);
                        break;
                    case WeatherDataSource.IBM:
                        zip = await ExportIbmPoint(viewModel);
                        break;
                    case WeatherDataSource.UGM:
                        zip = await ExportUgmPoint(viewModel);
                        break;
                }

                if (zip.Length == 0)
                {
                    return NotFound();
                }

                var fileName = $"{GetSource(viewModel.WeatherDataSource)}_{viewModel.Latitude}_{viewModel.Longitude}_{viewModel.Date:yyyyMMdd}";
                return File(zip, "application/zip", fileName);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Экспорт погодного слоя в KML, GeoJson, ESRI Shapefile форматах в ZIP архиве
        /// </summary>
        /// <param name="viewModel">Параметры экспорта слоев</param>
        /// <returns>ZIP файл с данными слоев</returns>
        /// <response code="200">ZIP файл с экспортированными слоями</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ExportLayer([FromBody] LayerExportViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ExportLayer)}";
            try
            {
                byte[] zip = [];
                switch (viewModel.WeatherDataSource)
                {
                    case WeatherDataSource.Era5:
                        zip = await ExportEra(viewModel);
                        break;
                    case WeatherDataSource.IBM:
                        zip = await ExportIbm(viewModel);
                        break;
                    case WeatherDataSource.UGM:
                        zip = await ExportUgm(viewModel);
                        break;
                }
                var fileName = $"{GetFileName(viewModel)}.zip";
                return File(zip, "application/zip", fileName);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private async Task<byte[]> ExportEra(LayerExportViewModel viewModel)
        {
            List<string> availableLayers = typeof(BaseEra5WeatherReport)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<ReportValueAttribute>() != null)
                .Select(p => p.Name)
                .ToList();

            if (!availableLayers.Contains(viewModel.LayerName))
            {
                NotFound("Layer not found on server");
            }

            IEnumerable<BaseEra5WeatherReport>? reports;
            reports = await EraDbContext.Era5DayWeatherReports
                .Where(r => r.Date == viewModel.Date)
                .ToListAsync();

            if (!reports.Any())
            {
                reports = await EraDbContext.Era5DayWeatherForecasts
                    .Where(r => r.Date == viewModel.Date)
                    .ToListAsync();
            }

            if (!reports.Any())
            {
                NotFound("Found no data on server");
            }

            var stations = EraDbContext.EraWeatherStations
                .Where(s => reports.Select(r => r.WeatherStation).ToList().Contains(s.Id))
                .ToDictionary(s => s.Id, s => s);

            var valueProperties = typeof(BaseEra5WeatherReport)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.GetCustomAttribute<ReportValueAttribute>() != null)
                        .ToList();

            var prop = valueProperties.FirstOrDefault(p => p.Name == viewModel.LayerName);
            if (prop == null)
            {
                NotFound("Layer not found on server");
            }

            var points = new List<(double lon, double lat, double value)>();
            foreach (var report in reports)
            {
                if (stations.TryGetValue(report.WeatherStation.Value, out var station))
                {
                    double lon = station.Point.X;
                    double lat = station.Point.Y;
                    double val = (double)prop.GetValue(report)!;
                    points.Add((lon, lat, val));
                }
            }

            if (viewModel.ExportFileType == ExportFileType.CSV)
            {
                return ExportCsv(points, $"{GetFileName(viewModel)}.csv");
            }

            var result = await Mediator.Send(new WeatherReportVectorExportCommand()
            {
                Points = points,
                FileName = GetFileName(viewModel),
                Ext = [GetExt(viewModel.ExportFileType)],
                Driver = GetDriver(viewModel.ExportFileType),
                FieldName = viewModel.LayerName[..4].ToLower()
            });

            return result;
        }

        private async Task<byte[]> ExportIbm(LayerExportViewModel viewModel)
        {
            List<string> availableLayers = typeof(BaseIbmWeatherReport)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<ReportValueAttribute>() != null)
                .Select(p => p.Name)
                .ToList();

            if (!availableLayers.Contains(viewModel.LayerName))
            {
                NotFound("Layer not found on server");
            }

            var reports = await IbmDbContext.IbmDayWeatherForecasts
                .Where(r => r.Date == viewModel.Date)
                .ToListAsync();

            if (reports.Count == 0)
            {
                NotFound("Found no data on server");
            }

            var stations = IbmDbContext.IBMMeteoStations
                .Where(s => reports.Select(r => r.WeatherStation).ToList().Contains(s.Id))
                .ToDictionary(s => s.Id, s => s);

            var valueProperties = typeof(BaseIbmWeatherReport)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.GetCustomAttribute<ReportValueAttribute>() != null)
                        .ToList();

            var prop = valueProperties.FirstOrDefault(p => p.Name == viewModel.LayerName);
            if (prop == null)
            {
                NotFound("Layer not found on server");
            }

            var points = new List<(double lon, double lat, double value)>();
            foreach (var report in reports)
            {
                if (stations.TryGetValue(report.WeatherStation.Value, out var station))
                {
                    double lon = station.Point.X;
                    double lat = station.Point.Y;
                    double val = (double)prop.GetValue(report)!;
                    points.Add((lon, lat, val));
                }
            }

            if (viewModel.ExportFileType == ExportFileType.CSV)
            {
                return ExportCsv(points, $"{GetFileName(viewModel)}.csv");
            }

            var result = await Mediator.Send(new WeatherReportVectorExportCommand()
            {
                Points = points,
                FileName = GetFileName(viewModel),
                Ext = [GetExt(viewModel.ExportFileType)],
                Driver = GetDriver(viewModel.ExportFileType),
                FieldName = viewModel.LayerName[..4].ToLower()
            });

            return result;
        }

        private async Task<byte[]> ExportUgm(LayerExportViewModel viewModel)
        {
            List<string> availableLayers = typeof(BaseIbmWeatherReport)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<ReportValueAttribute>() != null)
                .Select(p => p.Name)
                .ToList();

            if (viewModel.LayerName != "TemperatureAverage")
            {
                NotFound("Layer not found on server");
            }

            var data = await UgmDbContext.UgmWeatherData
                .Where(r => r.Date == viewModel.Date && r.DayPart == Masofa.Common.Models.Ugm.DayPart.Day)
                .ToListAsync();

            if (!data.Any())
            {
                NotFound("Found no data on server");
            }

            var stations = UgmDbContext.UgmWeatherStations
                .Where(s => data.Select(r => r.RegionId).ToList().Contains(s.UgmRegionId))
                .ToDictionary(s => s.UgmRegionId, s => s);

            var points = new List<(double lon, double lat, double value)>();
            foreach (var report in data)
            {
                if (stations.TryGetValue(report.RegionId, out var station))
                {
                    double lon = station.Longitude.Value;
                    double lat = station.Latitude.Value;
                    double val = report.AirTAverage.Value;
                    points.Add((lon, lat, val));
                }
            }

            if (viewModel.ExportFileType == ExportFileType.CSV)
            {
                return ExportCsv(points, $"{GetFileName(viewModel)}.csv");
            }

            var result = await Mediator.Send(new WeatherReportVectorExportCommand()
            {
                Points = points,
                FileName = GetFileName(viewModel),
                Ext = [GetExt(viewModel.ExportFileType)],
                Driver = GetDriver(viewModel.ExportFileType),
                FieldName = viewModel.LayerName[..4].ToLower()
            });

            return result;
        }

        private async Task<byte[]> ExportEraPoint(PointExportViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ExportEraPoint)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(viewModel)), requestPath);
                    return [];
                }

                var targetPoint = new NetTopologySuite.Geometries.Point(viewModel.Longitude, viewModel.Latitude) { SRID = 4326 };

                var closestStation = await EraDbContext.EraWeatherStations
                    .OrderBy(s => s.Point.Distance(targetPoint))
                    .FirstOrDefaultAsync();

                if (closestStation is null)
                {
                    return [];
                }

                var closestFutureReport = await EraDbContext.Era5DayWeatherForecasts
                    .Where(x => x.WeatherStation == closestStation.Id && x.Date >= viewModel.Date)
                    .OrderBy(x => x.Date)
                    .FirstOrDefaultAsync();
                if (closestFutureReport is null)
                {
                    return [];
                }

                var valueProperties = typeof(BaseEra5WeatherReport)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.GetCustomAttribute<ReportValueAttribute>() != null)
                        .ToList();

                List<(string fieldName, double value)> values = [];
                foreach (var prop in valueProperties)
                {
                    values.Add((prop.Name, (double)prop.GetValue(closestFutureReport)!));
                }

                var fileName = $"{GetSource(viewModel.WeatherDataSource)}_{viewModel.Latitude}_{viewModel.Longitude}_{viewModel.Date:yyyyMMdd}";

                if (viewModel.ExportFileType == ExportFileType.CSV)
                {
                    return ExportCsvPoint(targetPoint, values, $"{fileName}.csv");
                }

                var result = await Mediator.Send(new WeatherReportPointVectorExportCommand()
                {
                    Point = targetPoint,
                    Values = values,
                    FileName = fileName,
                    Ext = [GetExt(viewModel.ExportFileType)],
                    Driver = GetDriver(viewModel.ExportFileType)
                });

                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return [];
            }
        }

        private async Task<byte[]> ExportIbmPoint(PointExportViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ExportIbmPoint)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(viewModel)), requestPath);
                    return [];
                }

                var targetPoint = new NetTopologySuite.Geometries.Point(viewModel.Longitude, viewModel.Latitude) { SRID = 4326 };

                var closestStation = await IbmDbContext.IBMMeteoStations
                    .OrderBy(s => s.Point.Distance(targetPoint))
                    .FirstOrDefaultAsync();

                if (closestStation is null)
                {
                    return [];
                }

                var closestFutureReport = await IbmDbContext.IbmDayWeatherForecasts
                    .Where(x => x.WeatherStation == closestStation.Id && x.Date >= viewModel.Date)
                    .OrderBy(x => x.Date)
                    .FirstOrDefaultAsync();
                if (closestFutureReport is null)
                {
                    return [];
                }

                var valueProperties = typeof(BaseIbmWeatherReport)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.GetCustomAttribute<ReportValueAttribute>() != null)
                        .ToList();

                List<(string fieldName, double value)> values = [];
                foreach (var prop in valueProperties)
                {
                    values.Add((prop.Name, (double)prop.GetValue(closestFutureReport)!));
                }

                var fileName = $"{GetSource(viewModel.WeatherDataSource)}_{viewModel.Latitude}_{viewModel.Longitude}_{viewModel.Date:yyyyMMdd}";

                if (viewModel.ExportFileType == ExportFileType.CSV)
                {
                    return ExportCsvPoint(targetPoint, values, $"{fileName}.csv");
                }

                var result = await Mediator.Send(new WeatherReportPointVectorExportCommand()
                {
                    Point = targetPoint,
                    Values = values,
                    FileName = fileName,
                    Ext = [GetExt(viewModel.ExportFileType)],
                    Driver = GetDriver(viewModel.ExportFileType)
                });

                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return [];
            }
        }

        private async Task<byte[]> ExportUgmPoint(PointExportViewModel viewModel)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(ExportUgmPoint)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                if (!ModelState.IsValid)
                {
                    await BusinessLogicLogger.LogErrorAsync(LogMessageResource.ModelValidationFailed(requestPath, Newtonsoft.Json.JsonConvert.SerializeObject(viewModel)), requestPath);
                    return [];
                }

                var targetPoint = new NetTopologySuite.Geometries.Point(viewModel.Longitude, viewModel.Latitude) { SRID = 4326 };

                var sql = @"
                    SELECT *
                    FROM ""UgmWeatherStations""
                    WHERE ""Longitude"" IS NOT NULL 
                    AND ""Latitude"" IS NOT NULL
                    AND ST_DWithin(
                        ST_SetSRID(ST_MakePoint(""Longitude"", ""Latitude""), 4326)::geography,
                        ST_SetSRID(ST_MakePoint(@lon, @lat), 4326)::geography,
                        @maxDistanceMeters
                    )";

                var nearbyStations = await UgmDbContext.UgmWeatherStations
                    .FromSqlRaw(sql,
                        new NpgsqlParameter("@lon", viewModel.Longitude),
                        new NpgsqlParameter("@lat", viewModel.Latitude),
                        new NpgsqlParameter("@maxDistanceMeters", 10_000))
                    .Select(s => s.UgmRegionId)
                    .ToListAsync();

                if (nearbyStations.Count == 0)
                {
                    var errorMsg = LogMessageResource.NoWeatherStationsNearby();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return [];
                }

                var report = await UgmDbContext.UgmWeatherData
                    .Where(d => nearbyStations.Contains(d.RegionId) && d.Date >= viewModel.Date && d.DayPart == DayPart.Day)
                    .OrderBy(d => d.Date)
                    .FirstOrDefaultAsync();

                if (report == null)
                {
                    var errorMsg = $"There is no weather data";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return [];
                }

                List<(string fieldName, double value)> values = [];

                values.Add(("TemperatureAverage", report.AirTAverage.Value));

                var fileName = $"{GetSource(viewModel.WeatherDataSource)}_{viewModel.Latitude}_{viewModel.Longitude}_{viewModel.Date:yyyyMMdd}";

                if (viewModel.ExportFileType == ExportFileType.CSV)
                {
                    return ExportCsvPoint(targetPoint, values, $"{fileName}.csv");
                }

                var result = await Mediator.Send(new WeatherReportPointVectorExportCommand()
                {
                    Point = targetPoint,
                    Values = values,
                    FileName = fileName,
                    Ext = [GetExt(viewModel.ExportFileType)],
                    Driver = GetDriver(viewModel.ExportFileType)
                });

                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return [];
            }
        }

        private byte[] ExportCsv(List<(double lon, double lat, double value)> data, string csvFileName)
        {
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: false))
            {
                var entry = archive.CreateEntry(csvFileName, CompressionLevel.Optimal);

                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream, Encoding.UTF8);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteField("Longitude");
                csv.WriteField("Latitude");
                csv.WriteField("Value");
                csv.NextRecord();

                foreach (var (lon, lat, value) in data)
                {
                    csv.WriteField(lon);
                    csv.WriteField(lat);
                    csv.WriteField(value);
                    csv.NextRecord();
                }

                writer.Flush();
            }

            return memoryStream.ToArray();
        }

        private byte[] ExportCsvPoint(NetTopologySuite.Geometries.Point point, List<(string fieldName, double value)> data, string csvFileName)
        {
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: false))
            {
                var entry = archive.CreateEntry(csvFileName, CompressionLevel.Optimal);

                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream, Encoding.UTF8);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteField("Longitude");
                csv.WriteField("Latitude");
                //csv.WriteField("Value");
                foreach (var (fieldName, value) in data)
                {
                    csv.WriteField(fieldName);
                }
                csv.NextRecord();

                csv.WriteField(point.X);
                csv.WriteField(point.Y);

                foreach (var (fieldName, value) in data)
                {
                    csv.WriteField(value);
                }
                csv.NextRecord();

                writer.Flush();
            }

            return memoryStream.ToArray();
        }

        private string GetFileName(LayerExportViewModel viewModel) => $"{GetSource(viewModel.WeatherDataSource)}{viewModel.LayerName}_{viewModel.Date:yyyyMMdd}";

        private string GetExt(ExportFileType eft) =>
            eft switch
            {
                ExportFileType.Shapefile => ".shp",
                ExportFileType.KML => ".kml",
                ExportFileType.GeoJSON => ".geojson",
                _ => string.Empty,
            };

        private string GetDriver(ExportFileType eft) =>
            eft switch
            {
                ExportFileType.Shapefile => "ESRI Shapefile",
                ExportFileType.KML => "KML",
                ExportFileType.GeoJSON => "GeoJSON",
                _ => string.Empty,
            };

        private string GetSource(WeatherDataSource wds) =>
            wds switch
            {
                WeatherDataSource.Era5 => "Era",
                WeatherDataSource.IBM => "Ibm",
                WeatherDataSource.UGM => "Ugm",
                _ => string.Empty,
            };
    }
}
