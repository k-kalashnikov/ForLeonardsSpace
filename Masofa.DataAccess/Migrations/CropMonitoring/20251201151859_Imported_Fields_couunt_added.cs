using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Imported_Fields_couunt_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FieldsCount",
                table: "ImportedFieldReports",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "ImportedFieldReports",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldsCount",
                table: "ImportedFieldReports");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "ImportedFieldReports");
        }
    }
}
