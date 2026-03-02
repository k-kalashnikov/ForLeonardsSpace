using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.IBMWeather
{
    /// <inheritdoc />
    public partial class Report_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IbmDayWeatherForecasts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
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
                    table.PrimaryKey("PK_IbmDayWeatherForecasts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IbmDayWeatherForecasts");
        }
    }
}
