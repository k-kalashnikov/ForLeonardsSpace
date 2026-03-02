using Masofa.BusinessLogic.Attributes;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Sentinel;
using Masofa.DataAccess;
using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Masofa.BusinessLogic.FieldSatellite.Requests;


namespace Masofa.BusinessLogic.CropMonitoring.Fields
{
    /// <summary>
    /// Запрос для получения Sentinel продуктов по полю
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetFieldSatelliteSentinelProductsRequest : IRequest<FieldSatelliteSentinelProductsResponse>
    {
        /// <summary>
        /// Идентификатор поля
        /// </summary>
        public Guid FieldId { get; set; }
    }

    /// <summary>
    /// Ответ с Sentinel продуктами для поля
    /// </summary>
    public class FieldSatelliteSentinelProductsResponse
    {
        /// <summary>
        /// Список продуктов Sentinel2
        /// </summary>
        public List<Sentinel2ProductEntity> SentinelProducts { get; set; } = new();
    }

    /// <summary>
    /// Обработчик запроса для получения Sentinel продуктов по полю
    /// </summary>
    public class GetFieldSatelliteSentinelProductsRequestHandler : IRequestHandler<GetFieldSatelliteSentinelProductsRequest, FieldSatelliteSentinelProductsResponse>
    {
        private readonly MasofaCropMonitoringDbContext _cropMonitoringContext;
        private readonly MasofaSentinelDbContext _sentinelDbContext;
        private readonly IMediator _mediator;
        private readonly ILogger<GetFieldSatelliteSentinelProductsRequestHandler> _logger;

        public GetFieldSatelliteSentinelProductsRequestHandler(
            MasofaCropMonitoringDbContext cropMonitoringContext,
            MasofaSentinelDbContext sentinelDbContext,
            IMediator mediator,
            ILogger<GetFieldSatelliteSentinelProductsRequestHandler> logger)
        {
            _cropMonitoringContext = cropMonitoringContext;
            _sentinelDbContext = sentinelDbContext;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<FieldSatelliteSentinelProductsResponse> Handle(GetFieldSatelliteSentinelProductsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting Sentinel products for field {FieldId}", request.FieldId);

            var response = new FieldSatelliteSentinelProductsResponse();

            // Сначала пытаемся получить из маппинга
            try
            {
                var mappings = await _mediator.Send(new GetProductsForFieldRequest(
                    request.FieldId,
                    ProductSourceType.Sentinel2,
                    cancellationToken), cancellationToken);

                if (mappings.Any())
                {
                    // Получаем продукты по ID из маппинга
                    var productIds = mappings.Select(m => m.ProductId).ToList();
                    var products = await _sentinelDbContext.Sentinel2Products
                        .Where(p => productIds.Contains(p.SatellateProductId))
                        .ToListAsync(cancellationToken);

                    foreach (var product in products)
                    {
                        var inspire = await _sentinelDbContext.SentinelInspireMetadata.FirstOrDefaultAsync(x => x.Id == product.SentinelInspireMetadataId);
                        if(inspire != null)
                        {
                            product.OriginDate = inspire.DateStamp;
                        }
                    }

                    response.SentinelProducts = products;
                    _logger.LogInformation("Found {SentinelCount} Sentinel products for field {FieldId} from mapping",
                        response.SentinelProducts.Count, request.FieldId);
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting products from mapping for field {FieldId}, falling back to geometry calculation", request.FieldId);
            }

            // Fallback к старому методу
            var field = await _cropMonitoringContext.Fields
                .Where(f => f.Id == request.FieldId && f.Status == Masofa.Common.Models.StatusType.Active)
                .FirstOrDefaultAsync(cancellationToken);

            if (field?.Polygon == null)
            {
                _logger.LogWarning("Field {FieldId} not found or has no polygon", request.FieldId);
                return response;
            }

            try
            {
                // Оптимизированный метод получения продуктов Sentinel2 с использованием PostgreSQL функций геометрии
                try
                {
                    // Оптимизированный запрос с использованием raw SQL и PostgreSQL функций геометрии
                    var intersectingProducts = await _sentinelDbContext.Sentinel2Products
                        .FromSqlRaw(@"
                            SELECT DISTINCT s2p.* FROM ""Sentinel2Products"" s2p
                            INNER JOIN ""SentinelInspireMetadata"" sim ON s2p.""SentinelInspireMetadataId"" = sim.""Id""
                            WHERE ST_Intersects(
                                ST_MakeEnvelope(
                                    sim.""WestBoundLongitude"", 
                                    sim.""SouthBoundLatitude"", 
                                    sim.""EastBoundLongitude"", 
                                    sim.""NorthBoundLatitude"", 
                                    4326
                                ),
                                ST_GeomFromText({0}, 4326)
                            )", field.Polygon.AsText())
                        .ToListAsync(cancellationToken);

                    response.SentinelProducts = intersectingProducts;

                    foreach (var product in intersectingProducts)
                    {
                        var inspire = await _sentinelDbContext.SentinelInspireMetadata.FirstOrDefaultAsync(x => x.Id == product.SentinelInspireMetadataId);
                        if (inspire != null)
                        {
                            product.OriginDate = inspire.DateStamp;
                        }
                    }
                    _logger.LogInformation("Found {SentinelCount} Sentinel products for field {FieldId} using geometry calculation",
                        response.SentinelProducts.Count, request.FieldId);
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting Sentinel products with intersection");

                    // Fallback к предыдущему методу если raw SQL не работает
                    // Получаем bounding box поля для быстрой предварительной фильтрации
                    var fieldBoundingBox = field.Polygon.EnvelopeInternal;

                    // Оптимизированный запрос с использованием JOIN и геометрических функций PostgreSQL
                    var intersectingProducts = await _sentinelDbContext.Sentinel2Products
                        .Join(_sentinelDbContext.SentinelInspireMetadata,
                            product => product.SentinelInspireMetadataId,
                            inspire => inspire.Id,
                            (product, inspire) => new { product, inspire })
                        .Where(joined =>
                            // Предварительная фильтрация по bounding box
                            (double)joined.inspire.WestBoundLongitude <= fieldBoundingBox.MaxX &&
                            (double)joined.inspire.EastBoundLongitude >= fieldBoundingBox.MinX &&
                            (double)joined.inspire.SouthBoundLatitude <= fieldBoundingBox.MaxY &&
                            (double)joined.inspire.NorthBoundLatitude >= fieldBoundingBox.MinY)
                        .Select(joined => new { joined.product, joined.inspire })
                        .ToListAsync(cancellationToken);

                    var validProducts = new List<Sentinel2ProductEntity>();

                    // Проверяем точное пересечение только для предварительно отфильтрованных записей
                    foreach (var item in intersectingProducts)
                    {
                        try
                        {
                            // Проверяем пересечение между полем и метаданными Sentinel Inspire
                            try
                            {
                                // Создаем bounding box из метаданных Inspire
                                var inspireBoundingBox = new Envelope(
                                    (double)item.inspire.WestBoundLongitude,
                                    (double)item.inspire.EastBoundLongitude,
                                    (double)item.inspire.SouthBoundLatitude,
                                    (double)item.inspire.NorthBoundLatitude);

                                // Создаем геометрию bounding box
                                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory();
                                var inspireGeometry = geometryFactory.ToGeometry(inspireBoundingBox);

                                // Проверяем пересечение
                                if (field.Polygon.Intersects(inspireGeometry))
                                {
                                    validProducts.Add(item.product);
                                }
                            }
                            catch (Exception ex2)
                            {
                                _logger.LogWarning(ex2, "Error checking intersection for Inspire metadata {InspireId}", item.inspire.Id);
                            }
                        }
                        catch (Exception ex2)
                        {
                            _logger.LogWarning(ex2, "Error checking intersection for Sentinel product {ProductId}", item.product.SatellateProductId);
                        }
                    }

                    response.SentinelProducts = validProducts;
                }

                _logger.LogInformation("Found {SentinelCount} Sentinel products for field {FieldId} using geometry calculation",
                    response.SentinelProducts.Count, request.FieldId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Sentinel products for field {FieldId}", request.FieldId);
                response.SentinelProducts = new List<Sentinel2ProductEntity>();
            }

            return response;
        }
    }
}
