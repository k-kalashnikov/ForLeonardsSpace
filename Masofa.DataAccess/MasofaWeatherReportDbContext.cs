//using Masofa.Common.Models.Era;
//using Microsoft.EntityFrameworkCore;

//namespace Masofa.DataAccess
//{
//    public class MasofaWeatherReportDbContext : DbContext
//    {
//        public MasofaWeatherReportDbContext(DbContextOptions<MasofaWeatherReportDbContext> options) : base(options)
//        {
//            ChangeTracker.LazyLoadingEnabled = false;
//        }

//        public DbSet<Era5DayWeatherForecast> Era5DayWeatherForecasts { get; set; }
//        public DbSet<Era5DayWeatherReport> Era5DayWeatherReports { get; set; }
//        public DbSet<Era5HourWeatherForecast> Era5HourWeatherForecasts { get; set; }
//        public DbSet<Era5HourWeatherReport> Era5HourWeatherReports { get; set; }
//        public DbSet<Era5MonthWeatherReport> Era5MonthWeatherReports { get; set; }
//        public DbSet<Era5WeekWeatherReport> Era5WeekWeatherReports { get; set; }
//        public DbSet<Era5YearWeatherReport> Era5YearWeatherReports { get; set; }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            this.ApplyLocalizationStringSettings(modelBuilder);

//        }
//    }
//}