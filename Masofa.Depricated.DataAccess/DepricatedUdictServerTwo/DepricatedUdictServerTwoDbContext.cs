using System;
using System.Collections.Generic;
using Masofa.Depricated.DataAccess;
using Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;
using Microsoft.EntityFrameworkCore;

namespace Masofa.Depricated.DataAccess.DepricatedUdictServerTwo;

public partial class DepricatedUdictServerTwoDbContext : DbContext
{
    public DepricatedUdictServerTwoDbContext()
    {
    }

    public DepricatedUdictServerTwoDbContext(DbContextOptions<DepricatedUdictServerTwoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdministrativeUnit> AdministrativeUnits { get; set; }

    public virtual DbSet<AgroMachineType> AgroMachineTypes { get; set; }

    public virtual DbSet<AgroOperation> AgroOperations { get; set; }

    public virtual DbSet<AgroTerm> AgroTerms { get; set; }

    public virtual DbSet<AgroclimaticZone> AgroclimaticZones { get; set; }

    public virtual DbSet<AgrotechnicalMeasure> AgrotechnicalMeasures { get; set; }

    public virtual DbSet<AttachedFile> AttachedFiles { get; set; }

    public virtual DbSet<BidContent> BidContents { get; set; }

    public virtual DbSet<BidState> BidStates { get; set; }

    public virtual DbSet<BidType> BidTypes { get; set; }

    public virtual DbSet<BusinessType> BusinessTypes { get; set; }

    public virtual DbSet<ClimaticStandard> ClimaticStandards { get; set; }

    public virtual DbSet<Crop> Crops { get; set; }

    public virtual DbSet<CropPeriod> CropPeriods { get; set; }

    public virtual DbSet<CropPeriodVegetationIndex> CropPeriodVegetationIndexes { get; set; }

    public virtual DbSet<DataType> DataTypes { get; set; }

    public virtual DbSet<DicitonaryType> DicitonaryTypes { get; set; }

    public virtual DbSet<Dictionary> Dictionaries { get; set; }

    public virtual DbSet<DictionaryCell> DictionaryCells { get; set; }

    public virtual DbSet<DictionaryColumn> DictionaryColumns { get; set; }

    public virtual DbSet<DictionaryRow> DictionaryRows { get; set; }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<EfMigration> EfMigrations { get; set; }

    public virtual DbSet<EntomophageType> EntomophageTypes { get; set; }

    public virtual DbSet<ExperimentalFarmingMethod> ExperimentalFarmingMethods { get; set; }

    public virtual DbSet<Fertilizer> Fertilizers { get; set; }

    public virtual DbSet<FertilizerType> FertilizerTypes { get; set; }

    public virtual DbSet<FieldUsageStatus> FieldUsageStatuses { get; set; }

    public virtual DbSet<Firm> Firms { get; set; }

    public virtual DbSet<FlightTarget> FlightTargets { get; set; }

    public virtual DbSet<IrrigationMethod> IrrigationMethods { get; set; }

    public virtual DbSet<IrrigationSource> IrrigationSources { get; set; }

    public virtual DbSet<LegacyRegion> LegacyRegions { get; set; }

    public virtual DbSet<MeasurementUnit> MeasurementUnits { get; set; }

    public virtual DbSet<MeliorativeMeasureType> MeliorativeMeasureTypes { get; set; }

    public virtual DbSet<ObjectTag> ObjectTags { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    public virtual DbSet<PersonsTmp> PersonsTmps { get; set; }

    public virtual DbSet<PestType> PestTypes { get; set; }

    public virtual DbSet<Pesticide> Pesticides { get; set; }

    public virtual DbSet<PesticideType> PesticideTypes { get; set; }

    public virtual DbSet<ProductQualityStandard> ProductQualityStandards { get; set; }

    public virtual DbSet<ProviderWeatherCondition> ProviderWeatherConditions { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<RegionMap> RegionMaps { get; set; }

    public virtual DbSet<RegionType> RegionTypes { get; set; }

    public virtual DbSet<SoilType> SoilTypes { get; set; }

    public virtual DbSet<SolarRadiationInfluence> SolarRadiationInfluences { get; set; }

    public virtual DbSet<SystemDataSource> SystemDataSources { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.TaskStatus> TaskStatuses { get; set; }

    public virtual DbSet<UavCameraType> UavCameraTypes { get; set; }

    public virtual DbSet<UavDataType> UavDataTypes { get; set; }

    public virtual DbSet<Variety> Varieties { get; set; }

    public virtual DbSet<VarietyFeature> VarietyFeatures { get; set; }

    public virtual DbSet<VegetationIndex> VegetationIndexes { get; set; }

    public virtual DbSet<VegetationPeriod> VegetationPeriods { get; set; }

    public virtual DbSet<WaterResource> WaterResources { get; set; }

    public virtual DbSet<WeatherStation> WeatherStations { get; set; }

    public virtual DbSet<WeatherStationType> WeatherStationTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=217.29.121.2;Port=5432;Database=udict;Username=postgres;Password=W4xZ9bNmR2tY", x => x
                .UseNodaTime()
                .UseNetTopologySuite());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<AdministrativeUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_administrative_units");

            entity.ToTable("administrative_units", "dictionaries", tb => tb.HasComment("Справочник типов административных единиц"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.Level)
                .HasComment("Уровень иерархии")
                .HasColumnName("level");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<AgroMachineType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_agro_machine_types");

            entity.ToTable("agro_machine_types", "dictionaries", tb => tb.HasComment("Справочник видов сельскохозяйственной техники"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.IsSoilSafe)
                .HasComment("Признак почовосберегающей техники")
                .HasColumnName("is_soil_safe");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<AgroOperation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_agro_operations");

            entity.ToTable("agro_operations", "dictionaries", tb => tb.HasComment("Справочник сельскохозяйственных операций"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<AgroTerm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_agro_terms");

            entity.ToTable("agro_terms", "dictionaries", tb => tb.HasComment("Справочник аграрных терминов и их определений"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.DescrEn)
                .HasComment("Описание на английском")
                .HasColumnName("descr_en");
            entity.Property(e => e.DescrRu)
                .HasComment("Описание на русском")
                .HasColumnName("descr_ru");
            entity.Property(e => e.DescrUz)
                .HasComment("Описание на узбекском")
                .HasColumnName("descr_uz");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<AgroclimaticZone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_agroclimatic_zones");

            entity.ToTable("agroclimatic_zones", "dictionaries", tb => tb.HasComment("Справочник агроклиматических зон"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<AgrotechnicalMeasure>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_agrotechnical_measures");

            entity.ToTable("agrotechnical_measures", "dictionaries", tb => tb.HasComment("Справочник агротехнических мероприятий"));

            entity.HasIndex(e => e.CropId, "ix_agrotechnical_measures_crop_id");

            entity.HasIndex(e => e.VarietyId, "ix_agrotechnical_measures_variety_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.CropId)
                .HasComment("Идентификатор культуры")
                .HasColumnName("crop_id");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.VarietyId)
                .HasComment("Идентификатор сорта")
                .HasColumnName("variety_id");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.Crop).WithMany(p => p.AgrotechnicalMeasures)
            //    .HasForeignKey(d => d.CropId)
            //    .HasConstraintName("fk_agrotechnical_measures_crops_crop_id");

            //entity.HasOne(d => d.Variety).WithMany(p => p.AgrotechnicalMeasures)
            //    .HasForeignKey(d => d.VarietyId)
            //    .HasConstraintName("fk_agrotechnical_measures_varieties_variety_id");
        });

        modelBuilder.Entity<AttachedFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_attached_files");

            entity.ToTable("attached_files", tb => tb.HasComment("Файлы, прикрепленные к записям справочников"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.Description)
                .HasComment("Описание")
                .HasColumnName("description");
            entity.Property(e => e.DictionaryItemId)
                .HasComment("Идентификатор связанной записи справочника")
                .HasColumnName("dictionary_item_id");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.FileExtension)
                .HasComment("Расширение файла")
                .HasColumnName("file_extension");
            entity.Property(e => e.FileName)
                .HasComment("Имя файла (без расширения)")
                .HasColumnName("file_name");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<BidContent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_bid_contents");

            entity.ToTable("bid_contents", "dictionaries", tb => tb.HasComment("Справочник содержаний заявок"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<BidState>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_bid_states");

            entity.ToTable("bid_states", "dictionaries", tb => tb.HasComment("Справочник статусов заявок"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<BidType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_bid_types");

            entity.ToTable("bid_types", "dictionaries", tb => tb.HasComment("Справочник типов заявок"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<BusinessType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_business_types");

            entity.ToTable("business_types", "dictionaries", tb => tb.HasComment("Справочник видов деятельности юридических и физических лиц"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasMany(d => d.Firms).WithMany(p => p.BusinessTypes)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "BusinessTypeFirm",
            //        r => r.HasOne<Firm>().WithMany()
            //            .HasForeignKey("FirmsId")
            //            .HasConstraintName("fk_business_type_firm_firms_firms_id"),
            //        l => l.HasOne<BusinessType>().WithMany()
            //            .HasForeignKey("BusinessTypesId")
            //            .HasConstraintName("fk_business_type_firm_business_types_business_types_id"),
            //        j =>
            //        {
            //            j.HasKey("BusinessTypesId", "FirmsId").HasName("pk_business_type_firm");
            //            j.ToTable("business_type_firm", "dictionaries");
            //            j.HasIndex(new[] { "FirmsId" }, "ix_business_type_firm_firms_id");
            //            j.IndexerProperty<Guid>("BusinessTypesId").HasColumnName("business_types_id");
            //            j.IndexerProperty<Guid>("FirmsId").HasColumnName("firms_id");
            //        });

            //entity.HasMany(d => d.Persons).WithMany(p => p.BusinessTypes)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "BusinessTypePerson",
            //        r => r.HasOne<Person>().WithMany()
            //            .HasForeignKey("PersonsId")
            //            .HasConstraintName("fk_business_type_person_persons_persons_id"),
            //        l => l.HasOne<BusinessType>().WithMany()
            //            .HasForeignKey("BusinessTypesId")
            //            .HasConstraintName("fk_business_type_person_business_types_business_types_id"),
            //        j =>
            //        {
            //            j.HasKey("BusinessTypesId", "PersonsId").HasName("pk_business_type_person");
            //            j.ToTable("business_type_person", "dictionaries");
            //            j.HasIndex(new[] { "PersonsId" }, "ix_business_type_person_persons_id");
            //            j.IndexerProperty<Guid>("BusinessTypesId").HasColumnName("business_types_id");
            //            j.IndexerProperty<Guid>("PersonsId").HasColumnName("persons_id");
            //        });
        });

        modelBuilder.Entity<ClimaticStandard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_climatic_standards");

            entity.ToTable("climatic_standards", "dictionaries", tb => tb.HasComment("Справочник климатических норм"));

            entity.HasIndex(e => e.RegionId, "ix_climatic_standards_region_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.CoefSel)
                .HasComment("Коэффициент Селянинова")
                .HasColumnName("coef_sel");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.Day)
                .HasComment("Число")
                .HasColumnName("day");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.HumAvg)
                .HasComment("Среднее значение влажности")
                .HasColumnName("hum_avg");
            entity.Property(e => e.Month)
                .HasComment("Месяц")
                .HasColumnName("month");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.PrecDayAvg)
                .HasComment("Суммарное значение накопленных осадков")
                .HasColumnName("prec_day_avg");
            entity.Property(e => e.RadDayAvg)
                .HasComment("Суммарное значение накопленной солнечной радиции")
                .HasColumnName("rad_day_avg");
            entity.Property(e => e.RegionId)
                .HasComment("Идентификатор региона")
                .HasColumnName("region_id");
            entity.Property(e => e.TempAvg)
                .HasComment("Средняя температура")
                .HasColumnName("temp_avg");
            entity.Property(e => e.TempMax)
                .HasComment("Температура максимум")
                .HasColumnName("temp_max");
            entity.Property(e => e.TempMin)
                .HasComment("Температура минимум")
                .HasColumnName("temp_min");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.Region).WithMany(p => p.ClimaticStandards)
            //    .HasForeignKey(d => d.RegionId)
            //    .HasConstraintName("fk_climatic_standards_regions_region_id");
        });

        modelBuilder.Entity<Crop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_crops");

            entity.ToTable("crops", "dictionaries", tb => tb.HasComment("Справочник культур"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.IsMonitoring)
                .HasComment("Признак осуществления мониторинга")
                .HasColumnName("is_monitoring");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameLa)
                .HasComment("Наименование на латыни")
                .HasColumnName("name_la");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<CropPeriod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_crop_periods");

            entity.ToTable("crop_periods", "dictionaries", tb => tb.HasComment("Справочник периодов развития культур"));

            entity.HasIndex(e => e.CropId, "ix_crop_periods_crop_id");

            entity.HasIndex(e => e.RegionId, "ix_crop_periods_region_id");

            entity.HasIndex(e => e.VarietyId, "ix_crop_periods_variety_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.CropId)
                .HasComment("Идентификатор культуры")
                .HasColumnName("crop_id");
            entity.Property(e => e.DayEnd)
                .HasComment("День окончания периода")
                .HasColumnName("day_end");
            entity.Property(e => e.DayStart)
                .HasComment("День начала периода")
                .HasColumnName("day_start");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.RegionId)
                .HasComment("Идентификатор региона")
                .HasColumnName("region_id");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.VarietyId)
                .HasComment("Идентификатор сорта")
                .HasColumnName("variety_id");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.Crop).WithMany(p => p.CropPeriods)
            //    .HasForeignKey(d => d.CropId)
            //    .HasConstraintName("fk_crop_periods_crops_crop_id");

            //entity.HasOne(d => d.Region).WithMany(p => p.CropPeriods)
            //    .HasForeignKey(d => d.RegionId)
            //    .HasConstraintName("fk_crop_periods_regions_region_id");

            //entity.HasOne(d => d.Variety).WithMany(p => p.CropPeriods)
            //    .HasForeignKey(d => d.VarietyId)
            //    .HasConstraintName("fk_crop_periods_varieties_variety_id");
        });

        modelBuilder.Entity<CropPeriodVegetationIndex>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_crop_period_vegetation_indexes");

            entity.ToTable("crop_period_vegetation_indexes", "dictionaries", tb => tb.HasComment("Справочник индексов периодов развития культур"));

            entity.HasIndex(e => new { e.CropPeriodId, e.VegetationIndexId }, "ix_crop_period_vegetation_indexes_crop_period_id_vegetation_in").IsUnique();

            entity.HasIndex(e => e.VegetationIndexId, "ix_crop_period_vegetation_indexes_vegetation_index_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.CropPeriodId)
                .HasComment("Идентификатор периода развития культуры")
                .HasColumnName("crop_period_id");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.Max)
                .HasComment("Максимальное значение индекса")
                .HasColumnName("max");
            entity.Property(e => e.Min)
                .HasComment("Минимальное значение индекса")
                .HasColumnName("min");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Value)
                .HasComment("Значение индекса")
                .HasColumnName("value");
            entity.Property(e => e.VegetationIndexId)
                .HasComment("Идентификатор вегетационного индекса")
                .HasColumnName("vegetation_index_id");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.CropPeriod).WithMany(p => p.CropPeriodVegetationIndices)
            //    .HasForeignKey(d => d.CropPeriodId)
            //    .HasConstraintName("fk_crop_period_vegetation_indexes_crop_periods_crop_period_id");

            //entity.HasOne(d => d.VegetationIndex).WithMany(p => p.CropPeriodVegetationIndices)
            //    .HasForeignKey(d => d.VegetationIndexId)
            //    .HasConstraintName("fk_crop_period_vegetation_indexes_vegetation_indexes_vegetatio");
        });

        modelBuilder.Entity<DataType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("data_types_pkey");

            entity.ToTable("data_types");

            entity.HasIndex(e => e.Name, "uix_data_types_name").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.SqlType)
                .HasMaxLength(16)
                .HasColumnName("sql_type");
        });

        modelBuilder.Entity<DicitonaryType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_dicitonary_types");

            entity.ToTable("dicitonary_types", "dictionaries", tb => tb.HasComment("Справочник типов справочников"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<Dictionary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dictionaries_pkey");

            entity.ToTable("dictionaries");

            entity.HasIndex(e => e.Name, "uix_dictionaries_name").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
        });

        modelBuilder.Entity<DictionaryCell>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dictionary_cells_pkey");

            entity.ToTable("dictionary_cells");

            entity.HasIndex(e => new { e.DictionaryId, e.ColumnId }, "ix_dictionary_cells_dictionary_id_column_id");

            entity.HasIndex(e => new { e.DictionaryId, e.ColumnId, e.RowId }, "ix_dictionary_cells_dictionary_id_column_id_row_id");

            entity.HasIndex(e => e.DictionaryId, "ix_dictionary_cells_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CellData).HasColumnName("cell_data");
            entity.Property(e => e.ColumnId).HasColumnName("column_id");
            entity.Property(e => e.DictionaryId).HasColumnName("dictionary_id");
            entity.Property(e => e.RowId).HasColumnName("row_id");
        });

        modelBuilder.Entity<DictionaryColumn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dictionary_columns_pkey");

            entity.ToTable("dictionary_columns");

            entity.HasIndex(e => e.DictionaryId, "ix_dictionary_columnss_dictionary_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.DataType).HasColumnName("data_type");
            entity.Property(e => e.DictionaryId).HasColumnName("dictionary_id");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
        });

        modelBuilder.Entity<DictionaryRow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dictionary_rows_pkey");

            entity.ToTable("dictionary_rows");

            entity.HasIndex(e => e.DictionaryId, "ix_dictionary_rows_dictionary_id");

            entity.HasIndex(e => e.ParentRowId, "ix_dictionary_rows_parent_row_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.DictionaryId).HasColumnName("dictionary_id");
            entity.Property(e => e.ParentRowId).HasColumnName("parent_row_id");
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_diseases");

            entity.ToTable("diseases", "dictionaries", tb => tb.HasComment("Справочник болезней растений"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameLa)
                .HasComment("Наименование на латыни")
                .HasColumnName("name_la");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<EfMigration>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("pk__ef_migrations");

            entity.ToTable("_ef_migrations");

            entity.Property(e => e.MigrationId)
                .HasMaxLength(150)
                .HasColumnName("migration_id");
            entity.Property(e => e.ProductVersion)
                .HasMaxLength(32)
                .HasColumnName("product_version");
        });

        modelBuilder.Entity<EntomophageType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_entomophage_types");

            entity.ToTable("entomophage_types", "dictionaries", tb => tb.HasComment("Справочник энтомофагов"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameLa)
                .HasComment("Наименование на латыни")
                .HasColumnName("name_la");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<ExperimentalFarmingMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_experimental_farming_methods");

            entity.ToTable("experimental_farming_methods", "dictionaries", tb => tb.HasComment("Справочник видов эксперементальных способов земледелия"));

            entity.HasIndex(e => e.CropId, "ix_experimental_farming_methods_crop_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.CropId)
                .HasComment("Идентификатор культуры")
                .HasColumnName("crop_id");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.Name)
                .HasComment("Наименование")
                .HasColumnName("name");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.Crop).WithMany(p => p.ExperimentalFarmingMethods)
            //    .HasForeignKey(d => d.CropId)
            //    .HasConstraintName("fk_experimental_farming_methods_crops_crop_id");
        });

        modelBuilder.Entity<Fertilizer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_fertilizers");

            entity.ToTable("fertilizers", "dictionaries", tb => tb.HasComment("Справочник удобрений"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.IsEco)
                .HasComment("Признак экологически чистого удобрения")
                .HasColumnName("is_eco");
            entity.Property(e => e.IsOrganic)
                .HasComment("Признак органического удобрения")
                .HasColumnName("is_organic");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<FertilizerType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_fertilizer_types");

            entity.ToTable("fertilizer_types", "dictionaries", tb => tb.HasComment("Справочник типов удобрений"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<FieldUsageStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_field_usage_statuses");

            entity.ToTable("field_usage_statuses", "dictionaries", tb => tb.HasComment("Справочник статусов использования поля"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<Firm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_firms");

            entity.ToTable("firms", "dictionaries", tb => tb.HasComment("Справочник юридических лиц"));

            entity.HasIndex(e => e.MainRegionId, "ix_firms_main_region_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Chief)
                .HasComment("Руководитель")
                .HasColumnName("chief");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.Egrpo)
                .HasComment("Регистрационный номер ЮЛ (ЕГРПО)")
                .HasColumnName("egrpo");
            entity.Property(e => e.Email)
                .HasComment("Email")
                .HasColumnName("email");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.FullName)
                .HasComment("Полное наименование")
                .HasColumnName("full_name");
            entity.Property(e => e.Inn)
                .HasComment("Налоговый номер ЮЛ (ИНН)")
                .HasColumnName("inn");
            entity.Property(e => e.InternationalName)
                .HasComment("Международное наименование")
                .HasColumnName("international_name");
            entity.Property(e => e.MailingAddress)
                .HasComment("Почтовый адрес")
                .HasColumnName("mailing_address");
            entity.Property(e => e.MainRegionId)
                .HasComment("Основной регион")
                .HasColumnName("main_region_id");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.Phones)
                .HasComment("Телефоны")
                .HasColumnName("phones");
            entity.Property(e => e.PhysicalAddress)
                .HasComment("Адрес нахождения")
                .HasColumnName("physical_address");
            entity.Property(e => e.ShortName)
                .HasDefaultValueSql("''::text")
                .HasComment("Краткое наименование")
                .HasColumnName("short_name");
            entity.Property(e => e.Site)
                .HasComment("Сайт")
                .HasColumnName("site");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.MainRegion).WithMany(p => p.Firms)
            //    .HasForeignKey(d => d.MainRegionId)
            //    .HasConstraintName("fk_firms_regions_main_region_id");
        });

        modelBuilder.Entity<FlightTarget>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_flight_targets");

            entity.ToTable("flight_targets", "dictionaries", tb => tb.HasComment("Справочник целей облета"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<IrrigationMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_irrigation_methods");

            entity.ToTable("irrigation_methods", "dictionaries", tb => tb.HasComment("Справочник по технологиям орошения"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.IsWaterSafe)
                .HasComment("Водосберегающие технологии")
                .HasColumnName("is_water_safe");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<IrrigationSource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_irrigation_sources");

            entity.ToTable("irrigation_sources", "dictionaries", tb => tb.HasComment("Справочник источников орошения"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<LegacyRegion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("legacy_regions_pkey");

            entity.ToTable("legacy_regions", "dictionaries");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.ColumnY).HasColumnName("column_y");
            entity.Property(e => e.Iso).HasColumnName("iso");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Lon).HasColumnName("lon");
            entity.Property(e => e.MapId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("map_id");
            entity.Property(e => e.Mhobt).HasColumnName("mhobt");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Polygon).HasColumnName("polygon");
            entity.Property(e => e.PolygonGeom).HasColumnName("polygon_geom");
            entity.Property(e => e.RegionLevel).HasColumnName("region_level");
            entity.Property(e => e.RegionName).HasColumnName("region_name");
            entity.Property(e => e.RegionNameEn).HasColumnName("region_name_en");
            entity.Property(e => e.RegionNameUz).HasColumnName("region_name_uz");
            entity.Property(e => e.RowX).HasColumnName("row_x");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<MeasurementUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_measurement_units");

            entity.ToTable("measurement_units", "dictionaries", tb => tb.HasComment("Справочник единиц измерения"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.Factor)
                .HasComment("Коэффициент (множитель) единицы СИ")
                .HasColumnName("factor");
            entity.Property(e => e.FullNameEn)
                .HasComment("Полное наименование на английском")
                .HasColumnName("full_name_en");
            entity.Property(e => e.FullNameRu)
                .HasComment("Полное наименование на русском")
                .HasColumnName("full_name_ru");
            entity.Property(e => e.FullNameUz)
                .HasComment("Полное наименование на узбекском")
                .HasColumnName("full_name_uz");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.SiUnit)
                .HasComment("Единица СИ")
                .HasColumnName("si_unit");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<MeliorativeMeasureType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_meliorative_measure_types");

            entity.ToTable("meliorative_measure_types", "dictionaries", tb => tb.HasComment("Справочник типов мелиоративных мероприятий"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<ObjectTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_object_tags");

            entity.ToTable("object_tags", tb => tb.HasComment("Файлы, прикрепленные к записям справочников"));

            entity.HasIndex(e => new { e.ObjectId, e.TagId }, "ix_object_tags_object_id_tag_id").IsUnique();

            entity.HasIndex(e => e.TagId, "ix_object_tags_tag_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.ObjectId)
                .HasComment("Идентификатор связанной записи справочника")
                .HasColumnName("object_id");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            entity.HasOne(d => d.Tag).WithMany(p => p.ObjectTags)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("fk_object_tags_tags_tag_id");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_persons");

            entity.ToTable("persons", "dictionaries", tb => tb.HasComment("Справочник физических лиц"));

            entity.HasIndex(e => e.MainRegionId, "ix_persons_main_region_id");

            entity.HasIndex(e => e.UserId, "ix_persons_user_id").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.Email)
                .HasComment("Email")
                .HasColumnName("email");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.FirstName)
                .HasDefaultValueSql("''::text")
                .HasComment("Имя")
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasDefaultValueSql("''::text")
                .HasComment("Фамилия")
                .HasColumnName("last_name");
            entity.Property(e => e.MailingAddress)
                .HasComment("Почтовый адрес")
                .HasColumnName("mailing_address");
            entity.Property(e => e.MainRegionId)
                .HasComment("Основной регион")
                .HasColumnName("main_region_id");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.Patronymic)
                .HasComment("Отчество")
                .HasColumnName("patronymic");
            entity.Property(e => e.Phones)
                .HasComment("Телефоны")
                .HasColumnName("phones");
            entity.Property(e => e.PhysicalAddress)
                .HasComment("Адрес осуществления деятельности")
                .HasColumnName("physical_address");
            entity.Property(e => e.Pinfl)
                .HasComment("Налоговый номер ФЛ")
                .HasColumnName("pinfl");
            entity.Property(e => e.Telegram)
                .HasComment("Телеграм")
                .HasColumnName("telegram");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.UserId)
                .HasComment("Идентификатор пользователя в системе")
                .HasColumnName("user_id");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.MainRegion).WithMany(p => p.People)
            //    .HasForeignKey(d => d.MainRegionId)
            //    .HasConstraintName("fk_persons_regions_main_region_id");
        });

        modelBuilder.Entity<PersonsTmp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("_persons_tmp_pkey");

            entity.ToTable("_persons_tmp", "dictionaries");

            entity.HasIndex(e => e.UserId, "ix__persons_tmp_user_id").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasColumnName("create_user");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.ExtData).HasColumnName("ext_data");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.MailingAddress).HasColumnName("mailing_address");
            entity.Property(e => e.MainRegionId).HasColumnName("main_region_id");
            entity.Property(e => e.NameEn).HasColumnName("name_en");
            entity.Property(e => e.NameRu).HasColumnName("name_ru");
            entity.Property(e => e.NameUz).HasColumnName("name_uz");
            entity.Property(e => e.OrderCode).HasColumnName("order_code");
            entity.Property(e => e.Phones).HasColumnName("phones");
            entity.Property(e => e.PhysicalAddress).HasColumnName("physical_address");
            entity.Property(e => e.Pinfl).HasColumnName("pinfl");
            entity.Property(e => e.Telegram).HasColumnName("telegram");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasColumnName("update_user");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Visible).HasColumnName("visible");
        });

        modelBuilder.Entity<PestType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_pest_types");

            entity.ToTable("pest_types", "dictionaries", tb => tb.HasComment("Справочник видов вредителей"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameLa)
                .HasComment("Наименование на латыни")
                .HasColumnName("name_la");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<Pesticide>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_pesticides");

            entity.ToTable("pesticides", "dictionaries", tb => tb.HasComment("Справочник пестицидов и агрохимикатов"));

            entity.HasIndex(e => e.TypeId, "ix_pesticides_type_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.IntCode)
                .HasComment("Международный код")
                .HasColumnName("int_code");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.TypeId)
                .HasComment("Идентификатор вида пестицида")
                .HasColumnName("type_id");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.Type).WithMany(p => p.Pesticides)
            //    .HasForeignKey(d => d.TypeId)
            //    .HasConstraintName("fk_pesticides_pesticide_types_type_id");
        });

        modelBuilder.Entity<PesticideType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_pesticide_types");

            entity.ToTable("pesticide_types", "dictionaries", tb => tb.HasComment("Справочник видов пестицидов и агрохимикатов"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.IntCode)
                .HasComment("Международный код")
                .HasColumnName("int_code");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<ProductQualityStandard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_product_quality_standards");

            entity.ToTable("product_quality_standards", "dictionaries", tb => tb.HasComment("Справочник стандартов качества продукции"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<ProviderWeatherCondition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_provider_weather_conditions");

            entity.ToTable("provider_weather_conditions", "dictionaries", tb => tb.HasComment("Справочник погодных условий провайдеров"));

            entity.HasIndex(e => e.RegionId, "ix_provider_weather_conditions_region_id");

            entity.HasIndex(e => e.WeatherStationTypeId, "ix_provider_weather_conditions_weather_station_type_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.Lat)
                .HasComment("Широта")
                .HasColumnName("lat");
            entity.Property(e => e.Lng)
                .HasComment("Долгота")
                .HasColumnName("lng");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.Radius)
                .HasComment("Радиус покрытия")
                .HasColumnName("radius");
            entity.Property(e => e.RegionId)
                .HasComment("Идентификатор региона")
                .HasColumnName("region_id");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
            entity.Property(e => e.WeatherStationTypeId)
                .HasComment("Тип погодной станции")
                .HasColumnName("weather_station_type_id");

            //entity.HasOne(d => d.Region).WithMany(p => p.ProviderWeatherConditions)
            //    .HasForeignKey(d => d.RegionId)
            //    .HasConstraintName("fk_provider_weather_conditions_regions_region_id");

            //entity.HasOne(d => d.WeatherStationType).WithMany(p => p.ProviderWeatherConditions)
            //    .HasForeignKey(d => d.WeatherStationTypeId)
            //    .HasConstraintName("fk_provider_weather_conditions_weather_station_types_weather_s");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_regions");

            entity.ToTable("regions", "dictionaries", tb => tb.HasComment("Справочник регионов"));

            entity.HasIndex(e => e.AgroclimaticZoneId, "ix_regions_agroclimatic_zone_id");

            entity.HasIndex(e => e.TypeId, "ix_regions_type_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.AgroclimaticZoneId)
                .HasComment("Идентификатор агроклиматической зоны")
                .HasColumnName("agroclimatic_zone_id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.Level)
                .HasComment("Уровень административной единицы")
                .HasColumnName("level");
            entity.Property(e => e.NameAdminEn)
                .HasComment("Наименование админ ед на англ")
                .HasColumnName("name_admin_en");
            entity.Property(e => e.NameAdminRu)
                .HasComment("Наименование админ ед на рус")
                .HasColumnName("name_admin_ru");
            entity.Property(e => e.NameAdminUz)
                .HasComment("Наименование админ ед на узб")
                .HasColumnName("name_admin_uz");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameMhobt)
                .HasComment("Условное обозначение")
                .HasColumnName("name_mhobt");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.ParentId)
                .HasComment("Идентификатор региона")
                .HasColumnName("parent_id");
            entity.Property(e => e.Population)
                .HasComment("Население")
                .HasColumnName("population");
            entity.Property(e => e.ShortNameEn)
                .HasComment("Условное обозначение аббревиатура (en)")
                .HasColumnName("short_name_en");
            entity.Property(e => e.ShortNameRu)
                .HasComment("Условное обозначение аббревиатура (ru)")
                .HasColumnName("short_name_ru");
            entity.Property(e => e.ShortNameUz)
                .HasComment("Условное обозначение аббревиатура (uz)")
                .HasColumnName("short_name_uz");
            entity.Property(e => e.TypeId)
                .HasComment("Идентификатор типа региона")
                .HasColumnName("type_id");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.AgroclimaticZone).WithMany(p => p.Regions)
            //    .HasForeignKey(d => d.AgroclimaticZoneId)
            //    .HasConstraintName("fk_regions_agroclimatic_zones_agroclimatic_zone_id");

            //entity.HasOne(d => d.Type).WithMany(p => p.Regions)
            //    .HasForeignKey(d => d.TypeId)
            //    .HasConstraintName("fk_regions_region_types_type_id");

            //entity.HasMany(d => d.Varieties).WithMany(p => p.Regions)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "VarietyRegion",
            //        r => r.HasOne<Variety>().WithMany()
            //            .HasForeignKey("VarietyId")
            //            .HasConstraintName("fk_variety_regions_varieties_variety_id"),
            //        l => l.HasOne<Region>().WithMany()
            //            .HasForeignKey("RegionId")
            //            .HasConstraintName("fk_variety_regions_regions_region_id"),
            //        j =>
            //        {
            //            j.HasKey("RegionId", "VarietyId").HasName("pk_variety_regions");
            //            j.ToTable("variety_regions", "dictionaries");
            //            j.HasIndex(new[] { "VarietyId" }, "ix_variety_regions_variety_id");
            //            j.IndexerProperty<Guid>("RegionId").HasColumnName("region_id");
            //            j.IndexerProperty<Guid>("VarietyId").HasColumnName("variety_id");
            //        });
        });

        modelBuilder.Entity<RegionMap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_region_maps");

            entity.ToTable("region_maps", "dictionaries", tb => tb.HasComment("Справочник карт регионов"));

            entity.HasIndex(e => new { e.RegionId, e.ActiveTo }, "ix_region_maps_region_id_active_to")
                .IsUnique()
                .AreNullsDistinct(false);

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.ActiveFrom)
                .HasComment("Начало периода актуальности")
                .HasColumnName("active_from");
            entity.Property(e => e.ActiveTo)
                .HasComment("Окончание периода актуальности")
                .HasColumnName("active_to");
            entity.Property(e => e.AgriculturalArea)
                .HasComment("Полезная сельскохозяйственная площадь региона")
                .HasColumnName("agricultural_area");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.Lat)
                .HasComment("Широта")
                .HasColumnName("lat");
            entity.Property(e => e.Lng)
                .HasComment("Долгота")
                .HasColumnName("lng");
            entity.Property(e => e.MozaikX)
                .HasComment("По оси Х")
                .HasColumnName("mozaik_x");
            entity.Property(e => e.MozaikY)
                .HasComment("По оси Y")
                .HasColumnName("mozaik_y");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.Polygon)
                .HasComment("Полигон")
                .HasColumnName("polygon");
            entity.Property(e => e.PolygonAsText)
                .HasComment("Полигон в виде текста (JSON)")
                .HasColumnName("polygon_as_text");
            entity.Property(e => e.RegionId)
                .HasComment("Идентификатор региона")
                .HasColumnName("region_id");
            entity.Property(e => e.TotalArea)
                .HasComment("Общая площадь региона")
                .HasColumnName("total_area");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.Region).WithMany(p => p.RegionMaps)
            //    .HasForeignKey(d => d.RegionId)
            //    .HasConstraintName("fk_region_maps_regions_region_id");
        });

        modelBuilder.Entity<RegionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_region_types");

            entity.ToTable("region_types", "dictionaries", tb => tb.HasComment("Справочник типов регионов"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<SoilType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_soil_types");

            entity.ToTable("soil_types", "dictionaries", tb => tb.HasComment("Справочник типов почвы"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<SolarRadiationInfluence>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_solar_radiation_influences");

            entity.ToTable("solar_radiation_influences", "dictionaries", tb => tb.HasComment("Справочник данных по солнечной активности и её влиянию на урожайность культур"));

            entity.HasIndex(e => e.CropId, "ix_solar_radiation_influences_crop_id");

            entity.HasIndex(e => e.RegionId, "ix_solar_radiation_influences_region_id");

            entity.HasIndex(e => e.VarietyId, "ix_solar_radiation_influences_variety_id");

            entity.HasIndex(e => e.VegetationPeriodId, "ix_solar_radiation_influences_vegetation_period_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.CropId)
                .HasComment("Идентификатор культуры")
                .HasColumnName("crop_id");
            entity.Property(e => e.DayEnd)
                .HasComment("Окончание периода вегетации")
                .HasColumnName("day_end");
            entity.Property(e => e.DayStart)
                .HasComment("Начало периода вегетации")
                .HasColumnName("day_start");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.RadNorm)
                .HasComment("Оптимальное значение накопленной солнечной радиации")
                .HasColumnName("rad_norm");
            entity.Property(e => e.RegionId)
                .HasComment("Идентификатор региона")
                .HasColumnName("region_id");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.VarietyId)
                .HasComment("Идентификатор сорта")
                .HasColumnName("variety_id");
            entity.Property(e => e.VegetationPeriodId)
                .HasComment("Идентификатор периода вегетации")
                .HasColumnName("vegetation_period_id");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.Crop).WithMany(p => p.SolarRadiationInfluences)
            //    .HasForeignKey(d => d.CropId)
            //    .HasConstraintName("fk_solar_radiation_influences_crops_crop_id");

            //entity.HasOne(d => d.Region).WithMany(p => p.SolarRadiationInfluences)
            //    .HasForeignKey(d => d.RegionId)
            //    .HasConstraintName("fk_solar_radiation_influences_regions_region_id");

            //entity.HasOne(d => d.Variety).WithMany(p => p.SolarRadiationInfluences)
            //    .HasForeignKey(d => d.VarietyId)
            //    .HasConstraintName("fk_solar_radiation_influences_varieties_variety_id");

            //entity.HasOne(d => d.VegetationPeriod).WithMany(p => p.SolarRadiationInfluences)
            //    .HasForeignKey(d => d.VegetationPeriodId)
            //    .HasConstraintName("fk_solar_radiation_influences_vegetation_periods_vegetation_pe");
        });

        modelBuilder.Entity<SystemDataSource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_system_data_sources");

            entity.ToTable("system_data_sources", "dictionaries", tb => tb.HasComment("Справочник источников данных для Системы"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_tags");

            entity.ToTable("tags", tb => tb.HasComment("Ярлыки, прикрепленные к записям справочников"));

            entity.HasIndex(e => e.Name, "ix_tags_name").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.Description)
                .HasComment("Описание")
                .HasColumnName("description");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.Name)
                .HasComment("Имя ярлыка")
                .HasColumnName("name");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.TaskStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_task_statuses");

            entity.ToTable("task_statuses", "dictionaries", tb => tb.HasComment("Справочник статусов задач"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<UavCameraType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_uav_camera_types");

            entity.ToTable("uav_camera_types", "dictionaries", tb => tb.HasComment("Справочник типов камер БПЛА"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<UavDataType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_uav_data_types");

            entity.ToTable("uav_data_types", "dictionaries", tb => tb.HasComment("Справочник типов данных БПЛА"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<Variety>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_varieties");

            entity.ToTable("varieties", "dictionaries", tb => tb.HasComment("Справочник сортов"));

            entity.HasIndex(e => e.CropId, "ix_varieties_crop_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.AverageYield)
                .HasComment("Средняя урожайность (ц/Га)")
                .HasColumnName("average_yield");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.CropId)
                .HasComment("Идентификатор культуры")
                .HasColumnName("crop_id");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameLa)
                .HasComment("Наименование на латыни")
                .HasColumnName("name_la");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.RipeningPeriod)
                .HasComment("Срок созревания в днях")
                .HasColumnName("ripening_period");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.Crop).WithMany(p => p.Varieties)
            //    .HasForeignKey(d => d.CropId)
            //    .HasConstraintName("fk_varieties_crops_crop_id");
        });

        modelBuilder.Entity<VarietyFeature>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_variety_features");

            entity.ToTable("variety_features", "dictionaries", tb => tb.HasComment("Справочник дополнительных характеристик сортов"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasMany(d => d.Varieties).WithMany(p => p.Features)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "VarietyVarietyFeature",
            //        r => r.HasOne<Variety>().WithMany()
            //            .HasForeignKey("VarietiesId")
            //            .HasConstraintName("fk_variety_variety_feature_varieties_varieties_id"),
            //        l => l.HasOne<VarietyFeature>().WithMany()
            //            .HasForeignKey("FeaturesId")
            //            .HasConstraintName("fk_variety_variety_feature_variety_features_features_id"),
            //        j =>
            //        {
            //            j.HasKey("FeaturesId", "VarietiesId").HasName("pk_variety_variety_feature");
            //            j.ToTable("variety_variety_feature", "dictionaries");
            //            j.HasIndex(new[] { "VarietiesId" }, "ix_variety_variety_feature_varieties_id");
            //            j.IndexerProperty<Guid>("FeaturesId").HasColumnName("features_id");
            //            j.IndexerProperty<Guid>("VarietiesId").HasColumnName("varieties_id");
            //        });
        });

        modelBuilder.Entity<VegetationIndex>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_vegetation_indexes");

            entity.ToTable("vegetation_indexes", "dictionaries", tb => tb.HasComment("Справочник вегетационных индексов"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.DescriptionEn)
                .HasComment("Описание на английском")
                .HasColumnName("description_en");
            entity.Property(e => e.DescriptionRu)
                .HasComment("Описание на русском")
                .HasColumnName("description_ru");
            entity.Property(e => e.DescriptionUz)
                .HasComment("Описание на узбекском")
                .HasColumnName("description_uz");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.Name)
                .HasComment("Название индекса")
                .HasColumnName("name");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<VegetationPeriod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_vegetation_periods");

            entity.ToTable("vegetation_periods", "dictionaries", tb => tb.HasComment("Справочник периодов вегетации"));

            entity.HasIndex(e => e.RegionId, "ix_vegetation_periods_region_id");

            entity.HasIndex(e => e.VarietyId, "ix_vegetation_periods_variety_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.ClassId)
                .HasComment("Идентификатор класса сорта")
                .HasColumnName("class_id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.DayEnd)
                .HasComment("День окончания периода")
                .HasColumnName("day_end");
            entity.Property(e => e.DayStart)
                .HasComment("День начала периода")
                .HasColumnName("day_start");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.RegionId)
                .HasComment("Идентификатор региона")
                .HasColumnName("region_id");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.VarietyId)
                .HasComment("Идентификатор сорта")
                .HasColumnName("variety_id");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.Region).WithMany(p => p.VegetationPeriods)
            //    .HasForeignKey(d => d.RegionId)
            //    .HasConstraintName("fk_vegetation_periods_regions_region_id");

            //entity.HasOne(d => d.Variety).WithMany(p => p.VegetationPeriods)
            //    .HasForeignKey(d => d.VarietyId)
            //    .HasConstraintName("fk_vegetation_periods_varieties_variety_id");
        });

        modelBuilder.Entity<WaterResource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_water_resources");

            entity.ToTable("water_resources", "dictionaries", tb => tb.HasComment("Справочник водных ресурсов"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        modelBuilder.Entity<WeatherStation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_weather_stations");

            entity.ToTable("weather_stations", "dictionaries", tb => tb.HasComment("Справочник погодных станций"));

            entity.HasIndex(e => e.FirmId, "ix_weather_stations_firm_id");

            entity.HasIndex(e => e.RegionId, "ix_weather_stations_region_id");

            entity.HasIndex(e => e.TypeId, "ix_weather_stations_type_id");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.ClassId)
                .HasComment("Идентификатор класса")
                .HasColumnName("class_id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.FirmId)
                .HasComment("Идентификатор компании-владельца станции")
                .HasColumnName("firm_id");
            entity.Property(e => e.IsAuto)
                .HasComment("Признак автоматизированной передачи данных")
                .HasColumnName("is_auto");
            entity.Property(e => e.Lat)
                .HasComment("Широта")
                .HasColumnName("lat");
            entity.Property(e => e.Lng)
                .HasComment("Долгота")
                .HasColumnName("lng");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.Radius)
                .HasComment("Радиус покрытия")
                .HasColumnName("radius");
            entity.Property(e => e.RegionId)
                .HasComment("Идентификатор региона")
                .HasColumnName("region_id");
            entity.Property(e => e.TypeId)
                .HasComment("Тип станции (станция/пост)")
                .HasColumnName("type_id");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");

            //entity.HasOne(d => d.Firm).WithMany(p => p.WeatherStations)
            //    .HasForeignKey(d => d.FirmId)
            //    .HasConstraintName("fk_weather_stations_firms_firm_id");

            //entity.HasOne(d => d.Region).WithMany(p => p.WeatherStations)
            //    .HasForeignKey(d => d.RegionId)
            //    .HasConstraintName("fk_weather_stations_regions_region_id");

            //entity.HasOne(d => d.Type).WithMany(p => p.WeatherStations)
            //    .HasForeignKey(d => d.TypeId)
            //    .HasConstraintName("fk_weather_stations_weather_station_types_type_id");
        });

        modelBuilder.Entity<WeatherStationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_weather_station_types");

            entity.ToTable("weather_station_types", "dictionaries", tb => tb.HasComment("Справочник типов погодных станций"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий")
                .HasColumnName("comment");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("create_date");
            entity.Property(e => e.CreateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор")
                .HasColumnName("create_user");
            entity.Property(e => e.ExtData)
                .HasComment("Дополнительная информация")
                .HasColumnName("ext_data");
            entity.Property(e => e.NameEn)
                .HasComment("Наименование на английском")
                .HasColumnName("name_en");
            entity.Property(e => e.NameRu)
                .HasComment("Наименование на русском")
                .HasColumnName("name_ru");
            entity.Property(e => e.NameUz)
                .HasComment("Наименование на узбекском")
                .HasColumnName("name_uz");
            entity.Property(e => e.OrderCode)
                .HasComment("Код сортировки")
                .HasColumnName("order_code");
            entity.Property(e => e.UpdateDate)
                .HasDefaultValueSql("now()")
                .HasComment("Дата обновления")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUser)
                .HasDefaultValueSql("'system'::text")
                .HasComment("Автор обновления")
                .HasColumnName("update_user");
            entity.Property(e => e.Visible)
                .HasComment("Публикация")
                .HasColumnName("visible");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
