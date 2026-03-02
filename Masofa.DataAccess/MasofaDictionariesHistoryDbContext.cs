using Masofa.Common.Models.Dictionaries;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public partial class MasofaDictionariesHistoryDbContext : DbContext
    {
        public MasofaDictionariesHistoryDbContext(DbContextOptions<MasofaDictionariesHistoryDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<AdministrativeUnitHistory> AdministrativeUnitHistories { get; set; }
        public DbSet<AgroclimaticZoneHistory> AgroclimaticZoneHistories { get; set; }
        public DbSet<AgroMachineTypeHistory> AgroMachineTypeHistories { get; set; }
        public DbSet<AgroOperationHistory> AgroOperationHistories { get; set; }
        public DbSet<AgrotechnicalMeasureHistory> AgrotechnicalMeasureHistories { get; set; }
        public DbSet<AgroTermHistory> AgroTermHistories { get; set; }
        public DbSet<BidContentHistory> BidContentHistories { get; set; }
        public DbSet<BidStateHistory> BidStateHistories { get; set; }
        public DbSet<BidTypeHistory> BidTypeHistories { get; set; }
        public DbSet<BusinessTypeFirmHistory> BusinessTypeFirmHistories { get; set; }
        public DbSet<BusinessTypeHistory> BusinessTypeHistories { get; set; }
        public DbSet<BusinessTypePersonHistory> BusinessTypePersonHistories { get; set; }
        public DbSet<ClimaticStandardHistory> ClimaticStandardHistories { get; set; }
        public DbSet<CropHistory> CropHistories { get; set; }
        public DbSet<CropPeriodHistory> CropPeriodHistories { get; set; }
        public DbSet<CropPeriodVegetationIndexHistory> CropPeriodVegetationIndexHistories { get; set; }
        public DbSet<DicitonaryTypeHistory> DicitonaryTypeHistories { get; set; }
        public DbSet<DiseaseHistory> DiseaseHistories { get; set; }
        public DbSet<EntomophageTypeHistory> EntomophageTypeHistories { get; set; }
        public DbSet<ExperimentalFarmingMethodHistory> ExperimentalFarmingMethodHistories { get; set; }
        public DbSet<FertilizerHistory> FertilizerHistories { get; set; }
        public DbSet<FertilizerTypeHistory> FertilizerTypeHistories { get; set; }
        public DbSet<FieldUsageStatusHistory> FieldUsageStatusHistories { get; set; }
        public DbSet<FirmHistory> FirmHistories { get; set; }
        public DbSet<FlightTargetHistory> FlightTargetHistories { get; set; }
        public DbSet<InsuranceHistory> InsuranceHistories { get; set; }
        public DbSet<IrrigationMethodHistory> IrrigationMethodHistories { get; set; }
        public DbSet<IrrigationSourceHistory> IrrigationSourceHistories { get; set; }
        public DbSet<MeasurementUnitHistory> MeasurementUnitHistories { get; set; }
        public DbSet<MeliorativeMeasureTypeHistory> MeliorativeMeasureTypeHistories { get; set; }
        public DbSet<PersonHistory> PersonHistories { get; set; }
        public DbSet<PesticideHistory> PesticideHistories { get; set; }
        public DbSet<PesticideTypeHistory> PesticideTypeHistories { get; set; }
        public DbSet<PestTypeHistory> PestTypeHistories { get; set; }
        public DbSet<ProductQualityStandardHistory> ProductQualityStandardHistories { get; set; }
        public DbSet<ProviderWeatherConditionHistory> ProviderWeatherConditionHistories { get; set; }
        public DbSet<RegionHistory> RegionHistories { get; set; }
        public DbSet<RegionMapHistory> RegionMapHistories { get; set; }
        public DbSet<RegionTypeHistory> RegionTypeHistories { get; set; }
        public DbSet<RegulatoryDocumentationHistory> RegulatoryDocumentationHistories { get; set; }
        public DbSet<SoilTypeHistory> SoilTypeHistories { get; set; }
        public DbSet<SolarRadiationInfluenceHistory> SolarRadiationInfluenceHistories { get; set; }
        public DbSet<SystemDataSourceHistory> SystemDataSourceHistories { get; set; }
        public DbSet<TaskStatusHistory> TaskStatusHistories { get; set; }
        public DbSet<UavCameraTypeHistory> UavCameraTypeHistories { get; set; }
        public DbSet<UavDataTypeHistory> UavDataTypeHistories { get; set; }
        public DbSet<VarietyHistory> VarietyHistories { get; set; }
        public DbSet<VarietyFeatureHistory> VarietyFeatureHistories { get; set; }
        public DbSet<VegetationIndexHistory> VegetationIndexHistories { get; set; }
        public DbSet<VegetationPeriodHistory> VegetationPeriodHistories { get; set; }
        public DbSet<WaterResourceHistory> WaterResourceHistories { get; set; }
        public DbSet<WeatherAlertTypeHistory> WeatherAlertTypeHistories { get; set; }
        public DbSet<WeatherConditionHistory> WeatherConditionHistories { get; set; }
        public DbSet<WeatherFrequencyHistory> WeatherFrequencyHistories { get; set; }
        public DbSet<WeatherImageTypeHistory> WeatherImageTypeHistories { get; set; }
        public DbSet<WeatherJobStatusHistory> WeatherJobStatusHistories { get; set; }
        public DbSet<WeatherProviderHistory> WeatherProviderHistories { get; set; }
        public DbSet<WeatherReportTypeHistory> WeatherReportTypeHistories { get; set; }
        public DbSet<WeatherStationHistory> WeatherStationHistories { get; set; }
        public DbSet<WeatherStationTypeHistory> WeatherStationTypeHistories { get; set; }
        public DbSet<WeatherTypeHistory> WeatherTypeHistories { get; set; }

        public DbSet<InsuranceCaseHistory> InsuranceCaseHistories { get; set; }
        public DbSet<FinancialConditionHistory> FinancialConditionHistories { get; set; }
        public DbSet<EncumbranceHistory> EncumbranceHistories { get; set; }
        public DbSet<SoilProfileAnalysisHistory> SoilProfileAnalysisHistories { get; set; }
        public DbSet<AnalyticalIndicatorHistory> AnalyticalIndicatorHistories { get; set; }
        public DbSet<RecommendedPlantingDatesHistory> RecommendedPlantingDatesHistories { get; set; }
        public DbSet<CropTypeClassifierHistory> CropTypeClassifierHistories { get; set; }
        public DbSet<AgriculturalProfessionHistory> AgriculturalProfessionHistories { get; set; }
        public DbSet<SoilIrrigationModeHistory> SoilIrrigationModeHistories { get; set; }
        public DbSet<PlantProtectionTechnologyHistory> PlantProtectionTechnologyHistories { get; set; }
        public DbSet<OrganicFarmingTechnologyHistory> OrganicFarmingTechnologyHistories { get; set; }
        public DbSet<PrecisionFarmingCropHistory> PrecisionFarmingCropHistories { get; set; }
        public DbSet<BioProtectionTechnologyHistory> BioProtectionTechnologyHistories { get; set; }
        public DbSet<HarvestProcessingTechnologyHistory> HarvestProcessingTechnologyHistories { get; set; }
        public DbSet<GmoCropRegistryHistory> GmoCropRegistryHistories { get; set; }
        public DbSet<SustainableFarmingTechnologyHistory> SustainableFarmingTechnologyHistories { get; set; }
        public DbSet<FieldWorkTechnologyHistory> FieldWorkTechnologyHistories { get; set; }
        public DbSet<SolarActivityDataHistory> SolarActivityDataHistories { get; set; }
        public DbSet<RegionalSoilMoistureHistory> RegionalSoilMoistureHistories { get; set; }
        public DbSet<SnowLoadDataHistory> SnowLoadDataHistories { get; set; }
        public DbSet<ClimateAnomalyHistory> ClimateAnomalyHistories { get; set; }
        public DbSet<DailyTemperatureDataHistory> DailyTemperatureDataHistories { get; set; }
        public DbSet<LandResourceFertilityHistory> LandResourceFertilityHistories { get; set; }
        public DbSet<RenewableEnergySourceHistory> RenewableEnergySourceHistories { get; set; }
        public DbSet<NatureReserveHistory> NatureReserveHistories { get; set; }
        public DbSet<MineralResourceHistory> MineralResourceHistories { get; set; }
        public DbSet<GroundwaterSourceHistory> GroundwaterSourceHistories { get; set; }
        public DbSet<BiofuelResourceHistory> BiofuelResourceHistories { get; set; }
        public DbSet<WaterResourceQualityHistory> WaterResourceQualityHistories { get; set; }
        public DbSet<SoilChemicalCompositionHistory> SoilChemicalCompositionHistories { get; set; }
        public DbSet<ProductQualityStandardNewHistory> ProductQualityStandardNewHistories { get; set; }
        public DbSet<FieldTypeHistory> FieldTypeHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.ApplyLocalizationStringSettings(modelBuilder);
        }
    }
}