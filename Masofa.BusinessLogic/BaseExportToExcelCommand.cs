using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Helper;
using Masofa.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic
{
    public class BaseExportToExcelCommand<TModel, TDbContext> : IRequest<byte[]>
        where TDbContext : DbContext
        where TModel : BaseEntity
    {
        public List<TModel> Models { get; set; } = [];
    }

    public class BaseExportToExcelCommandHandler<TModel, TDbContext> : IRequestHandler<BaseExportToExcelCommand<TModel, TDbContext>, byte[]>
        where TDbContext : DbContext
        where TModel : BaseEntity
    {
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private ILogger Logger { get; set; }
        public BaseExportToExcelCommandHandler(IBusinessLogicLogger businessLogicLogger, ILogger<BaseExportToExcelCommandHandler<TModel, TDbContext>> logger)
        {
            BusinessLogicLogger = businessLogicLogger;
            Logger = logger;
        }

        public async Task<byte[]> Handle(BaseExportToExcelCommand<TModel, TDbContext> request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var result = BaseEntityExportHelper<TModel>.ToExcel(request.Models);

                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,result.ToString()), requestPath);

                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }
    }
}
