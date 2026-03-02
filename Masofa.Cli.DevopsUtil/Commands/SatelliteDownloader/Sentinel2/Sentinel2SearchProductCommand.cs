//using Masofa.Client.Copernicus;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Models.SystemCrical;
//using Masofa.DataAccess;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//namespace Masofa.Cli.DevopsUtil.Commands.Satellite
//{
//    [BaseCommand("Sentinel2 Search Product", "Поиск продуктов Sentinel2 за последние 5 дней")]
//    public class Sentinel2SearchProductCommand : IBaseCommand
//    {
//        ILogger<Sentinel2SearchProductCommand> Logger { get; }
//        private SentinelServiceOptions SentinelServiceOptions { get; }
//        private CopernicusApiUnitOfWork CopernicusApiUnitOfWork { get; }
//        private MasofaSentinelDbContext SentinelDbContext { get; }

//        public Sentinel2SearchProductCommand(
//            ILogger<Sentinel2SearchProductCommand> logger,
//            CopernicusApiUnitOfWork copernicusApiUnitOfWork,
//            MasofaCommonDbContext commonDbContext,
//            MasofaSentinelDbContext sentinelDbContext,
//            IOptions<SentinelServiceOptions> options)
//        {
//            SentinelServiceOptions = options.Value;
//            Logger = logger;
//            CopernicusApiUnitOfWork = copernicusApiUnitOfWork;
//            SentinelDbContext = sentinelDbContext;
//        }

//        public async Task Execute()
//        {
//            Logger.LogInformation($"Start Sentinel2SearchProductJob");


//            if (!CopernicusApiUnitOfWork.IsAuthed)
//            {
//                await CopernicusApiUnitOfWork.LoginAsync(SentinelServiceOptions);
//            }

//            var productIds = await CopernicusApiUnitOfWork.ProductRepository.SearchProductAsync(SentinelServiceOptions, DateTime.Now.AddDays(-5), DateTime.Now);

//            if (productIds == null || !productIds.Any())
//            {
//                Logger.LogInformation("No suitable products found for AOI.");
//                return;
//            }

//            var allQueue = await SentinelDbContext.Set<Sentinel2ProductQueue>().ToListAsync();

//            var existsIds = allQueue
//                .Where(m => Guid.TryParse(m.ProductId, out var id) && productIds.Contains(id))
//                .Select(m => m.ProductId)
//                .ToList();

//            foreach (var item in productIds.Where(m => !existsIds.Contains(m.ToString())))
//            {
//                var tempPQ = new Sentinel2ProductQueue()
//                {
//                    CreateAt = DateTime.UtcNow,
//                    Id = Guid.NewGuid(),
//                    ProductId = item.ToString(),
//                    Status = ProductQueueStatusType.New,
//                    CreateUser = Guid.Empty
//                };

//                await SentinelDbContext.Set<Sentinel2ProductQueue>().AddAsync(tempPQ);
//            }

//            await SentinelDbContext.SaveChangesAsync();

//            Logger.LogInformation($"End Sentinel2SearchProductJob");
//        }
//        public void Dispose()
//        {
//        }

//        public Task Execute(string[] args)
//        {
//            return Execute();
//        }
//    }
//}
