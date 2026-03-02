using Masofa.Common.Resources;
using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Helper;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services;
using Masofa.DataAccess;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System.Reactive.Concurrency;

namespace Masofa.BusinessLogic.Common.SystemBackgroundTask
{
    /// <summary>
    /// Обработчик событий обновления SystemBackgroundTask
    /// </summary>
    public class UpdateEventHandler : INotificationHandler<BaseUpdateEvent<Masofa.Common.Models.SystemCrical.SystemBackgroundTask, MasofaCommonDbContext>>
    {
        private ILogger Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private IMediator Mediator { get; set; }
        private Quartz.IScheduler Scheduler { get; set; }


        public UpdateEventHandler(ILogger<UpdateEventHandler> logger, ISchedulerFactory schedulerFactory, IBusinessLogicLogger businessLogicLogger, IMediator mediator)
        {
            Logger = logger;
            Scheduler = schedulerFactory.GetScheduler().Result;
            BusinessLogicLogger = businessLogicLogger;
            Mediator = mediator;
        }

        public async Task Handle(BaseUpdateEvent<Masofa.Common.Models.SystemCrical.SystemBackgroundTask, MasofaCommonDbContext> notification, CancellationToken cancellationToken)
        {
            try
            {
                try
                {
                    switch (notification.CurrentModel.TaskType)
                    {
                        case SystemBackgroundTaskType.Schedule:
                            await UpdateScheduleTask(notification.CurrentModel);
                            break;
                        case SystemBackgroundTaskType.Command:
                            await UpdateCommandTask(notification.CurrentModel);
                            break;
                        case SystemBackgroundTaskType.Condition:
                            await UpdateConditionTask(notification.CurrentModel);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw ex; //TODO ADD LOGGER AND REMOVE THROW
                }
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.SystemBackgroundTaskStarted(notification.CurrentModel.Id.ToString(), notification.CurrentModel.ExecuteTypeName), $"{nameof(Masofa.BusinessLogic.Common.SystemBackgroundTask)} => {nameof(UpdateEventHandler)}");

            }
            catch (Exception ex)
            {
                await BusinessLogicLogger.LogCriticalAsync(LogMessageResource.GenericError($"{nameof(Masofa.BusinessLogic.Common.SystemBackgroundTask)} => {nameof(UpdateEventHandler)}", ex.Message), $"{nameof(Masofa.BusinessLogic.Common.SystemBackgroundTask)} => {nameof(UpdateEventHandler)}");
                Logger.LogError(ex, "Ошибка при обработке события обновления SystemBackgroundTask: {TaskId}", notification.CurrentModel.Id);
            }
        }

        private async Task UpdateScheduleTask(Masofa.Common.Models.SystemCrical.SystemBackgroundTask systemBackgroundTask)
        {
            var jobType = systemBackgroundTask.ExecuteType;

            if (jobType == null || !typeof(IJob).IsAssignableFrom(jobType))
            {
                throw new InvalidOperationException($"Тип {systemBackgroundTask.ExecuteTypeName} не является допустимым IJob");
            }

            var options = JsonConvert.DeserializeObject<ScheduleTaskOptions>(systemBackgroundTask.TaskOptionJson);

            var jobKey = new JobKey($"{systemBackgroundTask.ExecuteType.Name}_{systemBackgroundTask.Id.ToString()}", options.GroupName ?? "DEFAULT");

            var jobDetail = await Scheduler.GetJobDetail(jobKey);

            if (jobDetail == null)
            {
                throw new InvalidOperationException($"Задача с {jobKey} не найдета в Quartz.IScheduler");
            }

            if (!systemBackgroundTask.IsActive)
            {
                await Scheduler.Interrupt(jobKey);
                return;
            }

            var oldTriggerKey = (await Scheduler.GetTriggersOfJob(jobKey)).FirstOrDefault();


            // Создаем триггер
            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity($"trigger-{systemBackgroundTask.Id}-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm")}", options.GroupName ?? "DEFAULT")
                .WithCronSchedule(ScheduleOptionsToCronHelper.ToCronExpression(options));

            if (options.StartDelaySeconds > 0)
            {
                triggerBuilder = triggerBuilder.StartAt(DateTimeOffset.Now.AddSeconds(options.StartDelaySeconds));
            }
            var trigger = triggerBuilder.Build();

            if (oldTriggerKey == null)
            {
                await Scheduler.ScheduleJob(jobDetail, trigger);
            }
            else
            {
                await Scheduler.RescheduleJob(trigger.Key, oldTriggerKey);
            }
        }

        private async Task UpdateCommandTask(Masofa.Common.Models.SystemCrical.SystemBackgroundTask systemBackgroundTask)
        {
            throw new NotImplementedException("Command task not implemented");
        }

        private async Task UpdateConditionTask(Masofa.Common.Models.SystemCrical.SystemBackgroundTask systemBackgroundTask)
        {
            throw new NotImplementedException("Condition task not implemented");
        }
    }
}
