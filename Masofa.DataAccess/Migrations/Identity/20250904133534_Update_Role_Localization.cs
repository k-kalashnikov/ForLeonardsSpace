using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Identity
{
    /// <inheritdoc />
    public partial class Update_Role_Localization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescEn",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "DescRu",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<string>(
                name: "Descriptions",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descriptions",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<string>(
                name: "DescEn",
                table: "AspNetRoles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescRu",
                table: "AspNetRoles",
                type: "text",
                nullable: true);
        }
    }
}
