using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Sentinel
{
    /// <inheritdoc />
    public partial class Update_Database_IsPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "SentinelProductQualityMetadata",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "SentinelL1CTileMetadata",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "SentinelL1CProductMetadata",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "SentinelInspireMetadata",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Sentinel2ProductsQueue",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Sentinel2ProductsMetadata",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Sentinel2Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "SentinelProductQualityMetadata");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "SentinelL1CTileMetadata");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "SentinelL1CProductMetadata");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "SentinelInspireMetadata");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Sentinel2ProductsQueue");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Sentinel2ProductsMetadata");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Sentinel2Products");
        }
    }
}
