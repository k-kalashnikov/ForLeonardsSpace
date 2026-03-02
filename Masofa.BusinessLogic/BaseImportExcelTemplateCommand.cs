using ClosedXML.Excel;
using Masofa.BusinessLogic.Extentions;
using Masofa.Common.Attributes;
using Masofa.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Masofa.BusinessLogic
{
    public class BaseImportExcelTemplateCommand<TModel, TDbContext> : IRequest<byte[]>
        where TDbContext : DbContext
        where TModel : BaseEntity
    { }

    public class BaseImportExcelTemplateCommandHandler<TModel, TDbContext> : IRequestHandler<BaseImportExcelTemplateCommand<TModel, TDbContext>, byte[]>
        where TDbContext : DbContext
        where TModel : BaseEntity
    {
        public async Task<byte[]> Handle(BaseImportExcelTemplateCommand<TModel, TDbContext> request, CancellationToken cancellationToken)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet($"{typeof(TModel).Name}_Template");

            var properties = typeof(TModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p => !p.IsDefined(typeof(SystemFieldNotForImport), false))
                .Where(p => !p.IsDefined(typeof(NotMappedAttribute), false))
                .Where(p => !p.IsDefined(typeof(SwaggerIgnoreAttribute), false))
                .ToList();

            int col = 1;
            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(LocalizationString))
                {
                    foreach (var lang in LocalizationString.SupportedLanguages)
                    {
                        worksheet.Cell(1, col).Value = $"{prop.Name}_{lang.Replace('-', '_')}";
                        object? exampleValue = prop.PropertyType.GetExampleValue();
                        worksheet.Cell(2, col).Value = exampleValue?.ToString() ?? string.Empty;
                        col++;
                    }
                }
                else
                {
                    worksheet.Cell(1, col).Value = prop.Name;
                    object? exampleValue = prop.PropertyType.GetExampleValue();
                    worksheet.Cell(2, col).Value = exampleValue?.ToString() ?? string.Empty;
                    col++;
                }
            }

            worksheet.Columns().AdjustToContents();

            var tableRange = worksheet.Range(1, 1, 2, col - 1);
            var table = tableRange.CreateTable();
            table.ShowAutoFilter = false;
            table.Theme = XLTableTheme.TableStyleMedium4;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
