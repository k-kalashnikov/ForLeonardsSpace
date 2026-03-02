using Masofa.Common.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace Masofa.Common.Models
{
    public class BaseGetQuery<TModel>
    {
        public int? Take { get; set; }
        public int Offset { get; set; } = 0;
        public string? SortBy { get; set; }
        public SortType Sort { get; set; } = SortType.ASC;
        public List<string> SelectFields { get; set; } = new List<string>();
        public List<FieldFilter> Filters
        {
            get
            {
                return _filters;
            }
            set
            {
                _filters = FixFiltersNames(value);
            }
        }

        private List<FieldFilter> _filters = new List<FieldFilter>();

        public (bool, string) FilterIsValid()
        {
            foreach (var item in Filters)
            {
                var field = typeof(TModel).GetRuntimeProperties()
                    .FirstOrDefault(m => m.Name.Equals(item.FilterField));

                if (field == null)
                {
                    return (false, $"Field with name {item.FilterField} is not exist");
                }

                if (field.PropertyType != typeof(string))
                {
                    try
                    {
                        var temp = JsonConvert.DeserializeObject(item.FilterValue.ToString(), field.PropertyType);
                    }
                    catch (Exception e)
                    {
                        return (false, $"Can't convert {item.FilterValue} to {field.PropertyType.Name}");
                    }
                }
            }
            return (true, string.Empty);
        }

        private List<FieldFilter> FixFiltersNames(List<FieldFilter> fieldFilters)
        {
            var result = new List<FieldFilter>();

            foreach (var item in fieldFilters)
            {
                var field = typeof(TModel).GetRuntimeProperties()
                    .FirstOrDefault(m => m.Name.ToLower().Equals(item.FilterField.ToLower()));

                if (field == null)
                {
                    throw new ArgumentException($"Field with name {item.FilterField} is not exist", nameof(Filters));
                }

                result.Add(new FieldFilter()
                {
                    FilterField = field.Name,
                    FilterOperator = item.FilterOperator,
                    FilterValue = item.FilterValue
                });
            }

            return result;
        }
    }

    public class FieldFilter
    {
        public string? FilterField { get; set; }
        public object? FilterValue { get; set; }
        public FilterOperator? FilterOperator { get; set; }
    }

    public enum SortType
    {
        ASC,
        DSC
    }

    public enum FilterOperator
    {
        Equals = 0,
        NotEquals = 1,
        GreaterThan = 2,
        GreaterThanOrEqual = 3,
        LessThan = 4,
        LessThanOrEqual = 5,
        Contains = 6,
        StartsWith = 7,
        EndsWith = 8,
        Levenshtein = 9
    }
}
