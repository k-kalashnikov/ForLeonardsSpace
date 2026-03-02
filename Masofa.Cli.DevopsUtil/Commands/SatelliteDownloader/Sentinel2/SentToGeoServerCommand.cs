//using Masofa.BusinessLogic.Services;
//using Masofa.Common;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Models.Satellite.Indices;
//using Masofa.Common.Models.SystemCrical;
//using Masofa.Common.Services.FileStorage;
//using Masofa.DataAccess;
//using Masofa.Web.Monolith;
//using Masofa.Web.Monolith.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2
//{
//    [BaseCommand("Publish TIFFs to GeoServer as daily layers", "Publishes one GeoTIFF per day to GeoServer")]
//    public class SentToGeoServerCommand : IBaseCommand
//    {
//        private  IFileStorageProvider _fileStorageProvider {  get; set; }
//        private MasofaCommonDbContext _dbContext { get; set; }
//        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
//        private GeoServerService GeoServerService {  get; set; }
//        private GeoServerOptions GeoServerOptions { get; set; }

//        private static readonly string[] ColorBuckets = new[]
//        {
//            "sentinelarvicolore", "sentinelevicolore", "sentinelgndvicolore",
//            "sentinelmndwicolore", "sentinelndmicolore", "sentinelndvicolore",
//            "sentinelorvicolore", "sentinelosavicolore"
//        };

//        private static readonly Dictionary<Type, string> IndexTypeToName = new()
//        {
//            { typeof(ArviPolygon), "ARVI" },
//            { typeof(EviPolygon), "EVI" },
//            { typeof(GndviPolygon), "GNDVI" },
//            { typeof(MndwiPolygon), "MNDWI" },
//            { typeof(NdmiPolygon), "NDMI" },
//            { typeof(NdviPolygon), "NDVI" },
//            { typeof(OrviPolygon), "ORVI" },
//            { typeof(OsaviPolygon), "OSAVI" }
//        };

//        public SentToGeoServerCommand(IFileStorageProvider fileStorageProvider, MasofaCommonDbContext dbContext, GeoServerService geoServerService, IOptions<GeoServerOptions> geoServerOptions, MasofaIndicesDbContext masofaIndicesDbContext)
//        {
//            _fileStorageProvider = fileStorageProvider;
//            _dbContext = dbContext;
//            GeoServerService = geoServerService;
//            GeoServerOptions = geoServerOptions.Value;
//            MasofaIndicesDbContext = masofaIndicesDbContext;
//        }

//        public void Dispose()
//        {
            
//        }

//        public Task Execute()
//        {
//            return Execute(Array.Empty<string>());
//        }

//        public async Task Execute(string[] args)
//        {
//            await GeoServerService.CreateWorkspaceAsync(GeoServerOptions.Workspace);

//            var startDate = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc);
//            var endDate = new DateTime(2025, 9, 30, 23, 59, 59, 999, DateTimeKind.Utc);

//            Console.WriteLine($"Publishing daily layers for September 2025...");

//            var satelliteProducts = await _dbContext.Set<SatelliteProduct>()
//                .Where(p => p.ProductSourceType == ProductSourceType.Sentinel2)
//                .Where(p => p.OriginDate.HasValue)
//                .Where(p => p.OriginDate.Value.Date >= startDate && p.OriginDate.Value.Date <= endDate)
//                .ToListAsync();

//            if (!satelliteProducts.Any())
//            {
//                Console.WriteLine("No Sentinel-2 products found in the last 30 days.");
//                return;
//            }

//            Console.WriteLine($"Found {satelliteProducts.Count} Sentinel-2 products.");

//            var filesByDateAndIndex = new Dictionary<(DateOnly Date, string Index), List<FileStorageItem>>();

//            foreach (var product in satelliteProducts)
//            {
//                var productDate = DateOnly.FromDateTime(product.OriginDate.Value.Date);
//                var indices = await GetIndicesForProduct(product.Id);

//                foreach (var index in indices)
//                {
//                    var fileItem = await _dbContext.FileStorageItems
//                        .FirstOrDefaultAsync(f => f.Id == index.FileStorageItemId);

//                    if (fileItem == null || !ColorBuckets.Contains(fileItem.FileStorageBacket))
//                        continue;

//                    //if (fileItem == null ||
//                    //    !fileItem.FileStoragePath.Contains("_vis", StringComparison.OrdinalIgnoreCase) ||
//                    //    !fileItem.FileStoragePath.EndsWith(".tif", StringComparison.OrdinalIgnoreCase))
//                    //{
//                    //    continue;
//                    //}

//                    var indexName = IndexTypeToName[index.GetType()];
//                    var key = (productDate, indexName);

//                    if (!filesByDateAndIndex.ContainsKey(key))
//                        filesByDateAndIndex[key] = new List<FileStorageItem>();

//                    filesByDateAndIndex[key].Add(fileItem);
//                }
//            }

//            if (!filesByDateAndIndex.Any())
//            {
//                Console.WriteLine("No index files found for the products.");
//                return;
//            }

//            Console.WriteLine($" Found files for {filesByDateAndIndex.Count} index types.");

//            var successCount = 0;
//            foreach (var ((date, indexName), files) in filesByDateAndIndex.OrderBy(x => x.Key).ThenBy(x => x.Key.Index))
//            {
//                try
//                {
//                    var storeName = $"{indexName}_{date:yyyyMMdd}";
//                    var mosaicFolderPath = Path.Combine($"/deploy/prod/data-geoserver-prod/{indexName}", storeName);
//                    Console.Write($"\r Publishing {storeName} ({files.Count} tiles)... ");

//                    var targetDir = GeoServerService.GetFullDataPath(mosaicFolderPath);
//                    if (Directory.Exists(targetDir))
//                        Directory.Delete(targetDir, recursive: true);
//                    Directory.CreateDirectory(targetDir);

//                    foreach (var file in files)
//                    {
//                        var targetFile = Path.Combine(targetDir, Path.GetFileName(file.FileStoragePath));
//                        using var stream = await _fileStorageProvider.GetFileStreamAsync(file.FileStoragePath, file.FileStorageBacket);
//                        using var fileStream = File.Create(targetFile);
//                        await stream.CopyToAsync(fileStream);
//                    }

//                    await GeoServerService.RecreateImageMosaicStoreAsync(storeName, Path.Combine(indexName, storeName));
//                    await GeoServerService.PublishCoverageAsync(storeName, storeName);

//                    successCount++;
//                }
//                catch(Exception ex)
//                {
//                    Console.WriteLine($"\n Error publishing {indexName}: {ex.Message}");
//                }
//            }

//            await GeoServerService.ReloadConfigurationAsync();
//            Console.WriteLine($"\n\n Published {successCount} full-country mosaic layers.");
//        }

//        private async Task<List<BaseIndexPolygon>> GetIndicesForProduct(Guid productId)
//        {
//            var indices = new List<BaseIndexPolygon>();

//            indices.AddRange(await MasofaIndicesDbContext.Set<ArviPolygon>().Where(x => x.SatelliteProductId == productId).ToListAsync());
//            indices.AddRange(await MasofaIndicesDbContext.Set<EviPolygon>().Where(x => x.SatelliteProductId == productId).ToListAsync());
//            indices.AddRange(await MasofaIndicesDbContext.Set<GndviPolygon>().Where(x => x.SatelliteProductId == productId).ToListAsync());
//            indices.AddRange(await MasofaIndicesDbContext.Set<MndwiPolygon>().Where(x => x.SatelliteProductId == productId).ToListAsync());
//            indices.AddRange(await MasofaIndicesDbContext.Set<NdmiPolygon>().Where(x => x.SatelliteProductId == productId).ToListAsync());
//            indices.AddRange(await MasofaIndicesDbContext.Set<NdviPolygon>().Where(x => x.SatelliteProductId == productId).ToListAsync());
//            indices.AddRange(await MasofaIndicesDbContext.Set<OrviPolygon>().Where(x => x.SatelliteProductId == productId).ToListAsync());
//            indices.AddRange(await MasofaIndicesDbContext.Set<OsaviPolygon>().Where(x => x.SatelliteProductId == productId).ToListAsync());

//            return indices;
//        }

//        private bool TryExtractDateAndIndexFromFileName(string fileName, out DateOnly date, out string index)
//        {
//            date = DateOnly.MinValue;
//            index = string.Empty;

//            var name = Path.GetFileNameWithoutExtension(fileName);
//            var parts = name.Split('_', StringSplitOptions.RemoveEmptyEntries);

//            foreach (var part in parts)
//            {
//                if (part.Length >= 8 && DateTime.TryParseExact(part.Substring(0, 8), "yyyyMMdd", null, DateTimeStyles.None, out var dt))
//                {
//                    date = DateOnly.FromDateTime(dt);
//                    break;
//                }
//            }

//            if (date == DateOnly.MinValue)
//                return false;

//            foreach (var part in parts)
//            {
//                var upper = part.ToUpperInvariant();
//                if (upper is "ARVI" or "NDVI" or "EVI" or "ORVI" or "OSAVI" or "GNDVI" or "MNDWI" or "NDMI")
//                {
//                    index = upper;
//                    return true;
//                }
//            }

//            return false;
//        }
//    }
//}
