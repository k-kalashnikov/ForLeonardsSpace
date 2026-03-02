using Masofa.BusinessLogic.Index;
using Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2;
using Masofa.Client.Copernicus;
using Masofa.Common.Helper;
using Masofa.Common.Models;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.Satellite.Parse.Sentinel.Inspire;
using Masofa.Common.Models.Satellite.Sentinel;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MaxRev.Gdal.Core;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using OSGeo.OSR;
using Quartz.Logging;
using System.IO.Compression;
using System.Threading;

namespace Masofa.Cli.DevopsUtil.Commands.Demo
{
    [BaseCommand("AddDataForDemoCommand", "AddDataForDemoCommand")]
    public class AddDataForDemoCommand : IBaseCommand
    {

        private SentinelServiceOptions SentinelServiceOptions { get; }
        private CopernicusApiUnitOfWork CopernicusApiUnitOfWork { get; }
        private MasofaSentinelDbContext SentinelDbContext { get; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }
        private MasofaIdentityDbContext IdentityDbContext { get; set; }

        private static bool GdalInitialized = false;
        private const double EPS = 1e-8;
        private static int outputH = 0;
        private static readonly object consoleLock = new object();
        private static readonly object dbContextLock = new object();
        private static readonly object fileStorageLock = new object();
        private Dictionary<Guid, Dictionary<string, bool>> globalResult = new Dictionary<Guid, Dictionary<string, bool>>();
        private string TempPath = "/";

        private static readonly int _minValidPixels = 50;
        private static readonly int _srid = 4326;
        private static readonly double _singlePointSize = 10.0;
        private static readonly double _lineBufferRadius = 5.0;

        private static readonly Dictionary<string, (double low, double high)> _thresholds = new()
        {
            ["ndvi"] = (0.15, 0.85),
            ["ndmi"] = (0.15, 0.85),
            ["ndwi"] = (0.15, 0.85),
            ["gndvi"] = (0.15, 0.85),
            ["orvi"] = (0.15, 0.85)
        };

        private readonly Dictionary<AnomalyType, string> _colors = new()
        {
            [AnomalyType.Flooding] = "#1f78b4",
            [AnomalyType.Overwatering] = "#a6cee3",
            [AnomalyType.Drought] = "#e31a1c",
            [AnomalyType.SkippingOfSeedlings] = "#ff7f00",
            [AnomalyType.PrematureAging] = "#b2df8a",
            [AnomalyType.NutritionalStress] = "#6a3d9a"
        };


        public AddDataForDemoCommand(ILogger<ParallelesPointAndTiffFetchProductCommand> logger,
            CopernicusApiUnitOfWork copernicusApiUnitOfWork,
            MasofaSentinelDbContext sentinelDbContext,
            IOptions<SentinelServiceOptions> options,
            MasofaCommonDbContext masofaCommonDbContext,
            IFileStorageProvider fileStorageProvider,
            MasofaIndicesDbContext masofaIndicesDbContext,
            MasofaCropMonitoringDbContext masofaCropMonitoringDbContext,
            MasofaIdentityDbContext identityDbContext)
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
            IdentityDbContext = identityDbContext;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task Execute()
        {
            var needProducts = MasofaCommonDbContext.SatelliteProducts
                .OrderByDescending(sp => sp.OriginDate)
                .ToList();
            var satelliteFields = GetSatelluteProductWithFields(needProducts);
            var satelliteProducts = needProducts.Where(m => satelliteFields.Keys.Contains(m.Id)).ToList();

            var archives = MasofaCommonDbContext.FileStorageItems
                .Where(f => satelliteProducts.Select(m => m.MediadataPath).ToList().Contains(f.Id))
                .ToList();
            Console.WriteLine("Enter pls tempPath");
            TempPath = Console.ReadLine();

            foreach (var item in satelliteProducts)
            {
                var archive = archives.FirstOrDefault(f => f.Id == item.MediadataPath);
                if (archive == null)
                {
                    Console.WriteLine($"Archive not found for SatelliteProductId: {item.Id}");
                    continue;
                }
                Console.WriteLine($"SatelliteProductId: {item.Id}, FileStoragePath: {archive.FileStoragePath}");

                string tempZipPath = Path.Combine(TempPath, $"{item.ProductId}.zip");

                if (!File.Exists(tempZipPath))
                {
                    var zipStream = await FileStorageProvider.GetFileStreamAsyncWithProgress(archive);
                    using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
                    {
                        await zipStream.CopyToAsync(fileStream);
                    }
                }
            }

            await CreatePartionsTables();

            //await Generate(satelliteFields, needProducts);
            try
            {
                await GenerateAnomalies(satelliteProducts);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }

        #region Private Methods
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
        #endregion

        #region Generate Tiff and Points

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
                    .Where(f => f.Id.ToString() == "019b315c-59f8-7e86-a181-36598069192f")
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

        private async Task Generate(Dictionary<Guid, List<Field>> satelliteFields, List<SatelliteProduct> needProducts)
        {
            Console.WriteLine("Enter pls tempPath");
            TempPath = Console.ReadLine();
            var errors = new List<string>();
            var errorLogPath = Path.Combine(TempPath, $"errors-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm")}.md");
            errors.Add($"| 🐉 | ProductId | OriginalDate | Error |");
            errors.Add($"| - | - | - | - |");
            var errorIndex = 0;
            Console.Clear();
            var outputLine = 3;

            var result = new List<Sentinel2ProductQueue>();
            DrawProgressBar(outputLine, 0, satelliteFields.Count, "GenerateTiffProducts", $"Found {satelliteFields.Count} products to process.");
            var index = -1;
            var allSeasons = MasofaCropMonitoringDbContext.Seasons.AsNoTracking()
                .Where(s => s.FieldId != null)
                .Where(s => s.FieldId.ToString() == "019b315c-59f8-7e86-a181-36598069192f")
                .ToList();

            foreach (var productWithField in satelliteFields)
            {
                index++;
                var product = needProducts.First(np => np.Id == productWithField.Key);
                var fields = productWithField.Value;
                var fieldIds = fields.Select(f => f.Id).ToList();
                var seasons = allSeasons.Where(s => fieldIds.Contains(s.FieldId.Value)).ToList();
                DrawProgressBar(outputLine, index, satelliteFields.Count, "GenerateTiffProducts", $"Start for {index} of {satelliteFields.Count} product at {product.OriginDate.Value.ToString("yyyy-MM-dd")}");

                string tempExtractPath = null;
                DateTime originalDate = (DateTime)product.OriginDate;

                try
                {
                    var zipFileStorageItem = await MasofaCommonDbContext.FileStorageItems.FirstOrDefaultAsync(f => f.Id == product.MediadataPath);
                    if (zipFileStorageItem == null)
                    {
                        DrawProgressBar(outputLine, index, satelliteFields.Count, "GenerateTiffProducts", $"FileStorageItem not found for MediadataPath: {product.MediadataPath}");
                        continue;
                    }

                    DrawProgressBar(outputLine, index, satelliteFields.Count, "GenerateTiffProducts", $"Start download : {zipFileStorageItem.FileStoragePath}");
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




                    DrawProgressBar(outputLine, index, satelliteFields.Count, "GenerateTiffProducts", $"Downloaded ZIP to: {tempZipPath}");
                    ZipFile.ExtractToDirectory(tempZipPath, tempExtractRoot);
                    DrawProgressBar(outputLine, index, satelliteFields.Count, "GenerateTiffProducts", $"Extracted ZIP to: {tempExtractRoot}");

                    var imgDataPath = IndicesHelper.FindImgDataPath(tempExtractRoot);
                    if (string.IsNullOrEmpty(imgDataPath))
                    {
                        DrawProgressBar(outputLine, index, satelliteFields.Count, "GenerateTiffProducts", "IMG_DATA path could not be determined. Skipping product.");
                        continue;
                    }

                    var productEntity = SentinelDbContext.Sentinel2Products.First(m => m.SatellateProductId == product.Id.ToString());
                    var productInspire = SentinelDbContext.SentinelInspireMetadata.First(m => m.Id == productEntity.SentinelInspireMetadataId);

                    await GenerateProductPoints(satelliteFields.Count, outputLine, index, product.Id, seasons, fields, productInspire, product, imgDataPath);

                    var generateResult = await GenerateProductTiffs(imgDataPath, product.Id.ToString());
                    foreach (var item in generateResult)
                    {
                        try
                        {
                            var tempItem = item;
                            tempItem.Value.ProductId = product.ProductId;
                            tempItem.Value.SatelliteProductId = product.Id;
                            tempItem.Value.OriginDate = new DateTime(productInspire.DateStamp.Year, productInspire.DateStamp.Month, productInspire.DateStamp.Day, productInspire.DateStamp.Hour, productInspire.DateStamp.Minute, productInspire.DateStamp.Second, DateTimeKind.Utc);
                            await SendToMinioTiffActions(tempItem.Key, tempItem.Value);
                            DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Complite {tempItem.Key} for {product.Id}");
                        }
                        catch (Exception ex)
                        {
                            DrawProgressBar(outputLine, index, needProducts.Count, "GenerateTiffProducts", $"Exception: {ex.Message}");
                        }

                    }

                    SentinelDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    errors.Add($"| {errorIndex} | {product.Id} | {product.OriginDate.Value.ToString("yyyy-MM-dd")} | {ex.Message} |");
                    errorIndex++;
                }
            }
        }



        public async Task GenerateProductPoints(int totalCount, int outputLine, int index, Guid productId,
    List<Season> seasons, List<Field> fields,
    SentinelInspireMetadata productInspire, SatelliteProduct product,
    string imgDataPath)
        {
            
            var generateResult = new DbGenerationResult()
            {
                OriginDate = new DateTime(productInspire.DateStamp.Year, productInspire.DateStamp.Month, productInspire.DateStamp.Day, productInspire.DateStamp.Hour, productInspire.DateStamp.Minute, productInspire.DateStamp.Second, DateTimeKind.Utc),
                SatelliteProductId = product.Id,
                ProductId = product.ProductId,
                ArviPoints = IndicesHelper.ArchiveFolderWorkDbARVI(imgDataPath, product.ProductId, fields),
                EviPoints = IndicesHelper.ArchiveFolderWorkDbEVI(imgDataPath, product.ProductId, fields),
                GndviPoints = IndicesHelper.ArchiveFolderWorkDbGNDVI(imgDataPath, product.ProductId, fields),
                MndwiPoints = IndicesHelper.ArchiveFolderWorkDbMNDWI(imgDataPath, product.ProductId, fields),
                NdmiPoints = IndicesHelper.ArchiveFolderWorkDbNDMI(imgDataPath, product.ProductId, fields),
                NdviPoints = IndicesHelper.ArchiveFolderWorkDbNDVI(imgDataPath, product.ProductId, fields),
                NdwiPoints = IndicesHelper.ArchiveFolderWorkDbNDWI(imgDataPath, product.ProductId, fields),
                OrviPoints = IndicesHelper.ArchiveFolderWorkDbORVI(imgDataPath, product.ProductId, fields),
                OsaviPoints = IndicesHelper.ArchiveFolderWorkDbOSAVI(imgDataPath, product.ProductId, fields)
            };
            //(MasofaIndicesDbContext.ArviPoints.Any(m => (m.SatelliteProductId == product.Id) && (m.FieldId == Guid.Parse("019b315c-59f8-7e86-a181-36598069192f")))) ? new List<ArviPoint>() :
            if (generateResult.ArviPoints.Any())
            {
                var resultPoints = new List<ArviPoint>();
                foreach (var item in generateResult.ArviPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point)
                        && s.PlantingDate <= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day)
                        && s.HarvestingDate >= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day));
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
                    if (indexSeason.Key == null)
                    {
                        continue;
                    }
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
            }

            if (generateResult.EviPoints.Any())
            {
                var resultPoints = new List<EviPoint>();
                foreach (var item in generateResult.EviPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point)
                        && s.PlantingDate <= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day)
                        && s.HarvestingDate >= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day));
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
                    if (indexSeason.Key == null)
                    {
                        continue;
                    }
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
            }

            if (generateResult.GndviPoints.Any())
            {
                var resultPoints = new List<GndviPoint>();
                foreach (var item in generateResult.GndviPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point)
                        && s.PlantingDate <= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day)
                        && s.HarvestingDate >= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day));
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
                    if (indexSeason.Key == null)
                    {
                        continue;
                    }
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
            }

            if (generateResult.MndwiPoints.Any())
            {
                var resultPoints = new List<MndwiPoint>();
                foreach (var item in generateResult.MndwiPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point)
                        && s.PlantingDate <= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day)
                        && s.HarvestingDate >= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day));
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
                    if (indexSeason.Key == null)
                    {
                        continue;
                    }
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
            }

            if (generateResult.NdmiPoints.Any())
            {
                var resultPoints = new List<NdmiPoint>();
                foreach (var item in generateResult.NdmiPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point)
                        && s.PlantingDate <= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day)
                        && s.HarvestingDate >= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day));
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
                    if (indexSeason.Key == null)
                    {
                        continue;
                    }
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
            }

            if (generateResult.NdviPoints.Any())
            {
                var resultPoints = new List<NdviPoint>();
                foreach (var item in generateResult.NdviPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point)
                        && s.PlantingDate <= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day)
                        && s.HarvestingDate >= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day));
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
                    if (indexSeason.Key == null)
                    {
                        continue;
                    }
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
            }

            if (generateResult.NdwiPoints.Any())
            {
                var resultPoints = new List<NdwiPoint>();
                foreach (var item in generateResult.NdwiPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point)
                        && s.PlantingDate <= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day)
                        && s.HarvestingDate >= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day));
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
                    if (indexSeason.Key == null)
                    {
                        continue;
                    }
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
            }

            if (generateResult.OrviPoints.Any())
            {
                var resultPoints = new List<OrviPoint>();
                foreach (var item in generateResult.OrviPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point)
                        && s.PlantingDate <= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day)
                        && s.HarvestingDate >= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day));
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
                    if (indexSeason.Key == null)
                    {
                        continue;
                    }
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
            }

            if (generateResult.OsaviPoints.Any())
            {
                var resultPoints = new List<OsaviPoint>();
                foreach (var item in generateResult.OsaviPoints)
                {
                    var temp = item;
                    var tempSeason = seasons.FirstOrDefault(s => s.Polygon.Covers(temp.Point) 
                        && s.PlantingDate <= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day)
                        && s.HarvestingDate >= new DateOnly(generateResult.OriginDate.Year, generateResult.OriginDate.Month, generateResult.OriginDate.Day));

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
                    if (indexSeason.Key == null)
                    {
                        continue;
                    }
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
        }


        public async Task<Dictionary<string, TiffGenerationResult>> GenerateProductTiffs(string archiveFolder, string productId)
        {
            var result = new Dictionary<string, TiffGenerationResult>();

            var indicexFlags = typeof(Sentinel2GenerateIndexStatus).GetProperties()
                .Where(m => m.Name.Contains("Tiff"))
                .ToList();

            var generateTasks = new List<Task>();

            foreach (var item in indicexFlags)
            {

                var indexName = item.Name.Replace("Tiff", string.Empty);
                var indexMethod = typeof(IndicesHelper).GetMethod($"ArchiveFolderWork{indexName.ToUpper()}");
                if (indexMethod == null)
                {
                    continue;
                }
                var indexResult = (TiffGenerationResult)indexMethod.Invoke(null, [archiveFolder, productId]);
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

        #region Generate Anomalies

        public async Task GenerateAnomalies(List<SatelliteProduct> products)
        {
            var lastUpdateUser = await IdentityDbContext.Users.FirstAsync(m => m.UserName.ToLower().Equals("admin"));
            foreach (var product in products)
            {
                var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory();

                var gndviPoints = await MasofaIndicesDbContext.GndviPoints.AsNoTracking().Where(i => i.SatelliteProductId == product.Id).ToListAsync();
                var ndmiPoints = await MasofaIndicesDbContext.NdmiPoints.AsNoTracking().Where(i => i.SatelliteProductId == product.Id).ToListAsync();
                var ndviPoints = await MasofaIndicesDbContext.NdviPoints.AsNoTracking().Where(i => i.SatelliteProductId == product.Id).ToListAsync();
                var ndwiPoints = await MasofaIndicesDbContext.NdwiPoints.AsNoTracking().Where(i => i.SatelliteProductId == product.Id).ToListAsync();
                var orviPoints = await MasofaIndicesDbContext.OrviPoints.AsNoTracking().Where(i => i.SatelliteProductId == product.Id).ToListAsync();

                if (gndviPoints.Count == 0 || ndmiPoints.Count == 0 || ndviPoints.Count == 0 || ndwiPoints.Count == 0 || orviPoints.Count == 0) 
                    return;

                var gndviSeasons = gndviPoints.Where(p => p.SeasonId != null).GroupBy(i => i.SeasonId.Value).ToDictionary(x => x.Key, x => x.ToList());
                var ndmiSeasons = ndmiPoints.Where(p => p.SeasonId != null).GroupBy(i => i.SeasonId.Value).ToDictionary(x => x.Key, x => x.ToList());
                var ndviSeasons = ndviPoints.Where(p => p.SeasonId != null).GroupBy(i => i.SeasonId.Value).ToDictionary(x => x.Key, x => x.ToList());
                var ndwiSeasons = ndwiPoints.Where(p => p.SeasonId != null).GroupBy(i => i.SeasonId.Value).ToDictionary(x => x.Key, x => x.ToList());
                var orviSeasons = orviPoints.Where(p => p.SeasonId != null).GroupBy(i => i.SeasonId.Value).ToDictionary(x => x.Key, x => x.ToList());


                ProductSourceType productSourceType = ProductSourceType.Sentinel2;
                if (ndviPoints.Count > 0)
                {
                    productSourceType = ndviPoints[0].ProductSourceType;
                }

                HashSet<Guid> seasonsIds = [];
                seasonsIds.UnionWith(gndviSeasons.Keys);
                seasonsIds.UnionWith(ndmiSeasons.Keys);
                seasonsIds.UnionWith(ndviSeasons.Keys);
                seasonsIds.UnionWith(ndwiSeasons.Keys);
                seasonsIds.UnionWith(orviSeasons.Keys);

                List<AnomalyPoint> anomalyPoints = [];
                Dictionary<Guid, List<AnomalyPoint>> anomalyPointsBySeasonId = [];

                foreach (var seasonId in seasonsIds)
                {
                    List<AnomalyPoint> seasonPoints = [];
                    var validGndviPoints = gndviSeasons[seasonId].Where(p => p.Point != null && !double.IsNaN(p.Value)).Select(p => p.Point).ToList();
                    var validNdmiPoints = ndmiSeasons[seasonId].Where(p => p.Point != null && !double.IsNaN(p.Value)).Select(p => p.Point).ToList();
                    var validNdviPoints = ndviSeasons[seasonId].Where(p => p.Point != null && !double.IsNaN(p.Value)).Select(p => p.Point).ToList();
                    var validNdwiPoints = ndwiSeasons[seasonId].Where(p => p.Point != null && !double.IsNaN(p.Value)).Select(p => p.Point).ToList();
                    var validOrviPoints = orviSeasons[seasonId].Where(p => p.Point != null && !double.IsNaN(p.Value)).Select(p => p.Point).ToList();

                    HashSet<NetTopologySuite.Geometries.Point> validPoints = [];
                    validPoints.UnionWith(validGndviPoints);
                    validPoints.UnionWith(validNdmiPoints);
                    validPoints.UnionWith(validNdviPoints);
                    validPoints.UnionWith(validNdwiPoints);
                    validPoints.UnionWith(validOrviPoints);

                    if (validPoints.Count < _minValidPixels)
                    {
                        continue;
                    }

                    var gndviVals = gndviSeasons[seasonId].Select(p => (double)p.Value).ToArray();
                    var ndmiVals = ndmiSeasons[seasonId].Select(p => (double)p.Value).ToArray();
                    var ndviVals = ndviSeasons[seasonId].Select(p => (double)p.Value).ToArray();
                    var ndwiVals = ndwiSeasons[seasonId].Select(p => (double)p.Value).ToArray();
                    var orviVals = orviSeasons[seasonId].Select(p => (double)p.Value).ToArray();

                    if (gndviVals.Length == 0 || ndmiVals.Length == 0 || ndviVals.Length == 0 || ndwiVals.Length == 0 || orviVals.Length == 0) return;

                    var ndviLow = Quantile(ndviVals, _thresholds["ndvi"].low);
                    var ndviVeryLow = Quantile(ndviVals, _thresholds["ndvi"].low * 0.5);
                    var ndmiLow = Quantile(ndmiVals, _thresholds["ndmi"].low);
                    var ndmiHigh = Quantile(ndmiVals, _thresholds["ndmi"].high);
                    var ndwiLow = Quantile(ndwiVals, _thresholds["ndwi"].low);
                    var ndwiHigh = Quantile(ndwiVals, _thresholds["ndwi"].high);
                    var gndviLow = Quantile(gndviVals, _thresholds["gndvi"].low);
                    var orviLow = Quantile(orviVals, _thresholds["orvi"].low);

                    foreach (var p in validPoints)
                    {
                        var pGndvi = gndviPoints.FirstOrDefault(i => i.Point == p);
                        var pNdmi = ndmiPoints.FirstOrDefault(i => i.Point == p);
                        var pNdvi = ndviPoints.FirstOrDefault(i => i.Point == p);
                        var pNdwi = ndwiPoints.FirstOrDefault(i => i.Point == p);
                        var pOrvi = orviPoints.FirstOrDefault(i => i.Point == p);

                        var pGndviValue = pGndvi?.Value;
                        var pNdmiValue = pNdmi?.Value;
                        var pNdviValue = pNdvi?.Value;
                        var pNdwiValue = pNdwi?.Value;
                        var pOrviVallue = pOrvi?.Value;

                        bool ndviLowFlag = pNdviValue < ndviLow;
                        bool ndviVeryLowFlag = pNdviValue < ndviVeryLow;
                        bool ndmiLowFlag = pNdmiValue < ndmiLow;
                        bool ndmiHighFlag = pNdmiValue > ndmiHigh;
                        bool ndwiLowFlag = pNdwiValue < ndwiLow;
                        bool ndwiHighFlag = pNdwiValue > ndwiHigh;
                        bool chlLow = (pGndviValue < gndviLow) || (pOrviVallue < orviLow);

                        AnomalyType? anomalyType = null;

                        if (ndviLowFlag && ndwiHighFlag)
                        {
                            anomalyType = AnomalyType.Flooding;
                        }
                        else if (ndviLowFlag && ndmiHighFlag)
                        {
                            anomalyType = AnomalyType.Overwatering;
                        }
                        else if (ndviLowFlag && ndmiLowFlag && ndwiLowFlag)
                        {
                            anomalyType = AnomalyType.Drought;
                        }
                        else if (ndviVeryLowFlag && chlLow)
                        {
                            anomalyType = AnomalyType.SkippingOfSeedlings;
                        }
                        else if (ndviLowFlag && chlLow)
                        {
                            anomalyType = AnomalyType.PrematureAging;
                        }
                        else if (chlLow && !ndmiLowFlag)
                        {
                            anomalyType = AnomalyType.NutritionalStress;
                        }

                        var newAnomalyPoint = new AnomalyPoint()
                        {
                            AnomalyType = anomalyType,
                            Color = anomalyType == null ? null : _colors.GetValueOrDefault(anomalyType.Value, null),
                            Point = p,
                            RegionId = pNdvi.RegionId,
                            FieldId = pNdvi.FieldId,
                            SeasonId = pNdvi.SeasonId,
                            CreateAt = pNdvi.CreateAt,
                            ProductSourceType = pNdvi.ProductSourceType,
                            SatelliteProductId = pNdvi.SatelliteProductId
                        };

                        seasonPoints.Add(newAnomalyPoint);
                    }
                    anomalyPoints.AddRange(seasonPoints);
                    anomalyPointsBySeasonId[seasonId] = anomalyPoints;
                }

                foreach (var (seasonId, seasonPoints) in anomalyPointsBySeasonId)
                {
                    if (seasonPoints.Count == 0) 
                        continue;

                    var pointsWithoutAnomaly = seasonPoints.Where(p => p.AnomalyType == null).ToList();
                    await MasofaIndicesDbContext.AnomalyPoints.AddRangeAsync(pointsWithoutAnomaly);
                    await MasofaIndicesDbContext.SaveChangesAsync();

                    var pointsByAnomaly = seasonPoints.Where(p => p.AnomalyType != null).GroupBy(p => p.AnomalyType.Value).ToDictionary(x => x.Key, x => x.ToList());

                    foreach (var (anomalyType, points) in pointsByAnomaly)
                    {
                        var clusters = ClusterPointsByDistance(points.Where(p => p.AnomalyType != null).Select(p => p.Point).ToList(), 10.0);

                        List<NetTopologySuite.Geometries.Polygon> polygons = [];

                        foreach (var cluster in clusters)
                        {
                            if (cluster.Count == 1)
                            {
                                var pt = cluster[0];
                                var half = _singlePointSize / 2.0;
                                var square = geometryFactory.CreatePolygon(
                                [
                                    new NetTopologySuite.Geometries.Coordinate(pt.X - half, pt.Y - half),
                                    new NetTopologySuite.Geometries.Coordinate(pt.X + half, pt.Y - half),
                                    new NetTopologySuite.Geometries.Coordinate(pt.X + half, pt.Y + half),
                                    new NetTopologySuite.Geometries.Coordinate(pt.X - half, pt.Y + half),
                                    new NetTopologySuite.Geometries.Coordinate(pt.X - half, pt.Y - half)
                                ]);
                                polygons.Add(square);
                            }
                            else if (cluster.Count == 2)
                            {
                                var p1 = cluster[0];
                                var p2 = cluster[1];
                                var line = geometryFactory.CreateLineString([p1.Coordinate, p2.Coordinate]);

                                var buffered = line.Buffer(_lineBufferRadius, NetTopologySuite.Operation.Buffer.EndCapStyle.Flat);

                                if (buffered is NetTopologySuite.Geometries.Polygon poly && !poly.IsEmpty)
                                {
                                    polygons.Add(poly);
                                }
                                else if (buffered is NetTopologySuite.Geometries.MultiPolygon mp && mp.NumGeometries > 0)
                                {
                                    polygons.AddRange(mp.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().ToList());
                                }
                            }
                            else
                            {
                                var poly = CreateConcaveHullFromPoints(cluster, 0.3);
                                if (poly != null && poly.Count > 0)
                                {
                                    polygons.AddRange(poly);
                                }
                            }

                        }

                        foreach (var p in polygons)
                        {
                            var newPolygon = new AnomalyPolygon()
                            {
                                OriginalDate = points[0].CreateAt,
                                ProductSourceType = productSourceType,
                                SatelliteProductId = points[0].SatelliteProductId,
                                AnomalyType = anomalyType,
                                Color = _colors.GetValueOrDefault(anomalyType, string.Empty),
                                Polygon = p,
                                RegionId = points[0].RegionId,
                                FieldId = points[0].FieldId,
                                SeasonId = points[0].SeasonId,
                                CreateAt = DateTime.UtcNow,
                                CreateUser = lastUpdateUser.Id,
                                LastUpdateUser = lastUpdateUser.Id,
                                LastUpdateAt = DateTime.UtcNow,
                                Status = StatusType.Active
                            };

                            await MasofaIndicesDbContext.AnomalyPolygons.AddAsync(newPolygon);
                            await MasofaIndicesDbContext.SaveChangesAsync();

                            var currentPolygonPoints = points.Where(p => newPolygon.Polygon.Covers(p.Point)).ToList();
                            foreach (var pn in currentPolygonPoints)
                            {
                                pn.AnomalyPolygonId = newPolygon.Id;
                            }

                            await MasofaIndicesDbContext.AnomalyPoints.AddRangeAsync(currentPolygonPoints);
                            await MasofaIndicesDbContext.SaveChangesAsync();
                        }
                    }
                }
            }

        }

        private static double Quantile(double[] data, double q)
        {
            if (data.Length == 0)
            {
                return double.NaN;
            }

            return Accord.Statistics.Measures.Quantile(data, q);
        }

        private static List<NetTopologySuite.Geometries.Polygon>? CreateConcaveHullFromPoints(List<NetTopologySuite.Geometries.Point> points, double ratio = 0.25)
        {
            if (points.Count < 3)
            {
                return null;
            }

            var geometryFactory = new NetTopologySuite.Geometries.GeometryFactory();
            var multiPoint = geometryFactory.CreateMultiPoint(points.ToArray());

            var concaveHull = NetTopologySuite.Algorithm.Hull.ConcaveHull.ConcaveHullByLengthRatio(multiPoint, ratio);

            if (concaveHull is NetTopologySuite.Geometries.Polygon polygon)
            {
                polygon.SRID = _srid;
                return [polygon];
            }

            if (concaveHull is NetTopologySuite.Geometries.MultiPolygon mp)
            {
                var pols = mp.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().ToList();
                foreach (var p in pols)
                {
                    p.SRID = _srid;
                }

                return pols;
            }

            return null;
        }

        private static List<List<NetTopologySuite.Geometries.Point>> ClusterPointsByDistance(List<NetTopologySuite.Geometries.Point> points, double maxDistance)
        {
            if (points.Count == 0) return new();

            var clusters = new List<List<NetTopologySuite.Geometries.Point>>();
            var visited = new bool[points.Count];
            var tree = new NetTopologySuite.Index.Strtree.STRtree<NetTopologySuite.Geometries.Point>();

            for (int i = 0; i < points.Count; i++)
                tree.Insert(points[i].EnvelopeInternal, points[i]);

            for (int i = 0; i < points.Count; i++)
            {
                if (visited[i]) continue;

                var cluster = new List<NetTopologySuite.Geometries.Point>();
                var queue = new Queue<NetTopologySuite.Geometries.Point>();
                queue.Enqueue(points[i]);
                visited[i] = true;

                while (queue.TryDequeue(out var current))
                {
                    cluster.Add(current);
                    var env = new NetTopologySuite.Geometries.Envelope(
                        current.X - maxDistance, current.X + maxDistance,
                        current.Y - maxDistance, current.Y + maxDistance);

                    var neighbors = tree.Query(env)
                        .OfType<NetTopologySuite.Geometries.Point>()
                        .Where(p => !visited[points.IndexOf(p)] && p.Distance(current) <= maxDistance);

                    foreach (var neighbor in neighbors)
                    {
                        int idx = points.IndexOf(neighbor);
                        if (!visited[idx])
                        {
                            visited[idx] = true;
                            queue.Enqueue(neighbor);
                        }
                    }
                }

                clusters.Add(cluster);
            }

            return clusters;
        }

        #endregion

        #region Export to GeoServer

        #endregion

        private async Task CreatePartionsTables()
        {
            var listIndex = new List<string>()
            {
                "AnomalyPoints"
            };

            foreach (var item in listIndex)
            {
                try
                {
                    for (DateOnly dateOnly = new DateOnly(2022, 01, 01); dateOnly < DateOnly.FromDateTime(DateTime.Now); dateOnly = dateOnly.AddDays(1))
                    {
                        string text = $"Create for {item} on {dateOnly.ToString("yyyy_MM_dd")}";
                        Console.Clear();
                        Console.SetCursorPosition(0, 4);
                        Console.Write(text.PadRight(Console.WindowWidth - 1));
                        MasofaIndicesDbContext.CreatePartitionForDateAsync(item, dateOnly);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException?.Message);
                }

            }
        }
    }
}
