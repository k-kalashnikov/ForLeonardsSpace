using Masofa.Common.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextExtention
    {
        public static bool IsTableExists(this DbContext dbContext, string tableName)
        {
            var connection = dbContext.Database.GetDbConnection();
            try
            {
                connection.Open();
                using var command = connection.CreateCommand();

                // Для PostgreSQL
                command.CommandText = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public' AND table_name = '{tableName}'";


                var result = command.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }

        public static void ApplyLocalizationStringSettings(this DbContext dbContext, ModelBuilder modelBuilder)
        {
            var stringConverter = new ValueConverter<LocalizationString, string>(
                v => v.ValuesJson,
                v => new LocalizationString { ValuesJson = v }
            );

            var fileStorageConverter = new ValueConverter<LocalizationFileStorageItem, string>(
                v => v.ValuesJson,
                v => new LocalizationFileStorageItem { ValuesJson = v }
            );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetRuntimeProperties())
                {
                    if (property.Value.PropertyType == typeof(LocalizationString))
                    {
                        modelBuilder.Entity(entityType.ClrType)
                            .Property(property.Value.Name)
                            .HasConversion(stringConverter)
                            .HasColumnType("text");
                    }
                    else if (property.Value.PropertyType == typeof(LocalizationFileStorageItem))
                    {
                        modelBuilder.Entity(entityType.ClrType)
                            .Property(property.Value.Name)
                            .HasConversion(fileStorageConverter)
                            .HasColumnType("text");
                    }
                }
            }
        }

        public static void CreatePartitionForDateAsync(this DbContext dbContext, string modelName, DateOnly date)
        {

            var tableName = $"{modelName}_{date:yyyy_MM_dd}";
            var nextDay = date.AddDays(1);
            if (dbContext.IsTableExists(tableName))
            {
                return;
            }

            var sql = $"CREATE TABLE \"{tableName}\" PARTITION OF \"{modelName}\" FOR VALUES FROM ('{date:yyyy-MM-dd}') TO ('{nextDay:yyyy-MM-dd}');";

            dbContext.Database.ExecuteSqlRaw(sql);
        }

        public static void CreatePartitionForDateAsync(this DbContext dbContext, Type modelType, DateOnly date)
        {
            var entryType = dbContext.Model.FindEntityType(modelType);
            CreatePartitionForDateAsync(dbContext, entryType.GetTableName(), date);
        }

        public static IQueryable GetQueryableSet(this DbContext context, string entityName)
        {
            var entityType = context.Model.GetEntityTypes().Select(e => e.ClrType).FirstOrDefault(t => t.Name == entityName)
                ?? throw new ArgumentException($"Entity '{entityName}' not found in model.");

            var setMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes)!.MakeGenericMethod(entityType);

            var result = setMethod?.Invoke(context, null);

            return (IQueryable)result!;
        }
    }
}
