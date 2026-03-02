using Masofa.Common.Models.Notifications;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.SystemCrical;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public partial class MasofaCommonHistoryDbContext : DbContext
    {
        public MasofaCommonHistoryDbContext(DbContextOptions<MasofaCommonHistoryDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.ApplyLocalizationStringSettings(modelBuilder);
        }

        public DbSet<FileStorageItemHistory> FileStorageItemHistories { get; set; }
        public DbSet<SatelliteProductHistory> SatelliteProductHistories { get; set; }
        public DbSet<EmailMessageHistory> EmailMessageHistories { get; set; }
        public DbSet<SystemBackgroundTaskHistory> SystemBackgroundTaskHistories { get; set; }
        public DbSet<SystemBackgroundTaskResultHistory> SystemBackgroundTaskResultHistories { get; set; }
        public DbSet<UserTicketHistory> UserTicketHistories { get; set; }
        public DbSet<TagRelationHistory> TagRelationHistories { get; set; }
    }
}
