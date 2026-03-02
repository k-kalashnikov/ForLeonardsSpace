using Masofa.Common.Models.Satellite.Sentinel;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Cli.DevopsUtil.Commands.SatelliteDownloader.Sentinel2
{
    [BaseCommand("Move color index files to bucket root using DB records", "Uses FileStorageItem table to relocate .tif files")]
    public class StoringColorIndicesCommand : IBaseCommand
    {
        private IFileStorageProvider FileStorageItemProvider {  get; set; }
        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }

        public StoringColorIndicesCommand(
            IFileStorageProvider fileStorageProvider,
            MasofaCommonDbContext dbContext)
        {
            FileStorageItemProvider = fileStorageProvider;
            MasofaCommonDbContext = dbContext;
        }

        public void Dispose() { }

        public Task Execute() => Execute(Array.Empty<string>());

        public async Task Execute(string[] args)
        {
            const string searchPath = "/root/deploy/develop/";

            Console.WriteLine($"Fetching FileStorageItem records with path containing '{searchPath}'...");

            var itemsToMigrate = await MasofaCommonDbContext.FileStorageItems
                .Where(f => f.FileStoragePath.Contains(searchPath))
                .Where(f => f.FileContentType == FileContentType.ImageTiff)
                .ToListAsync();

            if (!itemsToMigrate.Any())
            {
                Console.WriteLine("ℹ️  No TIFF files found for migration.");
                return;
            }

            Console.WriteLine($"Found {itemsToMigrate.Count} TIFF records to migrate.");

            var total = itemsToMigrate.Count;
            var successCount = 0;
            var errorCount = 0;

            for (int i = 0; i < itemsToMigrate.Count; i++)
            {
                var item = itemsToMigrate[i];
                var progress = (int)((double)(i + 1) / total * 100);
                try
                {
                    var originalPath = item.FileStoragePath;
                    var fileName = Path.GetFileName(originalPath);

                    var extractPart = originalPath.Split('/')
                        .FirstOrDefault(p => p.StartsWith("extract_", StringComparison.OrdinalIgnoreCase));

                    string newFileName = !string.IsNullOrEmpty(extractPart)
                        ? $"{extractPart["extract_".Length..]}_{fileName}"
                        : fileName;

                    Console.Write($"\r Progress: {progress,3}% ({i + 1}/{total}) | Success: {successCount} | Errors: {errorCount} ");
                    //Console.WriteLine($"\n Processing: {item.FileStorageBacket}/{originalPath}");
                    //Console.WriteLine($"New name: {newFileName}");

                    await FileStorageItemProvider.CopyObjectAsync(
                        sourceBucket: item.FileStorageBacket,
                        sourceKey: originalPath,
                        destBucket: item.FileStorageBacket,
                        destKey: newFileName
                    );

                    item.FileStoragePath = newFileName;
                    await MasofaCommonDbContext.SaveChangesAsync();

                    //Console.WriteLine($"DB updated");

                    await FileStorageItemProvider.DeleteObjectAsync(
                        item.FileStorageBacket,
                        originalPath
                    );

                    //Console.WriteLine($"Original deleted");
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                    errorCount++;
                }
            }
            Console.WriteLine($"\n\n Migration complete!");
            Console.WriteLine($"   Total: {total}");
            Console.WriteLine($"   Success: {successCount}");
            Console.WriteLine($"   Errors: {errorCount}");
        }
    }
}
