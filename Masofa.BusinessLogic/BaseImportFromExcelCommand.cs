using ClosedXML.Excel;
using Masofa.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Masofa.BusinessLogic
{
    public class BaseImportFromExcelCommand<TModel, TDbContext> : IRequest<Unit>
        where TDbContext : DbContext
        where TModel : BaseEntity
    {
        [Required]
        public required IFormFile FormFile { get; set; }

        [Required]
        public required string Author { get; set; }
    }

    public class BaseImportFromExcelCommandHandler<TModel, TDbContext> : IRequestHandler<BaseImportFromExcelCommand<TModel, TDbContext>, Unit>
        where TDbContext : DbContext
        where TModel : BaseEntity
    {
        private IMediator Mediator { get; set; }

        public BaseImportFromExcelCommandHandler(IMediator mediator)
        {
            Mediator = mediator;
        }

        public async Task<Unit> Handle(BaseImportFromExcelCommand<TModel, TDbContext> request, CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream();

            await request.FormFile.CopyToAsync(ms, cancellationToken);

            ms.Seek(0, SeekOrigin.Begin);

            var records = CollectRecordsFromExcel(ms);

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

        public List<TModel> CollectRecordsFromExcel(Stream fileStream)
        {
            var namesColumns = GetNamesColumns();
            using var workbook = new XLWorkbook(fileStream);
            var ws = workbook.Worksheet(1);

            var records = new List<TModel>();

            List<string> headers = [];
            var headerRow = ws.FirstRowUsed();
            if (headerRow != null)
            {
                foreach (var headerCell in headerRow.CellsUsed())
                {
                    headers.Add(headerCell.GetValue<string>());
                }
            }

            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var record = Activator.CreateInstance<TModel>();
                LocalizationString names = new();

                for (var i = 0; i < headers.Count; i++)
                {
                    var header = headers[i];

                    var property = typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .FirstOrDefault(p => string.Equals(p.Name, header, StringComparison.OrdinalIgnoreCase));

                    var rawValue = row.Cell(i + 1).GetValue<string>();
                    try
                    {
                        if (string.IsNullOrEmpty(rawValue))
                        {
                            continue;
                        }
                        var trimmedValue = rawValue.Trim();
                        if (namesColumns.Contains(header))
                        {
                            names[header.Replace("Names_", "").Replace('_', '-')] = trimmedValue;
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
                        Console.WriteLine($"Error converting '{header}'='{rawValue}' to {property.Name}: {ex.Message}");
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

            return records;
        }

        private List<string> GetNamesColumns()
        {
            var names = new List<string>();
            foreach (var lang in LocalizationString.SupportedLanguages)
            {
                names.Add($"Names_{lang.Replace('-', '_')}");
            }
            return names;
        }
    }
}