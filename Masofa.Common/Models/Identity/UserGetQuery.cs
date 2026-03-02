using System.Reflection;

namespace Masofa.Common.Models.Identity
{
    public class UserGetQuery
    {
        public int? Take { get; set; }
        public int Offset { get; set; } = 0;
        public string? SortBy { get; set; }
        public SortType Sort { get; set; } = SortType.ASC;

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

        private List<FieldFilter> FixFiltersNames(List<FieldFilter> fieldFilters)
        {
            var result = new List<FieldFilter>();

            foreach (var item in fieldFilters)
            {
                var field = typeof(User).GetRuntimeProperties()
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
}