using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Era
{
    /// <inheritdoc />
    public partial class Added_Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FieldId",
                table: "EraWeatherStations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "SolarRadiation",
                table: "EraWeatherData",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldId",
                table: "EraWeatherStations");

            migrationBuilder.DropColumn(
                name: "SolarRadiation",
                table: "EraWeatherData");
        }
    }
}
