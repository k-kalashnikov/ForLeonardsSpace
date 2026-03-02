using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.Qwen;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.MasofaAnaliticReport;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Masofa.Web.Monolith.Jobs.AnaliticReport
{
    /// <summary>
    /// Job для отправки заявок на анализ в ии, для отчёта(рекомендации фермерам)
    /// </summary>
    public class StartQwenAnalysisJob : BaseJob<StartQwenAnalysisJobResult>, IJob
    {
        private MasofaAnaliticReportDbContext MasofaAnaliticReportDbContext {  get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private QwenUnitOfWork QwenUnitOfWork { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }

        // express=true  -> экспресс (без рекомендаций)
        // express=false -> full (с рекомендациями) - тут нужен именно этот
        private const bool Express = false;
        public StartQwenAnalysisJob(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger<StartQwenAnalysisJob> logger, MasofaCommonDbContext commonDbContext, MasofaIdentityDbContext identityDbContext, MasofaAnaliticReportDbContext masofaAnaliticReportDbContext, MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, QwenUnitOfWork qwenUnitOfWork, IFileStorageProvider fileStorageProvider, MasofaDictionariesDbContext masofaDictionariesDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            MasofaAnaliticReportDbContext = masofaAnaliticReportDbContext;
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            QwenUnitOfWork = qwenUnitOfWork;
            FileStorageProvider = fileStorageProvider;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation("Start StartQwenAnalysisJob for farmer recommendation. express={Express}", Express);

            try
            {
                var reports = await MasofaAnaliticReportDbContext.FarmerRecomendationReports
                .Where(r => r.ReportState == FarmerReportStateType.New)
                .Take(25)
                .ToListAsync();

                if (reports.Count == 0)
                {
                    Logger.LogInformation("No new FarmerRecomendationReports found.");
                    return;
                }

                var fieldIds = reports
                    .Select(r => r.FieldId)
                    .Distinct()
                    .ToList();

                var bids = await MasofaCropMonitoringDbContext.Bids
                   .Where(b => b.FieldId != null && fieldIds.Contains(b.FieldId.Value))
                   .Where(b => b.QwenTaskId == null && b.BidState != BidStateType.QwenAnalysisStart)
                   .ToListAsync(context.CancellationToken);

                var fileIds = bids
                    .Where(b => b.FileResultId.HasValue)
                    .Select(b => b.FileResultId!.Value)
                    .Distinct()
                    .ToList();

                var files = await CommonDbContext.FileStorageItems
                    .Where(f => fileIds.Contains(f.Id))
                    .ToDictionaryAsync(f => f.Id, context.CancellationToken);

                var cropIds = bids
                    .Where(b => b.CropId != null)
                    .Select(b => b.CropId)
                    .Distinct()
                    .ToList();

                var crops = await MasofaDictionariesDbContext.Crops
                    .Where(c => cropIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, context.CancellationToken);

                foreach (var bid in bids)
                {
                    if (!bid.FileResultId.HasValue)
                    {
                        Logger.LogWarning("Bid {BidId} has no FileResultId", bid.Id);
                        Result.Errors.Add($"Bid {bid.Id}: no FileResultId");
                        continue;
                    }

                    if (!files.TryGetValue(bid.FileResultId.Value, out var fileItem) || fileItem == null)
                    {
                        Logger.LogWarning("FileStorageItem not found for Bid {BidId}", bid.Id);
                        Result.Errors.Add($"Bid {bid.Id}: FileStorageItem not found");
                        continue;
                    }

                    var fileName =
                        !string.IsNullOrWhiteSpace(fileItem.FileStoragePath)
                            ? Path.GetFileName(fileItem.FileStoragePath)
                            : $"bid_{bid.Id}.zip";

                    string? cropType = null;
                    if (crops.TryGetValue(bid.CropId, out var crop) && crop != null)
                    {
                        cropType = crop.Names["en-US"];
                    }

                    try
                    {
                        await using var stream = await FileStorageProvider.GetFileStreamAsync(fileItem);

                        Stream uploadStream = stream;
                        MemoryStream? temp = null;

                        if (!stream.CanSeek)
                        {
                            temp = new MemoryStream();
                            await stream.CopyToAsync(temp, context.CancellationToken);
                            temp.Position = 0;
                            uploadStream = temp;
                        }
                        else
                        {
                            stream.Position = 0;
                        }

                        var jobId = await QwenUnitOfWork.QwenRepository.SubmitArchiveAsync(
                            uploadStream,
                            fileName,
                            express: Express,
                            cropType: cropType
                        );

                        if (!Guid.TryParse(jobId, out var guidJobId))
                        {
                            Logger.LogError("Qwen returned non-GUID jobId='{JobId}' for Bid {BidId}", jobId, bid.Id);
                            Result.Errors.Add($"Bid {bid.Id}: non-GUID jobId={jobId}");
                            continue;
                        }

                        bid.QwenTaskId = guidJobId;
                        bid.BidState = BidStateType.QwenAnalysisStart;
                        bid.LastUpdateAt = DateTime.UtcNow;

                        Result.SuccessCount++;
                        Logger.LogInformation("Bid {BidId} submitted to Qwen. JobId={JobId}, express={Express}",
                            bid.Id, jobId, Express);

                        if (temp != null)
                            await temp.DisposeAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Failed to submit Bid {BidId} to Qwen", bid.Id);
                        Result.Errors.Add($"Bid {bid.Id}: {ex.Message}");
                        continue;
                    }
                }

                await MasofaCropMonitoringDbContext.SaveChangesAsync(context.CancellationToken);

                foreach (var rep in reports)
                {
                    rep.ReportState = FarmerReportStateType.InProgress;
                }

                await MasofaAnaliticReportDbContext.SaveChangesAsync(context.CancellationToken);

                Logger.LogInformation("StartQwenAnalysisJob finished. SuccessCount={SuccessCount}, Errors={ErrorsCount}",
                    Result.SuccessCount, Result.Errors.Count);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "StartQwenAnalysisJob failed");
                Result.Errors.Add(ex.Message);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StartQwenAnalysisJobResult : BaseJobResult
    {
        public int SuccessCount { get; set; } = 0;
    }
}
