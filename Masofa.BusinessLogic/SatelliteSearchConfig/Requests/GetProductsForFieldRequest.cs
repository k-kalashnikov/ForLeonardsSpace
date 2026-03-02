using Masofa.Common.Resources;
using Masofa.Common.Models.Satellite;
using MediatR;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;

namespace Masofa.BusinessLogic.FieldSatellite.Requests
{
    /// <summary>
    /// Запрос для получения продуктов для поля из маппинга
    /// </summary>
    public class GetProductsForFieldRequest : IRequest<List<FieldProductMapping>>
    {
        public Guid FieldId { get; set; }
        public ProductSourceType? SatelliteType { get; set; }
        public CancellationToken CancellationToken { get; set; } = default;

        public GetProductsForFieldRequest(Guid fieldId, ProductSourceType? satelliteType = null, CancellationToken cancellationToken = default)
        {
            FieldId = fieldId;
            SatelliteType = satelliteType;
            CancellationToken = cancellationToken;
        }
    }

    /// <summary>
    /// Обработчик запроса для получения продуктов для поля
    /// </summary>
    public class GetProductsForFieldHandler : IRequestHandler<GetProductsForFieldRequest, List<FieldProductMapping>>
    {
        private readonly MasofaCropMonitoringDbContext _cropMonitoringContext;
        private readonly IBusinessLogicLogger _logger;

        public GetProductsForFieldHandler(MasofaCropMonitoringDbContext cropMonitoringContext, IBusinessLogicLogger logger)
        {
            _cropMonitoringContext = cropMonitoringContext;
            _logger = logger;
        }

        public async Task<List<FieldProductMapping>> Handle(GetProductsForFieldRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                _logger.LogInformationAsync(LogMessageResource.ProductsForFieldRequested(request.FieldId.ToString()), requestPath);
                
                var query = _cropMonitoringContext.FieldProductMappings
                    .Where(m => m.FieldId == request.FieldId);

                if (request.SatelliteType.HasValue)
                    query = query.Where(m => m.SatelliteType == request.SatelliteType.Value);

                var result = await query.ToListAsync(request.CancellationToken);
                
                _logger.LogInformationAsync(LogMessageResource.ProductsForFieldRetrieved(result.Count,request.FieldId.ToString()), requestPath);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogErrorAsync(LogMessageResource.GenericError(requestPath, ex.Message), requestPath);
                throw;
            }
        }
    }
}
