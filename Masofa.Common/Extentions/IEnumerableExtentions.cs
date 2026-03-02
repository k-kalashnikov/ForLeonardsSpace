using System.Linq.Expressions;
using System.Reflection;

namespace System.Collections.Generic
{
    public static class IEnumerableExtentions
    {
        public static Dictionary<TKey, List<TValue>> ApplyGrouping<TKey, TValue>(this IEnumerable<TValue> enumerable, string groupNameField)
        {
            IQueryable<TValue> query = enumerable.AsQueryable<TValue>();
            if (string.IsNullOrWhiteSpace(groupNameField))
            {
                throw new ArgumentException($"Property '{groupNameField}' can't be null of white space");
            }
            var property = typeof(TValue).GetProperty(groupNameField, BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                throw new ArgumentException($"Property '{groupNameField}' not exist in type {typeof(TValue).Name}");
            }

            var parameter = Expression.Parameter(typeof(TValue), "m");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var groupByExp = Expression.Lambda(propertyAccess, parameter);

            string methodName = "GroupBy";
            var resultExp = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(TValue), property.PropertyType },
                query.Expression,
                Expression.Quote(groupByExp)
            );

            var result = query.Provider.CreateQuery<IGrouping<TKey, TValue>>(resultExp).ToDictionary(m => m.Key, m => m.ToList());
            return result;
        }
    }
}
