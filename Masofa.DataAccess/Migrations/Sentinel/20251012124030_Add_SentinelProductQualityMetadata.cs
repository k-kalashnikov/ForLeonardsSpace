using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Sentinel
{
    /// <inheritdoc />
    public partial class Add_SentinelProductQualityMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QueueStatus",
                table: "Sentinel2ProductsQueue",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Sentinel2GenerateIndexStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Sentinel2ProductQueue = table.Column<Guid>(type: "uuid", nullable: false),
                    ArviTiff = table.Column<bool>(type: "boolean", nullable: false),
                    EviTiff = table.Column<bool>(type: "boolean", nullable: false),
                    GndviTiff = table.Column<bool>(type: "boolean", nullable: false),
                    MndwiTiff = table.Column<bool>(type: "boolean", nullable: false),
                    NdmiTiff = table.Column<bool>(type: "boolean", nullable: false),
                    NdviTiff = table.Column<bool>(type: "boolean", nullable: false),
                    OrviTiff = table.Column<bool>(type: "boolean", nullable: false),
                    OsaviTiff = table.Column<bool>(type: "boolean", nullable: false),
                    ArviDb = table.Column<bool>(type: "boolean", nullable: false),
                    EviDb = table.Column<bool>(type: "boolean", nullable: false),
                    GndviDb = table.Column<bool>(type: "boolean", nullable: false),
                    MndwiDb = table.Column<bool>(type: "boolean", nullable: false),
                    NdmiDb = table.Column<bool>(type: "boolean", nullable: false),
                    NdviDb = table.Column<bool>(type: "boolean", nullable: false),
                    OrviDb = table.Column<bool>(type: "boolean", nullable: false),
                    OsaviDb = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sentinel2GenerateIndexStatus", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sentinel2GenerateIndexStatus");

            migrationBuilder.DropColumn(
                name: "QueueStatus",
                table: "Sentinel2ProductsQueue");
        }
    }
}
