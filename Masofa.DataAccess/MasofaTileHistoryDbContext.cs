using Masofa.Common.Models.Gis;
using Masofa.Common.Models.Tiles;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public class MasofaTileHistoryDbContext : DbContext
    {
        public MasofaTileHistoryDbContext(DbContextOptions<MasofaTileHistoryDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.ApplyLocalizationStringSettings(modelBuilder);

        }

        public DbSet<TileHistory> TileHistories { get; set; }
        public DbSet<PointHistory> PointHistories { get; set; }
        public DbSet<MapMonitoringObjectHistory> MapMonitoringObjectHistories { get; set; }
    }
}
