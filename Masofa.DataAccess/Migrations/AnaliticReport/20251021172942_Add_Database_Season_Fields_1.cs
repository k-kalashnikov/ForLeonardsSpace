using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.AnaliticReport
{
    /// <inheritdoc />
    public partial class Add_Database_Season_Fields_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FieldJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SeasonJson",
                table: "FarmerRecomendationReports",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldJson",
                table: "FarmerRecomendationReports");

            migrationBuilder.DropColumn(
                name: "SeasonJson",
                table: "FarmerRecomendationReports");
        }
    }
}
