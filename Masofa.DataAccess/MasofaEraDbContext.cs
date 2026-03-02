using Masofa.Common.Models.Era;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public partial class MasofaEraDbContext : DbContext
    {
        public MasofaEraDbContext(DbContextOptions<MasofaEraDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.ApplyLocalizationStringSettings(modelBuilder);
        }

        public DbSet<EraWeatherData> EraWeatherData { get; set; }
        public DbSet<EraWeatherStation> EraWeatherStations { get; set; }
        public DbSet<Era5DayWeatherForecast> Era5DayWeatherForecasts { get; set; }
        public DbSet<Era5DayWeatherReport> Era5DayWeatherReports { get; set; }
        public DbSet<Era5MonthWeatherReport> Era5MonthWeatherReports { get; set; }
        public DbSet<Era5WeekWeatherReport> Era5WeekWeatherReports { get; set; }
        public DbSet<Era5YearWeatherReport> Era5YearWeatherReports { get; set; }
        public DbSet<Era5DayNormalizedWeather> Era5DayNormalizedWeather { get; set; }
    }
}