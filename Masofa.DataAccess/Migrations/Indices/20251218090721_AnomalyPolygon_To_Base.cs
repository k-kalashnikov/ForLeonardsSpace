using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Indices
{
    /// <inheritdoc />
    public partial class AnomalyPolygon_To_Base : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreateUser",
                table: "AnomalyPolygons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "AnomalyPolygons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateAt",
                table: "AnomalyPolygons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastUpdateUser",
                table: "AnomalyPolygons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "OriginalDate",
                table: "AnomalyPolygons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AnomalyPolygons",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateUser",
                table: "AnomalyPolygons");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "AnomalyPolygons");

            migrationBuilder.DropColumn(
                name: "LastUpdateAt",
                table: "AnomalyPolygons");

            migrationBuilder.DropColumn(
                name: "LastUpdateUser",
                table: "AnomalyPolygons");

            migrationBuilder.DropColumn(
                name: "OriginalDate",
                table: "AnomalyPolygons");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AnomalyPolygons");
        }
    }
}
