using Masofa.BusinessLogic.Services;
using Quartz;
using Microsoft.Extensions.Logging;

namespace Masofa.Web.Monolith.Services
{
    /// <summary>
    /// Реализация сервиса для запуска поиска Sentinel продуктов по полям
    /// </summary>
    public class FieldSatelliteSentinelSearchService /*: IFieldSatelliteSentinelSearchService*/
    {
        //private readonly ISchedulerFactory _schedulerFactory;
        //private readonly ILogger<FieldSatelliteSentinelSearchService> _logger;

        //public FieldSatelliteSentinelSearchService(
        //    ISchedulerFactory schedulerFactory,
        //    ILogger<FieldSatelliteSentinelSearchService> logger)
        //{
        //    _schedulerFactory = schedulerFactory;
        //    _logger = logger;
        //}

        public async Task StartSentinelSearchJob(Guid fieldId, DateOnly startAt, DateOnly finishAt, Guid taskId, CancellationToken cancellationToken)
        {
            //try
            //{
            //    var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            //    var jobKey = new JobKey($"SentinelSearch_{taskId}", "FieldSatelliteSearch");
            //    var triggerKey = new TriggerKey($"SentinelSearchTrigger_{taskId}", "FieldSatelliteSearch");

            //    var jobData = new JobDataMap
            //    {
            //        ["FieldId"] = fieldId,
            //        ["StartAt"] = startAt.ToString("yyyy-MM-dd"),
            //        ["FinishAt"] = finishAt.ToString("yyyy-MM-dd"),
            //        ["TaskId"] = taskId
            //    };

            //    var job = JobBuilder.Create<SentinelFieldSearchJob>()
            //        .WithIdentity(jobKey)
            //        .UsingJobData(jobData)
            //        .Build();

            //    var trigger = TriggerBuilder.Create()
            //        .WithIdentity(triggerKey)
            //        .StartNow()
            //        .Build();

            //    await scheduler.ScheduleJob(job, trigger, cancellationToken);

            //    _logger.LogInformation("Scheduled Sentinel search job {JobKey} for task {TaskId}", jobKey, taskId);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Failed to schedule Sentinel search job for task {TaskId}", taskId);
            //    throw;
            //}
        }
    }
}
