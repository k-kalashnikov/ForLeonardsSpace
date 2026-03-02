using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Common
{
    /// <inheritdoc />
    public partial class Update_Database_IsPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "UserTickets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "TagRelations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "SystemBackgroundTasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "SystemBackgroundTaskResults",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "SatelliteProducts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "FileStorageItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "EmailMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "UserTickets");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "TagRelations");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "SystemBackgroundTasks");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "SystemBackgroundTaskResults");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "SatelliteProducts");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "FileStorageItems");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "EmailMessages");
        }
    }
}
