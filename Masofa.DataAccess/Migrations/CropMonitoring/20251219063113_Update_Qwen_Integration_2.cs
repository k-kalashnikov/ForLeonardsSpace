using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Update_Qwen_Integration_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "QwenExpressAnalysisEnd",
                table: "Bids",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "QwenExpressAnalysisStart",
                table: "Bids",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QwenExpressResultJson",
                table: "Bids",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QwenExpressTaskId",
                table: "Bids",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QwenExpressAnalysisEnd",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "QwenExpressAnalysisStart",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "QwenExpressResultJson",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "QwenExpressTaskId",
                table: "Bids");
        }
    }
}
