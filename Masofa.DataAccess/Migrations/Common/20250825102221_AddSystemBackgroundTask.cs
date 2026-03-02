using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Common
{
    /// <inheritdoc />
    public partial class AddSystemBackgroundTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemBackgroundTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskType = table.Column<int>(type: "integer", nullable: false),
                    ExecuteTypeName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MaxExecutions = table.Column<int>(type: "integer", nullable: false),
                    ExecutionCount = table.Column<int>(type: "integer", nullable: false),
                    IsRetryable = table.Column<bool>(type: "boolean", nullable: false),
                    MaxRetryCount = table.Column<int>(type: "integer", nullable: false),
                    CurrentRetryCount = table.Column<int>(type: "integer", nullable: false),
                    TaskOptionJson = table.Column<string>(type: "text", nullable: true),
                    ParametrsJson = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemBackgroundTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemBackgroundTaskResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultType = table.Column<int>(type: "integer", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SystemBackgroundTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskResultJson = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemBackgroundTaskResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemBackgroundTaskResults_SystemBackgroundTasks_SystemBac~",
                        column: x => x.SystemBackgroundTaskId,
                        principalTable: "SystemBackgroundTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemBackgroundTaskResults_SystemBackgroundTaskId",
                table: "SystemBackgroundTaskResults",
                column: "SystemBackgroundTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemBackgroundTaskResults");

            migrationBuilder.DropTable(
                name: "SystemBackgroundTasks");
        }
    }
}
