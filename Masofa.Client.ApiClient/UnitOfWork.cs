using System.Buffers.Text;
using Masofa.Client.ApiClient.Repositories.Identity;
using Masofa.Client.ApiClient.Repositrories.Common;
using Masofa.Client.ApiClient.Repositrories.CropMonitoring;
using Masofa.Client.ApiClient.Repositrories.Identity;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Identity;
using Masofa.Common.ViewModels.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Masofa.Client.ApiClient
{
    public class UnitOfWork
    {
        public HttpClient HttpClient { get; private set; }
        public bool IsAuth
        {
            get
            {
                return !string.IsNullOrEmpty(_token);
            }
        }

        private string _token;
        private string _baseUrl;


        #region common
        public BusinessLogicLoggerRepository BusinessLogicLoggerRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.SystemCrical.FileStorageItem> FileStorageItemRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Satellite.SatelliteProduct> SatelliteProductRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.SystemCrical.SystemBackgroundTask> SystemBackgroundTaskRepostitory { get; private set; }
        #endregion

        #region dictionaries
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.AdministrativeUnit> AdministrativeUnitRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.AgroclimaticZone> AgroclimaticZoneRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.AgroMachineType> AgroMachineTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.AgroOperation> AgroOperationRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.AgrotechnicalMeasure> AgrotechnicalMeasureRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.AgroTerm> AgroTermRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.BidContent> BidContentRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.BidState> BidStateRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.BidType> BidTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.BusinessType> BusinessTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.ClimaticStandard> ClimaticStandardRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.Crop> CropRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.CropPeriod> CropPeriodRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.CropPeriodVegetationIndex> CropPeriodVegetationIndexRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.Disease> DiseaseRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.EntomophageType> EntomophageTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.ExperimentalFarmingMethod> ExperimentalFarmingMethodRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.Fertilizer> FertilizerRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.FertilizerType> FertilizerTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.FieldUsageStatus> FieldUsageStatusRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.Firm> FirmRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.FlightTarget> FlightTargetRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.IrrigationMethod> IrrigationMethodRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.IrrigationSource> IrrigationSourceRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.MeasurementUnit> MeasurementUnitRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.MeliorativeMeasureType> MeliorativeMeasureTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.Person> PersonRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.Pesticide> PesticideRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.PesticideType> PesticideTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.PestType> PestTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.ProductQualityStandard> ProductQualityStandardRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.ProviderWeatherCondition> ProviderWeatherConditionRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.Region> RegionRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.RegionMap> RegionMapRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.RegionType> RegionTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.SoilType> SoilTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.SolarRadiationInfluence> SolarRadiationInfluenceRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.SystemDataSource> SystemDataSourceRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.TaskStatus> TaskStatusRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.UavCameraType> UavCameraTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.UavDataType> UavDataTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.Variety> VarietyRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.VarietyFeature> VarietyFeatureRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.VegetationIndex> VegetationIndexRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.VegetationPeriod> VegetationPeriodRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.WaterResource> WaterResourceRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherAlertType> WeatherAlertTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherCondition> WeatherConditionRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherFrequency> WeatherFrequencyRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherImageType> WeatherImageTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherJobStatus> WeatherJobStatusRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherProvider> WeatherProviderRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherReportType> WeatherReportTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherStation> WeatherStationRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherStationType> WeatherStationTypeRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherType> WeatherTypeRepository { get; private set; }
        #endregion

        #region crop-monitoring
        public BidRepository BidRepository { get; private set; }
        public BidTemplateRepository BidTemplateRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.CropMonitoring.Field> FieldRepository { get; private set; }
        public BaseCrudRepository<Masofa.Common.Models.CropMonitoring.Season> SeasonRepository { get; private set; }
        public FieldAgroProducerHistoryRepository FieldAgroProducerHistoryRepository { get; private set; }
        #endregion

        #region identity
        public AccountRepository AccountRepository { get; private set; }
        public LockPermissionRepository LockPermissionRepository { get; private set; }
        public RoleRepository RoleRepository { get; private set; }
        public UserRepository UserRepository { get; private set; }
        public UserDeviceRepository UserDeviceRepository { get; private set; }
        #endregion

        #region landsat
        #endregion

        #region sentinel
        #endregion

        #region tiles
        #endregion

        #region weather
        #endregion

        public UnitOfWork(HttpClient httpClient, string baseUrl)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            Init();
        }

        public async Task LoginAsync(LoginAndPasswordViewModel viewModel, CancellationToken cancellationToken)
        {
            try
            {
                // Проверим, что AccountRepository инициализирован
                if (AccountRepository == null)
                {
                    throw new InvalidOperationException("AccountRepository is not initialized.");
                }

                var result = await AccountRepository.LoginAsync(viewModel, cancellationToken);
                _token = result;
                SetToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex}");
                throw;
            }
        }

        private void Init()
        {
            // common
            //BusinessLogicLoggerRepository = new BusinessLogicLoggerRepository(HttpClient, $"{_baseUrl}/business-logic-logger");
            FileStorageItemRepository = new BaseCrudRepository<Masofa.Common.Models.SystemCrical.FileStorageItem>(HttpClient, $"{_baseUrl}/file-storage-item");
            SatelliteProductRepository = new BaseCrudRepository<Masofa.Common.Models.Satellite.SatelliteProduct>(HttpClient, $"{_baseUrl}/satellite-product");
            SystemBackgroundTaskRepostitory = new BaseCrudRepository<Masofa.Common.Models.SystemCrical.SystemBackgroundTask>(HttpClient, $"{_baseUrl}/system-background-task");

            // dictionaries
            AdministrativeUnitRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.AdministrativeUnit>(HttpClient, $"{_baseUrl}/administrative-unit");
            AgroclimaticZoneRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.AgroclimaticZone>(HttpClient, $"{_baseUrl}/agroclimatic-zone");
            AgroMachineTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.AgroMachineType>(HttpClient, $"{_baseUrl}/agro-machine-type");
            AgroOperationRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.AgroOperation>(HttpClient, $"{_baseUrl}/agro-operation");
            AgrotechnicalMeasureRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.AgrotechnicalMeasure>(HttpClient, $"{_baseUrl}/agrotechnical-measure");
            AgroTermRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.AgroTerm>(HttpClient, $"{_baseUrl}/agro-term");
            BidContentRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.BidContent>(HttpClient, $"{_baseUrl}/bid-content");
            BidStateRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.BidState>(HttpClient, $"{_baseUrl}/bid-state");
            BidTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.BidType>(HttpClient, $"{_baseUrl}/bid-type");
            BusinessTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.BusinessType>(HttpClient, $"{_baseUrl}/business-type");
            ClimaticStandardRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.ClimaticStandard>(HttpClient, $"{_baseUrl}/climatic-standard");
            CropRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.Crop>(HttpClient, $"{_baseUrl}/crop");
            CropPeriodRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.CropPeriod>(HttpClient, $"{_baseUrl}/crop-period");
            CropPeriodVegetationIndexRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.CropPeriodVegetationIndex>(HttpClient, $"{_baseUrl}/crop-period-vegetation-index");
            DiseaseRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.Disease>(HttpClient, $"{_baseUrl}/disease");
            EntomophageTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.EntomophageType>(HttpClient, $"{_baseUrl}/entomophage-type");
            ExperimentalFarmingMethodRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.ExperimentalFarmingMethod>(HttpClient, $"{_baseUrl}/experimental-farming-method");
            FertilizerRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.Fertilizer>(HttpClient, $"{_baseUrl}/fertilizer");
            FertilizerTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.FertilizerType>(HttpClient, $"{_baseUrl}/fertilizer-type");
            FieldUsageStatusRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.FieldUsageStatus>(HttpClient, $"{_baseUrl}/field-usage-status");
            FirmRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.Firm>(HttpClient, $"{_baseUrl}/firm");
            FlightTargetRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.FlightTarget>(HttpClient, $"{_baseUrl}/flight-target");
            IrrigationMethodRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.IrrigationMethod>(HttpClient, $"{_baseUrl}/irrigation-method");
            IrrigationSourceRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.IrrigationSource>(HttpClient, $"{_baseUrl}/irrigation-source");
            MeasurementUnitRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.MeasurementUnit>(HttpClient, $"{_baseUrl}/measurement-unit");
            MeliorativeMeasureTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.MeliorativeMeasureType>(HttpClient, $"{_baseUrl}/meliorative-measure-type");
            PersonRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.Person>(HttpClient, $"{_baseUrl}/person");
            PesticideRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.Pesticide>(HttpClient, $"{_baseUrl}/pesticide");
            PesticideTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.PesticideType>(HttpClient, $"{_baseUrl}/pesticide-type");
            PestTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.PestType>(HttpClient, $"{_baseUrl}/pest-type");
            ProductQualityStandardRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.ProductQualityStandard>(HttpClient, $"{_baseUrl}/product-quality-standard");
            ProviderWeatherConditionRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.ProviderWeatherCondition>(HttpClient, $"{_baseUrl}/provider-weather-condition");
            RegionRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.Region>(HttpClient, $"{_baseUrl}/region");
            RegionMapRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.RegionMap>(HttpClient, $"{_baseUrl}/region-map");
            RegionTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.RegionType>(HttpClient, $"{_baseUrl}/region-type");
            SoilTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.SoilType>(HttpClient, $"{_baseUrl}/soil-type");
            SolarRadiationInfluenceRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.SolarRadiationInfluence>(HttpClient, $"{_baseUrl}/solar-radiation-influence");
            SystemDataSourceRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.SystemDataSource>(HttpClient, $"{_baseUrl}/system-data-source");
            TaskStatusRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.TaskStatus>(HttpClient, $"{_baseUrl}/task-status");
            UavCameraTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.UavCameraType>(HttpClient, $"{_baseUrl}/uav-camera-type");
            UavDataTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.UavDataType>(HttpClient, $"{_baseUrl}/uav-data-type");
            VarietyRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.Variety>(HttpClient, $"{_baseUrl}/variety");
            VarietyFeatureRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.VarietyFeature>(HttpClient, $"{_baseUrl}/variety-feature");
            VegetationIndexRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.VegetationIndex>(HttpClient, $"{_baseUrl}/vegetation-index");
            VegetationPeriodRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.VegetationPeriod>(HttpClient, $"{_baseUrl}/vegetation-period");
            WaterResourceRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.WaterResource>(HttpClient, $"{_baseUrl}/water-resource");
            WeatherAlertTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherAlertType>(HttpClient, $"{_baseUrl}/weather-alert-type");
            WeatherConditionRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherCondition>(HttpClient, $"{_baseUrl}/weather-condition");
            WeatherFrequencyRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherFrequency>(HttpClient, $"{_baseUrl}/weather-frequency");
            WeatherImageTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherImageType>(HttpClient, $"{_baseUrl}/weather-image-type");
            WeatherJobStatusRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherJobStatus>(HttpClient, $"{_baseUrl}/weather-job-status");
            WeatherProviderRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherProvider>(HttpClient, $"{_baseUrl}/weather-provider");
            WeatherReportTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherReportType>(HttpClient, $"{_baseUrl}/weather-report-type");
            WeatherStationRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherStation>(HttpClient, $"{_baseUrl}/weather-station");
            WeatherStationTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherStationType>(HttpClient, $"{_baseUrl}/weather-station-type");
            WeatherTypeRepository = new BaseCrudRepository<Masofa.Common.Models.Dictionaries.WeatherType>(HttpClient, $"{_baseUrl}/weather-type");

            // crop-monitoring
            BidRepository = new BidRepository(HttpClient, $"{_baseUrl}/cropMonitoring/Bid");
            BidTemplateRepository = new BidTemplateRepository(HttpClient, $"{_baseUrl}/cropMonitoring/BidTemplates");
            FieldRepository = new BaseCrudRepository<Masofa.Common.Models.CropMonitoring.Field>(HttpClient, $"{_baseUrl}/cropMonitoring/field");
            SeasonRepository = new BaseCrudRepository<Masofa.Common.Models.CropMonitoring.Season>(HttpClient, $"{_baseUrl}/cropMonitoring/season");
            FieldAgroProducerHistoryRepository = new FieldAgroProducerHistoryRepository(HttpClient, $"{_baseUrl}/cropMonitoring/FieldAgroProducerHistory");

            // identity
            UserDeviceRepository = new UserDeviceRepository(HttpClient, $"{_baseUrl}/identity/UserDevice");
            AccountRepository = new AccountRepository(HttpClient, $"{_baseUrl}/identity/Account");
            LockPermissionRepository = new LockPermissionRepository(HttpClient, $"{_baseUrl}/identity/LockPermission");
            RoleRepository = new RoleRepository(HttpClient, $"{_baseUrl}/identity/Role");
            UserRepository = new UserRepository(HttpClient, $"{_baseUrl}/identity/User");

            // landsat
            // sentinel
            // tiles
            // weather
        }

        private void SetToken()
        {
            // common
            //BusinessLogicLoggerRepository.Token = _token;
            FileStorageItemRepository.Token = _token;
            SatelliteProductRepository.Token = _token;
            SystemBackgroundTaskRepostitory.Token = _token;

            // dictionaries
            AdministrativeUnitRepository.Token = _token;
            AgroclimaticZoneRepository.Token = _token;
            AgroMachineTypeRepository.Token = _token;
            AgroOperationRepository.Token = _token;
            AgrotechnicalMeasureRepository.Token = _token;
            AgroTermRepository.Token = _token;
            BidContentRepository.Token = _token;
            BidStateRepository.Token = _token;
            BidTypeRepository.Token = _token;
            BusinessTypeRepository.Token = _token;
            ClimaticStandardRepository.Token = _token;
            CropRepository.Token = _token;
            CropPeriodRepository.Token = _token;
            CropPeriodVegetationIndexRepository.Token = _token;
            DiseaseRepository.Token = _token;
            EntomophageTypeRepository.Token = _token;
            ExperimentalFarmingMethodRepository.Token = _token;
            FertilizerRepository.Token = _token;
            FertilizerTypeRepository.Token = _token;
            FieldUsageStatusRepository.Token = _token;
            FirmRepository.Token = _token;
            FlightTargetRepository.Token = _token;
            IrrigationMethodRepository.Token = _token;
            IrrigationSourceRepository.Token = _token;
            MeasurementUnitRepository.Token = _token;
            MeliorativeMeasureTypeRepository.Token = _token;
            PersonRepository.Token = _token;
            PesticideRepository.Token = _token;
            PesticideTypeRepository.Token = _token;
            PestTypeRepository.Token = _token;
            ProductQualityStandardRepository.Token = _token;
            ProviderWeatherConditionRepository.Token = _token;
            RegionRepository.Token = _token;
            RegionMapRepository.Token = _token;
            RegionTypeRepository.Token = _token;
            SoilTypeRepository.Token = _token;
            SolarRadiationInfluenceRepository.Token = _token;
            SystemDataSourceRepository.Token = _token;
            TaskStatusRepository.Token = _token;
            UavCameraTypeRepository.Token = _token;
            UavDataTypeRepository.Token = _token;
            VarietyRepository.Token = _token;
            VarietyFeatureRepository.Token = _token;
            VegetationIndexRepository.Token = _token;
            VegetationPeriodRepository.Token = _token;
            WaterResourceRepository.Token = _token;
            WeatherAlertTypeRepository.Token = _token;
            WeatherConditionRepository.Token = _token;
            WeatherFrequencyRepository.Token = _token;
            WeatherImageTypeRepository.Token = _token;
            WeatherJobStatusRepository.Token = _token;
            WeatherProviderRepository.Token = _token;
            WeatherReportTypeRepository.Token = _token;
            WeatherStationRepository.Token = _token;
            WeatherStationTypeRepository.Token = _token;
            WeatherTypeRepository.Token = _token;

            // crop-monitoring
            BidRepository.Token = _token;
            BidTemplateRepository.Token = _token;
            FieldRepository.Token = _token;
            SeasonRepository.Token = _token;
            FieldAgroProducerHistoryRepository.Token = _token;

            // identity
            UserDeviceRepository.Token = _token;
            AccountRepository.Token = _token;
            LockPermissionRepository.Token = _token;
            RoleRepository.Token = _token;
            UserRepository.Token = _token;

            // landsat
            // sentinel
            // tiles
            // weather
        }
    }
}
