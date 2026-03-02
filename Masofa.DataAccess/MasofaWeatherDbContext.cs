using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.Weather;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Masofa.DataAccess
{
    public class MasofaWeatherDbContext : DbContext
    {
        public MasofaWeatherDbContext(DbContextOptions<MasofaWeatherDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<AgroClimaticZoneMonthNorm> AgroClimaticZoneMonthNorms { get; set; }
        public DbSet<AgroClimaticZoneNorm> AgroClimaticZoneNorms { get; set; }
        public DbSet<AgroClimaticZonesWeatherRate> AgroClimaticZonesWeatherRates { get; set; }
        public DbSet<ApplicationProperty> ApplicationPropertys { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Common.Models.Weather.Region> Regions { get; set; }
        public DbSet<RegionsAgroClimaticZone> RegionsAgroClimaticZones { get; set; }
        public DbSet<RegionsDump> RegionsDumps { get; set; }
        public DbSet<RegionsWeather> RegionsWeathers { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<WeatherStationAgroClimaticZone> WeatherStationAgroClimaticZones { get; set; }
        public DbSet<WeatherStationsDataEx> WeatherStationsDataEx { get; set; }
        public DbSet<WeatherStationsDatum> WeatherStationsDatum { get; set; }
        public DbSet<XslsUzUnputColumn> XslsUzUnputColumns { get; set; }
        public DbSet<Masofa.Common.Models.Weather.WeatherStation> WeatherStations { get; set; }
        public DbSet<Masofa.Common.Models.Weather.AgroClimaticZone> AgroClimaticZones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.ApplyLocalizationStringSettings(modelBuilder);

        }
    }
}
