using Masofa.Common.Models.Gis;
using Masofa.Common.Models.Tiles;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public class MasofaTileDbContext : DbContext
    {
        public MasofaTileDbContext(DbContextOptions<MasofaTileDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Tile> Tiles { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<MapMonitoringObject> MapMonitoringObjects { get; set; }
        public DbSet<TileLayer> TileLayers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.ApplyLocalizationStringSettings(modelBuilder);

        }
    }
}