using Masofa.Common.Resources;
using Masofa.Common.Models.Satellite;
using MediatR;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;

namespace Masofa.BusinessLogic.FieldSatellite.Requests
{
    /// <summary>
    /// Запрос для получения MBR для Landsat API
    /// </summary>
    public class GetLandsatMBRRequest : IRequest<LandsatMBR>
    {
        public CancellationToken CancellationToken { get; set; } = default;

        public GetLandsatMBRRequest(CancellationToken cancellationToken = default)
        {
            CancellationToken = cancellationToken;
        }
    }

    /// <summary>
    /// Обработчик запроса для получения MBR для Landsat API
    /// </summary>
    public class GetLandsatMBRHandler : IRequestHandler<GetLandsatMBRRequest, LandsatMBR>
    {
        private readonly IBusinessLogicLogger _logger;
        private readonly IMediator _mediator;

        public GetLandsatMBRHandler(IBusinessLogicLogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<LandsatMBR> Handle(GetLandsatMBRRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

            try
            {
                _logger.LogDebugAsync("LogMessageResource.LandsatMBRRequested()", requestPath);
                
                var config = await _mediator.Send(new GetActiveConfigRequest(request.CancellationToken), request.CancellationToken);
                
                if (config.LandsatLeftDown == null || config.LandsatRightUp == null)
                {
                    throw new InvalidOperationException("MBR не настроен в активной конфигурации");
                }

                var result = LandsatMBR.Create(config.LandsatLeftDown, config.LandsatRightUp);
                _logger.LogInformationAsync("LogMessageResource.LandsatMBRRetrieved()", requestPath);
                
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
