//using Masofa.BusinessLogic.Index;
//using Masofa.BusinessLogic.Services.BusinessLogicLogger;
//using Masofa.Common.Attributes;
//using Masofa.Common.Models;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Models.Satellite.Indices;
//using Masofa.Common.Models.Satellite.Sentinel;
//using Masofa.DataAccess;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json.Linq;
//using Quartz;
//using System.Data;

//namespace Masofa.Web.Monolith.Jobs.SatelliteIndices.Reports.Season
//{
//    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "ArviSeasonReportJob", "Indices")]
//    public class ArviSeasonReportJob : BaseJob<ArviSeasonReportJobResult>, IJob
//    {
//        private MasofaSentinelDbContext MasofaSentinelDbContext {  get; set; }
//        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
//        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
//        public ArviSeasonReportJob(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger<ArviSeasonReportJob> logger, MasofaSentinelDbContext masofaSentinelDbContext, MasofaCommonDbContext masofaCommonDbContext, MasofaIndicesDbContext masofaIndicesDbContext, MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, masofaCommonDbContext, identityDbContext)
//        {
//            MasofaSentinelDbContext = masofaSentinelDbContext;
//            MasofaCommonDbContext = masofaCommonDbContext;
//            MasofaIndicesDbContext = masofaIndicesDbContext;
//        }

//        public async Task Execute(IJobExecutionContext context)
//        {
//            Logger.LogInformation("ArviSeasonReportJob started");

//            var productQueue = await MasofaSentinelDbContext.Set<Sentinel2ProductQueue>()
//                .Where(m => m.QueueStatus == ProductQueueStatusType.GeoserverImportedIndex)
//                .ToListAsync();

//            var needProducts = await MasofaCommonDbContext.SatelliteProducts
//                .Where(m => productQueue.Select(m => m.ProductId).Contains(m.ProductId))
//                .Where(m => m.MediadataPath != Guid.Empty)
//                .Take(100)
//                .ToListAsync();

//            Logger.LogInformation($"Found {needProducts.Count} products to process.");

//            foreach(var product in needProducts)
//            {
//                try
//                {
//                    var indexes = await MasofaIndicesDbContext.ArviPoints
//                        .Where(i => i.SatelliteProductId.ToString() == product.ProductId)
//                        .Where(i => i.SeasonId != null)
//                        .ToListAsync();

//                    if (indexes == null)
//                    {
//                        continue;
//                    }
                    
//                    var indexesGroup = indexes.GroupBy(i => i.SeasonId);
//                    var models = new List<ArviSeasonReport>();
                    
//                    if(product.ProductId == null)
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
//                    catch(Exception ex)
//                    {
//                        Logger.LogWarning($"Can`t find inspireMetada for originalDate of satellite product with id={product.ProductId}. Exception: {ex.Message}, InnerException:{ex.InnerException}");
//                    }
                    
//                    var originDate = DateOnly.FromDateTime(product.OriginDate.Value);

//                    foreach (var index in indexesGroup)
//                    {
//                        var avarage = index.Select(i => i.Value).Sum() / index.Select(i => i.Value).Count();

//                        var newModel = new ArviSeasonReport()
//                        {
//                            SeasonId = index.Key.Value,
//                            DateOnly = originDate,
//                            Average = avarage,
//                            TotalMax = index.Select(i => i.Value).Max(),
//                            TotalMin = index.Select(i => i.Value).Min(),
//                        };

//                        models.Add(newModel);
//                    }

//                    await MasofaIndicesDbContext.ArviSeasonReports.AddRangeAsync(models);
//                    await MasofaIndicesDbContext.SaveChangesAsync();

//                    var queueItem = productQueue.FirstOrDefault(p => p.ProductId == product.ProductId);
//                    if (queueItem != null)
//                    {
//                        queueItem.QueueStatus = ProductQueueStatusType.ArvirReportSeason;
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

//            Logger.LogInformation("ArviSeasonReportJob ended");
//        }
//    }

//    public class ArviSeasonReportJobResult : BaseJobResult
//    {
//        int count = 0;
//    }
//}
