using Masofa.BusinessLogic.Services;
using Quartz;
using Microsoft.Extensions.Logging;

namespace Masofa.Web.Monolith.Services
{
    /// <summary>
    /// Реализация сервиса для запуска поиска Landsat продуктов по полям
    /// </summary>
    public class FieldSatelliteLandsatSearchService /*: IFieldSatelliteLandsatSearchService*/
    {
        //private readonly ISchedulerFactory _schedulerFactory;
        //private readonly ILogger<FieldSatelliteLandsatSearchService> _logger;

        //public FieldSatelliteLandsatSearchService(
        //    ISchedulerFactory schedulerFactory,
        //    ILogger<FieldSatelliteLandsatSearchService> logger)
        //{
        //    _schedulerFactory = schedulerFactory;
        //    _logger = logger;
        //}

        public async Task StartLandsatSearchJob(Guid fieldId, DateOnly startAt, DateOnly finishAt, Guid taskId, CancellationToken cancellationToken)
        {
            //try
            //{
            //    var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            //    var jobKey = new JobKey($"LandsatSearch_{taskId}", "FieldSatelliteSearch");
            //    var triggerKey = new TriggerKey($"LandsatSearchTrigger_{taskId}", "FieldSatelliteSearch");

            //    var jobData = new JobDataMap
            //    {
            //        ["FieldId"] = fieldId,
            //        ["StartAt"] = startAt.ToString("yyyy-MM-dd"),
            //        ["FinishAt"] = finishAt.ToString("yyyy-MM-dd"),
            //        ["TaskId"] = taskId
            //    };

            //    var job = JobBuilder.Create<LandsatFieldSearchJob>()
            //        .WithIdentity(jobKey)
            //        .UsingJobData(jobData)
            //        .Build();

            //    var trigger = TriggerBuilder.Create()
            //        .WithIdentity(triggerKey)
            //        .StartNow()
            //        .Build();

            //    await scheduler.ScheduleJob(job, trigger, cancellationToken);

            //    _logger.LogInformation("Scheduled Landsat search job {JobKey} for task {TaskId}", jobKey, taskId);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Failed to schedule Landsat search job for task {TaskId}", taskId);
            //    throw;
            //}
        }
    }
}
