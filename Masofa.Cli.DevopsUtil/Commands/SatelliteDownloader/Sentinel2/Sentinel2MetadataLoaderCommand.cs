//using Masofa.Client.Copernicus;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Models.SystemCrical;
//using Masofa.DataAccess;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//namespace Masofa.Cli.DevopsUtil.Commands.Satellite
//{
//    [BaseCommand("Sentinel2 Metadata Loader", "Загрузка метаданных Sentinel2 из очереди")]
//    public class Sentinel2MetadataLoaderCommand : IBaseCommand
//    {
//        ILogger<Sentinel2MetadataLoaderCommand> Logger { get; }
//        private SentinelServiceOptions SentinelServiceOptions { get; }
//        private CopernicusApiUnitOfWork CopernicusApiUnitOfWork { get; }
//        private MasofaCommonDbContext CommonDbContext { get; }
//        private MasofaSentinelDbContext SentinelDbContext { get; }


//        public Sentinel2MetadataLoaderCommand(
//            ILogger<Sentinel2MetadataLoaderCommand> logger,
//            CopernicusApiUnitOfWork copernicusApiUnitOfWork,
//            MasofaSentinelDbContext sentinelDbContext,
//            MasofaCommonDbContext commonDbContext,
//            IOptions<SentinelServiceOptions> options)
//        {
//            SentinelServiceOptions = options.Value;
//            Logger = logger;
//            CopernicusApiUnitOfWork = copernicusApiUnitOfWork;
//            CommonDbContext = commonDbContext;
//            SentinelDbContext = sentinelDbContext;
//        }

//        public async Task Execute()
//        {
//            Logger.LogInformation($"Start Sentinel2MetadataLoaderJob");


//            var productQueue = await SentinelDbContext.Set<Sentinel2ProductQueue>()
//                .Where(m => m.Status == ProductQueueStatusType.New)
//                .ToListAsync();

//            var existProducts = await CommonDbContext.SatelliteProducts
//                .Where(m => productQueue.Select(m => m.ProductId).Contains(m.ProductId))
//                .ToListAsync();

//            var exitsMetadatas = await SentinelDbContext.Set<Sentinel2ProductMetadata>()
//                .Where(m => existProducts.Select(m => m.Id.ToString()).Contains(m.ProductId))
//                .ToListAsync();

//            existProducts = existProducts.Where(p => exitsMetadatas.Select(pm => pm.ProductId).Contains(p.Id.ToString()))
//                .ToList();

//            productQueue = productQueue.Where(pq => !existProducts.Select(p => p.ProductId).Contains(pq.ProductId))
//                .Take(100)
//                .ToList();

//            foreach (var item in productQueue)
//            {
//                if (!CopernicusApiUnitOfWork.IsAuthed)
//                {
//                    await CopernicusApiUnitOfWork.LoginAsync(SentinelServiceOptions);
//                }

//                var result = await CopernicusApiUnitOfWork.ProductRepository.GetProductMetadataAsync(SentinelServiceOptions, new Guid(item.ProductId));

//                var product = existProducts.FirstOrDefault(m => m.ProductId.Equals(item.ProductId));
//                if (product == null)
//                {
//                    product = new SatelliteProduct()
//                    {
//                        ProductId = item.ProductId,
//                        CreateAt = DateTime.UtcNow,
//                        ProductSourceType = ProductSourceType.Sentinel2,
//                        CreateUser = Guid.Empty,
//                        Status = Common.Models.StatusType.Active,
//                    };

//                    await CommonDbContext.SatelliteProducts.AddAsync(product);
//                }

//                result.ProductId = product.Id.ToString();
//                await SentinelDbContext.Set<Sentinel2ProductMetadata>().AddAsync(result);
//                item.Status = ProductQueueStatusType.MetadataLoaded;

//                SentinelDbContext.Set<Sentinel2ProductQueue>().Update(item);
//                SentinelDbContext.SaveChanges();
//                CommonDbContext.SaveChanges();
//            }

//            Logger.LogInformation($"End Sentinel2MetadataLoaderJob");

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
