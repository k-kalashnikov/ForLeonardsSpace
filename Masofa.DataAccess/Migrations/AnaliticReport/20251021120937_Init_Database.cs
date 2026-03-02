using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.AnaliticReport
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
                name: "FarmerRecomendationReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileStorageItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    HeaderJson = table.Column<string>(type: "text", nullable: false),
                    SoilJson = table.Column<string>(type: "text", nullable: false),
                    CalendarJson = table.Column<string>(type: "text", nullable: false),
                    IrrigationJson = table.Column<string>(type: "text", nullable: false),
                    WeatherJson = table.Column<string>(type: "text", nullable: false),
                    MonitoringJson = table.Column<string>(type: "text", nullable: false),
                    FertilizationJson = table.Column<string>(type: "text", nullable: false),
                    GrowthStagesJson = table.Column<string>(type: "text", nullable: false),
                    IndicesJson = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmerRecomendationReports", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FarmerRecomendationReports");
        }
    }
}
