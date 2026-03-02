using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.IBMWeather
{
    /// <inheritdoc />
    public partial class Update_Database_Add_Reports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IbmDayNormalizedWeathers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IbmDayNormalizedWeathers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IbmDayWeatherReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsFrostDanger = table.Column<bool>(type: "boolean", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IbmDayWeatherReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IbmMonthWeatherReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsFrostDanger = table.Column<bool>(type: "boolean", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IbmMonthWeatherReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IbmWeekWeatherReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsFrostDanger = table.Column<bool>(type: "boolean", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IbmWeekWeatherReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IbmYearWeatherReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsFrostDanger = table.Column<bool>(type: "boolean", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTotal = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTotal = table.Column<double>(type: "double precision", nullable: false),
                    Fallout = table.Column<double>(type: "double precision", nullable: false),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    WindSpeed = table.Column<double>(type: "double precision", nullable: false),
                    WindDerection = table.Column<double>(type: "double precision", nullable: false),
                    WeatherStation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IbmYearWeatherReports", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IbmDayNormalizedWeathers");

            migrationBuilder.DropTable(
                name: "IbmDayWeatherReports");

            migrationBuilder.DropTable(
                name: "IbmMonthWeatherReports");

            migrationBuilder.DropTable(
                name: "IbmWeekWeatherReports");

            migrationBuilder.DropTable(
                name: "IbmYearWeatherReports");
        }
    }
}
