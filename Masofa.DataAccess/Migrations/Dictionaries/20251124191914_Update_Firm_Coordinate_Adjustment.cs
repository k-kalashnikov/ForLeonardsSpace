using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Masofa.DataAccess.Migrations.Dictionaries
{
    /// <inheritdoc />
    public partial class Update_Firm_Coordinate_Adjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Firms");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Firms");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Firms",
                type: "geometry",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Firms");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Firms",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Firms",
                type: "double precision",
                nullable: true);
        }
    }
}
