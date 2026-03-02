using Masofa.Common.Models.Ugm;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public partial class MasofaUgmDbContext : DbContext
    {
        public MasofaUgmDbContext(DbContextOptions<MasofaUgmDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.ApplyLocalizationStringSettings(modelBuilder);
        }

        public DbSet<UgmWeatherStation> UgmWeatherStations { get; set; }
        public DbSet<UgmWeatherData> UgmWeatherData { get; set; }
    }
}
