using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.AnaliticReport
{
    /// <inheritdoc />
    public partial class Update_Database1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalizationFile",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QwenJobId",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportState",
                table: "FarmerRecomendationReports",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalizationFile",
                table: "FarmerRecomendationReports");

            migrationBuilder.DropColumn(
                name: "QwenJobId",
                table: "FarmerRecomendationReports");

            migrationBuilder.DropColumn(
                name: "ReportState",
                table: "FarmerRecomendationReports");
        }
    }
}
