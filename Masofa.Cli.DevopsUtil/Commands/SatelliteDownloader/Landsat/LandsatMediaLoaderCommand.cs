using Masofa.Client.EarthExplorer;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Masofa.Cli.DevopsUtil.Commands.Satellite
{
    [BaseCommand("Landsat Media Loader", "Загрузка медиа файлов Landsat из очереди")]
    public class LandsatMediaLoaderCommand : IBaseCommand
    {
        private ILogger<LandsatMediaLoaderCommand> Logger { get; set; }
        private LandsatApiUnitOfWork UnitOfWork { get; set; }
        private MasofaLandsatDbContext LandsatDbContext { get; set; }
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private LandsatServiceOptions Options { get; set; }
        private IFileStorageProvider FileStorageProvider { get; set; }

        public LandsatMediaLoaderCommand(
            ILogger<LandsatMediaLoaderCommand> logger,
            LandsatApiUnitOfWork unitOfWork,
            IOptions<LandsatServiceOptions> options,
            IFileStorageProvider fileStorageProvider,
            MasofaLandsatDbContext landsatDbContext,
            MasofaCommonDbContext commonDbContext)
        {
            Logger = logger;
            UnitOfWork = unitOfWork;
            Options = options.Value;
            FileStorageProvider = fileStorageProvider;
            CommonDbContext = commonDbContext;
            LandsatDbContext = landsatDbContext;
        }

        //public async Task Execute()
        //{
        //    Logger.LogInformation("Start LandsatProductDownloadJob");

        //    if (!UnitOfWork.IsAuthed)
        //    {
        //        await UnitOfWork.LoginAsync(Options);
        //    }

        //    var items = await LandsatDbContext.Set<LandsatProductQueue>()
        //        .Where(x => x.Status == ProductQueueStatusType.MetadataLoaded)
        //        .Take(100)
        //        .ToListAsync();

        //    foreach (var item in items)
        //    {
        //        var product = await CommonDbContext.SatelliteProducts.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
        //        if (product == null)
        //        {
        //            continue;
        //        }


        //        try
        //        {
        //            Logger.LogInformation($"Downloaded product stream for {product.ProductId}");
        //            var filePath = string.Empty;
        //            using (var mStream = new MemoryStream())
        //            {
        //                await productStream.CopyToAsync(mStream);
        //                mStream.Position = 0;
        //                filePath = await FileStorageProvider.PushFileAsync(mStream, $"{product.ProductId}.zip", "sentinel2");
        //            }
        //            //var stream = await UnitOfWork.ProductRepository.DownloadProductByIdAsync(product.ProductId);
        //            //if (stream == null)
        //            //{
        //            //    Logger.LogWarning("Skipping product {ProductId}, stream not available.", product.ProductId);
        //            //    continue;
        //            //}
        //            //var filePath = await FileStorageProvider.PushFileAsync(stream, product.ProductId + ".zip", "landsat");
        //            var fileStorageItem = new FileStorageItem()
        //            {
        //                CreateAt = DateTime.Now,
        //                CreateUser = Guid.Empty,
        //                FileContentType = FileContentType.ArchiveZIP,
        //                Status = Common.Models.StatusType.Active,
        //                FileStoragePath = filePath,
        //                FileStorageBacket = "Landsat"
        //            };
        //            fileStorageItem = (await CommonDbContext.FileStorageItems.AddAsync(fileStorageItem)).Entity;
        //            product.MediadataPath = fileStorageItem.Id;
        //            CommonDbContext.SatelliteProducts.Update(product);

        //            item.Status = ProductQueueStatusType.MediaLoaded;
        //            LandsatDbContext.Set<LandsatProductQueue>().Update(item);

        //            await CommonDbContext.SaveChangesAsync();
        //            await LandsatDbContext.SaveChangesAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.LogError(ex, "Error downloading Landsat product: {ProductId}", product.ProductId);
        //        }
        //    }

        //    Logger.LogInformation("End LandsatProductDownloadJob");
        //}

        public async Task Execute()
        {
            Logger.LogInformation($"Start LandsatProductDownloadJob");


            var productQueue = await LandsatDbContext.Set<LandsatProductQueue>()
                .Where(m => m.Status == ProductQueueStatusType.MetadataLoaded)
                .ToListAsync();

            var needProducts = await CommonDbContext.SatelliteProducts
                .Where(m => productQueue.Select(m => m.ProductId).Contains(m.ProductId))
                .Where(m => m.MediadataPath.Equals(Guid.Empty))
                .Take(100)
                .ToListAsync();

            foreach (var product in needProducts)
            {
                try
                {
                    if (!UnitOfWork.IsAuthed)
                    {
                        await UnitOfWork.LoginAsync(Options);
                    }

                    using (var productStream = await UnitOfWork.ProductRepository.DownloadProductByIdAsync(product.ProductId))
                    {
                        Logger.LogInformation($"Downloaded product stream for {product.ProductId}");
                        var filePath = string.Empty;
                        using (var mStream = new MemoryStream())
                        {
                            await productStream.CopyToAsync(mStream);
                            mStream.Position = 0;
                            filePath = await FileStorageProvider.PushFileAsync(mStream, $"{product.ProductId}.zip", "landsat");
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
                            FileStorageBacket = "landsat",
                        };
                        fileStorageItem = (await CommonDbContext.FileStorageItems.AddAsync(fileStorageItem))
                            .Entity;
                        product.MediadataPath = fileStorageItem.Id;
                        CommonDbContext.SatelliteProducts.Update(product);
                    }
                    var tempPQ = productQueue.First(m => m.ProductId.Equals(product.ProductId));
                    tempPQ.Status = ProductQueueStatusType.MediaLoaded;
                    LandsatDbContext.Set<LandsatProductQueue>().Update(tempPQ);
                    LandsatDbContext.SaveChanges();
                    CommonDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            Logger.LogInformation($"End Sentinel2MediaLoaderJob");
        }

        public void Dispose()
        {
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
