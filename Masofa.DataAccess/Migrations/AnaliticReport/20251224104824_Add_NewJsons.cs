using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.AnaliticReport
{
    /// <inheritdoc />
    public partial class Add_NewJsons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WeatherJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "SoilJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "SeasonJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "MonitoringJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "IrrigationJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "IndicesJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "HeaderJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "GrowthStagesJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FieldJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FertilizationJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CalendarJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "BidResultsJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClimaticSummJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FertPestJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QwenResult",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BidResultsJson",
                table: "FarmerRecomendationReports");

            migrationBuilder.DropColumn(
                name: "ClimaticSummJson",
                table: "FarmerRecomendationReports");

            migrationBuilder.DropColumn(
                name: "FertPestJson",
                table: "FarmerRecomendationReports");

            migrationBuilder.DropColumn(
                name: "QwenResult",
                table: "FarmerRecomendationReports");

            migrationBuilder.AlterColumn<string>(
                name: "WeatherJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SoilJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeasonJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MonitoringJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IrrigationJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IndicesJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HeaderJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GrowthStagesJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FieldJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FertilizationJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CalendarJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
