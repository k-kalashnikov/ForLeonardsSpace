using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Masofa.DataAccess.Migrations.Uav
{
    /// <inheritdoc />
    public partial class AddedUAVFlyPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "UAVFlyPath",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    FlyPath = table.Column<Geometry>(type: "geometry", nullable: true),
                    ProcessingStatus = table.Column<int>(type: "integer", nullable: false),
                    DataTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CameraTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UAVFlyPath", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UAVPhotoCollection",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UAVFlyPathId = table.Column<Guid>(type: "uuid", nullable: false),
                    Point = table.Column<Point>(type: "geometry", nullable: true),
                    AnalysisOnly = table.Column<bool>(type: "boolean", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UAVPhotoCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UAVPhotoCollection_UAVFlyPath_UAVFlyPathId",
                        column: x => x.UAVFlyPathId,
                        principalTable: "UAVFlyPath",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UAVPhoto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    FileStorageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Height = table.Column<double>(type: "double precision", nullable: false),
                    Width = table.Column<double>(type: "double precision", nullable: false),
                    OriginalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    UAVPhotoCollectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UAVPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UAVPhoto_UAVPhotoCollection_UAVPhotoCollectionId",
                        column: x => x.UAVPhotoCollectionId,
                        principalTable: "UAVPhotoCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UAVPhotoCollectionRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    FirmId = table.Column<Guid>(type: "uuid", nullable: true),
                    UAVPhotoCollectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UAVPhotoCollectionRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UAVPhotoCollectionRelation_UAVPhotoCollection_UAVPhotoColle~",
                        column: x => x.UAVPhotoCollectionId,
                        principalTable: "UAVPhotoCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UAVPhoto_UAVPhotoCollectionId",
                table: "UAVPhoto",
                column: "UAVPhotoCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_UAVPhotoCollection_UAVFlyPathId",
                table: "UAVPhotoCollection",
                column: "UAVFlyPathId");

            migrationBuilder.CreateIndex(
                name: "IX_UAVPhotoCollectionRelation_UAVPhotoCollectionId",
                table: "UAVPhotoCollectionRelation",
                column: "UAVPhotoCollectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UAVPhoto");

            migrationBuilder.DropTable(
                name: "UAVPhotoCollectionRelation");

            migrationBuilder.DropTable(
                name: "UAVPhotoCollection");

            migrationBuilder.DropTable(
                name: "UAVFlyPath");
        }
    }
}
