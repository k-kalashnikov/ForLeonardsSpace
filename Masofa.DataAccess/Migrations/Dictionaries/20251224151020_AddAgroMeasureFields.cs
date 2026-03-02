using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Dictionaries
{
    /// <inheritdoc />
    public partial class AddAgroMeasureFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DayEnd",
                table: "AgrotechnicalMeasures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DayStart",
                table: "AgrotechnicalMeasures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Descriptions",
                table: "AgrotechnicalMeasures",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SoilRecommendations",
                table: "AgrotechnicalMeasures",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayEnd",
                table: "AgrotechnicalMeasures");

            migrationBuilder.DropColumn(
                name: "DayStart",
                table: "AgrotechnicalMeasures");

            migrationBuilder.DropColumn(
                name: "Descriptions",
                table: "AgrotechnicalMeasures");

            migrationBuilder.DropColumn(
                name: "SoilRecommendations",
                table: "AgrotechnicalMeasures");
        }
    }
}
