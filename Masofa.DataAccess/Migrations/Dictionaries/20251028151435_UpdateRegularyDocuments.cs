using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Dictionaries
{
    /// <inheritdoc />
    public partial class UpdateRegularyDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentUrl",
                table: "RegulatoryDocumentations");

            migrationBuilder.AddColumn<Guid>(
                name: "FileStorageId",
                table: "RegulatoryDocumentations",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileStorageId",
                table: "RegulatoryDocumentations");

            migrationBuilder.AddColumn<string>(
                name: "DocumentUrl",
                table: "RegulatoryDocumentations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
