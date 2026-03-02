using System;
using System.Collections.Generic;
using Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo.Models;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Depricated.DataAccess.DepricatedUmapiServerTwo;

public partial class DepricatedUmapiServerTwoDbContext : DbContext
{
    public DepricatedUmapiServerTwoDbContext()
    {
    }

    public DepricatedUmapiServerTwoDbContext(DbContextOptions<DepricatedUmapiServerTwoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bid> Bids { get; set; }

    public virtual DbSet<BidContent> BidContents { get; set; }

    public virtual DbSet<BidFile> BidFiles { get; set; }

    public virtual DbSet<BidState> BidStates { get; set; }

    public virtual DbSet<BidType> BidTypes { get; set; }

    public virtual DbSet<Crop> Crops { get; set; }

    public virtual DbSet<DataType> DataTypes { get; set; }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<ForeginUalertsCrop> ForeginUalertsCrops { get; set; }

    public virtual DbSet<ForeginWeatherRegion> ForeginWeatherRegions { get; set; }

    public virtual DbSet<Pest> Pests { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Regions3> Regions3s { get; set; }

    public virtual DbSet<SoilType> SoilTypes { get; set; }

    public virtual DbSet<Templ> Templs { get; set; }

    public virtual DbSet<TemplBlock> TemplBlocks { get; set; }

    public virtual DbSet<TemplControl> TemplControls { get; set; }

    public virtual DbSet<TemplStep> TemplSteps { get; set; }

    public virtual DbSet<Template> Templates { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    public virtual DbSet<WateringType> WateringTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=217.29.121.2;Port=5432;Database=umapi;Username=postgres;Password=W4xZ9bNmR2tY", x => x
                .UseNodaTime()
                .UseNetTopologySuite());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgres_fdw");

        modelBuilder.Entity<Bid>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bids_pkey");

            entity.ToTable("bids");

            entity.HasIndex(e => e.Active, "ix_bids_active");

            entity.HasIndex(e => e.Cancelled, "ix_bids_cancelled");

            entity.HasIndex(e => e.ContentId, "ix_bids_content");

            entity.HasIndex(e => e.CreateUser, "ix_bids_create_user");

            entity.HasIndex(e => new { e.CreateUser, e.CreateDate }, "ix_bids_create_user_date");

            entity.HasIndex(e => e.CropId, "ix_bids_crop");

            entity.HasIndex(e => e.FieldId, "ix_bids_field");

            entity.HasIndex(e => e.ForemanId, "ix_bids_foreman");

            entity.HasIndex(e => e.Published, "ix_bids_published");

            entity.HasIndex(e => e.RegionId, "ix_bids_region");

            entity.HasIndex(e => e.StartDate, "ix_bids_start_date");

            entity.HasIndex(e => e.WorkerId, "ix_bids_worker");

            entity.HasIndex(e => e.ParentId, "ix_parent");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.BidStateId)
                .HasDefaultValueSql("'b26966f4-367b-458b-9d1e-bd27f85220a9'::uuid")
                .HasColumnName("bid_state_id");
            entity.Property(e => e.BidTypeId)
                .HasDefaultValueSql("'04bd0dee-01c3-460d-94df-a799033af05a'::uuid")
                .HasColumnName("bid_type_id");
            entity.Property(e => e.Cancelled)
                .HasDefaultValue(false)
                .HasColumnName("cancelled");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.ContentId).HasColumnName("content_id");
            entity.Property(e => e.ContourId)
                .HasMaxLength(256)
                .HasColumnName("contour_id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser).HasColumnName("create_user");
            entity.Property(e => e.CropId).HasColumnName("crop_id");
            entity.Property(e => e.DeadlineDate).HasColumnName("deadline_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FieldId).HasColumnName("field_id");
            entity.Property(e => e.FieldPlantingDate).HasColumnName("field_planting_date");
            entity.Property(e => e.ForemanId).HasColumnName("foreman_id");
            entity.Property(e => e.GeoJson).HasColumnName("geo_json");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Lng).HasColumnName("lng");
            entity.Property(e => e.ModifyDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser).HasColumnName("modify_user");
            entity.Property(e => e.Number)
                .ValueGeneratedOnAdd()
                .HasColumnName("number");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Published).HasColumnName("published");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.VarietyId).HasColumnName("variety_id");
            entity.Property(e => e.WorkerId).HasColumnName("worker_id");
        });

        modelBuilder.Entity<BidContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bid_contents_pkey");

            entity.ToTable("bid_contents");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(64)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(64)
                .HasColumnName("name_uz");
        });

        modelBuilder.Entity<BidFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bid_files_pkey");

            entity.ToTable("bid_files", tb => tb.HasComment("Таблица для хранения информации о файлах, связанных с заявками"));

            entity.HasIndex(e => e.BidId, "ix_bid_files_bid_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Уникальный идентификатор записи")
                .HasColumnName("id");
            entity.Property(e => e.BidId)
                .HasComment("Идентификатор заявки")
                .HasColumnName("bid_id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Дата создания записи (по времени Ташкент, UTC+5)")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasComment("Идентификатор пользователя, создавшего запись")
                .HasColumnName("create_user");
            entity.Property(e => e.LocalUrl)
                .HasMaxLength(500)
                .HasComment("Локальный URL файла")
                .HasColumnName("local_url");

            entity.HasOne(d => d.Bid).WithMany(p => p.BidFiles)
                .HasForeignKey(d => d.BidId)
                .HasConstraintName("fk_bid_files_bids");
        });

        modelBuilder.Entity<BidState>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bid_states_pkey");

            entity.ToTable("bid_states");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(64)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(64)
                .HasColumnName("name_uz");
        });

        modelBuilder.Entity<BidType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bid_types_pkey");

            entity.ToTable("bid_types");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(64)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(64)
                .HasColumnName("name_uz");
        });

        modelBuilder.Entity<Crop>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("crops");

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

        modelBuilder.Entity<DataType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("data_types_pkey");

            entity.ToTable("data_types");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(64)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(64)
                .HasColumnName("name_uz");
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("diseases_pkey");

            entity.ToTable("diseases");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CropId).HasColumnName("crop_id");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(128)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(128)
                .HasColumnName("name_uz");
        });

        modelBuilder.Entity<ForeginUalertsCrop>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("foregin_ualerts_crops");

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

        modelBuilder.Entity<ForeginWeatherRegion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("foregin_weather_regions");

            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
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

        modelBuilder.Entity<Pest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pests_pkey");

            entity.ToTable("pests");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CropId).HasColumnName("crop_id");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(128)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(128)
                .HasColumnName("name_uz");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("regions");

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

        modelBuilder.Entity<Regions3>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("regions3");

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
            entity.Property(e => e.Region2Id).HasColumnName("region2_id");
        });

        modelBuilder.Entity<SoilType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("soil_types_pkey");

            entity.ToTable("soil_types");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(64)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(64)
                .HasColumnName("name_uz");
        });

        modelBuilder.Entity<Templ>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("templ_pkey");

            entity.ToTable("templ");

            entity.HasIndex(e => new { e.CropId, e.Lang, e.Version }, "ix_templ_crop").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(false)
                .HasColumnName("active");
            entity.Property(e => e.CropId).HasColumnName("crop_id");
            entity.Property(e => e.Lang)
                .HasMaxLength(2)
                .HasDefaultValueSql("'en'::character varying")
                .HasColumnName("lang");
            entity.Property(e => e.Version)
                .HasMaxLength(255)
                .HasColumnName("version");
        });

        modelBuilder.Entity<TemplBlock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("templ_block_pkey");

            entity.ToTable("templ_block");

            entity.HasIndex(e => e.TemplId, "ix_templ_block_templ");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.TemplId).HasColumnName("templ_id");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
        });

        modelBuilder.Entity<TemplControl>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("templ_control_pkey");

            entity.ToTable("templ_control");

            entity.HasIndex(e => e.TemplBlockId, "ix_templ_control_block");

            entity.HasIndex(e => e.TemplStepId, "ix_templ_control_step");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Auto)
                .HasDefaultValue(false)
                .HasColumnName("auto");
            entity.Property(e => e.Days)
                .HasMaxLength(255)
                .HasColumnName("days");
            entity.Property(e => e.Descr).HasColumnName("descr");
            entity.Property(e => e.Label)
                .HasMaxLength(255)
                .HasColumnName("label");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Readonly)
                .HasDefaultValue(false)
                .HasColumnName("readonly");
            entity.Property(e => e.Required)
                .HasDefaultValue(false)
                .HasColumnName("required");
            entity.Property(e => e.Source).HasColumnName("source");
            entity.Property(e => e.TemplBlockId).HasColumnName("templ_block_id");
            entity.Property(e => e.TemplStepId).HasColumnName("templ_step_id");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type");
            entity.Property(e => e.Values).HasColumnName("values");
        });

        modelBuilder.Entity<TemplStep>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("templ_step_pkey");

            entity.ToTable("templ_step");

            entity.HasIndex(e => e.TemplBlockId, "ix_templ_step_block");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Days)
                .HasMaxLength(255)
                .HasColumnName("days");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Num)
                .HasMaxLength(255)
                .HasColumnName("num");
            entity.Property(e => e.StepsCount).HasColumnName("steps_count");
            entity.Property(e => e.TemplBlockId).HasColumnName("templ_block_id");
        });

        modelBuilder.Entity<Template>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("templates_pkey");

            entity.ToTable("templates");

            entity.HasIndex(e => e.CropId, "templates_unique_active_idx")
                .IsUnique()
                .HasFilter("(is_active = true)");

            entity.HasIndex(e => new { e.CropId, e.SchemaVersion, e.ContentVersion }, "templates_unique_version_idx").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.ContentVersion).HasColumnName("content_version");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasColumnName("create_user");
            entity.Property(e => e.CropId).HasColumnName("crop_id");
            entity.Property(e => e.Data)
                .HasColumnType("jsonb")
                .HasColumnName("data");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(false)
                .HasColumnName("is_active");
            entity.Property(e => e.SchemaVersion).HasColumnName("schema_version");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasColumnName("update_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.ParentId, "ix_users_parent");

            entity.HasIndex(e => e.TypeId, "ix_users_type");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_types_pkey");

            entity.ToTable("user_types");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(64)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(64)
                .HasColumnName("name_uz");
        });

        modelBuilder.Entity<WateringType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("watering_types_pkey");

            entity.ToTable("watering_types");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(64)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(64)
                .HasColumnName("name_uz");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
