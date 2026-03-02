using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.Qwen;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Masofa.Web.Monolith.Jobs.QwenAnalysis
{
    public class BidSentToQwenAnalyseJob : BaseJob<BidSentToQwenAnalyseJobResult>, IJob
    {
        private MasofaDictionariesDbContext DictionariesdbContext { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private QwenUnitOfWork QwenUnitOfWork { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }

        // express=true  -> экспресс (без рекомендаций) - тут нужен именно этот
        // express=false -> full (с рекомендациями) 
        private const bool Express = true;
        public BidSentToQwenAnalyseJob(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger<BidSentToQwenAnalyseJob> logger, MasofaCommonDbContext commonDbContext, MasofaIdentityDbContext identityDbContext, MasofaDictionariesDbContext dictionariesdbContext, MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, IFileStorageProvider fileStorageProvider, QwenUnitOfWork qwenUnitOfWork) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            DictionariesdbContext = dictionariesdbContext;
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            FileStorageProvider = fileStorageProvider;
            QwenUnitOfWork = qwenUnitOfWork;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation("Запуск джобы: {JobName}", nameof(BidSentToQwenAnalyseJob));

            var taskResult = new SystemBackgroundTaskResult
            {
                CreateAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow
            };

            try
            {
                var bids = await MasofaCropMonitoringDbContext.Bids
                    .Where(b => b.BidState == BidStateType.Finished && b.QwenTaskId == null)
                    .ToListAsync(context.CancellationToken);

                var successCount = 0;

                foreach (var bid in bids)
                {
                    Stream? archiveStream = null;
                    try
                    {
                        // 1. Получаем FileStorageItem из CommonDbContext
                        if (bid.FileResultId == null)
                        {
                            Logger.LogWarning("Заявка {BidId} не имеет FileResultId", bid.Id);
                            continue;
                        }

                        var fileItem = await CommonDbContext.FileStorageItems
                            .FirstOrDefaultAsync(f => f.Id == bid.FileResultId.Value, context.CancellationToken);

                        if (fileItem == null)
                        {
                            Logger.LogWarning("FileStorageItem не найден для заявки {BidId}", bid.Id);
                            continue;
                        }

                        // 2. Скачиваем архив из MinIO
                        archiveStream = await FileStorageProvider.GetFileStreamAsync(fileItem);

                        if (archiveStream is not MemoryStream ms || !ms.CanSeek)
                        {
                            Logger.LogError("Поток из MinIO не является seekable MemoryStream для заявки {BidId}", bid.Id);
                            Result.Errors.Add($"Non-seekable stream for Bid {bid.Id}");
                            continue;
                        }

                        // 3. Получаем культуру и diag_type
                        var crop = await DictionariesdbContext.Crops
                            .FirstOrDefaultAsync(c => c.Id == bid.CropId, context.CancellationToken);

                        if (string.IsNullOrWhiteSpace(crop?.Names))
                        {
                            Logger.LogWarning("DiagType не задан для культуры {CropId} в заявке {BidId}", bid.CropId, bid.Id);
                            Result.Errors.Add($"DiagType отсутствует для культуры {bid.CropId} (Bid {bid.Id})");
                            continue;
                        }

                        string? cropType = null;
                        cropType = crop.Names["en-US"];

                        // 4. Отправляем в Qwen
                        var jobId = await QwenUnitOfWork.QwenRepository.SubmitArchiveAsync(
                            archiveStream,
                            fileItem.FileStoragePath ?? Path.GetFileName(fileItem.FileStoragePath),
                            express: Express,
                            cropType: cropType
                        );

                        // 5. Обновляем заявку
                        bid.QwenTaskId = Guid.Parse(jobId);
                        bid.BidState = BidStateType.QwenAnalysisStart;
                        bid.LastUpdateAt = DateTime.UtcNow;

                        successCount++;
                        Logger.LogInformation("Заявка {BidId} отправлена на анализ. JobId: {JobId}", bid.Id, jobId);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Ошибка при обработке заявки {BidId}", bid.Id);
                        Result.Errors.Add($"Bid {bid.Id}: {ex.Message}");
                    }
                    finally
                    {
                        archiveStream?.Dispose();
                    }
                }

                await MasofaCropMonitoringDbContext.SaveChangesAsync(context.CancellationToken);

                Result.SuccessCount = successCount;

                taskResult.ResultType = Result.Errors.Count == 0
                    ? SystemBackgroundTaskResultType.Success
                    : (successCount > 0
                        ? SystemBackgroundTaskResultType.Success
                        : SystemBackgroundTaskResultType.Failed);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Критическая ошибка в джобе {JobName}", nameof(BidSentToQwenAnalyseJob));
                Result.Errors.Add($"Критическая ошибка: {ex.Message}");
                taskResult.ResultType = SystemBackgroundTaskResultType.Failed;
            }

            await SaveResult(taskResult, context);
        }

    }

    public class BidSentToQwenAnalyseJobResult : BaseJobResult
    {
        public int SuccessCount { get; set; } = 0;
    }
}
