using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Update_Database_IsPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "SoilDatas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Seasons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "SatelliteSearchConfigs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Fields",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "FieldInsuranceHistories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "FieldAgroProducerHistories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "FieldAgroOperations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Field_Product_Mapping",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "BidTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Bids",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "SoilDatas");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "SatelliteSearchConfigs");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "FieldInsuranceHistories");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "FieldAgroProducerHistories");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "FieldAgroOperations");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Field_Product_Mapping");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "BidTemplates");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Bids");
        }
    }
}
