//using Masofa.BusinessLogic.Services.BusinessLogicLogger;
//using Masofa.Common.Attributes;
//using Masofa.Common.Models.Satellite;
//using Masofa.Common.Models.Satellite.Indices;
//using Masofa.DataAccess;
//using Masofa.Web.Monolith.Jobs.SatelliteIndices.Reports.Season;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Quartz;

//namespace Masofa.Web.Monolith.Jobs.SatelliteIndices.Reports.Shared
//{
//    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "OrviSharedReportJob", "Indices")]
//    public class OrviSharedReportJob : BaseJob<OrviReportJobResult>, IJob
//    {
//        private MasofaSentinelDbContext MasofaSentinelDbContext { get; set; }
//        private MasofaCommonDbContext MasofaCommonDbContext { get; set; }
//        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }
//        private MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
//        public OrviSharedReportJob(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger<OrviSharedReportJob> logger, MasofaSentinelDbContext masofaSentinelDbContext, MasofaCommonDbContext masofaCommonDbContext, MasofaIndicesDbContext masofaIndicesDbContext, MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, masofaCommonDbContext, identityDbContext)
//        {
//            MasofaSentinelDbContext = masofaSentinelDbContext;
//            MasofaCommonDbContext = masofaCommonDbContext;
//            MasofaIndicesDbContext = masofaIndicesDbContext;
//            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
//        }

//        public async Task Execute(IJobExecutionContext context)
//        {
//            Logger.LogInformation("OrviSharedReportJob started");

//            try
//            {
//                var queueItems = await MasofaSentinelDbContext.Set<Sentinel2ProductQueue>()
//                    .Where(q => q.QueueStatus == ProductQueueStatusType.NdviReportSeason)
//                    .Take(100)
//                    .ToListAsync();

//                if (!queueItems.Any())
//                {
//                    Logger.LogInformation("No items in queue.");
//                    await MarkJobAsSuccess(context);
//                    return;
//                }

//                var sharedReports = new List<OrviSharedReport>();

//                foreach (var queueItem in queueItems)
//                {
//                    try
//                    {
//                        var points = await MasofaIndicesDbContext.OrviPoints
//                            .Where(p => p.SeasonId != null)
//                            .Where(p => p.RegionId != null)
//                            .Where(a => a.SatelliteProductId.ToString() == queueItem.ProductId)
//                            .ToListAsync();

//                        var seasonIds = points
//                            .Select(p => p.SeasonId.Value)
//                            .Distinct()
//                            .ToList();

//                        if (!seasonIds.Any())
//                        {
//                            continue;
//                        }

//                        var seasonReports = await MasofaIndicesDbContext.OrviSeasonReports
//                            .Where(r => seasonIds.Contains(r.SeasonId))
//                            .ToListAsync();

//                        if (!seasonReports.Any())
//                        {
//                            continue;
//                        }

//                        var seasons = await MasofaCropMonitoringDbContext.Seasons
//                            .Where(s => seasonIds.Contains(s.Id))
//                            .ToListAsync();

//                        var cropIds = seasons
//                            .Where(s => s.CropId != null)
//                            .Select(s => s.CropId)
//                            .ToList();

//                        foreach (var seasonReport in seasonReports.GroupBy(s => s.SeasonId))
//                        {
//                            if (!seasonReport.Any())
//                            {
//                                continue;
//                            }

//                            if (seasonReport.Key == null)
//                            {
//                                continue;
//                            }

//                            var seasonRegion = seasonReport.FirstOrDefault(s => s.RegionId != null);

//                            if (seasonRegion == null)
//                            {
//                                continue;
//                            }

//                            var orviSharedReport = new OrviSharedReport()
//                            {
//                                RegionId = seasonRegion.RegionId,
//                                CropId = seasons.First(s => s.Id == seasonRegion.SeasonId).CropId,
//                                DateOnly = seasonRegion.DateOnly,
//                                Average = seasonReport.Select(s => s.Average).Average(),
//                                AverageMax = seasonReport.Select(s => s.TotalMax).Average(),
//                                AverageMin = seasonReport.Select(s => s.TotalMin).Average(),
//                                TotalMax = seasonReport.Select(s => s.TotalMax).Max(),
//                                TotalMin = seasonReport.Select(s => s.TotalMax).Min()
//                            };

//                            sharedReports.Add(orviSharedReport);
//                        }

//                        queueItem.QueueStatus = ProductQueueStatusType.OrviReportShared;
//                    }
//                    catch (Exception ex)
//                    {
//                        Logger.LogError(ex, "Error processing product {ProductId}", queueItem.ProductId);
//                    }
//                }

//                if (sharedReports.Any())
//                {
//                    await MasofaIndicesDbContext.OrviSharedReports.AddRangeAsync(sharedReports);
//                    await MasofaIndicesDbContext.SaveChangesAsync();
//                }

//                MasofaSentinelDbContext.UpdateRange(queueItems);
//                await MasofaSentinelDbContext.SaveChangesAsync();

//                await MarkJobAsSuccess(context);
//            }
//            catch (Exception ex)
//            {
//                Logger.LogError(ex, "Critical error in OrviSharedReportJob");
//                await MarkJobAsFailed(context);
//            }

//            Logger.LogInformation("OrviSharedReportJob ended");
//        }


//        private async Task MarkJobAsSuccess(IJobExecutionContext context)
//        {
//            await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult
//            {
//                ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success
//            }, context);
//        }

//        private async Task MarkJobAsFailed(IJobExecutionContext context)
//        {
//            await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult
//            {
//                ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed
//            }, context);
//        }
//    }

//    public class OrviReportJobResult : BaseJobResult
//    {
//        int count = 0;
//    }
//}
