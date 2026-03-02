using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Masofa.Web.Monolith.Helpers
{
    public class StringEnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!context.Type.IsEnum)
            {
                return;
            }

            if (context.Type.GetCustomAttribute<Newtonsoft.Json.JsonConverterAttribute>() == null)
            {
                return;
            }

            var jca = context.Type.GetCustomAttribute<Newtonsoft.Json.JsonConverterAttribute>();

            if (jca.ConverterType != typeof(Newtonsoft.Json.Converters.StringEnumConverter))
            {
                return;
            }

            schema.Type = "string";

            var names = new List<string>();
            var values = Enum.GetValues(context.Type);

            foreach (var value in values)
            {
                var fieldInfo = context.Type.GetField(value.ToString());
                var displayAttribute = fieldInfo?.GetCustomAttribute<DisplayAttribute>();

                names.Add(displayAttribute?.Name ?? value.ToString());
            }

            schema.Enum.Clear();
            foreach (var name in names)
            {
                schema.Enum.Add(new OpenApiString(name));
            }
        }
    }
}
