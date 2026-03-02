//using Masofa.Common.Attributes;
//using Masofa.Common.Models;
//using Masofa.Common.Models.SystemCrical;
//using Masofa.DataAccess;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Npgsql;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Reflection;

//namespace Masofa.Cli.DevopsUtil.Commands.Satellite
//{
//    public class LandsatCreatePartitionCommandParameters
//    {
//        [TaskParameter("Дата начала периода для создания партиций", true, "2024-01-01")]
//        public DateTime StartDate { get; set; }
        
//        [TaskParameter("Дата окончания периода для создания партиций", true, "2024-12-31")]
//        public DateTime EndDate { get; set; }

//        public static LandsatCreatePartitionCommandParameters Parse(string[] args)
//        {
//            if (args.Length < 2 || !DateTime.TryParse(args[0], out var startDate) || !DateTime.TryParse(args[1], out var endDate))
//                throw new ArgumentException("Неверные параметры. Используйте: <startDate> <endDate> (формат: yyyy-MM-dd)");

//            if (startDate >= endDate)
//                throw new ArgumentException("Дата начала должна быть меньше даты конца");

//            return new LandsatCreatePartitionCommandParameters { StartDate = startDate, EndDate = endDate };
//        }

//        public static LandsatCreatePartitionCommandParameters GetFromUser()
//        {
//            Console.WriteLine("Введите дату начала периода (формат: yyyy-MM-dd):");
//            var startDateInput = Console.ReadLine();
            
//            Console.WriteLine("Введите дату конца периода (формат: yyyy-MM-dd):");
//            var endDateInput = Console.ReadLine();

//            if (!DateTime.TryParse(startDateInput, out var startDate) || !DateTime.TryParse(endDateInput, out var endDate))
//                throw new ArgumentException("Неверный формат даты. Используйте формат yyyy-MM-dd");

//            if (startDate >= endDate)
//                throw new ArgumentException("Дата начала должна быть меньше даты конца");

//            return new LandsatCreatePartitionCommandParameters { StartDate = startDate, EndDate = endDate };
//        }
//    }

//    /// <summary>
//    /// Job для создания партиций таблиц для моделей спутников Landsat
//    /// </summary>
//    /// 
//    [BaseCommand("Landsat Create Partition", "Создание партиций для таблиц Landsat", typeof(LandsatCreatePartitionCommandParameters))]
//    public class LandsatCreatePartitionCommand : IBaseCommand
//    {
//        private IConfiguration Configuration { get; set; }
//        private ILogger<LandsatCreatePartitionCommand> Logger { get; set; }
//        private MasofaLandsatDbContext MasofaLandsatDbContext { get; set; }

//        public LandsatCreatePartitionCommand(IConfiguration configuration, ILogger<LandsatCreatePartitionCommand> logger, MasofaLandsatDbContext masofaLandsatDbContext)
//        {
//            Configuration = configuration;
//            Logger = logger;
//            MasofaLandsatDbContext = masofaLandsatDbContext;
//        }

//        public async Task Execute()
//        {
//            var parameters = LandsatCreatePartitionCommandParameters.GetFromUser();
//            await ExecuteCore(parameters);
//        }

//        public async Task Execute(string[] args)
//        {
//            var parameters = LandsatCreatePartitionCommandParameters.Parse(args);
//            await ExecuteCore(parameters);
//        }

//        private async Task ExecuteCore(LandsatCreatePartitionCommandParameters parameters)
//        {
//            var partitionedTableNames = Assembly.Load("Masofa.Common").GetTypes()
//                .Where(t => t.GetCustomAttribute<PartitionedTableAttribute>() != null && t.Namespace != null && t.Namespace.Contains("Landsat"))
//                .Select(x => x.GetCustomAttribute<TableAttribute>()?.Name ?? x.Name)
//                .ToList();

//            var currentDate = parameters.StartDate.Date;
//            while (currentDate < parameters.EndDate.Date)
//            {
//                foreach (var tableName in partitionedTableNames)
//                {
//                    await CreatePartitionForModel(tableName, currentDate);
//                }
//                currentDate = currentDate.AddDays(1);
//            }
//        }

//        private async Task CreatePartitionForModel(string modelName, DateTime date)
//        {
//            var tableName = $"{modelName}_{date:yyyy_MM_dd}";
//            var startDate = date.ToString("yyyy-MM-dd");
//            var endDate = date.AddDays(1).ToString("yyyy-MM-dd");

//            if (MasofaLandsatDbContext.IsTableExists(tableName))
//            {
//                return;
//            }

//            var sql = $"CREATE TABLE \"{tableName}\" PARTITION OF \"{modelName}\" FOR VALUES FROM ('{startDate}') TO ('{endDate}');";

//            try
//            {
//                await MasofaLandsatDbContext.Database.ExecuteSqlRawAsync(sql);
//                Logger.LogInformation("Создана партиция: {TableName}", tableName);
//            }
//            catch (Exception ex)
//            {
//                Logger.LogError(ex, "Ошибка при создании партиции {TableName}", tableName);
//            }
//        }

//        public void Dispose()
//        {
//        }


//    }
//} 