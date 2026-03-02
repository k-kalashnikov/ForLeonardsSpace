using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Minio.DataModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Masofa.Cli.DevopsUtil.Commands.Import
{
    /// <summary>
    /// Импорт демо-результатов: по zip из D:\Bids создаёт завершённые задачи + прикрепляет zip и json анализа.
    /// </summary>
    [BaseCommand("Import Demo Bid Results", "Import Demo Bid Results")]
    public sealed class ImportDemoBidResultsCommand : IBaseCommand
    {
        private const string DefaultBidsFolder = @"D:\Bids";

        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }

        public ImportDemoBidResultsCommand(MasofaCropMonitoringDbContext db, IFileStorageProvider fileStorageProvider, MasofaCommonDbContext masofaCommonDbContext)
        {
            MasofaCropMonitoringDbContext = db;
            FileStorageProvider = fileStorageProvider;
            MasofaCommonDbContext = masofaCommonDbContext;
        }

        public void Dispose()
        {
            Console.WriteLine("\nImportDemoBidResultsCommand END");
        }

        public async Task Execute()
        {
            Console.WriteLine("ImportDemoBidResultsCommand START\n");

            if (!Directory.Exists(DefaultBidsFolder))
                throw new DirectoryNotFoundException(DefaultBidsFolder);

            var zipPaths = Directory
                .EnumerateFiles(DefaultBidsFolder, "bid_*.zip", SearchOption.TopDirectoryOnly)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (zipPaths.Count == 0)
            {
                Console.WriteLine($"No zip files found in {DefaultBidsFolder}");
                return;
            }

            foreach (var zipPath in zipPaths)
            {
                try
                {
                    await ProcessOneZipAsync(zipPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERR] {Path.GetFileName(zipPath)}: {ex.Message}");
                }
            }

            Console.WriteLine("\nDone.");
        }

        public Task Execute(string[] args) => Execute();

        private async Task ProcessOneZipAsync(string zipPath)
        {
            var fileName = Path.GetFileName(zipPath);

            if (!TryParseBidId(fileName, out var oldBidId))
            {
                Console.WriteLine($"[SKIP] invalid filename: {fileName}");
                return;
            }

            var oldBid = await MasofaCropMonitoringDbContext.Bids
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == oldBidId);

            if (oldBid is null)
            {
                Console.WriteLine($"[SKIP] Bid not found: {oldBidId}");
                return;
            }

            var seasons = await MasofaCropMonitoringDbContext.Seasons
                .AsNoTracking()
                .Where(s => s.FieldId == Guid.Parse("019b315c-59f8-7e86-a181-36598069192f") && s.CropId == oldBid.CropId)
                .ToListAsync()
                ?? new List<Season>();

            var field = await MasofaCropMonitoringDbContext.Fields.FirstAsync(f => f.Id == Guid.Parse("019b315c-59f8-7e86-a181-36598069192f"));

            if (!seasons.Any())
            {
                Console.WriteLine($"[SKIP] Season not found for Bid: {oldBidId}");
                return;
            }

            foreach (var season in seasons)
            {
                var sowingStart = season.PlantingDate;
                var sowingEnd = season.HarvestingDate;

                if (sowingStart is null || sowingEnd is null)
                {
                    Console.WriteLine($"[SKIP] Sowing period is empty for Season={season.Id} (Bid={oldBidId})");
                    return;
                }

                var completedAt = PickRandomDateInSecondHalf(sowingStart, sowingEnd);

                var newBid = new Bid
                {
                    FieldId = oldBid.FieldId,
                    CropId = oldBid.CropId,
                    WorkerId = oldBid.WorkerId,
                    ForemanId = oldBid.ForemanId,
                    RegionId = field.RegionId,
                    Polygon = season.Polygon,
                    BidState = BidStateType.Finished,
                    CreateAt = completedAt,
                    LastUpdateAt = completedAt,
                    CreateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                    LastUpdateUser = Guid.Parse("01993e2c-0880-716b-9f41-27686a6a5e0a"),
                    DeadlineDate = completedAt.AddDays(14),
                    StartDate = completedAt.AddDays(13),
                    EndDate = completedAt.AddDays(15),
                    QwenExpressAnalysisStart = completedAt,
                    QwenExpressAnalysisEnd = completedAt,
                    Status = StatusType.Active,
                    Lat = season.Latitude.Value,
                    Lng = season.Longitude.Value
                };

                newBid = (await MasofaCropMonitoringDbContext.Bids.AddAsync(newBid)).Entity;
                await MasofaCropMonitoringDbContext.SaveChangesAsync();

                var zipBytes = await File.ReadAllBytesAsync(zipPath);
                var fileStoragePath = $"bid_{newBid.Id}.zip";
                var bucket = "bidresults";

                await FileStorageProvider.PushFileAsync(zipBytes, fileStoragePath, bucket);

                var resultFile = new FileStorageItem
                {
                    CreateAt = completedAt,
                    LastUpdateAt = completedAt,
                    FileContentType = FileContentType.ArchiveZIP,
                    FileStorageBacket = bucket,
                    FileStoragePath = fileStoragePath,
                    OwnerTypeFullName = typeof(Bid).FullName,
                    OwnerId = newBid.Id,
                    Status = StatusType.Active,
                    CreateUser = Guid.Empty
                };

                resultFile = (await MasofaCommonDbContext.FileStorageItems.AddAsync(resultFile)).Entity;
                await MasofaCommonDbContext.SaveChangesAsync();

                newBid.FileResultId = resultFile.Id;
                MasofaCropMonitoringDbContext.Bids.Update(newBid);

                var dir = Path.GetDirectoryName(zipPath)!;
                var baseName = Path.GetFileNameWithoutExtension(zipPath);
                var expressJsonPath = Path.Combine(dir, $"{baseName}_result.json");

                if (File.Exists(expressJsonPath))
                {
                    newBid.QwenExpressResultJson = await PatchExpressJsonKeepSummaryAsync(expressJsonPath, newBid.Id);
                }
                else
                {
                    Console.WriteLine($"[WARN] Express json not found: {Path.GetFileName(expressJsonPath)}");
                }

                await MasofaCropMonitoringDbContext.SaveChangesAsync();

                Console.WriteLine($"[OK] OldBid={oldBidId} => NewBid={newBid.Id} ({fileStoragePath})");
            }
        }

        private static bool TryParseBidId(string fileName, out Guid bidId)
        {

            var bidStr = fileName.Replace("bid_", string.Empty)
                .Replace(".zip", string.Empty);

            return Guid.TryParse(bidStr, out bidId);
        }

        private static DateTime PickRandomDateInSecondHalf(DateOnly? start, DateOnly? end)
        {
            if (start is null || end is null)
                return DateTime.UtcNow;

            var startDt = start.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var endDt = end.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

            if (endDt <= startDt)
                return endDt;

            var midTicks = startDt.Ticks + ((endDt.Ticks - startDt.Ticks) / 2);
            var from = new DateTime(midTicks, DateTimeKind.Utc);

            var span = (endDt - from).TotalSeconds;
            var add = Random.Shared.NextDouble() * Math.Max(1, span);

            return from.AddSeconds(add);
        }

        private static async Task<string> PatchExpressJsonKeepSummaryAsync(string analysisPath, Guid newBidId)
        {
            var json = await File.ReadAllTextAsync(analysisPath);

            var node = JsonNode.Parse(json);
            if (node is not JsonObject obj)
                return json;

            var newArchiveName = $"bid_{newBidId}.zip";

            if (obj["items"] is JsonArray items)
            {
                foreach (var it in items.OfType<JsonObject>())
                    it["archive_name"] = newArchiveName;
            }

            return obj.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
