using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Era
{
    /// <inheritdoc />
    public partial class Normalized_Day_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Era5DayNormalizedWeather",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_Era5DayNormalizedWeather", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Era5DayNormalizedWeather");
        }
    }
}
