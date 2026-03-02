using Masofa.BusinessLogic.FieldSatellite.Requests;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.Copernicus;
using Masofa.Common.Attributes;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Services;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System.Text.Json;
using static NodaTime.TimeZones.ZoneEqualityComparer;

namespace Masofa.Web.Monolith.Jobs.Sentinel2
{
    /// <summary>
    /// Джоб для поиска Sentinel2 продуктов
    /// </summary>
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "Sentinel2SearchProductJob", "Sentinel")]
    public class Sentinel2SearchProductJob : BaseJob<Sentinel2SearchProductJobResult>, IJob
    {
        private readonly ILogger<Sentinel2SearchProductJob> _logger;
        private readonly CopernicusApiUnitOfWork _unitOfWork;
        private readonly MasofaSentinelDbContext _sentinelDbContext;
        private readonly IConfiguration _configuration;

        public Sentinel2SearchProductJob(
            ILogger<Sentinel2SearchProductJob> logger,
            CopernicusApiUnitOfWork unitOfWork,
            MasofaSentinelDbContext sentinelDbContext,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger, 
            MasofaCommonDbContext commonDbContext, 
            MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _sentinelDbContext = sentinelDbContext;
            _configuration = configuration;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Start Sentinel2SearchProductJob");
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
                                
                var startDate = DateTime.Now.AddDays(-5);
                var endDate = DateTime.Now;
                _logger.LogInformation("Using default date range: {StartDate} - {EndDate}", startDate, endDate);

                var productIds = await _unitOfWork.ProductRepository.SearchProductAsync(options, startDate, endDate);

                if (productIds == null || !productIds.Any())
                {
                    Result.NewProducts = 0;
                    Result.PolygonJson = options.SatelliteSearchConfig.SentinelPolygon?.AsText() ?? string.Empty;

                    _logger.LogInformation("No suitable products found for AOI.");

                    await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                    {
                        ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success    
                    }, context);
                    return;
                }

                var allQueue = await _sentinelDbContext.Set<Sentinel2ProductQueue>().ToListAsync();

                var existsIds = allQueue
                    .Where(m => Guid.TryParse(m.ProductId, out var id) && productIds.Contains(id))
                    .Select(m => m.ProductId)
                    .ToList();

                var newProducts = new List<string>();
                foreach (var item in productIds.Where(m => !existsIds.Contains(m.ToString())))
                {
                    var tempPQ = new Sentinel2ProductQueue()
                    {
                        CreateAt = DateTime.UtcNow,
                        Id = Guid.NewGuid(),
                        ProductId = item.ToString(),
                        QueueStatus = ProductQueueStatusType.New,
                        CreateUser = Guid.Empty
                    };

                    await _sentinelDbContext.Set<Sentinel2ProductQueue>().AddAsync(tempPQ);
                    newProducts.Add(item.ToString());
                }

                await _sentinelDbContext.SaveChangesAsync();
                Result.NewProducts = 0;
                Result.PolygonJson = options.SatelliteSearchConfig.SentinelPolygon?.AsText() ?? string.Empty;

                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success
                }, context);
                return;
            }
            catch (Exception ex)
            {
                Result.Errors.Add(ex.Message);
                Result.NewProducts = 0;

                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed
                }, context);
            }

            _logger.LogInformation($"End Sentinel2SearchProductJob");
        }

    }

    public class Sentinel2SearchProductJobResult : BaseJobResult
    {
        public int NewProducts { get; set; }
        public string PolygonJson { get; set; }
    }
}
