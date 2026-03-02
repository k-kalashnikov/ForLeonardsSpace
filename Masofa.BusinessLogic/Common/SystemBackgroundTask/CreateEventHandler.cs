using Masofa.Common.Resources;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Helper;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

namespace Masofa.BusinessLogic.Common.SystemBackgroundTask
{
    /// <summary>
    /// Обработчик событий создания SystemBackgroundTask
    /// </summary>
    public class CreateEventHandler : INotificationHandler<BaseCreateEvent<Masofa.Common.Models.SystemCrical.SystemBackgroundTask, MasofaCommonDbContext>>
    {
        private ILogger Logger { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }
        private Quartz.IScheduler Scheduler { get; set; }


        public CreateEventHandler(
            ILogger<CreateEventHandler> logger, IBusinessLogicLogger businessLogicLogger, ISchedulerFactory schedulerFactory)
        {
            Logger = logger;
            BusinessLogicLogger = businessLogicLogger;
            Scheduler = schedulerFactory.GetScheduler().Result;
        }

        public async Task Handle(BaseCreateEvent<Masofa.Common.Models.SystemCrical.SystemBackgroundTask, MasofaCommonDbContext> notification, CancellationToken cancellationToken)
        {
            try
            {
                try
                {
                    switch (notification.Model.TaskType)
                    {
                        case SystemBackgroundTaskType.Schedule:
                            await CreateScheduleTask(notification.Model);
                            break;
                        case SystemBackgroundTaskType.Command:
                            await CreateCommandTask(notification.Model);
                            break;
                        case SystemBackgroundTaskType.Condition:
                            await CreateConditionTask(notification.Model);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw ex; //TODO ADD LOGGER AND REMOVE THROW
                }
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.SystemBackgroundTaskStarted(notification.Model.Id.ToString(), notification.Model.ExecuteTypeName), $"{nameof(Masofa.BusinessLogic.Common.SystemBackgroundTask)} => {nameof(CreateEventHandler)}");

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ошибка при обработке события создания SystemBackgroundTask: {TaskId}", notification.Model.Id);
            }
        }

        private async Task CreateScheduleTask(Masofa.Common.Models.SystemCrical.SystemBackgroundTask systemBackgroundTask)
        {
            var jobType = systemBackgroundTask.ExecuteType;

            if (jobType == null || !typeof(IJob).IsAssignableFrom(jobType))
            {
                throw new InvalidOperationException($"Тип {systemBackgroundTask.ExecuteTypeName} не является допустимым IJob");
            }

            var options = JsonConvert.DeserializeObject<ScheduleTaskOptions>(systemBackgroundTask.TaskOptionJson)
                ?? throw new InvalidOperationException($"TaskOptionJson cannot be null");

            var groupName = options.GroupName ?? "DEFAULT";

            var jobKey = new JobKey($"{systemBackgroundTask.ExecuteType?.Name}_{systemBackgroundTask.Id}", groupName);

            var jobDetail = JobBuilder.Create(jobType)
                .WithIdentity(jobKey)
                .UsingJobData("UserId", systemBackgroundTask.CreateUser)
                .UsingJobData("SystemTaskId", systemBackgroundTask.Id)
                .Build();

            // Создаем триггер
            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity($"trigger-{systemBackgroundTask.Id}-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm")}", groupName)
                .WithCronSchedule(ScheduleOptionsToCronHelper.ToCronExpression(options));

            if (options.StartDelaySeconds > 0)
            {
                triggerBuilder = triggerBuilder.StartAt(DateTimeOffset.Now.AddSeconds(options.StartDelaySeconds));
            }

            var trigger = triggerBuilder.Build();

            // Регистрируем в Quartz
            await Scheduler.ScheduleJob(jobDetail, trigger);
        }

        private async Task CreateCommandTask(Masofa.Common.Models.SystemCrical.SystemBackgroundTask systemBackgroundTask)
        {
            throw new NotImplementedException("Command task not implemented");
        }

        private async Task CreateConditionTask(Masofa.Common.Models.SystemCrical.SystemBackgroundTask systemBackgroundTask)
        {
            throw new NotImplementedException("Condition task not implemented");
        }
    }
}
