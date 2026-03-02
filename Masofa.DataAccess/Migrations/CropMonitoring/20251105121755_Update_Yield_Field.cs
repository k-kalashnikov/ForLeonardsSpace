using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Update_Yield_Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YieldHa",
                table: "Seasons",
                newName: "YieldHaFact");

            migrationBuilder.RenameColumn(
                name: "Yield",
                table: "Seasons",
                newName: "YieldFact");

            migrationBuilder.AddColumn<double>(
                name: "YieldPlan",
                table: "Seasons",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "YieldHaPlan",
                table: "Seasons",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YieldPlan",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "YieldHaPlan",
                table: "Seasons");

            migrationBuilder.RenameColumn(
                name: "YieldHaFact",
                table: "Seasons",
                newName: "YieldHa");

            migrationBuilder.RenameColumn(
                name: "YieldFact",
                table: "Seasons",
                newName: "Yield");
        }
    }
}
