using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Common
{
    /// <inheritdoc />
    public partial class SystemDocsLocalizing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileStorageId",
                table: "SystemDocumentations");

            migrationBuilder.AddColumn<string>(
                name: "FileStorageIds",
                table: "SystemDocumentations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileStorageIds",
                table: "SystemDocumentations");

            migrationBuilder.AddColumn<Guid>(
                name: "FileStorageId",
                table: "SystemDocumentations",
                type: "uuid",
                nullable: true);
        }
    }
}
