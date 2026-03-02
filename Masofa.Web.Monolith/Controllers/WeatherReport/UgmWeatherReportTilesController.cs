using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common;
using Masofa.Common.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Masofa.Web.Monolith.Controllers.WeatherReport
{
    [Route("weatherReports/[controller]")]
    [ApiExplorerSettings(GroupName = "WeatherReports")]
    public class UgmWeatherReportTilesController : BaseController
    {
        private HttpClient HttpClient { get; set; }
        private GeoServerOptions GeoServerOptions { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private const double _earthRadiusOnPi = 20037508.34;


        public UgmWeatherReportTilesController(ILogger<UgmWeatherReportTilesController> logger, IConfiguration configuration, IMediator mediator, IBusinessLogicLogger businessLogicLogger) : base(logger, configuration, mediator)
        {
            GeoServerOptions = configuration.GetSection("GeoServerOptions").Get<GeoServerOptions>();
            BusinessLogicLogger = businessLogicLogger;
            HttpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GeoServer-Tile-Client/1.0");
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
            [FromQuery] DateOnly reportDate,
            [Required] CoordinateSystemEnum coordinateSystem = CoordinateSystemEnum.Epsg4326Wgs84)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetWeatherReportTile)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var layer = "UgmTemperatureAverage";
                var layerName = $"{layer}_{reportDate:yyyyMMdd}";

                var tileX = double.Parse(lon);
                var tileY = double.Parse(lat);

                if (coordinateSystem == CoordinateSystemEnum.Epsg4326Wgs84)
                {
                    // Конвертация: EPSG:4326 → EPSG:900913 (в метрах)
                    var (x, y) = LonLatToWebMercator(double.Parse(lon), double.Parse(lat));

                    // Конвертация: Web Mercator → Tile XYZ
                    (tileX, tileY) = WebMercatorToTile(x, y, z);
                }

                // Формируем URL к GeoServer GWC
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
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var errorMsg = LogMessageResource.TileOrLayerNotFound();
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
