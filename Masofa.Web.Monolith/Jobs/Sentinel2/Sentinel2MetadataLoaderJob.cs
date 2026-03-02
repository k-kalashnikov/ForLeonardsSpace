using Masofa.BusinessLogic.FieldSatellite.Requests;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.Copernicus;
using Masofa.Common.Attributes;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Services;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Linq;
using System.Text.Json;

namespace Masofa.Web.Monolith.Jobs.Sentinel2
{
    /// <summary>
    /// Джоб для загрузки метаданных Sentinel2 продуктов
    /// </summary>
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "Sentinel2MetadataLoaderJob", "Sentinel")]
    public class Sentinel2MetadataLoaderJob : BaseJob<Sentinel2MetadataLoaderJobResult>, IJob
    {
        private readonly ILogger<Sentinel2MetadataLoaderJob> _logger;
        private readonly CopernicusApiUnitOfWork _unitOfWork;
        private readonly IFileStorageProvider _fileStorageProvider;
        private readonly MasofaCommonDbContext _commonDbContext;
        private readonly MasofaSentinelDbContext _sentinelDbContext;
        private readonly IConfiguration _configuration;

        public Sentinel2MetadataLoaderJob(
            ILogger<Sentinel2MetadataLoaderJob> logger,
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
            _logger.LogInformation($"Start Sentinel2MetadataLoaderJob");

            try
            {
                // Обычная обработка очереди
               var productQueue = await _sentinelDbContext.Set<Sentinel2ProductQueue>()
                    .Where(m => m.QueueStatus == ProductQueueStatusType.New)
                    .Take(100)
                    .ToListAsync();


                var existProducts = await _commonDbContext.SatelliteProducts
                    .Where(m => productQueue.Select(m => m.ProductId).Contains(m.ProductId))
                    .ToListAsync();

                var exitsMetadatas = await _sentinelDbContext.Set<Sentinel2ProductMetadata>()
                    .Where(m => existProducts.Select(m => m.Id.ToString()).Contains(m.ProductId))
                    .ToListAsync();

                existProducts = existProducts.Where(p => exitsMetadatas.Select(pm => pm.ProductId).Contains(p.Id.ToString()))
                    .ToList();

                productQueue = productQueue.Where(pq => !existProducts.Select(p => p.ProductId).Contains(pq.ProductId))
                    .ToList();

                foreach (var item in productQueue)
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

                        var result = await _unitOfWork.ProductRepository.GetProductMetadataAsync(options, new Guid(item.ProductId));

                        var product = existProducts.FirstOrDefault(m => m.ProductId.Equals(item.ProductId));
                        if (product == null)
                        {
                            product = new SatelliteProduct()
                            {
                                ProductId = item.ProductId,
                                CreateAt = DateTime.UtcNow,
                                ProductSourceType = ProductSourceType.Sentinel2,
                                CreateUser = Guid.Empty,
                                Status = Common.Models.StatusType.Active,
                            };

                            await _commonDbContext.SatelliteProducts.AddAsync(product);
                        }

                        result.ProductId = product.Id.ToString();
                        await _sentinelDbContext.Set<Sentinel2ProductMetadata>().AddAsync(result);
                        item.QueueStatus = ProductQueueStatusType.MetadataLoaded;

                        _sentinelDbContext.Set<Sentinel2ProductQueue>().Update(item);
                        _sentinelDbContext.SaveChanges();
                        _commonDbContext.SaveChanges();
                        
                        Result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        Result.Errors.Add($"Error processing product {item.ProductId}: {ex.Message}");
                        _logger.LogError(ex, "Error processing product {ProductId}", item.ProductId);
                    }
                }

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

            _logger.LogInformation($"End Sentinel2MetadataLoaderJob");
        }

    }

    public class Sentinel2MetadataLoaderJobResult : BaseJobResult
    {
        public int SuccessCount { get; set; } = 0;
    }
}
