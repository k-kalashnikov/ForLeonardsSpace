using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Attributes;
using Masofa.Common.Models;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Masofa.Web.Monolith.Jobs.System
{
    /// <summary>
    /// Job для создания партиций таблиц для моделей System
    /// </summary>
    [SystemBackgroundTask(BackgroundTaskType.Critical, "CreatePartitionJob", "System")]
    public class CreatePartitionJob : BaseJob<CreatePartitionJobResult>, IJob
    {
        private IServiceProvider ServiceProvider { get; set; }

        public CreatePartitionJob(
            ILogger<CreatePartitionJob> logger,
            IServiceProvider serviceProvider,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger, 
            MasofaCommonDbContext commonDbContext, 
            MasofaIdentityDbContext identityDbContext) : base(mediator, businessLogicLogger, logger, commonDbContext, identityDbContext)
        {
            ServiceProvider = serviceProvider;
        }



        public async Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation("Start CreatePartitionJob");

            var tomorrow = DateTime.Today.AddDays(1);

            var partitionedTableTypes = typeof(PartitionedTableAttribute).Assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<PartitionedTableAttribute>() != null)
                .ToList();

            foreach (var tableType in partitionedTableTypes)
            {

                try
                {

                    var partitionedTableAttribute = tableType.GetCustomAttribute<PartitionedTableAttribute>();
                    if (partitionedTableAttribute == null)
                    {
                        continue;
                    }

                    var dbContextType = GetTableDbContextType(tableType);
                    if (dbContextType == null)
                    {
                        var warning = $"DbContext not found for type: {tableType.FullName}";
                        Logger.LogWarning(warning);
                        continue;
                    }

                    using (var scope = ServiceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetService(dbContextType) as DbContext;
                        if (dbContext == null)
                        {
                            var warning = $"DbContext instance not resolved for type: {dbContextType.FullName}";
                            Logger.LogWarning(warning);
                            continue;
                        }

                        for (var tempDay = DateTime.Today.AddDays(-5); tempDay <= DateTime.Today.AddDays(7); tempDay = tempDay.AddDays(1))
                        {
                            var tableName = GetTableName(tableType, dbContext);
                            var partitionName = await CreatePartitionForModel(tableName, tempDay, dbContext);
                            var dbName = dbContext.Database.GetDbConnection().Database;
                            if (!Result.TableCreated.ContainsKey(dbName))
                            {
                                Result.TableCreated.Add(dbContext.Database.GetDbConnection().Database, new List<string>());
                            }
                            if (string.IsNullOrEmpty(partitionName))
                            {
                                Result.TableCreated[dbName].Add(partitionName);
                            }
                            else
                            {
                                Result.Errors.Add($"{$"{tableType.Name}_{tempDay:yyyy_MM_dd}"} is not created");
                            }

                        }
                        dbContext.Dispose();
                    }


                }
                catch (Exception ex)
                {
                    Result.Errors.Add(ex.ToString());
                }
            }
            try
            {
                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Success
                }, context);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in CreatePartitionJob");

                await SaveResult(new Common.Models.SystemCrical.SystemBackgroundTaskResult()
                {
                    ResultType = Common.Models.SystemCrical.SystemBackgroundTaskResultType.Failed
                }, context);
            }


            Logger.LogInformation("End CreatePartitionJob");
        } 

        private async Task<string?> CreatePartitionForModel(string modelName, DateTime date, DbContext dbContext)
        {
            var tableName = $"{modelName}_{date:yyyy_MM_dd}";
            var startDate = date.ToString("yyyy-MM-dd");
            var endDate = date.AddDays(1).ToString("yyyy-MM-dd");

            if (dbContext.IsTableExists(tableName))
            {
                return null; // Партиция уже существует
            }

            var sql = $"CREATE TABLE \"{tableName}\" PARTITION OF \"{modelName}\" FOR VALUES FROM ('{startDate}') TO ('{endDate}');";

            try
            {
                await dbContext.Database.ExecuteSqlRawAsync(sql);
                Logger.LogInformation("Partition created: {TableName}", tableName);
                return tableName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Partition creation error {TableName}", tableName);
                return null;
            }
        }

        private string GetTableName(Type tableType, DbContext dbContext)
        {
            return dbContext.Model
                .FindEntityType(tableType)
                .GetTableName();
        }

        private Type? GetTableDbContextType(Type tableType)
        {
            var dbContextTypes = typeof(MasofaCommonDbContext).Assembly.GetTypes()
                .Where(m => m.IsSubclassOf(typeof(DbContext)));

            foreach (var item in dbContextTypes)
            {
                using (var scope = ServiceProvider.CreateScope())
                {
                    var dbContext = (scope.ServiceProvider.GetService(item) as DbContext);
                    if (dbContext == null)
                    {
                        continue;
                    }
                    if (dbContext.Model.FindEntityType(tableType) != null)
                    {
                        return item;
                    }
                }
            }

            return null;
        }
    }

    public class CreatePartitionJobResult : BaseJobResult
    {
        public Dictionary<string, List<string>> TableCreated { get; set; } = new Dictionary<string, List<string>>();
    }
}
