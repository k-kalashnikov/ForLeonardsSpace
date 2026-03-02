//using Masofa.BusinessLogic.Services.BusinessLogicLogger;
//using Masofa.Common.Attributes;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Models.Satellite.Indices;
//using Masofa.DataAccess;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Quartz;

//namespace Masofa.Web.Monolith.Jobs.SatelliteIndices.Reports.Season
//{
//    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "OsaviSeasonReportJob", "Indices")]
//    public class OsaviSeasonReportJob : BaseJob<OsaviSeasonReportJobResult>, IJob
//    {
//        private MasofaSentinelDbContext MasofaSentinelDbContext { get; set; }
//        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
//        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
//        public OsaviSeasonReportJob(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger logger, MasofaSentinelDbContext masofaSentinelDbContext, MasofaCommonDbContext masofaCommonDbContext, MasofaIndicesDbContext masofaIndicesDbContext, MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, masofaCommonDbContext, identityDbContext)
//        {
//            MasofaSentinelDbContext = masofaSentinelDbContext;
//            MasofaCommonDbContext = masofaCommonDbContext;
//            MasofaIndicesDbContext = masofaIndicesDbContext;
//        }

//        public async Task Execute(IJobExecutionContext context)
//        {
//            Logger.LogInformation("OsaviSeasonReportJob started");

//            var productQueue = await MasofaSentinelDbContext.Set<Sentinel2ProductQueue>()
//                .Where(m => m.QueueStatus == ProductQueueStatusType.OrviReportSeason)
//                .ToListAsync();

//            var needProducts = await MasofaCommonDbContext.SatelliteProducts
//                .Where(m => productQueue.Select(m => m.ProductId).Contains(m.ProductId))
//                .Where(m => m.MediadataPath != Guid.Empty)
//                .Take(100)
//                .ToListAsync();

//            Logger.LogInformation($"Found {needProducts.Count} products to process.");

//            foreach (var product in needProducts)
//            {
//                try
//                {
//                    var indexes = await MasofaIndicesDbContext.OsaviPoints
//                        .Where(i => i.SatelliteProductId.ToString() == product.ProductId)
//                        .Where(i => i.SeasonId != null)
//                        .ToListAsync();

//                    if (indexes == null)
//                    {
//                        continue;
//                    }

//                    var indexesGroup = indexes.GroupBy(i => i.SeasonId);
//                    var models = new List<OsaviSeasonReport>();

//                    if (product.ProductId == null)
//                    {
//                        continue;
//                    }

//                    try
//                    {
//                        var sentinel2ProductEntity = await MasofaSentinelDbContext.Sentinel2Products
//                            .AsNoTracking()
//                            .FirstOrDefaultAsync(x => x.SatellateProductId == product.ProductId);
//                        var inspire = await MasofaSentinelDbContext.SentinelInspireMetadata
//                            .AsNoTracking()
//                            .FirstOrDefaultAsync(x => x.Id == sentinel2ProductEntity.SentinelInspireMetadataId);

//                        if (inspire != null)
//                        {
//                            product.OriginDate = inspire.DateStamp;
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Logger.LogWarning($"Can`t find inspireMetada for originalDate of satellite product with id={product.ProductId}. Exception: {ex.Message}, InnerException:{ex.InnerException}");
//                    }

//                    var originDate = DateOnly.FromDateTime(product.OriginDate.Value);

//                    foreach (var index in indexesGroup)
//                    {
//                        var avarage = index.Select(i => i.Value).Sum() / index.Select(i => i.Value).Count();

//                        var newModel = new OsaviSeasonReport()
//                        {
//                            SeasonId = index.Key.Value,
//                            DateOnly = originDate,
//                            Average = avarage,
//                            TotalMax = index.Select(i => i.Value).Max(),
//                            TotalMin = index.Select(i => i.Value).Min(),
//                        };

//                        models.Add(newModel);
//                    }

//                    await MasofaIndicesDbContext.OsaviSeasonReports.AddRangeAsync(models);
//                    await MasofaIndicesDbContext.SaveChangesAsync();

//                    var queueItem = productQueue.FirstOrDefault(p => p.ProductId == product.ProductId);
//                    if (queueItem != null)
//                    {
//                        queueItem.QueueStatus = ProductQueueStatusType.OsaviReportSeason;
//                        MasofaSentinelDbContext.Set<Sentinel2ProductQueue>().Update(queueItem);
//                        await MasofaSentinelDbContext.SaveChangesAsync();
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Result.Errors.Add(ex.ToString());
//                }
//            }
//            try
//            {
//                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
//                {
//                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success
//                }, context);
//            }
//            catch (Exception ex)
//            {
//                Logger.LogError(ex, "Error in CreatePartitionJob");

//                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
//                {
//                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed
//                }, context);
//            }

//            Logger.LogInformation("OsaviSeasonReportJob ended");
//        }
//    }

//    public class OsaviSeasonReportJobResult : BaseJobResult
//    {
//        int count = 0;
//    }
//}
