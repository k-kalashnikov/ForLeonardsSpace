using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Masofa.Web.Monolith.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Masofa.Web.Monolith.Jobs
{
    public abstract class BaseJob<TResult>
        where TResult : BaseJobResult
    {

        protected IMediator Mediator { get; set; }
        protected TResult Result { get; set; }
        protected IBusinessLogicLogger BusinessLogicLogger { get; set; }
        protected ILogger Logger { get; set; }
        protected MasofaCommonDbContext CommonDbContext { get; set; }
        protected MasofaIdentityDbContext IdentityDbContext { get; set; }

        public BaseJob(IMediator mediator, IBusinessLogicLogger businessLogicLogger, ILogger logger, MasofaCommonDbContext commonDbContext, MasofaIdentityDbContext identityDbContext)
        {
            Mediator = mediator;
            BusinessLogicLogger = businessLogicLogger;
            Logger = logger;
            Result = Activator.CreateInstance<TResult>();
            CommonDbContext = commonDbContext;
            IdentityDbContext = identityDbContext;
        }

        protected async Task SaveResult(SystemBackgroundTaskResult taskResult, IJobExecutionContext context)
        {
            try
            {
                var lastUpdateUser = await IdentityDbContext.Set<User>().FirstAsync(m => m.UserName.ToLower().Equals("admin"));

                var taskId = context.JobDetail?.JobDataMap["SystemTaskId"]?.ToString();
                if (string.IsNullOrEmpty(taskId))
                {
                    var currentTask = await CommonDbContext.SystemBackgroundTasks.Where(sbt => sbt.ExecuteTypeName == GetType().FullName).ToListAsync();

                    if ((currentTask == null) || (!currentTask.Any()))
                    {
                        var newSystemBackgroundTask = new SystemBackgroundTask()
                        {
                            CreateAt = DateTime.UtcNow,
                            CreateUser = lastUpdateUser.Id,
                            LastUpdateUser = lastUpdateUser.Id,
                            LastUpdateAt = DateTime.UtcNow,
                            Status = StatusType.Active,
                            CurrentRetryCount = 0,
                            IsActive = true,
                            ExecuteTypeName = GetType().FullName,
                            IsRetryable = true,
                            TaskType = SystemBackgroundTaskType.Schedule,
                            ParametrsJson = Newtonsoft.Json.JsonConvert.SerializeObject(new ScheduleTaskOptions()
                            {
                                StartDelaySeconds = 30,
                                GroupName = context.JobDetail.Key.Group,
                                Type = ScheduleType.Interval,
                                Interval = ScheduleInterval.Hours,
                                Frequency = 24
                            }),
                            Names = BackgroundTaskLocalization.GetLocalization(GetType().Name)
                        };

                        await CommonDbContext.SystemBackgroundTasks.AddAsync(newSystemBackgroundTask);
                        await CommonDbContext.SaveChangesAsync();

                        taskResult.SystemBackgroundTaskId = newSystemBackgroundTask.Id;
                    }
                    else
                    {
                        taskResult.SystemBackgroundTaskId = currentTask.First().Id;
                    }
                }
                else
                {
                    taskResult.SystemBackgroundTaskId = Guid.Parse(taskId);
                }
                taskResult.TaskResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(Result);
                taskResult.TaskResultJsonTypeName = typeof(TResult).FullName;

                taskResult.CreateAt = DateTime.UtcNow;
                taskResult.CreateUser = lastUpdateUser.Id;
                taskResult.LastUpdateUser = lastUpdateUser.Id;
                taskResult.LastUpdateAt = DateTime.UtcNow;
                taskResult.Status = StatusType.Active;

                await CommonDbContext.SystemBackgroundTaskResults.AddAsync(taskResult);
                await CommonDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await BusinessLogicLogger.LogCriticalAsync($"", $"{GetType().FullName}=>{nameof(SaveResult)}");
                Logger.LogCritical($"{ex.Message} in {GetType().FullName}=>{nameof(SaveResult)}");
            }
        }
    }

    public class BaseJobResult
    {
        public List<string> Errors { get; set; } = new List<string>();
    }
}
