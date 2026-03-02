using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Update_Season_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CalculationUpdateDate",
                table: "Seasons",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonitoringInterval",
                table: "Seasons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "MonitoringPerioidEnd",
                table: "Seasons",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "MonitoringPerioidStart",
                table: "Seasons",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculationUpdateDate",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "MonitoringInterval",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "MonitoringPerioidEnd",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "MonitoringPerioidStart",
                table: "Seasons");
        }
    }
}
