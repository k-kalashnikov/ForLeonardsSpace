using Masofa.Common.Models.CropMonitoring;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public class MasofaCropMonitoringHistoryDbContext : DbContext
    {
        public MasofaCropMonitoringHistoryDbContext(DbContextOptions<MasofaCropMonitoringHistoryDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.ApplyLocalizationStringSettings(modelBuilder);
        }

        public DbSet<BidHistory> BidHistories { get; set; }
        public DbSet<BidTemplateHistory> BidTemplateHistories { get; set; }
        public DbSet<FieldHistory> FieldHistories { get; set; }
        public DbSet<FieldAgroOperationHistory> FieldAgroOperationHistories { get; set; }
        public DbSet<FieldAgroProducerHistoryHistory> FieldAgroProducerHistoryHistories { get; set; }
        public DbSet<FieldInsuranceHistoryHistory> FieldInsuranceHistoryHistories { get; set; }
        public DbSet<SeasonHistory> SeasonHistories { get; set; }
        public DbSet<SoilDataHistory> SoilDataHistories { get; set; }
        public DbSet<ImportedFieldHistory> ImportedFieldHistories { get; set; }
        public DbSet<ImportedFieldReportHistory> ImportedFieldReportHistories { get; set; }
    }
}
