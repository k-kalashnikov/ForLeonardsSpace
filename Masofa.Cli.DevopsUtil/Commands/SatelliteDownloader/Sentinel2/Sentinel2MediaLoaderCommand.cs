//using Masofa.Client.Copernicus;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Models.SystemCrical;
//using Masofa.Common.Services.FileStorage;
//using Masofa.DataAccess;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using NetTopologySuite.IO;
//using System.Net;

//namespace Masofa.Cli.DevopsUtil.Commands.Satellite
//{
//    [BaseCommand("Sentinel2 Media Loader", "Загрузка медиа файлов Sentinel2 из очереди")]
//    public class Sentinel2MediaLoaderCommand : IBaseCommand
//    {
//        ILogger<Sentinel2MediaLoaderCommand> Logger { get; }
//        private SentinelServiceOptions SentinelServiceOptions { get; }
//        private CopernicusApiUnitOfWork CopernicusApiUnitOfWork { get; }
//        private MasofaCommonDbContext CommonDbContext { get; }
//        private MasofaSentinelDbContext SentinelDbContext { get; }
//        private IFileStorageProvider FileStorageProvider { get; }


//        public Sentinel2MediaLoaderCommand(
//            ILogger<Sentinel2MediaLoaderCommand> logger,
//            CopernicusApiUnitOfWork copernicusApiUnitOfWork,
//            IFileStorageProvider fileStorageProvider,
//            MasofaCommonDbContext commonDbContext,
//            MasofaSentinelDbContext sentinelDbContext,
//            IOptions<SentinelServiceOptions> options)
//        {
//            SentinelServiceOptions = options.Value;
//            Logger = logger;
//            CopernicusApiUnitOfWork = copernicusApiUnitOfWork;
//            FileStorageProvider = fileStorageProvider;
//            CommonDbContext = commonDbContext;
//            SentinelDbContext = sentinelDbContext;
//        }

//        public async Task Execute()
//        {
//            Logger.LogInformation($"Start Sentinel2MediaLoaderJob");


//            var productQueue = await SentinelDbContext.Set<Sentinel2ProductQueue>()
//                .Where(m => m.Status == ProductQueueStatusType.MetadataLoaded)
//                .ToListAsync();

//            var needProducts = await CommonDbContext.SatelliteProducts
//                .Where(m => productQueue.Select(m => m.ProductId).Contains(m.ProductId))
//                .Where(m => m.MediadataPath.Equals(Guid.Empty))
//                .Take(100)
//                .ToListAsync();

//            foreach (var product in needProducts)
//            {
//                try
//                {
//                    if (!CopernicusApiUnitOfWork.IsAuthed)
//                    {
//                        await CopernicusApiUnitOfWork.LoginAsync(SentinelServiceOptions);
//                    }

//                    using (var productStream = await CopernicusApiUnitOfWork.ProductRepository.GetProductMediadataAsync(SentinelServiceOptions, new Guid(product.ProductId)))
//                    {
//                        Logger.LogInformation($"Downloaded product stream for {product.ProductId}");
//                        var filePath = string.Empty;
//                        using (var mStream = new MemoryStream())
//                        {
//                            await productStream.CopyToAsync(mStream);
//                            mStream.Position = 0;
//                            filePath = await FileStorageProvider.PushFileAsync(mStream, $"{product.ProductId}.zip", "sentinel");
//                        }

//                        var fileStorageItem = new FileStorageItem()
//                        {
//                            CreateAt = DateTime.UtcNow,
//                            CreateUser = Guid.Empty,
//                            OwnerId = product.Id,
//                            OwnerTypeFullName = typeof(SatelliteProduct).FullName,
//                            FileContentType = FileContentType.ArchiveZIP,
//                            Status = Common.Models.StatusType.Active,
//                            FileStoragePath = filePath,
//                            FileStorageBacket = "sentinel",
//                        };
//                        fileStorageItem = (await CommonDbContext.FileStorageItems.AddAsync(fileStorageItem))
//                            .Entity;
//                        product.MediadataPath = fileStorageItem.Id;
//                        CommonDbContext.SatelliteProducts.Update(product);
//                    }
//                    var tempPQ = productQueue.First(m => m.ProductId.Equals(product.ProductId));
//                    tempPQ.Status = ProductQueueStatusType.MediaLoaded;
//                    SentinelDbContext.Set<Sentinel2ProductQueue>().Update(tempPQ);
//                    SentinelDbContext.SaveChanges();
//                    CommonDbContext.SaveChanges();
//                }
//                catch (Exception ex)
//                {
//                    throw ex;
//                }
//            }

//            Logger.LogInformation($"End Sentinel2MediaLoaderJob");
//        }
//        public void Dispose()
//        {
//        }

//        public Task Execute(string[] args)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
