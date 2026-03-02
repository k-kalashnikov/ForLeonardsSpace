using Masofa.Common.Models.MasofaAnaliticReport;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public class MasofaAnaliticReportHistoryDbContext : DbContext
    {
        public MasofaAnaliticReportHistoryDbContext(DbContextOptions<MasofaAnaliticReportHistoryDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.ApplyLocalizationStringSettings(modelBuilder);
        }

        public DbSet<FarmerRecomendationReportModelHistory> FarmerRecomendationReportModelHistories { get; set; }
    }
}
