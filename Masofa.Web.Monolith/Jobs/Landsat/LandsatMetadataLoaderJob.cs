//using Masofa.BusinessLogic.FieldSatellite.Requests;
//using Masofa.BusinessLogic.Services.BusinessLogicLogger;
//using Masofa.Client.EarthExplorer;
//using Masofa.Common.Attributes;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Services;
//using Masofa.DataAccess;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Quartz;
//using System.Text.Json;
//using static Masofa.Client.EarthExplorer.Repositories.LandsatProductRepository;

//namespace Masofa.Web.Monolith.Jobs.Landsat
//{
//    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "LandsatMetadataLoaderJob")]
//    public class LandsatMetadataLoaderJob : BaseJob<string>
//    {
//        private ILogger<LandsatMetadataLoaderJob> Logger { get; set; }
//        private LandsatApiUnitOfWork UnitOfWork { get; set; }
//        private MasofaLandsatDbContext LandsatDbContext { get; set; }
//        private MasofaCommonDbContext CommonDbContext { get; set; }
//        private IOptions<LandsatServiceOptions> Options { get; set; } 

//        public LandsatMetadataLoaderJob(
//            ILogger<LandsatMetadataLoaderJob> logger,
//            LandsatApiUnitOfWork unitOfWork,
//            IOptions<LandsatServiceOptions> options,
//            MasofaCommonDbContext commonDbContext,
//            MasofaLandsatDbContext landsatDbContext,
//            IMediator mediator,
//            IBusinessLogicLogger businessLogicLogger) : base(mediator, businessLogicLogger, logger)
//        {
//            UnitOfWork = unitOfWork;
//            Options = options;
//            CommonDbContext = commonDbContext;
//            LandsatDbContext = landsatDbContext;
//        }

//        public override async Task Execute(IJobExecutionContext context)
//        {
//            Logger.LogInformation("Start LandsatMetadataLoaderJob");

//            var processedCount = 0;
//            var errors = new List<string>();

//            try
//            {
//                // Загружаем активную конфигурацию из БД
//                var configRequest = new GetActiveConfigRequest();
//                var satelliteConfig = await Mediator.Send(configRequest);

//                var options = Options.Value;
//                options.SatelliteSearchConfig = satelliteConfig;

//                if (!UnitOfWork.IsAuthed)
//                    await UnitOfWork.LoginAsync(options);

//                // Получаем параметры из контекста
//                var dataMap = context.JobDetail.JobDataMap;
//                var productIdsString = dataMap.GetString("ProductIds");
//                var chainId = dataMap.GetString("ChainID");

//                List<LandsatProductQueue> queue;

//                // Если указаны конкретные продукты (Field задача) - работаем с ними
//                if (!string.IsNullOrEmpty(productIdsString))
//                {
//                    var productIds = productIdsString.Split(',').ToList();
//                    queue = await LandsatDbContext.Set<LandsatProductQueue>()
//                        .Where(x => productIds.Contains(x.ProductId) && x.Status == ProductQueueStatusType.New)
//                        .ToListAsync();
//                    Logger.LogInformation("Processing specific products for chain {ChainID}: {Count} items", chainId, queue.Count);
//                }
//                else
//                {
//                    // Обычная работа - берем из очереди
//                    queue = await LandsatDbContext.Set<LandsatProductQueue>()
//                        .Where(x => x.Status == ProductQueueStatusType.New)
//                        .Take(100)
//                        .ToListAsync();
//                    Logger.LogInformation("Processing from queue: {Count} items", queue.Count);
//                }

//                foreach (var item in queue)
//                {
//                    try
//                    {
//                        var product = await CommonDbContext.Set<SatelliteProduct>().FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
//                        if (product == null)
//                        {
//                            product = new SatelliteProduct
//                            {
//                                Id = Guid.NewGuid(),
//                                ProductId = item.ProductId,
//                                ProductSourceType = ProductSourceType.Landsat,
//                                CreateAt = DateTime.UtcNow,
//                                CreateUser = Guid.Empty,
//                                MediadataPath = Guid.Empty
//                            };
//                            await CommonDbContext.Set<SatelliteProduct>().AddAsync(product);
//                        }

//                        var metadata = await UnitOfWork.ProductRepository.GetProductMetadataAsync(
//                            options,
//                            "landsat_ot_c2_l2",
//                            item.ProductId,
//                            product.Id.ToString());

//                        await LandsatDbContext.Set<LandsatProductMetadata>().AddAsync(metadata);

//                        item.Status = ProductQueueStatusType.MetadataLoaded;
//                        LandsatDbContext.Set<LandsatProductQueue>().Update(item);

//                        await CommonDbContext.SaveChangesAsync();
//                        await LandsatDbContext.SaveChangesAsync();

//                        processedCount++;
//                    }
//                    catch (Exception ex)
//                    {
//                        errors.Add($"Error processing product {item.ProductId}: {ex.Message}");
//                        Logger.LogError(ex, "Error processing product {ProductId}", item.ProductId);
//                    }
//                }

//                // Джоба теперь просто выполняет загрузку метаданных и сохраняет результат
//                // Больше не запускает следующие джобы в цепочке

//                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
//                {
//                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success,
//                    TaskResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { 
//                        processedCount = processedCount,
//                        totalInQueue = queue.Count,
//                        errors = errors
//                    }),
//                    TaskResultJsonType = typeof(string)
//                }, context);
//            }
//            catch (Exception ex)
//            {
//                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
//                {
//                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed,
//                    TaskResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { 
//                        error = ex.Message,
//                        innerError = ex.InnerException?.Message,
//                        processedCount = processedCount,
//                        errors = errors
//                    }),
//                    TaskResultJsonType = typeof(string)
//                }, context);
//            }

//            Logger.LogInformation("End LandsatMetadataLoaderJob");
//        }

//    }
//}
