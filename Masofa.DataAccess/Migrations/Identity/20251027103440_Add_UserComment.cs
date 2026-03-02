using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Identity
{
    /// <inheritdoc />
    public partial class Add_UserComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConnectionBasis",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeputyId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ConnectionBasis",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DeputyId",
                table: "AspNetUsers");
        }
    }
}
