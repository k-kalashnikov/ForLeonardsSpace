using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Add_Qwen_Integration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "QwenAnalysisEnd",
                table: "Bids",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "QwenAnalysisStart",
                table: "Bids",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QwenResultJson",
                table: "Bids",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "QwenTaskId",
                table: "Bids",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QwenAnalysisEnd",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "QwenAnalysisStart",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "QwenResultJson",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "QwenTaskId",
                table: "Bids");
        }
    }
}
