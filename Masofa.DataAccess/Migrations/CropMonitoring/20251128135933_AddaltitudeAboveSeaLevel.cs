using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class AddaltitudeAboveSeaLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AltitudeAboveSeaLevel",
                table: "Fields",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltitudeAboveSeaLevel",
                table: "Fields");
        }
    }
}
