using System;
using System.Collections.Generic;
using Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne;

public partial class DepricatedWeatherServerOneDbContext : DbContext
{
    public DepricatedWeatherServerOneDbContext()
    {
    }

    public DepricatedWeatherServerOneDbContext(DbContextOptions<DepricatedWeatherServerOneDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AgroClimaticZone> AgroClimaticZones { get; set; }

    public virtual DbSet<AgroClimaticZoneMonthNorm> AgroClimaticZoneMonthNorms { get; set; }

    public virtual DbSet<AgroClimaticZoneNorm> AgroClimaticZoneNorms { get; set; }

    public virtual DbSet<AgroClimaticZonesWeatherRate> AgroClimaticZonesWeatherRates { get; set; }

    public virtual DbSet<Alert> Alerts { get; set; }

    public virtual DbSet<AlertType> AlertTypes { get; set; }

    public virtual DbSet<ApplicationProperty> ApplicationProperties { get; set; }

    public virtual DbSet<Condition> Conditions { get; set; }

    public virtual DbSet<Frequency> Frequencies { get; set; }

    public virtual DbSet<ImageType> ImageTypes { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobStatus> JobStatuses { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<RegionsAgroClimaticZone> RegionsAgroClimaticZones { get; set; }

    public virtual DbSet<RegionsDump> RegionsDumps { get; set; }

    public virtual DbSet<RegionsWeather> RegionsWeathers { get; set; }

    public virtual DbSet<RegionsWeather1> RegionsWeathers1 { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReportType> ReportTypes { get; set; }

    public virtual DbSet<VAlertsFrozen> VAlertsFrozens { get; set; }

    public virtual DbSet<WeatherDatesCompleted> WeatherDatesCompleteds { get; set; }

    public virtual DbSet<WeatherStation> WeatherStations { get; set; }

    public virtual DbSet<WeatherStationAgroClimaticZone> WeatherStationAgroClimaticZones { get; set; }

    public virtual DbSet<WeatherStationsDataEx> WeatherStationsDataExes { get; set; }

    public virtual DbSet<WeatherStationsDatum> WeatherStationsData { get; set; }

    public virtual DbSet<WeatherType> WeatherTypes { get; set; }

    public virtual DbSet<XslsUzUnputColumn> XslsUzUnputColumns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=80.80.213.218;Port=15432;Database=weather;Username=postgres;Password=pgPassw0rd", x => x
                .UseNodaTime()
                .UseNetTopologySuite()
                .CommandTimeout(10800)
        );

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("postgis")
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<AgroClimaticZone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("agro_climatic_zones_pkey");

            entity.ToTable("agro_climatic_zones");

            entity.HasIndex(e => e.PolygonGeom, "agro_climatic_zones_polygon_geom").HasMethod("gist");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(256)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(256)
                .HasColumnName("name_uz");
            entity.Property(e => e.Polygon).HasColumnName("polygon");
            entity.Property(e => e.PolygonGeom).HasColumnName("polygon_geom");
        });

        modelBuilder.Entity<AgroClimaticZoneMonthNorm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("agro_climatic_zone_month_norms_pkey");

            entity.ToTable("agro_climatic_zone_month_norms");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AgroClimaticZoneId).HasColumnName("agro_climatic_zone_id");
            entity.Property(e => e.M).HasColumnName("m");
            entity.Property(e => e.PrecipitationAvgNorm).HasColumnName("precipitation_avg_norm");
            entity.Property(e => e.PrecipitationMedNorm).HasColumnName("precipitation_med_norm");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.SolarRadiationAvgNorm).HasColumnName("solar_radiation_avg_norm");
            entity.Property(e => e.SolarRadiationMedNorm).HasColumnName("solar_radiation_med_norm");
            entity.Property(e => e.TemperatureAvgNorm).HasColumnName("temperature_avg_norm");
            entity.Property(e => e.TemperatureMedNorm).HasColumnName("temperature_med_norm");
        });

        modelBuilder.Entity<AgroClimaticZoneNorm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("agro_climatic_zone_norms_pkey");

            entity.ToTable("agro_climatic_zone_norms");

            entity.HasIndex(e => new { e.ProviderId, e.AgroClimaticZoneId, e.M, e.D }, "uix_agro_climatic_zone_norms").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AgroClimaticZoneId).HasColumnName("agro_climatic_zone_id");
            entity.Property(e => e.D).HasColumnName("d");
            entity.Property(e => e.M).HasColumnName("m");
            entity.Property(e => e.PrecipitationAvgNorm).HasColumnName("precipitation_avg_norm");
            entity.Property(e => e.PrecipitationMedNorm).HasColumnName("precipitation_med_norm");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.SolarRadiationAvgNorm).HasColumnName("solar_radiation_avg_norm");
            entity.Property(e => e.SolarRadiationMedNorm).HasColumnName("solar_radiation_med_norm");
            entity.Property(e => e.TemperatureAvgNorm).HasColumnName("temperature_avg_norm");
            entity.Property(e => e.TemperatureMedNorm).HasColumnName("temperature_med_norm");
        });

        modelBuilder.Entity<AgroClimaticZonesWeatherRate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("agro_climatic_zones_weather_rates_pkey");

            entity.ToTable("agro_climatic_zones_weather_rates");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.PrecRate).HasColumnName("prec_rate");
            entity.Property(e => e.TempRate).HasColumnName("temp_rate");
        });

        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("alerts_pkey");

            entity.ToTable("alerts");

            entity.HasIndex(e => new { e.RegionId, e.ProviderId, e.Date, e.AgroClimaticZonesId, e.TypeId }, "unique_region_provider_zone_date_type_id").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AgroClimaticZonesId).HasColumnName("agro_climatic_zones_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<AlertType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("alert_types_pkey");

            entity.ToTable("alert_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DescriptionEn)
                .HasMaxLength(256)
                .HasColumnName("description_en");
            entity.Property(e => e.DescriptionUz)
                .HasMaxLength(256)
                .HasColumnName("description_uz");
            entity.Property(e => e.FieldName)
                .HasMaxLength(255)
                .HasColumnName("field_name");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(64)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(64)
                .HasColumnName("name_uz");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<ApplicationProperty>(entity =>
        {
            entity.HasKey(e => new { e.Application, e.Key }).HasName("application_properties_pkey");

            entity.ToTable("application_properties");

            entity.Property(e => e.Application)
                .HasMaxLength(255)
                .HasColumnName("application");
            entity.Property(e => e.Key)
                .HasMaxLength(255)
                .HasColumnName("key");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Condition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("conditions_pkey");

            entity.ToTable("conditions");

            entity.HasIndex(e => new { e.ProviderId, e.ProviderCode }, "unique_provider_and_code").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.ProviderCode).HasColumnName("provider_code");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
        });

        modelBuilder.Entity<Frequency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("frequencies_pkey");

            entity.ToTable("frequencies");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ImageType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("image_types_pkey");

            entity.ToTable("image_types");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
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

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("jobs_pkey");

            entity.ToTable("jobs");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(255)
                .HasColumnName("action");
            entity.Property(e => e.Application)
                .HasMaxLength(255)
                .HasColumnName("application");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("date");
            entity.Property(e => e.JobStatusId).HasColumnName("job_status_id");
            entity.Property(e => e.Path)
                .HasMaxLength(255)
                .HasColumnName("path");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.Result)
                .HasMaxLength(255)
                .HasColumnName("result");
        });

        modelBuilder.Entity<JobStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("job_statuses_pkey");

            entity.ToTable("job_statuses");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(256)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(256)
                .HasColumnName("name_uz");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("logs_pkey");

            entity.ToTable("logs");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.ContentSize).HasColumnName("content_size");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("date");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.JobStatus)
                .HasMaxLength(255)
                .HasColumnName("job_status");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.UserInfo).HasColumnName("user_info");
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("providers_pkey");

            entity.ToTable("providers");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Editable)
                .HasDefaultValue(false)
                .HasColumnName("editable");
            entity.Property(e => e.FrequencyId).HasColumnName("frequency_id");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.Z).HasColumnName("z");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("regions_pkey");

            entity.ToTable("regions");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.ColumnY).HasColumnName("column_y");
            entity.Property(e => e.Iso)
                .HasMaxLength(6)
                .HasColumnName("iso");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Lon).HasColumnName("lon");
            entity.Property(e => e.Mhobt)
                .HasMaxLength(256)
                .HasColumnName("mhobt");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Polygon).HasColumnName("polygon");
            entity.Property(e => e.PolygonGeom).HasColumnName("polygon_geom");
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
            entity.Property(e => e.RowX).HasColumnName("row_x");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<RegionsAgroClimaticZone>(entity =>
        {
            entity.HasKey(e => new { e.RegionId, e.AgroClimaticZonesId }).HasName("regions_agro_climatic_zones_pkey");

            entity.ToTable("regions_agro_climatic_zones");

            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.AgroClimaticZonesId).HasColumnName("agro_climatic_zones_id");
        });

        modelBuilder.Entity<RegionsDump>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("regions_dump_pkey");

            entity.ToTable("regions_dump");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.ColumnY).HasColumnName("column_y");
            entity.Property(e => e.Iso)
                .HasMaxLength(6)
                .HasColumnName("iso");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Lon).HasColumnName("lon");
            entity.Property(e => e.Mhobt)
                .HasMaxLength(256)
                .HasColumnName("mhobt");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Polygon).HasColumnName("polygon");
            entity.Property(e => e.PolygonGeom).HasColumnName("polygon_geom");
            entity.Property(e => e.RegionLevel).HasColumnName("region_level");
            entity.Property(e => e.RegionName)
                .HasMaxLength(256)
                .HasColumnName("region_name");
            entity.Property(e => e.RowX).HasColumnName("row_x");
        });

        modelBuilder.Entity<RegionsWeather>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("regions_weathers_pkey");

            entity.ToTable("regions_weathers");

            entity.HasIndex(e => new { e.RegionId, e.ProviderId, e.Date, e.AgroClimaticZoneId }, "unique_region_provider_zone_date").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AgroClimaticZoneId).HasColumnName("agro_climatic_zone_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.PrecDevMax).HasColumnName("prec_dev_max");
            entity.Property(e => e.PrecDevMin).HasColumnName("prec_dev_min");
            entity.Property(e => e.Precipitation).HasColumnName("precipitation");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.Temp).HasColumnName("temp");
            entity.Property(e => e.TempDevMax).HasColumnName("temp_dev_max");
            entity.Property(e => e.TempDevMin).HasColumnName("temp_dev_min");
        });

        modelBuilder.Entity<RegionsWeather1>(entity =>
        {
            entity.HasKey(e => new { e.RegionId, e.ProviderId, e.Date }).HasName("regions_weather_pkey");

            entity.ToTable("regions_weather");

            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.PrecDev).HasColumnName("prec_dev");
            entity.Property(e => e.Precipitation).HasColumnName("precipitation");
            entity.Property(e => e.Temp).HasColumnName("temp");
            entity.Property(e => e.TempDev).HasColumnName("temp_dev");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reports_pkey");

            entity.ToTable("reports");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Link)
                .HasMaxLength(256)
                .HasColumnName("link");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.ReportType).HasColumnName("report_type");
            entity.Property(e => e.SourceQuery).HasColumnName("source_query");
            entity.Property(e => e.UpdateDate).HasColumnName("update_date");
        });

        modelBuilder.Entity<ReportType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("report_types_pkey");

            entity.ToTable("report_types");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Css).HasColumnName("css");
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

        modelBuilder.Entity<VAlertsFrozen>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_alerts_frozen");

            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.Areapercent).HasColumnName("areapercent");
            entity.Property(e => e.DisasterId).HasColumnName("disaster_id");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
        });

        modelBuilder.Entity<WeatherDatesCompleted>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("weather_dates_completed");

            entity.HasIndex(e => e.Date, "weather_dates_completed_date_key").IsUnique();

            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.FinishedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("finished_at");
        });

        modelBuilder.Entity<WeatherStation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("weather_stations_pkey");

            entity.ToTable("weather_stations");

            entity.HasIndex(e => e.Code, "ix_weather_stations_code");

            entity.HasIndex(e => new { e.Lat, e.Lon }, "ix_weather_stations_lat_lon");

            entity.HasIndex(e => new { e.ProviderId, e.X, e.Y }, "unique_x_y_weather_stations").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Application)
                .HasMaxLength(256)
                .HasColumnName("application");
            entity.Property(e => e.Code)
                .HasMaxLength(64)
                .HasColumnName("code");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Lon).HasColumnName("lon");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.X).HasColumnName("x");
            entity.Property(e => e.Y).HasColumnName("y");
        });

        modelBuilder.Entity<WeatherStationAgroClimaticZone>(entity =>
        {
            entity.HasKey(e => e.WeatherStationId).HasName("weather_station_agro_climatic_zones_pkey");

            entity.ToTable("weather_station_agro_climatic_zones");

            entity.HasIndex(e => e.AgroClimaticZonesId, "agro_climatic_zone_index");

            entity.Property(e => e.WeatherStationId)
                .ValueGeneratedNever()
                .HasColumnName("weather_station_id");
            entity.Property(e => e.AgroClimaticZonesId).HasColumnName("agro_climatic_zones_id");
        });

        modelBuilder.Entity<WeatherStationsDataEx>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("weather_stations_data_ex_pkey");

            entity.ToTable("weather_stations_data_ex");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.HumiditySoil2m).HasColumnName("humidity_soil_2m");
            entity.Property(e => e.HumiditySoil50cm).HasColumnName("humidity_soil_50cm");
            entity.Property(e => e.Temp1030cm).HasColumnName("temp_10_30cm");
            entity.Property(e => e.Temp10cmUnder).HasColumnName("temp_10cm_under");
            entity.Property(e => e.Temp30100cm).HasColumnName("temp_30_100cm");
            entity.Property(e => e.Temperature1mAbove).HasColumnName("temperature_1m_above");
            entity.Property(e => e.Temperature2mUnder).HasColumnName("temperature_2m_under");
            entity.Property(e => e.TemperatureGroundLevel).HasColumnName("temperature_ground_level");
            entity.Property(e => e.TemperatureSoil).HasColumnName("temperature_soil");
        });

        modelBuilder.Entity<WeatherStationsDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("weather_stations_data_pkey");

            entity.ToTable("weather_stations_data");

            entity.HasIndex(e => new { e.WeatherStationId, e.Date }, "unique_weather_station_date").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CloudCover).HasColumnName("cloud_cover");
            entity.Property(e => e.ConditionCode).HasColumnName("condition_code");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.DewPoint).HasColumnName("dew_point");
            entity.Property(e => e.HumidityMax).HasColumnName("humidity_max");
            entity.Property(e => e.HumidityMin).HasColumnName("humidity_min");
            entity.Property(e => e.Precipitation).HasColumnName("precipitation");
            entity.Property(e => e.RelativeHumidity).HasColumnName("relative_humidity");
            entity.Property(e => e.SolarRadiation).HasColumnName("solar_radiation");
            entity.Property(e => e.Temperature).HasColumnName("temperature");
            entity.Property(e => e.TemperatureMax).HasColumnName("temperature_max");
            entity.Property(e => e.TemperatureMin).HasColumnName("temperature_min");
            entity.Property(e => e.WeatherStationId).HasColumnName("weather_station_id");
            entity.Property(e => e.WindDirection).HasColumnName("wind_direction");
            entity.Property(e => e.WindSpeed).HasColumnName("wind_speed");
            entity.Property(e => e.WindSpeedMax).HasColumnName("wind_speed_max");
            entity.Property(e => e.WindSpeedMin).HasColumnName("wind_speed_min");
            entity.Property(e => e.Windchill).HasColumnName("windchill");
        });

        modelBuilder.Entity<WeatherType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("weather_types_pkey");

            entity.ToTable("weather_types");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Gis)
                .HasDefaultValue(false)
                .HasColumnName("gis");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.NameEn)
                .HasMaxLength(64)
                .HasColumnName("name_en");
            entity.Property(e => e.NameUz)
                .HasMaxLength(64)
                .HasColumnName("name_uz");
            entity.Property(e => e.TiledMap)
                .HasDefaultValue(false)
                .HasColumnName("tiled_map");
        });

        modelBuilder.Entity<XslsUzUnputColumn>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("xsls_uz_unput_columns");

            entity.Property(e => e.DbColumnName)
                .HasMaxLength(32)
                .HasColumnName("db_column_name");
            entity.Property(e => e.DbTableName)
                .HasMaxLength(32)
                .HasColumnName("db_table_name");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.XlsColumnName)
                .HasMaxLength(128)
                .HasColumnName("xls_column_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
