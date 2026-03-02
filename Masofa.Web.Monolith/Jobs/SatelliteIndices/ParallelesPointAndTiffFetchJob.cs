using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Attributes;
using Masofa.Common.Helper;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Web.Monolith.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Quartz;
using System.IO.Compression;

namespace Masofa.Web.Monolith.Jobs.SatelliteIndices
{
    /// <summary>
    /// Работа для расчета индексов и тифов к ним
    /// </summary>
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "ParallelesPointAndTiffFetchJob", "Sentinel")]
    public class ParallelesPointAndTiffFetchJob : BaseJob<ParallelesPointAndTiffFetchJobbResul>, IJob
    {
        //ILogger<ParallelesPointAndTiffFetchJob> Logger { get; }
        //private SentinelServiceOptions SentinelServiceOptions { get; }
        private MasofaSentinelDbContext SentinelDbContext { get; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private GdalInitializer GdalInitializer { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }

        private static bool GdalInitialized = false;

        public ParallelesPointAndTiffFetchJob(IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            ILogger<ParallelesPointAndTiffFetchJob> logger,
            //SentinelServiceOptions sentinelServiceOptions, 
            MasofaSentinelDbContext sentinelDbContext,
            MasofaCommonDbContext masofaCommonDbContext,
            MasofaIndicesDbContext masofaIndicesDbContext,
            MasofaCropMonitoringDbContext masofaCropMonitoringDbContext,
            IFileStorageProvider fileStorageProvider,
            GdalInitializer gdalInitializer,
            MasofaIdentityDbContext masofaIdentityDbContext) : base(mediator, businessLogicLogger, logger, masofaCommonDbContext, masofaIdentityDbContext)
        {
            //SentinelServiceOptions = sentinelServiceOptions;
            SentinelDbContext = sentinelDbContext;
            MasofaCommonDbContext = masofaCommonDbContext;
            MasofaIndicesDbContext = masofaIndicesDbContext;
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            FileStorageProvider = fileStorageProvider;
            GdalInitializer = gdalInitializer;

            GdalInitializer.Initialize();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {

                Logger.LogInformation("ParallelesPointAndTiffFetchJob Job started.");

                var productQueue = await SentinelDbContext.Sentinel2ProductsQueue
                    .Where(q => q.QueueStatus == ProductQueueStatusType.Parsed)
                    .OrderBy(q => q.CreateAt)
                    .Take(100)
                    .ToListAsync();

                if (!productQueue.Any())
                {
                    Logger.LogInformation("No products with PreviewGenerated status in queue.");
                    return;
                }

                var queueIds = productQueue.Select(q => q.Id).ToList();

                var existingStatuses = await SentinelDbContext.Sentinel2GenerateIndexStatus
                    .Where(s => queueIds.Contains(s.Sentinel2ProductQueue))
                    .ToListAsync();

                var existingQueueIds = existingStatuses
                    .Select(s => s.Sentinel2ProductQueue)
                    .ToHashSet();

                var newStatuses = new List<Sentinel2GenerateIndexStatus>();
                foreach (var q in productQueue)
                {
                    if (existingQueueIds.Contains(q.Id))
                        continue;

                    newStatuses.Add(new Sentinel2GenerateIndexStatus
                    {
                        Id = Guid.NewGuid(),
                        Sentinel2ProductQueue = q.Id
                    });
                }

                if (newStatuses.Any())
                {
                    await SentinelDbContext.Sentinel2GenerateIndexStatus.AddRangeAsync(newStatuses);
                    await SentinelDbContext.SaveChangesAsync();
                    existingStatuses.AddRange(newStatuses);
                }

                var statusesForDb = existingStatuses
                    .Where(s => !s.IsDbComplite)
                    .ToList();

                var statusesForTiff = existingStatuses
                    .Where(s => !s.IsTiffComplite)
                    .ToList();

                if (!statusesForDb.Any() && !statusesForTiff.Any())
                {
                    Logger.LogInformation("No pending indices (Db/Tiff) for current queue batch.");
                    return;
                }

                var productIds = productQueue.Select(q => q.ProductId).ToList();

                var needProducts = await MasofaCommonDbContext.SatelliteProducts
                    .Where(p => productIds.Contains(p.ProductId))
                    .Where(p => p.MediadataPath != Guid.Empty)
                    .ToListAsync();

                Logger.LogInformation($"Found {needProducts.Count} products to process in this batch.");

                if (!needProducts.Any())
                {
                    Logger.LogInformation("No SatelliteProducts with media data for current queue batch.");
                    return;
                }

                var needProductIds = needProducts.Select(p => p.ProductId).ToList();
                var queueForFoundProducts = productQueue
                    .Where(q => needProductIds.Contains(q.ProductId))
                    .ToList();

                var satelliteProductsWithFields = GetSatelliteProductWithFields(needProducts);

                if (statusesForDb.Any())
                {
                    await GeneratePoints(satelliteProductsWithFields, needProducts, queueForFoundProducts, statusesForDb, 3);
                }

                if (statusesForTiff.Any())
                {
                    await GenerateTiffs(needProducts, queueForFoundProducts, statusesForTiff, 3);
                }

                Logger.LogInformation("ParallelesPointAndTiffFetchJob Job ended.");
            }
            catch (Exception ex)
            {
                Result.Errors.Add($"Error processing: {ex.Message}");
                Logger.LogError($"Error processing: {ex.Message}", ex);
            }
        }

        #region Tiff
        private async Task<List<Sentinel2ProductQueue>> GenerateTiffs(List<SatelliteProduct> needProducts,
            List<Sentinel2ProductQueue> needProductsQueue,
            List<Sentinel2GenerateIndexStatus> productQueueStatuses, int outputLine)
        {
            string TempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(TempPath);
            var result = new List<Sentinel2ProductQueue>();
            Logger.LogInformation(outputLine, "GenerateTiffProducts", $"Found {needProducts.Count} products to process.");
            var index = -1;


            foreach (var product in needProducts)
            {
                index++;
                Logger.LogInformation(outputLine, "GenerateTiffProducts", $"Start for {index} of {needProducts.Count} product");

                string tempExtractPath = null;
                var satelliteMetadata = await SentinelDbContext.Sentinel2ProductsMetadata.FirstOrDefaultAsync(s => s.ProductId == product.Id.ToString());
                if (satelliteMetadata?.OriginDate == null)
                {
                    continue;
                }
                DateTime originalDate = (DateTime)satelliteMetadata.OriginDate;

                try
                {
                    var zipFileStorageItem = await MasofaCommonDbContext.FileStorageItems
                        .FirstOrDefaultAsync(f => f.Id == product.MediadataPath);
                    if (zipFileStorageItem == null)
                    {
                        Logger.LogInformation(outputLine, "GenerateTiffProducts", $"FileStorageItem not found for MediadataPath: {product.MediadataPath}");
                        continue;
                    }

                    Logger.LogInformation(outputLine, "GenerateTiffProducts", $"Start download : {zipFileStorageItem.FileStoragePath}");
                    var zipStream = await FileStorageProvider.GetFileStreamAsyncWithProgress(zipFileStorageItem);
                    string tempZipPath = Path.Combine(TempPath, $"{Guid.NewGuid()}_{product.ProductId}.zip");
                    string tempExtractRoot = Path.Combine(TempPath, $"extract_{Guid.NewGuid()}");
                    Directory.CreateDirectory(tempExtractRoot);
                    tempExtractPath = tempExtractRoot;

                    using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
                    {
                        await zipStream.CopyToAsync(fileStream);
                    }

                    Logger.LogInformation(outputLine, "GenerateTiffProducts", $"Downloaded ZIP to: {tempZipPath}");
                    ZipFile.ExtractToDirectory(tempZipPath, tempExtractRoot);
                    Logger.LogInformation(outputLine, "GenerateTiffProducts", $"Extracted ZIP to: {tempExtractRoot}");

                    var imgDataPath = IndicesHelper.FindImgDataPath(tempExtractRoot);
                    if (string.IsNullOrEmpty(imgDataPath))
                    {
                        Logger.LogInformation(outputLine, "GenerateTiffProducts", "IMG_DATA path could not be determined. Skipping product.");
                        continue;
                    }

                    //var b02Files = Directory.GetFiles(imgDataPath, "*B02.jp2", SearchOption.TopDirectoryOnly).ToList();
                    //var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
                    //var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
                    //var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();
                    //var b11Files = Directory.GetFiles(imgDataPath, "*B11.jp2", SearchOption.TopDirectoryOnly).ToList();

                    var needProductQueueItem = needProductsQueue.First(n => n.ProductId == product.ProductId);
                    var productStatus = productQueueStatuses.First(ps => ps.Sentinel2ProductQueue == needProductQueueItem.Id);
                    var generateResult = GenerateOneProductTiff(needProductQueueItem, productStatus, imgDataPath);
                    var productEntity = SentinelDbContext.Sentinel2Products.First(m => m.SatellateProductId == product.Id.ToString());
                    var productInspire = SentinelDbContext.SentinelInspireMetadata.First(m => m.Id == productEntity.SentinelInspireMetadataId);
                    foreach (var item in generateResult)
                    {
                        try
                        {
                            var tempItem = item;
                            tempItem.Value.ProductId = needProductQueueItem.ProductId;
                            tempItem.Value.SatelliteProductId = product.Id;
                            tempItem.Value.OriginDate = new DateTime(productInspire.DateStamp.Year, productInspire.DateStamp.Month, productInspire.DateStamp.Day, productInspire.DateStamp.Hour, productInspire.DateStamp.Minute, productInspire.DateStamp.Second, DateTimeKind.Utc);
                            await SendToMinioTiffActions(tempItem.Key, tempItem.Value);
                            typeof(Sentinel2GenerateIndexStatus).GetProperty(tempItem.Key).SetValue(productStatus, true);
                            Logger.LogInformation(outputLine, "GenerateTiffProducts", $"Complite {tempItem.Key} for {needProductQueueItem.ProductId}");
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(outputLine, "GenerateTiffProducts", $"Exception: {ex.Message}");
                        }

                    }
                    SentinelDbContext.Sentinel2GenerateIndexStatus.Update(productStatus);
                    SentinelDbContext.SaveChanges();

                }
                catch (Exception ex)
                {
                    Logger.LogError(outputLine, "GenerateTiffProducts", $"Exception: {ex.Message}");
                    continue;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempExtractPath) && Directory.Exists(tempExtractPath))
                        {
                            Directory.Delete(tempExtractPath, true);
                            Logger.LogInformation(outputLine, "GenerateTiffProducts", $"Cleaned up temp extract path: {tempExtractPath}");
                        }
                    }
                    catch (Exception cleanupEx)
                    {
                        Logger.LogError(outputLine, "GenerateTiffProducts", $"Cleanup error: {cleanupEx.Message}");
                    }
                }
            }

            Logger.LogInformation(outputLine, "GenerateTiffProducts", "ARVIJob completed successfully.");
            return result;
        }

        private Dictionary<string, TiffGenerationResult> GenerateOneProductTiff(Sentinel2ProductQueue needProduct, Sentinel2GenerateIndexStatus productStatus, string archiveFolder)
        {
            var tempProductStatus = productStatus;
            var result = new Dictionary<string, TiffGenerationResult>();
            if (tempProductStatus == null)
            {
                tempProductStatus = new Sentinel2GenerateIndexStatus()
                {
                    Id = Guid.NewGuid(),
                    Sentinel2ProductQueue = needProduct.Id
                };
            }
            var indicexFlags = typeof(Sentinel2GenerateIndexStatus).GetProperties()
                .Where(m => m.Name.Contains("Tiff"))
                .ToList();

            var generateTasks = new List<Task>();

            foreach (var item in indicexFlags)
            {
                if (((bool)item.GetValue(productStatus)) == true)
                {
                    continue;
                }
                var indexName = item.Name.Replace("Tiff", string.Empty);
                var indexMethod = typeof(IndicesHelper).GetMethod($"ArchiveFolderWork{indexName.ToUpper()}");
                if (indexMethod == null)
                {
                    continue;
                }
                var indexResult = (TiffGenerationResult)indexMethod.Invoke(null, [archiveFolder, needProduct.ProductId]);
                result.Add(item.Name, indexResult);
            }

            return result;
        }

        private async Task SendToMinioTiff<TIndexModel>(string arviTiffPath, string tiffFileName, string tiffBucket, Guid Id, string ProductId, DateTime originalDate, bool isColored)
                where TIndexModel : BaseIndexPolygon
        {
            using (var tiffStream = File.OpenRead(arviTiffPath))
            {
                string minioTiffPath = await FileStorageProvider.PushFileAsync(tiffStream, tiffFileName, tiffBucket);
                var fileLength = new FileInfo(arviTiffPath).Length;

                var tiffFileStorageItem = new FileStorageItem()
                {
                    CreateAt = DateTime.UtcNow,
                    CreateUser = Guid.Empty,
                    OwnerId = Id,
                    OwnerTypeFullName = typeof(SatelliteProduct).FullName,
                    FileContentType = FileContentType.ImageTiff,
                    Status = StatusType.Active,
                    FileStoragePath = minioTiffPath,
                    FileStorageBacket = tiffBucket,
                    FileLength = fileLength,
                };

                tiffFileStorageItem = (await MasofaCommonDbContext.FileStorageItems.AddAsync(tiffFileStorageItem)).Entity;
                await MasofaCommonDbContext.SaveChangesAsync();

                var sentinelProduct = await SentinelDbContext.Sentinel2Products
                    .FirstOrDefaultAsync(s => s.SatellateProductId == Id.ToString());

                if (sentinelProduct == null)
                {
                    Logger.LogInformation($"Sentinel2Product not found for ProductId: {Id.ToString()}");
                    throw new ArgumentException($"SatellateProductId with Id - {Id.ToString()} is not found", "Id");
                }

                var sentinelProductMetadata = await SentinelDbContext.SentinelInspireMetadata
                    .FirstOrDefaultAsync(m => m.Id == sentinelProduct.SentinelInspireMetadataId);

                if (sentinelProductMetadata == null)
                {
                    Logger.LogInformation($"SentinelInspireMetadata not found for Id: {sentinelProduct.SentinelInspireMetadataId}");
                    throw new ArgumentException($"SentinelInspireMetadata with Id  - {Id.ToString()} is not found", "Id");

                }

                var factory = new GeometryFactory();

                var west = (double)sentinelProductMetadata.WestBoundLongitude;
                var east = (double)sentinelProductMetadata.EastBoundLongitude;
                var south = (double)sentinelProductMetadata.SouthBoundLatitude;
                var north = (double)sentinelProductMetadata.NorthBoundLatitude;

                var coordinates = new[]
                {
                    new Coordinate(west, north),  // Северо-Запад
                    new Coordinate(east, north),  // Северо-Восток
                    new Coordinate(east, south),  // Юго-Восток
                    new Coordinate(west, south),  // Юго-Запад
                    new Coordinate(west, north)   // Замыкаем на первую
                };

                var linearRing = factory.CreateLinearRing(coordinates);
                var polygonWgs84 = factory.CreatePolygon(linearRing);
                var polygonEntity = Activator.CreateInstance<TIndexModel>();

                polygonEntity.CreateAt = originalDate;
                polygonEntity.ProductSourceType = ProductSourceType.Sentinel2;
                polygonEntity.SatelliteProductId = Id;
                polygonEntity.FileStorageItemId = tiffFileStorageItem.Id;
                polygonEntity.Polygon = polygonWgs84;
                polygonEntity.IsColored = isColored;


                await MasofaIndicesDbContext.Set<TIndexModel>().AddAsync(polygonEntity);
                await MasofaIndicesDbContext.SaveChangesAsync();
            }
        }
        private async Task SendToMinioTiffActions(string key, TiffGenerationResult generationResult)
        {
            switch (key)
            {
                case "ArviTiff":
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.ArviPolygon>(generationResult.GrayTiffLocalPath, generationResult.GrayTiffFileName, generationResult.GrayTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, false);
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.ArviPolygon>(generationResult.ColorTiffLocalPath, generationResult.ColorTiffLocalPath, generationResult.ColorTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, true);
                    return;
                case "EviTiff":
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.EviPolygon>(generationResult.GrayTiffLocalPath, generationResult.GrayTiffFileName, generationResult.GrayTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, false);
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.EviPolygon>(generationResult.ColorTiffLocalPath, generationResult.ColorTiffLocalPath, generationResult.ColorTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, true);
                    return;
                case "GndviTiff":
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.GndviPolygon>(generationResult.GrayTiffLocalPath, generationResult.GrayTiffFileName, generationResult.GrayTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, false);
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.GndviPolygon>(generationResult.ColorTiffLocalPath, generationResult.ColorTiffLocalPath, generationResult.ColorTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, true);
                    return;
                case "MndwiTiff":
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.MndwiPolygon>(generationResult.GrayTiffLocalPath, generationResult.GrayTiffFileName, generationResult.GrayTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, false);
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.MndwiPolygon>(generationResult.ColorTiffLocalPath, generationResult.ColorTiffLocalPath, generationResult.ColorTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, true);
                    return;
                case "NdmiTiff":
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.NdmiPolygon>(generationResult.GrayTiffLocalPath, generationResult.GrayTiffFileName, generationResult.GrayTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, false);
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.NdmiPolygon>(generationResult.ColorTiffLocalPath, generationResult.ColorTiffLocalPath, generationResult.ColorTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, true);
                    return;
                case "NdviTiff":
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.NdviPolygon>(generationResult.GrayTiffLocalPath, generationResult.GrayTiffFileName, generationResult.GrayTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, false);
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.NdviPolygon>(generationResult.ColorTiffLocalPath, generationResult.ColorTiffLocalPath, generationResult.ColorTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, true);
                    return;
                case "NdwiTiff":
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.NdwiPolygon>(generationResult.GrayTiffLocalPath, generationResult.GrayTiffFileName, generationResult.GrayTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, false);
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.NdwiPolygon>(generationResult.ColorTiffLocalPath, generationResult.ColorTiffLocalPath, generationResult.ColorTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, true);
                    return;
                case "OrviTiff":
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.OrviPolygon>(generationResult.GrayTiffLocalPath, generationResult.GrayTiffFileName, generationResult.GrayTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, false);
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.OrviPolygon>(generationResult.ColorTiffLocalPath, generationResult.ColorTiffLocalPath, generationResult.ColorTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, true);
                    return;
                case "OsaviTiff":
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.OsaviPolygon>(generationResult.GrayTiffLocalPath, generationResult.GrayTiffFileName, generationResult.GrayTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, false);
                    await SendToMinioTiff<Masofa.Common.Models.Satellite.Indices.OsaviPolygon>(generationResult.ColorTiffLocalPath, generationResult.ColorTiffLocalPath, generationResult.ColorTiffBucket, generationResult.SatelliteProductId, generationResult.ProductId, generationResult.OriginDate, true);
                    return;
                default:
                    return;
            }

        }
        #endregion

        #region SupportMethods
        private Dictionary<Guid, List<Common.Models.CropMonitoring.Field>> GetSatelliteProductWithFields(List<SatelliteProduct> sourceProduct)
        {
            var satelliteProductsWithFields = new Dictionary<Guid, List<Common.Models.CropMonitoring.Field>>();

            try
            {
                var satelliteProducts = sourceProduct
                    .Where(m => m.ProductSourceType == ProductSourceType.Sentinel2)
                    .Where(m => m.Polygon != null)
                    .ToList();

                var fields = MasofaCropMonitoringDbContext.Fields
                    .ToList();

                var neededFields = fields
                    .Where(f => satelliteProducts.Select(sp => sp.Polygon).Any(sp => sp.Contains(f.Polygon) || sp.Intersects(f.Polygon)))
                    .ToList();


                foreach (var satelliteProduct in satelliteProducts)
                {
                    var tempFields = new List<Common.Models.CropMonitoring.Field>();
                    foreach (var field in neededFields)
                    {
                        if (satelliteProduct.Polygon.Contains(field.Polygon) || satelliteProduct.Polygon.Intersects(field.Polygon))
                        {
                            tempFields.Add(field);
                            continue;
                        }
                    }
                    if (tempFields.Any())
                    {
                        if (!satelliteProductsWithFields.ContainsKey(satelliteProduct.Id))
                        {
                            satelliteProductsWithFields.Add(satelliteProduct.Id, new List<Common.Models.CropMonitoring.Field>());
                        }
                        satelliteProductsWithFields[satelliteProduct.Id].AddRange(tempFields);
                    }
                }

                Logger.LogInformation($"satelliteProducts without fields = {satelliteProductsWithFields.Where(m => !m.Value.Any()).Count()}");
                Logger.LogInformation($"satelliteProducts with fields = {satelliteProductsWithFields.Where(m => m.Value.Any()).Count()}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                Logger.LogError(ex.InnerException?.Message);
            }
            return satelliteProductsWithFields;
        }
        #endregion

        #region steps
        private async Task<List<Sentinel2ProductQueue>> GeneratePoints(Dictionary<Guid, List<Common.Models.CropMonitoring.Field>> satelliteProductsWithFields, List<SatelliteProduct> needProducts,
            List<Sentinel2ProductQueue> needProductsQueue,
            List<Sentinel2GenerateIndexStatus> productQueueStatuses, int outputLine)
        {
            string TempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(TempPath);

            var result = new List<Sentinel2ProductQueue>();
            var index = -1;
            var today = DateTime.UtcNow;
            var allSeasons = MasofaCropMonitoringDbContext.Seasons.AsNoTracking()
                .Where(s => s.FieldId != null)
                .Where(s => !s.EndDate.HasValue || s.EndDate >= DateOnly.FromDateTime(today))
                .ToList();

            foreach (var productWithField in satelliteProductsWithFields)
            {
                index++;
                var product = needProducts.First(np => np.Id == productWithField.Key);
                var fields = productWithField.Value;
                var fieldIds = fields.Select(f => f.Id).ToList();
                var seasons = allSeasons.Where(s => fieldIds.Contains(s.FieldId.Value)).ToList();

                string tempExtractPath = null;
                var satelliteMetadata = await SentinelDbContext.Sentinel2ProductsMetadata.FirstOrDefaultAsync(s => s.ProductId == product.Id.ToString());
                if (satelliteMetadata?.OriginDate == null)
                {
                    continue;
                }
                DateTime originalDate = (DateTime)satelliteMetadata.OriginDate;

                try
                {
                    var zipFileStorageItem = await MasofaCommonDbContext.FileStorageItems
                        .FirstOrDefaultAsync(f => f.Id == product.MediadataPath);
                    if (zipFileStorageItem == null)
                    {
                        continue;
                    }

                    string tempZipPath = Path.Combine(TempPath, $"{product.ProductId}.zip");
                    string tempExtractRoot = Path.Combine(TempPath, $"extract_{product.ProductId}");

                    if (!File.Exists(tempZipPath))
                    {
                        var zipStream = await FileStorageProvider.GetFileStreamAsyncWithProgress(zipFileStorageItem);
                        Directory.CreateDirectory(tempExtractRoot);
                        tempExtractPath = tempExtractRoot;
                        using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
                        {
                            await zipStream.CopyToAsync(fileStream);
                        }
                    }

                    ZipFile.ExtractToDirectory(tempZipPath, tempExtractRoot);

                    var imgDataPath = IndicesHelper.FindImgDataPath(tempExtractRoot);
                    if (string.IsNullOrEmpty(imgDataPath))
                    {
                        continue;
                    }

                    var needProductQueueItem = needProductsQueue.First(n => n.ProductId == product.ProductId);
                    var productStatus = productQueueStatuses.First(ps => ps.Sentinel2ProductQueue == needProductQueueItem.Id);
                    var productEntity = SentinelDbContext.Sentinel2Products.First(m => m.SatellateProductId == product.Id.ToString());
                    var productInspire = SentinelDbContext.SentinelInspireMetadata.First(m => m.Id == productEntity.SentinelInspireMetadataId);
                    try
                    {
                        var generateResult = new DbGenerationResult()
                        {
                            OriginDate = new DateTime(productInspire.DateStamp.Year, productInspire.DateStamp.Month, productInspire.DateStamp.Day, productInspire.DateStamp.Hour, productInspire.DateStamp.Minute, productInspire.DateStamp.Second, DateTimeKind.Utc),
                            SatelliteProductId = product.Id,
                            ProductId = needProductQueueItem.ProductId,
                            ArviPoints = productStatus.ArviDb ? new List<ArviPoint>() : IndicesHelper.ArchiveFolderWorkDbARVI(imgDataPath, needProductQueueItem.ProductId, fields, seasons),
                            EviPoints = productStatus.EviDb ? new List<EviPoint>() : IndicesHelper.ArchiveFolderWorkDbEVI(imgDataPath, needProductQueueItem.ProductId, fields, seasons),
                            GndviPoints = productStatus.GndviDb ? new List<GndviPoint>() : IndicesHelper.ArchiveFolderWorkDbGNDVI(imgDataPath, needProductQueueItem.ProductId, fields, seasons),
                            MndwiPoints = productStatus.MndwiDb ? new List<MndwiPoint>() : IndicesHelper.ArchiveFolderWorkDbMNDWI(imgDataPath, needProductQueueItem.ProductId, fields, seasons),
                            NdmiPoints = productStatus.NdmiDb ? new List<NdmiPoint>() : IndicesHelper.ArchiveFolderWorkDbNDMI(imgDataPath, needProductQueueItem.ProductId, fields, seasons),
                            NdviPoints = productStatus.NdviDb ? new List<NdviPoint>() : IndicesHelper.ArchiveFolderWorkDbNDVI(imgDataPath, needProductQueueItem.ProductId, fields, seasons),
                            NdwiPoints = productStatus.NdwiDb ? new List<NdwiPoint>() : IndicesHelper.ArchiveFolderWorkDbNDWI(imgDataPath, needProductQueueItem.ProductId, fields, seasons),
                            OrviPoints = productStatus.OrviDb ? new List<OrviPoint>() : IndicesHelper.ArchiveFolderWorkDbORVI(imgDataPath, needProductQueueItem.ProductId, fields, seasons),
                            OsaviPoints = productStatus.OsaviDb ? new List<OsaviPoint>() : IndicesHelper.ArchiveFolderWorkDbOSAVI(imgDataPath, needProductQueueItem.ProductId, fields, seasons)
                        };

                        if (generateResult.ArviPoints.Any())
                        {
                            var resultPoints = new List<ArviPoint>();
                            foreach (var item in generateResult.ArviPoints)
                            {
                                var temp = item;
                                var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                                temp.CreateAt = generateResult.OriginDate;
                                temp.SatelliteProductId = generateResult.SatelliteProductId;
                                temp.ProductSourceType = ProductSourceType.Sentinel2;
                                temp.SeasonId = tempSeason?.Id;
                                temp.FieldId = tempSeason?.FieldId;
                                temp.RegionId = tempField?.RegionId;
                                resultPoints.Add(temp);
                            }

                            MasofaIndicesDbContext.ArviPoints.AddRange(resultPoints);
                            MasofaIndicesDbContext.SaveChanges();

                            var indexesGroup = resultPoints.GroupBy(i => i.SeasonId);

                            var indexSeasonsResult = new List<ArviSeasonReport>();
                            foreach (var indexSeason in indexesGroup)
                            {
                                var avarage = indexSeason.Select(i => i.Value).Sum() / indexSeason.Select(i => i.Value).Count();
                                var tempSeason = seasons.FirstOrDefault(s => s.Id == indexSeason.Key);
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

                                var newModel = new ArviSeasonReport()
                                {
                                    SeasonId = indexSeason.Key.Value,
                                    DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                                    Average = avarage,
                                    TotalMax = indexSeason.Select(i => i.Value).Max(),
                                    TotalMin = indexSeason.Select(i => i.Value).Min(),
                                    RegionId = tempField?.RegionId
                                };

                                indexSeasonsResult.Add(newModel);
                            }

                            MasofaIndicesDbContext.ArviSeasonReports.AddRange(indexSeasonsResult);
                            MasofaIndicesDbContext.SaveChanges();
                            productStatus.ArviDb = true;
                        }

                        if (generateResult.EviPoints.Any())
                        {
                            var resultPoints = new List<EviPoint>();
                            foreach (var item in generateResult.EviPoints)
                            {
                                var temp = item;
                                var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                                temp.CreateAt = generateResult.OriginDate;
                                temp.SatelliteProductId = generateResult.SatelliteProductId;
                                temp.ProductSourceType = ProductSourceType.Sentinel2;
                                temp.SeasonId = tempSeason?.Id;
                                temp.FieldId = tempSeason?.FieldId;
                                temp.RegionId = tempField?.RegionId;
                                resultPoints.Add(temp);
                            }

                            MasofaIndicesDbContext.EviPoints.AddRange(resultPoints);
                            MasofaIndicesDbContext.SaveChanges();

                            var indexesGroup = resultPoints.GroupBy(i => i.SeasonId);

                            var indexSeasonsResult = new List<EviSeasonReport>();
                            foreach (var indexSeason in indexesGroup)
                            {
                                var avarage = indexSeason.Select(i => i.Value).Sum() / indexSeason.Select(i => i.Value).Count();
                                var tempSeason = seasons.FirstOrDefault(s => s.Id == indexSeason.Key);
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

                                var newModel = new EviSeasonReport()
                                {
                                    SeasonId = indexSeason.Key.Value,
                                    DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                                    Average = avarage,
                                    TotalMax = indexSeason.Select(i => i.Value).Max(),
                                    TotalMin = indexSeason.Select(i => i.Value).Min(),
                                    RegionId = tempField?.RegionId
                                };

                                indexSeasonsResult.Add(newModel);
                            }

                            MasofaIndicesDbContext.EviSeasonReports.AddRange(indexSeasonsResult);
                            MasofaIndicesDbContext.SaveChanges();
                            productStatus.EviDb = true;
                        }

                        if (generateResult.GndviPoints.Any())
                        {
                            var resultPoints = new List<GndviPoint>();
                            foreach (var item in generateResult.GndviPoints)
                            {
                                var temp = item;
                                var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                                temp.CreateAt = generateResult.OriginDate;
                                temp.SatelliteProductId = generateResult.SatelliteProductId;
                                temp.ProductSourceType = ProductSourceType.Sentinel2;
                                temp.SeasonId = tempSeason?.Id;
                                temp.FieldId = tempSeason?.FieldId;
                                temp.RegionId = tempField?.RegionId;
                                resultPoints.Add(temp);
                            }

                            MasofaIndicesDbContext.GndviPoints.AddRange(resultPoints);
                            MasofaIndicesDbContext.SaveChanges();

                            var indexesGroup = resultPoints.GroupBy(i => i.SeasonId);

                            var indexSeasonsResult = new List<GndviSeasonReport>();
                            foreach (var indexSeason in indexesGroup)
                            {
                                var avarage = indexSeason.Select(i => i.Value).Sum() / indexSeason.Select(i => i.Value).Count();
                                var tempSeason = seasons.FirstOrDefault(s => s.Id == indexSeason.Key);
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

                                var newModel = new GndviSeasonReport()
                                {
                                    SeasonId = indexSeason.Key.Value,
                                    DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                                    Average = avarage,
                                    TotalMax = indexSeason.Select(i => i.Value).Max(),
                                    TotalMin = indexSeason.Select(i => i.Value).Min(),
                                    RegionId = tempField?.RegionId
                                };

                                indexSeasonsResult.Add(newModel);
                            }

                            MasofaIndicesDbContext.GndviSeasonReports.AddRange(indexSeasonsResult);
                            MasofaIndicesDbContext.SaveChanges();
                            productStatus.GndviDb = true;
                        }

                        if (generateResult.MndwiPoints.Any())
                        {
                            var resultPoints = new List<MndwiPoint>();
                            foreach (var item in generateResult.MndwiPoints)
                            {
                                var temp = item;
                                var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                                temp.CreateAt = generateResult.OriginDate;
                                temp.SatelliteProductId = generateResult.SatelliteProductId;
                                temp.ProductSourceType = ProductSourceType.Sentinel2;
                                temp.SeasonId = tempSeason?.Id;
                                temp.FieldId = tempSeason?.FieldId;
                                temp.RegionId = tempField?.RegionId;
                                resultPoints.Add(temp);
                            }

                            MasofaIndicesDbContext.MndwiPoints.AddRange(resultPoints);
                            MasofaIndicesDbContext.SaveChanges();

                            var indexesGroup = resultPoints.GroupBy(i => i.SeasonId);

                            var indexSeasonsResult = new List<MndwiSeasonReport>();
                            foreach (var indexSeason in indexesGroup)
                            {
                                var avarage = indexSeason.Select(i => i.Value).Sum() / indexSeason.Select(i => i.Value).Count();
                                var tempSeason = seasons.FirstOrDefault(s => s.Id == indexSeason.Key);
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

                                var newModel = new MndwiSeasonReport()
                                {
                                    SeasonId = indexSeason.Key.Value,
                                    DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                                    Average = avarage,
                                    TotalMax = indexSeason.Select(i => i.Value).Max(),
                                    TotalMin = indexSeason.Select(i => i.Value).Min(),
                                    RegionId = tempField?.RegionId
                                };

                                indexSeasonsResult.Add(newModel);
                            }

                            MasofaIndicesDbContext.MndwiSeasonReports.AddRange(indexSeasonsResult);
                            MasofaIndicesDbContext.SaveChanges();
                            productStatus.GndviDb = true;
                        }

                        if (generateResult.NdmiPoints.Any())
                        {
                            var resultPoints = new List<NdmiPoint>();
                            foreach (var item in generateResult.NdmiPoints)
                            {
                                var temp = item;
                                var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                                temp.CreateAt = generateResult.OriginDate;
                                temp.SatelliteProductId = generateResult.SatelliteProductId;
                                temp.ProductSourceType = ProductSourceType.Sentinel2;
                                temp.SeasonId = tempSeason?.Id;
                                temp.FieldId = tempSeason?.FieldId;
                                temp.RegionId = tempField?.RegionId;
                                resultPoints.Add(temp);
                            }

                            MasofaIndicesDbContext.NdmiPoints.AddRange(resultPoints);
                            MasofaIndicesDbContext.SaveChanges();

                            var indexesGroup = resultPoints.GroupBy(i => i.SeasonId);

                            var indexSeasonsResult = new List<NdmiSeasonReport>();
                            foreach (var indexSeason in indexesGroup)
                            {
                                var avarage = indexSeason.Select(i => i.Value).Sum() / indexSeason.Select(i => i.Value).Count();
                                var tempSeason = seasons.FirstOrDefault(s => s.Id == indexSeason.Key);
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

                                var newModel = new NdmiSeasonReport()
                                {
                                    SeasonId = indexSeason.Key.Value,
                                    DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                                    Average = avarage,
                                    TotalMax = indexSeason.Select(i => i.Value).Max(),
                                    TotalMin = indexSeason.Select(i => i.Value).Min(),
                                    RegionId = tempField?.RegionId
                                };

                                indexSeasonsResult.Add(newModel);
                            }

                            MasofaIndicesDbContext.NdmiSeasonReports.AddRange(indexSeasonsResult);
                            MasofaIndicesDbContext.SaveChanges();
                            productStatus.NdmiDb = true;
                        }

                        if (generateResult.NdviPoints.Any())
                        {
                            var resultPoints = new List<NdviPoint>();
                            foreach (var item in generateResult.NdviPoints)
                            {
                                var temp = item;
                                var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                                temp.CreateAt = generateResult.OriginDate;
                                temp.SatelliteProductId = generateResult.SatelliteProductId;
                                temp.ProductSourceType = ProductSourceType.Sentinel2;
                                temp.SeasonId = tempSeason?.Id;
                                temp.FieldId = tempSeason?.FieldId;
                                temp.RegionId = tempField?.RegionId;
                                resultPoints.Add(temp);
                            }

                            MasofaIndicesDbContext.NdviPoints.AddRange(resultPoints);
                            MasofaIndicesDbContext.SaveChanges();

                            var indexesGroup = resultPoints.GroupBy(i => i.SeasonId);

                            var indexSeasonsResult = new List<NdviSeasonReport>();
                            foreach (var indexSeason in indexesGroup)
                            {
                                var avarage = indexSeason.Select(i => i.Value).Sum() / indexSeason.Select(i => i.Value).Count();
                                var tempSeason = seasons.FirstOrDefault(s => s.Id == indexSeason.Key);
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

                                var newModel = new NdviSeasonReport()
                                {
                                    SeasonId = indexSeason.Key.Value,
                                    DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                                    Average = avarage,
                                    TotalMax = indexSeason.Select(i => i.Value).Max(),
                                    TotalMin = indexSeason.Select(i => i.Value).Min(),
                                    RegionId = tempField?.RegionId
                                };

                                indexSeasonsResult.Add(newModel);
                            }

                            MasofaIndicesDbContext.NdviSeasonReports.AddRange(indexSeasonsResult);
                            MasofaIndicesDbContext.SaveChanges();
                            productStatus.NdviDb = true;
                        }

                        if (generateResult.NdwiPoints.Any())
                        {
                            var resultPoints = new List<NdwiPoint>();
                            foreach (var item in generateResult.NdwiPoints)
                            {
                                var temp = item;
                                var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                                temp.CreateAt = generateResult.OriginDate;
                                temp.SatelliteProductId = generateResult.SatelliteProductId;
                                temp.ProductSourceType = ProductSourceType.Sentinel2;
                                temp.SeasonId = tempSeason?.Id;
                                temp.FieldId = tempSeason?.FieldId;
                                temp.RegionId = tempField?.RegionId;
                                resultPoints.Add(temp);
                            }

                            MasofaIndicesDbContext.NdwiPoints.AddRange(resultPoints);
                            MasofaIndicesDbContext.SaveChanges();

                            var indexesGroup = resultPoints.GroupBy(i => i.SeasonId);

                            var indexSeasonsResult = new List<NdwiSeasonReport>();
                            foreach (var indexSeason in indexesGroup)
                            {
                                var avarage = indexSeason.Select(i => i.Value).Sum() / indexSeason.Select(i => i.Value).Count();
                                var tempSeason = seasons.FirstOrDefault(s => s.Id == indexSeason.Key);
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

                                var newModel = new NdwiSeasonReport()
                                {
                                    SeasonId = indexSeason.Key.Value,
                                    DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                                    Average = avarage,
                                    TotalMax = indexSeason.Select(i => i.Value).Max(),
                                    TotalMin = indexSeason.Select(i => i.Value).Min(),
                                    RegionId = tempField?.RegionId
                                };

                                indexSeasonsResult.Add(newModel);
                            }

                            MasofaIndicesDbContext.NdwiSeasonReports.AddRange(indexSeasonsResult);
                            MasofaIndicesDbContext.SaveChanges();
                            productStatus.NdwiDb = true;
                        }

                        if (generateResult.OrviPoints.Any())
                        {
                            var resultPoints = new List<OrviPoint>();
                            foreach (var item in generateResult.OrviPoints)
                            {
                                var temp = item;
                                var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                                temp.CreateAt = generateResult.OriginDate;
                                temp.SatelliteProductId = generateResult.SatelliteProductId;
                                temp.ProductSourceType = ProductSourceType.Sentinel2;
                                temp.SeasonId = tempSeason?.Id;
                                temp.FieldId = tempSeason?.FieldId;
                                temp.RegionId = tempField?.RegionId;
                                resultPoints.Add(temp);
                            }

                            MasofaIndicesDbContext.OrviPoints.AddRange(resultPoints);
                            MasofaIndicesDbContext.SaveChanges();

                            var indexesGroup = resultPoints.GroupBy(i => i.SeasonId);

                            var indexSeasonsResult = new List<OrviSeasonReport>();
                            foreach (var indexSeason in indexesGroup)
                            {
                                var avarage = indexSeason.Select(i => i.Value).Sum() / indexSeason.Select(i => i.Value).Count();
                                var tempSeason = seasons.FirstOrDefault(s => s.Id == indexSeason.Key);
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

                                var newModel = new OrviSeasonReport()
                                {
                                    SeasonId = indexSeason.Key.Value,
                                    DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                                    Average = avarage,
                                    TotalMax = indexSeason.Select(i => i.Value).Max(),
                                    TotalMin = indexSeason.Select(i => i.Value).Min(),
                                    RegionId = tempField?.RegionId
                                };

                                indexSeasonsResult.Add(newModel);
                            }

                            MasofaIndicesDbContext.OrviSeasonReports.AddRange(indexSeasonsResult);
                            MasofaIndicesDbContext.SaveChanges();
                            productStatus.OrviDb = true;
                        }

                        if (generateResult.OsaviPoints.Any())
                        {
                            var resultPoints = new List<OsaviPoint>();
                            foreach (var item in generateResult.OsaviPoints)
                            {
                                var temp = item;
                                var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                                temp.CreateAt = generateResult.OriginDate;
                                temp.SatelliteProductId = generateResult.SatelliteProductId;
                                temp.ProductSourceType = ProductSourceType.Sentinel2;
                                temp.SeasonId = tempSeason?.Id;
                                temp.FieldId = tempSeason?.FieldId;
                                temp.RegionId = tempField?.RegionId;
                                resultPoints.Add(temp);
                            }

                            MasofaIndicesDbContext.OsaviPoints.AddRange(resultPoints);
                            MasofaIndicesDbContext.SaveChanges();

                            var indexesGroup = resultPoints.GroupBy(i => i.SeasonId);

                            var indexSeasonsResult = new List<OsaviSeasonReport>();
                            foreach (var indexSeason in indexesGroup)
                            {
                                var avarage = indexSeason.Select(i => i.Value).Sum() / indexSeason.Select(i => i.Value).Count();
                                var tempSeason = seasons.FirstOrDefault(s => s.Id == indexSeason.Key);
                                var tempField = tempSeason == null ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

                                var newModel = new OsaviSeasonReport()
                                {
                                    SeasonId = indexSeason.Key.Value,
                                    DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                                    Average = avarage,
                                    TotalMax = indexSeason.Select(i => i.Value).Max(),
                                    TotalMin = indexSeason.Select(i => i.Value).Min(),
                                    RegionId = tempField?.RegionId
                                };

                                indexSeasonsResult.Add(newModel);
                            }

                            MasofaIndicesDbContext.OsaviSeasonReports.AddRange(indexSeasonsResult);
                            MasofaIndicesDbContext.SaveChanges();
                            productStatus.OsaviDb = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("GenerateTiffProducts", $"Exception: {ex.Message}");
                    }
                    SentinelDbContext.Sentinel2GenerateIndexStatus.Update(productStatus);
                    SentinelDbContext.SaveChanges();

                }
                catch (Exception ex)
                {
                    Logger.LogError("GenerateTiffProducts", $"Exception: {ex.Message}");
                    continue;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempExtractPath) && Directory.Exists(tempExtractPath))
                        {
                            Directory.Delete(tempExtractPath, true);

                            Logger.LogInformation("GenerateTiffProducts", $"Cleaned up temp extract path: {tempExtractPath}");
                        }
                    }
                    catch (Exception cleanupEx)
                    {
                        Logger.LogError("GenerateTiffProducts", $"Cleanup error: {cleanupEx.Message}");
                    }
                }
            }

            Logger.LogInformation(outputLine, "GenerateTiffProducts", "ARVIJob completed successfully.");
            return result;
        }
        #endregion
    }

    public class ParallelesPointAndTiffFetchJobbResul : BaseJobResult
    {
        public int SuccessCount { get; set; } = 0;
    }
}
