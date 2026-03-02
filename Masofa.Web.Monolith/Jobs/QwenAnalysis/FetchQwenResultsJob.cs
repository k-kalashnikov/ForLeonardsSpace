using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.Qwen;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Text.Json;

namespace Masofa.Web.Monolith.Jobs.QwenAnalysis
{
    public class FetchQwenResultsJob : BaseJob<FetchQwenResultsJobResult>, IJob
    {
        private MasofaCropMonitoringDbContext _cropMonitoringDbContext { get; set; }
        private QwenUnitOfWork QwenUnitOfWork { get; set; }

        public FetchQwenResultsJob(
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<FetchQwenResultsJob> logger,
            MasofaCommonDbContext commonDbContext,
            MasofaIdentityDbContext identityDbContext,
            MasofaCropMonitoringDbContext cropMonitoringDbContext,
            QwenUnitOfWork qwenUnitOfWork)
            : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            _cropMonitoringDbContext = cropMonitoringDbContext;
            QwenUnitOfWork = qwenUnitOfWork;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation("Запуск джобы: {JobName}", nameof(FetchQwenResultsJob));

            var taskResult = new SystemBackgroundTaskResult
            {
                CreateAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow
            };

            try
            {
                var bids = await _cropMonitoringDbContext.Bids
                    .Where(b => b.BidState == BidStateType.QwenAnalysisStart && b.QwenTaskId.HasValue)
                    .ToListAsync(context.CancellationToken);

                var successCount = 0;

                foreach (var bid in bids)
                {
                    try
                    {
                        var jobId = bid.QwenTaskId.Value.ToString();

                        // 1. Получаем статус задачи
                        var statusResponse = await QwenUnitOfWork.QwenRepository.GetJobStatusAsync(jobId);

                        if (statusResponse.status == "SUCCESS")
                        {
                            // 2. Получаем результат
                            var results = await QwenUnitOfWork.QwenRepository.GetJobResultAsync(jobId);
                            bid.QwenAnalysisStart = DateTime.UtcNow;

                            // 3. Сохраняем в JSON
                            bid.QwenResults = results;
                            bid.BidState = BidStateType.QwenAnalysisEnd;
                            bid.LastUpdateAt = DateTime.UtcNow;
                            bid.QwenAnalysisEnd = DateTime.UtcNow;

                            successCount++;
                            Logger.LogInformation("Результат анализа получен и сохранён для заявки {BidId}", bid.Id);
                        }
                        else if (statusResponse.status is "REVOKED" or "FAILURE")
                        {
                            // 4. Обработка ошибок или отмены
                            bid.BidState = BidStateType.Rejected;
                            bid.LastUpdateAt = DateTime.UtcNow;
                            Logger.LogWarning("Задача Qwen отменена или завершилась с ошибкой для заявки {BidId}. Статус: {Status}", bid.Id, statusResponse.status);
                            Result.Errors.Add($"Bid {bid.Id}: задача завершилась со статусом {statusResponse.status}");
                        }
                        // Если "PENDING", "RUNNING" — ничего не делаем, ждём следующей итерации
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Ошибка при получении результата для заявки {BidId}", bid.Id);
                        Result.Errors.Add($"Bid {bid.Id}: {ex.Message}");
                    }
                }

                await _cropMonitoringDbContext.SaveChangesAsync(context.CancellationToken);

                Result.SuccessCount = successCount;

                taskResult.ResultType = Result.Errors.Count == 0
                    ? SystemBackgroundTaskResultType.Success
                    : (successCount > 0
                        ? SystemBackgroundTaskResultType.Success
                        : SystemBackgroundTaskResultType.Failed);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Критическая ошибка в джобе {JobName}", nameof(FetchQwenResultsJob));
                Result.Errors.Add($"Критическая ошибка: {ex.Message}");
                taskResult.ResultType = SystemBackgroundTaskResultType.Failed;
            }

            await SaveResult(taskResult, context);
        }
    }

    public class FetchQwenResultsJobResult : BaseJobResult
    {
        public int SuccessCount { get; set; } = 0;
    }
}
