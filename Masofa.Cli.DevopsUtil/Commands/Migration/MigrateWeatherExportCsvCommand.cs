using Masofa.Common.Attributes;
using Masofa.Common.Models;
using Masofa.Common.Models.SystemCrical;
using Masofa.Depricated.DataAccess.DepricatedWeatherServerOne;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using System.Reflection;

namespace Masofa.Cli.DevopsUtil.Commands.Migration
{
    public class MigrateWeatherExportCsvCommandParameters
    {
        [TaskParameter("Путь для экспорта CSV файлов", true, "C:\\WeatherExport")]
        public string BaseExportPath { get; set; } = string.Empty;

        public static MigrateWeatherExportCsvCommandParameters Parse(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
                throw new ArgumentException("Необходимо указать путь экспорта");

            return new MigrateWeatherExportCsvCommandParameters { BaseExportPath = args[0] };
        }

        public static MigrateWeatherExportCsvCommandParameters GetFromUser()
        {
            Console.WriteLine($"Pls enter the current base path");
            var baseExportPath = Console.ReadLine() ?? string.Empty;
            return new MigrateWeatherExportCsvCommandParameters { BaseExportPath = baseExportPath };
        }
    }

    [BaseCommand("Migrate Weather Export CSV", "Экспорт данных погоды в CSV формат", typeof(MigrateWeatherExportCsvCommandParameters))]
    public class MigrateWeatherExportCsvCommand : IBaseCommand
    {
        private DepricatedWeatherServerOneDbContext DepricatedWeatherServerOneDbContext { get; set; }

        private string BaseExportPath { get; set; }

        public MigrateWeatherExportCsvCommand(DepricatedWeatherServerOneDbContext depricatedWeatherServerOneDbContext)
        {
            DepricatedWeatherServerOneDbContext = depricatedWeatherServerOneDbContext;
        }

        public async Task Execute()
        {
            var parameters = MigrateWeatherExportCsvCommandParameters.GetFromUser();
            await ExecuteCore(parameters);
        }

        public async Task Execute(string[] args)
        {
            var parameters = MigrateWeatherExportCsvCommandParameters.Parse(args);
            await ExecuteCore(parameters);
        }

        private async Task ExecuteCore(MigrateWeatherExportCsvCommandParameters parameters)
        {
            BaseExportPath = parameters.BaseExportPath;
            await ExportWeatherTablesAsync();
        }

        public void Dispose()
        {
        }

        #region MigrateWeather

        private async Task ExportWeatherTablesAsync()
        {
            var entityTypes = DepricatedWeatherServerOneDbContext.Model
                .GetEntityTypes()
                .Where(e => e.ClrType.IsClass && !e.ClrType.Name.Contains("Migration"))
                .ToList();

            foreach (var entityType in entityTypes)
            {
                var clrType = entityType.ClrType;
                var tableName = entityType.GetTableName();

                try
                {
                    //if (tableName is "weather_stations_data" or "weather_stations_data_ex")
                    //{

                    //}

                    var pagedMethod = GetType().GetMethod(nameof(ExportPagedTableManuallyAsync), BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(clrType);
                    await (Task)pagedMethod.Invoke(this, new object[] { tableName })!;
                    continue;

                    //var method = GetType()
                    //    .GetMethod(nameof(ExportTableDynamicAsync), BindingFlags.NonPublic | BindingFlags.Instance)!
                    //    .MakeGenericMethod(clrType);

                    //await (Task)method.Invoke(this, new object[] { tableName })!;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: Failed to export table '{tableName}': {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"ERROR: {ex.InnerException.Message}");
                }
            }
        }

        private async Task ExportPagedTableManuallyAsync<TEntity>(string tableName)
            where TEntity : class
        {
            int pageSize = 2000;
            int offset = 0;
            bool hasMore = true;
            int currentDirectory = 0;

            var properties = typeof(TEntity)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p =>
                {
                    var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

                    return type.IsPrimitive ||
                           type == typeof(string) ||
                           type == typeof(Guid) ||
                           type == typeof(DateTime) ||
                           type == typeof(LocalDateTime) ||
                           type == typeof(decimal) ||
                           type == typeof(double) ||
                           type == typeof(float) ||
                           type == typeof(bool) ||
                           type == typeof(int) ||
                           type == typeof(long);
                })
                .ToList();

            var fieldNames = properties.Select(p => p.Name).ToList();
            //var filePath = Path.Combine(BaseExportPath, tableName, date.ToString("yyyy"), date.ToString("MM"), $"{date:dd}.csv");
            var messages = new Dictionary<DateOnly, List<string>>();
            var updated = new List<string>();
            try
            {
                //using var writer = new StreamWriter(filePath, append: true);

                while (hasMore)
                {
                    var page = await DepricatedWeatherServerOneDbContext.Set<TEntity>()
                        .AsNoTracking()
                        .Skip(offset)
                        .Take(pageSize)
                        .ToListAsync();

                    if (page.Count == 0)
                        break;

                    var filtered = page;

                    //if (!headerWritten)
                    //{
                    //    await writer.WriteLineAsync(string.Join(",", fieldNames));
                    //    headerWritten = true;
                    //}

                    foreach (var row in filtered)
                    {
                        var values = fieldNames.Select(f =>
                        {
                            var prop = typeof(TEntity).GetProperty(f)!;
                            var value = prop.GetValue(row);
                            return Escape(value);
                        });
                        DateOnly tempKey = DateOnly.FromDateTime(DateTime.Now);

                        var tempDateProp = typeof(TEntity).GetProperty("Date");
                        if ((tempDateProp == null))
                        {
                            updated.Add(string.Join(',', values));
                            continue;
                        }

                        if ((tempDateProp != null) && (tempDateProp.PropertyType == typeof(LocalDate)))
                        {
                            LocalDate tempDate = (LocalDate)tempDateProp.GetValue(row);
                            tempKey = new DateOnly(tempDate.Year, tempDate.Month, tempDate.Day);

                        }
                        else if ((tempDateProp != null) && ((tempDateProp.PropertyType == typeof(LocalDateTime)) || (tempDateProp.PropertyType == typeof(LocalDateTime?))))
                        {
                            LocalDateTime tempDate = (LocalDateTime)tempDateProp.GetValue(row);
                            tempKey = new DateOnly(tempDate.Year, tempDate.Month, tempDate.Day);
                        }
                        if (!messages.ContainsKey(tempKey))
                        {
                            messages.Add(tempKey, new List<string>());
                        }

                        messages[tempKey].Add(string.Join(',', values));
                    }
                    if (updated.Any())
                    {
                        if (!Directory.Exists(Path.Combine(BaseExportPath, tableName, $"Backup_{currentDirectory}")))
                        {
                            EnsureDirectoriesExistRecursive(Path.Combine(BaseExportPath, tableName, $"Backup_{currentDirectory}"));
                        }
                        if ((Directory.GetFiles(Path.Combine(BaseExportPath, tableName, $"Backup_{currentDirectory}")).Count() >= 500))
                        {
                            currentDirectory++;
                        }
                        var fileUndatedPath = Path.Combine(BaseExportPath, tableName, $"Backup_{currentDirectory}", $"{offset.ToString()}.csv");
                        File.AppendAllLines(fileUndatedPath, updated);
                    }


                    offset += pageSize;
                    hasMore = page.Count == pageSize;

                    if (messages.Any())
                    {
                        foreach (var item in messages)
                        {
                            var filePath = Path.Combine(BaseExportPath, tableName, item.Key.Year.ToString(), item.Key.Month.ToString(), $"{item.Key.Day.ToString()}.csv");
                            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                            }
                            File.AppendAllLines(filePath, item.Value);
                        }
                    }




                    Console.WriteLine($"SUCCESS: Exported 1000 record paged table '{tableName}' to files");

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to export paged table '{tableName}': {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"ERROR: {ex.InnerException.Message}");
            }
            Console.WriteLine("END: Export Paged tables ");
        }

        private static string Escape(object? value)
        {
            if (value == null) return "";
            var str = value.ToString();
            if (str!.Contains(',') || str.Contains('"'))
            {
                str = str.Replace("\"", "\"\"");
                return $"\"{str}\"";
            }
            return str;
        }

        private void EnsureDirectoriesExistRecursive(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(directoryPath));
            }

            string[] pathParts = directoryPath.Split(Path.DirectorySeparatorChar);
            string currentPath = string.Empty;

            foreach (string part in pathParts)
            {
                if (string.IsNullOrEmpty(part))
                {
                    continue; // Пропускаем пустые части пути
                }
                currentPath = Path.Combine(currentPath, part);
                Console.WriteLine($"Checking path: {currentPath}");
                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }

        //private async Task ExportTableDynamicAsync<TEntity>(string tableName)
        //    where TEntity : class
        //{
        //    var properties = typeof(TEntity)
        //        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //        .Where(p =>
        //        {
        //            var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

        //            return type.IsPrimitive ||
        //                   type == typeof(string) ||
        //                   type == typeof(Guid) ||
        //                   type == typeof(DateTime) ||
        //                   type == typeof(decimal) ||
        //                   type == typeof(double) ||
        //                   type == typeof(float) ||
        //                   type == typeof(bool) ||
        //                   type == typeof(int) ||
        //                   type == typeof(long);
        //        })
        //        .ToList();

        //    if (properties.Count == 0)
        //    {
        //        Console.WriteLine($"WARNING: No exportable fields found for table '{tableName}'");
        //        return;
        //    }

        //    var fieldNames = properties.Select(p => p.Name).ToList();

        //    var filePath = Path.Combine(BaseExportPath, tableName, date.ToString("yyyy"), date.ToString("MM"), $"{date:dd}.csv");
        //    var messages = new Dictionary<DateTime, List<string>>();
        //    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        //    try
        //    {
        //        var records = await DepricatedWeatherServerOneDbContext
        //            .Set<TEntity>()
        //            .AsNoTracking()
        //            .SelectFields(fieldNames)
        //            .ToListAsync();

        //        using var writer = new StreamWriter(filePath);

        //        //// Заголовки
        //        //await writer.WriteLineAsync(string.Join(",", fieldNames));

        //        foreach (var record in records)
        //        {
        //            var values = fieldNames.Select(f => Escape(record[f]));
        //            await writer.WriteLineAsync(string.Join(",", values));
        //        }

        //        Console.WriteLine($"SUCCESS: Exported table '{tableName}' to '{filePath}'");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"ERROR: Failed to write table '{tableName}' to file: {ex.Message}");
        //        if (ex.InnerException != null)
        //            Console.WriteLine($"ERROR: {ex.InnerException.Message}");
        //    }

        //    Console.WriteLine("END: Export Dynamic tables ");
        //}
        #endregion
    }
}
