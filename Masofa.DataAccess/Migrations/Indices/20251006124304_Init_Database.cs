using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Masofa.DataAccess.Migrations.Indices
{
    /// <inheritdoc />
    public partial class Init_Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "ArviPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BZero8 = table.Column<float>(type: "real", nullable: false),
                    BZero4 = table.Column<float>(type: "real", nullable: false),
                    BZero2 = table.Column<float>(type: "real", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Point = table.Column<Point>(type: "geometry", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArviPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArviPolygonRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArviId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArviPolygonRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArviPolygons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Polygon = table.Column<Polygon>(type: "geometry", nullable: false),
                    FileStorageItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsColored = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArviPolygons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EviPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BZero8 = table.Column<float>(type: "real", nullable: false),
                    BZero4 = table.Column<float>(type: "real", nullable: false),
                    BZero2 = table.Column<float>(type: "real", nullable: true),
                    EPS = table.Column<float>(type: "real", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Point = table.Column<Point>(type: "geometry", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EviPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EviPolygonRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EviId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EviPolygonRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EviPolygons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Polygon = table.Column<Polygon>(type: "geometry", nullable: false),
                    FileStorageItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsColored = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EviPolygons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GndviPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BZero8 = table.Column<float>(type: "real", nullable: false),
                    BZero3 = table.Column<float>(type: "real", nullable: false),
                    EPS = table.Column<float>(type: "real", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Point = table.Column<Point>(type: "geometry", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GndviPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GndviPolygonRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GNdviId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GndviPolygonRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GndviPolygons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Polygon = table.Column<Polygon>(type: "geometry", nullable: false),
                    FileStorageItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsColored = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GndviPolygons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MndwiPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BZero3 = table.Column<float>(type: "real", nullable: false),
                    B11 = table.Column<float>(type: "real", nullable: false),
                    EPS = table.Column<float>(type: "real", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Point = table.Column<Point>(type: "geometry", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MndwiPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MndwiPolygonRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MndWiId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MndwiPolygonRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MndwiPolygons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Polygon = table.Column<Polygon>(type: "geometry", nullable: false),
                    FileStorageItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsColored = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MndwiPolygons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NdmiPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BZero8 = table.Column<float>(type: "real", nullable: false),
                    B11 = table.Column<float>(type: "real", nullable: false),
                    EPS = table.Column<float>(type: "real", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Point = table.Column<Point>(type: "geometry", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdmiPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NdmiPolygonRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NdMiId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdmiPolygonRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NdmiPolygons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Polygon = table.Column<Polygon>(type: "geometry", nullable: false),
                    FileStorageItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsColored = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdmiPolygons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NdviPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BZero8 = table.Column<float>(type: "real", nullable: false),
                    BZero4 = table.Column<float>(type: "real", nullable: false),
                    EPS = table.Column<float>(type: "real", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Point = table.Column<Point>(type: "geometry", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdviPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NdviPolygonRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NdViId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdviPolygonRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NdviPolygons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Polygon = table.Column<Polygon>(type: "geometry", nullable: false),
                    FileStorageItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsColored = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdviPolygons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrviPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BZero8 = table.Column<float>(type: "real", nullable: false),
                    BZero3 = table.Column<float>(type: "real", nullable: false),
                    EPS = table.Column<float>(type: "real", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Point = table.Column<Point>(type: "geometry", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrviPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrviPolygonRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrViId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrviPolygonRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrviPolygons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Polygon = table.Column<Polygon>(type: "geometry", nullable: false),
                    FileStorageItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsColored = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrviPolygons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OsaviPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BZero8 = table.Column<float>(type: "real", nullable: false),
                    BZero4 = table.Column<float>(type: "real", nullable: false),
                    EPS = table.Column<float>(type: "real", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Point = table.Column<Point>(type: "geometry", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsaviPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OsaviPolygonRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OsaViId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsaviPolygonRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OsaviPolygons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSourceType = table.Column<int>(type: "integer", nullable: false),
                    SatelliteProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Polygon = table.Column<Polygon>(type: "geometry", nullable: false),
                    FileStorageItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsColored = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsaviPolygons", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArviPoints");

            migrationBuilder.DropTable(
                name: "ArviPolygonRelations");

            migrationBuilder.DropTable(
                name: "ArviPolygons");

            migrationBuilder.DropTable(
                name: "EviPoints");

            migrationBuilder.DropTable(
                name: "EviPolygonRelations");

            migrationBuilder.DropTable(
                name: "EviPolygons");

            migrationBuilder.DropTable(
                name: "GndviPoints");

            migrationBuilder.DropTable(
                name: "GndviPolygonRelations");

            migrationBuilder.DropTable(
                name: "GndviPolygons");

            migrationBuilder.DropTable(
                name: "MndwiPoints");

            migrationBuilder.DropTable(
                name: "MndwiPolygonRelations");

            migrationBuilder.DropTable(
                name: "MndwiPolygons");

            migrationBuilder.DropTable(
                name: "NdmiPoints");

            migrationBuilder.DropTable(
                name: "NdmiPolygonRelations");

            migrationBuilder.DropTable(
                name: "NdmiPolygons");

            migrationBuilder.DropTable(
                name: "NdviPoints");

            migrationBuilder.DropTable(
                name: "NdviPolygonRelations");

            migrationBuilder.DropTable(
                name: "NdviPolygons");

            migrationBuilder.DropTable(
                name: "OrviPoints");

            migrationBuilder.DropTable(
                name: "OrviPolygonRelations");

            migrationBuilder.DropTable(
                name: "OrviPolygons");

            migrationBuilder.DropTable(
                name: "OsaviPoints");

            migrationBuilder.DropTable(
                name: "OsaviPolygonRelations");

            migrationBuilder.DropTable(
                name: "OsaviPolygons");
        }
    }
}
