using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Landsat;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public partial class MasofaLandsatHistoryDbContext : DbContext
    {
        public MasofaLandsatHistoryDbContext(DbContextOptions<MasofaLandsatHistoryDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.ApplyLocalizationStringSettings(modelBuilder);
        }

        public DbSet<LandsatProductEntityHistory> LandsatProductHistories { get; set; }
        public DbSet<LandsatProductQueueHistory> LandsatProductsQueueHistories { get; set; }
        public DbSet<LandsatProductMetadataHistory> LandsatProductMetadataHistories { get; set; }
        public DbSet<StacFeatureEntityHistory> StacFeatureHistories { get; set; }
        public DbSet<StacLinkEntityHistory> StacLinkHistories { get; set; }
        public DbSet<StacAssetEntityHistory> StacAssetHistories { get; set; }
        public DbSet<LandsatSrStacMetadataEntityHistory> LandsatSrStacMetadataHistories { get; set; }
        public DbSet<LandsatStStacMetadataEntityHistory> LandsatStStacMetadataHistories { get; set; }
        public DbSet<SrBandAssetEntityHistory> SrBandAssetHistories { get; set; }
        public DbSet<SrQualityAssetEntityHistory> SrQualityAssetHistories { get; set; }
        public DbSet<StTemperatureAssetEntityHistory> StTemperatureAssetHistories { get; set; }
        public DbSet<StAuxiliaryAssetEntityHistory> StAuxiliaryAssetHistories { get; set; }
        public DbSet<StQualityAssetEntityHistory> StQualityAssetHistories { get; set; }
        public DbSet<ProjectionAttributesEntityHistory> ProjectionAttributeHistories { get; set; }
        public DbSet<ProductContentsEntityHistory> ProductContentHistories { get; set; }
        public DbSet<Level2SurfaceTemperatureParametersEntityHistory> Level2SurfaceTemperatureParameterHistories { get; set; }
        public DbSet<Level2SurfaceReflectanceParametersEntityHistory> Level2SurfaceReflectanceParameterHistories { get; set; }
        public DbSet<Level2ProcessingRecordEntityHistory> Level2ProcessingRecordHistories { get; set; }
        public DbSet<Level1ThermalConstantsEntityHistory> Level1ThermalConstantHistories { get; set; }
        public DbSet<Level1RadiometricRescalingEntityHistory> Level1RadiometricRescalingHistories { get; set; }
        public DbSet<Level1ProjectionParametersEntityHistory> Level1ProjectionParameterHistories { get; set; }
        public DbSet<Level1ProcessingRecordEntityHistory> Level1ProcessingRecordHistories { get; set; }
        public DbSet<Level1MinMaxReflectanceEntityHistory> Level1MinMaxReflectanceHistories { get; set; }
        public DbSet<Level1MinMaxRadianceEntityHistory> Level1MinMaxRadianceHistories { get; set; }
        public DbSet<Level1MinMaxPixelValueEntityHistory> Level1MinMaxPixelValueHistories { get; set; }
        public DbSet<LandsatSourceMetadataEntityHistory> LandsatSourceMetadataHistories { get; set; }
        public DbSet<ImageAttributesEntityHistory> ImageAttributeHistories { get; set; }
    }
}
