using Masofa.BusinessLogic.Index;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Attributes;
using Masofa.DataAccess;
using MediatR;
using Quartz;

namespace Masofa.Web.Monolith.Jobs.SatelliteIndices
{
    /// <summary>
    /// Работа для расчета аномалий в точках по значениям индексов
    /// </summary>
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "AnomaliesPointAndTiffJob", "Sentinel")]
    public class AnomaliesPointAndTiffJob : BaseJob<AnomaliesPointAndTiffJobResult>, IJob
    {
        /// <summary>
        /// Конструктор работы для расчета аномалий в точках по значениям индексов
        /// </summary>
        public AnomaliesPointAndTiffJob(
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<AnomaliesPointAndTiffJob> logger,
            MasofaCommonDbContext commonDbContext,
            MasofaIdentityDbContext identityDbContext) : base(
                mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
        }

        /// <summary>
        /// Основной метод выполнения
        /// </summary>
        /// <param name="context">Констекст выполнения</param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var anomalyDate = DateOnly.FromDateTime(DateTime.UtcNow);

                await Mediator.Send(new CalculateAnomalyOnDateCommand()
                {
                    Date = anomalyDate
                });
            }
            catch (Exception ex)
            {
                Result.Errors.Add($"Error processing: {ex.Message}");
                Logger.LogError($"Error processing: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Результат работа для расчета аномалий в точках по значениям индексов
    /// </summary>
    public class AnomaliesPointAndTiffJobResult : BaseJobResult
    {
        /// <summary>
        /// Количество успешных результатов
        /// </summary>
        public int SuccessCount { get; set; } = 0;
    }
}
