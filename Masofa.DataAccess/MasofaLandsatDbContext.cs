using Masofa.Common.Models;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Landsat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Masofa.DataAccess
{
    /// <summary>
    /// Контекст данных для работы с моделями спутников семейства Landsat
    /// </summary>
    public partial class MasofaLandsatDbContext : DbContext
    {
        public MasofaLandsatDbContext(DbContextOptions<MasofaLandsatDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация индексов для оптимизации геометрических запросов

            // Индексы для StacFeatures
            modelBuilder.Entity<StacFeatureEntity>(entity =>
            {
                // Индекс для FeatureId (используется в JOIN)
                entity.HasIndex(e => e.FeatureId)
                    .HasDatabaseName("idx_stac_features_feature_id");

                // Индекс для GeometryJson (для фильтрации)
                entity.HasIndex(e => e.GeometryJson)
                    .HasDatabaseName("idx_stac_features_geometry_json")
                    .HasFilter("\"GeometryJson\" IS NOT NULL");

                // Индекс для Datetime (для партиционирования и фильтрации по дате)
                entity.HasIndex(e => e.Datetime)
                    .HasDatabaseName("idx_stac_features_datetime");

                // Составной индекс для даты и геометрии
                entity.HasIndex(e => new { e.Datetime, e.GeometryJson })
                    .HasDatabaseName("idx_stac_features_datetime_geometry")
                    .HasFilter("\"GeometryJson\" IS NOT NULL");
            });

            // Индексы для LandsatProducts
            modelBuilder.Entity<LandsatProductEntity>(entity =>
            {
                // Индекс для SatellateProductId (используется в JOIN)
                entity.HasIndex(e => e.SatellateProductId)
                    .HasDatabaseName("idx_landsat_products_satellite_id");

                // Индекс для CreateAt (для партиционирования)
                entity.HasIndex(e => e.CreateAt)
                    .HasDatabaseName("idx_landsat_products_create_at");
            });

            // Индексы для LandsatProductMetadata
            modelBuilder.Entity<LandsatProductMetadata>(entity =>
            {
                // Индекс для ProductId
                entity.HasIndex(e => e.ProductId)
                    .HasDatabaseName("idx_landsat_product_metadata_product_id");

                // Индекс для AcquisitionDate
                entity.HasIndex(e => e.AcquisitionDate)
                    .HasDatabaseName("idx_landsat_product_metadata_acquisition_date");
            });

            // Индексы для LandsatProductQueue
            modelBuilder.Entity<LandsatProductQueue>(entity =>
            {
                // Индекс для ProductId
                entity.HasIndex(e => e.ProductId)
                    .HasDatabaseName("idx_landsat_product_queue_product_id");

                // Индекс для Status
                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("idx_landsat_product_queue_status");
            });

            this.ApplyLocalizationStringSettings(modelBuilder);

        }

        public DbSet<LandsatProductEntity> LandsatProducts { get; set; }

        /// <summary>
        /// Очередь продуктов Landsat
        /// </summary>
        public DbSet<LandsatProductQueue> LandsatProductsQueue { get; set; }

        /// <summary>
        /// Метаданные продуктов Landsat
        /// </summary>
        public DbSet<LandsatProductMetadata> LandsatProductMetadata { get; set; }

        // STAC модели
        public DbSet<StacFeatureEntity> StacFeatures { get; set; }
        public DbSet<StacLinkEntity> StacLinks { get; set; }
        public DbSet<StacAssetEntity> StacAssets { get; set; }
        public DbSet<LandsatSrStacMetadataEntity> LandsatSrStacMetadatas { get; set; }
        public DbSet<LandsatStStacMetadataEntity> LandsatStStacMetadatas { get; set; }

        // STAC SR модели
        public DbSet<SrBandAssetEntity> SrBandAssets { get; set; }
        public DbSet<SrQualityAssetEntity> SrQualityAssets { get; set; }

        // STAC ST модели
        public DbSet<StTemperatureAssetEntity> StTemperatureAssets { get; set; }
        public DbSet<StAuxiliaryAssetEntity> StAuxiliaryAssets { get; set; }
        public DbSet<StQualityAssetEntity> StQualityAssets { get; set; }

        // MTL модели
        public DbSet<ProjectionAttributesEntity> ProjectionAttributes { get; set; }
        public DbSet<ProductContentsEntity> ProductContents { get; set; }
        public DbSet<Level2SurfaceTemperatureParametersEntity> Level2SurfaceTemperatureParameters { get; set; }
        public DbSet<Level2SurfaceReflectanceParametersEntity> Level2SurfaceReflectanceParameters { get; set; }
        public DbSet<Level2ProcessingRecordEntity> Level2ProcessingRecords { get; set; }
        public DbSet<Level1ThermalConstantsEntity> Level1ThermalConstants { get; set; }
        public DbSet<Level1RadiometricRescalingEntity> Level1RadiometricRescalings { get; set; }
        public DbSet<Level1ProjectionParametersEntity> Level1ProjectionParameters { get; set; }
        public DbSet<Level1ProcessingRecordEntity> Level1ProcessingRecords { get; set; }
        public DbSet<Level1MinMaxReflectanceEntity> Level1MinMaxReflectances { get; set; }
        public DbSet<Level1MinMaxRadianceEntity> Level1MinMaxRadiances { get; set; }
        public DbSet<Level1MinMaxPixelValueEntity> Level1MinMaxPixelValues { get; set; }
        public DbSet<LandsatSourceMetadataEntity> LandsatSourceMetadata { get; set; }
        public DbSet<ImageAttributesEntity> ImageAttributes { get; set; }
    }
}
