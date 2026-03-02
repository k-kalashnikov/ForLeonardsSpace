using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Init_Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Bids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    BidTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    BidStateId = table.Column<Guid>(type: "uuid", nullable: false),
                    ForemanId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeadlineDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    VarietyId = table.Column<Guid>(type: "uuid", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Lat = table.Column<double>(type: "double precision", nullable: true),
                    Lng = table.Column<double>(type: "double precision", nullable: true),
                    Number = table.Column<long>(type: "bigint", nullable: false),
                    FieldPlantingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Customer = table.Column<string>(type: "text", nullable: true),
                    FileResultId = table.Column<Guid>(type: "uuid", nullable: true),
                    BidTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bids", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BidTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CropId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchemaVersion = table.Column<int>(type: "integer", nullable: false),
                    ContentVersion = table.Column<int>(type: "integer", nullable: false),
                    DataJson = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    FieldArea = table.Column<double>(type: "double precision", nullable: true),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    SoilTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    AgroclimaticZoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    AgricultureProducerId = table.Column<Guid>(type: "uuid", nullable: true),
                    IrrigationTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    IrrigationSourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    WaterSaving = table.Column<bool>(type: "boolean", nullable: true),
                    SoilIndex = table.Column<double>(type: "double precision", nullable: true),
                    Polygon = table.Column<Polygon>(type: "geometry", nullable: true),
                    Control = table.Column<bool>(type: "boolean", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    PlantingDate = table.Column<DateOnly>(type: "date", nullable: true),
                    HarvestingDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    VarietyId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlantingDatePlan = table.Column<DateOnly>(type: "date", nullable: true),
                    FieldArea = table.Column<double>(type: "double precision", nullable: true),
                    HarvestingDatePlan = table.Column<DateOnly>(type: "date", nullable: true),
                    YieldHa = table.Column<double>(type: "double precision", nullable: true),
                    Yield = table.Column<double>(type: "double precision", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bids");

            migrationBuilder.DropTable(
                name: "BidTemplates");

            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "Seasons");
        }
    }
}
