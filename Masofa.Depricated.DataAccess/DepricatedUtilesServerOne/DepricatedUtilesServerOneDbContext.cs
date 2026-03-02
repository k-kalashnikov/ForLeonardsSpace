using System;
using System.Collections.Generic;
using Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Depricated.DataAccess.DepricatedUtilesServerOne;

public partial class DepricatedUtilesServerOneDbContext : DbContext
{
    public DepricatedUtilesServerOneDbContext()
    {
    }

    public DepricatedUtilesServerOneDbContext(DbContextOptions<DepricatedUtilesServerOneDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<App> Apps { get; set; }

    public virtual DbSet<Band> Bands { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Date> Dates { get; set; }

    public virtual DbSet<Download> Downloads { get; set; }

    public virtual DbSet<DownloadHistory> DownloadHistories { get; set; }

    public virtual DbSet<Field> Fields { get; set; }

    public virtual DbSet<FieldService> FieldServices { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupShape> GroupShapes { get; set; }

    public virtual DbSet<Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models.Index> Indexes { get; set; }

    public virtual DbSet<LoadTask> LoadTasks { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Ndvi> Ndvis { get; set; }

    public virtual DbSet<Processing> Processings { get; set; }

    public virtual DbSet<ProcessingBand> ProcessingBands { get; set; }

    public virtual DbSet<Queue> Queues { get; set; }

    public virtual DbSet<SceneKey> SceneKeys { get; set; }

    public virtual DbSet<Sensor> Sensors { get; set; }

    public virtual DbSet<Shape> Shapes { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Upload> Uploads { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }

    public virtual DbSet<ZoneCountry> ZoneCountries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=80.80.213.218;Port=15432;Database=utiles;Username=postgres;Password=pgPassw0rd", x => x
                .UseNodaTime()
                .UseNetTopologySuite());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<App>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("app_pkey");

            entity.ToTable("app");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.Alive)
                .HasDefaultValue(false)
                .HasColumnName("alive");
            entity.Property(e => e.Host)
                .HasMaxLength(1024)
                .HasColumnName("host");
            entity.Property(e => e.LastStartedAt).HasColumnName("last_started_at");
            entity.Property(e => e.LastStopAt).HasColumnName("last_stop_at");
            entity.Property(e => e.Properties).HasColumnName("properties");
            entity.Property(e => e.State)
                .HasMaxLength(4096)
                .HasColumnName("state");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<Band>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bands_pkey");

            entity.ToTable("bands");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.ArviCreatedAt).HasColumnName("arvi_created_at");
            entity.Property(e => e.ArviDetails).HasColumnName("arvi_details");
            entity.Property(e => e.ArviStateCode)
                .HasDefaultValue(0)
                .HasColumnName("arvi_state_code");
            entity.Property(e => e.Attempts).HasColumnName("attempts");
            entity.Property(e => e.Cloudy).HasColumnName("cloudy");
            entity.Property(e => e.CopernicusJson)
                .HasColumnType("json")
                .HasColumnName("copernicus_json");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("now()")
                .HasColumnName("create_time");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.EviCreatedAt).HasColumnName("evi_created_at");
            entity.Property(e => e.EviDetails).HasColumnName("evi_details");
            entity.Property(e => e.EviStateCode)
                .HasDefaultValue(0)
                .HasColumnName("evi_state_code");
            entity.Property(e => e.Label)
                .HasMaxLength(1024)
                .HasColumnName("label");
            entity.Property(e => e.LaiCreatedAt).HasColumnName("lai_created_at");
            entity.Property(e => e.LaiDetails).HasColumnName("lai_details");
            entity.Property(e => e.LaiStateCode)
                .HasDefaultValue(0)
                .HasColumnName("lai_state_code");
            entity.Property(e => e.NdwiCreatedAt).HasColumnName("ndwi_created_at");
            entity.Property(e => e.NdwiDetails).HasColumnName("ndwi_details");
            entity.Property(e => e.NdwiStateCode)
                .HasDefaultValue(0)
                .HasColumnName("ndwi_state_code");
            entity.Property(e => e.NoData)
                .HasDefaultValueSql("'-1.0'::numeric")
                .HasColumnName("no_data");
            entity.Property(e => e.PlantCreatedAt).HasColumnName("plant_created_at");
            entity.Property(e => e.PlantDetails).HasColumnName("plant_details");
            entity.Property(e => e.PlantStateCode)
                .HasDefaultValue(0)
                .HasColumnName("plant_state_code");
            entity.Property(e => e.SaviCreatedAt).HasColumnName("savi_created_at");
            entity.Property(e => e.SaviDetails).HasColumnName("savi_details");
            entity.Property(e => e.SaviStateCode)
                .HasDefaultValue(0)
                .HasColumnName("savi_state_code");
            entity.Property(e => e.SensorCode).HasColumnName("sensor_code");
            entity.Property(e => e.State)
                .HasDefaultValue(0)
                .HasColumnName("state");
            entity.Property(e => e.TrueCreatedAt).HasColumnName("true_created_at");
            entity.Property(e => e.TrueDetails).HasColumnName("true_details");
            entity.Property(e => e.TrueStateCode)
                .HasDefaultValue(0)
                .HasColumnName("true_state_code");
            entity.Property(e => e.Uid).HasColumnName("uid");

            entity.HasOne(d => d.SensorCodeNavigation).WithMany(p => p.Bands)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.SensorCode)
                .HasConstraintName("sensor_code_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("country_pkey");

            entity.ToTable("country");

            entity.HasIndex(e => e.Name, "country_name_key").IsUnique();

            entity.Property(e => e.Uid)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("uid");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Enabled)
                .HasDefaultValue(true)
                .HasColumnName("enabled");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.Region).HasColumnName("region");
            entity.Property(e => e.SimpleRegion).HasColumnName("simple_region");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasDefaultValue(0)
                .HasColumnName("status");
            entity.Property(e => e.UpdateDate).HasColumnName("update_date");

            entity.HasOne(d => d.App).WithMany(p => p.Countries)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("country_app_id_fkey");
        });

        modelBuilder.Entity<Date>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("date_pkey");

            entity.ToTable("date");

            entity.HasIndex(e => new { e.State, e.SensorCode, e.AppId }, "date_ssa");

            entity.HasIndex(e => new { e.Uid, e.SensorCode, e.Date1, e.FieldModified }, "date_unique_id").IsUnique();

            entity.HasIndex(e => new { e.Priority, e.Date1 }, "tiles_date_priority_date").IsDescending(false, true);

            entity.HasIndex(e => new { e.State, e.SensorCode, e.AppId, e.Priority, e.Date1 }, "tiles_date_priority_date_ssa").IsDescending(false, false, false, false, true);

            entity.HasIndex(e => new { e.Uid, e.FieldModified, e.SensorCode }, "tiles_date_uid_field_modified_sensor_code");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.ArviCreatedAt).HasColumnName("arvi_created_at");
            entity.Property(e => e.ArviDetails).HasColumnName("arvi_details");
            entity.Property(e => e.ArviState)
                .HasDefaultValue(0)
                .HasColumnName("arvi_state");
            entity.Property(e => e.Attempts).HasColumnName("attempts");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CviCreatedAt).HasColumnName("cvi_created_at");
            entity.Property(e => e.CviDetails).HasColumnName("cvi_details");
            entity.Property(e => e.CviGerminationHistogram).HasColumnName("cvi_germination_histogram");
            entity.Property(e => e.CviHistogram).HasColumnName("cvi_histogram");
            entity.Property(e => e.CviRelativeHistogram).HasColumnName("cvi_relative_histogram");
            entity.Property(e => e.CviState).HasColumnName("cvi_state");
            entity.Property(e => e.Date1).HasColumnName("date");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.East).HasColumnName("east");
            entity.Property(e => e.EastNdvi).HasColumnName("east_ndvi");
            entity.Property(e => e.EviCreatedAt).HasColumnName("evi_created_at");
            entity.Property(e => e.EviDetails).HasColumnName("evi_details");
            entity.Property(e => e.EviState)
                .HasDefaultValue(0)
                .HasColumnName("evi_state");
            entity.Property(e => e.FieldModified).HasColumnName("field_modified");
            entity.Property(e => e.GerminationCreatedAt).HasColumnName("germination_created_at");
            entity.Property(e => e.GerminationDetails).HasColumnName("germination_details");
            entity.Property(e => e.GerminationHistogram).HasColumnName("germination_histogram");
            entity.Property(e => e.GerminationState)
                .HasDefaultValue(0)
                .HasColumnName("germination_state");
            entity.Property(e => e.GndviCreatedAt).HasColumnName("gndvi_created_at");
            entity.Property(e => e.GndviDetails).HasColumnName("gndvi_details");
            entity.Property(e => e.GndviState)
                .HasDefaultValue(0)
                .HasColumnName("gndvi_state");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.LaiCreatedAt).HasColumnName("lai_created_at");
            entity.Property(e => e.LaiDetails).HasColumnName("lai_details");
            entity.Property(e => e.LaiGerminationHistogram).HasColumnName("lai_germination_histogram");
            entity.Property(e => e.LaiHistogram).HasColumnName("lai_histogram");
            entity.Property(e => e.LaiRelativeHistogram).HasColumnName("lai_relative_histogram");
            entity.Property(e => e.LaiState).HasColumnName("lai_state");
            entity.Property(e => e.NdmiCreatedAt).HasColumnName("ndmi_created_at");
            entity.Property(e => e.NdmiDetails).HasColumnName("ndmi_details");
            entity.Property(e => e.NdmiState)
                .HasDefaultValue(0)
                .HasColumnName("ndmi_state");
            entity.Property(e => e.NdviHistogram).HasColumnName("ndvi_histogram");
            entity.Property(e => e.NdwiCreatedAt).HasColumnName("ndwi_created_at");
            entity.Property(e => e.NdwiDetails).HasColumnName("ndwi_details");
            entity.Property(e => e.NdwiState)
                .HasDefaultValue(0)
                .HasColumnName("ndwi_state");
            entity.Property(e => e.North).HasColumnName("north");
            entity.Property(e => e.NorthNdvi).HasColumnName("north_ndvi");
            entity.Property(e => e.PlantCreatedAt).HasColumnName("plant_created_at");
            entity.Property(e => e.PlantDetails).HasColumnName("plant_details");
            entity.Property(e => e.PlantState)
                .HasDefaultValue(0)
                .HasColumnName("plant_state");
            entity.Property(e => e.Priority)
                .HasDefaultValue(0L)
                .HasColumnName("priority");
            entity.Property(e => e.RelativeCreatedAt).HasColumnName("relative_created_at");
            entity.Property(e => e.RelativeDetails).HasColumnName("relative_details");
            entity.Property(e => e.RelativeHistogram).HasColumnName("relative_histogram");
            entity.Property(e => e.RelativeState)
                .HasDefaultValue(0)
                .HasColumnName("relative_state");
            entity.Property(e => e.Restored)
                .HasDefaultValue(0)
                .HasColumnName("restored");
            entity.Property(e => e.RestoredSceneKey).HasColumnName("restored_scene_key");
            entity.Property(e => e.RtvicCreatedAt).HasColumnName("rtvic_created_at");
            entity.Property(e => e.RtvicDetails).HasColumnName("rtvic_details");
            entity.Property(e => e.RtvicState)
                .HasDefaultValue(0)
                .HasColumnName("rtvic_state");
            entity.Property(e => e.SaviCreatedAt).HasColumnName("savi_created_at");
            entity.Property(e => e.SaviDetails).HasColumnName("savi_details");
            entity.Property(e => e.SaviState)
                .HasDefaultValue(0)
                .HasColumnName("savi_state");
            entity.Property(e => e.SceneKeyId).HasColumnName("scene_key_id");
            entity.Property(e => e.SensorCode).HasColumnName("sensor_code");
            entity.Property(e => e.SourceCreatedAt).HasColumnName("source_created_at");
            entity.Property(e => e.SourceDetails).HasColumnName("source_details");
            entity.Property(e => e.SourceState)
                .HasDefaultValue(0)
                .HasColumnName("source_state");
            entity.Property(e => e.South).HasColumnName("south");
            entity.Property(e => e.SouthNdvi).HasColumnName("south_ndvi");
            entity.Property(e => e.SrcNumber).HasColumnName("src_number");
            entity.Property(e => e.State)
                .HasDefaultValue(0)
                .HasColumnName("state");
            entity.Property(e => e.Test)
                .HasDefaultValue(false)
                .HasColumnName("test");
            entity.Property(e => e.TrueCreatedAt).HasColumnName("true_created_at");
            entity.Property(e => e.TrueDetails).HasColumnName("true_details");
            entity.Property(e => e.TrueState)
                .HasDefaultValue(0)
                .HasColumnName("true_state");
            entity.Property(e => e.Uid).HasColumnName("uid");
            entity.Property(e => e.VariCreatedAt).HasColumnName("vari_created_at");
            entity.Property(e => e.VariDetails).HasColumnName("vari_details");
            entity.Property(e => e.VariState)
                .HasDefaultValue(0)
                .HasColumnName("vari_state");
            entity.Property(e => e.West).HasColumnName("west");
            entity.Property(e => e.WestNdvi).HasColumnName("west_ndvi");
            entity.Property(e => e.Width).HasColumnName("width");

            entity.HasOne(d => d.PlantStateNavigation).WithMany(p => p.DatePlantStateNavigations)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.PlantState)
                .HasConstraintName("plant_state_fkey");

            entity.HasOne(d => d.RelativeStateNavigation).WithMany(p => p.DateRelativeStateNavigations)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.RelativeState)
                .HasConstraintName("relative_state_fkey");

            entity.HasOne(d => d.SensorCodeNavigation).WithMany(p => p.Dates)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.SensorCode)
                .HasConstraintName("sensor_code_fkey");

            entity.HasOne(d => d.StateNavigation).WithMany(p => p.DateStateNavigations)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.State)
                .HasConstraintName("state_fkey");

            entity.HasOne(d => d.TrueStateNavigation).WithMany(p => p.DateTrueStateNavigations)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.TrueState)
                .HasConstraintName("true_state_fkey");
        });

        modelBuilder.Entity<Download>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("downloads_pkey");

            entity.ToTable("downloads");

            entity.HasIndex(e => new { e.Host, e.Path }, "host_path").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.FileName)
                .HasMaxLength(256)
                .HasColumnName("file_name");
            entity.Property(e => e.FinishedAt).HasColumnName("finished_at");
            entity.Property(e => e.Host)
                .HasMaxLength(1024)
                .HasColumnName("host");
            entity.Property(e => e.Path)
                .HasMaxLength(1024)
                .HasColumnName("path");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("started_at");

            entity.HasOne(d => d.App).WithMany(p => p.Downloads)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("downloads_app_id_fkey");
        });

        modelBuilder.Entity<DownloadHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("download_history_pkey");

            entity.ToTable("download_history");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("now()")
                .HasColumnName("date");
            entity.Property(e => e.Path)
                .HasMaxLength(1024)
                .HasColumnName("path");
        });

        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("field_uid_key");

            entity.ToTable("field");

            entity.HasIndex(e => e.Name, "name_unique_id").IsUnique();

            entity.Property(e => e.Uid)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("uid");
            entity.Property(e => e.Amazon)
                .HasDefaultValue(false)
                .HasColumnName("amazon");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.CheckDate).HasColumnName("check_date");
            entity.Property(e => e.Copernicus)
                .HasDefaultValue(true)
                .HasColumnName("copernicus");
            entity.Property(e => e.CpLat).HasColumnName("cp_lat");
            entity.Property(e => e.CpLon).HasColumnName("cp_lon");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.East).HasColumnName("east");
            entity.Property(e => e.Enabled)
                .HasDefaultValue(true)
                .HasColumnName("enabled");
            entity.Property(e => e.Fallow)
                .HasDefaultValue(false)
                .HasColumnName("fallow");
            entity.Property(e => e.GenerateTiles)
                .HasDefaultValue(true)
                .HasColumnName("generate_tiles");
            entity.Property(e => e.Germination)
                .HasDefaultValue(false)
                .HasColumnName("germination");
            entity.Property(e => e.HasNewDate)
                .HasDefaultValue(false)
                .HasColumnName("has_new_date");
            entity.Property(e => e.LSavi)
                .HasDefaultValueSql("0.5")
                .HasColumnName("l_savi");
            entity.Property(e => e.Modified)
                .HasDefaultValueSql("now()")
                .HasColumnName("modified");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.North).HasColumnName("north");
            entity.Property(e => e.PlantingDate).HasColumnName("planting_date");
            entity.Property(e => e.Region).HasColumnName("region");
            entity.Property(e => e.SeasonMonths)
                .HasDefaultValue(12)
                .HasColumnName("season_months");
            entity.Property(e => e.SentDatesAmount)
                .HasDefaultValue(0)
                .HasColumnName("sent_dates_amount");
            entity.Property(e => e.SkipClouds)
                .HasDefaultValue(true)
                .HasColumnName("skip_clouds");
            entity.Property(e => e.South).HasColumnName("south");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasDefaultValue(0)
                .HasColumnName("status");
            entity.Property(e => e.Test)
                .HasDefaultValue(false)
                .HasColumnName("test");
            entity.Property(e => e.TilesOrder)
                .HasDefaultValue(0L)
                .HasColumnName("tiles_order");
            entity.Property(e => e.UpdateDate).HasColumnName("update_date");
            entity.Property(e => e.West).HasColumnName("west");

            entity.HasOne(d => d.App).WithMany(p => p.Fields)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("field_app_id_fkey");
        });

        modelBuilder.Entity<FieldService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("field_service_pkey");

            entity.ToTable("field_service");

            entity.HasIndex(e => e.Code, "field_service_code_key").IsUnique();

            entity.HasIndex(e => e.Name, "field_service_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.Properties).HasColumnName("properties");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("group_id_key");

            entity.ToTable("group");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.East).HasColumnName("east");
            entity.Property(e => e.Label)
                .HasMaxLength(1024)
                .HasColumnName("label");
            entity.Property(e => e.Modified)
                .HasDefaultValueSql("now()")
                .HasColumnName("modified");
            entity.Property(e => e.North).HasColumnName("north");
            entity.Property(e => e.SensorCode).HasColumnName("sensor_code");
            entity.Property(e => e.South).HasColumnName("south");
            entity.Property(e => e.West).HasColumnName("west");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.Zone).WithMany(p => p.Groups)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("zone_id_fkey");
        });

        modelBuilder.Entity<GroupShape>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("group_shape");

            entity.HasIndex(e => new { e.GroupId, e.ShapeId }, "group_id_shape_id").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.ShapeId).HasColumnName("shape_id");
        });

        modelBuilder.Entity<Masofa.Depricated.DataAccess.DepricatedUtilesServerOne.Models.Index>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("indexes");

            entity.HasIndex(e => new { e.FieldId, e.Date }, "px_indexes").IsUnique();

            entity.Property(e => e.Cloud).HasColumnName("cloud");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("now()")
                .HasColumnName("create_time");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.FieldId).HasColumnName("field_id");
            entity.Property(e => e.Ndvi).HasColumnName("ndvi");
            entity.Property(e => e.RawNdvi).HasColumnName("raw_ndvi");
            entity.Property(e => e.SensorCode).HasColumnName("sensor_code");
            entity.Property(e => e.Uid).HasColumnName("uid");

            entity.HasOne(d => d.SensorCodeNavigation).WithMany()
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.SensorCode)
                .HasConstraintName("sensor_code_fkey");

            entity.HasOne(d => d.UidNavigation).WithMany()
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("uid_fkey");
        });

        modelBuilder.Entity<LoadTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("load_task_id_key");

            entity.ToTable("load_task");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.FieldModified).HasColumnName("field_modified");
            entity.Property(e => e.FinishedAt).HasColumnName("finished_at");
            entity.Property(e => e.Result).HasColumnName("result");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.Status)
                .HasDefaultValue(0)
                .HasColumnName("status");
            entity.Property(e => e.Uid).HasColumnName("uid");

            entity.HasOne(d => d.App).WithMany(p => p.LoadTasks)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("app_id_fkey");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.LoadTasks)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.Status)
                .HasConstraintName("status_fkey");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("log");

            entity.HasIndex(e => e.Line, "log_line_key").IsUnique();

            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Host)
                .HasMaxLength(1024)
                .HasColumnName("host");
            entity.Property(e => e.Line)
                .ValueGeneratedOnAdd()
                .HasColumnName("line");
            entity.Property(e => e.Message).HasColumnName("message");
        });

        modelBuilder.Entity<Ndvi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ndvi_pkey");

            entity.ToTable("ndvi");

            entity.HasIndex(e => new { e.Uid, e.SensorCode, e.NdviDate }, "ndvi_unique_id").IsUnique();

            entity.HasIndex(e => new { e.FieldName, e.SensorCode, e.NdviDate }, "ndvi_unique_id_1").IsUnique();

            entity.HasIndex(e => new { e.FieldName, e.SensorCode, e.NdviDate }, "px_ndvi").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.Cloud).HasColumnName("cloud");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("now()")
                .HasColumnName("create_time");
            entity.Property(e => e.Cvi).HasColumnName("cvi");
            entity.Property(e => e.FieldModified).HasColumnName("field_modified");
            entity.Property(e => e.FieldName)
                .HasMaxLength(256)
                .HasColumnName("field_name");
            entity.Property(e => e.Gndvi).HasColumnName("gndvi");
            entity.Property(e => e.H).HasColumnName("h");
            entity.Property(e => e.HasBoundary)
                .HasDefaultValue(true)
                .HasColumnName("has_boundary");
            entity.Property(e => e.InternalXy).HasColumnName("internal_xy");
            entity.Property(e => e.Internals).HasColumnName("internals");
            entity.Property(e => e.Lai).HasColumnName("lai");
            entity.Property(e => e.Ndmi).HasColumnName("ndmi");
            entity.Property(e => e.NdviDate).HasColumnName("ndvi_date");
            entity.Property(e => e.NdviValue).HasColumnName("ndvi_value");
            entity.Property(e => e.Ndwi).HasColumnName("ndwi");
            entity.Property(e => e.Rtvic).HasColumnName("rtvic");
            entity.Property(e => e.Savi).HasColumnName("savi");
            entity.Property(e => e.SensorCode).HasColumnName("sensor_code");
            entity.Property(e => e.Uid).HasColumnName("uid");
            entity.Property(e => e.Vari).HasColumnName("vari");
            entity.Property(e => e.Visibility)
                .HasDefaultValue(true)
                .HasColumnName("visibility");
            entity.Property(e => e.W).HasColumnName("w");
            entity.Property(e => e.Xy).HasColumnName("xy");

            entity.HasOne(d => d.SensorCodeNavigation).WithMany(p => p.Ndvis)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.SensorCode)
                .HasConstraintName("sensor_code_fkey");
        });

        modelBuilder.Entity<Processing>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("processing");

            entity.HasIndex(e => e.Id, "processing_id_key").IsUnique();

            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("started_at");
        });

        modelBuilder.Entity<ProcessingBand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("processing_bands_pkey");

            entity.ToTable("processing_bands");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
        });

        modelBuilder.Entity<Queue>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("queue");

            entity.HasIndex(e => e.Id, "queue_id_key").IsUnique();

            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.SensorCode).HasColumnName("sensor_code");
        });

        modelBuilder.Entity<SceneKey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("scene_key_pkey");

            entity.ToTable("scene_key");

            entity.HasIndex(e => new { e.Label, e.SensorCode }, "scene_key_select_i2");

            entity.HasIndex(e => new { e.Uid, e.Date, e.FieldModified }, "scene_key_udm");

            entity.HasIndex(e => new { e.Label, e.ShapeId, e.Uid, e.FieldModified }, "scene_unique_id_0").IsUnique();

            entity.HasIndex(e => new { e.Uid, e.SensorCode, e.Date, e.FieldModified }, "tiles_scene_key_usds");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.Cloud).HasColumnName("cloud");
            entity.Property(e => e.Copernicus)
                .HasDefaultValue(false)
                .HasColumnName("copernicus");
            entity.Property(e => e.CopernicusJson)
                .HasColumnType("json")
                .HasColumnName("copernicus_json");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("now()")
                .HasColumnName("create_time");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.FieldModified).HasColumnName("field_modified");
            entity.Property(e => e.Label)
                .HasMaxLength(1024)
                .HasColumnName("label");
            entity.Property(e => e.Merged)
                .HasDefaultValue(false)
                .HasColumnName("merged");
            entity.Property(e => e.NoData)
                .HasDefaultValueSql("'-1.0'::numeric")
                .HasColumnName("no_data");
            entity.Property(e => e.Selected)
                .HasDefaultValue(false)
                .HasColumnName("selected");
            entity.Property(e => e.SensorCode).HasColumnName("sensor_code");
            entity.Property(e => e.ShapeId).HasColumnName("shape_id");
            entity.Property(e => e.SrcNumber).HasColumnName("src_number");
            entity.Property(e => e.State)
                .HasDefaultValue(0)
                .HasColumnName("state");
            entity.Property(e => e.Uid).HasColumnName("uid");

            entity.HasOne(d => d.SensorCodeNavigation).WithMany(p => p.SceneKeys)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.SensorCode)
                .HasConstraintName("sensor_code_fkey");

            entity.HasOne(d => d.StateNavigation).WithMany(p => p.SceneKeys)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.State)
                .HasConstraintName("state_fkey");
        });

        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sensor_pkey");

            entity.ToTable("sensor");

            entity.HasIndex(e => e.Code, "sensor_code_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Describe)
                .HasMaxLength(256)
                .HasColumnName("describe");
        });

        modelBuilder.Entity<Shape>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shape_pkey");

            entity.ToTable("shape");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.FieldModified).HasColumnName("field_modified");
            entity.Property(e => e.Label)
                .HasMaxLength(8)
                .HasColumnName("label");
            entity.Property(e => e.SensorCode).HasColumnName("sensor_code");
            entity.Property(e => e.Uid).HasColumnName("uid");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.SensorCodeNavigation).WithMany(p => p.Shapes)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.SensorCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sensor_code_fkey");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("state_pkey");

            entity.ToTable("state");

            entity.HasIndex(e => e.Code, "state_code_key").IsUnique();

            entity.HasIndex(e => e.Describe, "state_describe_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Describe)
                .HasMaxLength(256)
                .HasColumnName("describe");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_pkey");

            entity.ToTable("status");

            entity.HasIndex(e => e.Code, "status_code_key").IsUnique();

            entity.HasIndex(e => e.Describe, "status_describe_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Describe)
                .HasMaxLength(256)
                .HasColumnName("describe");
        });

        modelBuilder.Entity<Upload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("uploads_pkey");

            entity.ToTable("uploads");

            entity.HasIndex(e => e.Path, "uploads_path_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.DateId).HasColumnName("date_id");
            entity.Property(e => e.Path)
                .HasMaxLength(1024)
                .HasColumnName("path");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("started_at");
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("zone_pkey");

            entity.ToTable("zone");

            entity.HasIndex(e => e.Label, "zone_label_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Label)
                .HasMaxLength(8)
                .HasColumnName("label");
            entity.Property(e => e.SensorCode).HasColumnName("sensor_code");

            entity.HasOne(d => d.SensorCodeNavigation).WithMany(p => p.Zones)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.SensorCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sensor_code_fkey");
        });

        modelBuilder.Entity<ZoneCountry>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("zone_country");

            entity.Property(e => e.Uid).HasColumnName("uid");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
