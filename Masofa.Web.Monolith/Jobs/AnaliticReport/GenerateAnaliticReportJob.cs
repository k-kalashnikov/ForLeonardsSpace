using Masofa.BusinessLogic.AnaliticReport;
using Masofa.BusinessLogic.Index;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.MasofaAnaliticReport;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Masofa.Web.Monolith.Jobs.AnaliticReport
{
    /// <summary>
    /// Генерация HTML отчётов (рекомендации фермерам) после того, как:
    /// - отчёт в InProgress
    /// - QwenResult уже сохранён (CollectQwenFullAnalysisToReportJob)
    /// - HTML ещё не создан (FileStorageItemId == Guid.Empty)
    ///
    /// Важно: падение одного репорта не валит всю джобу.
    /// </summary>
    public sealed class GenerateAnaliticReportJob
        : BaseJob<GenerateAnaliticReportJobbResult>, IJob
    {
        private readonly MasofaAnaliticReportDbContext _reportDb;

        // По умолчанию делаем основной язык (чтобы не перетирать FileStorageItemId несколько раз).
        // Если позже захочешь 3 языка — надо менять модель (например, хранить LocalizationFile как реально persisted поле).
        private const string DefaultLocale = "ru-RU";
        private const bool AlsoPdf = false;

        public GenerateAnaliticReportJob(
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<GenerateAnaliticReportJob> logger,
            MasofaCommonDbContext commonDbContext,
            MasofaIdentityDbContext identityDbContext,
            MasofaAnaliticReportDbContext masofaAnaliticReportDbContext
        ) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            _reportDb = masofaAnaliticReportDbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation("Start GenerateAnaliticReportJob. Locale={Locale}", DefaultLocale);

            try
            {
                var reports = await _reportDb.FarmerRecomendationReports
                    .Where(r => r.ReportState == FarmerReportStateType.InProgress)
                    //.Where(r => !string.IsNullOrWhiteSpace(r.QwenJobResultJson))
                    .Where(r => r.FileStorageItemId == Guid.Empty)
                    .ToListAsync(context.CancellationToken);

                Result.CheckedReports = reports.Count;

                if (reports.Count == 0)
                {
                    Logger.LogInformation("No reports to generate.");
                    return;
                }

                foreach (var report in reports)
                {
                    try
                    {
                        var cmd = new BuildFarmerReportCommand
                        {
                            ReportId = report.Id,
                            ReportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            Locale = DefaultLocale,
                            AlsoPdf = AlsoPdf
                        };

                        var buildResult = await Mediator.Send(cmd, context.CancellationToken);

                        report.ReportState = FarmerReportStateType.Finished;
                        report.LastUpdateAt = DateTime.UtcNow;

                        Result.SuccessCount++;

                        Logger.LogInformation(
                            "Report generated. ReportId={ReportId}, HtmlObjectKey={HtmlObjectKey}",
                            report.Id, buildResult?.HtmlObjectKey
                        );
                    }
                    catch (Exception ex)
                    {
                        report.ReportState = FarmerReportStateType.Failed;
                        report.LastUpdateAt = DateTime.UtcNow;

                        var msg = $"Generate report failed. ReportId={report.Id}. Error={ex.Message}";
                        Result.Errors.Add(msg);

                        Logger.LogError(ex, "Generate report failed. ReportId={ReportId}", report.Id);
                        continue;
                    }
                }

                await _reportDb.SaveChangesAsync(context.CancellationToken);

                Logger.LogInformation(
                    "Finish GenerateAnaliticReportJob. SuccessCount={SuccessCount}, Errors={ErrorsCount}",
                    Result.SuccessCount, Result.Errors.Count
                );
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GenerateAnaliticReportJob failed");
                Result.Errors.Add(ex.Message);
            }
        }
    }

    public class GenerateAnaliticReportJobbResult : BaseJobResult
    {
        public int CheckedReports { get; set; }
        public int SuccessCount { get; set; } = 0;
    }
}
