using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.Qwen;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.MasofaAnaliticReport;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Masofa.BusinessLogic.AnaliticReport
{
    public class StartQwenAnalysisCommand : IRequest<StartQwenAnalysisResult>
    {
        public Guid FieldId { get; set; }
        public Guid SeasonId { get; set; }
        public DateOnly ReportDate { get; set; }
        public string Locale { get; set; } = "ru-RU";
        public bool AlsoPdf { get; set; } = true;
    }

    public class StartQwenAnalysisResult
    {
        public List<Guid> ReportIds { get; set; } = new();
    }

    public class StartQwenAnalysisHandler : IRequestHandler<StartQwenAnalysisCommand, StartQwenAnalysisResult>
    {
        private MasofaAnaliticReportDbContext MasofaAnaliticReportDbContext { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private QwenUnitOfWork QwenUnitOfWork { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }
        private ILogger Logger { get; set; }

        // express=true  -> экспресс (без рекомендаций)
        // express=false -> full (с рекомендациями) - тут нужен именно этот
        private const bool Express = false;
        public StartQwenAnalysisHandler(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger<StartQwenAnalysisHandler> logger, MasofaCommonDbContext commonDbContext, MasofaIdentityDbContext identityDbContext, MasofaAnaliticReportDbContext masofaAnaliticReportDbContext, MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, QwenUnitOfWork qwenUnitOfWork, IFileStorageProvider fileStorageProvider, MasofaDictionariesDbContext masofaDictionariesDbContext)
        {
            MasofaAnaliticReportDbContext = masofaAnaliticReportDbContext;
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            CommonDbContext = commonDbContext;
            QwenUnitOfWork = qwenUnitOfWork;
            FileStorageProvider = fileStorageProvider;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            Logger = logger;
        }

        public async Task<StartQwenAnalysisResult> Handle(StartQwenAnalysisCommand request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Start StartQwenAnalysisCommand. FieldId={FieldId}, express={Express}",
                request.FieldId, Express);

            // 1) Берём сезон поля
            var season= await MasofaCropMonitoringDbContext.Seasons
                .AsNoTracking()
                .Where(s => s.Id == request.SeasonId)
                .FirstAsync(cancellationToken);

            if (season == null)
                return new StartQwenAnalysisResult();

            // 2) Грузим сразу все bids поля (чтобы потом фильтровать в памяти по CropId)
            var allBids = await MasofaCropMonitoringDbContext.Bids
                .Where(b => b.FieldId == request.FieldId)
                .Where(b => b.QwenTaskId == null && b.BidState != BidStateType.QwenAnalysisStart)
                .ToListAsync(cancellationToken);

            // 3) Подготовка файлов батчем
            var fileIds = allBids
                .Where(b => b.FileResultId.HasValue)
                .Select(b => b.FileResultId!.Value)
                .Distinct()
                .ToList();

            var files = await CommonDbContext.FileStorageItems
                .Where(f => fileIds.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, cancellationToken);

            var cropId = season.CropId;

            var crops = await MasofaDictionariesDbContext.Crops
                .AsNoTracking()
                .Where(c => c.Id == cropId)
                .ToDictionaryAsync(c => c.Id, cancellationToken);

            // 4) Результат
            var result = new StartQwenAnalysisResult();


            if (season.CropId == null)
                return result;

            var localizationFile = new LocalizationFileStorageItem();
            localizationFile.ForceSet(request.Locale, Guid.Empty);
            //localizationFile[request.Locale] = Guid.Empty;

            // 4.1) создаём запись отчёта на сезон (как в BuildFarmerReportCommand)
            var report = new FarmerRecomendationReport
            {
                FieldId = request.FieldId,
                SeasonId = season.Id,
                ReportState = FarmerReportStateType.InProgress,
                CreateAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow,
                LocalizationFile = localizationFile
            };

            var reportEntity = await MasofaAnaliticReportDbContext.FarmerRecomendationReports.AddAsync(report, cancellationToken);
            var entity = reportEntity.Entity;
            await MasofaAnaliticReportDbContext.SaveChangesAsync(cancellationToken);

            result.ReportIds.Add(entity.Id);

            // 4.2) bids для этого сезона через CropId
            var bids = allBids
                .Where(b => b.CropId == season.CropId)
                .ToList();

            if (bids.Count == 0)
            {
                Logger.LogInformation("No bids for FieldId={FieldId}, SeasonId={SeasonId}, CropId={CropId}",
                    request.FieldId, season.Id, season.CropId);
                return result;
            }

            string? firstJobId = null;

            foreach (var bid in bids)
            {
                if (!bid.FileResultId.HasValue)
                    continue;

                if (!files.TryGetValue(bid.FileResultId.Value, out var fileItem) || fileItem == null)
                    continue;

                var fileName = !string.IsNullOrWhiteSpace(fileItem.FileStoragePath)
                    ? Path.GetFileName(fileItem.FileStoragePath)
                    : $"bid_{bid.Id}.zip";

                try
                {
                    await using var stream = await FileStorageProvider.GetFileStreamAsync(fileItem);

                    Stream uploadStream = stream;
                    await using var temp = stream.CanSeek ? null : new MemoryStream();

                    if (!stream.CanSeek)
                    {
                        await stream.CopyToAsync(temp!, cancellationToken);
                        temp!.Position = 0;
                        uploadStream = temp!;
                    }
                    else
                    {
                        stream.Position = 0;
                    }

                    string? cropType = null;

                    if (season.CropId != null && crops.TryGetValue(season.CropId.Value, out var crop) && crop != null)
                    {
                        cropType = crop.Names["en-US"];

                        cropType = cropType?.Trim().ToLower();
                    }

                    var jobId = await QwenUnitOfWork.QwenRepository.SubmitArchiveAsync(
                        archiveStream: uploadStream,
                        fileName: fileName,
                        express: Express,
                        cropType: cropType
                    );

                    firstJobId ??= jobId;

                    if (!Guid.TryParse(jobId, out var guidJobId))
                        continue;

                    bid.QwenTaskId = guidJobId;
                    bid.QwenAnalysisStart = DateTime.UtcNow;
                    bid.BidState = BidStateType.QwenAnalysisStart;
                    bid.LastUpdateAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed submit bid {BidId} for season {SeasonId}", bid.Id, season.Id);
                }
            }

            report.QwenJobId = firstJobId;
            report.LastUpdateAt = DateTime.UtcNow;

            await MasofaAnaliticReportDbContext.SaveChangesAsync(cancellationToken);
            await MasofaCropMonitoringDbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
