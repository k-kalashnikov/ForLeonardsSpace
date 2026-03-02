using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.CropMonitoring
{
    /// <inheritdoc />
    public partial class UpdateFiledPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "FieldId",
                table: "FieldPhotos",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "CaptureDateUtc",
                table: "FieldPhotos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FieldPhotos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentRegionId",
                table: "FieldPhotos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RegionId",
                table: "FieldPhotos",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaptureDateUtc",
                table: "FieldPhotos");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "FieldPhotos");

            migrationBuilder.DropColumn(
                name: "ParentRegionId",
                table: "FieldPhotos");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "FieldPhotos");

            migrationBuilder.AlterColumn<Guid>(
                name: "FieldId",
                table: "FieldPhotos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
