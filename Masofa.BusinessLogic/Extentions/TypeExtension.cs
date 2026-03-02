using Masofa.Common.Models;
using Masofa.DataAccess;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Masofa.BusinessLogic.Extentions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Находит тип DbContext, который содержит DbSet для указанного типа сущности.
        /// </summary>
        /// <param name="entityType">Тип сущности (например, AdministrativeUnitHistory)</param>
        /// <returns>Тип DbContext или null, если не найден</returns>
        public static Type? GetDbContextTypeForEntity(this Type? entityType)
        {
            if (entityType == null)
                return null;

            var assembly = typeof(MasofaIdentityDbContext).Assembly;

            var dbContextTypes = assembly.GetTypes()
                .Where(t => t.IsClass &&
                            !t.IsAbstract &&
                            t.IsSubclassOf(typeof(DbContext)));

            foreach (var dbContextType in dbContextTypes)
            {
                var dbSetProps = dbContextType.GetProperties()
                    .Where(p => p.PropertyType.IsGenericType &&
                                p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .Select(p => p.PropertyType.GetGenericArguments()[0]);

                if (dbSetProps.Contains(entityType))
                {
                    return dbContextType;
                }
            }

            return null;
        }

        /// <summary>
        /// Возвращает пример значения для типа (например, "text" для string, 0 для int)
        /// </summary>
        /// <param name="type">Тип</param>
        /// <returns>Пример значения или null</returns>
        public static object? GetExampleValue(this Type? type)
        {
            if (type == null)
            {
                return null;
            }

            if (type == typeof(LocalizationString))
            {
                return "text";
            }

            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            if (underlyingType == typeof(Guid))
            {
                return Guid.NewGuid().ToString();
            }

            if (underlyingType.IsEnum)
            {
                return Enum.GetNames(underlyingType).FirstOrDefault() ?? "value";
            }

            return Type.GetTypeCode(underlyingType) switch
            {
                TypeCode.String => "text",
                TypeCode.Int32 => 0,
                TypeCode.Int16 => (short)0,
                TypeCode.Int64 => 0L,
                TypeCode.Decimal => 0.0m,
                TypeCode.Double => 0.0,
                TypeCode.Single => 0.0f,
                TypeCode.Boolean => false,
                TypeCode.DateTime => DateTime.Today.ToString("yyyy-MM-dd"),
                TypeCode.Empty => null,
                TypeCode.Object => "value",
                _ => "value"
            };
        }
    }
}
