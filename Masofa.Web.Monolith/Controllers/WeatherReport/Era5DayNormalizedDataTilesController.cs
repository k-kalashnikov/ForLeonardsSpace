using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common;
using Masofa.Common.Attributes;
using Masofa.Common.Enums;
using Masofa.Common.Models.Era;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Masofa.Web.Monolith.Controllers.WeatherReport
{
    [Route("weatherReports/[controller]")]
    [ApiExplorerSettings(GroupName = "WeatherReports")]
    public class Era5DayNormalizedDataTilesController : BaseController
    {
        private HttpClient HttpClient { get; set; }
        private GeoServerOptions GeoServerOptions { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private MasofaEraDbContext WeatherReportDbContext { get; set; }
        private const double _earthRadiusOnPi = 20037508.34;
        public Era5DayNormalizedDataTilesController(
            ILogger<Era5DayNormalizedDataTilesController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            MasofaEraDbContext weatherReportDbContext) : base(logger, configuration, mediator)
        {
            HttpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GeoServer-Tile-Client/1.0");
            GeoServerOptions = configuration.GetSection("GeoServerOptions").Get<GeoServerOptions>();
            BusinessLogicLogger = businessLogicLogger;
            WeatherReportDbContext = weatherReportDbContext;
        }

        /// <summary>
        /// Возвращает список доступных слоёв (тайловых наборов) для отображения на карте.
        /// </summary>
        /// <returns>Список слоёв</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<List<string>> GetAvailableLayers()
        {
            List<string> result = [];
            var requestPath = $"{GetType().FullName}=>{nameof(GetAvailableLayers)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                result = typeof(BaseEra5WeatherReport)
                   .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   .Where(p => p.GetCustomAttribute<ReportValueAttribute>() != null)
                   .Select(p => p.Name)
                   .ToList();

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath, string.Join(",", result)), requestPath);
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
            }

            return result;
        }

        /// <summary>
        /// Получает тайл карты по координатам и уровню масштабирования
        /// </summary>
        /// <param name="lat">Широта или Y координата</param>
        /// <param name="lon">Долгота или X координата</param>
        /// <param name="z">Уровень масштабирования (zoom level)</param>
        /// <param name="coordinateSystem">Система координат (по умолчанию EPSG:4326)</param>
        /// <returns>PNG изображение тайла карты</returns>
        /// <response code="200">Тайл успешно получен</response>
        /// <response code="404">Тайл не найден на сервере</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetWeatherReportTile(
            [FromQuery] string lat,
            [FromQuery] string lon,
            [FromQuery] int z,
            [FromQuery] string layer,
            [FromQuery] DateOnly reportDate,
            [Required] CoordinateSystemEnum coordinateSystem = CoordinateSystemEnum.Epsg4326Wgs84)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetWeatherReportTile)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                if (!(await GetAvailableLayers()).Contains(layer))
                {
                    var errorMsg = LogMessageResource.TileOrLayerNotFound();
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var layerName = $"Normalized{layer}_{reportDate:MMdd}";

                var tileX = double.Parse(lon);
                var tileY = double.Parse(lat);

                if (coordinateSystem == CoordinateSystemEnum.Epsg4326Wgs84)
                {
                    // Конвертация: EPSG:4326 → EPSG:900913 (в метрах)
                    var (x, y) = LonLatToWebMercator(double.Parse(lon), double.Parse(lat));

                    // Конвертация: Web Mercator → Tile XYZ
                    (tileX, tileY) = WebMercatorToTile(x, y, z);
                }

                var tileUrl = $"{GeoServerOptions.GeoServerUrl}/geoserver/gwc/service/wmts/rest/" +
                              $"{GeoServerOptions.Workspace}:{layerName}/raster/EPSG:900913/EPSG:900913:{z}/{tileY}/{tileX}?format=image/png";

                // Получаем тайл
                var response = await HttpClient.GetAsync(tileUrl);
                if (response.IsSuccessStatusCode)
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    var contentType = "image/png";
                    return File(imageBytes, contentType);
                }

                tileUrl = $"{GeoServerOptions.GeoServerUrl}/geoserver/gwc/service/wmts/rest/" +
                              $"{GeoServerOptions.Workspace}:Era{layerName}/raster/EPSG:900913/EPSG:900913:{z}/{tileY}/{tileX}?format=image/png";

                response = await HttpClient.GetAsync(tileUrl);
                if (response.IsSuccessStatusCode)
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    var contentType = "image/png";
                    return File(imageBytes, contentType);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var errorMsg = $"Tile not found on server";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }
                else
                {
                    var errorMsg = "Error getting tile";
                    await BusinessLogicLogger.LogErrorAsync(errorMsg, requestPath);
                    return StatusCode((int)response.StatusCode, errorMsg);
                }
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // EPSG:4326 → EPSG:900913 (в метрах)
        private (double x, double y) LonLatToWebMercator(double lon, double lat)
        {
            double x = lon * _earthRadiusOnPi / 180;
            double y = Math.Log(Math.Tan((90 + lat) * Math.PI / 360)) / (Math.PI / 180);
            y = y * _earthRadiusOnPi / 180;
            return (x, y);
        }

        // Web Mercator (метры) → Tile XYZ
        private (int tileX, int tileY) WebMercatorToTile(double x, double y, int z)
        {
            double tileCount = Math.Pow(2, z);
            double tileSize = _earthRadiusOnPi * 2 / tileCount;

            int tileX = (int)((x + _earthRadiusOnPi) / tileSize);
            int tileY = (int)((_earthRadiusOnPi - y) / tileSize); // XYZ: y сверху вниз

            return (tileX, tileY);
        }
    }
}
