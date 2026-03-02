using Masofa.Common.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public class MasofaIdentityHistoryDbContext : DbContext
    {
        public MasofaIdentityHistoryDbContext(DbContextOptions<MasofaIdentityHistoryDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            this.ApplyLocalizationStringSettings(builder);

        }

        public DbSet<UserDeviceHistory> UserDeviceHistories { get; set; }
        public DbSet<LockPermissionHistory> LockPermissionHistories { get; set; }
    }
}
