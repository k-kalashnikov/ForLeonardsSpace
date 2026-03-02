using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.Common.Models.Satellite;
using MediatR;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;

namespace Masofa.BusinessLogic.FieldSatellite.Requests
{
    /// <summary>
    /// Запрос для получения активной конфигурации поиска
    /// </summary>
    public class GetActiveConfigRequest : IRequest<SatelliteSearchConfig>
    {
        public CancellationToken CancellationToken { get; set; } = default;

        public GetActiveConfigRequest(CancellationToken cancellationToken = default)
        {
            CancellationToken = cancellationToken;
        }
    }

    /// <summary>
    /// Обработчик запроса для получения активной конфигурации поиска
    /// </summary>
    public class GetActiveConfigHandler : IRequestHandler<GetActiveConfigRequest, SatelliteSearchConfig>
    {
        private readonly MasofaCropMonitoringDbContext _cropMonitoringContext;
        private readonly IBusinessLogicLogger _logger;

        public GetActiveConfigHandler(MasofaCropMonitoringDbContext cropMonitoringContext, IBusinessLogicLogger logger)
        {
            _cropMonitoringContext = cropMonitoringContext;
            _logger = logger;
        }

        public async Task<SatelliteSearchConfig> Handle(GetActiveConfigRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";

            try
            {
                await _logger.LogDebugAsync("LogMessageResource.ActiveConfigRetrieved()", requestPath);
                
                var config = await _cropMonitoringContext.SatelliteSearchConfigs
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.CreateAt)
                    .FirstOrDefaultAsync(request.CancellationToken);

                if (config == null)
                {
                    await _logger.LogWarningAsync("LogMessageResource.ActiveConfigNotFound()", requestPath);
                    return await GetOrCreateDefaultConfigAsync(request.CancellationToken);
                }

                return config;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(LogMessageResource.GenericError(requestPath, ex.Message), requestPath);
                throw;
            }
        }

        private async Task<SatelliteSearchConfig> GetOrCreateDefaultConfigAsync(CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(GetOrCreateDefaultConfigAsync)}";

            var defaultConfig = await _cropMonitoringContext.SatelliteSearchConfigs
                .Where(c => c.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (defaultConfig == null)
            {
                defaultConfig = new SatelliteSearchConfig
                {
                    Id = Guid.NewGuid(),
                    CreateAt = DateTime.UtcNow,
                    IsActive = true,
                    FieldsCount = 0
                };

                _cropMonitoringContext.SatelliteSearchConfigs.Add(defaultConfig);
                await _cropMonitoringContext.SaveChangesAsync(cancellationToken);
                
                await _logger.LogInformationAsync("LogMessageResource.DefaultConfigCreated()", requestPath);
            }

            return defaultConfig;
        }
    }
}
