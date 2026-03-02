using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Attributes;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Web.Monolith.Converters;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.IO.Compression;

namespace Masofa.Web.Monolith.Jobs.Sentinel2
{
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "Sentinel2ConvertTilesJob", "Sentinel")]
    public class Sentinel2ConvertTilesJob : BaseJob<Sentinel2ConvertTilesJobResult>, IJob
    {
        private ILogger<Sentinel2ConvertTilesJob> Logger { get; set; }
        private MasofaSentinelDbContext SentinelDbContext { get; set; }
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }
        private IMediator Mediator { get; set; }
        private readonly string _outputPath;

        private static readonly Dictionary<ProductQueueStatusType, (string bucket, string folder)> IndexMapping = new()
        {
            //{ ProductQueueStatusType.NdviTiff, ("sentinelndvi", "ndvi") },
            //{ ProductQueueStatusType.GndviTiff, ("sentinelgndvi", "gndvi") },
            //{ ProductQueueStatusType.MndwiTiff, ("sentinelmndwi", "mndwi") },
            //{ ProductQueueStatusType.NdmiTiff, ("sentinelndmi", "ndmi") },
            //{ ProductQueueStatusType.EviTiff, ("sentinelevi", "evi") },
            //{ ProductQueueStatusType.OrviTiff, ("sentinelorvi", "orvi") },
            //{ ProductQueueStatusType.OsaviTiff, ("sentinelosavi", "osavi") },
            //{ ProductQueueStatusType.OsaviTiff, ("sentinelarvi", "arvi") }
        };

        private static readonly Dictionary<ProductQueueStatusType, string> IndexNameMapping = new()
        {
            //{ ProductQueueStatusType.NdviTiff, "NDVI" },
            //{ ProductQueueStatusType.GndviTiff, "GNDVI" },
            //{ ProductQueueStatusType.MndwiTiff, "MNDWI" },
            //{ ProductQueueStatusType.NdmiTiff, "NDMI" },
            //{ ProductQueueStatusType.EviTiff, "EVI" },
            //{ ProductQueueStatusType.OrviTiff, "ORVI" },
            //{ ProductQueueStatusType.OsaviTiff, "OSAVI" }
        };


        public Sentinel2ConvertTilesJob(ILogger<Sentinel2ConvertTilesJob> logger, MasofaSentinelDbContext sentinelDbContext, MasofaCommonDbContext commonDbContext, IFileStorageProvider fileStorageProvider, IConfiguration configuration, IMediator mediator, IBusinessLogicLogger businessLogicLogger, MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            Logger = logger;
            SentinelDbContext = sentinelDbContext;
            CommonDbContext = commonDbContext;
            FileStorageProvider = fileStorageProvider;
            _outputPath = configuration.GetValue<string>("Sentinel:ConvertedTilesPath") ?? string.Empty;
            Mediator = mediator;
        }

        public async Task Execute(IJobExecutionContext context)
        {

            Logger.LogInformation($"Start Sentinel2ConvertTilesJob");

            try
            {
                await ProcessOriginalTiles();

                await ProcessIndexTiles();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unhandled exception in Sentinel2ConvertTilesJob");
                throw; 
            }

            Logger.LogInformation($"End Sentinel2ConvertTilesJob");
        }

        private async Task ProcessOriginalTiles()
        {
            Logger.LogInformation($"Processing original GeoTIFF tiles...");

            try
            {
                var productQueue = await SentinelDbContext.Set<Sentinel2ProductQueue>()
                    .Where(m => m.QueueStatus == ProductQueueStatusType.MetadataLoaded
                                && m.QueueStatus != ProductQueueStatusType.GeoserverImported)
                    .ToListAsync();

                var needProducts = await CommonDbContext.SatelliteProducts
                    .Where(m => productQueue.Select(m => m.ProductId).Contains(m.ProductId))
                    .Where(m => !m.MediadataPath.Equals(Guid.Empty))
                    .ToListAsync();

                var fileStorageItems = await CommonDbContext.FileStorageItems
                    .Where(f => needProducts.Select(p => p.MediadataPath).Contains(f.Id))
                    .ToListAsync();

                var tempFile = Path.GetTempFileName().Replace(".tmp", ".jp2");

                foreach (var pq in productQueue)
                {
                    var product = needProducts.FirstOrDefault(p => p.Id.ToString() == pq.ProductId);
                    if (product == null) continue;

                    var fileStorageItem = fileStorageItems.FirstOrDefault(f => f.Id == product.MediadataPath);
                    if (fileStorageItem == null) continue;

                    //var fileStream = await FileStorageProvider.GetFileStreamAsync(fileStorageItem);
                    using (var archiveStream = await FileStorageProvider.GetFileStreamAsync(fileStorageItem))
                    {
                        using (var zip = new ZipArchive(archiveStream, ZipArchiveMode.Read))
                        {

                            var entries = zip.Entries
                                .Where(e => e.FullName.Contains("GRANULE") &&
                                            e.FullName.Contains("IMG_DATA") &&
                                            e.Name.EndsWith("_TCI.jp2", StringComparison.OrdinalIgnoreCase))
                                .ToList();

                            foreach (var entry in entries)
                            {
                                //results.Add((zipPath, entry.FullName));
                                using (var input = entry.Open())
                                using (var output = File.Create(tempFile))
                                {
                                    input.CopyTo(output);
                                }

                                if (!IndexNameMapping.TryGetValue(pq.QueueStatus, out var sourceType))
                                {
                                    Logger.LogWarning($"No display name mapping for status: {pq.QueueStatus}");
                                    continue;
                                }

                                var date = product.CreateAt;
                                var sourceName = product.ProductSourceType;
                                var folderName = $"{sourceName}_{sourceType}_{date:MM}_{date:dd}";
                                var targetFolder = Path.Combine(_outputPath, folderName);
                                Directory.CreateDirectory(targetFolder);

                                var outputFileName = entry.Name.Replace(".jp2", ".tiff");
                                var outputPath = Path.Combine(targetFolder, outputFileName);

                                if (File.Exists(outputPath))
                                {
                                    Logger.LogDebug($"Original TIFF already exists, skipping: {outputPath}");
                                }
                                else
                                {
                                    var convertRequest = new ConvertJp2ToGeotiffCommand()
                                    {
                                        InputPath = tempFile,
                                        OutputPath = outputPath,
                                    };

                                    var result = await Mediator.Send(convertRequest);
                                }
                            }
                        }
                    }
                    pq.QueueStatus = ProductQueueStatusType.GeoserverImported;
                    await CommonDbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Logger.LogInformation("Original tiles processing completed.");
        }

        private async Task ProcessIndexTiles()
        {
            Logger.LogInformation("Processing index GeoTIFF tiles...");

            var indexProductQueue = await SentinelDbContext.Set<Sentinel2ProductQueue>()
                .Where(m => IndexMapping.Keys.Contains(m.QueueStatus) && m.QueueStatus != ProductQueueStatusType.GeoserverImported)
                .ToListAsync();

            if (!indexProductQueue.Any())
            {
                Logger.LogInformation("No index tiles to process.");
                return;
            }

            var productIds = indexProductQueue.Select(q => q.ProductId).ToList();
            var needProducts = await CommonDbContext.SatelliteProducts
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.Id);

            if (!needProducts.Any())
            {
                Logger.LogWarning("No SatelliteProducts found for index queues.");
                return;
            }

            foreach (var pq in indexProductQueue)
            {
                try
                {
                    if (!Guid.TryParse(pq.ProductId, out Guid productIdGuid))
                    {
                        Logger.LogWarning($"Invalid ProductId format: {pq.ProductId}");
                        continue;
                    }

                    if (!needProducts.TryGetValue(productIdGuid, out var product))
                    {
                        Logger.LogWarning($"SatelliteProduct not found for ProductId: {pq.ProductId}");
                        continue;
                    }

                    if (!IndexMapping.TryGetValue(pq.QueueStatus, out var mapping))
                    {
                        Logger.LogWarning($"No bucket/folder mapping for status: {pq.QueueStatus}");
                        continue;
                    }

                    if (!IndexNameMapping.TryGetValue(pq.QueueStatus, out var sourceType))
                    {
                        Logger.LogWarning($"No display name mapping for status: {pq.QueueStatus}");
                        continue;
                    }

                    var (bucketName, _) = mapping;

                    var date = product.CreateAt;
                    var sourceName = product.ProductSourceType;
                    var folderName = $"{sourceName}_{sourceType}_{date:MM}_{date:dd}";
                    var targetFolder = Path.Combine(_outputPath, folderName);
                    Directory.CreateDirectory(targetFolder);

                    var fileStorageItems = await CommonDbContext.FileStorageItems
                        .Where(f =>
                            f.OwnerId == product.Id &&
                            f.OwnerTypeFullName == typeof(SatelliteProduct).FullName &&
                            f.FileStorageBacket == bucketName)
                        .ToListAsync();

                    if (!fileStorageItems.Any())
                    {
                        Logger.LogWarning($"No FileStorageItems found for product {product.Id} in bucket {bucketName}");
                        continue;
                    }

                    Logger.LogInformation($"Copying {fileStorageItems.Count} file(s) for product {pq.ProductId} to folder '{folderName}'");

                    foreach (var fileStorageItem in fileStorageItems)
                    {
                        var targetFileName = Path.GetFileName(fileStorageItem.FileStoragePath);
                        var targetPath = Path.Combine(targetFolder, targetFileName);

                        if (File.Exists(targetPath))
                        {
                            Logger.LogDebug($"Index TIFF already exists, skipping: {targetPath}");
                            continue;
                        }

                        Logger.LogDebug($"Copying: {fileStorageItem.FileStorageBacket}/{fileStorageItem.FileStoragePath} → {targetPath}");

                        await CopyFileFromMinioToLocalByFileStorageItem(fileStorageItem, targetPath);
                    }

                    pq.QueueStatus = ProductQueueStatusType.GeoserverImportedIndex;
                    Logger.LogInformation($"Successfully processed {fileStorageItems.Count} files for product {pq.ProductId} ({pq.QueueStatus})");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Failed to process index tile for ProductQueue {pq.Id} with status {pq.QueueStatus}");
                }
            }

            await CommonDbContext.SaveChangesAsync();
            Logger.LogInformation("Index tiles processing completed.");
        }

        private async Task CopyFileFromMinioToLocalByFileStorageItem(FileStorageItem fileStorageItem, string targetPath)
        {
            var directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var sourceStream = await FileStorageProvider.GetFileStreamAsync(fileStorageItem);
            using var targetStream = File.Create(targetPath);

            await sourceStream.CopyToAsync(targetStream);

            Logger.LogInformation($"Copied file to: {targetPath}");
        }
    }

    public class Sentinel2ConvertTilesJobResult : BaseJobResult
    {

    }
}
