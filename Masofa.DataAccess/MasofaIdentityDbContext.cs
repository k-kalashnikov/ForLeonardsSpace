using Masofa.Common.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public class MasofaIdentityDbContext : IdentityDbContext<User, Role, Guid>
    {
        public MasofaIdentityDbContext(DbContextOptions<MasofaIdentityDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<LockPermission> LockPermissions { get; set; }
        public DbSet<OneIdUser> OneIdUsers { get; set; }
        public DbSet<OneIdSystemInfo> OneIdSystemInfos { get; set; }
        public DbSet<OneIdRoleInfo> OneIdRoleInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            this.ApplyLocalizationStringSettings(builder);

        }
    }
}
