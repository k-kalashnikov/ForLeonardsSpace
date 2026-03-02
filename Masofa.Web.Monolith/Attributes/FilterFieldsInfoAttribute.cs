using System.ComponentModel;
using System.Reflection;

namespace Masofa.Web.Monolith.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FilterFieldsInfoAttribute : Attribute
    {
        public FilterFieldsInfo[] Fields { get; }

        public FilterFieldsInfoAttribute(params FilterFieldsInfo[] fields)
        {
            Fields = fields;
        }

        public FilterFieldsInfoAttribute(Type type)
        {
            Fields = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<DescriptionAttribute>() != null)
                .Select(p => new FilterFieldsInfo()
                {
                    FieldName = p.Name,
                    FieldType = p.PropertyType.ToString()
                })
                .ToArray();
        }
    }

    public class FilterFieldsInfo
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }
    }
}
