using DocumentFormat.OpenXml.InkML;
using Masofa.Common.Attributes;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Notifications;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.Satellite.Parse.Sentinel.Inspire;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System.Reflection;
using System.Text;

namespace Masofa.Cli.DevopsUtil.Commands.CodeGenerators
{
    [BaseCommand("SqlGeneratorCommand", "SqlGeneratorCommand")]
    public class SqlGeneratorCommand : IBaseCommand
    {
        private IServiceProvider ServiceProvider { get; set; }
        public SqlGeneratorCommand(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task Execute()
        {
            Console.WriteLine("Start CreatePartitionJob");

            var tomorrow = DateTime.Today.AddDays(1);

            var partitionedTableTypes = typeof(PartitionedTableAttribute).Assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<PartitionedTableAttribute>() != null)
                .ToList();

            var dbContexts = GetDbContexts();

            Dictionary<string, List<string>> sqlStrs = new Dictionary<string, List<string>>();

            foreach (var tableType in partitionedTableTypes)
            {
                try
                {

                    var partitionedTableAttribute = tableType.GetCustomAttribute<PartitionedTableAttribute>();
                    if (partitionedTableAttribute == null)
                    {
                        continue;
                    }

                    var dbContext = GetTableDbContextType(tableType, dbContexts);

                    if (dbContext == null)
                    {
                        var warning = $"DbContext instance not resolved for type: {tableType.Name}";
                        Console.WriteLine(warning);
                        continue;
                    }

                    if (!sqlStrs.ContainsKey(dbContext.GetType().Name))
                    {
                        sqlStrs.Add(dbContext.GetType().Name, new List<string>());
                    }

                    for (var tempDay = new DateTime(2025, 1, 1); tempDay <= new DateTime(2026, 2, 10); tempDay = tempDay.AddDays(1))
                    {
                        var tableName = GetTableName(tableType, dbContext);
                        var partitionName = await CreatePartitionForModel(tableName, tempDay, dbContext);
                        sqlStrs[dbContext.GetType().Name].Add(partitionName);

                        if (string.IsNullOrEmpty(partitionName))
                        {
                            Console.WriteLine($"{$"{tableType.Name}_{tempDay:yyyy_MM_dd}"} is created");
                        }
                        else
                        {
                            Console.WriteLine($"{$"{tableType.Name}_{tempDay:yyyy_MM_dd}"} is not created");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }

            foreach (var sqlStr in sqlStrs)
            {
                File.AppendAllLines($"D:\\Debug\\masofa\\masofa_backup\\{sqlStr.Key}.sql", sqlStr.Value);
            }

            Console.WriteLine("End CreatePartitionJob");
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }

        public string GenerateInsertScript(DbContext context)
        {
            var modelBuilder = context.Model;

            var entityTypes = modelBuilder.GetEntityTypes()
                .OrderByDescending(et => HasForeignKeyToOthers(modelBuilder, et)) // сначала "зависимые", потом "родительские"
                .ToList();

            while (true)
            {
                Console.WriteLine("Choose DbContext or enter \"exit\"");
                var index = 0;
                foreach (var entityType in entityTypes)
                {
                    Console.WriteLine($"{index} {entityType.ClrType.Name}");
                    index++;
                }
                var entityTypeNumber = Console.ReadLine();
                if (entityTypeNumber.ToLower() == "exit")
                {
                    break;
                }
                var choosenEntityType = entityTypes[int.Parse(entityTypeNumber)];
                return GenerateInsertScript(context, choosenEntityType);
            }

            return string.Empty;
        }

        public string GenerateInsertScript(DbContext context, IEntityType entityType)
        {
            var script = new StringBuilder();
            try
            {
                var tableName = entityType.GetTableName() ?? entityType.GetSchemaQualifiedTableName();
                // Получаем метод Set<T>()
                var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes);

                // Создаём конкретную версию Set<TEntityType>()
                var genericSetMethod = setMethod.MakeGenericMethod(entityType.ClrType);

                // Вызываем db.Set<TEntityType>()
                var dbSet = (IQueryable)genericSetMethod.Invoke(context, null);

                // Применяем AsNoTracking и ToList через статические методы EF
                var entities = ((IQueryable)typeof(EntityFrameworkQueryableExtensions)
                    .GetMethod(nameof(EntityFrameworkQueryableExtensions.AsNoTracking))
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(null, new object[] { dbSet }))
                    .Cast<object>()
                    .ToList() ?? new List<object>();

                if (!entities.Any())
                {
                    return script.ToString();
                }

                var properties = entityType.GetProperties()
                    .Where(p => !p.IsShadowProperty()) // пропускаем shadow-свойства (если не нужны)
                    .ToList();

                script.AppendLine($"-- Data for {tableName}");
                var errorLines = new Dictionary<int, object>();
                var index = 0;
                foreach (var entity in entities)
                {
                    try
                    {
                        var values = new List<string>();
                        foreach (var prop in properties)
                        {
                            var value = prop.PropertyInfo?.GetValue(entity);
                            var tempStrValue = FormatValue(value, prop.ClrType).Replace("\n", string.Empty);
                            values.Add(tempStrValue);
                        }

                        var columns = string.Join(", ", properties.Select(p => QuoteIdentifier(p.GetColumnName() ?? p.Name)));
                        var vals = string.Join(", ", values).Replace("\u000D", string.Empty);
                        script.AppendLine($"INSERT INTO {QuoteIdentifier(tableName)} ({columns}) VALUES ({vals});");
                    }
                    catch (Exception ex)
                    {
                        errorLines[index] = entity;
                    }

                    index++;
                }
                script.AppendLine();
                foreach (var error in errorLines)
                {
                    script.AppendLine($"-- Entity {entityType.ClrType.Name} with Id = {entityType.ClrType.GetProperty("Id").GetValue(entities[error.Key])} is broken");
                }
            }
            catch (Exception ex)
            {
                script.AppendLine(ex.Message);
            }
            return script.ToString();
        }

        public void GenerateInsertScriptAllEntities(DbContext context, string path)
        {
            var modelBuilder = context.Model;

            var entityTypes = modelBuilder.GetEntityTypes()
                .OrderByDescending(et => HasForeignKeyToOthers(modelBuilder, et)) // сначала "зависимые", потом "родительские"
                .ToList();

            foreach (var entityType in entityTypes)
            {
                if (DbContextBackUpExceptions.Entities.ContainsKey(context.GetType()))
                {
                    if (!DbContextBackUpExceptions.Entities[context.GetType()].Any())
                    {
                        Console.WriteLine($"Context {context.GetType().Name} is in the Exceptions");
                        continue;
                    }

                    if (DbContextBackUpExceptions.Entities[context.GetType()].Contains(entityType.ClrType))
                    {
                        Console.WriteLine($"Type {entityType.ClrType.Name} is in the Exceptions");
                        continue;
                    }
                }
                Console.WriteLine($"Start for {entityType.ClrType.Name}");
                var entityScriptLines = GenerateInsertScript(context, entityType)
                    .Split('\n');

                var contextDirectoryOutput = Path.Combine(path, context.GetType().Name);
                if (!Directory.Exists(contextDirectoryOutput))
                {
                    Directory.CreateDirectory(contextDirectoryOutput);
                }

                if (entityScriptLines.Length <= 100000)
                {
                    File.WriteAllLines(Path.Combine(contextDirectoryOutput, $"{entityType.ClrType.Name}.sql"), entityScriptLines);
                    continue;
                }

                var index = 0;
                while (true)
                {
                    var output = entityScriptLines
                        .Skip(index * 100000)
                        .Take(100000)
                        .ToArray();
                    if (!output.Any())
                    {
                        break;
                    }

                    File.WriteAllLines(Path.Combine(contextDirectoryOutput, $"{entityType.ClrType.Name}_{index}.sql"), output);
                    index++;
                }
                
            }

        }

        #region SupportMethods
        private bool HasForeignKeyToOthers(IModel model, IEntityType entityType)
        {
            // Проверяем, есть ли у этой сущности FK на другие сущности
            return entityType.GetForeignKeys().Any(fk => fk.PrincipalEntityType != entityType);
        }

        private string FormatValue(object? value, Type type)
        {
            if (value == null)
            {
                return "NULL";
            }
            return type switch
            {
                _ when type == typeof(DateTime)
                    => $"'{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}'",
                _ when type == typeof(DateOnly)
                    => $"'{((DateOnly)value).ToString("yyyy-MM-dd")}'",
                _ when type == typeof(TimeOnly)
                    => $"'{((TimeOnly)value).ToString("HH:mm:ss")}'",
                _ when type == typeof(string) || type == typeof(Guid)
                    => $"'{EscapeSqlString(value.ToString()!).Replace("\n", string.Empty)}'",
                _ when type == typeof(bool)
                    => ((bool)value) ? "true" : "false", // или TRUE/FALSE — зависит от СУБД
                _ when type == typeof(byte[]) // binary
                    => $"x'{BitConverter.ToString((byte[])value).Replace("-", "").Replace("\n", string.Empty)}'", // MySQL hex literal
                _ when IsNumericType(type)
                    => value.ToString()!,
                _ when type.IsEnum
                    => $"{((int)value).ToString()}",
                _ => $"'{EscapeSqlString(value.ToString()!).Replace("\n", string.Empty)}'" // fallback
            };
        }

        private string EscapeSqlString(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.Replace("\n", string.Empty).Replace("'", "''"); // стандартное экранирование для SQL
        }

        private string QuoteIdentifier(string identifier)
        {
            return $"\"{identifier.Replace("\"", "\"\"")}\"";
        }

        private bool IsNumericType(Type type)
        {
            return type == typeof(int) || type == typeof(long) || type == typeof(short) ||
                   type == typeof(byte) || type == typeof(uint) || type == typeof(ulong) ||
                   type == typeof(ushort) || type == typeof(sbyte) ||
                   type == typeof(decimal) || type == typeof(double) || type == typeof(float);
        }

        public List<DbContext> GetDbContexts()
        {
            var dbContextTypes = typeof(MasofaAnaliticReportDbContext).Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(DbContext)))
                .Where(t => !t.Name.Contains("History"))
                .ToList();

            var result = new List<DbContext>();

            foreach (var dbContextType in dbContextTypes)
            {
                try
                {
                    if (ServiceProvider.GetService(dbContextType) != null)
                    {
                        result.Add((DbContext)ServiceProvider.GetService(dbContextType));
                        continue;
                    }
                    var dbContextOption = typeof(DbContextOptions<>).MakeGenericType(dbContextType);
                    result.Add((DbContext)Activator.CreateInstance(dbContextType, dbContextOption));

                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }
        private string GetTableName(Type tableType, DbContext dbContext)
        {
            return dbContext.Model
                .FindEntityType(tableType)
                .GetTableName();
        }

        private async Task<string?> CreatePartitionForModel(string modelName, DateTime date, DbContext dbContext)
        {
            var tableName = $"{modelName}_{date:yyyy_MM_dd}";
            var startDate = date.ToString("yyyy-MM-dd");
            var endDate = date.AddDays(1).ToString("yyyy-MM-dd");

            //if (dbContext.IsTableExists(tableName))
            //{
            //    return null; // Партиция уже существует
            //}

            return $"CREATE TABLE  IF NOT EXISTS \"{tableName}\" PARTITION OF \"{modelName}\" FOR VALUES FROM ('{startDate}') TO ('{endDate}');";
            //var sql = $"CREATE TABLE \"{tableName}\" PARTITION OF \"{modelName}\" FOR VALUES FROM ('{startDate}') TO ('{endDate}');";

            //try
            //{
            //    await dbContext.Database.ExecuteSqlRawAsync(sql);
            //    Console.WriteLine("Partition created: {TableName}", tableName);
            //    return tableName;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Partition creation error {tableName} => {ex.Message}");
            //    return null;
            //}
        }

        private DbContext? GetTableDbContextType(Type tableType, List<DbContext> dbContexts)
        {
            foreach (var dbContext in dbContexts)
            {
                if (dbContext.Model.FindEntityType(tableType) != null)
                {
                    return dbContext;
                }
            }

            return null;
        }
        #endregion
    }

    public static class DbContextBackUpExceptions
    {
        public static Dictionary<Type, List<Type>> Entities = new Dictionary<Type, List<Type>>()
        {
            {
                typeof(MasofaCommonDbContext),
                new List<Type>()
                {
                    //typeof(LogMessage),
                    //typeof(CallStack),
                    //typeof(EmailMessage),
                    //typeof(UserTicket),
                    //typeof(HealthCheckResult)
                }
            },
            {
                typeof(MasofaCropMonitoringDbContext),
                new List<Type>()
            },
            {
                typeof(MasofaDictionariesDbContext),
                new List<Type>()
            },
            {
                typeof(MasofaEraDbContext),
                new List<Type>()
            },
            {
                typeof(MasofaIBMWeatherDbContext),
                new List<Type>()
            },
            {
                typeof(MasofaIdentityDbContext),
                new List<Type>()
            }
        };
    }
}
