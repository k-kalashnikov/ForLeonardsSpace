using Masofa.Common.Models.IBMWeather;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    /// <summary>
    /// Контекст данных для работы с моделями погоды с коннектора IBM
    /// </summary>
    public partial class MasofaIBMWeatherDbContext : DbContext
    {
        public MasofaIBMWeatherDbContext(DbContextOptions<MasofaIBMWeatherDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<IBMMeteoStation> IBMMeteoStations { get; set; } = default!;
        public DbSet<IBMWeatherData> IBMWeatherData { get; set; } = default!;
        public DbSet<IBMWeatherAlertFloodInfo> IBMWeatherAlertFloodInfos { get; set; } = default!;
        public DbSet<IBMWeatherAlert> IBMWeatherAlerts { get; set; } = default!;
        public DbSet<IbmDayWeatherForecast> IbmDayWeatherForecasts { get; set; } = default!;
        public DbSet<IbmDayNormalizedWeather> IbmDayNormalizedWeathers { get; set; } = default!;
        public DbSet<IbmDayWeatherReport> IbmDayWeatherReports { get; set; } = default!;
        public DbSet<IbmMonthWeatherReport> IbmMonthWeatherReports { get; set; } = default!;
        public DbSet<IbmWeekWeatherReport> IbmWeekWeatherReports { get; set; } = default!;
        public DbSet<IbmYearWeatherReport> IbmYearWeatherReports { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.ApplyLocalizationStringSettings(modelBuilder);
        }
    }
}
