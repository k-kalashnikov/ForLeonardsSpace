using Masofa.Cli.DevopsUtil.Commands.Satellite;
using Masofa.Client.Copernicus;
using Masofa.Common.Extentions;
using Masofa.Common.Helper;
using Masofa.Common.Models;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.Satellite.Parse.Sentinel.Inspire;
using Masofa.Common.Models.Satellite.Sentinel;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;
using MaxRev.Gdal.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json.Linq;
using OSGeo.GDAL;
using OSGeo.OSR;
using Quartz.Logging;
using SharpKml.Dom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;

namespace Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2
{
    [BaseCommand("Paralleles Fetch Tiff Products And Indices Command", "Fetch Products And Indices Command")]
    public class ParallelesTiffFetchProductCommand : IBaseCommand
    {
        ILogger<ParallelesTiffFetchProductCommand> Logger { get; }
        private SentinelServiceOptions SentinelServiceOptions { get; }
        private CopernicusApiUnitOfWork CopernicusApiUnitOfWork { get; }
        private MasofaSentinelDbContext SentinelDbContext { get; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }


        private static bool GdalInitialized = false;
        private const double EPS = 1e-8;
        private static int outputH = 0;
        private static readonly object consoleLock = new object();
        private static readonly object dbContextLock = new object();
        private static readonly object fileStorageLock = new object();
        private Dictionary<Guid, Dictionary<string, bool>> globalResult = new Dictionary<Guid, Dictionary<string, bool>>();
        private string TempPath = "/";

        public ParallelesTiffFetchProductCommand(ILogger<ParallelesTiffFetchProductCommand> logger, 
            CopernicusApiUnitOfWork copernicusApiUnitOfWork, 
            MasofaSentinelDbContext sentinelDbContext, 
            IOptions<SentinelServiceOptions> options, 
            MasofaCommonDbContext masofaCommonDbContext, 
            IFileStorageProvider fileStorageProvider, 
            MasofaIndicesDbContext masofaIndicesDbContext)
        {
            Logger = logger;
            CopernicusApiUnitOfWork = copernicusApiUnitOfWork;
            SentinelDbContext = sentinelDbContext;
            SentinelServiceOptions = options.Value;
            SentinelServiceOptions.SatelliteSearchConfig = new SatelliteSearchConfig();
            MasofaCommonDbContext = masofaCommonDbContext;
            FileStorageProvider = fileStorageProvider;
            MasofaIndicesDbContext = masofaIndicesDbContext;
            InitializeGdal();
        }


        public async Task Execute()
        {
            try
            {
                await FetchStatus();

                var needProductsStates = SentinelDbContext.Sentinel2GenerateIndexStatus.ToList()
                    .Where(ps => !ps.IsTiffComplite).ToList();

                var needProductsQueueIds = needProductsStates.Select(m => m.Sentinel2ProductQueue).ToList();

                var needProductsQueue = SentinelDbContext.Sentinel2ProductsQueue
                    .Where(pq => needProductsQueueIds.Contains(pq.Id))
                    .ToList();

                var needProductIds = needProductsQueue.Select(pq => pq.ProductId).ToList();

                var needProducts = MasofaCommonDbContext.SatelliteProducts
                    .Where(sp => needProductIds.Contains(sp.ProductId))
                    .ToList();

                await GenerateTiffs(needProducts, needProductsQueue, needProductsStates, 3);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }

        #region QueueMethods
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
        }
        #endregion

        #region SupportMethods
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
        private void EnsureDirectoriesExistRecursive(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("The value cannot be an empty", nameof(filePath));
            }

            string directoryPath = Path.GetDirectoryName(filePath);

            Console.WriteLine($"{directoryPath}");

            if (string.IsNullOrEmpty(directoryPath))
            {
                return;
            }

            string[] pathParts = directoryPath.Split(Path.DirectorySeparatorChar);
            string currentPath = "/";

            foreach (string part in pathParts)
            {
                currentPath = Path.Combine(currentPath, part);

                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }
        private void DrawProgressBar(int line, int current, int total, string taksName, string message)
        {
            // Ограничиваем текущий прогресс
            current = Math.Min(current, total);
            int filledLength = ((int)Math.Round((double)current / total) * 100);

            string bar = new string('█', filledLength).PadRight(100);
            string text = $"{taksName}: [{bar}] {current}% of {total.ToString()}";
            Console.SetCursorPosition(0, line);
            Console.Write(message.PadRight(Console.WindowWidth - 1)); // затираем старый текст
            Console.SetCursorPosition(0, line + 1);
            Console.Write(text.PadRight(Console.WindowWidth - 1));
        }

        private void SaveGlobalResult()
        {
            var newResult = new List<Sentinel2GenerateIndexStatus>();
            var oldResult = new List<Sentinel2GenerateIndexStatus>();
            var current = SentinelDbContext.Sentinel2GenerateIndexStatus.ToList();

            foreach (var itemG in globalResult)
            {
                var tempItem = new Sentinel2GenerateIndexStatus()
                {
                    Sentinel2ProductQueue = itemG.Key
                };

                foreach (var itemI in itemG.Value)
                {
                    typeof(Sentinel2GenerateIndexStatus).GetProperty(itemI.Key).SetValue(tempItem, itemI.Value);
                }
                if(current.FirstOrDefault(m => m.Sentinel2ProductQueue == itemG.Key) != null)
                {
                    tempItem.Id = current.FirstOrDefault(m => m.Sentinel2ProductQueue == itemG.Key).Id;
                    oldResult.Add(tempItem);

                }
                newResult.Add(tempItem);
            }
            SentinelDbContext.Sentinel2GenerateIndexStatus.UpdateRange(oldResult);
            SentinelDbContext.Sentinel2GenerateIndexStatus.AddRange(oldResult);
            SentinelDbContext.SaveChanges();
        }
        #endregion

        #region Steps
        private async Task<List<Sentinel2ProductQueue>> GenerateTiffs(List<SatelliteProduct> needProducts,
            List<Sentinel2ProductQueue> needProductsQueue,
            List<Sentinel2GenerateIndexStatus> productQueueStatuses, int outputLine)
        {
            Console.WriteLine("Enter pls tempPath");
            TempPath = Console.ReadLine();
            Console.Clear();
            var result = new List<Sentinel2ProductQueue>();
            DrawProgressBar(outputLine, 0, needProducts.Count, "GenerateTiffProducts", $"Found {needProducts.Count} products to process.");
            var index = -1;


            foreach (var product in needProducts)
            {
                index++;
                DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Start for {index} of {needProducts.Count} product");

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
                        DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"FileStorageItem not found for MediadataPath: {product.MediadataPath}");
                        continue;
                    }

                    DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Start download : {zipFileStorageItem.FileStoragePath}");
                    var zipStream = await FileStorageProvider.GetFileStreamAsyncWithProgress(zipFileStorageItem);
                    string tempZipPath = Path.Combine(TempPath, $"{Guid.NewGuid()}_{product.ProductId}.zip");
                    string tempExtractRoot = Path.Combine(TempPath, $"extract_{Guid.NewGuid()}");
                    Directory.CreateDirectory(tempExtractRoot);
                    tempExtractPath = tempExtractRoot;

                    using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
                    {
                        await zipStream.CopyToAsync(fileStream);
                    }

                    DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Downloaded ZIP to: {tempZipPath}");
                    ZipFile.ExtractToDirectory(tempZipPath, tempExtractRoot);
                    DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Extracted ZIP to: {tempExtractRoot}");

                    var imgDataPath = IndicesHelper.FindImgDataPath(tempExtractRoot);
                    if (string.IsNullOrEmpty(imgDataPath))
                    {
                        DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", "IMG_DATA path could not be determined. Skipping product.");
                        continue;
                    }

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
                            DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Complite {tempItem.Key} for {needProductQueueItem.ProductId}");
                        }
                        catch (Exception ex) 
                        {
                            DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Exception: {ex.Message}");
                        }

                    }
                    SentinelDbContext.Sentinel2GenerateIndexStatus.Update(productStatus);
                    SentinelDbContext.SaveChanges();

                }
                catch (Exception ex)
                {
                    DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Exception: {ex.Message}");
                    continue;
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(tempExtractPath) && Directory.Exists(tempExtractPath))
                        {
                            Directory.Delete(tempExtractPath, true);
                            DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Cleaned up temp extract path: {tempExtractPath}");
                        }
                    }
                    catch (Exception cleanupEx)
                    {
                        DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Cleanup error: {cleanupEx.Message}");
                    }
                }
            }

            DrawProgressBar(outputLine, needProducts.Count, needProducts.Count, "GenerateTiffProducts", "ARVIJob completed successfully.");
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
                    OwnerTypeFullName = typeof(TIndexModel).FullName,
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



    }
}
