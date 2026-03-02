using Masofa.BusinessLogic.Attributes;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Landsat;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using Masofa.BusinessLogic.FieldSatellite.Requests;
using Masofa.Common.Helper;

namespace Masofa.BusinessLogic.CropMonitoring.Fields
{
    /// <summary>
    /// Запрос для получения Landsat продуктов по полю
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetFieldSatelliteLandsatProductsRequest : IRequest<FieldSatelliteLandsatProductsResponse>
    {
        /// <summary>
        /// Идентификатор поля
        /// </summary>
        public Guid FieldId { get; set; }
    }

    /// <summary>
    /// Ответ с Landsat продуктами для поля
    /// </summary>
    public class FieldSatelliteLandsatProductsResponse
    {
        /// <summary>
        /// Список продуктов Landsat
        /// </summary>
        public List<LandsatProductEntity> LandsatProducts { get; set; } = new();
    }

    /// <summary>
    /// Обработчик запроса для получения Landsat продуктов по полю
    /// </summary>
    public class GetFieldSatelliteLandsatProductsRequestHandler : IRequestHandler<GetFieldSatelliteLandsatProductsRequest, FieldSatelliteLandsatProductsResponse>
    {
        private readonly MasofaCropMonitoringDbContext _cropMonitoringContext;
        private readonly MasofaLandsatDbContext _landsatDbContext;
        private readonly IMediator _mediator;
        private readonly ILogger<GetFieldSatelliteLandsatProductsRequestHandler> _logger;

        public GetFieldSatelliteLandsatProductsRequestHandler(
            MasofaCropMonitoringDbContext cropMonitoringContext,
            MasofaLandsatDbContext landsatDbContext,
            IMediator mediator,
            ILogger<GetFieldSatelliteLandsatProductsRequestHandler> logger)
        {
            _cropMonitoringContext = cropMonitoringContext;
            _landsatDbContext = landsatDbContext;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<FieldSatelliteLandsatProductsResponse> Handle(GetFieldSatelliteLandsatProductsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting Landsat products for field {FieldId}", request.FieldId);

            var response = new FieldSatelliteLandsatProductsResponse();

            // Сначала пытаемся получить из маппинга
            try
            {
                var mappings = await _mediator.Send(new GetProductsForFieldRequest(
                    request.FieldId,
                    ProductSourceType.Landsat,
                    cancellationToken), cancellationToken);

                if (mappings.Any())
                {
                    // Получаем продукты по ID из маппинга
                    var productIds = mappings.Select(m => m.ProductId).ToList();
                    var products = await _landsatDbContext.LandsatProducts
                        .Where(p => productIds.Contains(p.SatellateProductId))
                        .ToListAsync(cancellationToken);

                    response.LandsatProducts = products;
                    _logger.LogInformation("Found {LandsatCount} Landsat products for field {FieldId} from mapping", 
                        response.LandsatProducts.Count, request.FieldId);
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
                // Оптимизированный метод получения продуктов Landsat с использованием PostgreSQL функций геометрии
                try
                {
                    // Оптимизированный запрос с использованием PostgreSQL функций геометрии
                    // Сначала фильтруем по bounding box для быстрой предварительной фильтрации
                    var fieldBoundingBox = field.Polygon.EnvelopeInternal;
                    
                    // Используем raw SQL для максимальной производительности с PostgreSQL функциями геометрии
                    var intersectingFeatureIds = await _landsatDbContext.StacFeatures
                        .FromSqlRaw(@"
                            SELECT * FROM ""StacFeatures"" 
                            WHERE ""GeometryJson"" IS NOT NULL 
                            AND ""GeometryJson"" != ''
                            AND ST_Intersects(
                                ST_GeomFromGeoJSON(""GeometryJson""), 
                                ST_GeomFromText({0})
                            )", field.Polygon.AsText())
                        .Select(sf => sf.FeatureId)
                        .ToListAsync(cancellationToken);

                    if (intersectingFeatureIds.Any())
                    {
                        // Получаем продукты только для валидных feature IDs
                        var products = await _landsatDbContext.LandsatProducts
                            .Where(p => intersectingFeatureIds.Contains(p.SatellateProductId))
                            .ToListAsync(cancellationToken);

                        response.LandsatProducts = products;
                        _logger.LogInformation("Found {LandsatCount} Landsat products for field {FieldId} using geometry calculation", 
                            response.LandsatProducts.Count, request.FieldId);
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting Landsat products with intersection");
                    
                    // Fallback к предыдущему методу если raw SQL не работает
                    // Fallback метод с предварительной фильтрацией по bounding box
                    var fieldBoundingBox = field.Polygon.EnvelopeInternal;
                    
                    var intersectingFeatureIds = await _landsatDbContext.StacFeatures
                        .Where(sf => !string.IsNullOrEmpty(sf.GeometryJson))
                        .Where(sf => 
                            // Предварительная фильтрация по bounding box из JSON
                            EF.Functions.JsonContains(sf.BoundingBoxJson, 
                                $"[{fieldBoundingBox.MinX}, {fieldBoundingBox.MinY}, {fieldBoundingBox.MaxX}, {fieldBoundingBox.MaxY}]"))
                        .Select(sf => new { sf.FeatureId, sf.GeometryJson })
                        .ToListAsync(cancellationToken);

                    var validFeatureIds = new List<string>();

                    // Проверяем точное пересечение только для предварительно отфильтрованных записей
                    foreach (var feature in intersectingFeatureIds)
                    {
                        try
                        {
                            var geometry = GeometryCalculationHelper.ParseGeometryFromJson(feature.GeometryJson);
                            if (geometry != null && field.Polygon.Intersects(geometry))
                            {
                                validFeatureIds.Add(feature.FeatureId);
                            }
                        }
                        catch (Exception ex2)
                        {
                            _logger.LogWarning(ex2, "Error parsing geometry for STAC feature {FeatureId}", feature.FeatureId);
                        }
                    }

                    if (validFeatureIds.Any())
                    {
                        // Получаем продукты только для валидных feature IDs
                        var products = await _landsatDbContext.LandsatProducts
                            .Where(p => validFeatureIds.Contains(p.SatellateProductId))
                            .ToListAsync(cancellationToken);

                        response.LandsatProducts = products;
                    }
                }

                _logger.LogInformation("Found {LandsatCount} Landsat products for field {FieldId} using geometry calculation", 
                    response.LandsatProducts.Count, request.FieldId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Landsat products for field {FieldId}", request.FieldId);
                response.LandsatProducts = new List<LandsatProductEntity>();
            }

            return response;
        }
    }
}
