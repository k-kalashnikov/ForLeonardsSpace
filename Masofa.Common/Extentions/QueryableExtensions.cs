using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Helper;
using NetTopologySuite.Index.HPRtree;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Tls;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq
{
    public static class QueryableExtensions
    {
        public static IOrderedQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, string? sortBy, SortType sortType)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query as IOrderedQueryable<T>;
            }

            var property = typeof(T).GetProperty(sortBy, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                throw new ArgumentException($"Property '{sortBy}' not exist in type {typeof(T).Name}");
            }

            var parameter = Expression.Parameter(typeof(T), "m");

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            string methodName = sortType == SortType.ASC ? "OrderBy" : "OrderByDescending";
            var resultExp = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), property.PropertyType },
                query.Expression,
                Expression.Quote(orderByExp)
            );

            return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(resultExp);
        }

        public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query, FieldFilter filterParam)
        {
            if (string.IsNullOrWhiteSpace(filterParam.FilterField) || filterParam.FilterValue == null || !filterParam.FilterOperator.HasValue)
            {
                return query;
            }

            var property = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name.ToLower().Equals(filterParam.FilterField.ToLower()));
            //var property = typeof(T).GetProperty(filterParam.FilterField, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                throw new ArgumentException($"Свойство '{filterParam.FilterField}' не существует в типе {typeof(T).Name}");
            }

            if (filterParam.FilterOperator.Value == FilterOperator.Levenshtein)
            {
                return ApplyLevenshteinFiltering(query, property, filterParam);
            }

            var parameter = Expression.Parameter(typeof(T), "m");

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            object filterTypedValue;
            Expression filterValue;
            
            // Special handling for LocalizationString - don't convert, use as string
            if (property.PropertyType == typeof(LocalizationString))
            {
                filterTypedValue = filterParam.FilterValue.ToString();
                filterValue = Expression.Constant(filterTypedValue, typeof(string));
            }
            else
            {
                filterTypedValue = ConvertToType(filterParam.FilterValue.ToString(), property.PropertyType);
                filterValue = Expression.Constant(filterTypedValue, property.PropertyType);
            }



            Expression left = propertyAccess;
            Expression right = filterValue;

            if (property.PropertyType == typeof(string) && !property.Name.Contains("Json", StringComparison.OrdinalIgnoreCase))
            {
                var toLowerMethod = typeof(string).GetMethod("ToUpper", Type.EmptyTypes)!;
                left = Expression.Call(propertyAccess, toLowerMethod);
                right = Expression.Call(filterValue, toLowerMethod);
            }

            Expression comparison = GetComparisonExpression(left, right, filterParam.FilterOperator.Value);

            var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<T> ApplySelectFields<T>(this IQueryable<T> query, List<string> fields)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var bindings = new List<MemberBinding>();
            foreach (var field in fields)
            {
                var tempFieldName = field.ToLower();
                if (tempFieldName.Contains("json"))
                {
                    tempFieldName = tempFieldName.Replace("json", string.Empty);
                }
                var property = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name.ToLower() == tempFieldName);
                if (property == null || !property.CanWrite)
                    continue;

                var propertyAccess = Expression.Property(parameter, property);
                bindings.Add(Expression.Bind(property, propertyAccess));
            }

            var memberInit = Expression.MemberInit(Expression.New(typeof(T)), bindings);
            var lambda = Expression.Lambda<Func<T, T>>(memberInit, parameter);

            return query.Select(lambda);
        }

        public static Dictionary<TKey, List<TValue>> ApplyGrouping<TKey, TValue>(this IQueryable<TValue> query, string groupNameField)
        {
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

            var result= query.Provider.CreateQuery<IGrouping<TKey, TValue>>(resultExp).ToDictionary(m => m.Key, m => m.ToList());
            return result;
        }

        public static Dictionary<object, List<TItem>> ApplyGrouping<TItem>(this IQueryable<TItem> query, Type typeKey, string groupNameField)
        {
            return (Dictionary<object, List<TItem>>)(query.GetType().GetMethods().FirstOrDefault(m => m.Name == "ApplyGrouping").MakeGenericMethod(typeof(DateTime), typeof(TItem)).Invoke(query, new object[] { "Date" }));
        }

        private static Expression GetComparisonExpression(Expression property, Expression value, FilterOperator op)
        {
            switch (op)
            {
                case FilterOperator.Equals:
                    return Expression.Equal(property, value);
                case FilterOperator.NotEquals:
                    return Expression.NotEqual(property, value);
                case FilterOperator.GreaterThan:
                    return Expression.GreaterThan(property, value);
                case FilterOperator.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(property, value);
                case FilterOperator.LessThan:
                    return Expression.LessThan(property, value);
                case FilterOperator.LessThanOrEqual:
                    return Expression.LessThanOrEqual(property, value);
                case FilterOperator.Contains:
                    if (property.Type == typeof(LocalizationString))
                    {
                        // For LocalizationString, convert to string first using implicit conversion
                        var convertedProperty = Expression.Convert(property, typeof(string));
                        return Expression.Call(convertedProperty, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, value);
                    }
                    else if (property.Type != typeof(string))
                    {
                        throw new InvalidOperationException("Оператор 'Contains' может использоваться только со строками или LocalizationString.");
                    }
                    return Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, value);
                case FilterOperator.StartsWith:
                    if (property.Type != typeof(string))
                        throw new InvalidOperationException("Оператор 'StartsWith' может использоваться только со строками.");
                    return Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!, value);
                case FilterOperator.EndsWith:
                    if (property.Type != typeof(string))
                        throw new InvalidOperationException("Оператор 'EndsWith' может использоваться только со строками.");
                    return Expression.Call(property, typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!, value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, "Неизвестный оператор фильтрации.");
            }
        }

        private static IQueryable<T> ApplyLevenshteinFiltering<T>(IQueryable<T> query, PropertyInfo property, FieldFilter filterParam)
        {
            var underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            
            if (underlyingType != typeof(string) && property.PropertyType != typeof(LocalizationString))
            {
                throw new InvalidOperationException("Levenshtein operator can only be used with string fields.");
            }

            if (property.PropertyType == typeof(LocalizationFileStorageItem))
            {
                throw new InvalidOperationException("Levenshtein operator cannot be used with LocalizationFileStorageItem.");
            }

            if (property.Name.Contains("Json", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Levenshtein operator cannot be used with JSON interpretation fields.");
            }

            var hasJsonConverter = property.GetCustomAttributes(typeof(JsonConverterAttribute), true).Any();
            if (hasJsonConverter)
            {
                throw new InvalidOperationException("Levenshtein operator cannot be used with fields that have JsonConverter attribute.");
            }

            var targetValue = (filterParam.FilterValue?.ToString() ?? string.Empty).ToLowerInvariant();
            var maxDistance = 3;

            if (property.PropertyType == typeof(LocalizationString))
            {
                return query.AsEnumerable().Where(item =>
                {
                    var propertyValue = property.GetValue(item);
                    if (propertyValue == null)
                        return false;
                        
                    var localizationString = (LocalizationString)propertyValue;
                    
                    foreach (var langKey in LocalizationString.SupportedLanguages)
                    {
                        var localizedValue = localizationString[langKey];
                        if (string.IsNullOrEmpty(localizedValue))
                            continue;
                            
                        var localizedValueLower = localizedValue.ToLowerInvariant();
                        var distance = LevenshteinHelper.ComputeLevenshteinDistance(targetValue, localizedValueLower);
                        if (distance <= maxDistance)
                        {
                            return true;
                        }
                    }
                    
                    return false;
                }).AsQueryable();
            }

            return query.AsEnumerable().Where(item =>
            {
                var propertyValue = (property.GetValue(item)?.ToString() ?? string.Empty).ToLowerInvariant();
                var distance = LevenshteinHelper.ComputeLevenshteinDistance(targetValue, propertyValue);
                return distance <= maxDistance;
            }).AsQueryable();
        }

        public static object ConvertFromString(string value, Type targetType)
        {
            var converter = TypeDescriptor.GetConverter(targetType);
            if (converter != null && converter.CanConvertFrom(typeof(string)))
            {
                var result = converter.ConvertFromInvariantString(value);
                if (targetType == typeof(DateTime))
                {
                    result = ((DateTime)result).ToUniversalTime();
                }
                return result;
            }
            throw new NotSupportedException($"Cannot convert {value} to {targetType.Name}");
        }

        private static object? ConvertToType(object? value, Type target)
        {
            if (value == null) return null;

            if (value is Newtonsoft.Json.Linq.JToken jt)
                return jt.ToObject(target);

            var t = Nullable.GetUnderlyingType(target) ?? target;

            if (t == typeof(string)) return value.ToString();
            if (t == typeof(Guid)) return value is Guid g ? g : Guid.Parse(value.ToString()!);
            if (t == typeof(DateTime))
            {
                if (value is DateTime dt) return dt;
                return DateTime.Parse(value.ToString()!, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
            }
            if (t == typeof(DateOnly))
            {
                if (value is DateOnly d) return d;
                if (value is DateTime dd) return DateOnly.FromDateTime(dd);
                return DateOnly.Parse(value.ToString()!, CultureInfo.InvariantCulture);
            }
            if (t.IsEnum) return Enum.Parse(t, value.ToString()!, true);

            return Convert.ChangeType(value, t, CultureInfo.InvariantCulture);
        }
    }
}
