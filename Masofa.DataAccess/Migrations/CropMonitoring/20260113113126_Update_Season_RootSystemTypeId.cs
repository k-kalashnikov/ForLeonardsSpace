using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class Update_Season_RootSystemTypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RootSystemType",
                table: "Seasons",
                newName: "GovernmentContractNumber");

            migrationBuilder.AddColumn<DateOnly>(
                name: "GovernmentContractEndDate",
                table: "Seasons",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "GovernmentContractStartDate",
                table: "Seasons",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RootSystemTypeId",
                table: "Seasons",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GovernmentContractEndDate",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "GovernmentContractStartDate",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "RootSystemTypeId",
                table: "Seasons");

            migrationBuilder.RenameColumn(
                name: "GovernmentContractNumber",
                table: "Seasons",
                newName: "RootSystemType");
        }
    }
}
