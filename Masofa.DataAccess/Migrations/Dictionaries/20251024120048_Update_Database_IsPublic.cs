using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Dictionaries
{
    /// <inheritdoc />
    public partial class Update_Database_IsPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "WeatherTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "WeatherStationTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "WeatherStations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "WeatherReportTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "WeatherProviders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "WeatherJobStatuses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "WeatherImageTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "WeatherFrequencies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "WeatherConditions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "WeatherAlertTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "WaterResources",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "VegetationPeriods",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "VegetationIndexes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "VarietyFeatures",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Varieties",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "UavDataTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "UavCameraTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "TaskStatuses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Tags",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "SystemDataSources",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "SolarRadiationInfluences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "SoilTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "RegulatoryDocumentations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "RegionTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Regions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "RegionMaps",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "ProviderWeatherConditions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "ProductQualityStandards",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "PestTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "PesticideTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Pesticides",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Persons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "MeliorativeMeasureTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "MeasurementUnits",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "IrrigationSources",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "IrrigationMethods",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Insurances",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "FlightTargets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Firms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "FieldUsageStatuses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "FertilizerTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Fertilizers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "ExperimentalFarmingMethods",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "EntomophageTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Diseases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "DicitonaryTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Crops",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "CropPeriodVegetationIndexes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "CropPeriods",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "ClimaticStandards",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "BusinessTypesPerson",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "BusinessTypesFirm",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "BusinessTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "BidTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "BidStates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "BidContents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "AgroTerms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "AgrotechnicalMeasures",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "AgroOperations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "AgroMachineTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "AgroclimaticZones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "AdministrativeUnits",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "WeatherTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "WeatherStationTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "WeatherStations");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "WeatherReportTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "WeatherProviders");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "WeatherJobStatuses");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "WeatherImageTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "WeatherFrequencies");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "WeatherConditions");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "WeatherAlertTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "WaterResources");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "VegetationPeriods");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "VegetationIndexes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "VarietyFeatures");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Varieties");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "UavDataTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "UavCameraTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "TaskStatuses");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "SystemDataSources");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "SolarRadiationInfluences");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "SoilTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "RegulatoryDocumentations");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "RegionTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "RegionMaps");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "ProviderWeatherConditions");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "ProductQualityStandards");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "PestTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "PesticideTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Pesticides");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "MeliorativeMeasureTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "MeasurementUnits");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "IrrigationSources");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "IrrigationMethods");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Insurances");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "FlightTargets");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Firms");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "FieldUsageStatuses");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "FertilizerTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Fertilizers");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "ExperimentalFarmingMethods");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "EntomophageTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Diseases");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "DicitonaryTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Crops");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "CropPeriodVegetationIndexes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "CropPeriods");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "ClimaticStandards");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "BusinessTypesPerson");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "BusinessTypesFirm");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "BusinessTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "BidTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "BidStates");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "BidContents");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "AgroTerms");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "AgrotechnicalMeasures");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "AgroOperations");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "AgroMachineTypes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "AgroclimaticZones");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "AdministrativeUnits");
        }
    }
}
