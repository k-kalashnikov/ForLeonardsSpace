using Masofa.Common.Models.MasofaAnaliticReport;
using Masofa.Common.Models.Reports;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public class MasofaAnaliticReportDbContext : DbContext
    {
        public MasofaAnaliticReportDbContext(DbContextOptions<MasofaAnaliticReportDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.ApplyLocalizationStringSettings(modelBuilder);
        }

        public DbSet<FarmerReport> FarmerReports { get; set; }
        public DbSet<FarmerRecomendationReport> FarmerRecomendationReports { get; set; }
    }
}
