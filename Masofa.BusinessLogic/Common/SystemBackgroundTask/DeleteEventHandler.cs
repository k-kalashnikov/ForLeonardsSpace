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

namespace Masofa.BusinessLogic.Common.SystemBackgroundTask
{
    /// <summary>
    /// Обработчик событий удаления SystemBackgroundTask
    /// </summary>
    public class DeleteEventHandler : INotificationHandler<BaseDeleteEvent<Masofa.Common.Models.SystemCrical.SystemBackgroundTask, MasofaCommonDbContext>>
    {
        private ILogger Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private IMediator Mediator { get; set; }
        private Quartz.IScheduler Scheduler { get; set; }

        public DeleteEventHandler(ILogger<DeleteEventHandler> logger, 
            IBusinessLogicLogger businessLogicLogger, 
            IMediator mediator,
            ISchedulerFactory schedulerFactory)
        {
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
            Mediator = mediator;
            Scheduler = schedulerFactory.GetScheduler().Result;
        }

        public async Task Handle(BaseDeleteEvent<Masofa.Common.Models.SystemCrical.SystemBackgroundTask, MasofaCommonDbContext> notification, CancellationToken cancellationToken)
        {
            try
            {
                try
                {
                    switch (notification.Model.TaskType)
                    {
                        case SystemBackgroundTaskType.Schedule:
                            await DeleteScheduleTask(notification.Model);
                            break;
                        case SystemBackgroundTaskType.Command:
                            await DeleteCommandTask(notification.Model);
                            break;
                        case SystemBackgroundTaskType.Condition:
                            await DeleteConditionTask(notification.Model);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw ex; //TODO ADD LOGGER AND REMOVE THROW
                }
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.SystemBackgroundTaskStopped(notification.Model.Id.ToString(), notification.Model.ExecuteTypeName), $"{nameof(Masofa.BusinessLogic.Common.SystemBackgroundTask)} => {nameof(DeleteEventHandler)}");

            }
            catch (Exception ex)
            {
                await BusinessLogicLogger.LogCriticalAsync(LogMessageResource.GenericError($"{nameof(Masofa.BusinessLogic.Common.SystemBackgroundTask)} => {nameof(DeleteEventHandler)}", ex.Message), $"{nameof(Masofa.BusinessLogic.Common.SystemBackgroundTask)} => {nameof(DeleteEventHandler)}");
                Logger.LogError(ex, LogMessageResource.GenericError($"{nameof(Masofa.BusinessLogic.Common.SystemBackgroundTask)} => {nameof(DeleteEventHandler)}", ex.Message));
            }
        }

        private async Task DeleteScheduleTask(Masofa.Common.Models.SystemCrical.SystemBackgroundTask systemBackgroundTask)
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

            var triggers = await Scheduler.GetTriggersOfJob(jobKey);
            if (triggers != null && triggers.Any())
            {
                var triggerKey = triggers.First().Key;
                await Scheduler.UnscheduleJob(triggerKey);
            }
            await Scheduler.Interrupt(jobKey);
        }

        private async Task DeleteCommandTask(Masofa.Common.Models.SystemCrical.SystemBackgroundTask systemBackgroundTask)
        {
            throw new NotImplementedException("Command task not implemented");
        }

        private async Task DeleteConditionTask(Masofa.Common.Models.SystemCrical.SystemBackgroundTask systemBackgroundTask)
        {
            throw new NotImplementedException("Condition task not implemented");
        }
    }
}
