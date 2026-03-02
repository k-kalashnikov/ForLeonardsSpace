using Masofa.Common.Attributes;
using Masofa.Common.Models;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Masofa.Cli.DevopsUtil.Commands.Satellite
{
    public class SentinelCreatePartitionCommandParameters
    {
        [TaskParameter("Дата начала периода для создания партиций", true, "2021-01-01")]
        public DateTime StartDate { get; set; }

        [TaskParameter("Дата окончания периода для создания партиций", true, "2025-10-25")]
        public DateTime EndDate { get; set; }

        public static SentinelCreatePartitionCommandParameters Parse(string[] args)
        {
            if (args.Length < 2 || !DateTime.TryParse(args[0], out var startDate) || !DateTime.TryParse(args[1], out var endDate))
                throw new ArgumentException("Неверные параметры. Используйте: <startDate> <endDate> (формат: yyyy-MM-dd)");

            if (startDate >= endDate)
                throw new ArgumentException("Дата начала должна быть меньше даты конца");

            return new SentinelCreatePartitionCommandParameters { StartDate = startDate, EndDate = endDate };
        }

        public static SentinelCreatePartitionCommandParameters GetFromUser()
        {
            Console.WriteLine("Введите дату начала периода (формат: yyyy-MM-dd):");
            var startDateInput = Console.ReadLine();

            Console.WriteLine("Введите дату конца периода (формат: yyyy-MM-dd):");
            var endDateInput = Console.ReadLine();

            if (!DateTime.TryParse(startDateInput, out var startDate) || !DateTime.TryParse(endDateInput, out var endDate))
                throw new ArgumentException("Неверный формат даты. Используйте формат yyyy-MM-dd");

            if (startDate >= endDate)
                throw new ArgumentException("Дата начала должна быть меньше даты конца");

            return new SentinelCreatePartitionCommandParameters { StartDate = startDate, EndDate = endDate };
        }
    }

    /// <summary>
    /// Job для создания партиций таблиц для моделей спутников Sentinel
    /// </summary>
    [BaseCommand("Sentinel Create Partition", "Создание партиций для таблиц Sentinel", typeof(SentinelCreatePartitionCommandParameters))]
    public class SentinelCreatePartitionCommand : IBaseCommand
    {
        private IConfiguration Configuration { get; set; }
        private ILogger<SentinelCreatePartitionCommand> Logger { get; set; }
        private MasofaSentinelDbContext MasofaSentinelDbContext { get; set; }
        private MasofaIndicesDbContext MasofaIndicesDbContext { get; set; }

        public SentinelCreatePartitionCommand(IConfiguration configuration, 
            ILogger<SentinelCreatePartitionCommand> logger, 
            MasofaSentinelDbContext masofaSentinelDbContext,
            MasofaIndicesDbContext masofaIndicesDb)
        {
            Configuration = configuration;
            Logger = logger;
            MasofaSentinelDbContext = masofaSentinelDbContext;
            MasofaIndicesDbContext = masofaIndicesDb;
        }

        public async Task Execute()
        {
            var parameters = SentinelCreatePartitionCommandParameters.GetFromUser();
            await ExecuteCore(parameters);
        }

        public async Task Execute(string[] args)
        {
            var parameters = SentinelCreatePartitionCommandParameters.Parse(args);
            await ExecuteCore(parameters);
        }

        private async Task ExecuteCore(SentinelCreatePartitionCommandParameters parameters)
        {
            var entryTypes = MasofaIndicesDbContext.Model.GetEntityTypes();
            foreach (var entityType in entryTypes)
            {
                var baseType = entityType.ClrType;
                if (baseType.GetProperty("SatelliteProductId") == null)
                {
                    continue;
                }

                for (var currentDate = parameters.StartDate; currentDate <= parameters.EndDate; currentDate = currentDate.AddDays(1))
                {
                    MasofaIndicesDbContext.CreatePartitionForDateAsync(baseType, DateOnly.FromDateTime(currentDate));

                }

            }
        }
            //// Список моделей Sentinel для которых нужно создать партиции
            //var partitionedTableNames = Assembly.Load("Masofa.Common").GetTypes()
            //    .Where(t => t.GetCustomAttribute<PartitionedTableAttribute>() != null && t.Namespace != null && t.Namespace.Contains("Sentinel"))
            //    .Select(x => x.GetCustomAttribute<TableAttribute>()?.Name ?? x.Name)
            //    .ToList();

            //// Создаем партиции для каждого дня в указанном периоде
            //var currentDate = parameters.StartDate.Date;
            //while (currentDate < parameters.EndDate.Date)
            //{
            //    foreach (var tableName in partitionedTableNames)
            //    {
            //        await CreatePartitionForModel(tableName, currentDate);
            //    }
            //    currentDate = currentDate.AddDays(1);
            //}
        //}

        private async Task CreatePartitionForModel(string modelName, DateTime date)
        {
            var tableName = $"{modelName}_{date:yyyy_MM_dd}";
            var startDate = date.ToString("yyyy-MM-dd");
            var endDate = date.AddDays(1).ToString("yyyy-MM-dd");

            if (MasofaSentinelDbContext.IsTableExists(tableName))
            {
                return;
            }

            var sql = $"CREATE TABLE \"{tableName}\" PARTITION OF \"{modelName}\" FOR VALUES FROM ('{startDate}') TO ('{endDate}');";


            try
            {
                await MasofaSentinelDbContext.Database.ExecuteSqlRawAsync(sql);
                Logger.LogInformation("Создана партиция: {TableName}", tableName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ошибка при создании партиции {TableName}", tableName);
            }
        }
        public void Dispose()
        {
        }
    }
}