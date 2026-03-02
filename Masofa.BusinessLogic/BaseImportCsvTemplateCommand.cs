using Masofa.BusinessLogic.Extentions;
using Masofa.Common.Attributes;
using Masofa.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace Masofa.BusinessLogic
{
    public class BaseImportCsvTemplateCommand<TModel, TDbContext> : IRequest<byte[]>
        where TDbContext : DbContext
        where TModel : BaseEntity
    { }

    public class BaseImportCsvTemplateCommandHandler<TModel, TDbContext> : IRequestHandler<BaseImportCsvTemplateCommand<TModel, TDbContext>, byte[]>
        where TDbContext : DbContext
        where TModel : BaseEntity
    {
        public async Task<byte[]> Handle(BaseImportCsvTemplateCommand<TModel, TDbContext> request, CancellationToken cancellationToken)
        {
            var properties = typeof(TModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p => !p.IsDefined(typeof(SystemFieldNotForImport), false))
                .Where(p => !p.IsDefined(typeof(NotMappedAttribute), false))
                .Where(p => !p.IsDefined(typeof(SwaggerIgnoreAttribute), false))
                .ToList();

            List<string> headers = [];
            List<string> values = [];

            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(LocalizationString))
                {
                    foreach (var lang in LocalizationString.SupportedLanguages)
                    {
                        headers.Add($"{prop.Name}_{lang.Replace('-', '_')}");
                        object? exampleValue = prop.PropertyType.GetExampleValue();
                        values.Add(exampleValue?.ToString() ?? string.Empty);
                    }
                }
                else
                {
                    headers.Add(prop.Name);
                    object? exampleValue = prop.PropertyType.GetExampleValue();
                    values.Add(exampleValue?.ToString() ?? string.Empty);
                }
            }

            List<string> lines = [];
            lines.Add(string.Join(";", headers));
            lines.Add(string.Join(";", values));

            return Encoding.UTF8.GetBytes(string.Join("\n", lines));
        }
    }
}
