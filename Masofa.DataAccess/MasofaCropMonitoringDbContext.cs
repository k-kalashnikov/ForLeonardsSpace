using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Satellite;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public class MasofaCropMonitoringDbContext : DbContext
    {
        public MasofaCropMonitoringDbContext(DbContextOptions<MasofaCropMonitoringDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Masofa.Common.Models.CropMonitoring.Bid> Bids { get; set; }
        public DbSet<Masofa.Common.Models.CropMonitoring.BidTemplate> BidTemplates { get; set; }
        public DbSet<Masofa.Common.Models.CropMonitoring.Field> Fields { get; set; }
        public DbSet<Masofa.Common.Models.CropMonitoring.FieldPhoto> FieldPhotos { get; set; }
        public DbSet<Masofa.Common.Models.CropMonitoring.Season> Seasons { get; set; }
        public DbSet<Masofa.Common.Models.CropMonitoring.SoilData> SoilDatas { get; set; }
        public DbSet<SatelliteSearchConfig> SatelliteSearchConfigs { get; set; }
        public DbSet<FieldProductMapping> FieldProductMappings { get; set; }
        public DbSet<FieldAgroOperation> FieldAgroOperations { get; set; }
        public DbSet<FieldAgroProducerHistory> FieldAgroProducerHistories { get; set; }
        public DbSet<FieldInsuranceHistory> FieldInsuranceHistories { get; set; }
        public DbSet<FieldInsuranceCase> FieldInsuranceCases { get; set; }
        public DbSet<FieldFinancialCondition> FieldFinancialConditions { get; set; }
        public DbSet<FieldEncumbrance> FieldEncumbrances { get; set; }
        public DbSet<FieldSoilProfileAnalysis> FieldSoilProfileAnalyses { get; set; }
        public DbSet<FieldIrrigationData> FieldIrrigationDatas { get; set; }
        public DbSet<ImportedField> ImportedFields { get; set; }
        public DbSet<ImportedFieldReport> ImportedFieldReports { get; set; }
        public DbSet<ImportedFieldLog> ImportedFieldLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<long>("BidNumberSequence")
                .StartsAt(1)
                .IncrementsBy(1);

            modelBuilder.Entity<Bid>(entity =>
            {
                entity.Property(e => e.Number)
                    .HasDefaultValueSql("NEXT VALUE FOR BidNumberSequence");
            });

            base.OnModelCreating(modelBuilder);
            // Конфигурация для геометрических типов SatelliteSearchConfig
            modelBuilder.Entity<SatelliteSearchConfig>()
                .Property(e => e.SentinelPolygon)
                .HasColumnType("geometry");
            modelBuilder.Entity<SatelliteSearchConfig>()
                .Property(e => e.LandsatLeftDown)
                .HasColumnType("geometry");
            modelBuilder.Entity<SatelliteSearchConfig>()
                .Property(e => e.LandsatRightUp)
                .HasColumnType("geometry");
            // Индекс для активных конфигураций
            modelBuilder.Entity<SatelliteSearchConfig>()
                .HasIndex(e => e.IsActive);
            // Составной индекс на (IsActive, CreateAt)
            modelBuilder.Entity<SatelliteSearchConfig>()
                .HasIndex(e => new { e.IsActive, e.CreateAt });
            // Конфигурация для FieldProductMapping
            modelBuilder.Entity<FieldProductMapping>(entity =>
            {
                // Индекс для быстрого поиска по полю
                entity.HasIndex(e => e.FieldId)
                    .HasDatabaseName("idx_field_product_mapping_field_id");
                // Индекс для поиска по продукту
                entity.HasIndex(e => e.ProductId)
                    .HasDatabaseName("idx_field_product_mapping_product_id");
                // Индекс для фильтрации по типу спутника
                entity.HasIndex(e => e.SatelliteType)
                    .HasDatabaseName("idx_field_product_mapping_satellite_type");
                // Составной индекс для быстрого поиска
                entity.HasIndex(e => new { e.FieldId, e.SatelliteType })
                    .HasDatabaseName("idx_field_product_mapping_field_satellite");
                // Уникальность связи поле-продукт
                entity.HasIndex(e => new { e.FieldId, e.ProductId, e.SatelliteType })
                    .IsUnique()
                    .HasDatabaseName("uk_field_product_mapping_field_product");
            });
            // Конфигурация для геометрических типов
            modelBuilder.Entity<Field>()
                .Property(e => e.Polygon)
                .HasColumnType("geometry");
            // Конфигурация индексов для оптимизации геометрических запросов
            // Индексы для Fields
            modelBuilder.Entity<Field>(entity =>
            {
                // GIST индекс для геометрии полигона
                entity.HasIndex(e => e.Polygon)
                    .HasDatabaseName("idx_fields_polygon")
                    .HasMethod("GIST");
                // Индекс для RegionId (для фильтрации по региону)
                entity.HasIndex(e => e.RegionId)
                    .HasDatabaseName("idx_fields_region_id");
                // Индекс для Name (для поиска по названию)
                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("idx_fields_name");
                // Индекс для ExternalId (для внешних идентификаторов)
                entity.HasIndex(e => e.ExternalId)
                    .HasDatabaseName("idx_fields_external_id");
                // Индекс для CreateAt (для партиционирования)
                entity.HasIndex(e => e.CreateAt)
                    .HasDatabaseName("idx_fields_create_at");
            });

            // Индексы для новых сущностей паспорта поля
            modelBuilder.Entity<FieldInsuranceCase>(entity =>
            {
                entity.HasIndex(e => e.FieldId).HasDatabaseName("idx_field_insurance_case_field_id");
                entity.HasIndex(e => e.PaymentDate).HasDatabaseName("idx_field_insurance_case_payment_date");
            });

            modelBuilder.Entity<FieldFinancialCondition>(entity =>
            {
                entity.HasIndex(e => e.FieldId).HasDatabaseName("idx_field_financial_condition_field_id");
                entity.HasIndex(e => new { e.StartDate, e.EndDate }).HasDatabaseName("idx_field_financial_condition_dates");
            });

            modelBuilder.Entity<FieldEncumbrance>(entity =>
            {
                entity.HasIndex(e => e.FieldId).HasDatabaseName("idx_field_encumbrance_field_id");
                entity.HasIndex(e => new { e.StartDate, e.EndDate }).HasDatabaseName("idx_field_encumbrance_dates");
            });

            modelBuilder.Entity<FieldSoilProfileAnalysis>(entity =>
            {
                entity.HasIndex(e => e.FieldId).HasDatabaseName("idx_field_soil_profile_field_id");
                entity.HasIndex(e => e.AnalysisDate).HasDatabaseName("idx_field_soil_profile_analysis_date");
                entity.Property(e => e.Coordinates).HasColumnType("geometry");
            });

            modelBuilder.Entity<FieldIrrigationData>(entity =>
            {
                entity.HasIndex(e => e.FieldId).HasDatabaseName("idx_field_irrigation_data_field_id");
                entity.HasIndex(e => e.Year).HasDatabaseName("idx_field_irrigation_data_year");
            });

            this.ApplyLocalizationStringSettings(modelBuilder);

        }
    }
}
