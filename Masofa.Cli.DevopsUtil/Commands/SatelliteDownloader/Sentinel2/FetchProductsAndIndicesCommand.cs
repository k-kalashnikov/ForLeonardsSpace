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
using System.Text;


namespace Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2
{
    [BaseCommand("Fetch Products And Indices Command", "Fetch Products And Indices Command")]
    public class FetchProductsAndIndicesCommand : IBaseCommand
    {
        ILogger<FetchProductsAndIndicesCommand> Logger { get; }
        private SentinelServiceOptions SentinelServiceOptions { get; }
        private CopernicusApiUnitOfWork CopernicusApiUnitOfWork { get; }
        private MasofaSentinelDbContext SentinelDbContext { get; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }


        private static bool GdalInitialized = false;
        private const double EPS = 1e-8;

        public FetchProductsAndIndicesCommand(ILogger<FetchProductsAndIndicesCommand> logger, CopernicusApiUnitOfWork copernicusApiUnitOfWork, MasofaSentinelDbContext sentinelDbContext, IOptions<SentinelServiceOptions> options, MasofaCommonDbContext masofaCommonDbContext, IFileStorageProvider fileStorageProvider, MasofaIndicesDbContext masofaIndicesDbContext)
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
            Console.WriteLine($"Start FetchProductsAndIndicesCommand");
            var reader = new NetTopologySuite.IO.WKTReader();
            var geometry = reader.Read($"POLYGON((56.00 37.00, 73.00 37.00, 73.00 45.50, 56.00 45.50 , 56.00 37.00))");

            if (geometry is NetTopologySuite.Geometries.Polygon polygon)
            {
                this.SentinelServiceOptions.SatelliteSearchConfig.SentinelPolygon = polygon;
            }


            var needProducts = await SearchProduct();
            Console.WriteLine($"Founded {needProducts.Count()} products");

            var metasProducts = await LoadMetaDataProducts();
            Console.WriteLine($"Metas loaded for {metasProducts.Count()} products");

            var mediaProducts = await LoadMediaProducts();
            Console.WriteLine($"Media loaded for {mediaProducts.Count()} products");

            var parsedProducts = await ParseProducts();
            Console.WriteLine($"Parsing complite for {parsedProducts.Count()} products");

            Console.WriteLine($"End FetchProductsAndIndicesCommand");
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        #region Steps
        private async Task<List<Sentinel2ProductQueue>> SearchProduct()
        {
            var startDate = new DateTime(2025, 10, 16);
            var endDate = new DateTime(2025, 12, 15);

            for (var currentDate = endDate; currentDate >= startDate; currentDate = currentDate.AddMonths(-1))
            {

                Console.WriteLine($"Search products from {currentDate.ToString("yyyy-MM-dd")} to {currentDate.AddMonths(-1).AddDays(1).ToString("yyyy-MM-dd")}");
                await CopernicusApiUnitOfWork.LoginAsync(SentinelServiceOptions);

                var productIds = await CopernicusApiUnitOfWork.ProductRepository.SearchProductAsync(SentinelServiceOptions, currentDate.AddMonths(-1).AddDays(1), currentDate);

                if (productIds == null || !productIds.Any())
                {
                    Console.WriteLine("No suitable products found for AOI.");
                    productIds = new List<Guid>();
                }

                var allQueue = await SentinelDbContext.Set<Sentinel2ProductQueue>().ToListAsync();

                foreach (var item in productIds)
                {
                    var currentProduct = allQueue.FirstOrDefault(m => m.ProductId == item.ToString());

                    if (currentProduct != null)
                    {
                        continue;
                    }

                    var tempPQ = new Sentinel2ProductQueue()
                    {
                        CreateAt = DateTime.UtcNow,
                        Id = Guid.NewGuid(),
                        ProductId = item.ToString(),
                        QueueStatus = ProductQueueStatusType.New,
                        Status = StatusType.Active,
                        CreateUser = Guid.Empty
                    };

                    await SentinelDbContext.Set<Sentinel2ProductQueue>().AddAsync(tempPQ);
                }

                await SentinelDbContext.SaveChangesAsync();
                await Task.Delay(Random.Shared.Next(2000, 4000));
            }

            return await SentinelDbContext.Set<Sentinel2ProductQueue>().ToListAsync();
        }
        private async Task<List<Sentinel2ProductQueue>> LoadMetaDataProducts()
        {
            var result = new List<Sentinel2ProductQueue>();

            var needProducts = await SentinelDbContext.Set<Sentinel2ProductQueue>()
                    .Where(m => m.QueueStatus == ProductQueueStatusType.New)
                    .ToListAsync();

            var existProducts = await MasofaCommonDbContext.SatelliteProducts
                .Where(m => needProducts.Select(m => m.ProductId).Contains(m.ProductId))
                .ToListAsync();

            var exitsMetadatas = await SentinelDbContext.Set<Sentinel2ProductMetadata>()
                .Where(m => existProducts.Select(m => m.Id.ToString()).Contains(m.ProductId))
                .ToListAsync();

            existProducts = existProducts.Where(p => exitsMetadatas.Select(pm => pm.ProductId).Contains(p.Id.ToString()))
                .ToList();

            needProducts = needProducts.Where(pq => !existProducts.Select(p => p.ProductId).Contains(pq.ProductId))
                .ToList();

            var index = 0;
            foreach (var item in needProducts)
            {
                try
                {
                    Console.WriteLine($"Try load metadata for product {item.ProductId} is {index} of {needProducts.Count()}");
                    if (index % 50 == 0)
                    {
                        Console.WriteLine($"ReAuth");
                        await CopernicusApiUnitOfWork.LoginAsync(SentinelServiceOptions);
                    }

                    var resultMeta = await CopernicusApiUnitOfWork.ProductRepository.GetProductMetadataAsync(this.SentinelServiceOptions, new Guid(item.ProductId));

                    var product = existProducts.FirstOrDefault(m => m.ProductId.Equals(item.ProductId));
                    if (product == null)
                    {
                        product = new SatelliteProduct()
                        {
                            ProductId = item.ProductId,
                            CreateAt = DateTime.UtcNow,
                            ProductSourceType = ProductSourceType.Sentinel2,
                            CreateUser = Guid.Empty,
                            Status = Common.Models.StatusType.Active,
                        };

                        await MasofaCommonDbContext.SatelliteProducts.AddAsync(product);
                    }

                    resultMeta.ProductId = product.Id.ToString();
                    await SentinelDbContext.Set<Sentinel2ProductMetadata>().AddAsync(resultMeta);
                    item.QueueStatus = ProductQueueStatusType.MetadataLoaded;
                    SentinelDbContext.Set<Sentinel2ProductQueue>().Update(item);
                    SentinelDbContext.SaveChanges();
                    MasofaCommonDbContext.SaveChanges();
                    Console.WriteLine($"Metadata loaded for product {item.ProductId} is {index} of {needProducts.Count()}");


                    result.Add(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException?.Message);
                }
                index++;
            }

            return result;
        }
        private async Task<List<Sentinel2ProductQueue>> LoadMediaProducts()
        {
            var result = new List<Sentinel2ProductQueue>();

            var productQueue = await SentinelDbContext.Set<Sentinel2ProductQueue>()
                .Where(m => m.QueueStatus == ProductQueueStatusType.MetadataLoaded)
                .ToListAsync();

            var needProducts = await MasofaCommonDbContext.SatelliteProducts
                .Where(m => productQueue.Select(m => m.ProductId).Contains(m.ProductId))
                .Where(m => m.MediadataPath.Equals(Guid.Empty))
                .ToListAsync();
            var tryIndex = 1;
            while (productQueue.Count > 0)
            {
                Console.WriteLine($"Trying {tryIndex}");
                var index = 0;
                foreach (var product in needProducts)
                {
                    try
                    {
                        if (index % 15 == 0)
                        {
                            Console.WriteLine($"ReAuth");
                            await CopernicusApiUnitOfWork.LoginAsync(SentinelServiceOptions);
                        }

                        var productStream = await CopernicusApiUnitOfWork.ProductRepository.GetProductMediadataAsync(this.SentinelServiceOptions, new Guid(product.ProductId));

                        using (productStream)
                        {
                            Console.WriteLine($"Downloaded product stream for {product.ProductId} is {index} of {needProducts.Count()}");
                            var filePath = string.Empty;
                            long fileLength = 0;
                            using (var mStream = new MemoryStream())
                            {
                                await productStream.CopyToAsync(mStream);
                                mStream.Position = 0;
                                fileLength = mStream.Length;
                                filePath = await FileStorageProvider.PushFileAsync(mStream, $"{product.ProductId}.zip", "sentinel");
                            }

                            var fileStorageItem = new FileStorageItem()
                            {
                                CreateAt = DateTime.UtcNow,
                                CreateUser = Guid.Empty,
                                OwnerId = product.Id,
                                OwnerTypeFullName = typeof(SatelliteProduct).FullName,
                                FileContentType = FileContentType.ArchiveZIP,
                                Status = Common.Models.StatusType.Active,
                                FileStoragePath = filePath,
                                FileStorageBacket = "sentinel",
                                FileLength = fileLength,
                            };
                            fileStorageItem = (await MasofaCommonDbContext.FileStorageItems.AddAsync(fileStorageItem)).Entity;
                            product.MediadataPath = fileStorageItem.Id;
                            MasofaCommonDbContext.SatelliteProducts.Update(product);
                        }
                        var tempPQ = productQueue.First(m => m.ProductId.Equals(product.ProductId));
                        tempPQ.QueueStatus = ProductQueueStatusType.MediaLoaded;
                        SentinelDbContext.Set<Sentinel2ProductQueue>().Update(tempPQ);
                        SentinelDbContext.SaveChanges();
                        MasofaCommonDbContext.SaveChanges();
                        result.Add(tempPQ);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.InnerException?.Message);
                    }
                    index++;
                }

                productQueue = await SentinelDbContext.Set<Sentinel2ProductQueue>()
                    .Where(m => m.QueueStatus == ProductQueueStatusType.MetadataLoaded)
                    .ToListAsync();

                needProducts = await MasofaCommonDbContext.SatelliteProducts
                    .Where(m => productQueue.Select(m => m.ProductId).Contains(m.ProductId))
                    .Where(m => m.MediadataPath.Equals(Guid.Empty))
                    .ToListAsync();
                tryIndex++;
            }



            return result;
        }
        private async Task<List<Sentinel2ProductQueue>> ParseProducts()
        {
            var result = new List<Sentinel2ProductQueue>();
            List<Sentinel2ProductQueue> productQueue = await SentinelDbContext.Set<Sentinel2ProductQueue>()
                .Where(m => m.QueueStatus == ProductQueueStatusType.MediaLoaded)
                .ToListAsync();

            var pqIds = productQueue.Select(m => m.ProductId).ToList();

            var products = await MasofaCommonDbContext.SatelliteProducts
                .Where(sp => pqIds.Contains(sp.ProductId ?? string.Empty))
                .ToListAsync();

            var pIds = products.Select(m => m.Id).ToList();

            var files = await MasofaCommonDbContext.FileStorageItems
                .Where(fs => fs.FileStorageBacket == "sentinel")
                .Where(fs => pIds.Contains(fs.OwnerId))
                .ToListAsync();

            var index = 0;

            foreach (var file in files)
            {
                try
                {
                    Console.WriteLine($"Processing archive: {file.FileStoragePath}");

                    //var archiveBytes = await FileStorageProvider.GetFileBytesAsyncWithProgress(file);
                    //using var archiveStream = new MemoryStream(archiveBytes);
                    using var archiveStream = await FileStorageProvider.GetFileStreamAsyncWithProgress(file);
                    using var zip = new ZipArchive(archiveStream, ZipArchiveMode.Read);

                    string? productId = null;
                    Guid? l1cMetadataId = null;
                    Guid? inspireMetadataId = null;
                    Guid? tileMetadataId = null;
                    Guid? qualityMetadataId = null;

                    // Сначала обрабатываем mtd_msil1c файл для извлечения даты спутника
                    DateTime? satelliteDate = null;
                    var mtdEntry = zip.Entries.FirstOrDefault(e => e.Name.ToLowerInvariant().Contains("mtd_msil1c"));

                    //if (mtdEntry != null)
                    //{
                    //    Console.WriteLine("Processing MTD_MSIL1C entry first to extract satellite date: {MtdEntry}", mtdEntry.FullName);

                    //    using var mtdStream = mtdEntry.Open();
                    //    var l1cParseResult = await Sentinel2ArchiveParserHelper.ParseL1CProductMetadataAsync(mtdStream);
                    //    l1cMetadataId = await SaveSentinelL1CParseResultAsync(l1cParseResult);

                    //    // Извлекаем дату спутника из L1C файла
                    //    satelliteDate = l1cParseResult.SatelliteDate;
                    //    productId = l1cParseResult.Metadata.ProductUri;

                    //    Console.WriteLine("Extracted satellite date from MTD_MSIL1C: {SatelliteDate}", satelliteDate);
                    //}

                    // Теперь обрабатываем остальные файлы, используя извлеченную дату
                    foreach (var entry in zip.Entries)
                    {
                        if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                        {
                            var name = entry.Name.ToLowerInvariant();

                            // Пропускаем уже обработанный mtd_msil1c файл
                            if (name.Contains("mtd_msil1c"))
                                continue;

                            if (name.Contains("inspire"))
                            {
                                using (var stream = new StreamReader(entry.Open()))
                                {
                                    string xmlContent = await stream.ReadToEndAsync();
                                    string simplifiedXml = Sentinel2ArchiveParserHelper.SimplifyInspireMetadataXml(xmlContent);
                                    var parseResult = Sentinel2ArchiveParserHelper.ParseInspireMetadataAsyncAsString(simplifiedXml);
                                    inspireMetadataId = await SaveSentinelInspireParseResultAsync(parseResult);
                                    Console.WriteLine($"INSPIRE metadata saved with ID: {inspireMetadataId}");
                                }

                            }
                            //else if (name.Contains("mtd_tl"))
                            //{
                            //    var parseResult = await Sentinel2ArchiveParserHelper.ParseL1CTileMetadataAsync(entryStream);
                            //    //Устанавливаем дату спутника из mtd_msil1c
                            //    if (satelliteDate.HasValue)
                            //    {
                            //        parseResult.Metadata.CreateAt = satelliteDate.Value;
                            //    }
                            //    tileMetadataId = await SaveSentinelTileParseResultAsync(parseResult);
                            //}
                            //else if (name.Contains("general_quality"))
                            //{
                            //    var parseResult = await Sentinel2ArchiveParserHelper.ParseProductQualityMetadataAsync(entryStream);
                            //    //Устанавливаем дату спутника из mtd_msil1c
                            //    if (satelliteDate.HasValue)
                            //    {
                            //        parseResult.Metadata.CreateAt = satelliteDate.Value;
                            //    }
                            //    qualityMetadataId = await SaveSentinelQualityParseResultAsync(parseResult);
                            //}
                        }
                    }

                    // Получаем продукт для дальнейшего использования
                    var pId = products.First(m => m.Id.Equals(file.OwnerId));

                    var sentinelProductEntity = new Sentinel2ProductEntity
                    {
                        SatellateProductId = pId.Id.ToString(),
                        SentinelL1CProductMetadataId = l1cMetadataId,
                        SentinelInspireMetadataId = inspireMetadataId,
                        SentinelL1CTileMetadataId = tileMetadataId,
                        SentinelProductQualityMetadataId = qualityMetadataId
                    };

                    SentinelDbContext.Sentinel2Products.Add(sentinelProductEntity);

                    // Обновляем статус в очереди
                    var productQueueItem = productQueue.First(pq => pq.ProductId.Equals(pId.ProductId));
                    productQueueItem.QueueStatus = ProductQueueStatusType.Parsed;
                    SentinelDbContext.Set<Sentinel2ProductQueue>().Update(productQueueItem);
                    await SentinelDbContext.SaveChangesAsync();
                    result.Add(productQueueItem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException?.Message);
                }
            }

            return result;
        }
        #endregion

        #region ParseArchive
        private async Task<Guid?> SaveSentinelL1CParseResultAsync(SentinelL1CParseResult parseResult)
        {
            if (parseResult.Metadata != null)
            {
                SentinelDbContext.SentinelL1CProductMetadata.Add(parseResult.Metadata);
                await SentinelDbContext.SaveChangesAsync();
                return parseResult.Metadata.Id;
            }
            return null;
        }

        private async Task<Guid?> SaveSentinelInspireParseResultAsync(SentinelInspireMetadata parseResult)
        {
            if (parseResult != null)
            {
                var result = (await SentinelDbContext.SentinelInspireMetadata.AddAsync(parseResult)).Entity;
                await SentinelDbContext.SaveChangesAsync();
                return result.Id;
            }
            return null;
        }

        private async Task<Guid?> SaveSentinelTileParseResultAsync(SentinelTileParseResult parseResult)
        {
            if (parseResult.Metadata != null)
            {
                SentinelDbContext.SentinelL1CTileMetadata.Add(parseResult.Metadata);
                await SentinelDbContext.SaveChangesAsync();
                return parseResult.Metadata.Id;
            }
            return null;
        }

        private async Task<Guid?> SaveSentinelQualityParseResultAsync(SentinelQualityParseResult parseResult)
        {
            if (parseResult.Metadata != null)
            {
                SentinelDbContext.SentinelProductQualityMetadata.Add(parseResult.Metadata);
                await SentinelDbContext.SaveChangesAsync();
                return parseResult.Metadata.Id;
            }
            return null;
        }

        private NetTopologySuite.Geometries.Polygon? CreatePolygonFromBoundingBox(decimal west, decimal south, decimal east, decimal north)
        {
            try
            {
                var coordinates = new[]
                {
                    new Coordinate((double)west, (double)south),
                    new Coordinate((double)east, (double)south),
                    new Coordinate((double)east, (double)north),
                    new Coordinate((double)west, (double)north),
                    new Coordinate((double)west, (double)south)
                };

                var shell = new NetTopologySuite.Geometries.LinearRing(coordinates);
                return new NetTopologySuite.Geometries.Polygon(shell);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        #region GenerateTiffs
        private async Task<List<CreateReportCommandResultItem>> CreateARVIs(List<CreateReportCommandResultItem> items)
        {
            var result = new List<CreateReportCommandResultItem>();
            foreach (var item in items)
            {
                var tempItem = item;
                var directoryPath = tempItem.ProductUnZipFilePath;
                var taskName = $"{nameof(GenerateTiffARVI)} with product {tempItem.ProductId} of {tempItem.OrinalDate}";
                var tiffFilePath = await GenerateTiffARVI(directoryPath, taskName);
                tempItem.ARVIFilePath = tiffFilePath;
                result.Add(tempItem);
                Console.WriteLine($"{taskName} complited");

            }
            return result;
        }

        private async Task<string> GenerateTiffARVI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Console.WriteLine("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }
            var b02Files = Directory.GetFiles(imgDataPath, "*B02.jp2", SearchOption.AllDirectories).ToList();
            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.AllDirectories).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.AllDirectories).ToList();

            if (!b04Files.Any() || !b08Files.Any() || !b02Files.Any())
            {
                Console.WriteLine("B02.jp2, B04.jp2 or B08.jp2 files not found.");
                return string.Empty;
            }

            Console.WriteLine($"Found {b02Files.Count} B02, {b04Files.Count} B04 files, {b08Files.Count} B08 files.");

            var granules = MatchGranulesARVI(b02Files, b04Files, b08Files);

            if (granules.Count == 0)
            {
                Console.WriteLine("Failed to match any B04/B08 pairs.");
                return string.Empty;
            }
            var result = string.Empty;
            try
            {
                string localArviColoredPath = await CreateColorfulARVITiff(granules[0].B02Path, granules[0].B04Path, granules[0].B08Path);
                Console.WriteLine($"Local ARVI GeoTIFF saved: {localArviColoredPath}");
                result = localArviColoredPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {Path.GetFileName(granules[0].B04Path)}: {ex.Message}", ex);
            }
            return result;
        }

        private async Task<List<CreateReportCommandResultItem>> CreateEVIs(List<CreateReportCommandResultItem> items)
        {
            var result = new List<CreateReportCommandResultItem>();
            foreach (var item in items)
            {
                var tempItem = item;
                var directoryPath = tempItem.ProductUnZipFilePath;
                var taskName = $"{nameof(GenerateTiffEVI)} with product {tempItem.ProductId} of {tempItem.OrinalDate}";
                var tiffFilePath = await GenerateTiffEVI(directoryPath, taskName);
                tempItem.EVIFilePath = tiffFilePath;
                result.Add(tempItem);
                Console.WriteLine($"{taskName} complited");
            }
            return result;
        }

        private async Task<string> GenerateTiffEVI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Console.WriteLine("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var b02Files = Directory.GetFiles(imgDataPath, "*B02.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b04Files.Any() || !b08Files.Any() || !b02Files.Any())
            {
                Console.WriteLine("B02.jp2, B04.jp2 or B08.jp2 files not found.");
                return string.Empty;
            }

            var b02Path = b02Files[0];
            var b04Path = b04Files[0];
            var b08Path = b08Files[0];

            Console.WriteLine($"Found {b02Files.Count} B02, {b04Files.Count} B04 files, {b08Files.Count} B08 files.");

            var granules = MatchGranulesEVI(b02Files, b04Files, b08Files);

            if (granules.Count == 0)
            {
                Console.WriteLine("Failed to match any B04/B08 pairs.");
                return string.Empty;
            }

            Console.WriteLine($"Found {granules.Count} granules to process.");
            var result = string.Empty;
            string localEviColoredPath = await CreateColorfulEVITiff(granules[0].B02Path, granules[0].B04Path, granules[0].B08Path);
            Console.WriteLine($"Local EVI GeoTIFF saved: {localEviColoredPath}");
            result = localEviColoredPath;

            Console.WriteLine("EVIJob completed successfully.");
            return result;
        }

        private async Task<List<CreateReportCommandResultItem>> CreateGNDVIs(List<CreateReportCommandResultItem> items)
        {
            var result = new List<CreateReportCommandResultItem>();
            foreach (var item in items)
            {
                var tempItem = item;
                var directoryPath = tempItem.ProductUnZipFilePath;
                var taskName = $"{nameof(GenerateTiffGNDVI)} with product {tempItem.ProductId} of {tempItem.OrinalDate}";
                var tiffFilePath = await GenerateTiffGNDVI(directoryPath, taskName);
                tempItem.GNDVIFilePath = tiffFilePath;
                result.Add(tempItem);
                Console.WriteLine($"{taskName} complited");
            }
            return result;
        }

        private async Task<string> GenerateTiffGNDVI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Console.WriteLine("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b08Files.Any())
            {
                Console.WriteLine("B03.jp2 or B08.jp2 files not found.");
                return string.Empty;
            }

            Console.WriteLine($"Found {b03Files.Count} B03, {b08Files.Count} B08 files.");

            var granules = MatchGranulesGNDVI(b03Files, b08Files);

            if (granules.Count == 0)
            {
                Console.WriteLine("Failed to match any B03/B08 pairs.");
                return string.Empty;
            }

            Console.WriteLine($"Found {granules.Count} granules to process.");
            var result = string.Empty;
            try
            {
                string gndviColoredTiffPath = await CreateColorfulGNDVITiff(granules[0].B03Path, granules[0].B08Path);
                result = gndviColoredTiffPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        private async Task<List<CreateReportCommandResultItem>> CreateMNDWIs(List<CreateReportCommandResultItem> items)
        {
            var result = new List<CreateReportCommandResultItem>();
            foreach (var item in items)
            {
                var tempItem = item;
                var directoryPath = tempItem.ProductUnZipFilePath;
                var taskName = $"{nameof(GenerateTiffMNDWI)} with product {tempItem.ProductId} of {tempItem.OrinalDate}";
                var tiffFilePath = await GenerateTiffMNDWI(directoryPath, taskName);
                tempItem.MNDWIFilePath = tiffFilePath;
                result.Add(tempItem);
                Console.WriteLine($"{taskName} complited");

            }
            return result;
        }

        private async Task<string> GenerateTiffMNDWI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Console.WriteLine("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var B03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var B11Files = Directory.GetFiles(imgDataPath, "*B11.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!B03Files.Any() || !B11Files.Any())
            {
                Console.WriteLine("B03.jp2 or B11.jp2 files not found.");
                return string.Empty;
            }

            Console.WriteLine($"Found {B03Files.Count} B03 files and {B11Files.Count} B11 files.");

            var granules = MatchGranulesMNDWI(B03Files, B11Files);

            if (granules.Count == 0)
            {
                Console.WriteLine("Failed to match any B03/B11 pairs.");
                return string.Empty;
            }

            Console.WriteLine($"Found {granules.Count} granules to process.");
            var result = string.Empty;
            try
            {
                string mndwiColoredTiffPath = await CreateColorfulMNDWITiff(granules[0].B03Path, granules[0].B11Path);
                result = mndwiColoredTiffPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {Path.GetFileName(granules[0].B03Path)}: {ex.Message}", ex);
            }
            return result;
        }

        private async Task<List<CreateReportCommandResultItem>> CreateNDMIs(List<CreateReportCommandResultItem> items)
        {
            var result = new List<CreateReportCommandResultItem>();
            foreach (var item in items)
            {
                var tempItem = item;
                var directoryPath = tempItem.ProductUnZipFilePath;
                var taskName = $"{nameof(GenerateTiffNDMI)} with product {tempItem.ProductId} of {tempItem.OrinalDate}";
                var tiffFilePath = await GenerateTiffNDMI(directoryPath, taskName);
                tempItem.NDMIFilePath = tiffFilePath;
                result.Add(tempItem);
                Console.WriteLine($"{taskName} complited");

            }
            return result;
        }

        private async Task<string> GenerateTiffNDMI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Console.WriteLine("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var B08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();
            var B11Files = Directory.GetFiles(imgDataPath, "*B11.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!B08Files.Any() || !B11Files.Any())
            {
                Console.WriteLine("B08.jp2 or B11.jp2 files not found.");
                return string.Empty;
            }

            Console.WriteLine($"Found {B08Files.Count} B08 files and {B11Files.Count} B11 files.");

            var granules = MatchGranulesNDMI(B08Files, B11Files);

            if (granules.Count == 0)
            {
                Console.WriteLine("Failed to match any B08/B11 pairs.");
                return string.Empty;
            }

            Console.WriteLine($"Found {granules.Count} granules to process.");

            var result = string.Empty;
            try
            {
                string ndmiColoredTiffPath = await CreateColorfulNDMITiff(granules[0].B08Path, granules[0].B11Path);
                result = ndmiColoredTiffPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {Path.GetFileName(granules[0].B08Path)}: {ex.Message}", ex);
            }
            return result;
        }

        private async Task<List<CreateReportCommandResultItem>> CreateNDVIs(List<CreateReportCommandResultItem> items)
        {
            var result = new List<CreateReportCommandResultItem>();
            foreach (var item in items)
            {
                var tempItem = item;
                var directoryPath = tempItem.ProductUnZipFilePath;
                var taskName = $"{nameof(GenerateTiffNDVI)} with product {tempItem.ProductId} of {tempItem.OrinalDate}";
                var tiffFilePath = await GenerateTiffNDVI(directoryPath, taskName);
                tempItem.NDVIFilePath = tiffFilePath;
                result.Add(tempItem);
                Console.WriteLine($"{taskName} complited");

            }
            return result;
        }

        private async Task<string> GenerateTiffNDVI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Console.WriteLine("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b04Files.Any() || !b08Files.Any())
            {
                Console.WriteLine("B04.jp2 or B08.jp2 files not found.");
                return string.Empty;
            }

            Console.WriteLine($"Found {b04Files.Count} B04 files and {b08Files.Count} B08 files.");

            var granules = MatchGranulesNDVI(b04Files, b08Files);

            if (granules.Count == 0)
            {
                Console.WriteLine("Failed to match any B04/B08 pairs.");
                return string.Empty;
            }

            Console.WriteLine($"Found {granules.Count} granules to process.");
            var result = string.Empty;
            try
            {
                string ndviColoredTiffPath = await CreateColorfulNDVITiff(granules[0].B04Path, granules[0].B08Path);
                result = ndviColoredTiffPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {Path.GetFileName(granules[0].B04Path)}: {ex.Message}", ex);
            }

            Console.WriteLine("NDVIJob completed successfully.");
            return result;
        }

        private async Task<List<CreateReportCommandResultItem>> CreateORVIs(List<CreateReportCommandResultItem> items)
        {
            var result = new List<CreateReportCommandResultItem>();
            foreach (var item in items)
            {
                var tempItem = item;
                var directoryPath = tempItem.ProductUnZipFilePath;
                var taskName = $"{nameof(GenerateTiffORVI)} with product {tempItem.ProductId} of {tempItem.OrinalDate}";
                var tiffFilePath = await GenerateTiffORVI(directoryPath, taskName);
                tempItem.ORVIFilePath = tiffFilePath;
                result.Add(tempItem);
                Console.WriteLine($"{taskName} complited");

            }
            return result;
        }

        private async Task<string> GenerateTiffORVI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Console.WriteLine("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b08Files.Any())
            {
                Console.WriteLine("B03.jp2 or B08.jp2 files not found.");
                return string.Empty;
            }

            Console.WriteLine($"Found {b03Files.Count} B03 files and {b08Files.Count} B08 files.");

            var granules = MatchGranulesORVI(b03Files, b08Files);

            if (granules.Count == 0)
            {
                Console.WriteLine("Failed to match any B03/B08 pairs.");
                return string.Empty;
            }

            Console.WriteLine($"Found {granules.Count} granules to process.");
            var result = string.Empty;
            try
            {
                string orviColoredTiffPath = await CreateColorfulORVITiff(granules[0].B03Path, granules[0].B08Path);
                result = orviColoredTiffPath;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing granule: {ex.Message}", ex);
            }
            return result;
        }

        private async Task<List<CreateReportCommandResultItem>> CreateOSAVIs(List<CreateReportCommandResultItem> items)
        {
            var result = new List<CreateReportCommandResultItem>();
            foreach (var item in items)
            {
                var tempItem = item;
                var directoryPath = tempItem.ProductUnZipFilePath;
                var taskName = $"{nameof(GenerateTiffOSAVI)} with product {tempItem.ProductId} of {tempItem.OrinalDate}";
                var tiffFilePath = await GenerateTiffOSAVI(directoryPath, taskName);
                tempItem.OSAVIFilePath = tiffFilePath;
                result.Add(tempItem);
                Console.WriteLine($"{taskName} complited");

            }
            return result;
        }

        private async Task<string> GenerateTiffOSAVI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Console.WriteLine("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b04Files.Any() || !b08Files.Any())
            {
                Console.WriteLine("B04.jp2 or B08.jp2 files not found.");
                return string.Empty;
            }

            Console.WriteLine($"Found {b04Files.Count} B04 files and {b08Files.Count} B08 files.");

            var granules = MatchGranulesOSAVI(b04Files, b08Files);

            if (granules.Count == 0)
            {
                Console.WriteLine("Failed to match any B04/B08 pairs.");
                return string.Empty;
            }

            Console.WriteLine($"Found {granules.Count} granules to process.");

            var result = string.Empty;
            try
            {
                string osaviColoredTiffPath = await CreateColorfulOSAVITiff(granules[0].B04Path, granules[0].B08Path);
                result = osaviColoredTiffPath;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing granule: {ex.Message}", ex);
            }
            return result;
        }

        #endregion

        #region GenerateTiffSupport
        private string FindImgDataPath(string extractRoot)
        {
            var granuleDir = Directory.GetDirectories(extractRoot, "GRANULE", SearchOption.AllDirectories).FirstOrDefault();
            if (granuleDir == null)
            {
                Console.WriteLine("GRANULE directory not found.");
                return null;
            }

            var l1cDirs = Directory.GetDirectories(granuleDir, "L1C_T*", SearchOption.AllDirectories);
            if (l1cDirs.Length == 0)
            {
                Console.WriteLine("No L1C_T* directory found inside GRANULE.");
                return null;
            }

            var l1cDir = l1cDirs[0];

            var imgDataDir = Path.Combine(l1cDir, "IMG_DATA");
            if (!Directory.Exists(imgDataDir))
            {
                Console.WriteLine("IMG_DATA directory not found inside L1C_T* folder.");
                return null;
            }

            return imgDataDir;
        }
        private static List<(string B02Path, string B04Path, string B08Path)> MatchGranulesARVI(List<string> b02Files, List<string> b04Files, List<string> b08Files)
        {
            var granules = new List<(string B02Path, string B04Path, string B08Path)>();

            foreach (var b04 in b04Files)
            {
                string fileName = Path.GetFileName(b04);
                if (!fileName.EndsWith("_B04.jp2")) continue;

                string basePart = fileName.Replace("_B04.jp2", "");
                string expectedB02 = basePart + "_B02.jp2";
                string expectedB08 = basePart + "_B08.jp2";

                string b02Match = b02Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB02);
                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b02Match != null && b08Match != null)
                {
                    granules.Add((b02Match, b04, b08Match));
                }
            }

            return granules;
        }
        private async Task<string> CreateColorfulARVITiff(string b02Path, string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_ARVI_vis") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b02Dataset = Gdal.Open(b02Path, Access.GA_ReadOnly); // BLUE
            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly); // RED
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b02Dataset == null || b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b02Path}, {b04Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b04Dataset.RasterXSize || height != b04Dataset.RasterYSize ||
                width != b02Dataset.RasterXSize || height != b02Dataset.RasterYSize)
            {
                throw new Exception("B02, B04 and B08 must have the same dimensions!");
            }

            var b02Band = b02Dataset.GetRasterBand(1); // BLUE
            var b04Band = b04Dataset.GetRasterBand(1); // RED
            var b08Band = b08Dataset.GetRasterBand(1); // NIR

            var b02Buffer = new float[width * height];
            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b02Scale, b02Offset;
            int b02HasScale, b02HasOffset;
            b02Band.GetScale(out b02Scale, out b02HasScale);
            b02Band.GetOffset(out b02Offset, out b02HasOffset);

            double b04Scale, b04Offset;
            int b04HasScale, b04HasOffset;
            b04Band.GetScale(out b04Scale, out b04HasScale);
            b04Band.GetOffset(out b04Offset, out b04HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b02HasScale == 0)
            {
                Console.WriteLine("B02 scale not found. Using default 0.0001");
                b02Scale = 0.0001;
            }
            if (b04HasScale == 0 || b04Scale == 0)
            {
                Console.WriteLine("B04 scale not found. Using default 0.0001");
                b04Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                Console.WriteLine("B08 scale not found. Using default 0.0001");
                b08Scale = 0.0001;
            }

            Console.WriteLine($"B02 Scale: {b02Scale}, Offset: {b02Offset}");
            Console.WriteLine($"B04 Scale: {b04Scale}, Offset: {b04Offset}");
            Console.WriteLine($"B08 Scale: {b08Scale}, Offset: {b08Offset}");

            b02Band.ReadRaster(0, 0, width, height, b02Buffer, width, height, 0, 0);
            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b02Buffer.Length; i++)
            {
                b02Buffer[i] = (float)(b02Offset + b02Buffer[i] * b02Scale);
                b04Buffer[i] = (float)(b04Offset + b04Buffer[i] * b04Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                // Защита от NaN/Infinity
                if (float.IsNaN(b02Buffer[i]) || float.IsInfinity(b02Buffer[i])) b02Buffer[i] = 0f;
                if (float.IsNaN(b04Buffer[i]) || float.IsInfinity(b04Buffer[i])) b04Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var arviBuffer = new float[width * height];
            int noDataCount = 0;

            const float G = 2.5f;
            const float C1 = 6.0f;
            const float C2 = 7.5f;
            const float L = 1.0f;
            const float EPS = 1e-9f;

            for (int i = 0; i < arviBuffer.Length; i++)
            {
                float blue = b02Buffer[i];
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                // Фильтруем нереальные или нулевые значения
                if (blue <= 0 || red <= 0 || nir <= 0)
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + (2 * red) + blue;
                if (Math.Abs(denominator) < EPS)
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float arvi = (nir - (2 * red) + blue) / denominator;

                // Защита от NaN/Infinity
                if (float.IsNaN(arvi) || float.IsInfinity(arvi))
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                // Опционально: обрезаем по физически возможному диапазону EVI
                // MODIS EVI обычно от -0.2 до 1.0, но в Sentinel-2 может быть немного шире
                if (arvi < -1.0f || arvi > 1.5f)
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                arviBuffer[i] = arvi;
            }

            Console.WriteLine($"ARVI computed. NoData pixels: {noDataCount}");

            // 📊 Находим реальный min/max среди валидных данных
            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < arviBuffer.Length; i++)
            {
                if (arviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (arviBuffer[i] < actualMin) actualMin = arviBuffer[i];
                    if (arviBuffer[i] > actualMax) actualMax = arviBuffer[i];
                }
            }

            // Защита от случая, когда все значения одинаковы
            if (actualMin == actualMax || validPixelCount == 0)
            {
                Console.WriteLine("Warning: All ARVI values are identical or no valid pixels. Using default range [-0.2, 1.0].");
                actualMin = -0.2f;
                actualMax = 1.0f;
            }

            Console.WriteLine($"Valid ARVI Range: {actualMin:F4} to {actualMax:F4} (based on {validPixelCount} pixels)");

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < arviBuffer.Length; i++)
            {
                if (arviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → прозрачный
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (arviBuffer[i] - actualMin) / (actualMax - actualMin);
                    // Clamp к [0, 1] на случай выбросов
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var arviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            arviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            arviDataset.SetProjection(projection);

            var arviBand = arviDataset.GetRasterBand(1);
            arviBand.SetNoDataValue(255);

            // 🎨 Цветовая палитра для ARVI: от коричневого → жёлтого → светло-зелёного → тёмно-зелёного
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Хелперы
            static byte Lerp(byte a, byte b, double t) =>
                (byte)Math.Clamp(Math.Round(a + (b - a) * t), 0, 255);
            static (byte r, byte g, byte b) Lerp3((byte r, byte g, byte b) c1, (byte r, byte g, byte b) c2, double t) =>
                (Lerp(c1.r, c2.r, t), Lerp(c1.g, c2.g, t), Lerp(c1.b, c2.b, t));

            // Узловые цвета (подобраны для высокого контраста)
            var waterLo = (r: (byte)70, g: (byte)85, b: (byte)115); // глубокая вода/тени
            var waterHi = (r: (byte)95, g: (byte)110, b: (byte)140); // мелкая вода/асфальт/низкий ARVI<0
            var soilLo = (r: (byte)150, g: (byte)90, b: (byte)40);  // коричневый (почва)
            var soilHi = (r: (byte)205, g: (byte)140, b: (byte)60);  // охра
            var yellow = (r: (byte)255, g: (byte)230, b: (byte)40);  // яркий жёлтый (стресс)
            var greenLo = (r: (byte)170, g: (byte)235, b: (byte)100); // светло-зелёный
            var greenMd = (r: (byte)70, g: (byte)190, b: (byte)85);  // средний зелёный
            var greenHi = (r: (byte)10, g: (byte)110, b: (byte)35);  // тёмно-зелёный (густая растительность)

            // Пороговые значения ARVI (можешь подправить при желании)
            double t0 = 0.0;  // вода/асфальт ниже 0
            double t1 = 0.2;  // почва/оч. слабая
            double t2 = 0.4;  // стресс → начало вегетации
            double t3 = 0.6;  // нормальная растительность
            double t4 = 0.8;  // хорошая растительность

            // NoData — прозрачный
            colorTable.SetColorEntry(255, new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 });

            // 1..254 — валидные значения; 0 мы не используем (индекс всегда >=1 при твоём маппинге)
            for (int idx = 1; idx <= 254; idx++)
            {
                // восстановим приблизительный ARVI из индекса (линейная обратная нормализация)
                double arvi = actualMin + (idx / 254.0) * (actualMax - actualMin);

                ColorEntry ce;

                if (arvi < t0)
                {
                    // < 0.0: вода/асфальт — сине-серые, чтобы резко отличались от почвы
                    double t = Math.Clamp((arvi - (t0 - 0.2)) / (0.2), 0, 1); // плавный переход -0.2..0.0
                    var c = Lerp3(waterLo, waterHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (arvi < t1)
                {
                    // 0.0–0.2: почва — от тёмной к охре (контрастно относительно воды)
                    double t = (arvi - t0) / (t1 - t0);
                    var c = Lerp3(soilLo, soilHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (arvi < t2)
                {
                    // 0.2–0.4: охра → яркий жёлтый (стресс выделяется)
                    double t = (arvi - t1) / (t2 - t1);
                    var c = Lerp3(soilHi, yellow, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (arvi < t3)
                {
                    // 0.4–0.6: жёлтый → светло-зелёный (резкий поворот в «зелень»)
                    double t = (arvi - t2) / (t3 - t2);
                    var c = Lerp3(yellow, greenLo, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (arvi < t4)
                {
                    // 0.6–0.8: светло-зелёный → зелёный
                    double t = (arvi - t3) / (t4 - t3);
                    var c = Lerp3(greenLo, greenMd, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else
                {
                    // ≥ 0.8: зелёный → тёмно-зелёный (густая листовая масса)
                    // небольшой «подзавал» в тёмные тона для визуальной доминанты
                    double t = Math.Clamp((arvi - t4) / 0.3, 0, 1); // до ~1.1
                    var c = Lerp3(greenMd, greenHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }

                colorTable.SetColorEntry(idx, ce);
            }

            arviBand.SetRasterColorTable(colorTable);
            arviBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            arviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            Console.WriteLine($"ARVI GeoTIFF saved: {outputPath}");
            return outputPath;
        }
        private async Task<string> CreateGrayscaleARVITiff(string b02Path, string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_ARVI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                Console.WriteLine($"Grayscale ARVI file already exists: {outputPath}");
                return outputPath;
            }

            using var b02Dataset = Gdal.Open(b02Path, Access.GA_ReadOnly); // BLUE
            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly); // RED
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b02Dataset == null || b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b02Path}, {b04Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b04Dataset.RasterXSize || height != b04Dataset.RasterYSize ||
                width != b02Dataset.RasterXSize || height != b02Dataset.RasterYSize)
            {
                throw new Exception("B02, B04 and B08 must have the same dimensions!");
            }

            var b02Band = b02Dataset.GetRasterBand(1);
            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b02Buffer = new float[width * height];
            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b02Scale, b02Offset;
            int b02HasScale, b02HasOffset;
            b02Band.GetScale(out b02Scale, out b02HasScale);
            b02Band.GetOffset(out b02Offset, out b02HasOffset);

            double b04Scale, b04Offset;
            int b04HasScale, b04HasOffset;
            b04Band.GetScale(out b04Scale, out b04HasScale);
            b04Band.GetOffset(out b04Offset, out b04HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b02HasScale == 0 || b02Scale == 0) b02Scale = 0.0001;
            if (b04HasScale == 0 || b04Scale == 0) b04Scale = 0.0001;
            if (b08HasScale == 0 || b08Scale == 0) b08Scale = 0.0001;

            b02Band.ReadRaster(0, 0, width, height, b02Buffer, width, height, 0, 0);
            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b02Buffer.Length; i++)
            {
                b02Buffer[i] = (float)(b02Offset + b02Buffer[i] * b02Scale);
                b04Buffer[i] = (float)(b04Offset + b04Buffer[i] * b04Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b02Buffer[i]) || float.IsInfinity(b02Buffer[i])) b02Buffer[i] = 0f;
                if (float.IsNaN(b04Buffer[i]) || float.IsInfinity(b04Buffer[i])) b04Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var arviBuffer = new float[width * height];
            int noDataCount = 0;

            const float G = 2.5f;
            const float C1 = 6.0f;
            const float C2 = 7.5f;
            const float L = 1.0f;
            const float EPS = 1e-9f;

            for (int i = 0; i < arviBuffer.Length; i++)
            {
                float blue = b02Buffer[i];
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                if (blue <= 0 || red <= 0 || nir <= 0)
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + C1 * red - C2 * blue + L;
                if (Math.Abs(denominator) < EPS)
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float arvi = G * (nir - red) / denominator;

                if (float.IsNaN(arvi) || float.IsInfinity(arvi))
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                if (arvi < -1.0f || arvi > 1.5f)
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                arviBuffer[i] = arvi;
            }

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < arviBuffer.Length; i++)
            {
                if (arviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (arviBuffer[i] < actualMin) actualMin = arviBuffer[i];
                    if (arviBuffer[i] > actualMax) actualMax = arviBuffer[i];
                }
            }

            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = -0.2f;
                actualMax = 1.0f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < arviBuffer.Length; i++)
            {
                if (arviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → белый или прозрачный
                }
                else
                {
                    // Вариант A: Чёрное = низкий ARVI, Белое = высокий ARVI (рекомендуется)
                    float normalized = (arviBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)scaled;

                    /*
                    // Вариант B: ИНВЕРТИРОВАННЫЙ — Белое = низкий ARVI, Чёрное = высокий ARVI
                    float normalized = (arviBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round((1 - normalized) * 254);
                    byteBuffer[i] = (byte)scaled;
                    */
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var arviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            arviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            arviDataset.SetProjection(projection);

            var arviBand = arviDataset.GetRasterBand(1);
            arviBand.SetNoDataValue(255);

            arviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        private static List<(string B02Path, string B04Path, string B08Path)> MatchGranulesEVI(List<string> b02Files, List<string> b04Files, List<string> b08Files)
        {
            var granules = new List<(string B02Path, string B04Path, string B08Path)>();

            foreach (var b04 in b04Files)
            {
                string fileName = Path.GetFileName(b04);
                if (!fileName.EndsWith("_B04.jp2")) continue;

                string basePart = fileName.Replace("_B04.jp2", "");
                string expectedB02 = basePart + "_B02.jp2";
                string expectedB08 = basePart + "_B08.jp2";

                string b02Match = b02Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB02);
                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b02Match != null && b08Match != null)
                {
                    granules.Add((b02Match, b04, b08Match));
                }
            }

            return granules;
        }
        private async Task<string> CreateColorfulEVITiff(string b02Path, string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_EVI_vis3") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b02Dataset = Gdal.Open(b02Path, Access.GA_ReadOnly); // BLUE
            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly); // RED
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b02Dataset == null || b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b02Path}, {b04Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b04Dataset.RasterXSize || height != b04Dataset.RasterYSize ||
                width != b02Dataset.RasterXSize || height != b02Dataset.RasterYSize)
            {
                throw new Exception("B02, B04 and B08 must have the same dimensions!");
            }

            var b02Band = b02Dataset.GetRasterBand(1); // BLUE
            var b04Band = b04Dataset.GetRasterBand(1); // RED
            var b08Band = b08Dataset.GetRasterBand(1); // NIR

            var b02Buffer = new float[width * height];
            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b02Scale, b02Offset;
            int b02HasScale, b02HasOffset;
            b02Band.GetScale(out b02Scale, out b02HasScale);
            b02Band.GetOffset(out b02Offset, out b02HasOffset);

            double b04Scale, b04Offset;
            int b04HasScale, b04HasOffset;
            b04Band.GetScale(out b04Scale, out b04HasScale);
            b04Band.GetOffset(out b04Offset, out b04HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b02HasScale == 0)
            {
                Console.WriteLine("B02 scale not found. Using default 0.0001");
                b02Scale = 0.0001;
            }
            if (b04HasScale == 0 || b04Scale == 0)
            {
                Console.WriteLine("B04 scale not found. Using default 0.0001");
                b04Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                Console.WriteLine("B08 scale not found. Using default 0.0001");
                b08Scale = 0.0001;
            }

            Console.WriteLine($"B02 Scale: {b02Scale}, Offset: {b02Offset}");
            Console.WriteLine($"B04 Scale: {b04Scale}, Offset: {b04Offset}");
            Console.WriteLine($"B08 Scale: {b08Scale}, Offset: {b08Offset}");

            b02Band.ReadRaster(0, 0, width, height, b02Buffer, width, height, 0, 0);
            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b02Buffer.Length; i++)
            {
                b02Buffer[i] = (float)(b02Offset + b02Buffer[i] * b02Scale);
                b04Buffer[i] = (float)(b04Offset + b04Buffer[i] * b04Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                // Защита от NaN/Infinity
                if (float.IsNaN(b02Buffer[i]) || float.IsInfinity(b02Buffer[i])) b02Buffer[i] = 0f;
                if (float.IsNaN(b04Buffer[i]) || float.IsInfinity(b04Buffer[i])) b04Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var eviBuffer = new float[width * height];
            int noDataCount = 0;

            const float G = 2.5f;
            const float C1 = 6.0f;
            const float C2 = 7.5f;
            const float L = 1.0f;
            const float EPS = 1e-9f;

            for (int i = 0; i < eviBuffer.Length; i++)
            {
                float blue = b02Buffer[i];
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                // Фильтруем нереальные или нулевые значения
                if (blue <= 0 || red <= 0 || nir <= 0)
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + C1 * red - C2 * blue + L;
                if (Math.Abs(denominator) < EPS)
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float evi = G * (nir - red) / denominator;

                // Защита от NaN/Infinity
                if (float.IsNaN(evi) || float.IsInfinity(evi))
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                // Опционально: обрезаем по физически возможному диапазону EVI
                // MODIS EVI обычно от -0.2 до 1.0, но в Sentinel-2 может быть немного шире
                if (evi < -1.0f || evi > 1.5f)
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                eviBuffer[i] = evi;
            }

            Console.WriteLine($"EVI computed. NoData pixels: {noDataCount}");

            // 📊 Находим реальный min/max среди валидных данных
            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < eviBuffer.Length; i++)
            {
                if (eviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (eviBuffer[i] < actualMin) actualMin = eviBuffer[i];
                    if (eviBuffer[i] > actualMax) actualMax = eviBuffer[i];
                }
            }

            // Защита от случая, когда все значения одинаковы
            if (actualMin == actualMax || validPixelCount == 0)
            {
                Console.WriteLine("Warning: All EVI values are identical or no valid pixels. Using default range [-0.2, 1.0].");
                actualMin = -0.2f;
                actualMax = 1.0f;
            }

            Console.WriteLine($"Valid EVI Range: {actualMin:F4} to {actualMax:F4} (based on {validPixelCount} pixels)");

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < eviBuffer.Length; i++)
            {
                if (eviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → прозрачный
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (eviBuffer[i] - actualMin) / (actualMax - actualMin);
                    // Clamp к [0, 1] на случай выбросов
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var eviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            eviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            eviDataset.SetProjection(projection);

            var eviBand = eviDataset.GetRasterBand(1);
            eviBand.SetNoDataValue(255);

            // 🎨 Цветовая палитра для EVI: от коричневого → жёлтого → светло-зелёного → тёмно-зелёного
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Хелперы
            static byte Lerp(byte a, byte b, double t) =>
                (byte)Math.Clamp(Math.Round(a + (b - a) * t), 0, 255);
            static (byte r, byte g, byte b) Lerp3((byte r, byte g, byte b) c1, (byte r, byte g, byte b) c2, double t) =>
                (Lerp(c1.r, c2.r, t), Lerp(c1.g, c2.g, t), Lerp(c1.b, c2.b, t));

            // Узловые цвета (подобраны для высокого контраста)
            var waterLo = (r: (byte)70, g: (byte)85, b: (byte)115); // глубокая вода/тени
            var waterHi = (r: (byte)95, g: (byte)110, b: (byte)140); // мелкая вода/асфальт/низкий EVI<0
            var soilLo = (r: (byte)150, g: (byte)90, b: (byte)40);  // коричневый (почва)
            var soilHi = (r: (byte)205, g: (byte)140, b: (byte)60);  // охра
            var yellow = (r: (byte)255, g: (byte)230, b: (byte)40);  // яркий жёлтый (стресс)
            var greenLo = (r: (byte)170, g: (byte)235, b: (byte)100); // светло-зелёный
            var greenMd = (r: (byte)70, g: (byte)190, b: (byte)85);  // средний зелёный
            var greenHi = (r: (byte)10, g: (byte)110, b: (byte)35);  // тёмно-зелёный (густая растительность)

            // Пороговые значения EVI (можешь подправить при желании)
            double t0 = 0.0;  // вода/асфальт ниже 0
            double t1 = 0.2;  // почва/оч. слабая
            double t2 = 0.4;  // стресс → начало вегетации
            double t3 = 0.6;  // нормальная растительность
            double t4 = 0.8;  // хорошая растительность

            // NoData — прозрачный
            colorTable.SetColorEntry(255, new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 });

            // 1..254 — валидные значения; 0 мы не используем (индекс всегда >=1 при твоём маппинге)
            for (int idx = 1; idx <= 254; idx++)
            {
                // восстановим приблизительный EVI из индекса (линейная обратная нормализация)
                double evi = actualMin + (idx / 254.0) * (actualMax - actualMin);

                ColorEntry ce;

                if (evi < t0)
                {
                    // < 0.0: вода/асфальт — сине-серые, чтобы резко отличались от почвы
                    double t = Math.Clamp((evi - (t0 - 0.2)) / (0.2), 0, 1); // плавный переход -0.2..0.0
                    var c = Lerp3(waterLo, waterHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (evi < t1)
                {
                    // 0.0–0.2: почва — от тёмной к охре (контрастно относительно воды)
                    double t = (evi - t0) / (t1 - t0);
                    var c = Lerp3(soilLo, soilHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (evi < t2)
                {
                    // 0.2–0.4: охра → яркий жёлтый (стресс выделяется)
                    double t = (evi - t1) / (t2 - t1);
                    var c = Lerp3(soilHi, yellow, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (evi < t3)
                {
                    // 0.4–0.6: жёлтый → светло-зелёный (резкий поворот в «зелень»)
                    double t = (evi - t2) / (t3 - t2);
                    var c = Lerp3(yellow, greenLo, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (evi < t4)
                {
                    // 0.6–0.8: светло-зелёный → зелёный
                    double t = (evi - t3) / (t4 - t3);
                    var c = Lerp3(greenLo, greenMd, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else
                {
                    // ≥ 0.8: зелёный → тёмно-зелёный (густая листовая масса)
                    // небольшой «подзавал» в тёмные тона для визуальной доминанты
                    double t = Math.Clamp((evi - t4) / 0.3, 0, 1); // до ~1.1
                    var c = Lerp3(greenMd, greenHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }

                colorTable.SetColorEntry(idx, ce);
            }

            eviBand.SetRasterColorTable(colorTable);
            eviBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            eviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            Console.WriteLine($"✅ EVI GeoTIFF saved: {outputPath}");
            return outputPath;
        }
        private async Task<string> CreateGrayscaleEVITiff(string b02Path, string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_EVI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                Console.WriteLine($"Grayscale EVI file already exists: {outputPath}");
                return outputPath;
            }

            using var b02Dataset = Gdal.Open(b02Path, Access.GA_ReadOnly); // BLUE
            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly); // RED
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b02Dataset == null || b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b02Path}, {b04Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b04Dataset.RasterXSize || height != b04Dataset.RasterYSize ||
                width != b02Dataset.RasterXSize || height != b02Dataset.RasterYSize)
            {
                throw new Exception("B02, B04 and B08 must have the same dimensions!");
            }

            var b02Band = b02Dataset.GetRasterBand(1);
            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b02Buffer = new float[width * height];
            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b02Scale, b02Offset;
            int b02HasScale, b02HasOffset;
            b02Band.GetScale(out b02Scale, out b02HasScale);
            b02Band.GetOffset(out b02Offset, out b02HasOffset);

            double b04Scale, b04Offset;
            int b04HasScale, b04HasOffset;
            b04Band.GetScale(out b04Scale, out b04HasScale);
            b04Band.GetOffset(out b04Offset, out b04HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b02HasScale == 0 || b02Scale == 0) b02Scale = 0.0001;
            if (b04HasScale == 0 || b04Scale == 0) b04Scale = 0.0001;
            if (b08HasScale == 0 || b08Scale == 0) b08Scale = 0.0001;

            b02Band.ReadRaster(0, 0, width, height, b02Buffer, width, height, 0, 0);
            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b02Buffer.Length; i++)
            {
                b02Buffer[i] = (float)(b02Offset + b02Buffer[i] * b02Scale);
                b04Buffer[i] = (float)(b04Offset + b04Buffer[i] * b04Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b02Buffer[i]) || float.IsInfinity(b02Buffer[i])) b02Buffer[i] = 0f;
                if (float.IsNaN(b04Buffer[i]) || float.IsInfinity(b04Buffer[i])) b04Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var eviBuffer = new float[width * height];
            int noDataCount = 0;

            const float G = 2.5f;
            const float C1 = 6.0f;
            const float C2 = 7.5f;
            const float L = 1.0f;
            const float EPS = 1e-9f;

            for (int i = 0; i < eviBuffer.Length; i++)
            {
                float blue = b02Buffer[i];
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                if (blue <= 0 || red <= 0 || nir <= 0)
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + C1 * red - C2 * blue + L;
                if (Math.Abs(denominator) < EPS)
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float evi = G * (nir - red) / denominator;

                if (float.IsNaN(evi) || float.IsInfinity(evi))
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                if (evi < -1.0f || evi > 1.5f)
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                eviBuffer[i] = evi;
            }

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < eviBuffer.Length; i++)
            {
                if (eviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (eviBuffer[i] < actualMin) actualMin = eviBuffer[i];
                    if (eviBuffer[i] > actualMax) actualMax = eviBuffer[i];
                }
            }

            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = -0.2f;
                actualMax = 1.0f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < eviBuffer.Length; i++)
            {
                if (eviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → белый или прозрачный
                }
                else
                {
                    // Вариант A: Чёрное = низкий EVI, Белое = высокий EVI (рекомендуется)
                    float normalized = (eviBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)scaled;

                    /*
                    // Вариант B: ИНВЕРТИРОВАННЫЙ — Белое = низкий EVI, Чёрное = высокий EVI
                    float normalized = (eviBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round((1 - normalized) * 254);
                    byteBuffer[i] = (byte)scaled;
                    */
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var eviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            eviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            eviDataset.SetProjection(projection);

            var eviBand = eviDataset.GetRasterBand(1);
            eviBand.SetNoDataValue(255);

            eviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        private static List<(string B03Path, string B08Path)> MatchGranulesGNDVI(List<string> b03Files, List<string> b08Files)
        {
            var granules = new List<(string B03Path, string B08Path)>();

            foreach (var b03 in b03Files)
            {
                string fileName = Path.GetFileName(b03);
                if (!fileName.EndsWith("_B03.jp2")) continue;

                string basePart = fileName.Replace("_B03.jp2", "");
                string expectedB08 = basePart + "_B08.jp2";

                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b08Match != null)
                {
                    granules.Add((b03, b08Match));
                }
            }

            return granules;
        }
        private async Task<string> CreateColorfulGNDVITiff(string b03Path, string b08Path)
        {
            var datetime = DateTime.Now;
            string outputFileName = Path.GetFileNameWithoutExtension(b03Path).Replace("_B03", "_GNDVI_vis1") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b03Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b03Dataset = Gdal.Open(b03Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b03Path}, {b08Path}");
            }

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
            {
                throw new Exception("B03 and B08 must have the same dimensions!");
            }

            var b03Band = b03Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            // Получаем масштаб и смещение для корректного преобразования DN → реальные значения
            double b03Scale, b03Offset;
            int b03HasScale, b03HasOffset;
            b03Band.GetScale(out b03Scale, out b03HasScale);
            b03Band.GetOffset(out b03Offset, out b03HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b03HasScale == 0 || b03Scale == 0)
            {
                Console.WriteLine("B03 scale not found. Using default 0.0001");
                b03Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                Console.WriteLine("B08 scale not found. Using default 0.0001");
                b08Scale = 0.0001;
            }

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b03Buffer.Length; i++)
            {
                b03Buffer[i] = (float)(b03Offset + b03Buffer[i] * b03Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b03Buffer[i]) || float.IsInfinity(b03Buffer[i])) b03Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var gndviBuffer = new float[width * height];
            int noDataCount = 0;

            const float EPS = 1e-9f;

            for (int i = 0; i < gndviBuffer.Length; i++)
            {
                float green = b03Buffer[i];
                float nir = b08Buffer[i];

                if (green <= 0 || nir <= 0)
                {
                    gndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + green;
                if (Math.Abs(denominator) < EPS)
                {
                    gndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float gndvi = (nir - green) / denominator;

                if (float.IsNaN(gndvi) || float.IsInfinity(gndvi))
                {
                    gndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                // Обрезаем значения по диапазону [-0.1, 1.0]
                gndvi = Math.Max(-0.1f, Math.Min(1.0f, gndvi));
                gndviBuffer[i] = gndvi;
            }

            Console.WriteLine($"GNDVI computed. NoData pixels: {noDataCount}");

            // Создаём байтовый буфер для индексов палитры (0–254)
            var byteBuffer = new byte[width * height];

            // Фиксированные пороги и цвета
            var thresholds = new[] { -0.1f, 0.0f, 0.25f, 0.5f, 0.75f, 1.0f };
            var colors = new[]
            {
                new ColorEntry { c1 = 255, c2 = 255, c3 = 255, c4 = 255 }, // #FFFFFF — Open land
                new ColorEntry { c1 = 152, c2 = 251, c3 = 152, c4 = 255 }, // #98FB98 — Very bad
                new ColorEntry { c1 = 59, c2 = 179, c3 = 113, c4 = 255 },  // #3CB371 — Stress
                new ColorEntry { c1 = 46, c2 = 139, c3 = 87, c4 = 255 },   // #2E8B57 — Good
                new ColorEntry { c1 = 0, c2 = 100, c3 = 0, c4 = 255 }      // #006400 — Very good
            };

            // Каждое значение GNDVI привязываем к цвету по порогам
            for (int i = 0; i < gndviBuffer.Length; i++)
            {
                if (gndviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData
                }
                else
                {
                    // Найдём диапазон
                    int idx = 0;
                    for (int j = 0; j < thresholds.Length - 1; j++)
                    {
                        if (gndviBuffer[i] >= thresholds[j] && gndviBuffer[i] < thresholds[j + 1])
                        {
                            idx = j;
                            break;
                        }
                    }

                    // Устанавливаем индекс палитры (0–4)
                    byteBuffer[i] = (byte)idx;
                }
            }

            // Создание TIFF
            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var gndviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);
            gndviDataset.SetGeoTransform(geoTransform);

            string projection = b03Dataset.GetProjection();
            gndviDataset.SetProjection(projection);

            var gndviBand = gndviDataset.GetRasterBand(1);
            gndviBand.SetNoDataValue(255);

            // Устанавливаем цветовую таблицу
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Добавляем цвета для каждого уровня
            for (int i = 0; i < colors.Length; i++)
            {
                colorTable.SetColorEntry(i, colors[i]);
            }

            // NoData — чёрный (прозрачный)
            var noDataEntry = new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 };
            colorTable.SetColorEntry(255, noDataEntry);

            gndviBand.SetRasterColorTable(colorTable);
            gndviBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            gndviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        private async Task<string> CreateGrayGNDVITiff(string b03Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b03Path).Replace("_B03", "_GNDVI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b03Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b03Dataset = Gdal.Open(b03Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b03Path}, {b08Path}");
            }

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
            {
                throw new Exception("B03 and B08 must have the same dimensions!");
            }

            var b03Band = b03Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            // Получаем масштаб и смещение
            double b03Scale, b03Offset;
            int b03HasScale, b03HasOffset;
            b03Band.GetScale(out b03Scale, out b03HasScale);
            b03Band.GetOffset(out b03Offset, out b03HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b03HasScale == 0 || b03Scale == 0)
            {
                b03Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b03Buffer.Length; i++)
            {
                b03Buffer[i] = (float)(b03Offset + b03Buffer[i] * b03Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b03Buffer[i]) || float.IsInfinity(b03Buffer[i])) b03Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var gndviBuffer = new float[width * height];
            int noDataCount = 0;

            const float EPS = 1e-9f;

            for (int i = 0; i < gndviBuffer.Length; i++)
            {
                float green = b03Buffer[i];
                float nir = b08Buffer[i];

                if (green <= 0 || nir <= 0)
                {
                    gndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + green;
                if (Math.Abs(denominator) < EPS)
                {
                    gndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float gndvi = (nir - green) / denominator;

                if (float.IsNaN(gndvi) || float.IsInfinity(gndvi))
                {
                    gndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                // Обрезаем по диапазону [-0.1, 1.0]
                gndvi = Math.Max(-0.1f, Math.Min(1.0f, gndvi));
                gndviBuffer[i] = gndvi;
            }

            // Нормализация в [0, 255] для 8-битного серого
            var byteBuffer = new byte[width * height];
            float minVal = -0.1f;
            float maxVal = 1.0f;

            for (int i = 0; i < gndviBuffer.Length; i++)
            {
                if (gndviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 0; // NoData = 0 (чёрный)
                }
                else
                {
                    float normalized = (gndviBuffer[i] - minVal) / (maxVal - minVal);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    byteBuffer[i] = (byte)Math.Round(normalized * 255);
                }
            }

            // Создание TIFF
            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var gndviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);
            gndviDataset.SetGeoTransform(geoTransform);

            string projection = b03Dataset.GetProjection();
            gndviDataset.SetProjection(projection);

            var gndviBand = gndviDataset.GetRasterBand(1);
            gndviBand.SetNoDataValue(0); // 0 = NoData

            // Убираем палитру — это просто серый канал
            gndviBand.SetRasterColorInterpretation(ColorInterp.GCI_GrayIndex);

            gndviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        private static List<(string B03Path, string B11Path)> MatchGranulesMNDWI(List<string> B03Files, List<string> B11Files)
        {
            var granules = new List<(string B03Path, string B11Path)>();

            foreach (var B03 in B03Files)
            {
                string fileName = Path.GetFileName(B03);
                if (!fileName.EndsWith("_B03.jp2")) continue;

                string basePart = fileName.Replace("_B03.jp2", "");
                string expectedB11 = basePart + "_B11.jp2";

                string B11Match = B11Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB11);

                if (B11Match != null)
                {
                    granules.Add((B03, B11Match));
                }
            }

            return granules;
        }
        private async Task<string> CreateColorfulMNDWITiff(string b03Path, string b11Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b03Path).Replace("_B03", "_MNDWI_vis2") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b03Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b03Dataset = Gdal.Open(b03Path, Access.GA_ReadOnly);
            using var b11Dataset = Gdal.Open(b11Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b11Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b03Path}, {b11Path}");
            }

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            var b03Band = b03Dataset.GetRasterBand(1);
            var b11Band = b11Dataset.GetRasterBand(1);

            // Буферы для double
            var greenDouble = new double[width * height];
            var swirDouble = new double[width * height];

            // Читаем B03 как double
            b03Band.ReadRaster(0, 0, width, height, greenDouble, width, height, 0, 0);

            // Ресемплируем B11 до размеров B03
            var extraArgs = new RasterIOExtraArg();
            extraArgs.nVersion = 1;
            extraArgs.eResampleAlg = (RIOResampleAlg)ResampleAlg.GRA_Bilinear;

            b11Band.ReadRaster(
                0, 0,
                b11Dataset.RasterXSize, b11Dataset.RasterYSize,
                swirDouble,
                width, height,
                0, 0,
                extraArgs);

            // Расчёт MNDWI
            var mndwiBuffer = new double[width * height];
            int noDataCount = 0;

            for (int i = 0; i < greenDouble.Length; i++)
            {
                double green = greenDouble[i];
                double swir = swirDouble[i];

                // Фильтр NoData
                if (green <= 0 || swir <= 0)
                {
                    mndwiBuffer[i] = -9999.0;
                    noDataCount++;
                    continue;
                }

                double denominator = green + swir + EPS;
                if (Math.Abs(denominator) < 1e-9)
                {
                    mndwiBuffer[i] = -9999.0;
                    noDataCount++;
                    continue;
                }

                double mndwi = (green - swir) / denominator;
                mndwiBuffer[i] = mndwi;
            }

            Console.WriteLine($"MNDWI computed. NoData pixels: {noDataCount}");

            // Находим реальный диапазон значений
            double actualMin = double.MaxValue;
            double actualMax = double.MinValue;

            for (int i = 0; i < mndwiBuffer.Length; i++)
            {
                if (mndwiBuffer[i] > -1000)
                {
                    if (mndwiBuffer[i] < actualMin) actualMin = mndwiBuffer[i];
                    if (mndwiBuffer[i] > actualMax) actualMax = mndwiBuffer[i];
                }
            }

            if (actualMin == actualMax)
            {
                actualMax = actualMin + 0.001;
            }

            Console.WriteLine($"MNDWI Range: {actualMin:F4} to {actualMax:F4}");

            // Нормализация к [0, 254]
            var byteBuffer = new byte[width * height];

            for (int i = 0; i < mndwiBuffer.Length; i++)
            {
                if (mndwiBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData
                }
                else
                {
                    double normalized = (mndwiBuffer[i] - actualMin) / (actualMax - actualMin);
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            // Создание GeoTIFF
            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var mndwiDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);
            mndwiDataset.SetGeoTransform(geoTransform);

            string projection = b03Dataset.GetProjection();
            mndwiDataset.SetProjection(projection);

            var mndwiBand = mndwiDataset.GetRasterBand(1);
            mndwiBand.SetNoDataValue(255);

            // 🎨 Цветовая палитра
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            for (int i = 0; i < 255; i++)
            {
                byte r, g, b;

                if (i < 85)
                {
                    // Коричневый → оранжевый
                    r = (byte)(139 + i * 1.4);
                    g = (byte)(69 + i * 0.8);
                    b = (byte)(19 + i * 0.3);
                }
                else if (i < 170)
                {
                    // Оранжевый → белый
                    r = (byte)(255 - (i - 85) * 1.5);
                    g = (byte)(137 + (i - 85) * 1.3);
                    b = (byte)(44 + (i - 85) * 2.4);
                }
                else
                {
                    // Белый → синий (вода)
                    r = (byte)(255 - (i - 170) * 1.5);
                    g = (byte)(255 - (i - 170) * 1.5);
                    b = 255;
                }
                //if (i < 85)
                //{
                //    // Коричневый → оранжевый
                //    r = (byte)(139 + i * 1.4);
                //    g = (byte)(69 + i * 0.8);
                //    b = (byte)(19 + i * 0.3);
                //}
                //else if (i < 170)
                //{
                //    // Оранжевый → белый
                //    r = (byte)(255 - (i - 85) * 1.5);
                //    g = (byte)(137 + (i - 85) * 1.3);
                //    b = (byte)(44 + (i - 85) * 2.4);
                //}
                //else
                //{
                //    // Белый → ярко-синий (вода)
                //    r = 0;
                //    g = (byte)(255 - (i - 170) * 2); // 255 → 0
                //    b = 255;
                //}

                var colorEntry = new ColorEntry();
                colorEntry.c1 = r;
                colorEntry.c2 = g;
                colorEntry.c3 = b;
                colorEntry.c4 = 255;
                colorTable.SetColorEntry(i, colorEntry);
            }

            var noDataEntry = new ColorEntry();
            noDataEntry.c1 = 0;
            noDataEntry.c2 = 0;
            noDataEntry.c3 = 0;
            noDataEntry.c4 = 0;
            colorTable.SetColorEntry(255, noDataEntry);

            mndwiBand.SetRasterColorTable(colorTable);
            mndwiBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            mndwiBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        private async Task<string> CreateGrayscaleMNDWITiff(string b03Path, string b11Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b03Path).Replace("_B03", "_MNDWI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b03Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b03Dataset = Gdal.Open(b03Path, Access.GA_ReadOnly);
            using var b11Dataset = Gdal.Open(b11Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b11Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b03Path}, {b11Path}");
            }

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            var b03Band = b03Dataset.GetRasterBand(1);
            var b11Band = b11Dataset.GetRasterBand(1);

            // Буферы для double
            var greenDouble = new double[width * height];
            var swirDouble = new double[width * height];

            // Читаем B03 как double
            b03Band.ReadRaster(0, 0, width, height, greenDouble, width, height, 0, 0);

            // Ресемплируем B11 до размеров B03
            var extraArgs = new RasterIOExtraArg();
            extraArgs.nVersion = 1;
            extraArgs.eResampleAlg = (RIOResampleAlg)ResampleAlg.GRA_Bilinear;

            b11Band.ReadRaster(
                0, 0,
                b11Dataset.RasterXSize, b11Dataset.RasterYSize,
                swirDouble,
                width, height,
                0, 0,
                extraArgs);

            // Расчёт MNDWI
            var mndwiBuffer = new double[width * height];
            int noDataCount = 0;

            for (int i = 0; i < greenDouble.Length; i++)
            {
                double green = greenDouble[i];
                double swir = swirDouble[i];

                // Фильтр NoData
                if (green <= 0 || swir <= 0)
                {
                    mndwiBuffer[i] = -9999.0;
                    noDataCount++;
                    continue;
                }

                double denominator = green + swir + EPS;
                if (Math.Abs(denominator) < 1e-9)
                {
                    mndwiBuffer[i] = -9999.0;
                    noDataCount++;
                    continue;
                }

                double mndwi = (green - swir) / denominator;
                mndwiBuffer[i] = mndwi;
            }

            // Находим реальный диапазон значений
            double actualMin = double.MaxValue;
            double actualMax = double.MinValue;

            for (int i = 0; i < mndwiBuffer.Length; i++)
            {
                if (mndwiBuffer[i] > -1000)
                {
                    if (mndwiBuffer[i] < actualMin) actualMin = mndwiBuffer[i];
                    if (mndwiBuffer[i] > actualMax) actualMax = mndwiBuffer[i];
                }
            }

            if (actualMin == actualMax)
            {
                actualMax = actualMin + 0.001;
            }

            // Нормализация к [0, 254] — градации серого
            var byteBuffer = new byte[width * height];

            for (int i = 0; i < mndwiBuffer.Length; i++)
            {
                if (mndwiBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData
                }
                else
                {
                    double normalized = (mndwiBuffer[i] - actualMin) / (actualMax - actualMin);
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            // Создание GeoTIFF
            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var mndwiDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);
            mndwiDataset.SetGeoTransform(geoTransform);

            string projection = b03Dataset.GetProjection();
            mndwiDataset.SetProjection(projection);

            var mndwiBand = mndwiDataset.GetRasterBand(1);
            mndwiBand.SetNoDataValue(255);

            mndwiBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        static List<(string B08Path, string B11Path)> MatchGranulesNDMI(List<string> b08Files, List<string> b11Files)
        {
            var granules = new List<(string B08Path, string B11Path)>();

            foreach (var b08 in b08Files)
            {
                string fileName = Path.GetFileName(b08);
                if (!fileName.EndsWith("_B08.jp2")) continue;

                string basePart = fileName.Replace("_B08.jp2", "");
                string expectedB11 = basePart + "_B11.jp2";

                string b11Match = b11Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB11);

                if (b11Match != null)
                {
                    granules.Add((b08, b11Match));
                }
            }

            return granules;
        }
        private async Task<string> CreateColorfulNDMITiff(string b08Path, string b11Path)
        {
            var datetime = DateTime.Now;
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_NDMI_vis7") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly);
            using var b11Dataset = Gdal.Open(b11Path, Access.GA_ReadOnly);

            if (b08Dataset == null || b11Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b08Path}, {b11Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            var b08Band = b08Dataset.GetRasterBand(1);
            var b11Band = b11Dataset.GetRasterBand(1);

            var nirDouble = new double[width * height];
            var swirDouble = new double[width * height];

            b08Band.ReadRaster(0, 0, width, height, nirDouble, width, height, 0, 0);

            var extraArgs = new RasterIOExtraArg();
            extraArgs.nVersion = 1;
            extraArgs.eResampleAlg = (RIOResampleAlg)ResampleAlg.GRA_Bilinear;

            b11Band.ReadRaster(
                0, 0,
                b11Dataset.RasterXSize, b11Dataset.RasterYSize,
                swirDouble,
                width, height,
                0, 0,
                extraArgs);

            var ndmiBuffer = new float[width * height];
            int noDataCount = 0;

            for (int i = 0; i < nirDouble.Length; i++)
            {
                double nir = nirDouble[i];
                double swir = swirDouble[i];

                if (nir <= 0 || swir <= 0)
                {
                    ndmiBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                double denominator = nir + swir;
                if (Math.Abs(denominator) < 1e-9)
                {
                    ndmiBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float ndmi = (float)((nir - swir) / denominator);
                ndmiBuffer[i] = ndmi;
            }

            // Найти действительные мин/макс
            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;

            for (int i = 0; i < ndmiBuffer.Length; i++)
            {
                if (ndmiBuffer[i] > -1000)
                {
                    if (ndmiBuffer[i] < actualMin) actualMin = ndmiBuffer[i];
                    if (ndmiBuffer[i] > actualMax) actualMax = ndmiBuffer[i];
                }
            }

            // Если все значения одинаковые — добавляем небольшой разброс
            if (actualMin == actualMax)
            {
                actualMax = actualMin + 0.001f;
            }

            // Преобразуем в byte [0..254] (NoData = 255)
            var byteBuffer = new byte[width * height];

            for (int i = 0; i < ndmiBuffer.Length; i++)
            {
                if (ndmiBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → чёрный
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (ndmiBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized)); // Clamp
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            // Создаем выходной файл
            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var ndmiDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            ndmiDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            ndmiDataset.SetProjection(projection);

            var ndmiBand = ndmiDataset.GetRasterBand(1);
            ndmiBand.SetNoDataValue(255);

            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Хелперы для интерполяции
            static byte Lerp(byte a, byte b, double t) =>
                (byte)Math.Clamp(Math.Round(a + (b - a) * t), 0, 255);

            static (byte r, byte g, byte b) Lerp3((byte r, byte g, byte b) c1, (byte r, byte g, byte b) c2, double t) =>
                (Lerp(c1.r, c2.r, t), Lerp(c1.g, c2.g, t), Lerp(c1.b, c2.b, t));

            // Узловые цвета (по HEX)
            var lightBlue = (r: (byte)173, g: (byte)216, b: (byte)230);  // #ADD8E6
            var skyBlue = (r: (byte)0, g: (byte)187, b: (byte)255);     // #00BFFF
            var blue = (r: (byte)0, g: (byte)0, b: (byte)255);          // #0000FF
            var darkBlue = (r: (byte)0, g: (byte)0, b: (byte)139);      // #00008B

            // Диапазоны NDMI
            double minNDMI = -0.1;
            double maxNDMI = 1.0;

            // Задаем пороги
            double t0 = 0.0;   // (-0.1 → 0)
            double t1 = 0.3;   // (0 → 0.3)
            double t2 = 0.7;   // (0.3 → 0.7)
            double t3 = 1.0;   // (0.7 → 1)

            colorTable.SetColorEntry(255, new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 });

            for (int idx = 0; idx <= 254; idx++)
            {
                double normalizedNDMI = minNDMI + (idx / 254.0) * (maxNDMI - minNDMI);
                ColorEntry ce;

                if (normalizedNDMI < t0)
                {
                    // Интерполируем от #ADD8E6 к #00BFFF
                    double t = (normalizedNDMI - minNDMI) / (t0 - minNDMI);
                    var c = Lerp3(lightBlue, skyBlue, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (normalizedNDMI < t1)
                {
                    // #00BFFF → #0000FF
                    double t = (normalizedNDMI - t0) / (t1 - t0);
                    var c = Lerp3(skyBlue, blue, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (normalizedNDMI < t2)
                {
                    // #0000FF → #00008B
                    double t = (normalizedNDMI - t1) / (t2 - t1);
                    var c = Lerp3(blue, darkBlue, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else
                {
                    // #00008B → #00008B (константа)
                    ce = new ColorEntry { c1 = darkBlue.r, c2 = darkBlue.g, c3 = darkBlue.b, c4 = 255 };
                }

                colorTable.SetColorEntry(idx, ce);
            }

            ndmiBand.SetRasterColorTable(colorTable);
            ndmiBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            ndmiBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        private async Task<string> CreateGrayNDMITiff(string b08Path, string b11Path)
        {
            var datetime = DateTime.Now;
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_NDMI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly);
            using var b11Dataset = Gdal.Open(b11Path, Access.GA_ReadOnly);

            if (b08Dataset == null || b11Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b08Path}, {b11Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            var b08Band = b08Dataset.GetRasterBand(1);
            var b11Band = b11Dataset.GetRasterBand(1);

            var nirDouble = new double[width * height];
            var swirDouble = new double[width * height];

            b08Band.ReadRaster(0, 0, width, height, nirDouble, width, height, 0, 0);

            var extraArgs = new RasterIOExtraArg();
            extraArgs.nVersion = 1;
            extraArgs.eResampleAlg = (RIOResampleAlg)ResampleAlg.GRA_Bilinear;

            b11Band.ReadRaster(
                0, 0,
                b11Dataset.RasterXSize, b11Dataset.RasterYSize,
                swirDouble,
                width, height,
                0, 0,
                extraArgs);

            var ndmiBuffer = new float[width * height];
            int noDataCount = 0;

            for (int i = 0; i < nirDouble.Length; i++)
            {
                double nir = nirDouble[i];
                double swir = swirDouble[i];

                if (nir <= 0 || swir <= 0)
                {
                    ndmiBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                double denominator = nir + swir;
                if (Math.Abs(denominator) < 1e-9)
                {
                    ndmiBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float ndmi = (float)((nir - swir) / denominator);
                ndmiBuffer[i] = ndmi;
            }

            // Найти реальные min/max
            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;

            for (int i = 0; i < ndmiBuffer.Length; i++)
            {
                if (ndmiBuffer[i] > -1000)
                {
                    if (ndmiBuffer[i] < actualMin) actualMin = ndmiBuffer[i];
                    if (ndmiBuffer[i] > actualMax) actualMax = ndmiBuffer[i];
                }
            }

            if (actualMin == actualMax)
            {
                actualMax = actualMin + 0.001f;
            }

            // Преобразуем в byte [0..255], NoData → 0
            var byteBuffer = new byte[width * height];

            for (int i = 0; i < ndmiBuffer.Length; i++)
            {
                if (ndmiBuffer[i] < -1000)
                {
                    byteBuffer[i] = 0; // NoData → черный (0)
                }
                else
                {
                    // Нормализация: [actualMin, actualMax] → [0, 255]
                    float normalized = (ndmiBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized)); // Clamp
                    int scaled = (int)Math.Round(normalized * 255);
                    byteBuffer[i] = (byte)scaled;
                }
            }

            // Создаём GeoTIFF
            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var ndmiDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            ndmiDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            ndmiDataset.SetProjection(projection);

            var ndmiBand = ndmiDataset.GetRasterBand(1);
            ndmiBand.SetNoDataValue(0); // NoData = 0 (черный)

            // Устанавливаем интерпретацию как "gray"
            ndmiBand.SetRasterColorInterpretation(ColorInterp.GCI_GrayIndex);

            ndmiBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        private static List<(string B04Path, string B08Path)> MatchGranulesNDVI(List<string> b04Files, List<string> b08Files)
        {
            var granules = new List<(string B04Path, string B08Path)>();

            foreach (var b04 in b04Files)
            {
                string fileName = Path.GetFileName(b04);
                if (!fileName.EndsWith("_B04.jp2")) continue;

                string basePart = fileName.Replace("_B04.jp2", "");
                string expectedB08 = basePart + "_B08.jp2";

                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b08Match != null)
                {
                    granules.Add((b04, b08Match));
                }
            }

            return granules;
        }
        private async Task<string> CreateColorfulNDVITiff(string b04Path, string b08Path)
        {
            var datetime = DateTime.Now;
            string outputFileName = Path.GetFileNameWithoutExtension(b04Path).Replace("_B04", "_NDVI_vis") + 8 + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b04Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly);

            if (b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b04Path}, {b08Path}");
            }

            int width = b04Dataset.RasterXSize;
            int height = b04Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
            {
                throw new Exception("B04 and B08 must have the same dimensions!");
            }

            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            var ndviBuffer = new float[width * height];
            int noDataCount = 0;

            for (int i = 0; i < b04Buffer.Length; i++)
            {
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                if (red <= 0 || nir <= 0)
                {
                    ndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + red;
                if (Math.Abs(denominator) < 1e-9f)
                {
                    ndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float ndvi = (nir - red) / denominator;
                ndviBuffer[i] = ndvi;
            }

            Console.WriteLine($"NDVI computed. NoData pixels: {noDataCount}");

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;

            for (int i = 0; i < ndviBuffer.Length; i++)
            {
                if (ndviBuffer[i] > -1000)
                {
                    if (ndviBuffer[i] < actualMin) actualMin = ndviBuffer[i];
                    if (ndviBuffer[i] > actualMax) actualMax = ndviBuffer[i];
                }
            }

            // Защита от деления на ноль
            if (actualMin == actualMax)
            {
                actualMax = actualMin + 0.001f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < ndviBuffer.Length; i++)
            {
                if (ndviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData
                }
                else
                {
                    float normalized = (ndviBuffer[i] - actualMin) / (actualMax - actualMin);
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var ndviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b04Dataset.GetGeoTransform(geoTransform);
            ndviDataset.SetGeoTransform(geoTransform);

            string projection = b04Dataset.GetProjection();
            ndviDataset.SetProjection(projection);

            var ndviBand = ndviDataset.GetRasterBand(1);
            ndviBand.SetNoDataValue(255);

            // Создаём цветовую таблицу: красный → жёлтый → зелёный
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Палитра: красный → жёлтый → зелёный
            for (int i = 0; i < 255; i++)
            {
                byte r, g, b;

                if (i < 60)
                {
                    // Красный → оранжевый
                    r = 255;
                    g = (byte)(i * 4); // 0 → 240
                    b = 0;
                }
                else if (i < 120)
                {
                    // Оранжевый → жёлтый
                    r = (byte)(255 - (i - 60) * 2); // 255 → 195
                    g = 255;
                    b = 0;
                }
                else if (i < 180)
                {
                    // Жёлтый → светло-зелёный
                    r = 0;
                    g = 255;
                    b = (byte)((i - 120) * 2); // 0 → 120
                }
                else
                {
                    // Светло-зелёный → тёмно-зелёный
                    r = 0;
                    g = (byte)(255 - (i - 180) * 2); // 255 → 90
                    b = 255;
                }

                var colorEntry = new ColorEntry();
                colorEntry.c1 = r;
                colorEntry.c2 = g;
                colorEntry.c3 = b;
                colorEntry.c4 = 255;

                colorTable.SetColorEntry(i, colorEntry);
            }

            // NoData — чёрный
            var noDataEntry = new ColorEntry();
            noDataEntry.c1 = 0;
            noDataEntry.c2 = 0;
            noDataEntry.c3 = 0;
            noDataEntry.c4 = 0;

            colorTable.SetColorEntry(255, noDataEntry);

            ndviBand.SetRasterColorTable(colorTable);
            ndviBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            ndviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        private async Task<string> CreateNDVITiff(string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b04Path).Replace("_B04", "_NDVI") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b04Path), outputFileName);

            if (File.Exists(outputPath))
            {
                //logger.LogInformation("skip");
                return outputPath; // или логируй, что пропускаем
            }

            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly);

            if (b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b04Path} or {b08Path}");
            }

            int width = b04Dataset.RasterXSize;
            int height = b04Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
            {
                throw new Exception("B04 and B08 must have the same dimensions!");
            }

            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b04Buffer = new double[width * height];
            var b08Buffer = new double[width * height];

            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            var ndviBuffer = new double[width * height];
            int noDataCount = 0;

            for (int i = 0; i < b04Buffer.Length; i++)
            {
                double b04 = b04Buffer[i];
                double b08 = b08Buffer[i];

                if (b04 <= 0 || b08 <= 0 || b04 + b08 == 0)
                {
                    ndviBuffer[i] = -9999f;
                    noDataCount++;
                }
                else
                {
                    ndviBuffer[i] = (b08 - b04) / (b08 + b04 == 0 ? b08 + b04 + EPS : b08 + b04);
                }
            }

            string[] creationOptions =
            {
                "COMPRESS=LZW",
                "TILED=YES",
                "BIGTIFF=IF_SAFER",
                "PREDICTOR=2"
            };

            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var ndviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Float32, creationOptions);

            double[] geoTransform = new double[6];
            b04Dataset.GetGeoTransform(geoTransform);
            ndviDataset.SetGeoTransform(geoTransform);

            string projection = b04Dataset.GetProjection();
            ndviDataset.SetProjection(projection);

            var ndviBand = ndviDataset.GetRasterBand(1);
            ndviBand.SetNoDataValue(-9999.0);
            ndviBand.WriteRaster(0, 0, width, height, ndviBuffer, width, height, 0, 0);

            //ndviDataset.SetMetadataItem("CREATED_BY", "Masofa NDVI Processor");
            //ndviDataset.SetMetadataItem("SOURCE_B04", Path.GetFileName(b04Path));
            //ndviDataset.SetMetadataItem("SOURCE_B08", Path.GetFileName(b08Path));

            return outputPath;
        }
        private static List<(string B03Path, string B08Path)> MatchGranulesORVI(List<string> b03Files, List<string> b08Files)
        {
            var granules = new List<(string B03Path, string B08Path)>();

            foreach (var b03 in b03Files)
            {
                string fileName = Path.GetFileName(b03);
                if (!fileName.EndsWith("_B03.jp2")) continue;

                string basePart = fileName.Replace("_B03.jp2", "");
                string expectedB08 = basePart + "_B08.jp2";

                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b08Match != null)
                {
                    granules.Add((b03, b08Match));
                }
            }

            return granules;
        }
        private async Task<string> CreateColorfulORVITiff(string b03Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_ORVI_vis3") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b03Dataset = Gdal.Open(b03Path, Access.GA_ReadOnly); // GREEN
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b03Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b03Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b03Dataset.RasterXSize || height != b03Dataset.RasterYSize)
            {
                throw new Exception("B03 and B08 must have the same dimensions!");
            }

            var b03Band = b03Dataset.GetRasterBand(1); // GREEN
            var b08Band = b08Dataset.GetRasterBand(1); // NIR

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b03Scale, b03Offset;
            int b03HasScale, b03HasOffset;
            b03Band.GetScale(out b03Scale, out b03HasScale);
            b03Band.GetOffset(out b03Offset, out b03HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b03HasScale == 0 || b03Scale == 0)
            {
                b03Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b03Buffer.Length; i++)
            {
                b03Buffer[i] = (float)(b03Offset + b03Buffer[i] * b03Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b03Buffer[i]) || float.IsInfinity(b03Buffer[i])) b03Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var orviBuffer = new float[width * height];
            int noDataCount = 0;

            const float EPS = 1e-9f;

            for (int i = 0; i < orviBuffer.Length; i++)
            {
                float green = b03Buffer[i];
                float nir = b08Buffer[i];

                if (green <= 0 || nir <= 0)
                {
                    orviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float orvi = nir / (green + EPS);

                if (float.IsNaN(orvi) || float.IsInfinity(orvi))
                {
                    orviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                if (orvi < 0.0f || orvi > 10.0f)
                {
                    orviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                orviBuffer[i] = orvi;
            }

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < orviBuffer.Length; i++)
            {
                if (orviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (orviBuffer[i] < actualMin) actualMin = orviBuffer[i];
                    if (orviBuffer[i] > actualMax) actualMax = orviBuffer[i];
                }
            }

            // Защита от случая, когда все значения одинаковы
            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = 0.5f;
                actualMax = 3.0f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < orviBuffer.Length; i++)
            {
                if (orviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → прозрачный
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (orviBuffer[i] - actualMin) / (actualMax - actualMin);
                    // Clamp к [0, 1] на случай выбросов
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var orviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            orviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            orviDataset.SetProjection(projection);

            var orviBand = orviDataset.GetRasterBand(1);
            orviBand.SetNoDataValue(255);

            // Цветовая палитра для ORVI (NIR/Green): от тёмно-коричневого → жёлтого → зелёного → тёмно-зелёного
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Хелперы
            static byte Lerp(byte a, byte b, double t) =>
                (byte)Math.Clamp(Math.Round(a + (b - a) * t), 0, 255);
            static (byte r, byte g, byte b) Lerp3((byte r, byte g, byte b) c1, (byte r, byte g, byte b) c2, double t) =>
                (Lerp(c1.r, c2.r, t), Lerp(c1.g, c2.g, t), Lerp(c1.b, c2.b, t));

            // Узловые цвета
            var water = (r: (byte)80, g: (byte)100, b: (byte)130);   // вода/тени
            var soilLo = (r: (byte)140, g: (byte)80, b: (byte)40);   // сухая почва
            var soilHi = (r: (byte)200, g: (byte)130, b: (byte)50);  // влажная почва/начало роста
            var yellow = (r: (byte)255, g: (byte)220, b: (byte)40);  // стресс/начало вегетации
            var greenLo = (r: (byte)160, g: (byte)220, b: (byte)100); // слабая растительность
            var greenMd = (r: (byte)80, g: (byte)180, b: (byte)70);  // хорошая растительность
            var greenHi = (r: (byte)20, g: (byte)100, b: (byte)30);  // густая растительность

            // Пороговые значения ORVI (NIR/Green)
            double t0 = 0.8;  // вода/почва
            double t1 = 1.2;  // сухая → влажная почва
            double t2 = 1.6;  // начало вегетации
            double t3 = 2.0;  // нормальная растительность
            double t4 = 2.5;  // густая растительность

            // NoData — прозрачный
            colorTable.SetColorEntry(255, new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 });

            // Заполняем палитру
            for (int idx = 1; idx <= 254; idx++)
            {
                double orvi = actualMin + (idx / 254.0) * (actualMax - actualMin);
                ColorEntry ce;

                if (orvi < t0)
                {
                    double t = Math.Clamp((orvi - (t0 - 0.5)) / 0.5, 0, 1);
                    var c = Lerp3(water, soilLo, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (orvi < t1)
                {
                    double t = (orvi - t0) / (t1 - t0);
                    var c = Lerp3(soilLo, soilHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (orvi < t2)
                {
                    double t = (orvi - t1) / (t2 - t1);
                    var c = Lerp3(soilHi, yellow, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (orvi < t3)
                {
                    double t = (orvi - t2) / (t3 - t2);
                    var c = Lerp3(yellow, greenLo, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (orvi < t4)
                {
                    double t = (orvi - t3) / (t4 - t3);
                    var c = Lerp3(greenLo, greenMd, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else
                {
                    double t = Math.Clamp((orvi - t4) / 1.0, 0, 1);
                    var c = Lerp3(greenMd, greenHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }

                colorTable.SetColorEntry(idx, ce);
            }

            orviBand.SetRasterColorTable(colorTable);
            orviBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            orviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        private async Task<string> CreateGrayscaleORVITiff(string b03Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_ORVI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b03Dataset = Gdal.Open(b03Path, Access.GA_ReadOnly); // GREEN
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b03Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b03Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b03Dataset.RasterXSize || height != b03Dataset.RasterYSize)
            {
                throw new Exception("B03 and B08 must have the same dimensions!");
            }

            var b03Band = b03Dataset.GetRasterBand(1); // GREEN
            var b08Band = b08Dataset.GetRasterBand(1); // NIR

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b03Scale, b03Offset;
            int b03HasScale, b03HasOffset;
            b03Band.GetScale(out b03Scale, out b03HasScale);
            b03Band.GetOffset(out b03Offset, out b03HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b03HasScale == 0 || b03Scale == 0)
            {
                b03Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b03Buffer.Length; i++)
            {
                b03Buffer[i] = (float)(b03Offset + b03Buffer[i] * b03Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b03Buffer[i]) || float.IsInfinity(b03Buffer[i])) b03Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var orviBuffer = new float[width * height];
            int noDataCount = 0;

            const float EPS = 1e-9f;

            for (int i = 0; i < orviBuffer.Length; i++)
            {
                float green = b03Buffer[i];
                float nir = b08Buffer[i];

                if (green <= 0 || nir <= 0)
                {
                    orviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float orvi = nir / (green + EPS);

                if (float.IsNaN(orvi) || float.IsInfinity(orvi))
                {
                    orviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                if (orvi < 0.0f || orvi > 10.0f)
                {
                    orviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                orviBuffer[i] = orvi;
            }

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < orviBuffer.Length; i++)
            {
                if (orviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (orviBuffer[i] < actualMin) actualMin = orviBuffer[i];
                    if (orviBuffer[i] > actualMax) actualMax = orviBuffer[i];
                }
            }

            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = 0.5f;
                actualMax = 3.0f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < orviBuffer.Length; i++)
            {
                if (orviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255;
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (orviBuffer[i] - actualMin) / (actualMax - actualMin);
                    // Clamp к [0, 1] на случай выбросов
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var orviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            orviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            orviDataset.SetProjection(projection);

            var orviBand = orviDataset.GetRasterBand(1);
            orviBand.SetNoDataValue(255);

            orviBand.SetRasterColorInterpretation(ColorInterp.GCI_GrayIndex);

            orviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        private static List<(string B04Path, string B08Path)> MatchGranulesOSAVI(List<string> b04Files, List<string> b08Files)
        {
            var granules = new List<(string B04Path, string B08Path)>();

            foreach (var b04 in b04Files)
            {
                string fileName = Path.GetFileName(b04);
                if (!fileName.EndsWith("_B04.jp2")) continue;

                string basePart = fileName.Replace("_B04.jp2", "");
                string expectedB08 = basePart + "_B08.jp2";

                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b08Match != null)
                {
                    granules.Add((b04, b08Match));
                }
            }

            return granules;
        }
        private async Task<string> CreateColorfulOSAVITiff(string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_OSAVI_vis3") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly); // RED
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b04Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b04Dataset.RasterXSize || height != b04Dataset.RasterYSize)
            {
                throw new Exception("B04 and B08 must have the same dimensions!");
            }

            var b04Band = b04Dataset.GetRasterBand(1); // RED
            var b08Band = b08Dataset.GetRasterBand(1); // NIR

            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b04Scale, b04Offset;
            int b04HasScale, b04HasOffset;
            b04Band.GetScale(out b04Scale, out b04HasScale);
            b04Band.GetOffset(out b04Offset, out b04HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b04HasScale == 0 || b04Scale == 0)
            {
                b04Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }

            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b04Buffer.Length; i++)
            {
                b04Buffer[i] = (float)(b04Offset + b04Buffer[i] * b04Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b04Buffer[i]) || float.IsInfinity(b04Buffer[i])) b04Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var osaviBuffer = new float[width * height];
            int noDataCount = 0;

            const float OSAVI_X = 0.16f; // Почвенная коррекция
            const float EPS = 1e-9f;

            for (int i = 0; i < osaviBuffer.Length; i++)
            {
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                if (red <= 0 || nir <= 0)
                {
                    osaviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float osavi = (nir - red) / (nir + red + OSAVI_X + EPS);

                if (float.IsNaN(osavi) || float.IsInfinity(osavi) || osavi < -1.0f || osavi > 1.0f)
                {
                    osaviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                osaviBuffer[i] = osavi;
            }

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < osaviBuffer.Length; i++)
            {
                if (osaviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (osaviBuffer[i] < actualMin) actualMin = osaviBuffer[i];
                    if (osaviBuffer[i] > actualMax) actualMax = osaviBuffer[i];
                }
            }

            // Защита от случая, когда все значения одинаковы
            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = -0.1f;
                actualMax = 0.8f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < osaviBuffer.Length; i++)
            {
                if (osaviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → прозрачный
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (osaviBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var osaviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            osaviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            osaviDataset.SetProjection(projection);

            var osaviBand = osaviDataset.GetRasterBand(1);
            osaviBand.SetNoDataValue(255);

            // Цветовая палитра для OSAVI: от коричневого (почва) → жёлтого → зелёного → тёмно-зелёного (густая растительность)
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Хелперы
            static byte Lerp(byte a, byte b, double t) =>
                (byte)Math.Clamp(Math.Round(a + (b - a) * t), 0, 255);
            static (byte r, byte g, byte b) Lerp3((byte r, byte g, byte b) c1, (byte r, byte g, byte b) c2, double t) =>
                (Lerp(c1.r, c2.r, t), Lerp(c1.g, c2.g, t), Lerp(c1.b, c2.b, t));

            // Узловые цвета для OSAVI (диапазон ~ -0.1 до 0.8)
            var bareSoil = (r: (byte)139, g: (byte)69, b: (byte)19);   // Коричневый — голая почва
            var dryVegetation = (r: (byte)210, g: (byte)180, b: (byte)140); // Сухая/начальная растительность
            var stressed = (r: (byte)255, g: (byte)220, b: (byte)40);  // Жёлтый — стресс/начало роста
            var moderate = (r: (byte)160, g: (byte)220, b: (byte)100); // Светло-зелёный — умеренная растительность
            var healthy = (r: (byte)80, g: (byte)180, b: (byte)70);    // Зелёный — здоровая растительность
            var dense = (r: (byte)20, g: (byte)100, b: (byte)30);      // Тёмно-зелёный — густая растительность

            // Пороговые значения OSAVI (адаптивные, но типичные диапазоны)
            double t0 = 0.0;   // Почва → начальная растительность
            double t1 = 0.2;   // Начало вегетации
            double t2 = 0.35;  // Стресс/умеренный рост
            double t3 = 0.5;   // Хорошая растительность
            double t4 = 0.65;  // Густая растительность

            // NoData — прозрачный
            colorTable.SetColorEntry(255, new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 });

            // Заполняем палитру
            for (int idx = 1; idx <= 254; idx++)
            {
                double osavi = actualMin + (idx / 254.0) * (actualMax - actualMin);
                ColorEntry ce;

                if (osavi < 0.0)
                {
                    // Очень низкие значения — почва, тени, вода
                    double t = Math.Clamp((osavi - (actualMin)) / (0.0 - actualMin), 0, 1);
                    var c = Lerp3(bareSoil, dryVegetation, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (osavi < t0)
                {
                    double t = osavi / t0;
                    var c = Lerp3(dryVegetation, stressed, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (osavi < t1)
                {
                    double t = (osavi - t0) / (t1 - t0);
                    var c = Lerp3(stressed, moderate, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (osavi < t3)
                {
                    double t = (osavi - t1) / (t3 - t1);
                    var c = Lerp3(moderate, healthy, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (osavi < t4)
                {
                    double t = (osavi - t3) / (t4 - t3);
                    var c = Lerp3(healthy, dense, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else
                {
                    double t = Math.Clamp((osavi - t4) / (actualMax - t4), 0, 1);
                    var c = Lerp3(dense, dense, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }

                colorTable.SetColorEntry(idx, ce);
            }

            osaviBand.SetRasterColorTable(colorTable);
            osaviBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            osaviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        private async Task<string> CreateGrayscaleOSAVITiff(string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_OSAVI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly); // RED
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b04Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b04Dataset.RasterXSize || height != b04Dataset.RasterYSize)
            {
                throw new Exception("B04 and B08 must have the same dimensions!");
            }

            var b04Band = b04Dataset.GetRasterBand(1); // RED
            var b08Band = b08Dataset.GetRasterBand(1); // NIR

            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b04Scale, b04Offset;
            int b04HasScale, b04HasOffset;
            b04Band.GetScale(out b04Scale, out b04HasScale);
            b04Band.GetOffset(out b04Offset, out b04HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b04HasScale == 0 || b04Scale == 0)
            {
                b04Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }

            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b04Buffer.Length; i++)
            {
                b04Buffer[i] = (float)(b04Offset + b04Buffer[i] * b04Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b04Buffer[i]) || float.IsInfinity(b04Buffer[i])) b04Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var osaviBuffer = new float[width * height];
            int noDataCount = 0;

            const float OSAVI_X = 0.16f; // Почвенная коррекция
            const float EPS = 1e-9f;

            for (int i = 0; i < osaviBuffer.Length; i++)
            {
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                if (red <= 0 || nir <= 0)
                {
                    osaviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float osavi = (nir - red) / (nir + red + OSAVI_X + EPS);

                if (float.IsNaN(osavi) || float.IsInfinity(osavi) || osavi < -1.0f || osavi > 1.0f)
                {
                    osaviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                osaviBuffer[i] = osavi;
            }

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < osaviBuffer.Length; i++)
            {
                if (osaviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (osaviBuffer[i] < actualMin) actualMin = osaviBuffer[i];
                    if (osaviBuffer[i] > actualMax) actualMax = osaviBuffer[i];
                }
            }

            // Защита от случая, когда все значения одинаковы
            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = -0.1f;
                actualMax = 0.8f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < osaviBuffer.Length; i++)
            {
                if (osaviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → белый или прозрачный? Решаем ниже
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (osaviBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var osaviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            osaviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            osaviDataset.SetProjection(projection);

            var osaviBand = osaviDataset.GetRasterBand(1);
            osaviBand.SetNoDataValue(255);

            // 🔥 ВАЖНО: Убираем палитру, интерпретируем как градации серого
            osaviBand.SetRasterColorInterpretation(ColorInterp.GCI_GrayIndex);

            // Пишем данные
            osaviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }

        private async Task SendToMionioTiff<TIndexModel>(string arviTiffPath, string tiffFileName, string tiffBucket, Guid Id, string ProductId, DateTime originalDate, bool isColored)
            where TIndexModel : BaseIndexPolygon
        {
            using (var tiffStream = File.OpenRead(arviTiffPath))
            {
                string minioTiffPath = await FileStorageProvider.PushFileAsync(tiffStream, tiffFileName, tiffBucket);
                Console.WriteLine($"Uploaded TIFF to MinIO: {tiffBucket}/{minioTiffPath}");
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
        #endregion
    }
}
