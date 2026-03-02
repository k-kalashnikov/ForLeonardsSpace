using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.IBMWeather
{
    /// <inheritdoc />
    public partial class Add_AgroClimaticZone_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AgroclimaticZoneId",
                table: "IBMMeteoStations",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgroclimaticZoneId",
                table: "IBMMeteoStations");
        }
    }
}
