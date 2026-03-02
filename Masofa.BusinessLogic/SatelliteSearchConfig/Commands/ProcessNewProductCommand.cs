using Masofa.Common.Resources;
using Masofa.Common.Models.Satellite;
using MediatR;
using NetTopologySuite.Geometries;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Helper;

namespace Masofa.BusinessLogic.FieldSatellite.Commands
{
    /// <summary>
    /// Команда для обработки нового продукта и вычисления пересечений с полями
    /// </summary>
    public class ProcessNewProductCommand : IRequest
    {
        public string ProductId { get; set; }
        public ProductSourceType SatelliteType { get; set; }
        public Polygon ProductGeometry { get; set; }
        public CancellationToken CancellationToken { get; set; } = default;

        public ProcessNewProductCommand(string productId, ProductSourceType satelliteType, Polygon productGeometry, CancellationToken cancellationToken = default)
        {
            ProductId = productId;
            SatelliteType = satelliteType;
            ProductGeometry = productGeometry;
            CancellationToken = cancellationToken;
        }
    }

    /// <summary>
    /// Обработчик команды для обработки нового продукта
    /// </summary>
    public class ProcessNewProductHandler : IRequestHandler<ProcessNewProductCommand>
    {
        private readonly MasofaCropMonitoringDbContext _cropMonitoringContext;
        private readonly IBusinessLogicLogger _logger;

        public ProcessNewProductHandler(
            MasofaCropMonitoringDbContext cropMonitoringContext,
            IBusinessLogicLogger logger)
        {
            _cropMonitoringContext = cropMonitoringContext;
            _logger = logger;
        }

        public async Task Handle(ProcessNewProductCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                _logger.LogInformationAsync(LogMessageResource.NewProductProcessingStarted(request.ProductId, request.SatelliteType.ToString()), "ProcessNewProductHandler.Handle");

                // Получаем все поля
                var fields = await _cropMonitoringContext.Fields
                    .Where(f => f.Polygon != null && f.Status == Masofa.Common.Models.StatusType.Active)
                    .ToListAsync(request.CancellationToken);

                var mappingsToAdd = new List<FieldProductMapping>();

                foreach (var field in fields)
                {
                    var hasIntersection = GeometryCalculationHelper.CalculateIntersection(field.Polygon!, request.ProductGeometry);
                    
                    if (hasIntersection)
                    {
                        var mapping = new FieldProductMapping
                        {
                            FieldId = field.Id,
                            ProductId = request.ProductId,
                            SatelliteType = request.SatelliteType
                        };
                        mappingsToAdd.Add(mapping);
                    }
                }

                if (mappingsToAdd.Any())
                {
                    await _cropMonitoringContext.FieldProductMappings.AddRangeAsync(mappingsToAdd, request.CancellationToken);
                    await _cropMonitoringContext.SaveChangesAsync(request.CancellationToken);
                    _logger.LogInformationAsync(LogMessageResource.FieldMappingsAdded(mappingsToAdd.Count,request.ProductId), "ProcessNewProductHandler.Handle");
                }
                else
                {
                    _logger.LogInformationAsync(LogMessageResource.NoFieldIntersections(request.ProductId), "ProcessNewProductHandler.Handle");
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorAsync(LogMessageResource.GenericError(requestPath, ex.Message), "ProcessNewProductHandler.Handle");
                throw;
            }
        }
    }
}
