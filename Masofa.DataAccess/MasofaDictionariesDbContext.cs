using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Models.SystemDocumentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Masofa.DataAccess
{
    public partial class MasofaDictionariesDbContext : DbContext
    {
        public MasofaDictionariesDbContext(DbContextOptions<MasofaDictionariesDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<AdministrativeUnit> AdministrativeUnits { get; set; }
        public DbSet<AgroclimaticZone> AgroclimaticZones { get; set; }
        public DbSet<AgroMachineType> AgroMachineTypes { get; set; }
        public DbSet<AgroOperation> AgroOperations { get; set; }
        public DbSet<AgrotechnicalMeasure> AgrotechnicalMeasures { get; set; }
        public DbSet<AgroTerm> AgroTerms { get; set; }
        public DbSet<BidContent> BidContents { get; set; }
        public DbSet<BidState> BidStates { get; set; }
        public DbSet<BidType> BidTypes { get; set; }
        public DbSet<BusinessType> BusinessTypes { get; set; }
        public DbSet<BusinessTypeFirm> BusinessTypesFirm { get; set; }
        public DbSet<BusinessTypePerson> BusinessTypesPerson { get; set; }
        public DbSet<ClimaticStandard> ClimaticStandards { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<CropPeriod> CropPeriods { get; set; }
        public DbSet<CropPeriodVegetationIndex> CropPeriodVegetationIndexes { get; set; }
        public DbSet<DicitonaryType> DicitonaryTypes { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<EntomophageType> EntomophageTypes { get; set; }
        public DbSet<ExperimentalFarmingMethod> ExperimentalFarmingMethods { get; set; }
        public DbSet<Fertilizer> Fertilizers { get; set; }
        public DbSet<FertilizerType> FertilizerTypes { get; set; }
        public DbSet<FieldUsageStatus> FieldUsageStatuses { get; set; }
        public DbSet<Firm> Firms { get; set; }
        public DbSet<FlightTarget> FlightTargets { get; set; }
        public DbSet<IrrigationMethod> IrrigationMethods { get; set; }
        public DbSet<IrrigationSource> IrrigationSources { get; set; }
        public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
        public DbSet<MeliorativeMeasureType> MeliorativeMeasureTypes { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Pesticide> Pesticides { get; set; }
        public DbSet<PesticideType> PesticideTypes { get; set; }
        public DbSet<PestType> PestTypes { get; set; }
        public DbSet<ProductQualityStandard> ProductQualityStandards { get; set; }
        public DbSet<ProviderWeatherCondition> ProviderWeatherConditions { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<RegionMap> RegionMaps { get; set; }
        public DbSet<RegionType> RegionTypes { get; set; }
        public DbSet<RegulatoryDocumentation> RegulatoryDocumentations { get; set; }
        public DbSet<SoilType> SoilTypes { get; set; }
        public DbSet<SolarRadiationInfluence> SolarRadiationInfluences { get; set; }
        public DbSet<SystemDataSource> SystemDataSources { get; set; }
        public DbSet<Common.Models.Dictionaries.TaskStatus> TaskStatuses { get; set; }
        public DbSet<UavCameraType> UavCameraTypes { get; set; }
        public DbSet<UavDataType> UavDataTypes { get; set; }
        public DbSet<Variety> Varieties { get; set; }
        public DbSet<VarietyFeature> VarietyFeatures { get; set; }
        public DbSet<VegetationIndex> VegetationIndexes { get; set; }
        public DbSet<VegetationPeriod> VegetationPeriods { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DbSet<WaterResource> WaterResources { get; set; }
        public DbSet<WeatherStation> WeatherStations { get; set; }
        public DbSet<WeatherStationType> WeatherStationTypes { get; set; }
        public DbSet<WeatherAlertType> WeatherAlertTypes { get; set; }
        public DbSet<WeatherCondition> WeatherConditions { get; set; }
        public DbSet<WeatherFrequency> WeatherFrequencies { get; set; }
        public DbSet<WeatherImageType> WeatherImageTypes { get; set; }
        public DbSet<WeatherJobStatus> WeatherJobStatuses { get; set; }
        public DbSet<WeatherProvider> WeatherProviders { get; set; }
        public DbSet<WeatherReportType> WeatherReportTypes { get; set; }
        public DbSet<WeatherType> WeatherTypes { get; set; }


        public DbSet<Insurance> Insurances { get; set; }

        public DbSet<SystemDocumentationBlock> SystemDocumentationBlocks { get; set; }

        public DbSet<InsuranceCase> InsuranceCases { get; set; }
        public DbSet<FinancialCondition> FinancialConditions { get; set; }
        public DbSet<Encumbrance> Encumbrances { get; set; }
        public DbSet<SoilProfileAnalysis> SoilProfileAnalyses { get; set; }
        public DbSet<AnalyticalIndicator> AnalyticalIndicators { get; set; }
        public DbSet<RecommendedPlantingDates> RecommendedPlantingDates { get; set; }
        public DbSet<CropTypeClassifier> CropTypeClassifiers { get; set; }
        public DbSet<AgriculturalProfession> AgriculturalProfessions { get; set; }
        public DbSet<SoilIrrigationMode> SoilIrrigationModes { get; set; }
        public DbSet<PlantProtectionTechnology> PlantProtectionTechnologies { get; set; }
        public DbSet<OrganicFarmingTechnology> OrganicFarmingTechnologies { get; set; }
        public DbSet<PrecisionFarmingCrop> PrecisionFarmingCrops { get; set; }
        public DbSet<BioProtectionTechnology> BioProtectionTechnologies { get; set; }
        public DbSet<HarvestProcessingTechnology> HarvestProcessingTechnologies { get; set; }
        public DbSet<GmoCropRegistry> GmoCropRegistries { get; set; }
        public DbSet<SustainableFarmingTechnology> SustainableFarmingTechnologies { get; set; }
        public DbSet<FieldWorkTechnology> FieldWorkTechnologies { get; set; }
        public DbSet<SolarActivityData> SolarActivityDatas { get; set; }
        public DbSet<RegionalSoilMoisture> RegionalSoilMoistures { get; set; }
        public DbSet<SnowLoadData> SnowLoadDatas { get; set; }
        public DbSet<ClimateAnomaly> ClimateAnomalies { get; set; }
        public DbSet<DailyTemperatureData> DailyTemperatureDatas { get; set; }
        public DbSet<LandResourceFertility> LandResourceFertilities { get; set; }
        public DbSet<RenewableEnergySource> RenewableEnergySources { get; set; }
        public DbSet<NatureReserve> NatureReserves { get; set; }
        public DbSet<MineralResource> MineralResources { get; set; }
        public DbSet<GroundwaterSource> GroundwaterSources { get; set; }
        public DbSet<BiofuelResource> BiofuelResources { get; set; }
        public DbSet<WaterResourceQuality> WaterResourceQualities { get; set; }
        public DbSet<SoilChemicalComposition> SoilChemicalCompositions { get; set; }
        public DbSet<ProductQualityStandardNew> ProductQualityStandardNews { get; set; }
        public DbSet<AgroclimaticZoneRegion> AgroclimaticZoneRegionRelations { get; set; }
        public DbSet<FieldType> FieldTypes { get; set; }
        public DbSet<RootSystemType> RootSystemTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			this.ApplyLocalizationStringSettings(modelBuilder);
		}


	}
}