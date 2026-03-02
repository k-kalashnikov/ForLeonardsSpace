using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Masofa.DataAccess.Migrations.Identity
{
    /// <inheritdoc />
    public partial class OneId_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OneIdUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Pinfl = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Subdivision = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Base = table.Column<string>(type: "text", nullable: false),
                    Organization = table.Column<int>(type: "integer", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneIdUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OneIdRoleInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OneIdUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneIdRoleInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OneIdRoleInfos_OneIdUsers_OneIdUserId",
                        column: x => x.OneIdUserId,
                        principalTable: "OneIdUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OneIdSystemInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OneIdUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OneIdSystemInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OneIdSystemInfos_OneIdUsers_OneIdUserId",
                        column: x => x.OneIdUserId,
                        principalTable: "OneIdUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OneIdRoleInfos_OneIdUserId",
                table: "OneIdRoleInfos",
                column: "OneIdUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OneIdSystemInfos_OneIdUserId",
                table: "OneIdSystemInfos",
                column: "OneIdUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OneIdRoleInfos");

            migrationBuilder.DropTable(
                name: "OneIdSystemInfos");

            migrationBuilder.DropTable(
                name: "OneIdUsers");
        }
    }
}
