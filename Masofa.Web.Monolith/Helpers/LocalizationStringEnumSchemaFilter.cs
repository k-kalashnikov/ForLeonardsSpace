using Masofa.Common.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Masofa.Web.Monolith.Helpers
{
    public class LocalizationStringSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type != typeof(LocalizationString))
            {
                return;
            }

            schema.Type = "object";
            var supportedLanguages = LocalizationString.SupportedLanguages;

            foreach (var lang in supportedLanguages)
            {
                schema.Properties.Add(lang, new OpenApiSchema
                {
                    Type = "string",
                    Description = $"Localized value for {lang}",
                    Nullable = true // если хочешь разрешить null
                });
            }

            schema.Required.Clear();
        }
    }
}
