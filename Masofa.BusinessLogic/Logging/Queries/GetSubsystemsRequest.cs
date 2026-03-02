using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.Logging.Queries
{
    [RequestPermission(ActionType = "Read")]
    public class GetSubsystemsRequest : IRequest<List<string>>
    {
    }

    public class GetSubsystemsRequestHandler : IRequestHandler<GetSubsystemsRequest, List<string>>
    {
        private readonly ILogger<GetSubsystemsRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetSubsystemsRequestHandler(
            ILogger<GetSubsystemsRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<List<string>> Handle(GetSubsystemsRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var dbContexts = typeof(MasofaCommonDbContext).Assembly.GetTypes()
                    .Where(m => m.IsSubclassOf(typeof(DbContext)) 
                             && !m.IsAbstract 
                             && !m.Name.Contains("History", StringComparison.OrdinalIgnoreCase));

                var subsystems = new HashSet<string>();
                foreach (var dbContextType in dbContexts)
                {
                    var moduleName = NormalizeModuleName(dbContextType.Name);
                    if (!string.IsNullOrEmpty(moduleName))
                    {
                        subsystems.Add(moduleName);
                    }
                }

                var result = subsystems.OrderBy(s => s).ToList();

                await _businessLogicLogger.LogInformationAsync(
                    LogMessageResource.RequestFinishedWithResult(requestPath, $"{result.Count} subsystems"), 
                    requestPath);
                
                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }

        private static string NormalizeModuleName(string dbContextName)
        {
            var name = dbContextName;

            if (name.StartsWith("Masofa", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring("Masofa".Length);
            }

            if (name.EndsWith("DbContext", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.Length - "DbContext".Length);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            return string.Empty;
        }
    }
}

