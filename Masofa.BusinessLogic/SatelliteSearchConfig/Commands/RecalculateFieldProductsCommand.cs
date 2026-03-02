using Masofa.Common.Resources;
using MediatR;
using Masofa.DataAccess;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using NetTopologySuite.Geometries;
using Masofa.Common.Models.Satellite;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Helper;

namespace Masofa.BusinessLogic.FieldSatellite.Commands
{
    /// <summary>
    /// Команда для пересчета маппинга для конкретного поля
    /// </summary>
    public class RecalculateFieldProductsCommand : IRequest
    {
        public Guid FieldId { get; set; }
        public CancellationToken CancellationToken { get; set; } = default;

        public RecalculateFieldProductsCommand(Guid fieldId, CancellationToken cancellationToken = default)
        {
            FieldId = fieldId;
            CancellationToken = cancellationToken;
        }
    }

    /// <summary>
    /// Обработчик команды для пересчета маппинга поля
    /// </summary>
    public class RecalculateFieldProductsHandler : IRequestHandler<RecalculateFieldProductsCommand>
    {
        private readonly MasofaCropMonitoringDbContext _cropMonitoringContext;
        private readonly MasofaLandsatDbContext _landsatDbContext;
        private readonly MasofaSentinelDbContext _sentinelDbContext;
        private readonly IBusinessLogicLogger _logger;

        public RecalculateFieldProductsHandler(
            MasofaCropMonitoringDbContext cropMonitoringContext,
            MasofaLandsatDbContext landsatDbContext,
            MasofaSentinelDbContext sentinelDbContext,
            IBusinessLogicLogger logger)
        {
            _cropMonitoringContext = cropMonitoringContext;
            _landsatDbContext = landsatDbContext;
            _sentinelDbContext = sentinelDbContext;
            _logger = logger;
        }

        public async Task Handle(RecalculateFieldProductsCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                _logger.LogInformationAsync(LogMessageResource.FieldRecalculationStarted(request.FieldId.ToString()), "RecalculateFieldProductsHandler.Handle");

                // Получаем поле
                var field = await _cropMonitoringContext.Fields
                    .Where(f => f.Id == request.FieldId && f.Status == Masofa.Common.Models.StatusType.Active)
                    .FirstOrDefaultAsync(request.CancellationToken);

                if (field?.Polygon == null)
                {
                    _logger.LogWarningAsync(LogMessageResource.FieldNotFoundOrNoPolygon(request.FieldId.ToString()), "RecalculateFieldProductsHandler.Handle");
                    return;
                }

                // Удаляем существующий маппинг для поля
                var existingMappings = await _cropMonitoringContext.FieldProductMappings
                    .Where(m => m.FieldId == request.FieldId)
                    .ToListAsync(request.CancellationToken);

                if (existingMappings.Any())
                {
                    _cropMonitoringContext.FieldProductMappings.RemoveRange(existingMappings);
                }

                // Пересчитываем из Landsat продуктов
                try
                {
                    _logger.LogInformationAsync(LogMessageResource.LandsatRecalculationStarted(request.FieldId.ToString()), "RecalculateFieldProductsHandler.Handle");

                    var stacFeatures = await _landsatDbContext.StacFeatures
                        .Where(sf => sf.GeometryJson != null)
                        .ToListAsync(cancellationToken);

                    var mappingsToAdd = new List<FieldProductMapping>();

                    foreach (var stacFeature in stacFeatures)
                    {
                        try
                        {
                            var productGeometry = GeometryCalculationHelper.ParseGeometryFromJson(stacFeature.GeometryJson!);
                            if (productGeometry != null && field.Polygon.Intersects(productGeometry))
                            {
                                var mapping = new FieldProductMapping
                                {
                                    FieldId = request.FieldId,
                                    ProductId = stacFeature.FeatureId,
                                    SatelliteType = ProductSourceType.Landsat
                                };
                                mappingsToAdd.Add(mapping);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarningAsync(LogMessageResource.StacFeatureProcessingError(stacFeature.FeatureId,ex.Message), "RecalculateFieldProductsHandler.Handle");
                        }
                    }

                    if (mappingsToAdd.Any())
                    {
                        await _cropMonitoringContext.FieldProductMappings.AddRangeAsync(mappingsToAdd, request.CancellationToken);
                        _logger.LogInformationAsync(LogMessageResource.LandsatMappingsAdded(mappingsToAdd.Count,request.FieldId.ToString()), "RecalculateFieldProductsHandler.Handle");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogErrorAsync(LogMessageResource.GenericError(requestPath, ex.Message), "RecalculateFieldProductsHandler.Handle");
                }
                
                // Пересчитываем из Sentinel продуктов
                try
                {
                    _logger.LogInformationAsync(LogMessageResource.SentinelRecalculationStarted(request.FieldId.ToString()), "RecalculateFieldProductsHandler.Handle");

                    var sentinelProducts = await _sentinelDbContext.Sentinel2Products
                        .Join(_sentinelDbContext.SentinelInspireMetadata,
                            product => product.SentinelInspireMetadataId,
                            inspire => inspire.Id,
                            (product, inspire) => new { product, inspire })
                        .ToListAsync(cancellationToken);

                    var mappingsToAdd = new List<FieldProductMapping>();

                    foreach (var item in sentinelProducts)
                    {
                        try
                        {
                            var inspireBoundingBox = new Envelope(
                                (double)item.inspire.WestBoundLongitude,
                                (double)item.inspire.EastBoundLongitude,
                                (double)item.inspire.SouthBoundLatitude,
                                (double)item.inspire.NorthBoundLatitude);

                            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory();
                            var inspireGeometry = geometryFactory.ToGeometry(inspireBoundingBox);

                            if (field.Polygon.Intersects(inspireGeometry))
                            {
                                var mapping = new FieldProductMapping
                                {
                                    FieldId = request.FieldId,
                                    ProductId = item.product.SatellateProductId,
                                    SatelliteType = ProductSourceType.Sentinel2
                                };
                                mappingsToAdd.Add(mapping);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarningAsync(LogMessageResource.SentinelProductProcessingError(item.product.SatellateProductId,ex.Message), "RecalculateFieldProductsHandler.Handle");
                        }
                    }

                    if (mappingsToAdd.Any())
                    {
                        await _cropMonitoringContext.FieldProductMappings.AddRangeAsync(mappingsToAdd, request.CancellationToken);
                        _logger.LogInformationAsync(LogMessageResource.SentinelMappingsAdded(mappingsToAdd.Count,request.FieldId.ToString()), "RecalculateFieldProductsHandler.Handle");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogErrorAsync(LogMessageResource.GenericError(requestPath, ex.Message), "RecalculateFieldProductsHandler.Handle");
                }

                await _cropMonitoringContext.SaveChangesAsync(request.CancellationToken);

                _logger.LogInformationAsync(LogMessageResource.FieldProductsRecalculated(request.FieldId.ToString()), "RecalculateFieldProductsHandler.Handle");
            }
            catch (Exception ex)
            {
                _logger.LogErrorAsync(LogMessageResource.GenericError(requestPath, ex.Message), "RecalculateFieldProductsHandler.Handle");
                throw;
            }
        }
    }
}
