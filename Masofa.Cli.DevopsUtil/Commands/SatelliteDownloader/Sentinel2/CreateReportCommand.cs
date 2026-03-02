using Azure;
using Masofa.Cli.DevopsUtil.Commands.Satellite;
using Masofa.Client.Copernicus;
using Masofa.Common.Models.Satellite;
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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO.Compression;
using System.Text;

namespace Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2
{
    [BaseCommand("Sentinel2 Create Report", "Создание отчёта по полям")]
    public class CreateReportCommand : IBaseCommand
    {
        ILogger<CreateReportCommand> Logger { get; }
        private SentinelServiceOptions SentinelServiceOptions { get; }
        private CopernicusApiUnitOfWork CopernicusApiUnitOfWork { get; }
        private MasofaSentinelDbContext SentinelDbContext { get; }
        private Dictionary<string, Task> GenerateTasks = new Dictionary<string, Task>();
        private static bool GdalInitialized = false;
        private const double EPS = 1e-8;
        private WKTReader wktReader = new WKTReader();
        private List<Masofa.Common.Models.CropMonitoring.Field> fields = new List<Masofa.Common.Models.CropMonitoring.Field>();
        private DateTime startDay = new DateTime(2025, 7, 2);
        private DateTime endDay = DateTime.Now;



        public CreateReportCommand(
            ILogger<CreateReportCommand> logger,
            CopernicusApiUnitOfWork copernicusApiUnitOfWork,
            MasofaCommonDbContext commonDbContext,
            MasofaSentinelDbContext sentinelDbContext,
            IOptions<SentinelServiceOptions> options)
        {
            SentinelServiceOptions = options.Value;
            SentinelServiceOptions.SatelliteSearchConfig = new SatelliteSearchConfig();
            Logger = logger;
            CopernicusApiUnitOfWork = copernicusApiUnitOfWork;
            SentinelDbContext = sentinelDbContext;
            InitializeGdal();

        }

        public async Task Execute()
        {
            Logger.LogInformation($"Start Sentinel2SearchProductJob");


            if (!CopernicusApiUnitOfWork.IsAuthed)
            {
                await CopernicusApiUnitOfWork.LoginAsync(SentinelServiceOptions);
            }

            var reader = new NetTopologySuite.IO.WKTReader();
            var geometry = reader.Read($"POLYGON((33.0 35.0 , 33.0 36.7 , 34.7 36.7 , 34.7 35.0 , 33.0 35.0 ))");
            if (geometry is Polygon polygon)
            {
                this.SentinelServiceOptions.SatelliteSearchConfig.SentinelPolygon = polygon;
            }

            InitFields();

            var products = await GetProducts();
            var tryCount = 0;
            while (products.Count(m => m.IsComplite == false) > 0)
            {
                Console.WriteLine($"Tring {tryCount}");
                var loadedProducts = await LoadArchives(products);

                products = products.Select(m =>
                {
                    var result = m;
                    var temp = loadedProducts.FirstOrDefault(l => l.ProductId.Equals(m.ProductId));
                    result.ProductFilePath = (temp == null) ? m.ProductFilePath : temp.ProductFilePath;
                    return result;
                }).ToList();

                var extractProducts = await ExtractArchives(products);

                products = products.Select(m =>
                {
                    var result = m;
                    var temp = extractProducts.FirstOrDefault(l => l.ProductId.Equals(m.ProductId));
                    result.ProductUnZipFilePath = (temp == null) ? m.ProductUnZipFilePath : temp.ProductUnZipFilePath;
                    return result;
                }).ToList();

                var arviProducts = await CreateARVIs(products);

                products = products.Select(m =>
                {
                    var result = m;
                    var temp = arviProducts.FirstOrDefault(l => l.ProductId.Equals(m.ProductId));
                    result.ARVIFilePath = (temp == null) ? m.ARVIFilePath : temp.ARVIFilePath;
                    return result;
                }).ToList();

                var eviProducts = await CreateEVIs(products);

                products = products.Select(m =>
                {
                    var result = m;
                    var temp = eviProducts.FirstOrDefault(l => l.ProductId.Equals(m.ProductId));
                    result.EVIFilePath = (temp == null) ? m.EVIFilePath : temp.EVIFilePath;
                    return result;
                }).ToList();

                var gndviProducts = await CreateGNDVIs(products);

                products = products.Select(m =>
                {
                    var result = m;
                    var temp = gndviProducts.FirstOrDefault(l => l.ProductId.Equals(m.ProductId));
                    result.GNDVIFilePath = (temp == null) ? m.GNDVIFilePath : temp.GNDVIFilePath;
                    return result;
                }).ToList();

                var mndwiProducts = await CreateMNDWIs(products);

                products = products.Select(m =>
                {
                    var result = m;
                    var temp = mndwiProducts.FirstOrDefault(l => l.ProductId.Equals(m.ProductId));
                    result.MNDWIFilePath = (temp == null) ? m.MNDWIFilePath : temp.MNDWIFilePath;
                    return result;
                }).ToList();

                var ndmiProducts = await CreateNDMIs(products);

                products = products.Select(m =>
                {
                    var result = m;
                    var temp = ndmiProducts.FirstOrDefault(l => l.ProductId.Equals(m.ProductId));
                    result.NDMIFilePath = (temp == null) ? m.NDMIFilePath : temp.NDMIFilePath;
                    return result;
                }).ToList();

                var ndviProducts = await CreateNDVIs(products);

                products = products.Select(m =>
                {
                    var result = m;
                    var temp = ndviProducts.FirstOrDefault(l => l.ProductId.Equals(m.ProductId));
                    result.NDVIFilePath = (temp == null) ? m.NDVIFilePath : temp.NDVIFilePath;
                    return result;
                }).ToList();

                var orviProducts = await CreateORVIs(products);

                products = products.Select(m =>
                {
                    var result = m;
                    var temp = orviProducts.FirstOrDefault(l => l.ProductId.Equals(m.ProductId));
                    result.ORVIFilePath = (temp == null) ? m.ORVIFilePath : temp.ORVIFilePath;
                    return result;
                }).ToList();

                var osaviProducts = await CreateOSAVIs(products);

                products = products.Select(m =>
                {
                    var result = m;
                    var temp = osaviProducts.FirstOrDefault(l => l.ProductId.Equals(m.ProductId));
                    result.OSAVIFilePath = (temp == null) ? m.OSAVIFilePath : temp.OSAVIFilePath;
                    return result;
                }).ToList();

                SaveReportToMarkdown(products);
                tryCount++;
            }

            Logger.LogInformation($"End Sentinel2SearchProductJob");
        }

        public void SaveReportToMarkdown(List<CreateReportCommandResultItem> items)
        {
            var outputDirectory = "/root/Debug/liban/satellite/sentinel2/reports";
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var groupedItems = items.GroupBy(m => m.OrinalDate);

            foreach (var group in groupedItems)
            {
                // Определяем имя файла с текущей датой
                string fileName = $"Report_{group.Key.ToString("yyyy_MM_dd")}.md";
                string fullPath = Path.Combine(outputDirectory, fileName);
                EnsureDirectoriesExistRecursive(fullPath);
                var sb = new StringBuilder();

                // Заголовок таблицы
                sb.AppendLine("| ProductId | Original Date | Product File Path | Unzipped Path | ARVI | EVI | GNDVI | MNDWI | NDMI | NDVI | ORVI | OSAVI |");
                sb.AppendLine("|-----------|---------------|-------------------|----------------|------|-----|--------|--------|------|------|------|-------|");

                // Строки таблицы
                foreach (var item in group)
                {
                    string productId = item.ProductId.ToString();
                    string originalDate = item.OrinalDate.ToString("yyyy-MM-dd");
                    string productFilePath = EscapeMarkdown(item.ProductFilePath);
                    string unzippedPath = EscapeMarkdown(item.ProductUnZipFilePath);
                    string arvi = EscapeMarkdown(item.ARVIFilePath);
                    string evi = EscapeMarkdown(item.EVIFilePath);
                    string gndvi = EscapeMarkdown(item.GNDVIFilePath);
                    string mndwi = EscapeMarkdown(item.MNDWIFilePath);
                    string ndmi = EscapeMarkdown(item.NDMIFilePath);
                    string ndvi = EscapeMarkdown(item.NDVIFilePath);
                    string orvi = EscapeMarkdown(item.ORVIFilePath);
                    string osavi = EscapeMarkdown(item.OSAVIFilePath);

                    sb.AppendLine($"| {productId} | {originalDate} | {productFilePath} | {unzippedPath} | {arvi} | {evi} | {gndvi} | {mndwi} | {ndmi} | {ndvi} | {orvi} | {osavi} |");
                }
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                File.WriteAllText(fullPath, sb.ToString(), Encoding.UTF8);
            }

            var jsonReport = Newtonsoft.Json.JsonConvert.SerializeObject(items);
            string fullPathReport = Path.Combine(outputDirectory, "report.json");
            if (File.Exists(fullPathReport))
            {
                File.Delete(fullPathReport);
            }
            File.WriteAllText(fullPathReport, jsonReport, Encoding.UTF8);

        }

        private async Task<List<CreateReportCommandResultItem>> GetProducts()
        {
            var products = new List<CreateReportCommandResultItem>();
            var outputDirectory = "/root/Debug/liban/satellite/sentinel2/reports";
            string fullPathReport = Path.Combine(outputDirectory, "report.json");
            if (File.Exists(fullPathReport))
            {
                var tempProducts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CreateReportCommandResultItem>>(fullPathReport);
                if ((tempProducts != null) && (tempProducts.Any()))
                {
                    products.AddRange(tempProducts);
                }

            }

            for (var cDay = startDay; cDay <= endDay; cDay = cDay.AddDays(1))
            {
                Console.WriteLine($"Start {cDay.AddDays(-1).ToString("yyyy-MM-dd")}");
                var tempProductIds = await CopernicusApiUnitOfWork.ProductRepository.SearchProductAsync(SentinelServiceOptions, cDay.AddDays(-1), cDay);
                var tempProducts = new List<CreateReportCommandResultItem>();
                foreach (var item in tempProductIds)
                {
                    if (products.Any(p => p.ProductId == item))
                    {
                        continue;
                    }
                    tempProducts.Add(new CreateReportCommandResultItem()
                    {
                        ProductId = item,
                        OrinalDate = cDay.AddDays(-1),
                    });
                }

                products.AddRange(tempProducts);
                Task.Delay(2000).Wait();
                //if (products.Any())
                //{
                //    break;
                //}
            }

            if (products == null || !products.Any())
            {
                Logger.LogInformation("No suitable products found for AOI.");
            }

            foreach (var productId in products)
            {
                Logger.LogInformation($"{productId.ProductId}");
            }
            return products;
        }

        private async Task<List<CreateReportCommandResultItem>> LoadArchives(List<CreateReportCommandResultItem> items)
        {
            var basePath = "/root/Debug/liban/satellite/sentinel2/source";
            var result = new List<CreateReportCommandResultItem>();
            var index = 0;

            foreach (var item in items)
            {
                Console.WriteLine($"Start load {index} of {items.Count()}");
                var product = item;
                if (!string.IsNullOrEmpty(product.ProductFilePath))
                {
                    Console.WriteLine($"Product {item.ProductId} already exist in {product.ProductFilePath}");
                    continue;
                }
                var currentPath = Path.Combine(basePath, product.OrinalDate.ToString("yyyy-MM-dd"));
                try
                {
                    Console.WriteLine($"ReAuth");
                    await CopernicusApiUnitOfWork.LoginAsync(SentinelServiceOptions);
                    var responce = await CopernicusApiUnitOfWork.ProductRepository.GetProductMediadataAsMessageAsync(SentinelServiceOptions, product.ProductId);
                    using (var productStream = responce.Content.ReadAsStream())
                    {
                        Logger.LogInformation($"Downloaded product stream for {product.ProductId}");
                        var filePath = Path.Combine(currentPath, product.ProductId.ToString());
                        EnsureDirectoriesExistRecursive(filePath);

                        using (var mStream = new MemoryStream())
                        {
                            await productStream.CopyToAsync(mStream);
                            mStream.Position = 0;
                            File.WriteAllBytes(filePath, mStream.ToArray());
                        }
                        product.ProductFilePath = filePath;
                        Logger.LogInformation($"product for {product.ProductId} saved");
                    }
                    result.Add(product);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine($"Error: {ex.InnerException?.Message}");
                    Logger.LogError(ex.Message);
                }
                await Task.Delay(Random.Shared.Next(1000, 4000));
                index++;
            }
            return result;
        }

        private async Task<List<CreateReportCommandResultItem>> ExtractArchives(List<CreateReportCommandResultItem> items)
        {
            var basePath = "/root/Debug/liban/satellite/sentinel2/extract";
            var result = new List<CreateReportCommandResultItem>();
            foreach (var item in items)
            {
                var product = item;
                var tempExtractRoot = Path.Combine(basePath, product.OrinalDate.ToString("yyyy-MM-dd"), product.ProductId.ToString());
                ZipFile.ExtractToDirectory(product.ProductFilePath, tempExtractRoot);
                product.ProductUnZipFilePath = tempExtractRoot;
                result.Add(product);
                Logger.LogInformation($"Extracted ZIP to: {tempExtractRoot}");
            }
            return result;
        }

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
                Logger.LogInformation($"{taskName} complited");

            }
            return result;
        }

        private async Task<string> GenerateTiffARVI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Logger.LogWarning("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }
            var b02Files = Directory.GetFiles(imgDataPath, "*B02.jp2", SearchOption.AllDirectories).ToList();
            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.AllDirectories).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.AllDirectories).ToList();

            if (!b04Files.Any() || !b08Files.Any() || !b02Files.Any())
            {
                Logger.LogWarning("B02.jp2, B04.jp2 or B08.jp2 files not found.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {b02Files.Count} B02, {b04Files.Count} B04 files, {b08Files.Count} B08 files.");

            var granules = MatchGranulesARVI(b02Files, b04Files, b08Files);

            if (granules.Count == 0)
            {
                Logger.LogWarning("Failed to match any B04/B08 pairs.");
                return string.Empty;
            }
            var result = string.Empty;
            try
            {
                string localArviColoredPath = await CreateColorfulARVITiff(granules[0].B02Path, granules[0].B04Path, granules[0].B08Path);
                Logger.LogInformation($"Local ARVI GeoTIFF saved: {localArviColoredPath}");
                result = localArviColoredPath;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error processing {Path.GetFileName(granules[0].B04Path)}: {ex.Message}", ex);
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
                Logger.LogInformation($"{taskName} complited");
            }
            return result;
        }

        private async Task<string> GenerateTiffEVI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Logger.LogWarning("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var b02Files = Directory.GetFiles(imgDataPath, "*B02.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b04Files.Any() || !b08Files.Any() || !b02Files.Any())
            {
                Logger.LogWarning("B02.jp2, B04.jp2 or B08.jp2 files not found.");
                return string.Empty;
            }

            var b02Path = b02Files[0];
            var b04Path = b04Files[0];
            var b08Path = b08Files[0];

            Logger.LogInformation($"Found {b02Files.Count} B02, {b04Files.Count} B04 files, {b08Files.Count} B08 files.");

            var granules = MatchGranulesEVI(b02Files, b04Files, b08Files);

            if (granules.Count == 0)
            {
                Logger.LogWarning("Failed to match any B04/B08 pairs.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {granules.Count} granules to process.");
            var result = string.Empty;
            string localEviColoredPath = await CreateColorfulEVITiff(granules[0].B02Path, granules[0].B04Path, granules[0].B08Path);
            Logger.LogInformation($"Local EVI GeoTIFF saved: {localEviColoredPath}");
            result = localEviColoredPath;

            Logger.LogInformation("EVIJob completed successfully.");
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
                Logger.LogInformation($"{taskName} complited");
            }
            return result;
        }

        private async Task<string> GenerateTiffGNDVI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Logger.LogWarning("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b08Files.Any())
            {
                Logger.LogWarning("B03.jp2 or B08.jp2 files not found.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {b03Files.Count} B03, {b08Files.Count} B08 files.");

            var granules = MatchGranulesGNDVI(b03Files, b08Files);

            if (granules.Count == 0)
            {
                Logger.LogWarning("Failed to match any B03/B08 pairs.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {granules.Count} granules to process.");
            var result = string.Empty;
            try
            {
                string gndviColoredTiffPath = await CreateColorfulGNDVITiff(granules[0].B03Path, granules[0].B08Path);
                result = gndviColoredTiffPath;
            }
            catch (Exception ex) 
            {
                Logger.LogError(ex.Message);
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
                Logger.LogInformation($"{taskName} complited");

            }
            return result;
        }

        private async Task<string> GenerateTiffMNDWI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Logger.LogWarning("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var B03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var B11Files = Directory.GetFiles(imgDataPath, "*B11.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!B03Files.Any() || !B11Files.Any())
            {
                Logger.LogWarning("B03.jp2 or B11.jp2 files not found.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {B03Files.Count} B03 files and {B11Files.Count} B11 files.");

            var granules = MatchGranulesMNDWI(B03Files, B11Files);

            if (granules.Count == 0)
            {
                Logger.LogWarning("Failed to match any B03/B11 pairs.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {granules.Count} granules to process.");
            var result = string.Empty;
            try
            {
                string mndwiColoredTiffPath = await CreateColorfulMNDWITiff(granules[0].B03Path, granules[0].B11Path);
                result = mndwiColoredTiffPath;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error processing {Path.GetFileName(granules[0].B03Path)}: {ex.Message}", ex);
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
                Logger.LogInformation($"{taskName} complited");

            }
            return result;
        }

        private async Task<string> GenerateTiffNDMI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Logger.LogWarning("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var B08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();
            var B11Files = Directory.GetFiles(imgDataPath, "*B11.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!B08Files.Any() || !B11Files.Any())
            {
                Logger.LogWarning("B08.jp2 or B11.jp2 files not found.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {B08Files.Count} B08 files and {B11Files.Count} B11 files.");

            var granules = MatchGranulesNDMI(B08Files, B11Files);

            if (granules.Count == 0)
            {
                Logger.LogWarning("Failed to match any B08/B11 pairs.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {granules.Count} granules to process.");

            var result = string.Empty;
            try
            {
                string ndmiColoredTiffPath = await CreateColorfulNDMITiff(granules[0].B08Path, granules[0].B11Path);
                result = ndmiColoredTiffPath;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error processing {Path.GetFileName(granules[0].B08Path)}: {ex.Message}", ex);
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
                Logger.LogInformation($"{taskName} complited");

            }
            return result;
        }

        private async Task<string> GenerateTiffNDVI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Logger.LogWarning("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b04Files.Any() || !b08Files.Any())
            {
                Logger.LogWarning("B04.jp2 or B08.jp2 files not found.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {b04Files.Count} B04 files and {b08Files.Count} B08 files.");

            var granules = MatchGranulesNDVI(b04Files, b08Files);

            if (granules.Count == 0)
            {
                Logger.LogWarning("Failed to match any B04/B08 pairs.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {granules.Count} granules to process.");
            var result = string.Empty;
            try
            {
                string ndviColoredTiffPath = await CreateColorfulNDVITiff(granules[0].B04Path, granules[0].B08Path);
                result = ndviColoredTiffPath;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error processing {Path.GetFileName(granules[0].B04Path)}: {ex.Message}", ex);
            }

            Logger.LogInformation("NDVIJob completed successfully.");
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
                Logger.LogInformation($"{taskName} complited");

            }
            return result;
        }

        private async Task<string> GenerateTiffORVI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Logger.LogWarning("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b08Files.Any())
            {
                Logger.LogWarning("B03.jp2 or B08.jp2 files not found.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {b03Files.Count} B03 files and {b08Files.Count} B08 files.");

            var granules = MatchGranulesORVI(b03Files, b08Files);

            if (granules.Count == 0)
            {
                Logger.LogWarning("Failed to match any B03/B08 pairs.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {granules.Count} granules to process.");
            var result = string.Empty;
            try
            {
                string orviColoredTiffPath = await CreateColorfulORVITiff(granules[0].B03Path, granules[0].B08Path);
                result = orviColoredTiffPath;

            }
            catch (Exception ex)
            {
                Logger.LogError($"Error processing granule: {ex.Message}", ex);
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
                Logger.LogInformation($"{taskName} complited");

            }
            return result;
        }

        private async Task<string> GenerateTiffOSAVI(string directoryPath, string taskName)
        {
            var imgDataPath = FindImgDataPath(directoryPath);
            if (string.IsNullOrEmpty(imgDataPath))
            {
                Logger.LogWarning("IMG_DATA path could not be determined. Skipping product.");
                return string.Empty;
            }

            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b04Files.Any() || !b08Files.Any())
            {
                Logger.LogWarning("B04.jp2 or B08.jp2 files not found.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {b04Files.Count} B04 files and {b08Files.Count} B08 files.");

            var granules = MatchGranulesOSAVI(b04Files, b08Files);

            if (granules.Count == 0)
            {
                Logger.LogWarning("Failed to match any B04/B08 pairs.");
                return string.Empty;
            }

            Logger.LogInformation($"Found {granules.Count} granules to process.");

            var result = string.Empty;
            try
            {
                string osaviColoredTiffPath = await CreateColorfulOSAVITiff(granules[0].B04Path, granules[0].B08Path);
                result = osaviColoredTiffPath;

            }
            catch (Exception ex)
            {
                Logger.LogError($"Error processing granule: {ex.Message}", ex);
            }
            return result;
        }

        #endregion

        private async Task<List<CreateReportCommandResultItem>> DrawFields(List<CreateReportCommandResultItem> items)
        {
            var result = new List<CreateReportCommandResultItem>();
            foreach (var item in items)
            {
                var product = item;
                var currentFiels = GetFieldsInTiffExtent(product.ARVIFilePath, fields);
                if (!fields.Any())
                {
                    continue;
                }
                product.Fields = fields;
                product.WithFieldARVIFilePath = OverlayPolygonsOnTiff(product.ARVIFilePath, currentFiels); 
                product.WithFieldEVIFilePath = OverlayPolygonsOnTiff(product.EVIFilePath, currentFiels); 
                product.WithFieldGNDVIFilePath = OverlayPolygonsOnTiff(product.GNDVIFilePath, currentFiels); 
                product.WithFieldMNDWIFilePath = OverlayPolygonsOnTiff(product.MNDWIFilePath, currentFiels); 
                product.WithFieldNDMIFilePath = OverlayPolygonsOnTiff(product.NDMIFilePath, currentFiels); 
                product.WithFieldNDVIFilePath = OverlayPolygonsOnTiff(product.NDVIFilePath, currentFiels); 
                product.WithFieldORVIFilePath = OverlayPolygonsOnTiff(product.ORVIFilePath, currentFiels); 
                product.WithFieldOSAVIFilePath = OverlayPolygonsOnTiff(product.OSAVIFilePath, currentFiels);

                result.Add(product);
            }
            return result;
        }

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
                Logger.LogInformation($"PROJ/GDAL initialized. GEOGCS: {geogcs}");
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to initialize GDAL/PROJ", ex);
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

            Logger.LogInformation($"{directoryPath}");

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

        private string FindImgDataPath(string extractRoot)
        {
            var granuleDir = Directory.GetDirectories(extractRoot, "GRANULE", SearchOption.AllDirectories).FirstOrDefault();
            if (granuleDir == null)
            {
                Logger.LogWarning("GRANULE directory not found.");
                return null;
            }

            var l1cDirs = Directory.GetDirectories(granuleDir, "L1C_T*", SearchOption.AllDirectories);
            if (l1cDirs.Length == 0)
            {
                Logger.LogWarning("No L1C_T* directory found inside GRANULE.");
                return null;
            }

            var l1cDir = l1cDirs[0];

            var imgDataDir = Path.Combine(l1cDir, "IMG_DATA");
            if (!Directory.Exists(imgDataDir))
            {
                Logger.LogWarning("IMG_DATA directory not found inside L1C_T* folder.");
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

        private string EscapeMarkdown(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input.Replace("|", "\\|")
                        .Replace("\n", " ")
                        .Replace("\r", " ");
        }

        private Masofa.Common.Models.CropMonitoring.Field AddField(
            string id,
            string name,
            double fieldArea,
            string regionId,
            string externalId,
            string comment,
            bool waterSaving,
            double soilIndex,
            string polygonWkt,
            DateTime createAt,
            DateTime lastUpdateAt)
        {
            var field = new Masofa.Common.Models.CropMonitoring.Field
            {
                Id = Guid.Parse(id),
                Name = name,
                FieldArea = fieldArea,
                RegionId = Guid.Parse(regionId),
                ExternalId = externalId,
                Comment = comment,
                WaterSaving = waterSaving,
                SoilIndex = soilIndex,
                Control = true,
                Status = Common.Models.StatusType.Active,
                CreateAt = DateTime.Now,
                LastUpdateAt = lastUpdateAt,
                CreateUser = Guid.Empty,
                LastUpdateUser = Guid.Empty
            };

            if (!string.IsNullOrEmpty(polygonWkt))
            {
                try
                {
                    var geometry = wktReader.Read(polygonWkt);
                    if (geometry is Polygon polygon)
                    {
                        field.Polygon = polygon;
                    }
                }
                catch (Exception ex)
                {
                    // Логирование ошибки, если нужно
                    Console.WriteLine($"Ошибка парсинга полигона для поля {name}: {ex.Message}");
                }
            }

            return field;
        }

        private void InitFields()
        {
            fields.Add(AddField("87d1c710-7242-47f1-8ef9-2c8e78f59c56", "مطار بيروت-1", 15.0, "cd259f8e-8854-469f-8ede-3bb52ba403ac", "BEI_FIELD_001", "Тестовое поле в Бейрут", false, 6.8, "POLYGON ((35.4839 33.8179, 35.4929 33.8179, 35.4929 33.8239, 35.4839 33.8239, 35.4839 33.8179))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("e02ce475-a94c-49ed-bbd6-17773250f4e9", "مطار بيروت-2", 20.0, "cd259f8e-8854-469f-8ede-3bb52ba403ac", "BEI_FIELD_002", "Тестовое поле в Бейрут", true, 7.1, "POLYGON ((35.496 33.827, 35.504 33.827, 35.504 33.833, 35.496 33.833, 35.496 33.827))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("d0721f8b-8e7e-412a-b3f6-245157230dd6", "مطار بيروت-3", 25.0, "cd259f8e-8854-469f-8ede-3bb52ba403ac", "BEI_FIELD_003", "Тестовое поле в Бейрут", false, 7.4, "POLYGON ((35.506 33.82, 35.514 33.82, 35.514 33.826, 35.506 33.826, 35.506 33.82))", DateTime.Now, DateTime.Now));

            // Mount Lebanon
            fields.Add(AddField("7e1b5821-0b6a-408a-a8fd-fcf8da647eb7", "حقل جبل لبنان-1", 15.0, "d9dd8473-15bf-4a9c-af0e-5656e35ca2f7", "MLB_FIELD_001", "Тестовое поле в Горный Ливан", false, 6.8, "POLYGON ((35.586 33.82, 35.594 33.82, 35.594 33.826, 35.586 33.826, 35.586 33.82))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("e672b704-432f-489c-b2fe-1cd77790a478", "حقل جبل لبنان-2", 20.0, "d9dd8473-15bf-4a9c-af0e-5656e35ca2f7", "MLB_FIELD_002", "Тестовое поле в Горный Ливан", true, 7.1, "POLYGON ((35.596 33.82, 35.604 33.82, 35.604 33.826, 35.596 33.826, 35.596 33.82))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("0c93ac41-9b3e-423f-98b1-75af48a10d60", "حقل جبل لبنان-3", 25.0, "d9dd8473-15bf-4a9c-af0e-5656e35ca2f7", "MLB_FIELD_003", "Тестовое поле в Горный Ливан", false, 7.4, "POLYGON ((35.606 33.82, 35.614 33.82, 35.614 33.826, 35.606 33.826, 35.606 33.82))", DateTime.Now, DateTime.Now));

            // North Lebanon
            fields.Add(AddField("4391e591-d278-4f39-80af-ab8748b872af", "حقل شمال لبنان-1", 15.0, "1df98bff-db7c-43e9-956b-964a86db766d", "NLB_FIELD_001", "Тестовое поле в Северный Ливан", false, 6.8, "POLYGON ((35.8168 34.4237, 35.8248 34.4237, 35.8248 34.4297, 35.8168 34.4297, 35.8168 34.4237))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("b6ba55db-ebbc-40b2-98f2-57c46c37ac8f", "حقل شمال لبنان-2", 20.0, "1df98bff-db7c-43e9-956b-964a86db766d", "NLB_FIELD_002", "Тестовое поле в Северный Ливан", true, 7.1, "POLYGON ((35.8268 34.4237, 35.8348 34.4237, 35.8348 34.4297, 35.8268 34.4297, 35.8268 34.4237))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("2581f647-760c-4915-8519-51abece98a19", "حقل شمال لبنان-3", 25.0, "1df98bff-db7c-43e9-956b-964a86db766d", "NLB_FIELD_003", "Тестовое поле в Северный Ливан", false, 7.4, "POLYGON ((35.8368 34.4237, 35.8448 34.4237, 35.8448 34.4297, 35.8368 34.4297, 35.8368 34.4237))", DateTime.Now, DateTime.Now));

            // Akkar
            fields.Add(AddField("94c1dae8-7cd0-45d3-85f4-9d7ddf92219f", "حقل عكار-1", 15.0, "48d2b84d-f451-4b36-9256-6506dfeb9c5f", "AKK_FIELD_001", "Тестовое поле в Аккар", false, 6.8, "POLYGON ((36.0527 34.537, 36.0607 34.537, 36.0607 34.543, 36.0527 34.543, 36.0527 34.537))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("1f711a33-608a-4679-bef6-6ee642c29f20", "حقل عكار-2", 20.0, "48d2b84d-f451-4b36-9256-6506dfeb9c5f", "AKK_FIELD_002", "Тестовое поле в Аккар", true, 7.1, "POLYGON ((36.0627 34.537, 36.0707 34.537, 36.0707 34.543, 36.0627 34.543, 36.0627 34.537))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("93987b45-2b3d-491d-be8d-1f366a0c8b43", "حقل عكار-3", 25.0, "48d2b84d-f451-4b36-9256-6506dfeb9c5f", "AKK_FIELD_003", "Тестовое поле в Аккар", false, 7.4, "POLYGON ((36.0727 34.537, 36.0807 34.537, 36.0807 34.543, 36.0727 34.543, 36.0727 34.537))", DateTime.Now, DateTime.Now));

            // Bekaa
            fields.Add(AddField("2f7ab01e-2bbd-4b32-825f-3c5e5592b6ec", "حقل البقاع 1", 15.0, "da1f78dc-571d-446d-937b-88d64b3a6ab3", "BEK_FIELD_001", "Тестовое поле в Бекаа", false, 6.8, "POLYGON ((35.986 33.837, 35.994 33.837, 35.994 33.843, 35.986 33.843, 35.986 33.837))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("61569de3-229c-4de8-b05e-7606d50908dc", "حقل البقاع 2", 20.0, "da1f78dc-571d-446d-937b-88d64b3a6ab3", "BEK_FIELD_002", "Тестовое поле в Бекаа", true, 7.1, "POLYGON ((35.996 33.837, 36.004 33.837, 36.004 33.843, 35.996 33.843, 35.996 33.837))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("54966c95-3496-40c5-b75f-fc981a3c97a6", "حقل البقاع 3", 25.0, "da1f78dc-571d-446d-937b-88d64b3a6ab3", "BEK_FIELD_003", "Тестовое поле в Бекаа", false, 7.4, "POLYGON ((36.006 33.837, 36.014 33.837, 36.014 33.843, 36.006 33.843, 36.006 33.837))", DateTime.Now, DateTime.Now));

            // Baalbek-Hermel
            fields.Add(AddField("9a91761f-7e4a-42b3-be9d-9e91b5cc29ef", "حقل بعلبك-الهرمل-1", 15.0, "939cbb66-f885-4d4e-8a99-a24cf41d284d", "BAA_FIELD_001", "Тестовое поле в Баальбек-Хермель", false, 6.8, "POLYGON ((36.186 33.987, 36.194 33.987, 36.194 33.993, 36.186 33.993, 36.186 33.987))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("320da3ad-40f7-4bcc-8995-e2b375301074", "حقل بعلبك-الهرمل-2", 20.0, "939cbb66-f885-4d4e-8a99-a24cf41d284d", "BAA_FIELD_002", "Тестовое поле в Баальбек-Хермель", true, 7.1, "POLYGON ((36.196 33.987, 36.204 33.987, 36.204 33.993, 36.196 33.993, 36.196 33.987))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("870d6ec6-d62a-42d2-8081-c84813d26c6e", "حقل بعلبك-الهرمل-1", 25.0, "939cbb66-f885-4d4e-8a99-a24cf41d284d", "BAA_FIELD_003", "Тестовое поле в Баальбек-Хермель", false, 7.4, "POLYGON ((36.206 33.987, 36.214 33.987, 36.214 33.993, 36.206 33.993, 36.206 33.987))", DateTime.Now, DateTime.Now));

            // South Lebanon
            fields.Add(AddField("115937ba-c220-4db1-a8d0-3b255b225167", "حقل جنوب لبنان-1", 15.0, "401e68c1-4f81-4245-bd2d-9a61c49c23b1", "SLB_FIELD_001", "Тестовое поле в Южный Ливан", false, 6.8, "POLYGON ((35.1898 33.26, 35.1978 33.26, 35.1978 33.266, 35.1898 33.266, 35.1898 33.26))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("e01537f2-279c-43ad-95c9-d6f8274bf7be", "حقل جنوب لبنان-2", 20.0, "401e68c1-4f81-4245-bd2d-9a61c49c23b1", "SLB_FIELD_002", "Тестовое поле в Южный Ливан", true, 7.1, "POLYGON ((35.1998 33.26, 35.2078 33.26, 35.2078 33.266, 35.1998 33.266, 35.1998 33.26))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("e750206c-d51d-4ec2-be62-5f7107100f3e", "حقل جنوب لبنان-3", 25.0, "401e68c1-4f81-4245-bd2d-9a61c49c23b1", "SLB_FIELD_003", "Тестовое поле в Южный Ливан", false, 7.4, "POLYGON ((35.2098 33.26, 35.2178 33.26, 35.2178 33.266, 35.2098 33.266, 35.2098 33.26))", DateTime.Now, DateTime.Now));

            // Nabatieh
            fields.Add(AddField("fadc6778-162f-4cd8-bf10-57f1c247ec0b", "حقل النبطية-1", 15.0, "ab4f7e65-3f8f-49fb-b944-df6a76a520ff", "NAB_FIELD_001", "Тестовое поле в Набатия", false, 6.8, "POLYGON ((35.4699 33.3647, 35.4779 33.3647, 35.4779 33.3707, 35.4699 33.3707, 35.4699 33.3647))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("a40a59c7-4a7f-4078-9948-cdf3e3f83609", "حقل النبطية-2", 20.0, "ab4f7e65-3f8f-49fb-b944-df6a76a520ff", "NAB_FIELD_002", "Тестовое поле в Набатия", true, 7.1, "POLYGON ((35.4799 33.3647, 35.4879 33.3647, 35.4879 33.3707, 35.4799 33.3707, 35.4799 33.3647))", DateTime.Now, DateTime.Now));
            fields.Add(AddField("fd6ea124-4fb1-465d-906c-a47ae1ba871c", "حقل النبطية-3", 25.0, "ab4f7e65-3f8f-49fb-b944-df6a76a520ff", "NAB_FIELD_003", "Тестовое поле в Набатия", false, 7.4, "POLYGON ((35.4899 33.3647, 35.4979 33.3647, 35.4979 33.3707, 35.4899 33.3707, 35.4899 33.3647))", DateTime.Now, DateTime.Now));

        }

        private List<Masofa.Common.Models.CropMonitoring.Field> GetFieldsInTiffExtent(string tiffPath, IEnumerable<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            if (!File.Exists(tiffPath))
                throw new FileNotFoundException($"TIFF файл не найден: {tiffPath}");

            var result = new List<Masofa.Common.Models.CropMonitoring.Field>();

            // Открываем растровый датасет
            using var dataset = Gdal.Open(tiffPath, Access.GA_ReadOnly);
            if (dataset == null)
                throw new InvalidOperationException($"Не удалось открыть TIFF: {tiffPath}");

            // Получаем геотрансформацию (6 коэффициентов)
            double[] geoTransform = new double[6];
            dataset.GetGeoTransform(geoTransform);

            int width = dataset.RasterXSize;
            int height = dataset.RasterYSize;

            // Вычисляем координаты углов
            double x0 = geoTransform[0];                          // верхний левый X
            double y0 = geoTransform[3];                          // верхний левый Y
            double x1 = x0 + geoTransform[1] * width;             // верхний правый X
            double y1 = y0 + geoTransform[5] * height;            // нижний левый Y

            // Учитываем, что geoTransform[5] обычно отрицательный (Y идёт вниз)
            double minX = Math.Min(x0, x1);
            double maxX = Math.Max(x0, x1);
            double minY = Math.Min(y0, y1);
            double maxY = Math.Max(y0, y1);

            // Создаём ограничивающий прямоугольник (Envelope)
            var rasterEnvelope = new Envelope(minX, maxX, minY, maxY);
            var rasterPolygon = GeometryFactory.Default.CreatePolygon(new LinearRing(new[]
            {
                new Coordinate(minX, minY),
                new Coordinate(minX, maxY),
                new Coordinate(maxX, maxY),
                new Coordinate(maxX, minY),
                new Coordinate(minX, minY)
            }));

            // Проверяем каждое поле
            foreach (var field in fields)
            {
                if (field?.Polygon == null || field.Polygon.IsEmpty)
                    continue;

                // Проверка пересечения
                if (field.Polygon.Intersects(rasterPolygon))
                {
                    result.Add(field);
                }
            }

            return result;
        }

        private string OverlayPolygonsOnTiff(
            string inputTiffPath,
            IEnumerable<Masofa.Common.Models.CropMonitoring.Field> fields,
            SixLabors.ImageSharp.Color lineColor = default,
            int lineWidth = 2)
        {
            var outputTiffPath = Path.Combine(Path.GetDirectoryName(inputTiffPath), $"With_Field_{Path.GetFileName(inputTiffPath)}");
            if (lineColor == default) lineColor = SixLabors.ImageSharp.Color.Red;

            if (!File.Exists(inputTiffPath))
                throw new FileNotFoundException($"Файл не найден: {inputTiffPath}");

            // 1. Открываем GeoTIFF через GDAL
            using var srcDataset = Gdal.Open(inputTiffPath, Access.GA_ReadOnly);
            if (srcDataset == null)
                throw new InvalidOperationException("Не удалось открыть GeoTIFF.");

            int width = srcDataset.RasterXSize;
            int height = srcDataset.RasterYSize;
            int bandCount = srcDataset.RasterCount;

            if (bandCount != 1 && bandCount != 3 && bandCount != 4)
                throw new NotSupportedException("Поддерживаются только 1-, 3- или 4-канальные TIFF.");

            // 2. Читаем растр в байтовый массив
            var rasterData = new byte[width * height * bandCount];
            for (int i = 0; i < bandCount; i++)
            {
                var band = srcDataset.GetRasterBand(i + 1);
                var buffer = new byte[width * height];
                band.ReadRaster(0, 0, width, height, buffer, width, height, 0, 0);
                for (int j = 0; j < buffer.Length; j++)
                    rasterData[j * bandCount + i] = buffer[j];
            }

            // 3. Создаём ImageSharp-изображение
            using var image = new Image<Rgba32>(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = (y * width + x) * bandCount;
                    Rgba32 pixel;
                    if (bandCount == 1)
                    {
                        byte gray = rasterData[idx];
                        pixel = new Rgba32(gray, gray, gray, 255);
                    }
                    else if (bandCount == 3)
                    {
                        pixel = new Rgba32(
                            rasterData[idx],
                            rasterData[idx + 1],
                            rasterData[idx + 2],
                            255);
                    }
                    else // 4
                    {
                        pixel = new Rgba32(
                            rasterData[idx],
                            rasterData[idx + 1],
                            rasterData[idx + 2],
                            rasterData[idx + 3]);
                    }
                    image[x, y] = pixel;
                }
            }

            // 4. Получаем геотрансформацию
            double[] geoTransform = new double[6];
            srcDataset.GetGeoTransform(geoTransform);

            // Инвертируем для перевода гео → пиксели
            double inv0 = geoTransform[1];
            double inv1 = geoTransform[2];
            double inv3 = geoTransform[4];
            double inv5 = geoTransform[5];
            double det = inv0 * inv5 - inv1 * inv3;
            if (Math.Abs(det) < 1e-12)
                throw new InvalidOperationException("Невозможно инвертировать геотрансформацию.");

            double invDet = 1.0 / det;
            double a = inv5 * invDet;
            double b = -inv1 * invDet;
            double d = -inv3 * invDet;
            double e = inv0 * invDet;

            PointF GeoToPixel(Coordinate coord)
            {
                double dx = coord.X - geoTransform[0];
                double dy = coord.Y - geoTransform[3];
                float px = (float)(a * dx + b * dy);
                float py = (float)(d * dx + e * dy);
                return new PointF(px, py);
            }

            // 5. Рисуем контуры полигонов
            foreach (var field in fields)
            {
                if (field?.Polygon?.Coordinates == null || field.Polygon.IsEmpty)
                    continue;

                var coords = field.Polygon.Coordinates;
                if (coords.Length < 2) continue;

                var points = new PointF[coords.Length];
                for (int i = 0; i < coords.Length; i++)
                {
                    points[i] = GeoToPixel(coords[i]);
                }

                var pen = Pens.Solid(lineColor, lineWidth);
                // Рисуем только контур (без заливки)
                image.Mutate(ctx =>
                {
                    ctx.DrawLine(pen, points);
                });
            }

            // 6. Сохраняем как временный PNG (ImageSharp не пишет GeoTIFF!)
            using var pngStream = new MemoryStream();
            image.SaveAsPng(pngStream);
            pngStream.Position = 0;

            // 7. Теперь создаём GeoTIFF через GDAL, копируя геоданные
            var driver = Gdal.GetDriverByName("GTiff");
            using var dstDataset = driver.Create(
                outputTiffPath,
                width, height, bandCount,
                DataType.GDT_Byte,
                new[] { "COMPRESS=DEFLATE" });

            dstDataset.SetGeoTransform(geoTransform);
            var projection = srcDataset.GetProjection();
            if (!string.IsNullOrEmpty(projection))
                dstDataset.SetProjection(projection);

            // Читаем пиксели из PNG-потока обратно в байты
            using var tempImage = Image.Load<Rgba32>(pngStream);
            var outputData = new byte[width * height * bandCount];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixel = tempImage[x, y];
                    int idx = (y * width + x) * bandCount;
                    if (bandCount >= 1) outputData[idx] = pixel.R;
                    if (bandCount >= 2) outputData[idx + 1] = pixel.G;
                    if (bandCount >= 3) outputData[idx + 2] = pixel.B;
                    if (bandCount == 4) outputData[idx + 3] = pixel.A;
                }
            }

            // Записываем в GDAL-датасет
            for (int i = 0; i < bandCount; i++)
            {
                var band = dstDataset.GetRasterBand(i + 1);
                var channel = new byte[width * height];
                for (int j = 0; j < channel.Length; j++)
                    channel[j] = outputData[j * bandCount + i];
                band.WriteRaster(0, 0, width, height, channel, width, height, 0, 0);
            }

            return outputTiffPath;
        }
        
        #endregion


        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }
    }

    public class CreateReportCommandResultItem
    {
        public Guid ProductId { get; set; }
        public string ProductFilePath { get; set; }
        public DateTime OrinalDate { get; set; }
        public string ProductUnZipFilePath { get; set; }
        public string ARVIFilePath { get; set; }
        public string EVIFilePath { get; set; }
        public string GNDVIFilePath { get; set; }
        public string MNDWIFilePath { get; set; }
        public string NDMIFilePath { get; set; }
        public string NDVIFilePath { get; set; }
        public string ORVIFilePath { get; set; }
        public string OSAVIFilePath { get; set; }

        public string WithFieldARVIFilePath { get; set; }
        public string WithFieldEVIFilePath { get; set; }
        public string WithFieldGNDVIFilePath { get; set; }
        public string WithFieldMNDWIFilePath { get; set; }
        public string WithFieldNDMIFilePath { get; set; }
        public string WithFieldNDVIFilePath { get; set; }
        public string WithFieldORVIFilePath { get; set; }
        public string WithFieldOSAVIFilePath { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool IsComplite 
        { 
            get 
            {
                if (string.IsNullOrEmpty(ProductFilePath))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(ProductUnZipFilePath))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(ARVIFilePath))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(EVIFilePath))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(GNDVIFilePath))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(MNDWIFilePath))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(NDMIFilePath))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(NDVIFilePath))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(ORVIFilePath))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(OSAVIFilePath))
                {
                    return false;
                }
                return true;
            } 
        }

        [Newtonsoft.Json.JsonIgnore]
        public List<Masofa.Common.Models.CropMonitoring.Field> Fields { get; set; } = new List<Common.Models.CropMonitoring.Field>();
    }
}
