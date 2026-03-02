using Masofa.Common.Resources;
using Masofa.Common.Models.Satellite;
using MediatR;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.FieldSatellite.Commands
{
    /// <summary>
    /// Команда для удаления маппинга для продукта
    /// </summary>
    public class RemoveProductMappingCommand : IRequest
    {
        public string ProductId { get; set; }
        public ProductSourceType SatelliteType { get; set; }
        public CancellationToken CancellationToken { get; set; } = default;

        public RemoveProductMappingCommand(string productId, ProductSourceType satelliteType, CancellationToken cancellationToken = default)
        {
            ProductId = productId;
            SatelliteType = satelliteType;
            CancellationToken = cancellationToken;
        }
    }

    /// <summary>
    /// Обработчик команды для удаления маппинга продукта
    /// </summary>
    public class RemoveProductMappingHandler : IRequestHandler<RemoveProductMappingCommand>
    {
        private readonly MasofaCropMonitoringDbContext _cropMonitoringContext;
        private readonly IBusinessLogicLogger _logger;

        public RemoveProductMappingHandler(
            MasofaCropMonitoringDbContext cropMonitoringContext,
            IBusinessLogicLogger logger)
        {
            _cropMonitoringContext = cropMonitoringContext;
            _logger = logger;
        }

        public async Task Handle(RemoveProductMappingCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                _logger.LogInformationAsync(LogMessageResource.ProductMappingRemovalStarted(request.ProductId), "RemoveProductMappingHandler.Handle");
                
                var mappings = await _cropMonitoringContext.FieldProductMappings
                    .Where(m => m.ProductId == request.ProductId && m.SatelliteType == request.SatelliteType)
                    .ToListAsync(request.CancellationToken);

                if (mappings.Any())
                {
                    _cropMonitoringContext.FieldProductMappings.RemoveRange(mappings);
                    await _cropMonitoringContext.SaveChangesAsync(request.CancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorAsync(LogMessageResource.GenericError(requestPath, ex.Message), "RemoveProductMappingHandler.Handle");
                throw;
            }
        }
    }
}
