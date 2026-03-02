using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Masofa.Web.Monolith.Helpers
{
    public class SwaggerIgnoreFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
                return;

            var type = context.Type;

            // Получаем все свойства типа
            var propertiesToIgnore = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<SwaggerIgnoreAttribute>() != null)
                .Select(p => p.Name);

            // Удаляем свойства из документации Swagger
            foreach (var propertyName in propertiesToIgnore)
            {
                if (schema.Properties.ContainsKey(propertyName))
                {
                    schema.Properties.Remove(propertyName);
                }
            }
        }
    }
}
