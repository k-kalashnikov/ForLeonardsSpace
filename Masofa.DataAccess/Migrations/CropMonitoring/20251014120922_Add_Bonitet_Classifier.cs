using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Add_Bonitet_Classifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BonitetScore",
                table: "Fields",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Classifier",
                table: "Fields",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonitetScore",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "Classifier",
                table: "Fields");
        }
    }
}
