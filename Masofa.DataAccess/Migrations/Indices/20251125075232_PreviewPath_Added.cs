using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Indices
{
    /// <inheritdoc />
    public partial class PreviewPath_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PreviewImagePath",
                table: "OsaviPolygons",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviewImagePath",
                table: "OrviPolygons",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviewImagePath",
                table: "NdviPolygons",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviewImagePath",
                table: "NdmiPolygons",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviewImagePath",
                table: "MndwiPolygons",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviewImagePath",
                table: "GndviPolygons",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviewImagePath",
                table: "EviPolygons",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviewImagePath",
                table: "ArviPolygons",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviewImagePath",
                table: "OsaviPolygons");

            migrationBuilder.DropColumn(
                name: "PreviewImagePath",
                table: "OrviPolygons");

            migrationBuilder.DropColumn(
                name: "PreviewImagePath",
                table: "NdviPolygons");

            migrationBuilder.DropColumn(
                name: "PreviewImagePath",
                table: "NdmiPolygons");

            migrationBuilder.DropColumn(
                name: "PreviewImagePath",
                table: "MndwiPolygons");

            migrationBuilder.DropColumn(
                name: "PreviewImagePath",
                table: "GndviPolygons");

            migrationBuilder.DropColumn(
                name: "PreviewImagePath",
                table: "EviPolygons");

            migrationBuilder.DropColumn(
                name: "PreviewImagePath",
                table: "ArviPolygons");
        }
    }
}
