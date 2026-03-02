using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Attributes;
using Masofa.DataAccess;
using MediatR;
using Quartz;

namespace Masofa.Web.Monolith.Jobs.System
{
    /// <summary>
    /// Demo Job
    /// </summary>
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "HelloJob", "System")]
    public class HelloJob : BaseJob<HelloJobResult>, IJob
    {
        /// <inheritdoc/>
        public HelloJob(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger<HelloJob> logger, MasofaCommonDbContext commonDbContext, MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
        }

        /// <inheritdoc/>
        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation("### Привет от Quartz! Время: {Now}", DateTime.Now);
            try
            {
                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success,
                }, context);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in HelloJob");

                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed
                }, context);
            }
        }
    }

    /// <summary>
    /// Результат демонстрационной работы
    /// </summary>
    public class HelloJobResult : BaseJobResult
    {
        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
