using CsvHelper;
using Masofa.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;

namespace Masofa.BusinessLogic.Cli.DbSeeders
{
    public abstract class AbstractCsvSeeder<TContext> where TContext : DbContext
    {
        protected TContext Context { get; }
        protected readonly string CsvDirectory;

        protected AbstractCsvSeeder(TContext context, string csvSubPath)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));

            CsvDirectory = Path.Combine(AppContext.BaseDirectory, csvSubPath);

            if (!Directory.Exists(CsvDirectory))
            {
                throw new DirectoryNotFoundException($"CSV directory not found: {CsvDirectory}");
            }
        }

        public async Task SeedAsync()
        {
            Console.WriteLine($"Starting seeding from CSV files in {CsvDirectory}...");

            var csvFiles = Directory.GetFiles(CsvDirectory, "*.csv");
            foreach (var csvFile in csvFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(csvFile);
                var entityName = NormalizeEntityName(fileName);
                await SeedFromFileAsync(csvFile, entityName);
            }

            await Context.SaveChangesAsync();
            Console.WriteLine("Seeding completed.");
        }

        private async Task SeedFromFileAsync(string filePath, string entityName)
        {
            Type? entityType = null;
            PropertyInfo? dbSetProperty = null;

            foreach (var prop in Context.GetType().GetProperties())
            {
                if (prop.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase) &&
                    prop.PropertyType.IsGenericType &&
                    prop.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                {
                    entityType = prop.PropertyType.GetGenericArguments()[0];
                    dbSetProperty = prop;
                    break;
                }
            }

            if (entityType == null || dbSetProperty == null)
            {
                Console.WriteLine($"No matching DbSet found for file: {entityName}. Skipping.");
                return;
            }

            Console.WriteLine($"Seeding {entityType.Name} from {filePath}...");

            var delimiter = AbstractCsvSeeder<TContext>.DetectDelimiter(filePath);

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter.ToString(),
                HasHeaderRecord = true,
                TrimOptions = CsvHelper.Configuration.TrimOptions.Trim
            });

            csv.Read();
            csv.ReadHeader();
            var headers = csv.HeaderRecord;

            var records = new List<object>();
            while (csv.Read())
            {
                var record = Activator.CreateInstance(entityType);

                LocalizationString names = new();

                foreach (var header in headers ?? [])
                {
                    var property = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(p => string.Equals(p.Name, header, StringComparison.OrdinalIgnoreCase));

                    try
                    {
                        var rawValue = csv.GetField<string>(header);
                        if (string.IsNullOrEmpty(rawValue))
                        {
                            continue;
                        }

                        var trimmedValue = rawValue.Trim();
                        if (header == "name_en" || header == "NameEn")
                        {
                            names["en-US"] = trimmedValue;
                            continue;
                        }
                        if (header == "name_ru" || header == "NameRu")
                        {
                            names["ru-RU"] = trimmedValue;
                            continue;
                        }
                        if (header == "name_uz" || header == "NameUz")
                        {
                            names["uz-Latn-UZ"] = trimmedValue;
                            continue;
                        }

                        if (property == null)
                        {
                            continue;
                        }

                        Type targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        object? convertedValue = null;

                        if (targetType == typeof(Guid))
                        {
                            convertedValue = new Guid(trimmedValue);
                        }
                        else if (targetType == typeof(DateTime))
                        {
                            convertedValue = DateTime.SpecifyKind(DateTime.Parse(trimmedValue), DateTimeKind.Utc);
                        }
                        else if (targetType == typeof(int))
                        {
                            if (int.TryParse(trimmedValue, out int intValue))
                            {
                                convertedValue = intValue;
                            }
                        }
                        else if (targetType == typeof(LocalizationString))
                        {
                            convertedValue = new LocalizationString { ValuesJson = trimmedValue };
                        }
                        else
                        {
                            convertedValue = Convert.ChangeType(trimmedValue, targetType);
                        }

                        if (convertedValue != null)
                        {
                            property.SetValue(record, convertedValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting '{header}'='{csv[header]}' to {property.Name}: {ex.Message}");
                    }
                }


                if (record != null)
                {
                    var localizednamesProperty = record.GetType().GetProperty("Names");
                    if (localizednamesProperty != null && localizednamesProperty.CanWrite)
                    {
                        localizednamesProperty.SetValue(record, names);
                    }
                    records.Add(record);
                }
            }

            var idProperty = entityType.GetProperty("Id");
            if (idProperty == null)
            {
                Console.WriteLine($"Entity {entityType.Name} does not have 'Id' property. Skipping insert.");
                return;
            }

            if (dbSetProperty.GetValue(Context) is not IQueryable<object> dbSetAsQueryable) return;

            var existingEntities = await dbSetAsQueryable.ToListAsync();
            var namesProperty = entityType.GetProperty("Names");

            if (namesProperty != null)
            {
                var existingNames = new HashSet<string>(
                    existingEntities.Select(e =>
                    {
                        if (LocalizationString.SupportedLanguages.Contains("en-US"))
                        {
                            return ((LocalizationString)namesProperty!.GetValue(e)!)["en-US"]!;
                        }

                        return string.Empty;
                    }));

                var newRecords = records
                    .Where(r => !existingNames.Contains(((LocalizationString)namesProperty!.GetValue(r)!)["en-US"]!))
                    .ToList();

                if (newRecords.Count > 0)
                {
                    await Context.AddRangeAsync(newRecords);
                    Console.WriteLine($"Added {newRecords.Count} new records to {entityName}.");
                }
                else
                {
                    Console.WriteLine($"No new records found for {entityName}.");
                }
            }



            //var newRecords = records
            //    .Where(r => !existingIds.Contains((Guid)idProperty.GetValue(r)!))
            //    .ToList();


        }

        private static char DetectDelimiter(string filePath)
        {
            using var reader = new StreamReader(filePath);
            var line = reader.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                throw new InvalidDataException("CSV file is empty.");
            }
            var commaCount = line.Count(c => c == ',');
            var semicolonCount = line.Count(c => c == ';');
            return semicolonCount > commaCount ? ';' : ',';
        }

        private string NormalizeEntityName(string fileName)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);

            return CultureInfo.InvariantCulture.TextInfo
                .ToTitleCase(name.Replace("_", " ").ToLower())
                .Replace(" ", "");
        }
    }
}
