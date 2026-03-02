using System;
using System.Collections.Generic;
using Masofa.Depricated.DataAccess.DepricatedAuthServerTwo.Models;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Depricated.DataAccess.DepricatedAuthServerTwo;

public partial class DepricatedAuthServerTwoDbContext : DbContext
{
    public DepricatedAuthServerTwoDbContext()
    {
    }

    public DepricatedAuthServerTwoDbContext(DbContextOptions<DepricatedAuthServerTwoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public virtual DbSet<OpenIddictApplication> OpenIddictApplications { get; set; }

    public virtual DbSet<OpenIddictAuthorization> OpenIddictAuthorizations { get; set; }

    public virtual DbSet<OpenIddictScope> OpenIddictScopes { get; set; }

    public virtual DbSet<OpenIddictToken> OpenIddictTokens { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PermissionZone> PermissionZones { get; set; }

    public virtual DbSet<UserUiSetting> UserUiSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=217.29.121.2;Port=5432;Database=auth;Username=postgres;Password=W4xZ9bNmR2tY", x => x
                .UseNodaTime()
                .UseNetTopologySuite());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Custom).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasDefaultValueSql("''::text");
            entity.Property(e => e.LastName).HasDefaultValueSql("''::text");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.TaxId).HasDefaultValueSql("''::text");
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<OpenIddictApplication>(entity =>
        {
            entity.HasIndex(e => e.ClientId, "IX_OpenIddictApplications_ClientId").IsUnique();

            entity.Property(e => e.ApplicationType).HasMaxLength(50);
            entity.Property(e => e.ClientId).HasMaxLength(100);
            entity.Property(e => e.ClientType).HasMaxLength(50);
            entity.Property(e => e.ConcurrencyToken).HasMaxLength(50);
            entity.Property(e => e.ConsentType).HasMaxLength(50);
        });

        modelBuilder.Entity<OpenIddictAuthorization>(entity =>
        {
            entity.HasIndex(e => new { e.ApplicationId, e.Status, e.Subject, e.Type }, "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type");

            entity.Property(e => e.ConcurrencyToken).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Subject).HasMaxLength(400);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Application).WithMany(p => p.OpenIddictAuthorizations)
                .HasForeignKey(d => d.ApplicationId)
                .HasConstraintName("FK_OpenIddictAuthorizations_OpenIddictApplications_Application~");
        });

        modelBuilder.Entity<OpenIddictScope>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_OpenIddictScopes_Name").IsUnique();

            entity.Property(e => e.ConcurrencyToken).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<OpenIddictToken>(entity =>
        {
            entity.HasIndex(e => new { e.ApplicationId, e.Status, e.Subject, e.Type }, "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type");

            entity.HasIndex(e => e.AuthorizationId, "IX_OpenIddictTokens_AuthorizationId");

            entity.HasIndex(e => e.ReferenceId, "IX_OpenIddictTokens_ReferenceId").IsUnique();

            entity.Property(e => e.ConcurrencyToken).HasMaxLength(50);
            entity.Property(e => e.ReferenceId).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Subject).HasMaxLength(400);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Application).WithMany(p => p.OpenIddictTokens).HasForeignKey(d => d.ApplicationId);

            entity.HasOne(d => d.Authorization).WithMany(p => p.OpenIddictTokens).HasForeignKey(d => d.AuthorizationId);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions", "permissions");

            entity.HasIndex(e => new { e.Name, e.OwnerId, e.Type }, "IX_Permissions_Name_OwnerId_Type").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<PermissionZone>(entity =>
        {
            entity.ToTable("PermissionZones", "permissions");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<UserUiSetting>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
