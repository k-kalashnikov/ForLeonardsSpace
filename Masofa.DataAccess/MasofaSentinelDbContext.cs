using Masofa.Common.Models;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Landsat;
using Masofa.Common.Models.Satellite.Sentinel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Masofa.DataAccess
{
    /// <summary>
    /// Контекст данных для работы с моделями спутников семейства Sentinel
    /// </summary>
    public partial class MasofaSentinelDbContext : DbContext
    {
        public MasofaSentinelDbContext(DbContextOptions<MasofaSentinelDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация индексов для оптимизации геометрических запросов

            // Индексы для Sentinel2Products
            modelBuilder.Entity<Sentinel2ProductEntity>(entity =>
            {
                // Индекс для SatellateProductId (используется в JOIN)
                entity.HasIndex(e => e.SatellateProductId)
                    .HasDatabaseName("idx_sentinel2_products_satellite_id");

                // Индекс для SentinelInspireMetadataId (используется в JOIN)
                entity.HasIndex(e => e.SentinelInspireMetadataId)
                    .HasDatabaseName("idx_sentinel2_products_inspire_id");

                // Индекс для CreateAt (для партиционирования)
                entity.HasIndex(e => e.CreateAt)
                    .HasDatabaseName("idx_sentinel2_products_create_at");
            });

            // Индексы для SentinelInspireMetadata
            modelBuilder.Entity<SentinelInspireMetadata>(entity =>
            {
                // Составной индекс для bounding box координат
                entity.HasIndex(e => new { e.WestBoundLongitude, e.EastBoundLongitude, e.SouthBoundLatitude, e.NorthBoundLatitude })
                    .HasDatabaseName("idx_sentinel_inspire_bbox");

                // Индекс для DateStamp (для фильтрации по дате)
                entity.HasIndex(e => e.DateStamp)
                    .HasDatabaseName("idx_sentinel_inspire_datestamp");

                // Индекс для FileIdentifier
                entity.HasIndex(e => e.FileIdentifier)
                    .HasDatabaseName("idx_sentinel_inspire_file_identifier");
            });

            // Индексы для Sentinel2ProductQueue
            modelBuilder.Entity<Sentinel2ProductQueue>(entity =>
            {
                // Индекс для ProductId
                entity.HasIndex(e => e.ProductId)
                    .HasDatabaseName("idx_sentinel2_product_queue_product_id");

                // Индекс для Status
                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("idx_sentinel2_product_queue_status");

                // Индекс для CreateAt
                entity.HasIndex(e => e.CreateAt)
                    .HasDatabaseName("idx_sentinel2_product_queue_create_at");
            });

            // Индексы для Sentinel2ProductMetadata
            modelBuilder.Entity<Sentinel2ProductMetadata>(entity =>
            {
                // Индекс для ProductId
                entity.HasIndex(e => e.ProductId)
                    .HasDatabaseName("idx_sentinel2_product_metadata_product_id");

                // Индекс для ContentDateStart
                entity.HasIndex(e => e.ContentDateStart)
                    .HasDatabaseName("idx_sentinel2_product_metadata_content_date_start");

                // Индекс для ContentDateEnd
                entity.HasIndex(e => e.ContentDateEnd)
                    .HasDatabaseName("idx_sentinel2_product_metadata_content_date_end");

                // Составной индекс для дат съемки
                entity.HasIndex(e => new { e.ContentDateStart, e.ContentDateEnd })
                    .HasDatabaseName("idx_sentinel2_product_metadata_content_dates");
            });

            // Индексы для SentinelL1CProductMetadata
            modelBuilder.Entity<SentinelL1CProductMetadata>(entity =>
            {
                // Индекс для ProductStartTime
                entity.HasIndex(e => e.ProductStartTime)
                    .HasDatabaseName("idx_sentinel_l1c_product_metadata_start_time");

                // Индекс для ProductStopTime
                entity.HasIndex(e => e.ProductStopTime)
                    .HasDatabaseName("idx_sentinel_l1c_product_metadata_stop_time");

                // Индекс для GenerationTime
                entity.HasIndex(e => e.GenerationTime)
                    .HasDatabaseName("idx_sentinel_l1c_product_metadata_generation_time");
            });

            this.ApplyLocalizationStringSettings(modelBuilder);

        }

        public DbSet<Sentinel2ProductEntity> Sentinel2Products { get; set; }

        /// <summary>
        /// Очередь продуктов Sentinel-2
        /// </summary>
        public DbSet<Sentinel2ProductQueue> Sentinel2ProductsQueue { get; set; }

        /// <summary>
        /// Статус генерации 
        /// </summary>
        public DbSet<Sentinel2GenerateIndexStatus> Sentinel2GenerateIndexStatus { get; set; }

        /// <summary>
        /// Метаданные продуктов Sentinel-2
        /// </summary>
        public DbSet<Sentinel2ProductMetadata> Sentinel2ProductsMetadata { get; set; }

        /// <summary>
        /// Метаданные продуктов Sentinel-2 L1C
        /// </summary>
        public DbSet<SentinelL1CProductMetadata> SentinelL1CProductMetadata { get; set; }

        /// <summary>
        /// Метаданные Inspire Sentinel
        /// </summary>
        public DbSet<SentinelInspireMetadata> SentinelInspireMetadata { get; set; }

        /// <summary>
        /// Метаданные тайлов Sentinel-2 L1C
        /// </summary>
        public DbSet<SentinelL1CTileMetadata> SentinelL1CTileMetadata { get; set; }

        /// <summary>
        /// Метаданные качества продуктов Sentinel
        /// </summary>
        public DbSet<SentinelProductQualityMetadata> SentinelProductQualityMetadata { get; set; }
    }
}
