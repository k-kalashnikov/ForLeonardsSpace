using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.DictionariesHistory
{
    /// <inheritdoc />
    public partial class Init_database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "RegionHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldJson = table.Column<string>(type: "text", nullable: false),
                    NewJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionMapHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldJson = table.Column<string>(type: "text", nullable: false),
                    NewJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionMapHistory", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegionHistory");

            migrationBuilder.DropTable(
                name: "RegionMapHistory");
        }
    }
}
