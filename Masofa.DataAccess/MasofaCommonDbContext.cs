using Masofa.Common.Models.Notifications;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Models.SystemDocumentation;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public partial class MasofaCommonDbContext : DbContext
    {
        public MasofaCommonDbContext(DbContextOptions<MasofaCommonDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            this.ApplyLocalizationStringSettings(modelBuilder);
        }

        public DbSet<FileStorageItem> FileStorageItems { get; set; }
        public DbSet<SatelliteProduct> SatelliteProducts { get; set; }
        public DbSet<EmailMessage> EmailMessages { get; set; }
        public DbSet<SystemBackgroundTask> SystemBackgroundTasks { get; set; }
        public DbSet<SystemBackgroundTaskResult> SystemBackgroundTaskResults { get; set; }
        public DbSet<LogMessage> LogMessages { get; set; }
        public DbSet<CallStack> CallStacks { get; set; }
        public DbSet<UserTicket> UserTickets { get; set; }
        public DbSet<UserTicketMessage> UserTicketMessages { get; set; }
        public DbSet<HealthCheckResult> HealthCheckResults { get; set; }
        public DbSet<TagRelation> TagRelations { get; set; }
        public DbSet<SystemDocumentation> SystemDocumentations { get; set; }
        public DbSet<AccessMapItem> AccessMapItems { get; set; }
        public DbSet<SatelliteRegionRelation> SatelliteRegionRelations { get; set; }
    }
}
