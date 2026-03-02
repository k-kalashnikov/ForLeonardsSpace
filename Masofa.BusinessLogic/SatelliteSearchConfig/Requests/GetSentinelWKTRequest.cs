using Masofa.Common.Resources;
using MediatR;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Helper;

namespace Masofa.BusinessLogic.FieldSatellite.Requests
{
    /// <summary>
    /// Запрос для получения WKT для Sentinel2 API
    /// </summary>
    public class GetSentinelWKTRequest : IRequest<string>
    {
        public CancellationToken CancellationToken { get; set; } = default;

        public GetSentinelWKTRequest(CancellationToken cancellationToken = default)
        {
            CancellationToken = cancellationToken;
        }
    }

    /// <summary>
    /// Обработчик запроса для получения WKT для Sentinel2 API
    /// </summary>
    public class GetSentinelWKTHandler : IRequestHandler<GetSentinelWKTRequest, string>
    {
        private readonly IBusinessLogicLogger _logger;
        private readonly IMediator _mediator;

        public GetSentinelWKTHandler(IBusinessLogicLogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<string> Handle(GetSentinelWKTRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

            try
            {
                _logger.LogDebugAsync("LogMessageResource.SentinelWKTRequested()", requestPath);
                
                var config = await _mediator.Send(new GetActiveConfigRequest(request.CancellationToken), request.CancellationToken);
                
                if (config.SentinelPolygon == null)
                {
                    throw new InvalidOperationException("Sentinel полигон не настроен в активной конфигурации");
                }

                var result = GeometryCalculationHelper.CreateSentinelWKT(config.SentinelPolygon);
                _logger.LogInformationAsync("LogMessageResource.SentinelWKTRetrieved()", requestPath);
                
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
