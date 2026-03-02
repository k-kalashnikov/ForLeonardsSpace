using Masofa.Common.Models.Weather;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public class MasofaWeatherHistoryDbContext : DbContext
    {
        public MasofaWeatherHistoryDbContext(DbContextOptions<MasofaWeatherHistoryDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.ApplyLocalizationStringSettings(modelBuilder);

        }

        public DbSet<AgroClimaticZoneMonthNormHistory> AgroClimaticZoneMonthNormHistories { get; set; }
        public DbSet<AgroClimaticZoneNormHistory> AgroClimaticZoneNormHistories { get; set; }

    }
}
