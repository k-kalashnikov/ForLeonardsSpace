using Masofa.BusinessLogic.FieldSatellite.Requests;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.Copernicus;
using Masofa.Common.Attributes;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System.IO;
using System.Text.Json;

namespace Masofa.Web.Monolith.Jobs.Sentinel2
{
    /// <summary>
    /// Джоб для загрузки медиафайлов Sentinel2 продуктов
    /// </summary>
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "Sentinel2MediaLoaderJob", "Sentinel")]
    public class Sentinel2MediaLoaderJob : BaseJob<Sentinel2MediaLoaderJobResult>, IJob
    {
        private readonly ILogger<Sentinel2MediaLoaderJob> _logger;
        private readonly CopernicusApiUnitOfWork _unitOfWork;
        private readonly IFileStorageProvider _fileStorageProvider;
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly MasofaSentinelDbContext _sentinelDbContext;
        private readonly IConfiguration _configuration;

        public Sentinel2MediaLoaderJob(
            ILogger<Sentinel2MediaLoaderJob> logger,
            CopernicusApiUnitOfWork unitOfWork,
            IFileStorageProvider fileStorageProvider,
            MasofaCommonDbContext commonDbContext,
            MasofaSentinelDbContext sentinelDbContext,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger, 
            MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _fileStorageProvider = fileStorageProvider;
            _commonDbContext = commonDbContext;
            _sentinelDbContext = sentinelDbContext;
            _configuration = configuration;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Start Sentinel2MediaLoaderJob");
            try
            {
                // Обычная обработка очереди
                var productQueue = await _sentinelDbContext.Set<Sentinel2ProductQueue>()
                    .Where(m => m.QueueStatus == ProductQueueStatusType.MetadataLoaded)
                    .ToListAsync();

                var needProducts = await _commonDbContext.SatelliteProducts
                    .Where(m => productQueue.Select(m => m.ProductId).Contains(m.ProductId))
                    .Where(m => m.MediadataPath.Equals(Guid.Empty))
                    .Take(15)
                    .ToListAsync();
                
                foreach (var product in needProducts)
                {
                    try
                    {
                        // Загружаем активную конфигурацию из БД
                        var configRequest = new GetActiveConfigRequest();
                        var satelliteConfig = await Mediator.Send(configRequest);

                        var options = new SentinelServiceOptions()
                        {
                            ApiUrl = _configuration.GetValue<string>("Sentinel:AoiWkt"),
                            UserName = _configuration.GetValue<string>("Sentinel:UserName"),
                            Password = _configuration.GetValue<string>("Sentinel:Password"),
                            TokenApiUrl = _configuration.GetValue<string>("Sentinel:TokenApiUrl"),
                            ProductSearchApiUrl = _configuration.GetValue<string>("Sentinel:ProductSearchApiUrl"),
                            ProductDownloadApiUrl = _configuration.GetValue<string>("Sentinel:ProductDownloadApiUrl"),
                            SatelliteSearchConfig = satelliteConfig
                        };

                        if (!_unitOfWork.IsAuthed)
                        {
                            await _unitOfWork.LoginAsync(options);
                        }

                        using (var productStream = await _unitOfWork.ProductRepository.GetProductMediadataAsync(options, new Guid(product.ProductId)))
                        {
                            _logger.LogInformation($"Downloaded product stream for {product.ProductId}");
                            var filePath = string.Empty;
                            long fileLength = 0;
                            using (var mStream = new MemoryStream())
                            {
                                await productStream.CopyToAsync(mStream);
                                mStream.Position = 0;
                                fileLength = mStream.Length;
                                filePath = await _fileStorageProvider.PushFileAsync(mStream, $"{product.ProductId}.zip", "sentinel");
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
                                FileStorageBacket = "sentinel",
                                FileLength = fileLength,
                            };
                            fileStorageItem = (await _commonDbContext.FileStorageItems.AddAsync(fileStorageItem))
                                .Entity;
                            product.MediadataPath = fileStorageItem.Id;
                            _commonDbContext.SatelliteProducts.Update(product);
                        }
                        var tempPQ = productQueue.First(m => m.ProductId.Equals(product.ProductId));
                        tempPQ.QueueStatus = ProductQueueStatusType.MediaLoaded;
                        _sentinelDbContext.Set<Sentinel2ProductQueue>().Update(tempPQ);
                        _sentinelDbContext.SaveChanges();
                        _commonDbContext.SaveChanges();
                        
                        Result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        Result.Errors.Add($"Error processing product {product.ProductId}: {ex.Message}");
                        _logger.LogError(ex, "Error processing product {ProductId}", product.ProductId);
                    }
                }

                // Джоба теперь просто выполняет загрузку медиафайлов и сохраняет результат
                // Больше не запускает следующие джобы в цепочке

                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success
                }, context);
            }
            catch (Exception ex)
            {
                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed
                }, context);
            }

            _logger.LogInformation($"End Sentinel2MediaLoaderJob");
        }

    }

    public class Sentinel2MediaLoaderJobResult : BaseJobResult
    {
        public int SuccessCount { get; set; } = 0;
    }
}
