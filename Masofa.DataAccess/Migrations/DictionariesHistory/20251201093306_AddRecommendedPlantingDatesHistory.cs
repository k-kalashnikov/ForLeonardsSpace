using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.DictionariesHistory
{
    /// <inheritdoc />
    public partial class AddRecommendedPlantingDatesHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SolarRadiationInfluences",
                table: "SolarRadiationInfluences");

            migrationBuilder.RenameTable(
                name: "SolarRadiationInfluences",
                newName: "SolarRadiationInfluenceHistories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SolarRadiationInfluenceHistories",
                table: "SolarRadiationInfluenceHistories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RecommendedPlantingDatesHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewModelJson = table.Column<string>(type: "text", nullable: true),
                    OldModelJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendedPlantingDatesHistories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecommendedPlantingDatesHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SolarRadiationInfluenceHistories",
                table: "SolarRadiationInfluenceHistories");

            migrationBuilder.RenameTable(
                name: "SolarRadiationInfluenceHistories",
                newName: "SolarRadiationInfluences");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SolarRadiationInfluences",
                table: "SolarRadiationInfluences",
                column: "Id");
        }
    }
}
