using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Attributes;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO.Compression;
using System.Linq.Expressions;

namespace Masofa.Web.Monolith.Jobs.Sentinel2
{
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "Sentinel2PreviewImageJob", "Sentinel")]
    public class Sentinel2PreviewImageJob : BaseJob<PreviewImageJobResult>, IJob
    {
        private readonly ILogger<Sentinel2PreviewImageJob> Logger;
        private readonly IFileStorageProvider FileStorageProvider;
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private MasofaIndicesDbContext IndicesDbContext { get; set; }
        private MasofaSentinelDbContext SentinelDbContext { get; set; }

        public Sentinel2PreviewImageJob(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger<Sentinel2PreviewImageJob> logger, IFileStorageProvider fileStorageProvider, MasofaCommonDbContext masofaCommonDbContext, MasofaSentinelDbContext masofaSentinelDbContext, MasofaIdentityDbContext identityDbContext, MasofaIndicesDbContext masofaIndicesDbContext) : base(mediator, businessLogicLogger, logger, masofaCommonDbContext, identityDbContext)
        {
            Logger = logger;
            FileStorageProvider = fileStorageProvider;
            CommonDbContext = masofaCommonDbContext;
            SentinelDbContext = masofaSentinelDbContext;
            IndicesDbContext = masofaIndicesDbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation("Start Sentinel2PreviewImageJob");

            try
            {
                var productQueue = await SentinelDbContext.Set<Sentinel2ProductQueue>()
                    .Where(m => m.QueueStatus == ProductQueueStatusType.IndicesComplite)
                    .ToListAsync();

                var needProducts = await CommonDbContext.SatelliteProducts
                    .Where(m => productQueue.Select(m => m.ProductId).Contains(m.ProductId))
                    .Where(m => m.MediadataPath != Guid.Empty)
                    .Take(100)
                    .ToListAsync();

                Logger.LogInformation($"Found {needProducts.Count} products to process.");

                foreach (var product in needProducts)
                {
                    string tempExtractPath = null;

                    try
                    {
                        Logger.LogInformation($"Processing product: {product.ProductId}");

                        var zipFileStorageItem = await CommonDbContext.FileStorageItems
                            .FirstOrDefaultAsync(f => f.Id == product.MediadataPath);

                        if (zipFileStorageItem == null)
                        {
                            Logger.LogWarning($"FileStorageItem not found for MediadataPath: {product.MediadataPath}");
                            continue;
                        }

                        var zipStream = await FileStorageProvider.GetFileStreamAsync(zipFileStorageItem);
                        string tempZipPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{product.ProductId}.zip");
                        string tempExtractRoot = Path.Combine(Path.GetTempPath(), $"extract_{Guid.NewGuid()}");

                        Directory.CreateDirectory(tempExtractRoot);
                        tempExtractPath = tempExtractRoot;

                        using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
                        {
                            await zipStream.CopyToAsync(fileStream);
                        }
                        Logger.LogInformation($"Downloaded ZIP to: {tempZipPath}");

                        ZipFile.ExtractToDirectory(tempZipPath, tempExtractRoot);
                        Logger.LogInformation($"Extracted ZIP to: {tempExtractRoot}");

                        var rootDirectories = Directory.GetDirectories(tempExtractRoot);
                        if (rootDirectories.Length == 0)
                        {
                            Logger.LogWarning($"No directory found in extracted folder: {tempExtractRoot}");
                            return;
                        }

                        string safeFolder = rootDirectories[0];
                        Logger.LogInformation($"Found SAFE folder: {safeFolder}");

                        string safeFolderName = Path.GetFileNameWithoutExtension(safeFolder);
                        string qJpgPath = Path.Combine(safeFolder, $"{safeFolderName}-ql.jpg");

                        if (!File.Exists(qJpgPath))
                        {
                            Logger.LogWarning($"Quicklook file not found: {qJpgPath}");
                            continue;
                        }

                        Logger.LogInformation($"Found quicklook image: {qJpgPath}");

                        using var image = await Image.LoadAsync<Rgba32>(qJpgPath);
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(250, 250),
                            Mode = ResizeMode.Max
                        }));

                        string previewImagePath = Path.Combine(Path.GetTempPath(), $"{product.ProductId}_preview.png");
                        await image.SaveAsPngAsync(previewImagePath);

                        Logger.LogInformation($"Preview image saved to: {previewImagePath}");

                        var previewBucketName = "sentinelpreviewimage";
                        var previewObjectName = $"{product.ProductId}.png";

                        using var previewFileStream = File.OpenRead(previewImagePath);
                        string minioPreviewfPath = await FileStorageProvider.PushFileAsync(previewFileStream, previewObjectName, previewBucketName);
                        Logger.LogInformation($"Preview uploaded to MinIO: {previewBucketName}/{previewObjectName}");
                        var previewFileLength = new FileInfo(previewImagePath).Length;

                        var tiffFileStorageItem = new FileStorageItem()
                        {
                            CreateAt = DateTime.UtcNow,
                            CreateUser = Guid.Empty,
                            OwnerId = product.Id,
                            OwnerTypeFullName = typeof(SatelliteProduct).FullName,
                            FileContentType = FileContentType.ImagePNG,
                            Status = StatusType.Active,
                            FileStoragePath = minioPreviewfPath,
                            FileStorageBacket = previewBucketName,
                            FileLength = previewFileLength,
                        };

                        tiffFileStorageItem = (await CommonDbContext.FileStorageItems.AddAsync(tiffFileStorageItem)).Entity;

                        product.PreviewImagePath = tiffFileStorageItem.Id;

                        await CommonDbContext.SaveChangesAsync();

                        var sentinelProduct = await SentinelDbContext.Sentinel2Products
                            .FirstOrDefaultAsync(s => s.SatellateProductId == product.ProductId);

                        if (sentinelProduct == null)
                        {
                            Logger.LogWarning($"Sentinel2Product not found for ProductId: {product.ProductId}");
                            continue;
                        }

                        var queueItem = await SentinelDbContext.Set<Sentinel2ProductQueue>()
                            .FirstOrDefaultAsync(q => q.ProductId == product.ProductId);

                        if (queueItem != null)
                        {
                            queueItem.QueueStatus = ProductQueueStatusType.PreviewGenerated;
                            await SentinelDbContext.SaveChangesAsync();
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"Error processing product {product.ProductId}");
                    }
                    finally
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(tempExtractPath) && Directory.Exists(tempExtractPath))
                            {
                                Directory.Delete(tempExtractPath, true);
                                Logger.LogInformation($"Cleaned up temp extract path: {tempExtractPath}");
                            }

                            string tempZipPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{product.ProductId}.zip");
                        }
                        catch (Exception cleanupEx)
                        {
                            Logger.LogWarning(cleanupEx, "Failed to clean up temporary files.");
                        }
                    }

                    await ProcessIndicesPreviews(product.Id, product.ProductId);

                    Result.SuccessCount++;
                }

                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success
                }, context);
            }
            catch (Exception ex)
            {
                Result.Errors.Add(ex.Message);
                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed
                }, context);
            }

            Logger.LogInformation("End Sentinel2PreviewImageJob");
        }

        private async Task ProcessIndicesPreviews(Guid id, string? productId)
        {
            if (productId == null)
            {
                return;
            }

            string[] indices = ["ARVI", "EVI", "GNDVI", "MNDWI", "NDMI", "NDVI", "ORVI", "OSAVI"];
            foreach (var index in indices)
            {
                string typeName = $"{index[0].ToString().ToUpper()}{index[1..].ToLower()}Polygon";
                var dbSet = IndicesDbContext.GetQueryableSet(typeName);

                var type = FindTypeByName(typeName);

                if (type == null)
                {
                    continue;
                }

                var prodGuid = new Guid(productId);
                var indxs = await IndicesDbContext.ArviPolygons.Where(i => i.SatelliteProductId == prodGuid).ToListAsync();

                var param = Expression.Parameter(type, "h");

                var prop1 = Expression.Property(param, "SatelliteProductId");
                var value1 = Expression.Constant(prodGuid);
                var condition1 = Expression.Equal(prop1, value1);

                var predicate = Expression.Lambda(condition1, param);

                var whereMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(type);

                var filteredQuery = (IQueryable)whereMethod.Invoke(null, [dbSet, predicate]);

                var listMethod = typeof(Enumerable).GetMethod("ToList")!
                    .MakeGenericMethod(type);

                if (listMethod.Invoke(null, [filteredQuery]) is IEnumerable<BaseIndexPolygon> result)
                {
                    foreach (var item in result)
                    {
                        try
                        {
                            var isColored = item.IsColored;
                            var fsi = await CommonDbContext.FileStorageItems.FirstOrDefaultAsync(f => f.Id == item.FileStorageItemId);
                            if (fsi == null)
                            {
                                continue;
                            }

                            var fileBytes = await FileStorageProvider.GetFileBytesAsyncWithProgress(fsi);

                            await using var inputStream = new MemoryStream(fileBytes);

                            using var image = await Image.LoadAsync<Rgba32>(inputStream);
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(250, 250),
                                Mode = ResizeMode.Max
                            }));

                            string previewImagePath = Path.Combine(Path.GetTempPath(), $"{productId}_{index}{(isColored ? "_Colored" : "")}_preview.png");
                            await image.SaveAsPngAsync(previewImagePath);

                            Logger.LogInformation($"{index} index preview image saved to: {previewImagePath}");

                            var previewBucketName = $"sentinel{index.ToLower()}{(isColored ? "colored" : "")}preview";
                            var previewObjectName = $"{productId}.png";

                            using var previewFileStream = File.OpenRead(previewImagePath);
                            string minioPreviewfPath = await FileStorageProvider.PushFileAsync(previewFileStream, previewObjectName, previewBucketName);
                            Logger.LogInformation($"Preview uploaded to MinIO: {previewBucketName}/{previewObjectName}");
                            var previewFileLength = new FileInfo(previewImagePath).Length;

                            var tiffFileStorageItem = new FileStorageItem()
                            {
                                CreateAt = DateTime.UtcNow,
                                CreateUser = Guid.Empty,
                                OwnerId = id,
                                OwnerTypeFullName = typeof(SatelliteProduct).FullName,
                                FileContentType = FileContentType.ImagePNG,
                                Status = StatusType.Active,
                                FileStoragePath = minioPreviewfPath,
                                FileStorageBacket = previewBucketName,
                                FileLength = previewFileLength,
                            };

                            tiffFileStorageItem = (await CommonDbContext.FileStorageItems.AddAsync(tiffFileStorageItem)).Entity;

                            item.PreviewImagePath = tiffFileStorageItem.Id;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, $"Error in {index} index preview create for product = {productId}");
                        }
                    }

                    await IndicesDbContext.SaveChangesAsync();
                }

            }
        }

        public static Type? FindTypeByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(name);
                if (type != null)
                {
                    return type;
                }

                type = assembly.GetTypes().FirstOrDefault(t => t.Name == name);

                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }

    public class PreviewImageJobResult : BaseJobResult
    {
        public int SuccessCount { get; set; } = 0;
    }
}
