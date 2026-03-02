using CsvHelper;
using Masofa.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Masofa.BusinessLogic
{
    public class BaseImportFromCSVCommand<TModel, TDbContext> : IRequest<Unit>
        where TDbContext : DbContext
        where TModel : BaseEntity
    {
        [Required]
        public IFormFile FormFile { get; set; }

        [Required]
        public string Author { get; set; }
    }

    public class BaseImportFromCSVCommandHandler<TModel, TDbContext> : IRequestHandler<BaseImportFromCSVCommand<TModel, TDbContext>, Unit>
        where TDbContext : DbContext
        where TModel : BaseEntity
    {
        private TDbContext DbContext { get; set; }
        private IMediator Mediator { get; set; }

        public BaseImportFromCSVCommandHandler(TDbContext dbContext, IMediator mediator)
        {
            DbContext = dbContext;
            Mediator = mediator;
        }

        public async Task<Unit> Handle(BaseImportFromCSVCommand<TModel, TDbContext> request, CancellationToken cancellationToken)
        {
            var delimiter = DetectDelimiter(request.FormFile);
            var records = new List<TModel>();

            using (var reader = new StreamReader(request.FormFile.OpenReadStream()))
            {
                using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = delimiter.ToString(),
                    HasHeaderRecord = true,
                    TrimOptions = CsvHelper.Configuration.TrimOptions.Trim
                }))
                {
                    csv.Read();
                    csv.ReadHeader();
                    var headers = csv.HeaderRecord;


                    while (csv.Read())
                    {
                        var record = Activator.CreateInstance<TModel>();

                        LocalizationString names = new();

                        foreach (var header in headers ?? [])
                        {
                            var property = typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .FirstOrDefault(p => string.Equals(p.Name, header, StringComparison.OrdinalIgnoreCase));

                            try
                            {
                                var rawValue = csv.GetField<string>(header);
                                if (string.IsNullOrEmpty(rawValue))
                                {
                                    continue;
                                }

                                var trimmedValue = rawValue.Trim();
                                // Handle LocalizationString fields with proper header names
                                if (header == "Name (EN)" || header == "name_en" || header == "NameEn" ||
                                    ((header?.ToLower().Contains("name", StringComparison.CurrentCultureIgnoreCase) ?? false) &&
                                     (header?.ToLower().Contains("en", StringComparison.CurrentCultureIgnoreCase) ?? false)
                                    ))
                                {
                                    names["en-US"] = trimmedValue;
                                    continue;
                                }
                                if (header == "Name (RU)" || header == "name_ru" || header == "NameRu" ||
                                    ((header?.ToLower().Contains("name", StringComparison.CurrentCultureIgnoreCase) ?? false) &&
                                     (header?.ToLower().Contains("ru", StringComparison.CurrentCultureIgnoreCase) ?? false)
                                    ))
                                {
                                    names["ru-RU"] = trimmedValue;
                                    continue;
                                }
                                if (header == "Name (UZ)" || header == "name_uz" || header == "NameUz" ||
                                    ((header?.ToLower().Contains("name", StringComparison.CurrentCultureIgnoreCase) ?? false) &&
                                     (header?.ToLower().Contains("uz", StringComparison.CurrentCultureIgnoreCase) ?? false)
                                    ))
                                {
                                    if (header?.ToLower().Contains("latn", StringComparison.CurrentCultureIgnoreCase) ?? false)
                                    {
                                        names["uz-Latn-UZ"] = trimmedValue;
                                    }
                                    else if (header?.ToLower().Contains("cyrl", StringComparison.CurrentCultureIgnoreCase) ?? false)
                                    {
                                        names["uz-Cyrl-UZ"] = trimmedValue;
                                    }
                                    else
                                    {
                                        names["uz-Latn-UZ"] = trimmedValue;
                                    }

                                    continue;
                                }
                                if (header == "Name (AR)" || header == "name_ar" || header == "NameAr" ||
                                    ((header?.ToLower().Contains("name", StringComparison.CurrentCultureIgnoreCase) ?? false) &&
                                     (header?.ToLower().Contains("ar", StringComparison.CurrentCultureIgnoreCase) ?? false)
                                    ))
                                {
                                    names["ar-LB"] = trimmedValue;
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
                }
            }

            foreach (var record in records)
            {
                await Mediator.Send(new BaseCreateCommand<TModel, TDbContext>()
                {
                    Model = record,
                    Author = request.Author
                });
            }

            return new Unit();
        }

        private char DetectDelimiter(IFormFile formFile)
        {
            using var reader = new StreamReader(formFile.OpenReadStream());
            var line = reader.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                throw new InvalidDataException("CSV file is empty.");
            }
            var commaCount = line.Count(c => c == ',');
            var semicolonCount = line.Count(c => c == ';');
            reader.Close();
            reader.Dispose();
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
