using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Sentinel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Masofa.DataAccess
{
    public class MasofaSentinelHistoryDbContext : DbContext
    {
        public MasofaSentinelHistoryDbContext(DbContextOptions<MasofaSentinelHistoryDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.ApplyLocalizationStringSettings(modelBuilder);
        }

        public DbSet<Sentinel2ProductEntityHistory> Sentinel2ProductHistories { get; set; }
        public DbSet<Sentinel2ProductQueueHistory> Sentinel2ProductsQueueHistories { get; set; }
        public DbSet<Sentinel2GenerateIndexStatusHistory> Sentinel2GenerateIndexStatusHistories { get; set; }
        public DbSet<Sentinel2ProductMetadataHistory> Sentinel2ProductsMetadataHistories { get; set; }
        public DbSet<SentinelL1CProductMetadataHistory> SentinelL1CProductMetadataHistories { get; set; }
        public DbSet<SentinelInspireMetadataHistory> SentinelInspireMetadataHistories { get; set; }
        public DbSet<SentinelL1CTileMetadataHistory> SentinelL1CTileMetadataHistories { get; set; }
        public DbSet<SentinelProductQualityMetadataHistory> SentinelProductQualityMetadataHistories { get; set; }
    }
}
