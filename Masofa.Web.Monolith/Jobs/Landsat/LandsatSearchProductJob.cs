using Masofa.BusinessLogic.FieldSatellite.Requests;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Client.EarthExplorer;
using Masofa.Common.Attributes;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Services;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System.Text.Json;

namespace Masofa.Web.Monolith.Jobs.Landsat
{
    [SystemBackgroundTask(BackgroundTaskType.NotCritical, "LandsatSearchProductJob", "Landsat")]
    public class LandsatSearchProductJob : IJob
    {
        private ILogger<LandsatSearchProductJob> Logger { get; set; }
        private LandsatApiUnitOfWork UnitOfWork { get; set; }
        private MasofaLandsatDbContext LandsatDbContext { get; set; }
        private IOptions<LandsatServiceOptions> Options { get; set; }

        public LandsatSearchProductJob(
            ILogger<LandsatSearchProductJob> logger,
            LandsatApiUnitOfWork unitOfWork,
            MasofaLandsatDbContext landsatDbContext,
            MasofaCommonDbContext commonDbContext,
            IOptions<LandsatServiceOptions> options,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger)
        {
            Logger = logger;
            UnitOfWork = unitOfWork;
            LandsatDbContext = landsatDbContext;
            Options = options;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation("Start LandsatSearchProductJob");

            try
            {
                var options = Options.Value;

                if (!UnitOfWork.IsAuthed)
                {
                    await UnitOfWork.LoginAsync(options);
                }

                DateTime startDate = new DateTime(2025, 9, 30);
                DateTime endDate = new DateTime(2025, 10, 3);

                var productIds = await UnitOfWork.ProductRepository
                    .SearchProductsAsync(options, startDate, endDate);

                var existingIds = await LandsatDbContext.Set<LandsatProductQueue>()
                    .Select(x => x.ProductId)
                    .ToListAsync();

                var newIds = productIds.Except(existingIds).ToList();

                foreach (var id in newIds)
                {
                    var queueItem = new LandsatProductQueue
                    {
                        Id = Guid.NewGuid(),
                        ProductId = id,
                        Status = ProductQueueStatusType.New,
                        CreateAt = DateTime.UtcNow
                    };
                    await LandsatDbContext.Set<LandsatProductQueue>().AddAsync(queueItem);
                }

                await LandsatDbContext.SaveChangesAsync();

                // Джоба теперь просто выполняет поиск и сохраняет результат
                // Больше не запускает следующие джобы в цепочке

                //await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                //{
                //    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success,
                //    TaskResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
                //    {
                //        totalFound = productIds.Count,
                //        newProducts = newIds.Count,
                //        existingProducts = existingIds.Count,
                //        newProductIds = newIds
                //    }),
                //    TaskResultJsonType = typeof(string)
                //}, context);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
                Logger.LogCritical(ex.InnerException?.Message);
                //await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                //{
                //    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed,
                //    TaskResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
                //    {
                //        error = ex.Message,
                //        innerError = ex.InnerException?.Message
                //    }),
                //    TaskResultJsonType = typeof(string)
                //}, context);
            }

            Logger.LogInformation("End LandsatSearchProductJob");
        }

    }
}
