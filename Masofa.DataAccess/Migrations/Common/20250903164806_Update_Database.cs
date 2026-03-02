using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Common
{
    /// <inheritdoc />
    public partial class Update_Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "SystemBackgroundTasks");

            migrationBuilder.RenameColumn(
                name: "NameRu",
                table: "SystemBackgroundTasks",
                newName: "ParametrsTypeName");

            migrationBuilder.AlterColumn<string>(
                name: "TaskOptionJson",
                table: "SystemBackgroundTasks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParametrsJson",
                table: "SystemBackgroundTasks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Names",
                table: "SystemBackgroundTasks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Names",
                table: "SystemBackgroundTasks");

            migrationBuilder.RenameColumn(
                name: "ParametrsTypeName",
                table: "SystemBackgroundTasks",
                newName: "NameRu");

            migrationBuilder.AlterColumn<string>(
                name: "TaskOptionJson",
                table: "SystemBackgroundTasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ParametrsJson",
                table: "SystemBackgroundTasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "SystemBackgroundTasks",
                type: "text",
                nullable: true);
        }
    }
}
