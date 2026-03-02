using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Masofa.DataAccess.Migrations.Dictionaries
{
    /// <inheritdoc />
    public partial class AddDictionaryFieldType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Geometry>(
                name: "Polygon",
                table: "AgroclimaticZones",
                type: "geometry",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PolygonJson",
                table: "AgroclimaticZones",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AgroclimaticZoneRegionRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgroclimaticZoneId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgroclimaticZoneRegionRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    ExtData = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Names = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldTypes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgroclimaticZoneRegionRelations");

            migrationBuilder.DropTable(
                name: "FieldTypes");

            migrationBuilder.DropColumn(
                name: "Polygon",
                table: "AgroclimaticZones");

            migrationBuilder.DropColumn(
                name: "PolygonJson",
                table: "AgroclimaticZones");
        }
    }
}
