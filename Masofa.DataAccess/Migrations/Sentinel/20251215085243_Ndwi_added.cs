using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Sentinel
{
    /// <inheritdoc />
    public partial class Ndwi_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NdwiDb",
                table: "Sentinel2GenerateIndexStatus",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NdwiTiff",
                table: "Sentinel2GenerateIndexStatus",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NdwiDb",
                table: "Sentinel2GenerateIndexStatus");

            migrationBuilder.DropColumn(
                name: "NdwiTiff",
                table: "Sentinel2GenerateIndexStatus");
        }
    }
}
