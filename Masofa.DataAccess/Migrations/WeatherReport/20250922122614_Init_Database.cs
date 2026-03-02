using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.WeatherReport
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
                name: "Era5DayWeatherForecasts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    SolarRadiationInfluence = table.Column<double>(type: "double precision", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Era5DayWeatherForecasts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Era5DayWeatherReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    SolarRadiationInfluence = table.Column<double>(type: "double precision", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Era5DayWeatherReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Era5HourWeatherForecasts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Hour = table.Column<int>(type: "integer", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    SolarRadiationInfluence = table.Column<double>(type: "double precision", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true)
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
                    Hour = table.Column<int>(type: "integer", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    SolarRadiationInfluence = table.Column<double>(type: "double precision", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Era5HourWeatherReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Era5MonthWeatherReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    SolarRadiationInfluence = table.Column<double>(type: "double precision", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Era5MonthWeatherReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Era5WeekWeatherReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WeekStart = table.Column<DateOnly>(type: "date", nullable: false),
                    WeekEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    SolarRadiationInfluence = table.Column<double>(type: "double precision", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Era5WeekWeatherReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Era5YearWeatherReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    SolarRadiationInfluence = table.Column<double>(type: "double precision", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Era5YearWeatherReports", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Era5DayWeatherForecasts");

            migrationBuilder.DropTable(
                name: "Era5DayWeatherReports");

            migrationBuilder.DropTable(
                name: "Era5HourWeatherForecasts");

            migrationBuilder.DropTable(
                name: "Era5HourWeatherReports");

            migrationBuilder.DropTable(
                name: "Era5MonthWeatherReports");

            migrationBuilder.DropTable(
                name: "Era5WeekWeatherReports");

            migrationBuilder.DropTable(
                name: "Era5YearWeatherReports");
        }
    }
}
