using System;
using System.Collections.Generic;
using Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne.Models;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Depricated.DataAccess.DepricatedUfieldsServerOne;

public partial class DepricatedUfieldsServerOneDbContext : DbContext
{
    public DepricatedUfieldsServerOneDbContext()
    {
    }

    public DepricatedUfieldsServerOneDbContext(DbContextOptions<DepricatedUfieldsServerOneDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AgricultureProducer> AgricultureProducers { get; set; }

    public virtual DbSet<AgrotechnicalActivity> AgrotechnicalActivities { get; set; }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<Farmer> Farmers { get; set; }

    public virtual DbSet<FarmerMetadatum> FarmerMetadata { get; set; }

    public virtual DbSet<Fertilizer> Fertilizers { get; set; }

    public virtual DbSet<Field> Fields { get; set; }

    public virtual DbSet<FieldsHistory> FieldsHistories { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Insurance> Insurances { get; set; }

    public virtual DbSet<Irrigation> Irrigations { get; set; }

    public virtual DbSet<Machinery> Machineries { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<SeasonsHistory> SeasonsHistories { get; set; }

    public virtual DbSet<UavImage> UavImages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=80.80.213.218;Port=15432;Database=ufields;Username=postgres;Password=pgPassw0rd", x => x
                .UseNodaTime()
                .UseNetTopologySuite());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("postgis")
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<AgricultureProducer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("agriculture_producer_pkey");

            entity.ToTable("agriculture_producer");

            entity.HasIndex(e => e.FieldId, "agriculture_producer_field_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AgricultureProducerId).HasColumnName("agriculture_producer_id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FieldId).HasColumnName("field_id");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
        });

        modelBuilder.Entity<AgrotechnicalActivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("agrotechnical_activities_pkey");

            entity.ToTable("agrotechnical_activities");

            entity.HasIndex(e => e.SeasonId, "agrotechnical_activities_season_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.NameId).HasColumnName("name_id");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("diseases_pkey");

            entity.ToTable("diseases");

            entity.HasIndex(e => e.SeasonId, "diseases_season_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.IdentificationDate).HasColumnName("identification_date");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.NameId).HasColumnName("name_id");
            entity.Property(e => e.ProcessingDate).HasColumnName("processing_date");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
        });

        modelBuilder.Entity<Farmer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("farmers_pkey");

            entity.ToTable("farmers");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.ExternalId)
                .HasMaxLength(64)
                .HasColumnName("external_id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
        });

        modelBuilder.Entity<FarmerMetadatum>(entity =>
        {
            entity.HasKey(e => e.FarmerId).HasName("farmer_metadata_pkey");

            entity.ToTable("farmer_metadata");

            entity.Property(e => e.FarmerId)
                .ValueGeneratedNever()
                .HasColumnName("farmer_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(64)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(64)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(64)
                .HasColumnName("middle_name");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.Title)
                .HasMaxLength(256)
                .HasColumnName("title");

            entity.HasOne(d => d.Farmer).WithOne(p => p.FarmerMetadatum)
                .HasForeignKey<FarmerMetadatum>(d => d.FarmerId)
                .HasConstraintName("farmer_metadata_farmer_id_fkey");
        });

        modelBuilder.Entity<Fertilizer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("fertilizers_pkey");

            entity.ToTable("fertilizers");

            entity.HasIndex(e => e.SeasonId, "fertilizers_season_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.Units).HasColumnName("units");
        });

        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("fields_pkey");

            entity.ToTable("fields");

            entity.HasIndex(e => e.AgricultureProducerId, "field_agriculture_producer_id");

            entity.HasIndex(e => e.ContourId, "field_contour_id");

            entity.HasIndex(e => e.DistrictId, "field_district_id");

            entity.HasIndex(e => e.FarmerId, "field_farmer_id");

            entity.HasIndex(e => e.FieldArea, "field_field_area");

            entity.HasIndex(e => e.RegionId, "field_region_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AgricultureProducerId).HasColumnName("agriculture_producer_id");
            entity.Property(e => e.AgroclimaticZoneId).HasColumnName("agroclimatic_zone_id");
            entity.Property(e => e.ClassifierId).HasColumnName("classifier_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.ContourId)
                .HasMaxLength(64)
                .HasColumnName("contour_id");
            entity.Property(e => e.Control).HasColumnName("control");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.ExternalId)
                .HasMaxLength(64)
                .HasColumnName("external_id");
            entity.Property(e => e.FarmerId).HasColumnName("farmer_id");
            entity.Property(e => e.FieldArea).HasColumnName("field_area");
            entity.Property(e => e.IrrigationSourceId).HasColumnName("irrigation_source_id");
            entity.Property(e => e.IrrigationTypeId).HasColumnName("irrigation_type_id");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Polygon).HasColumnName("polygon");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.SoilIndex).HasColumnName("soil_index");
            entity.Property(e => e.SoilTypeId).HasColumnName("soil_type_id");
            entity.Property(e => e.WaterSaving).HasColumnName("water_saving");
        });

        modelBuilder.Entity<FieldsHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("fields_history_pkey");

            entity.ToTable("fields_history");

            entity.HasIndex(e => e.CreatedAt, "field_history_created_at");

            entity.HasIndex(e => e.FieldId, "field_history_field_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AgricultureProducerId).HasColumnName("agriculture_producer_id");
            entity.Property(e => e.AgroclimaticZoneId).HasColumnName("agroclimatic_zone_id");
            entity.Property(e => e.ClassifierId).HasColumnName("classifier_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.ContourId)
                .HasMaxLength(64)
                .HasColumnName("contour_id");
            entity.Property(e => e.Control).HasColumnName("control");
            entity.Property(e => e.CreateDate).HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.ExternalId)
                .HasMaxLength(64)
                .HasColumnName("external_id");
            entity.Property(e => e.FarmerId).HasColumnName("farmer_id");
            entity.Property(e => e.FieldArea).HasColumnName("field_area");
            entity.Property(e => e.FieldId).HasColumnName("field_id");
            entity.Property(e => e.IrrigationSourceId).HasColumnName("irrigation_source_id");
            entity.Property(e => e.IrrigationTypeId).HasColumnName("irrigation_type_id");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Polygon).HasColumnName("polygon");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.SoilIndex).HasColumnName("soil_index");
            entity.Property(e => e.SoilTypeId).HasColumnName("soil_type_id");
            entity.Property(e => e.WaterSaving).HasColumnName("water_saving");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("images_pkey");

            entity.ToTable("images");

            entity.HasIndex(e => e.Author, "images_author");

            entity.HasIndex(e => e.DistrictId, "images_district_id");

            entity.HasIndex(e => e.ImageDate, "images_image_date");

            entity.HasIndex(e => e.RegionId, "images_region_id");

            entity.HasIndex(e => e.Tags, "images_tags");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Author)
                .HasMaxLength(256)
                .HasColumnName("author");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.FileSize).HasColumnName("file_size");
            entity.Property(e => e.ImageDate).HasColumnName("image_date");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.Tags)
                .HasMaxLength(1024)
                .HasColumnName("tags");
        });

        modelBuilder.Entity<Insurance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("insurance_pkey");

            entity.ToTable("insurance");

            entity.HasIndex(e => e.EndDate, "insurance_end_date");

            entity.HasIndex(e => e.SeasonId, "insurance_season_id");

            entity.HasIndex(e => e.StartDate, "insurance_start_date");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.InsurancePremium).HasColumnName("insurance_premium");
            entity.Property(e => e.Insurer).HasColumnName("insurer");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.Payments).HasColumnName("payments");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.SumInsured).HasColumnName("sum_insured");
        });

        modelBuilder.Entity<Irrigation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("irrigation_pkey");

            entity.ToTable("irrigation");

            entity.HasIndex(e => e.SeasonId, "irrigation_season_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
        });

        modelBuilder.Entity<Machinery>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("machinery_pkey");

            entity.ToTable("machinery");

            entity.HasIndex(e => e.SeasonId, "machinery_season_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.MachineryTypeId).HasColumnName("machinery_type_id");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.OperationId).HasColumnName("operation_id");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.Usage).HasColumnName("usage");
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("seasons_pkey");

            entity.ToTable("seasons");

            entity.HasIndex(e => e.CropId, "season_crop_id");

            entity.HasIndex(e => e.FieldId, "season_field_id");

            entity.HasIndex(e => e.HarvestingDate, "season_harvesting_date");

            entity.HasIndex(e => e.PlantingDate, "season_planting_date");

            entity.HasIndex(e => e.Polygon, "season_polygon").HasMethod("gist");

            entity.HasIndex(e => e.VarietyId, "season_variety_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.CropId).HasColumnName("crop_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FieldArea)
                .HasDefaultValueSql("0")
                .HasColumnName("field_area");
            entity.Property(e => e.FieldId).HasColumnName("field_id");
            entity.Property(e => e.HarvestingDate).HasColumnName("harvesting_date");
            entity.Property(e => e.HarvestingDatePlan).HasColumnName("harvesting_date_plan");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.PlantingDate).HasColumnName("planting_date");
            entity.Property(e => e.PlantingDatePlan).HasColumnName("planting_date_plan");
            entity.Property(e => e.Polygon).HasColumnName("polygon");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Title)
                .HasMaxLength(64)
                .HasColumnName("title");
            entity.Property(e => e.VarietyId).HasColumnName("variety_id");
            entity.Property(e => e.Yield).HasColumnName("yield");
            entity.Property(e => e.YieldHa).HasColumnName("yield_ha");
        });

        modelBuilder.Entity<SeasonsHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("seasons_history_pkey");

            entity.ToTable("seasons_history");

            entity.HasIndex(e => e.CreatedAt, "season_history_created_at");

            entity.HasIndex(e => e.SeasonId, "season_history_season_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreateDate).HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CropId).HasColumnName("crop_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FieldArea)
                .HasDefaultValueSql("0")
                .HasColumnName("field_area");
            entity.Property(e => e.FieldId).HasColumnName("field_id");
            entity.Property(e => e.HarvestingDate).HasColumnName("harvesting_date");
            entity.Property(e => e.HarvestingDatePlan).HasColumnName("harvesting_date_plan");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.PlantingDate).HasColumnName("planting_date");
            entity.Property(e => e.PlantingDatePlan).HasColumnName("planting_date_plan");
            entity.Property(e => e.Polygon).HasColumnName("polygon");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Title)
                .HasMaxLength(64)
                .HasColumnName("title");
            entity.Property(e => e.VarietyId).HasColumnName("variety_id");
            entity.Property(e => e.Yield).HasColumnName("yield");
            entity.Property(e => e.YieldHa).HasColumnName("yield_ha");
        });

        modelBuilder.Entity<UavImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("uav_images_pkey");

            entity.ToTable("uav_images");

            entity.HasIndex(e => e.AgricultureProducerId, "uav_images_agriculture_producer_id");

            entity.HasIndex(e => e.Area, "uav_images_area");

            entity.HasIndex(e => e.Crops, "uav_images_crops");

            entity.HasIndex(e => e.DistrictId, "uav_images_district_id");

            entity.HasIndex(e => e.RegionId, "uav_images_field_id");

            entity.HasIndex(e => e.ImageDate, "uav_images_image_date");

            entity.HasIndex(e => new { e.ImageType, e.CameraType }, "uav_images_image_type_camera_type");

            entity.HasIndex(e => e.Tags, "uav_images_tags");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AgricultureProducerId)
                .HasMaxLength(64)
                .HasColumnName("agriculture_producer_id");
            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.Bbox).HasColumnName("bbox");
            entity.Property(e => e.CameraType).HasColumnName("camera_type");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasMaxLength(255)
                .HasColumnName("create_user");
            entity.Property(e => e.Crops).HasColumnName("crops");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.FileSize).HasColumnName("file_size");
            entity.Property(e => e.ImageDate).HasColumnName("image_date");
            entity.Property(e => e.ImageType)
                .HasMaxLength(255)
                .HasColumnName("image_type");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");
            entity.Property(e => e.ModifyUser)
                .HasMaxLength(255)
                .HasColumnName("modify_user");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.Tags)
                .HasMaxLength(1024)
                .HasColumnName("tags");
            entity.Property(e => e.Trajectory).HasColumnName("trajectory");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
