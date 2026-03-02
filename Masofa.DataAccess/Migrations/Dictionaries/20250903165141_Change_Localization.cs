using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Dictionaries
{
    /// <inheritdoc />
    public partial class Change_Localization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WeatherTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "WeatherTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WeatherStationTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "WeatherStationTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WeatherStations");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "WeatherStations");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WeatherReportTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "WeatherReportTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WeatherProviders");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "WeatherProviders");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WeatherJobStatuses");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "WeatherJobStatuses");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WeatherImageTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "WeatherImageTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WeatherFrequencies");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "WeatherFrequencies");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WeatherConditions");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "WeatherConditions");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "WeatherAlertTypes");

            migrationBuilder.DropColumn(
                name: "DescriptionRu",
                table: "WeatherAlertTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WeatherAlertTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "WeatherAlertTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "WaterResources");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "WaterResources");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "VegetationPeriods");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "VegetationPeriods");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "VegetationIndexes");

            migrationBuilder.DropColumn(
                name: "DescriptionRu",
                table: "VegetationIndexes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "VarietyFeatures");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "VarietyFeatures");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Varieties");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "Varieties");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "UavDataTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "UavDataTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "UavCameraTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "UavCameraTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "TaskStatuses");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "TaskStatuses");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "SystemDataSources");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "SystemDataSources");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "SoilTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "SoilTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "RegionTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "RegionTypes");

            migrationBuilder.DropColumn(
                name: "NameAdminEn",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "NameAdminRu",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "ShortNameEn",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "ShortNameRu",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "ProviderWeatherConditions");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "ProviderWeatherConditions");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "ProductQualityStandards");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "ProductQualityStandards");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "PestTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "PestTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "PesticideTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "PesticideTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Pesticides");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "Pesticides");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "MeliorativeMeasureTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "MeliorativeMeasureTypes");

            migrationBuilder.DropColumn(
                name: "FullNameEn",
                table: "MeasurementUnits");

            migrationBuilder.DropColumn(
                name: "FullNameRu",
                table: "MeasurementUnits");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "MeasurementUnits");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "MeasurementUnits");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "IrrigationSources");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "IrrigationSources");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "IrrigationMethods");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "IrrigationMethods");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "FlightTargets");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "FlightTargets");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Firms");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "Firms");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "FieldUsageStatuses");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "FieldUsageStatuses");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "FertilizerTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "FertilizerTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Fertilizers");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "Fertilizers");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "EntomophageTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "EntomophageTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Diseases");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "Diseases");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "DicitonaryTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "DicitonaryTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "CropPeriods");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "CropPeriods");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "BusinessTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "BusinessTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "BidTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "BidTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "BidStates");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "BidStates");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "BidContents");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "BidContents");

            migrationBuilder.DropColumn(
                name: "DescrEn",
                table: "AgroTerms");

            migrationBuilder.DropColumn(
                name: "DescrRu",
                table: "AgroTerms");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "AgroTerms");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "AgroTerms");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "AgrotechnicalMeasures");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "AgrotechnicalMeasures");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "AgroOperations");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "AgroOperations");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "AgroMachineTypes");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "AgroMachineTypes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "AgroclimaticZones");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "AgroclimaticZones");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "AdministrativeUnits");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "AdministrativeUnits");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "WeatherTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "WeatherStationTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "WeatherStations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "WeatherReportTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "WeatherProviders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "WeatherJobStatuses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "WeatherImageTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "WeatherFrequencies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "WeatherConditions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Descriptions",
                table: "WeatherAlertTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "WeatherAlertTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "WaterResources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "VegetationPeriods",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Descriptions",
                table: "VegetationIndexes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "VarietyFeatures",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "Varieties",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "UavDataTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "UavCameraTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "TaskStatuses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "SystemDataSources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "SoilTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "RegionTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdminNames",
                table: "Regions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "Regions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShortNames",
                table: "Regions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "ProviderWeatherConditions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "ProductQualityStandards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "PestTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "PesticideTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "Pesticides",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "Persons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "MeliorativeMeasureTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullNames",
                table: "MeasurementUnits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "MeasurementUnits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "IrrigationSources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "IrrigationMethods",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "FlightTargets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "Firms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "FieldUsageStatuses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "FertilizerTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "Fertilizers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "EntomophageTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "Diseases",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "DicitonaryTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "Crops",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "CropPeriods",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "BusinessTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "BidTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "BidStates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "BidContents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Descriptions",
                table: "AgroTerms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "AgroTerms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "AgrotechnicalMeasures",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "AgroOperations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "AgroMachineTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "AgroclimaticZones",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "AdministrativeUnits",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Names",
                table: "WeatherTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "WeatherStationTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "WeatherStations");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "WeatherReportTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "WeatherProviders");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "WeatherJobStatuses");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "WeatherImageTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "WeatherFrequencies");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "WeatherConditions");

            migrationBuilder.DropColumn(
                name: "Descriptions",
                table: "WeatherAlertTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "WeatherAlertTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "WaterResources");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "VegetationPeriods");

            migrationBuilder.DropColumn(
                name: "Descriptions",
                table: "VegetationIndexes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "VarietyFeatures");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "Varieties");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "UavDataTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "UavCameraTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "TaskStatuses");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "SystemDataSources");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "SoilTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "RegionTypes");

            migrationBuilder.DropColumn(
                name: "AdminNames",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "ShortNames",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "ProviderWeatherConditions");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "ProductQualityStandards");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "PestTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "PesticideTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "Pesticides");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "MeliorativeMeasureTypes");

            migrationBuilder.DropColumn(
                name: "FullNames",
                table: "MeasurementUnits");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "MeasurementUnits");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "IrrigationSources");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "IrrigationMethods");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "FlightTargets");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "Firms");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "FieldUsageStatuses");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "FertilizerTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "Fertilizers");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "EntomophageTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "Diseases");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "DicitonaryTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "CropPeriods");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "BusinessTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "BidTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "BidStates");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "BidContents");

            migrationBuilder.DropColumn(
                name: "Descriptions",
                table: "AgroTerms");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "AgroTerms");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "AgrotechnicalMeasures");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "AgroOperations");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "AgroMachineTypes");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "AgroclimaticZones");

            migrationBuilder.DropColumn(
                name: "Names",
                table: "AdministrativeUnits");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WeatherTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "WeatherTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WeatherStationTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "WeatherStationTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WeatherStations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "WeatherStations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WeatherReportTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "WeatherReportTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WeatherProviders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "WeatherProviders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WeatherJobStatuses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "WeatherJobStatuses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WeatherImageTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "WeatherImageTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WeatherFrequencies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "WeatherFrequencies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WeatherConditions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "WeatherConditions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "WeatherAlertTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionRu",
                table: "WeatherAlertTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WeatherAlertTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "WeatherAlertTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "WaterResources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "WaterResources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "VegetationPeriods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "VegetationPeriods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "VegetationIndexes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionRu",
                table: "VegetationIndexes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "VarietyFeatures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "VarietyFeatures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Varieties",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "Varieties",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "UavDataTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "UavDataTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "UavCameraTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "UavCameraTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "TaskStatuses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "TaskStatuses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "SystemDataSources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "SystemDataSources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "SoilTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "SoilTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "RegionTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "RegionTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAdminEn",
                table: "Regions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAdminRu",
                table: "Regions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Regions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "Regions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortNameEn",
                table: "Regions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortNameRu",
                table: "Regions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "ProviderWeatherConditions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "ProviderWeatherConditions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "ProductQualityStandards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "ProductQualityStandards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "PestTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "PestTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "PesticideTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "PesticideTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Pesticides",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "Pesticides",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Persons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "Persons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "MeliorativeMeasureTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "MeliorativeMeasureTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullNameEn",
                table: "MeasurementUnits",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullNameRu",
                table: "MeasurementUnits",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "MeasurementUnits",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "MeasurementUnits",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "IrrigationSources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "IrrigationSources",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "IrrigationMethods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "IrrigationMethods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "FlightTargets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "FlightTargets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Firms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "Firms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "FieldUsageStatuses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "FieldUsageStatuses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "FertilizerTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "FertilizerTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Fertilizers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "Fertilizers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "EntomophageTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "EntomophageTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Diseases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "Diseases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "DicitonaryTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "DicitonaryTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Crops",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "Crops",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "CropPeriods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "CropPeriods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "BusinessTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "BusinessTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "BidTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "BidTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "BidStates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "BidStates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "BidContents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "BidContents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescrEn",
                table: "AgroTerms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescrRu",
                table: "AgroTerms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "AgroTerms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "AgroTerms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "AgrotechnicalMeasures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "AgrotechnicalMeasures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "AgroOperations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "AgroOperations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "AgroMachineTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "AgroMachineTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "AgroclimaticZones",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "AgroclimaticZones",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "AdministrativeUnits",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "AdministrativeUnits",
                type: "text",
                nullable: true);
        }
    }
}
