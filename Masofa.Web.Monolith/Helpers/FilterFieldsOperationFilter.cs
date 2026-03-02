using Masofa.Web.Monolith.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Masofa.Web.Monolith.Helpers
{
    public class FilterFieldsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterFieldsAttr = context.MethodInfo.GetCustomAttributes(true)
                .OfType<FilterFieldsInfoAttribute>()
                .FirstOrDefault();

            if (filterFieldsAttr != null && operation.Parameters != null)
            {
                var filterFields = string.Join(",<br/>", filterFieldsAttr.Fields.Select(m => $" - Name: {m.FieldName} - Type:{m.FieldType}"));

                operation.Description += $"\n\n<b>Posible fields for filter:</b> <br/>{filterFields}";
            }
        }
    }
}
