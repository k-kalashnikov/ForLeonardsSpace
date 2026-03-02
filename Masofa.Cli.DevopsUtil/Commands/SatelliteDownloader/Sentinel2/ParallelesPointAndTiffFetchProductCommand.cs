using Masofa.Client.Copernicus;
using Masofa.Common.Helper;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.Satellite.Sentinel;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MaxRev.Gdal.Core;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using OSGeo.OSR;
using System.IO.Compression;
namespace Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2
{
    [BaseCommand("ParallelesPointAndTiffFetchProductCommand", "ParallelesPointAndTiffFetchProductCommand")]
    public class ParallelesPointAndTiffFetchProductCommand : IBaseCommand
    {
        private SentinelServiceOptions SentinelServiceOptions { get; }
        private CopernicusApiUnitOfWork CopernicusApiUnitOfWork { get; }
        private MasofaSentinelDbContext SentinelDbContext { get; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }

        private static bool GdalInitialized = false;
        private const double EPS = 1e-8;
        private static int outputH = 0;
        private static readonly object consoleLock = new object();
        private static readonly object dbContextLock = new object();
        private static readonly object fileStorageLock = new object();
        private Dictionary<Guid, Dictionary<string, bool>> globalResult = new Dictionary<Guid, Dictionary<string, bool>>();
        private string TempPath = "/";


        public ParallelesPointAndTiffFetchProductCommand(ILogger<ParallelesPointAndTiffFetchProductCommand> logger,
            CopernicusApiUnitOfWork copernicusApiUnitOfWork,
            MasofaSentinelDbContext sentinelDbContext,
            IOptions<SentinelServiceOptions> options,
            MasofaCommonDbContext masofaCommonDbContext,
            IFileStorageProvider fileStorageProvider,
            MasofaIndicesDbContext masofaIndicesDbContext,
            MasofaCropMonitoringDbContext masofaCropMonitoringDbContext)
        {
            CopernicusApiUnitOfWork = copernicusApiUnitOfWork;
            SentinelDbContext = sentinelDbContext;
            SentinelServiceOptions = options.Value;
            SentinelServiceOptions.SatelliteSearchConfig = new SatelliteSearchConfig();
            MasofaCommonDbContext = masofaCommonDbContext;
            FileStorageProvider = fileStorageProvider;
            MasofaIndicesDbContext = masofaIndicesDbContext;
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            InitializeGdal();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task Execute()
        {
            try
            {
                await FetchStatus();

                var needProductsStates = SentinelDbContext.Sentinel2GenerateIndexStatus.ToList();

                var needProductsQueueIds = needProductsStates.Select(m => m.Sentinel2ProductQueue).ToList();

                var needProductsQueue = SentinelDbContext.Sentinel2ProductsQueue
                    .Where(pq => needProductsQueueIds.Contains(pq.Id))
                    .ToList();

                var needProductIds = needProductsQueue.Select(pq => pq.ProductId).ToList();

                var needProducts = MasofaCommonDbContext.SatelliteProducts
                    .Where(sp => needProductIds.Contains(sp.ProductId))
                    .OrderByDescending(sp => sp.OriginDate)
                    .ToList();

                var satelliteProductsWithFields = GetSatelluteProductWithFields(needProducts);

                await Generate(satelliteProductsWithFields, needProducts, needProductsQueue, needProductsStates, 3);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }

        #region MainMethods

        private void InitializeGdal()
        {
            if (GdalInitialized)
                return;

            try
            {
                GdalBase.ConfigureAll();
                GdalInitialized = true;

                var testSrs = new SpatialReference(string.Empty);
                testSrs.SetWellKnownGeogCS("WGS84");
                string geogcs = testSrs.GetAttrValue("GEOGCS", 0);
                Console.WriteLine($"PROJ/GDAL initialized. GEOGCS: {geogcs}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize GDAL/PROJ", ex);
                throw;
            }
        }

        private async Task FetchStatus()
        {
            var products = SentinelDbContext.Sentinel2ProductsQueue.ToList();
            var produtcsStatuses = SentinelDbContext.Sentinel2GenerateIndexStatus?.ToList() ?? new List<Sentinel2GenerateIndexStatus>();
            var result = new List<Sentinel2ProductQueue>();
            var resultIndexStatus = new List<Sentinel2GenerateIndexStatus>();

            foreach (var item in products)
            {
                if (produtcsStatuses.Any(m => m.Sentinel2ProductQueue == item.Id))
                {
                    continue;
                }
                var temp = item;
                var tempQStatus = new Sentinel2GenerateIndexStatus()
                {
                    Sentinel2ProductQueue = temp.Id
                };

                resultIndexStatus.Add(tempQStatus);
            }
            if (resultIndexStatus.Any())
            {
                SentinelDbContext.Sentinel2GenerateIndexStatus.AddRange(resultIndexStatus);
                SentinelDbContext.SaveChanges();
            }

            var satelliteProducts = MasofaCommonDbContext.SatelliteProducts.ToList();
            var satelliteProductsIs = satelliteProducts.Select(m => m.Id.ToString()).ToList();
            var satelliteProductsEntity = SentinelDbContext.Sentinel2Products.Where(m => satelliteProductsIs.Contains(m.SatellateProductId)).ToList();
            var satelliteProductsInspireIds = satelliteProductsEntity.Select(m => m.SentinelInspireMetadataId).ToList();
            var satelliteInsprire = SentinelDbContext.SentinelInspireMetadata.Where(m => satelliteProductsInspireIds.Contains(m.Id)).ToList();
            var resultUpdate = new List<SatelliteProduct>();
            Console.WriteLine("Fetching OriginDate and Polygon");
            var index = 0;
            foreach (var item in satelliteProducts)
            {
                index++;
                var temp = item;
                var sentineProduct = satelliteProductsEntity.First(sp => sp.SatellateProductId == item.Id.ToString());
                if (sentineProduct == null)
                {
                    continue;
                }
                var tempInsipe = satelliteInsprire.FirstOrDefault(si => si.Id == sentineProduct.SentinelInspireMetadataId);
                if (tempInsipe == null)
                {
                    continue;
                }
                temp.OriginDate = tempInsipe.DateStamp;
                temp.Polygon = ToPolygon(tempInsipe);
                resultUpdate.Add(temp);
                Console.Write($"\r {index} of {satelliteProducts.Count}");
            }
            MasofaCommonDbContext.SatelliteProducts.UpdateRange(resultUpdate);
            MasofaCommonDbContext.SaveChanges();
        }
        private Dictionary<Guid, List<Masofa.Common.Models.CropMonitoring.Field>> GetSatelluteProductWithFields(List<Masofa.Common.Models.Satellite.SatelliteProduct> sourceProduct)
        {
            var satelliteProductsWithFields = new Dictionary<Guid, List<Masofa.Common.Models.CropMonitoring.Field>>();

            try
            {
                var satelliteProducts = sourceProduct
                    .Where(m => m.ProductSourceType == ProductSourceType.Sentinel2)
                    .Where(m => m.Polygon != null)
                    .ToList();

                var fields = MasofaCropMonitoringDbContext.Fields
                    .ToList();

                var neededFields = fields
                    .Where(f => f.Polygon != null)
                    .Where(f => satelliteProducts.Select(sp => sp.Polygon).Any(sp => sp.Contains(f.Polygon) || sp.Intersects(f.Polygon)))
                    .ToList();


                foreach (var satelliteProduct in satelliteProducts)
                {
                    var tempFields = new List<Common.Models.CropMonitoring.Field>();
                    foreach (var field in neededFields)
                    {
                        if ((satelliteProduct.Polygon.Contains(field.Polygon)) || (satelliteProduct.Polygon.Intersects(field.Polygon)))
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

                Console.WriteLine($"satelliteProducts without fields = {satelliteProductsWithFields.Where(m => !m.Value.Any()).Count()}");
                Console.WriteLine($"satelliteProducts with fields = {satelliteProductsWithFields.Where(m => m.Value.Any()).Count()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
            return satelliteProductsWithFields;
        }
        private void DrawProgressBar(int line, int current, int total, string taksName, string message)
        {
            // Ограничиваем текущий прогресс
            current = Math.Min(current, total);
            int filledLength = ((int)Math.Round((double)current / total) * 100);

            string bar = new string('█', filledLength).PadRight(100);
            string text = $"{taksName}: [{bar}] {current} of {total.ToString()}";
            Console.SetCursorPosition(0, line);
            Console.Write(message.PadRight(Console.WindowWidth - 1)); // затираем старый текст
            Console.SetCursorPosition(0, line + 1);
            Console.Write(text.PadRight(Console.WindowWidth - 1));
        }
        private static Polygon ToPolygon(SentinelInspireMetadata sim)
        {
            var west = (double)sim.WestBoundLongitude;
            var east = (double)sim.EastBoundLongitude;
            var south = (double)sim.SouthBoundLatitude;
            var north = (double)sim.NorthBoundLatitude;

            if (west >= east)
            {
                throw new ArgumentException("West must be < East.");
            }

            if (south >= north)
            {
                throw new ArgumentException("South must be < North.");
            }

            var coords = new[]
            {
                new Coordinate(west, south),
                new Coordinate(east, south),
                new Coordinate(east, north),
                new Coordinate(west, north),
                new Coordinate(west, south)
            };

            var factory = new GeometryFactory(new PrecisionModel(), 4326);
            var ring = factory.CreateLinearRing(coords);
            return factory.CreatePolygon(ring);
        }

        private async Task SendToMinioTiff<TIndexModel>(string arviTiffPath, string tiffFileName, string tiffBucket, Guid Id, string ProductId, DateTime originalDate, bool isColored)
        where TIndexModel : BaseIndexPolygon
        {
            using (var tiffStream = File.OpenRead(arviTiffPath))
            {
                string minioTiffPath = await FileStorageProvider.PushFileAsync(tiffStream, tiffFileName, tiffBucket);

                var tiffFileStorageItem = new FileStorageItem()
                {
                    CreateAt = DateTime.UtcNow,
                    CreateUser = Guid.Empty,
                    OwnerId = Id,
                    OwnerTypeFullName = typeof(TIndexModel).FullName,
                    FileContentType = FileContentType.ImageTiff,
                    Status = StatusType.Active,
                    FileStoragePath = minioTiffPath,
                    FileStorageBacket = tiffBucket,
                };

                tiffFileStorageItem = (await MasofaCommonDbContext.FileStorageItems.AddAsync(tiffFileStorageItem)).Entity;
                await MasofaCommonDbContext.SaveChangesAsync();

                var sentinelProduct = await SentinelDbContext.Sentinel2Products
                    .FirstOrDefaultAsync(s => s.SatellateProductId == Id.ToString());

                if (sentinelProduct == null)
                {
                    Console.WriteLine($"Sentinel2Product not found for ProductId: {Id.ToString()}");
                    throw new ArgumentException($"SatellateProductId with Id - {Id.ToString()} is not found", "Id");
                }

                var sentinelProductMetadata = await SentinelDbContext.SentinelInspireMetadata
                    .FirstOrDefaultAsync(m => m.Id == sentinelProduct.SentinelInspireMetadataId);

                if (sentinelProductMetadata == null)
                {
                    Console.WriteLine($"SentinelInspireMetadata not found for Id: {sentinelProduct.SentinelInspireMetadataId}");
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

        #region Generate
        public async Task Generate(Dictionary<Guid, List<Masofa.Common.Models.CropMonitoring.Field>> satelliteProductsWithFields,
            List<SatelliteProduct> needProducts,
            List<Sentinel2ProductQueue> needProductsQueue,
            List<Sentinel2GenerateIndexStatus> productQueueStatuses, int outputLine)
        {
            Console.WriteLine("Enter pls tempPath");
            TempPath = Console.ReadLine();
            var errors = new List<string>();
            var errorLogPath = Path.Combine(TempPath, $"errors-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm")}.md");
            errors.Add($"| 🐉 | ProductId | OriginalDate | Error |");
            errors.Add($"| - | - | - | - |");
            var errorIndex = 0;
            Console.Clear();

            var result = new List<Sentinel2ProductQueue>();
            DrawProgressBar(outputLine, 0, satelliteProductsWithFields.Count, "GenerateTiffProducts", $"Found {satelliteProductsWithFields.Count} products to process.");
            var index = -1;
            var allSeasons = MasofaCropMonitoringDbContext.Seasons.AsNoTracking()
                .Where(s => s.FieldId != null)
                .ToList();
    
            foreach (var productWithField in satelliteProductsWithFields)
            {
                index++;
                var product = needProducts.First(np => np.Id == productWithField.Key);
                var fields = productWithField.Value;
                var fieldIds = fields.Select(f => f.Id).ToList();
                var seasons = allSeasons.Where(s => fieldIds.Contains(s.FieldId.Value)).ToList();
                DrawProgressBar(outputLine, index, satelliteProductsWithFields.Count, "GenerateTiffProducts", $"Start for {index} of {satelliteProductsWithFields.Count} product at {product.OriginDate.Value.ToString("yyyy-MM-dd")}");

                string tempExtractPath = null;
                DateTime originalDate = (DateTime)product.OriginDate;

                try
                {
                    var zipFileStorageItem = await MasofaCommonDbContext.FileStorageItems.FirstOrDefaultAsync(f => f.Id == product.MediadataPath);
                    if (zipFileStorageItem == null)
                    {
                        DrawProgressBar(outputLine, index, satelliteProductsWithFields.Count, "GenerateTiffProducts", $"FileStorageItem not found for MediadataPath: {product.MediadataPath}");
                        continue;
                    }

                    DrawProgressBar(outputLine, index, satelliteProductsWithFields.Count, "GenerateTiffProducts", $"Start download : {zipFileStorageItem.FileStoragePath}");
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




                    DrawProgressBar(outputLine, index, satelliteProductsWithFields.Count, "GenerateTiffProducts", $"Downloaded ZIP to: {tempZipPath}");
                    ZipFile.ExtractToDirectory(tempZipPath, tempExtractRoot);
                    DrawProgressBar(outputLine, index, satelliteProductsWithFields.Count, "GenerateTiffProducts", $"Extracted ZIP to: {tempExtractRoot}");

                    var imgDataPath = IndicesHelper.FindImgDataPath(tempExtractRoot);
                    if (string.IsNullOrEmpty(imgDataPath))
                    {
                        DrawProgressBar(outputLine, index, satelliteProductsWithFields.Count, "GenerateTiffProducts", "IMG_DATA path could not be determined. Skipping product.");
                        continue;
                    }

                    var needProductQueueItem = needProductsQueue.First(n => n.ProductId == product.ProductId);
                    var productStatus = productQueueStatuses.First(ps => ps.Sentinel2ProductQueue == needProductQueueItem.Id);
                    var productEntity = SentinelDbContext.Sentinel2Products.First(m => m.SatellateProductId == product.Id.ToString());
                    var productInspire = SentinelDbContext.SentinelInspireMetadata.First(m => m.Id == productEntity.SentinelInspireMetadataId);

                    if (!productStatus.IsDbComplite)
                    {
                        productStatus = await GenerateProductPoints(satelliteProductsWithFields.Count, outputLine, index, product.Id, seasons, fields, productInspire, product, needProductQueueItem, productStatus, imgDataPath);
                    }

                    if (!productStatus.IsTiffComplite)
                    {
                        var generateResult = await GenerateProductTiffs(productStatus, needProductQueueItem, imgDataPath);
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
                                DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Complite {tempItem.Key} for {needProductQueueItem.ProductId}");
                            }
                            catch (Exception ex)
                            {
                                DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Exception: {ex.Message}");
                            }

                        }
                    }

                    SentinelDbContext.Sentinel2GenerateIndexStatus.Update(productStatus);
                    if (productStatus.IsDbComplite && productStatus.IsTiffComplite)
                    {
                        needProductQueueItem.QueueStatus = ProductQueueStatusType.IndicesComplite;
                        SentinelDbContext.Sentinel2ProductsQueue.Update(needProductQueueItem);
                    }
                    SentinelDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    errors.Add($"| {errorIndex} | {product.Id} | {product.OriginDate.Value.ToString("yyyy-MM-dd")} | {ex.Message} |");
                    errorIndex++;
                }
            }
            if (File.Exists(errorLogPath))
            {
                File.Delete(errorLogPath);
            }
            File.WriteAllLines(errorLogPath, errors.ToArray());
        }

        public async Task<Sentinel2GenerateIndexStatus> GenerateProductPoints(int totalCount, int outputLine, int index, Guid productId, 
            List<Season> seasons, List<Field> fields,
            SentinelInspireMetadata productInspire, SatelliteProduct product, Sentinel2ProductQueue needProductQueueItem, Sentinel2GenerateIndexStatus productStatus,
            string imgDataPath)
        {
            var result = productStatus;
            var generateResult = new DbGenerationResult()
            {
                OriginDate = new DateTime(productInspire.DateStamp.Year, productInspire.DateStamp.Month, productInspire.DateStamp.Day, productInspire.DateStamp.Hour, productInspire.DateStamp.Minute, productInspire.DateStamp.Second, DateTimeKind.Utc),
                SatelliteProductId = product.Id,
                ProductId = needProductQueueItem.ProductId,
                ArviPoints = (productStatus.ArviDb) ? new List<ArviPoint>() : IndicesHelper.ArchiveFolderWorkDbARVI(imgDataPath, needProductQueueItem.ProductId, fields),
                EviPoints = (productStatus.EviDb) ? new List<EviPoint>() : IndicesHelper.ArchiveFolderWorkDbEVI(imgDataPath, needProductQueueItem.ProductId, fields),
                GndviPoints = (productStatus.GndviDb) ? new List<GndviPoint>() : IndicesHelper.ArchiveFolderWorkDbGNDVI(imgDataPath, needProductQueueItem.ProductId, fields),
                MndwiPoints = (productStatus.MndwiDb) ? new List<MndwiPoint>() : IndicesHelper.ArchiveFolderWorkDbMNDWI(imgDataPath, needProductQueueItem.ProductId, fields),
                NdmiPoints = (productStatus.NdmiDb) ? new List<NdmiPoint>() : IndicesHelper.ArchiveFolderWorkDbNDMI(imgDataPath, needProductQueueItem.ProductId, fields),
                NdviPoints = (productStatus.NdviDb) ? new List<NdviPoint>() : IndicesHelper.ArchiveFolderWorkDbNDVI(imgDataPath, needProductQueueItem.ProductId, fields),
                NdwiPoints = (productStatus.NdwiDb) ? new List<NdwiPoint>() : IndicesHelper.ArchiveFolderWorkDbNDWI(imgDataPath, needProductQueueItem.ProductId, fields),
                OrviPoints = (productStatus.OrviDb) ? new List<OrviPoint>() : IndicesHelper.ArchiveFolderWorkDbORVI(imgDataPath, needProductQueueItem.ProductId, fields),
                OsaviPoints = (productStatus.OsaviDb) ? new List<OsaviPoint>() : IndicesHelper.ArchiveFolderWorkDbOSAVI(imgDataPath, needProductQueueItem.ProductId, fields)
            };

            if (generateResult.ArviPoints.Any())
            {
                var resultPoints = new List<ArviPoint>();
                foreach (var item in generateResult.ArviPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
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
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

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
                var cropGroup = new Dictionary<Guid, List<ArviSeasonReport>>();
                foreach (var item in indexSeasonsResult)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.SeasonId);
                    var tempCropId = tempSeason?.CropId;
                    if (tempSeason == null)
                    {
                        continue;
                    }
                    if (!cropGroup.ContainsKey(tempCropId.Value))
                    {
                        cropGroup.Add(tempCropId.Value, new List<ArviSeasonReport>());
                    }
                    cropGroup[tempCropId.Value].Add(item);
                }

                var sharedIndexResult = new List<ArviSharedReport>();
                foreach (var item in cropGroup)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.Key);
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                    var newModel = new ArviSharedReport()
                    {
                        CropId = item.Key,
                        DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                        Average = item.Value.Select(i => i.Average).Sum() / item.Value.Select(i => i.Average).Count(),
                        TotalMax = item.Value.Select(i => i.TotalMax).Max(),
                        TotalMin = item.Value.Select(i => i.TotalMin).Min(),
                        RegionId = tempField?.RegionId,
                        AverageMax = item.Value.Select(i => i.Average).Max(),
                        AverageMin = item.Value.Select(i => i.Average).Min()
                    };

                    sharedIndexResult.Add(newModel);
                }

                MasofaIndicesDbContext.ArviSharedReports.AddRange(sharedIndexResult);
                MasofaIndicesDbContext.SaveChanges();
                result.ArviDb = true;
            }

            if (generateResult.EviPoints.Any())
            {
                var resultPoints = new List<EviPoint>();
                foreach (var item in generateResult.EviPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
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
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

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

                var cropGroup = new Dictionary<Guid, List<EviSeasonReport>>();
                foreach (var item in indexSeasonsResult)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.SeasonId);
                    var tempCropId = tempSeason?.CropId;
                    if (tempSeason == null)
                    {
                        continue;
                    }
                    if (!cropGroup.ContainsKey(tempCropId.Value))
                    {
                        cropGroup.Add(tempCropId.Value, new List<EviSeasonReport>());
                    }
                    cropGroup[tempCropId.Value].Add(item);
                }

                var sharedIndexResult = new List<EviSharedReport>();
                foreach (var item in cropGroup)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.Key);
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                    var newModel = new EviSharedReport()
                    {
                        CropId = item.Key,
                        DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                        Average = item.Value.Select(i => i.Average).Sum() / item.Value.Select(i => i.Average).Count(),
                        TotalMax = item.Value.Select(i => i.TotalMax).Max(),
                        TotalMin = item.Value.Select(i => i.TotalMin).Min(),
                        RegionId = tempField?.RegionId,
                        AverageMax = item.Value.Select(i => i.Average).Max(),
                        AverageMin = item.Value.Select(i => i.Average).Min()
                    };

                    sharedIndexResult.Add(newModel);
                }

                MasofaIndicesDbContext.EviSharedReports.AddRange(sharedIndexResult);
                MasofaIndicesDbContext.SaveChanges();
                result.EviDb = true;
            }

            if (generateResult.GndviPoints.Any())
            {
                var resultPoints = new List<GndviPoint>();
                foreach (var item in generateResult.GndviPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
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
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

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
                result.GndviDb = true;
            }

            if (generateResult.MndwiPoints.Any())
            {
                var resultPoints = new List<MndwiPoint>();
                foreach (var item in generateResult.MndwiPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
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
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

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

                var cropGroup = new Dictionary<Guid, List<MndwiSeasonReport>>();
                foreach (var item in indexSeasonsResult)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.SeasonId);
                    var tempCropId = tempSeason?.CropId;
                    if (tempSeason == null)
                    {
                        continue;
                    }
                    if (!cropGroup.ContainsKey(tempCropId.Value))
                    {
                        cropGroup.Add(tempCropId.Value, new List<MndwiSeasonReport>());
                    }
                    cropGroup[tempCropId.Value].Add(item);
                }

                var sharedIndexResult = new List<MndwiSharedReport>();
                foreach (var item in cropGroup)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.Key);
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                    var newModel = new MndwiSharedReport()
                    {
                        CropId = item.Key,
                        DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                        Average = item.Value.Select(i => i.Average).Sum() / item.Value.Select(i => i.Average).Count(),
                        TotalMax = item.Value.Select(i => i.TotalMax).Max(),
                        TotalMin = item.Value.Select(i => i.TotalMin).Min(),
                        RegionId = tempField?.RegionId,
                        AverageMax = item.Value.Select(i => i.Average).Max(),
                        AverageMin = item.Value.Select(i => i.Average).Min()
                    };

                    sharedIndexResult.Add(newModel);
                }

                MasofaIndicesDbContext.MndwiSharedReports.AddRange(sharedIndexResult);
                MasofaIndicesDbContext.SaveChanges();
                result.MndwiDb = true;
            }

            if (generateResult.NdmiPoints.Any())
            {
                var resultPoints = new List<NdmiPoint>();
                foreach (var item in generateResult.NdmiPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
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
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

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

                var cropGroup = new Dictionary<Guid, List<NdmiSeasonReport>>();
                foreach (var item in indexSeasonsResult)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.SeasonId);
                    var tempCropId = tempSeason?.CropId;
                    if (tempSeason == null)
                    {
                        continue;
                    }
                    if (!cropGroup.ContainsKey(tempCropId.Value))
                    {
                        cropGroup.Add(tempCropId.Value, new List<NdmiSeasonReport>());
                    }
                    cropGroup[tempCropId.Value].Add(item);
                }

                var sharedIndexResult = new List<NdmiSharedReport>();
                foreach (var item in cropGroup)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.Key);
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                    var newModel = new NdmiSharedReport()
                    {
                        CropId = item.Key,
                        DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                        Average = item.Value.Select(i => i.Average).Sum() / item.Value.Select(i => i.Average).Count(),
                        TotalMax = item.Value.Select(i => i.TotalMax).Max(),
                        TotalMin = item.Value.Select(i => i.TotalMin).Min(),
                        RegionId = tempField?.RegionId,
                        AverageMax = item.Value.Select(i => i.Average).Max(),
                        AverageMin = item.Value.Select(i => i.Average).Min()
                    };

                    sharedIndexResult.Add(newModel);
                }

                MasofaIndicesDbContext.NdmiSharedReports.AddRange(sharedIndexResult);
                MasofaIndicesDbContext.SaveChanges();
                result.NdmiDb = true;
            }

            if (generateResult.NdviPoints.Any())
            {
                var resultPoints = new List<NdviPoint>();
                foreach (var item in generateResult.NdviPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
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
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

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

                var cropGroup = new Dictionary<Guid, List<NdviSeasonReport>>();
                foreach (var item in indexSeasonsResult)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.SeasonId);
                    var tempCropId = tempSeason?.CropId;
                    if (tempSeason == null)
                    {
                        continue;
                    }
                    if (!cropGroup.ContainsKey(tempCropId.Value))
                    {
                        cropGroup.Add(tempCropId.Value, new List<NdviSeasonReport>());
                    }
                    cropGroup[tempCropId.Value].Add(item);
                }

                var sharedIndexResult = new List<NdviSharedReport>();
                foreach (var item in cropGroup)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.Key);
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                    var newModel = new NdviSharedReport()
                    {
                        CropId = item.Key,
                        DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                        Average = item.Value.Select(i => i.Average).Sum() / item.Value.Select(i => i.Average).Count(),
                        TotalMax = item.Value.Select(i => i.TotalMax).Max(),
                        TotalMin = item.Value.Select(i => i.TotalMin).Min(),
                        RegionId = tempField?.RegionId,
                        AverageMax = item.Value.Select(i => i.Average).Max(),
                        AverageMin = item.Value.Select(i => i.Average).Min()
                    };

                    sharedIndexResult.Add(newModel);
                }

                MasofaIndicesDbContext.NdviSharedReports.AddRange(sharedIndexResult);
                MasofaIndicesDbContext.SaveChanges();
                result.NdviDb = true;
            }

            if (generateResult.NdwiPoints.Any())
            {
                var resultPoints = new List<NdwiPoint>();
                foreach (var item in generateResult.NdwiPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
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
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

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

                var cropGroup = new Dictionary<Guid, List<NdwiSeasonReport>>();
                foreach (var item in indexSeasonsResult)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.SeasonId);
                    var tempCropId = tempSeason?.CropId;
                    if (tempSeason == null)
                    {
                        continue;
                    }
                    if (!cropGroup.ContainsKey(tempCropId.Value))
                    {
                        cropGroup.Add(tempCropId.Value, new List<NdwiSeasonReport>());
                    }
                    cropGroup[tempCropId.Value].Add(item);
                }

                var sharedIndexResult = new List<NdwiSharedReport>();
                foreach (var item in cropGroup)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.Key);
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                    var newModel = new NdwiSharedReport()
                    {
                        CropId = item.Key,
                        DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                        Average = item.Value.Select(i => i.Average).Sum() / item.Value.Select(i => i.Average).Count(),
                        TotalMax = item.Value.Select(i => i.TotalMax).Max(),
                        TotalMin = item.Value.Select(i => i.TotalMin).Min(),
                        RegionId = tempField?.RegionId,
                        AverageMax = item.Value.Select(i => i.Average).Max(),
                        AverageMin = item.Value.Select(i => i.Average).Min()
                    };

                    sharedIndexResult.Add(newModel);
                }

                MasofaIndicesDbContext.NdwiSharedReports.AddRange(sharedIndexResult);
                MasofaIndicesDbContext.SaveChanges();
                result.NdwiDb = true;
            }

            if (generateResult.OrviPoints.Any())
            {
                var resultPoints = new List<OrviPoint>();
                foreach (var item in generateResult.OrviPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
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
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

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

                var cropGroup = new Dictionary<Guid, List<OrviSeasonReport>>();
                foreach (var item in indexSeasonsResult)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.SeasonId);
                    var tempCropId = tempSeason?.CropId;
                    if (tempSeason == null)
                    {
                        continue;
                    }
                    if (!cropGroup.ContainsKey(tempCropId.Value))
                    {
                        cropGroup.Add(tempCropId.Value, new List<OrviSeasonReport>());
                    }
                    cropGroup[tempCropId.Value].Add(item);
                }

                var sharedIndexResult = new List<OrviSharedReport>();
                foreach (var item in cropGroup)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.Key);
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                    var newModel = new OrviSharedReport()
                    {
                        CropId = item.Key,
                        DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                        Average = item.Value.Select(i => i.Average).Sum() / item.Value.Select(i => i.Average).Count(),
                        TotalMax = item.Value.Select(i => i.TotalMax).Max(),
                        TotalMin = item.Value.Select(i => i.TotalMin).Min(),
                        RegionId = tempField?.RegionId,
                        AverageMax = item.Value.Select(i => i.Average).Max(),
                        AverageMin = item.Value.Select(i => i.Average).Min()
                    };

                    sharedIndexResult.Add(newModel);
                }

                MasofaIndicesDbContext.OrviSharedReports.AddRange(sharedIndexResult);
                MasofaIndicesDbContext.SaveChanges();
                result.OrviDb = true;
            }

            if (generateResult.OsaviPoints.Any())
            {
                var resultPoints = new List<OsaviPoint>();
                foreach (var item in generateResult.OsaviPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point));
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
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
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);

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

                var cropGroup = new Dictionary<Guid, List<OsaviSeasonReport>>();
                foreach (var item in indexSeasonsResult)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.SeasonId);
                    var tempCropId = tempSeason?.CropId;
                    if (tempSeason == null)
                    {
                        continue;
                    }
                    if (!cropGroup.ContainsKey(tempCropId.Value))
                    {
                        cropGroup.Add(tempCropId.Value, new List<OsaviSeasonReport>());
                    }
                    cropGroup[tempCropId.Value].Add(item);
                }

                var sharedIndexResult = new List<OsaviSharedReport>();
                foreach (var item in cropGroup)
                {
                    var tempSeason = seasons.FirstOrDefault(s => s.Id == item.Key);
                    var tempField = (tempSeason == null) ? null : fields.FirstOrDefault(f => f.Id == tempSeason.FieldId.Value);
                    var newModel = new OsaviSharedReport()
                    {
                        CropId = item.Key,
                        DateOnly = DateOnly.FromDateTime(generateResult.OriginDate),
                        Average = item.Value.Select(i => i.Average).Sum() / item.Value.Select(i => i.Average).Count(),
                        TotalMax = item.Value.Select(i => i.TotalMax).Max(),
                        TotalMin = item.Value.Select(i => i.TotalMin).Min(),
                        RegionId = tempField?.RegionId,
                        AverageMax = item.Value.Select(i => i.Average).Max(),
                        AverageMin = item.Value.Select(i => i.Average).Min()
                    };

                    sharedIndexResult.Add(newModel);
                }

                MasofaIndicesDbContext.OsaviSharedReports.AddRange(sharedIndexResult);
                MasofaIndicesDbContext.SaveChanges();
                result.OsaviDb = true;
            }

            DrawProgressBar(outputLine + index, index, totalCount, "GenerateTiffProducts", $"Complite for {productId.ToString()} with " +
                $"ARVI:{generateResult.ArviPoints.Count} " +
                $"EVI:{generateResult.EviPoints.Count} " +
                $"GNDVI:{generateResult.GndviPoints.Count} " +
                $"MNDWI:{generateResult.MndwiPoints.Count} " +
                $"NDMI:{generateResult.NdmiPoints.Count} " +
                $"NDVI:{generateResult.NdviPoints.Count} " +
                $"NDWI:{generateResult.NdwiPoints.Count} " +
                $"ORVI:{generateResult.OrviPoints.Count} " +
                $"OSAVI:{generateResult.OsaviPoints.Count}"
            );

            return result;
        }


        public async Task<Dictionary<string, TiffGenerationResult>> GenerateProductTiffs(Sentinel2GenerateIndexStatus productStatus, Sentinel2ProductQueue needProductQueueItem, string archiveFolder)
        {
            var tempProductStatus = productStatus;
            var result = new Dictionary<string, TiffGenerationResult>();
            if (tempProductStatus == null)
            {
                tempProductStatus = new Sentinel2GenerateIndexStatus()
                {
                    Id = Guid.NewGuid(),
                    Sentinel2ProductQueue = needProductQueueItem.Id
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
                var indexResult = (TiffGenerationResult)indexMethod.Invoke(null, [archiveFolder, needProductQueueItem.ProductId]);
                result.Add(item.Name, indexResult);
            }

            return result;
        }
        #endregion
    }
}
