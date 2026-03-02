using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Sentinel
{
    /// <inheritdoc />
    public partial class AddIndexesGeo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_sentinel2_products_create_at",
                table: "Sentinel2Products",
                column: "CreateAt");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel2_products_inspire_id",
                table: "Sentinel2Products",
                column: "SentinelInspireMetadataId");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel2_products_satellite_id",
                table: "Sentinel2Products",
                column: "SatellateProductId");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel2_product_metadata_content_date_end",
                table: "Sentinel2ProductsMetadata",
                column: "ContentDateEnd");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel2_product_metadata_content_date_start",
                table: "Sentinel2ProductsMetadata",
                column: "ContentDateStart");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel2_product_metadata_content_dates",
                table: "Sentinel2ProductsMetadata",
                columns: new[] { "ContentDateStart", "ContentDateEnd" });

            migrationBuilder.CreateIndex(
                name: "idx_sentinel2_product_metadata_product_id",
                table: "Sentinel2ProductsMetadata",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel2_product_queue_create_at",
                table: "Sentinel2ProductsQueue",
                column: "CreateAt");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel2_product_queue_product_id",
                table: "Sentinel2ProductsQueue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel2_product_queue_status",
                table: "Sentinel2ProductsQueue",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel_inspire_bbox",
                table: "SentinelInspireMetadata",
                columns: new[] { "WestBoundLongitude", "EastBoundLongitude", "SouthBoundLatitude", "NorthBoundLatitude" });

            migrationBuilder.CreateIndex(
                name: "idx_sentinel_inspire_datestamp",
                table: "SentinelInspireMetadata",
                column: "DateStamp");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel_inspire_file_identifier",
                table: "SentinelInspireMetadata",
                column: "FileIdentifier");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel_l1c_product_metadata_generation_time",
                table: "SentinelL1CProductMetadata",
                column: "GenerationTime");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel_l1c_product_metadata_start_time",
                table: "SentinelL1CProductMetadata",
                column: "ProductStartTime");

            migrationBuilder.CreateIndex(
                name: "idx_sentinel_l1c_product_metadata_stop_time",
                table: "SentinelL1CProductMetadata",
                column: "ProductStopTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
