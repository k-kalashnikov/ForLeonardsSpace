using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Era
{
    /// <inheritdoc />
    public partial class Update_Database_Hours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Era5HourWeatherForecasts");

            migrationBuilder.DropTable(
                name: "Era5HourWeatherReports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Era5HourWeatherForecasts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Hour = table.Column<int>(type: "integer", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    SolarRadiationInfluence = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Era5HourWeatherForecasts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Era5HourWeatherReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Hour = table.Column<int>(type: "integer", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    SolarRadiationInfluence = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Era5HourWeatherReports", x => x.Id);
                });
        }
    }
}
