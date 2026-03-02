//using Azure;
//using Masofa.Cli.DevopsUtil.Converters;
//using Masofa.Common.Models;
//using Masofa.Common.Models.CropMonitoring;
//using Masofa.Common.Models.Identity;
//using Masofa.Common.Models.System;
//using Masofa.DataAccess;
//using Masofa.Depricated.DataAccess.DepricatedAuthServerOne;
//using Masofa.Depricated.DataAccess.DepricatedAuthServerTwo;
//using Masofa.Depricated.DataAccess.DepricatedUalertsServerOne;
//using Masofa.Depricated.DataAccess.DepricatedUdictServerTwo;
//using Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne;
//using Masofa.Depricated.DataAccess.DepricatedUmapiServerOne;
//using Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo;
//using Masofa.Depricated.DataAccess.DepricatedWeatherServerOne;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Minio.DataModel.Notification;
//using NetTopologySuite.Geometries;
//using NetTopologySuite.IO;
//using Newtonsoft.Json;
//using NodaTime;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Formats.Asn1;
//using System.Globalization;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Masofa.Cli.DevopsUtil.Commands.Migration
//{
//    [BaseCommand("Migrate Weather Import", "Импорт данных погоды из CSV формата")]
//    public class MigrateWeatherImportCsvCommand : IBaseCommand
//    {
//        private MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
//        private MasofaWeatherDbContext MasofaWeatherDbContext { get; set; }
//        private DepricatedWeatherServerOneDbContext DepricatedWeatherServerOneDbContext { get; set; }

//        private string BaseExportPath { get; set; }

//        public MigrateWeatherImportCsvCommand(MasofaWeatherDbContext masofaWeatherDbContext, DepricatedWeatherServerOneDbContext depricatedWeatherServerOneDbContext, MasofaDictionariesDbContext masofaDictionariesDbContext)
//        {

//            MasofaWeatherDbContext = masofaWeatherDbContext;
//            DepricatedWeatherServerOneDbContext = depricatedWeatherServerOneDbContext;
//            MasofaDictionariesDbContext = masofaDictionariesDbContext;
//        }

//        public async Task Execute(string[] args)
//        {
//            BaseExportPath = args[1];
//            Console.WriteLine(BaseExportPath);
//            await ImportWeatherTablesAsync();
//        }

//        public async Task Execute()
//        {

//            //Console.WriteLine($"Pls enter the current base path");
//            //BaseExportPath = Console.ReadLine();
//            //await CreateOldPartition();
//            await ImportWeatherTablesAsync();
//        }

//        public void Dispose()
//        {
//        }

//        #region MigrateWeather

//        private async Task ImportWeatherTablesAsync()
//        {
//            List<Type> skipTypes = new List<Type>()
//            {
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.AgroClimaticZone),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.AgroClimaticZoneMonthNorm),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.AgroClimaticZoneNorm),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.AgroClimaticZonesWeatherRate),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.Alert),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.AlertType),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.ApplicationProperty),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.Condition),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.Frequency),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.ImageType),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.Job),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.JobStatus),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.Log),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.Provider),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.Region),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.RegionsAgroClimaticZone),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.RegionsDump),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.RegionsWeather),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.RegionsWeather1),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.Report),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.ReportType),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.VAlertsFrozen),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.WeatherDatesCompleted),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.WeatherStation),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.WeatherStationAgroClimaticZone),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.WeatherStationsDataEx),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.WeatherType),
//                typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.XslsUzUnputColumn)
//            };
//            var entityTypes = typeof(Depricated.DataAccess.DepricatedWeatherServerOne.Models.AgroClimaticZone).Assembly
//                .GetTypes()
//                .Where(m => m.GetCustomAttribute<MigrationCompareAttributeWeather>() != null)
//                .ToList();

//            MasofaWeatherDbContext.ChangeTracker.AutoDetectChangesEnabled = false;
//            MasofaWeatherDbContext.ChangeTracker.LazyLoadingEnabled = false;

//            foreach (var entity in entityTypes)
//            {
//                if (skipTypes.Contains(entity))
//                {
//                    continue;
//                }
//                if (entity == typeof(Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models.WeatherStationsDataEx))
//                {
//                    continue;
//                }

//                var newType = entity.GetCustomAttribute<MigrationCompareAttributeWeather>()?.CompareToType;

//                MethodInfo oldSetMethod = typeof(DepricatedWeatherServerOneDbContext).GetMethods().First(m => m.Name.Equals("Set"));
//                MethodInfo oldGenericSetMethod = oldSetMethod.MakeGenericMethod(entity);
//                IQueryable<object> oldQuery = (IQueryable<object>)(oldGenericSetMethod.Invoke(DepricatedWeatherServerOneDbContext, null));


//                MethodInfo setMethod = entity.GetCustomAttribute<MigrationCompareAttributeWeather>().CompareToDbContext.GetMethods().First(m => m.Name.Equals("Set"));
//                MethodInfo genericSetMethod = oldSetMethod.MakeGenericMethod(newType);
//                DbContext newDbContext = entity.GetCustomAttribute<MigrationCompareAttributeWeather>().CompareToDbContext == typeof(MasofaDictionariesDbContext) ? MasofaDictionariesDbContext : MasofaWeatherDbContext;

//                IQueryable<object> query = (IQueryable<object>)(oldGenericSetMethod.Invoke(newDbContext, null));

//                int pageSize = 200000;
//                int offset = 0;
//                bool hasMore = true;
//                int currentDirectory = 0;

//                try
//                {
//                    while (hasMore) 
//                    {
//                        var page = await oldQuery
//                        .AsNoTracking()
//                        .Skip(offset)
//                        .Take(pageSize)
//                        .ToListAsync();

//                        if (page.Count == 0)
//                            break;

//                        var dbSetMethod = typeof(DbContext).GetMethods()
//                            .First(m => m.Name == "Set" && m.IsGenericMethod && m.GetParameters().Length == 0);

//                        var dbSet = dbSetMethod.MakeGenericMethod(newType).Invoke(newDbContext, null);

//                        var addMethod = dbSet.GetType().GetMethod("Add");

//                        var newItems = page.Select(m =>
//                        {
//                            var convertMethod = typeof(WeatherConverter).GetMethods(BindingFlags.Public | BindingFlags.Static)
//                                .FirstOrDefault(m => m.Name == "Convert" && m.GetParameters()[0].ParameterType == entity);
//                            var newItem = convertMethod.Invoke(null, new[] { m });
//                            return newItem;
//                        });

//                        if (newDbContext is MasofaWeatherDbContext)
//                        {

//                            var hasPartitionAttr = newType.GetCustomAttribute<PartitionedTableAttribute>() != null;
//                            var dateProp = entity.GetProperty("Date");
//                            if (hasPartitionAttr && dateProp != null)
//                            {
//                                var dates = page.Select(m =>
//                                {
//                                    var dateValue = dateProp.GetValue(m);
//                                    DateTime date = ResolveAnyDate(dateValue);
//                                    var dateOnly = DateOnly.FromDateTime(date);
//                                    return dateOnly;
//                                }).Distinct().ToList();

//                                foreach (var item in dates)
//                                {
//                                    var tableNameBase = newType.GetCustomAttribute<TableAttribute>()?.Name ?? newType.Name;
//                                    var partitionTableName = $"{tableNameBase}_{item:yyyy_MM_dd}";
//                                    if (!MasofaWeatherDbContext.IsTableExists(partitionTableName))
//                                    {
//                                        await CreatePartitionForModel(tableNameBase, item);
//                                        Console.WriteLine($"Partition added: {partitionTableName}");
//                                    }
//                                }
//                            }
//                        }
//                        if (newType == typeof(Masofa.Common.Models.Weather.WeatherStationsDatum))
//                        {
//                            var tempItems = newItems.Select(m => (Masofa.Common.Models.Weather.WeatherStationsDatum?)m).ToList();
//                            var sqlStrs = WeatherModelToSqlStringConverter.Convert(tempItems);
//                            foreach (var sqlStr in sqlStrs)
//                            {
//                                var tableNameBase = newType.GetCustomAttribute<TableAttribute>()?.Name ?? newType.Name;
//                                var partitionTableName = $"{tableNameBase}_{((DateTime)(sqlStr.Key)):yyyy_MM_dd}";
//                                await newDbContext.Database.ExecuteSqlRawAsync(sqlStr.Value.Item1);
//                                Console.WriteLine($"Insert in to {partitionTableName} {sqlStr.Value.Item2} records");
//                            }
//                        }
//                        else
//                        {
//                            newDbContext.AddRange(newItems);
//                            newDbContext.SaveChanges();
//                        }


//                        Console.WriteLine($"SUCCESS: {page.Count} записей из {entity.Name}");

//                        offset += pageSize;
//                        hasMore = page.Count == pageSize;
//                    }

//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"ERROR: {ex?.Message}");
//                    Console.WriteLine($"ERROR: {ex?.InnerException?.Message}");
//                }
//            }
//        }

//        private DateTime ResolveAnyDate(object value)
//        {
//            if (value is DateTime dt)
//                return dt.ToUniversalTime();

//            if (value is LocalDate localDate)
//                return ResolveDateTime(localDate);

//            if (value is LocalDateTime localDateTime)
//                return ResolveDateTime(localDateTime);

//            throw new InvalidOperationException($"Unsupported date type: {value?.GetType().Name ?? "null"}");
//        }

//        private DateTime ResolveDateTime(LocalDateTime localDateTime)
//        {
//            return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day)
//                 .AddHours(localDateTime.Hour)
//                 .AddMinutes(localDateTime.Minute)
//                 .ToUniversalTime();
//        }

//        private DateTime ResolveDateTime(LocalDate localDateTime)
//        {
//            return new DateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day).ToUniversalTime();
//        }

//        public async Task CreateOldPartition()
//        {
//            var startDate = new DateTime(2017, 1, 1);
//            var endDate = DateTime.Today.AddDays(1);

//            var partitionedTableNames = Assembly.Load("Masofa.Common").GetTypes()
//                .Where(t => t.GetCustomAttribute<PartitionedTableAttribute>() != null && t.Namespace != null && t.Namespace.Contains("Masofa.Common.Models.Weather"))
//                .Select(x => x.GetCustomAttribute<TableAttribute>()?.Name.ToLower() ?? x.Name.ToLower())
//                .ToList();

//            foreach (var tableName in partitionedTableNames)
//            {
//                if (tableName == "weatherstationsdataex")
//                {
//                    Console.WriteLine("SKIP: weather_stations_data_ex — нет подходящего поля для партиционирования");
//                    continue;
//                }

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
//                        CREATE TABLE ""{tableName}"" PARTITION OF ""{modelName}""
//                        FOR VALUES FROM (''{startDate}'') TO (''{endDate}'')
//                    ';
//                    RAISE NOTICE 'Создана партиция %', '{tableName}';
//                END IF;
//            END $$;";


//            try
//            {
//                await MasofaWeatherDbContext.Database.ExecuteSqlRawAsync(sql);
//                Console.WriteLine($"Создана партиция: {tableName}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"{ex.Message}");
//                Console.WriteLine($"Ошибка при создании партиции {tableName}");
//            }
//        }

//        private async Task CreatePartitionForModel(string modelName, DateOnly date)
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
//                        CREATE TABLE ""{tableName}"" PARTITION OF ""{modelName}""
//                        FOR VALUES FROM (''{startDate}'') TO (''{endDate}'')
//                    ';
//                    RAISE NOTICE 'Создана партиция %', '{tableName}';
//                END IF;
//            END $$;";


//            try
//            {
//                await MasofaWeatherDbContext.Database.ExecuteSqlRawAsync(sql);
//                Console.WriteLine($"Создана партиция: {tableName}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"{ex.Message}");
//                Console.WriteLine($"Ошибка при создании партиции {tableName}");
//            }
//        }

//        private string CreateSqlInsertString(Type objType, List<object> items)
//        {
//            var sb = new StringBuilder();
//            var props = objType.GetProperties();
//            foreach (var item in items)
//            {
//                sb.Append("(");
//                foreach (var itemProp in props)
//                {
//                    if (itemProp.Name == "Id")
//                    {
//                        continue;
//                    }
//                    if (props.Last() == itemProp)
//                    {
//                        sb.Append($"'{ResolveToSqlString(itemProp, item)}'");
//                        continue;
//                    }
//                    sb.Append($"'{ResolveToSqlString(itemProp, item)}',");
//                }
//                if (items.Last() == item)
//                {
//                    sb.Append(");");
//                    continue;
//                }
//                sb.Append("),");
//            }

//            return sb.ToString();
//        }

//        public string ResolveToSqlString(PropertyInfo prop, object item)
//        {
//            if (prop.PropertyType == typeof(DateTime))
//            {
//                return ((DateTime)(prop.GetValue(item))).ToString("yyyy-MM-dd HH:mm:ss zzz");

//            }
//            return prop.GetValue(item)?.ToString() ?? "NULL";

//        }
//        #endregion
//    }
//}
