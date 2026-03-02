using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Identity
{
    /// <inheritdoc />
    public partial class Change_Localization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "NameRu",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "AspNetRoles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Names",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "AspNetRoles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameRu",
                table: "AspNetRoles",
                type: "text",
                nullable: true);
        }
    }
}
