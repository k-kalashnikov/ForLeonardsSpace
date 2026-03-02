using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Common
{
    /// <inheritdoc />
    public partial class Update_Database_2_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemBackgroundTaskResults_SystemBackgroundTasks_SystemBac~",
                table: "SystemBackgroundTaskResults");

            migrationBuilder.DropIndex(
                name: "IX_SystemBackgroundTaskResults_SystemBackgroundTaskId",
                table: "SystemBackgroundTaskResults");

            migrationBuilder.AddColumn<string>(
                name: "TaskResultJsonTypeName",
                table: "SystemBackgroundTaskResults",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskResultJsonTypeName",
                table: "SystemBackgroundTaskResults");

            migrationBuilder.CreateIndex(
                name: "IX_SystemBackgroundTaskResults_SystemBackgroundTaskId",
                table: "SystemBackgroundTaskResults",
                column: "SystemBackgroundTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemBackgroundTaskResults_SystemBackgroundTasks_SystemBac~",
                table: "SystemBackgroundTaskResults",
                column: "SystemBackgroundTaskId",
                principalTable: "SystemBackgroundTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
