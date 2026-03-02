using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.Qwen;
using Masofa.Client.Qwen.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.MasofaAnaliticReport;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;

namespace Masofa.Web.Monolith.Jobs.AnaliticReport
{
    /// <summary>
    /// Джоба собирает результаты FULL анализа Qwen и сохраняет их в FarmerRecomendationReport.QwenResult (JSON).
    ///
    /// Логика (partial fail-friendly):
    /// - Если хотя бы один jobId в PROCESSING/неизвестном статусе -> репорт НЕ сохраняем, ждём следующий прогон.
    /// - Если часть jobId = FAILED/ERROR, часть = SUCCESS -> сохраняем partial результат:
    ///     - items: объединение items[] только из SUCCESS jobId
    ///     - failed_jobs: список упавших задач (jobId, status, message)
    /// - Если ВСЕ jobId упали (0 SUCCESS и есть FAILED/ERROR) -> переводим репорт в Failed и сохраняем failed_jobs.
    ///
    /// Привязка к сезону:
    /// - Репорт (FieldId + SeasonId)
    /// - Bids выбираются по FieldId + CropId сезона (как вы договорились).
    /// </summary>
    public class CollectQwenFullAnalysisToReportJob
        : BaseJob<CollectQwenFullAnalysisToReportJobResult>, IJob
    {
        private readonly MasofaCropMonitoringDbContext _cropDb;
        private readonly MasofaAnaliticReportDbContext _reportDb;
        private readonly QwenUnitOfWork _qwen;

        public CollectQwenFullAnalysisToReportJob(
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<CollectQwenFullAnalysisToReportJob> logger,
            MasofaCommonDbContext commonDbContext,
            MasofaIdentityDbContext identityDbContext,
            MasofaCropMonitoringDbContext masofaCropMonitoringDbContext,
            MasofaAnaliticReportDbContext masofaAnaliticReportDbContext,
            QwenUnitOfWork qwenUnitOfWork
        ) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            _cropDb = masofaCropMonitoringDbContext;
            _reportDb = masofaAnaliticReportDbContext;
            _qwen = qwenUnitOfWork;
        }

        /// <inheritdoc/>
        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation("Start CollectQwenFullAnalysisToReportJob");

            try
            {
                // Берём репорты, которые ждут результата
                var reports = await _reportDb.FarmerRecomendationReports
                    .Where(r => r.ReportState == FarmerReportStateType.InProgress)
                    .Where(r => string.IsNullOrWhiteSpace(r.QwenJobResultJson))
                    .ToListAsync(context.CancellationToken);

                Result.CheckedReports = reports.Count;

                if (reports.Count == 0)
                {
                    Logger.LogInformation("No reports waiting for Qwen FULL results");
                    return;
                }

                var seasonIds = reports.Select(r => r.SeasonId).Distinct().ToList();

                var seasons = await _cropDb.Seasons
                    .AsNoTracking()
                    .Where(s => seasonIds.Contains(s.Id))
                    .ToDictionaryAsync(s => s.Id, context.CancellationToken);

                var savedReports = 0;
                var failedReports = 0;
                var partialReports = 0;

                foreach (var report in reports)
                {
                    try
                    {
                        if (!seasons.TryGetValue(report.SeasonId, out var season) || season == null || season.CropId == null)
                        {
                            Result.Errors.Add($"Report {report.Id}: season/crop not found");
                            continue;
                        }

                        // Берём bids, относящиеся к этому "сезону" через CropId
                        var bids = await _cropDb.Bids
                            .Where(b => b.FieldId == report.FieldId)
                            .Where(b => b.CropId == season.CropId.Value)
                            .Where(b => b.QwenTaskId != null)
                            .ToListAsync(context.CancellationToken);

                        if (bids.Count == 0)
                            continue;

                        var jobIds = bids
                            .Select(b => b.QwenTaskId!.Value.ToString())
                            .Distinct()
                            .ToList();

                        var bidsQwenEnd = new List<Bid>();

                        // 1) Проверяем статусы: ждём, пока ВСЕ будут в терминальном состоянии (SUCCESS/FAILED/ERROR)
                        var hasProcessing = false;
                        var successJobIds = new List<string>(capacity: jobIds.Count);
                        var failedJobs = new List<object>();

                        foreach (var jobId in jobIds)
                        {
                            QwenJobStatusResponse? status = null;

                            try
                            {
                                const int maxAttempts = 3;

                                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                                {
                                    try
                                    {
                                        status = await _qwen.QwenRepository.GetJobStatusAsync(jobId);
                                        break;
                                    }
                                    catch (Exception ex) when (IsTransientQwenError(ex) && attempt < maxAttempts)
                                    {
                                        await Task.Delay(300 * attempt);
                                    }
                                }

                                if (status == null)
                                {
                                    failedJobs.Add(new
                                    {
                                        jobId,
                                        status = "STATUS_ERROR",
                                        message = "Failed to get status (null/failed after retries)"
                                    });
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogWarning(ex, "Qwen status request failed for jobId={JobId}", jobId);

                                failedJobs.Add(new
                                {
                                    jobId,
                                    status = "STATUS_EXCEPTION",
                                    message = ex.Message
                                });

                                continue;
                            }

                            var st = status.status?.Trim();

                            if (IsProcessing(st))
                            {
                                hasProcessing = true;
                                break;
                            }

                            if (IsSuccess(st))
                            {
                                successJobIds.Add(jobId);
                                continue;
                            }

                            if (IsFailed(st))
                            {
                                failedJobs.Add(new
                                {
                                    jobId,
                                    status = st,
                                    message = status.message
                                });
                                continue;
                            }

                            hasProcessing = true;
                            failedJobs.Add(new
                            {
                                jobId,
                                status = st ?? "UNKNOWN",
                                message = status.message
                            });

                            continue;
                        }

                        if (hasProcessing)
                            continue;

                        // 2) Если все упали — валим репорт (делать отчёт не из чего)
                        if (successJobIds.Count == 0 && failedJobs.Count > 0)
                        {
                            //report.ReportState = FarmerReportStateType.Failed;
                            report.QwenJobResultJson = JsonConvert.SerializeObject(new
                            {
                                items = Array.Empty<object>(),
                                failed_jobs = failedJobs
                            });

                            report.LastUpdateAt = DateTime.UtcNow;

                            failedReports++;
                            savedReports++;
                            continue;
                        }

                        // 3) Качаем результаты ТОЛЬКО для SUCCESS jobIds и мерджим items[]
                        var mergedItems = new JArray();

                        var totalImages = 0;
                        var anomalyCount = 0;

                        foreach (var jobId in successJobIds)
                        {
                            var qwenResult = await _qwen.QwenRepository.GetJobResultAsync(jobId);

                            var bid = bids.Where(b => b.QwenTaskId == Guid.Parse(jobId)).First();
                            if(bid != null)
                            {
                                bid.BidState = BidStateType.QwenAnalysisEnd;
                                bidsQwenEnd.Add(bid);
                            }

                            // Items
                            if (qwenResult?.Items != null)
                            {
                                foreach (var item in qwenResult.Items)
                                    mergedItems.Add(JToken.FromObject(item));
                            }

                            // Summary (если есть)
                            if (qwenResult?.Summary != null)
                            {
                                totalImages += qwenResult.Summary.TotalImages;
                                anomalyCount += qwenResult.Summary.AnomalyCount;
                            }
                            else
                            {
                                // fallback: если summary вдруг нет, хотя items есть
                                if (qwenResult?.Items != null)
                                {
                                    totalImages += qwenResult.Items.Count;
                                    anomalyCount += qwenResult.Items.Count(x => x.AnomalyPresence);
                                }
                            }
                        }

                        var mergedRoot = new JObject
                        {
                            ["items"] = mergedItems,
                            ["summary"] = new JObject
                            {
                                ["anomaly_count"] = anomalyCount,
                                ["total_images"] = totalImages
                            }
                        };

                        report.QwenJobResultJson = mergedRoot.ToString(Formatting.None);
                        report.LastUpdateAt = DateTime.UtcNow;

                        await _reportDb.SaveChangesAsync(context.CancellationToken);
                        await _cropDb.SaveChangesAsync(context.CancellationToken);

                        if (failedJobs.Count > 0) partialReports++;
                        savedReports++;
                    }
                    catch (Exception ex)
                    {
                        Result.Errors.Add($"Report {report.Id}: {ex.Message}");
                        Logger.LogError(ex, "Failed collect Qwen FULL result for report {ReportId}", report.Id);
                    }
                }

                Result.SavedReports = savedReports;
                Result.FailedReports = failedReports;
                Result.PartialReports = partialReports;

                if (savedReports > 0)
                    await _reportDb.SaveChangesAsync(context.CancellationToken);

                Logger.LogInformation(
                    "CollectQwenFullAnalysisToReportJob finished. Checked={Checked}, Saved={Saved}, Failed={Failed}, Partial={Partial}",
                    Result.CheckedReports, Result.SavedReports, Result.FailedReports, Result.PartialReports);
            }
            catch (Exception ex)
            {
                Result.Errors.Add(ex.Message);
                Logger.LogError(ex, "CollectQwenFullAnalysisToReportJob failed");
            }
        }

        private static bool IsProcessing(string? status)
            => string.Equals(status, "PROCESSING", StringComparison.OrdinalIgnoreCase);

        private static bool IsSuccess(string? status)
            => string.Equals(status, "SUCCESS", StringComparison.OrdinalIgnoreCase);

        private static bool IsFailed(string? status)
            => string.Equals(status, "FAILED", StringComparison.OrdinalIgnoreCase)
               || string.Equals(status, "ERROR", StringComparison.OrdinalIgnoreCase);

        static bool IsTransientQwenError(Exception ex)
        {
            if (ex is TaskCanceledException) return true; // timeout
            if (ex is HttpRequestException) return true;

            // если репозиторий кидает Exception("... Status: InternalServerError ...")
            var msg = ex.Message ?? "";
            if (msg.Contains("InternalServerError", StringComparison.OrdinalIgnoreCase)) return true;
            if (msg.Contains("Status: 5", StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }
    }

    public sealed class CollectQwenFullAnalysisToReportJobResult : BaseJobResult
    {
        public int CheckedReports { get; set; }
        public int SavedReports { get; set; }
        public int FailedReports { get; set; }
        public int PartialReports { get; set; }
    }
}
