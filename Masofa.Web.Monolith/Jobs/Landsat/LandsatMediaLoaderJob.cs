//using Masofa.BusinessLogic.FieldSatellite.Requests;
//using Masofa.BusinessLogic.Services.BusinessLogicLogger;
//using Masofa.Client.EarthExplorer;
//using Masofa.Common.Attributes;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Models.SystemCrical;
//using Masofa.Common.Services;
//using Masofa.Common.Services.FileStorage;
//using Masofa.DataAccess;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Quartz;
//using System.IO;
//using System.Text.Json;

//namespace Masofa.Web.Monolith.Jobs.Landsat
//{
//    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "LandsatMediaLoaderJob")]
//    public class LandsatMediaLoaderJob : BaseJob<string>
//    {
//        private ILogger<LandsatMediaLoaderJob> Logger { get; set; }
//        private LandsatApiUnitOfWork UnitOfWork { get; set; }
//        private MasofaLandsatDbContext LandsatDbContext { get; set; }
//        private MasofaCommonDbContext CommonDbContext { get; set; }
//        private IOptions<LandsatServiceOptions> Options { get; set; }
//        private IFileStorageProvider FileStorageProvider { get; set; }

//        public LandsatMediaLoaderJob(
//            ILogger<LandsatMediaLoaderJob> logger,
//            LandsatApiUnitOfWork unitOfWork,
//            IOptions<LandsatServiceOptions> options,
//            IFileStorageProvider fileStorageProvider,
//            MasofaLandsatDbContext landsatDbContext,
//            MasofaCommonDbContext commonDbContext,
//            IMediator mediator,
//            IBusinessLogicLogger businessLogicLogger) : base(mediator, businessLogicLogger, logger)
//        {
//            UnitOfWork = unitOfWork;
//            Options = options;
//            FileStorageProvider = fileStorageProvider;
//            CommonDbContext = commonDbContext;
//            LandsatDbContext = landsatDbContext;
//        }

//        public override async Task Execute(IJobExecutionContext context)
//        {
//            Logger.LogInformation("Start LandsatMediaLoaderJob");

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
//                {
//                    await UnitOfWork.LoginAsync(options);
//                }

//                // Получаем параметры из контекста
//                var dataMap = context.JobDetail.JobDataMap;
//                var productIdsString = dataMap.GetString("ProductIds");
//                var chainId = dataMap.GetString("ChainID");

//                List<LandsatProductQueue> productQueue = new(default(int));
//                List<SatelliteProduct> needProducts = new(default(int));

//                // Если указаны конкретные продукты (Field задача) - работаем с ними
//                if (!string.IsNullOrEmpty(productIdsString))
//                {
//                    var productIds = productIdsString.Split(',').ToList();
//                    needProducts = await CommonDbContext.SatelliteProducts
//                        .Where(m => productIds.Contains(m.ProductId))
//                        .Where(m => m.MediadataPath.Equals(Guid.Empty))
//                        .ToListAsync();
//                    Logger.LogInformation("Processing specific products for chain {ChainID}: {Count} items", chainId, needProducts.Count);
//                }
//                else
//                {
//                    // Обычная работа - берем из очереди
//                    productQueue = await LandsatDbContext.Set<LandsatProductQueue>()
//                        .Where(m => m.Status == ProductQueueStatusType.MetadataLoaded)
//                        .ToListAsync();

//                    needProducts = await CommonDbContext.SatelliteProducts
//                        .Where(m => productQueue.Select(m => m.ProductId).Contains(m.ProductId))
//                        .Where(m => m.MediadataPath.Equals(Guid.Empty))
//                        .Take(15)
//                        .ToListAsync();
//                    Logger.LogInformation("Processing from queue: {Count} items", needProducts.Count);
//                }

//                foreach (var product in needProducts)
//                {
//                    try
//                    {
//                        if (!UnitOfWork.IsAuthed)
//                        {
//                            await UnitOfWork.LoginAsync(options);
//                        }

//                        using (var productStream = await UnitOfWork.ProductRepository.DownloadProductByIdAsync(product.ProductId))
//                        {
//                            Logger.LogInformation($"Downloaded product stream for {product.ProductId}");
//                            var filePath = string.Empty;
//                            using (var mStream = new MemoryStream())
//                            {
//                                await productStream.CopyToAsync(mStream);
//                                mStream.Position = 0;
//                                filePath = await FileStorageProvider.PushFileAsync(mStream, $"{product.ProductId}.zip", "landsat");
//                            }

//                            var fileStorageItem = new FileStorageItem()
//                            {
//                                CreateAt = DateTime.UtcNow,
//                                CreateUser = Guid.Empty,
//                                OwnerId = product.Id,
//                                OwnerTypeFullName = typeof(SatelliteProduct).FullName,
//                                FileContentType = FileContentType.ArchiveZIP,
//                                Status = Common.Models.StatusType.Active,
//                                FileStoragePath = filePath,
//                                FileStorageBacket = "landsat",
//                            };
//                            fileStorageItem = (await CommonDbContext.FileStorageItems.AddAsync(fileStorageItem))
//                                .Entity;
//                            product.MediadataPath = fileStorageItem.Id;
//                            CommonDbContext.SatelliteProducts.Update(product);
//                        }
//                        var tempPQ = productQueue.First(m => m.ProductId.Equals(product.ProductId));
//                        tempPQ.Status = ProductQueueStatusType.MediaLoaded;
//                        LandsatDbContext.Set<LandsatProductQueue>().Update(tempPQ);
//                        LandsatDbContext.SaveChanges();
//                        CommonDbContext.SaveChanges();

//                        processedCount++;
//                    }
//                    catch (Exception ex)
//                    {
//                        errors.Add($"Error processing product {product.ProductId}: {ex.Message}");
//                        Logger.LogError(ex, "Error processing product {ProductId}", product.ProductId);
//                    }
//                }

//                // Джоба теперь просто выполняет загрузку медиафайлов и сохраняет результат
//                // Больше не запускает следующие джобы в цепочке

//                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
//                {
//                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success,
//                    TaskResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { 
//                        processedCount = processedCount,
//                        totalInQueue = needProducts.Count,
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

//            Logger.LogInformation("End LandsatMediaLoaderJob");
//        }

//    }
//}
