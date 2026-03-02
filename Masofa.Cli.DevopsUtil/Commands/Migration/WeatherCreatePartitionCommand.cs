//using Masofa.Common.Attributes;
//using Masofa.Common.Models.SystemCrical;
//using Masofa.DataAccess;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Npgsql;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Commands.Migration
//{
//    [BaseCommand("Create Weather Partition Command", "Создание партиций для таблиц погоды")]
//    public class WeatherCreatePartitionCommand : IBaseCommand
//    {
//        private readonly MasofaWeatherDbContext MasofaWeatherDbContext;
//        public WeatherCreatePartitionCommand(MasofaWeatherDbContext masofaWeatherDbContext) 
//        {
//            MasofaWeatherDbContext = masofaWeatherDbContext;
//        }

//        public void Dispose()
//        {

//        }

//        public async Task Execute()
//        {
//            await CreatePartition();
//        }

//        public Task Execute(string[] args)
//        {
//            return Execute();
//        }

//        public async Task CreatePartition()
//        {
//            var startDate = new DateTime(2017, 1, 1);
//            var endDate = DateTime.Today.AddDays(1);

//            var partitionedTableNames = Assembly.Load("Masofa.Common").GetTypes()
//                .Where(t => t.GetCustomAttribute<PartitionedTableAttribute>() != null && t.Namespace != null && t.Namespace.Contains("Masofa.Common.Models.Weather"))
//                .Select(x => x.GetCustomAttribute<TableAttribute>()?.Name.ToLower() ?? x.Name.ToLower())
//                .ToList();

//            foreach (var tableName in partitionedTableNames)
//            {
//                for (var date = startDate; date <= endDate; date = date.AddDays(1))
//                {
//                    var tableDataName = $"{tableName}_{date:yyyy_MM_dd}";
//                    if (MasofaWeatherDbContext.IsTableExists(tableDataName))
//                    {
//                        continue;
//                    }
//                    await CreatePartitionForModel(tableName, date);
//                }
//            }
//        }

//        private async Task CreatePartitionForModel(string modelName, DateTime date)
//        {
//            var tableName = $"{modelName}_{date:yyyy_MM_dd}";
//            var startDate = date.ToString("yyyy-MM-dd");
//            var endDate = date.AddDays(1).ToString("yyyy-MM-dd");

//            var sql = $@"
//            DO $$
//            BEGIN
//                IF NOT EXISTS (
//                    SELECT FROM pg_tables
//                    WHERE tablename = '{tableName}'
//                ) THEN
//                    EXECUTE '
//                        CREATE TABLE {tableName} PARTITION OF {modelName}
//                        FOR VALUES FROM (''{startDate}'') TO (''{endDate}'')
//                    ';
//                    RAISE NOTICE 'Создана партиция %', '{tableName}';
//                END IF;
//            END $$;";


//            try
//            {
//                await MasofaWeatherDbContext.Database.ExecuteSqlRawAsync(sql);
//                Console.WriteLine("Создана партиция: {TableName}", tableName);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"{ex.Message}");
//                Console.WriteLine("Ошибка при создании партиции {TableName}", tableName);
//            }
//        }
//    }
//}
