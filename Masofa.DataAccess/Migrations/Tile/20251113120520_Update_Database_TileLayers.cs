using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Tile
{
    /// <inheritdoc />
    public partial class Update_Database_TileLayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TileLayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeoServerName = table.Column<string>(type: "text", nullable: false),
                    GeoServerRelationPath = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Names = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TileLayers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TileLayers");
        }
    }
}
