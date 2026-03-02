using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Dictionaries
{
    /// <inheritdoc />
    public partial class AddActiveTemperatureSumToCropPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveTemperatureSumEnd",
                table: "CropPeriods",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActiveTemperatureSumStart",
                table: "CropPeriods",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveTemperatureSumEnd",
                table: "CropPeriods");

            migrationBuilder.DropColumn(
                name: "ActiveTemperatureSumStart",
                table: "CropPeriods");
        }
    }
}
