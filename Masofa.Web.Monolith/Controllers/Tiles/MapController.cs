using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common;
using Masofa.Common.Enums;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Masofa.Web.Monolith.Controllers.Tiles
{
    /// <summary>
    /// Контроллер для отдачи готовых тайлов-подложек.
    /// </summary>
    [Route("tiles/[controller]")]
    [ApiExplorerSettings(GroupName = "Tiles")]
    public class MapController : BaseController
    {
        private HttpClient HttpClient { get; set; }
        private GeoServerOptions GeoServerOptions { get; set; }

        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaSentinelDbContext MasofaSentinelDbContext { get; set; }

        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }

        private const double _earthRadiusOnPi = 20037508.34;

        private static readonly Dictionary<ProductQueueStatusType, (string bucket, string folder)> IndexMapping = new()
        {
            //{ ProductQueueStatusType.NdviTiff, ("sentinelndvi", "ndvi") },
            //{ ProductQueueStatusType.GndviTiff, ("sentinelgndvi", "gndvi") },
            //{ ProductQueueStatusType.MndwiTiff, ("sentinelmndwi", "mndwi") },
            //{ ProductQueueStatusType.NdmiTiff, ("sentinelndmi", "ndmi") },
            //{ ProductQueueStatusType.EviTiff, ("sentinelevi", "evi") },
            //{ ProductQueueStatusType.OrviTiff, ("sentinelorvi", "orvi") },
            //{ ProductQueueStatusType.OsaviTiff, ("sentinelosavi", "osavi") }
        };

        private static readonly Dictionary<ProductQueueStatusType, string> IndexNameMapping = new()
        {
            //{ ProductQueueStatusType.NdviTiff, "NDVI" },
            //{ ProductQueueStatusType.GndviTiff, "GNDVI" },
            //{ ProductQueueStatusType.MndwiTiff, "MNDWI" },
            //{ ProductQueueStatusType.NdmiTiff, "NDMI" },
            //{ ProductQueueStatusType.EviTiff, "EVI" },
            //{ ProductQueueStatusType.OrviTiff, "ORVI" },
            //{ ProductQueueStatusType.OsaviTiff, "OSAVI" }
        };

        public MapController(
            ILogger<MapController> logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor,
            MasofaCommonDbContext masofaCommonDbContext,
            MasofaSentinelDbContext masofaSentinelDbContext)
            : base(logger, configuration, mediator)
        {
            HttpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GeoServer-Tile-Client/1.0");
            GeoServerOptions = configuration.GetSection("GeoServerOptions").Get<GeoServerOptions>();
            BusinessLogicLogger = businessLogicLogger;
            HttpContextAccessor = httpContextAccessor;
            MasofaCommonDbContext = masofaCommonDbContext;
            MasofaSentinelDbContext = masofaSentinelDbContext;
        }

        // Оставил пока, на всякий случай
        #region SkiaSharp
        /*
        /// <summary>
        /// Отрисовать тайл-подложку с помощью SkiaSharp без фильтрации по дате.
        /// </summary>
        [HttpGet("skia/{zoom:int}/{x:int}/{y:int}.png")]
        public async Task<byte[]> DrawTileWithSkia(int zoom, int x, int y) =>
            await DrawTileWithSkia(zoom, x, y, null);

        /// <summary>
        /// Отрисовать тайл-подложку с помощью SkiaSharp c фильтрацией по дате.
        /// </summary>
        [HttpGet("skia/{zoom:int}/{x:int}/{y:int}/{date:DateOnly}.png")]
        public async Task<byte[]> DrawTileWithSkia(int zoom, int x, int y, DateOnly date) =>
            await DrawTileWithSkia(zoom, x, y, (DateTime?)date.ToDateTime(TimeOnly.MinValue));

        private async Task<byte[]> DrawTileWithSkia(int zoom, int x, int y, DateTime? date)
        {
            // ищем тайл
            var query = _tileDb.Tiles.AsNoTracking().Where(t => t.Zoom == zoom && t.X == x && t.Y == y);
            if (date.HasValue)
            {
                query = query.Where(t => t.TileSnapShotDate.Date == date.Value.Date);
            }

            var tile = await query.FirstOrDefaultAsync();
            if (tile != null)
            {
                // если есть готовый файл — отдаём как есть
                var fileStorageItem = await _commonDb.FileStorageItems.FindAsync(tile.FileId);
                if (fileStorageItem != null)
                {
                    return await _fileStorageProvider.GetFileBytesAsync(fileStorageItem);
                }
            }

            // рисуем пустой/заглушку
            using var surface = SKSurface.Create(new SKImageInfo(256, 256, SKColorType.Rgba8888, SKAlphaType.Premul));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.DarkGray);

            // примитивная сетка
            using var grid = new SKPaint { Color = SKColors.LightGray, StrokeWidth = 1 };
            for (int i = 0; i < 256; i += 32)
            {
                canvas.DrawLine(i, 0, i, 256, grid);
                canvas.DrawLine(0, i, 256, i, grid);
            }

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }
        */
        #endregion

        #region GeoServer
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
        public async Task<IActionResult> GetTile(
            [FromQuery] string lat,
            [FromQuery] string lon,
            [FromQuery] int z,
            [FromQuery] string layer = "mosaics",
            [Required] CoordinateSystemEnum coordinateSystem = CoordinateSystemEnum.Epsg4326Wgs84)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetTile)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
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
                              $"{GeoServerOptions.Workspace}:{layer}/raster/EPSG:900913/EPSG:900913:{z}/{tileY}/{tileX}?format=image/png";

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

        /// <summary>
        /// Возвращает список доступных слоёв (тайловых наборов) для отображения на карте.
        /// </summary>
        /// <returns>Список слоёв с типом и датой</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAvailableLayers()
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetAvailableLayers)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var processedQueues = await MasofaSentinelDbContext.Set<Sentinel2ProductQueue>()
                    .Where(q => q.QueueStatus == ProductQueueStatusType.GeoserverImported)
                    .Select(q => new { q.ProductId, q.QueueStatus })
                    .ToListAsync();

                if (!processedQueues.Any())
                {
                    return Ok(new List<LayerInfo>());
                }

                var productIds = processedQueues.Select(q => q.ProductId).ToList();
                var products = await MasofaCommonDbContext.SatelliteProducts
                    .Where(p => productIds.Contains(p.ProductId))
                    .ToDictionaryAsync(p => p.Id);

                if (products == null)
                {
                    var errorMsg = LogMessageResource.ProductsNotFoundOnServer();
                    await BusinessLogicLogger.LogInformationAsync(errorMsg, requestPath);
                    return NotFound(errorMsg);
                }

                var layers = new HashSet<string>();
                var layerInfos = new List<LayerInfo>();

                foreach (var queue in processedQueues)
                {
                    if (!Guid.TryParse(queue.ProductId, out Guid productIdGuid) ||
                        !products.TryGetValue(productIdGuid, out var product))
                    {
                        continue;
                    }

                    var date = product.CreateAt.Date;

                    string layerType;

                    if (!IndexMapping.Keys.Contains(queue.QueueStatus))
                    {
                        layerType = "Original";
                    }
                    else if (IndexNameMapping.TryGetValue(queue.QueueStatus, out var indexName))
                    {
                        layerType = indexName;
                    }
                    else
                    {
                        continue;
                    }

                    var layerName = $"{product.ProductSourceType}_{layerType}_{date:MM}_{date:dd}";
                    if (layers.Add(layerName))
                    {
                        layerInfos.Add(new LayerInfo
                        {
                            layerName = layerName,
                            type = layerType,
                            date = date.ToString("yyyy-MM-dd")
                        });
                    }
                }

                return Ok(layerInfos.OrderBy(l => l.date).ThenBy(l => l.type));
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        public class LayerInfo
        {
            public string layerName { get; set; }
            public string type { get; set; }
            public string date { get; set; }
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
        #endregion
    }
}