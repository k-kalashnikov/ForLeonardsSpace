using System;
using System.Collections.Generic;
using Masofa.Depricated.DataAccess.DepricatedUalertsServerOne.Models;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Depricated.DataAccess.DepricatedUalertsServerOne;

public partial class DepricatedUalertsServerOneDbContext : DbContext
{
    public DepricatedUalertsServerOneDbContext()
    {
    }

    public DepricatedUalertsServerOneDbContext(DbContextOptions<DepricatedUalertsServerOneDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AlertList> AlertLists { get; set; }

    public virtual DbSet<Crop> Crops { get; set; }

    public virtual DbSet<Disaster> Disasters { get; set; }

    public virtual DbSet<DisasterList> DisasterLists { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<VAlertsFrozen> VAlertsFrozens { get; set; }

    public virtual DbSet<WeatherRegion> WeatherRegions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=80.80.213.218;Port=15432;Database=ualerts;Username=postgres;Password=pgPassw0rd", x => x
                .UseNodaTime()
                .UseNetTopologySuite());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgres_fdw");

        modelBuilder.Entity<AlertList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("alert_list_pkey");

            entity.ToTable("alert_list");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.CropId).HasColumnName("crop_id");
            entity.Property(e => e.InfluenceClimate).HasColumnName("influence_climate");
            entity.Property(e => e.Loss).HasColumnName("loss");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
        });

        modelBuilder.Entity<Crop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("crop_pkey");

            entity.ToTable("crop");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(64)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(64)
                .HasColumnName("name_uz");
        });

        modelBuilder.Entity<Disaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("disaster_pkey");

            entity.ToTable("disaster");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(64)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(64)
                .HasColumnName("name_uz");
        });

        modelBuilder.Entity<DisasterList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("disaster_list_pkey");

            entity.ToTable("disaster_list");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.AreaPercent).HasColumnName("area_percent");
            entity.Property(e => e.Disaster).HasColumnName("disaster");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("region");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(256)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(256)
                .HasColumnName("name_uz");
        });

        modelBuilder.Entity<VAlertsFrozen>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("v_alerts_frozen");

            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.Areapercent).HasColumnName("areapercent");
            entity.Property(e => e.DisasterId).HasColumnName("disaster_id");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
        });

        modelBuilder.Entity<WeatherRegion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("weather_regions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RegionLevel).HasColumnName("region_level");
            entity.Property(e => e.RegionName)
                .HasMaxLength(256)
                .HasColumnName("region_name");
            entity.Property(e => e.RegionNameEn)
                .HasMaxLength(256)
                .HasColumnName("region_name_en");
            entity.Property(e => e.RegionNameUz)
                .HasMaxLength(256)
                .HasColumnName("region_name_uz");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
