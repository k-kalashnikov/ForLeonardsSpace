using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class AddIndexesGeo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Field_Product_Mapping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SatelliteType = table.Column<int>(type: "integer", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field_Product_Mapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SatelliteSearchConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SentinelPolygon = table.Column<Polygon>(type: "geometry", nullable: true),
                    LandsatLeftDown = table.Column<Point>(type: "geometry", nullable: true),
                    LandsatRightUp = table.Column<Point>(type: "geometry", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FieldsCount = table.Column<int>(type: "integer", nullable: false),
                    BufferDistance = table.Column<double>(type: "double precision", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SatelliteSearchConfigs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_fields_create_at",
                table: "Fields",
                column: "CreateAt");

            migrationBuilder.CreateIndex(
                name: "idx_fields_external_id",
                table: "Fields",
                column: "ExternalId");

            migrationBuilder.CreateIndex(
                name: "idx_fields_name",
                table: "Fields",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "idx_fields_polygon",
                table: "Fields",
                column: "Polygon")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "idx_fields_region_id",
                table: "Fields",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "idx_field_product_mapping_field_id",
                table: "Field_Product_Mapping",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "idx_field_product_mapping_field_satellite",
                table: "Field_Product_Mapping",
                columns: new[] { "FieldId", "SatelliteType" });

            migrationBuilder.CreateIndex(
                name: "idx_field_product_mapping_product_id",
                table: "Field_Product_Mapping",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "idx_field_product_mapping_satellite_type",
                table: "Field_Product_Mapping",
                column: "SatelliteType");

            migrationBuilder.CreateIndex(
                name: "uk_field_product_mapping_field_product",
                table: "Field_Product_Mapping",
                columns: new[] { "FieldId", "ProductId", "SatelliteType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SatelliteSearchConfigs_IsActive",
                table: "SatelliteSearchConfigs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SatelliteSearchConfigs_IsActive_CreateAt",
                table: "SatelliteSearchConfigs",
                columns: new[] { "IsActive", "CreateAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Field_Product_Mapping");

            migrationBuilder.DropTable(
                name: "SatelliteSearchConfigs");

            migrationBuilder.DropIndex(
                name: "idx_fields_create_at",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "idx_fields_external_id",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "idx_fields_name",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "idx_fields_polygon",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "idx_fields_region_id",
                table: "Fields");
        }
    }
}
