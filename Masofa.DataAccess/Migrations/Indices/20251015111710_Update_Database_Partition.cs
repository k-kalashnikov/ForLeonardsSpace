using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Masofa.DataAccess.Migrations.Indices
{
    /// <inheritdoc />
    public partial class Update_Database_Partition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ArviPoints");

            migrationBuilder.Sql(@"
                CREATE TABLE ""ArviPoints"" (
                    ""Id"" uuid NOT NULL,
                    ""BZero8"" real NOT NULL,
                    ""BZero4"" real NOT NULL,
                    ""BZero2"" real NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Point"" geometry NOT NULL,
                    ""RegionId"" uuid NULL,
                    ""FieldId"" uuid NULL,
                    ""SeasonId"" uuid NULL,
                    CONSTRAINT ""PK_ArviPoints"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "EviPoints");

            migrationBuilder.Sql(@"
                CREATE TABLE ""EviPoints"" (
                    ""Id"" uuid NOT NULL,
                    ""BZero8"" real NOT NULL,
                    ""BZero4"" real NOT NULL,
                    ""BZero2"" real NULL,
                    ""EPS"" real NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Point"" geometry NOT NULL,
                    ""RegionId"" uuid NULL,
                    ""FieldId"" uuid NULL,
                    ""SeasonId"" uuid NULL,
                    CONSTRAINT ""PK_EviPoints"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "GndviPoints");

            migrationBuilder.Sql(@"
                CREATE TABLE ""GndviPoints"" (
                    ""Id"" uuid NOT NULL,
                    ""BZero8"" real NOT NULL,
                    ""BZero3"" real NOT NULL,
                    ""EPS"" real NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Point"" geometry NOT NULL,
                    ""RegionId"" uuid NULL,
                    ""FieldId"" uuid NULL,
                    ""SeasonId"" uuid NULL,
                    CONSTRAINT ""PK_GndviPoints"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "MndwiPoints");

            migrationBuilder.Sql(@"
                CREATE TABLE ""MndwiPoints"" (
                    ""Id"" uuid NOT NULL,
                    ""BZero3"" real NOT NULL,
                    ""B11"" real NOT NULL,
                    ""EPS"" real NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Point"" geometry NOT NULL,
                    ""RegionId"" uuid NULL,
                    ""FieldId"" uuid NULL,
                    ""SeasonId"" uuid NULL,
                    CONSTRAINT ""PK_MndwiPoints"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "NdmiPoints");

            migrationBuilder.Sql(@"
                CREATE TABLE ""NdmiPoints"" (
                    ""Id"" uuid NOT NULL,
                    ""BZero8"" real NOT NULL,
                    ""B11"" real NOT NULL,
                    ""EPS"" real NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Point"" geometry NOT NULL,
                    ""RegionId"" uuid NULL,
                    ""FieldId"" uuid NULL,
                    ""SeasonId"" uuid NULL,
                    CONSTRAINT ""PK_NdmiPoints"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "NdviPoints");

            migrationBuilder.Sql(@"
                CREATE TABLE ""NdviPoints"" (
                    ""Id"" uuid NOT NULL,
                    ""BZero8"" real NOT NULL,
                    ""BZero4"" real NOT NULL,
                    ""EPS"" real NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Point"" geometry NOT NULL,
                    ""RegionId"" uuid NULL,
                    ""FieldId"" uuid NULL,
                    ""SeasonId"" uuid NULL,
                    CONSTRAINT ""PK_NdviPoints"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "OrviPoints");

            migrationBuilder.Sql(@"
                CREATE TABLE ""OrviPoints"" (
                    ""Id"" uuid NOT NULL,
                    ""BZero8"" real NOT NULL,
                    ""BZero3"" real NOT NULL,
                    ""EPS"" real NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Point"" geometry NOT NULL,
                    ""RegionId"" uuid NULL,
                    ""FieldId"" uuid NULL,
                    ""SeasonId"" uuid NULL,
                    CONSTRAINT ""PK_OrviPoints"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "OsaviPoints");

            migrationBuilder.Sql(@"
                CREATE TABLE ""OsaviPoints"" (
                    ""Id"" uuid NOT NULL,
                    ""BZero8"" real NOT NULL,
                    ""BZero4"" real NOT NULL,
                    ""EPS"" real NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Point"" geometry NOT NULL,
                    ""RegionId"" uuid NULL,
                    ""FieldId"" uuid NULL,
                    ""SeasonId"" uuid NULL,
                    CONSTRAINT ""PK_OsaviPoints"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "ArviPolygons");

            migrationBuilder.Sql(@"
                CREATE TABLE ""ArviPolygons"" (
                    ""Id"" uuid NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Polygon"" geometry NOT NULL,
                    ""FileStorageItemId"" uuid NOT NULL,
                    ""IsColored"" boolean NOT NULL,
                    CONSTRAINT ""PK_ArviPolygons"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "EviPolygons");

            migrationBuilder.Sql(@"
                CREATE TABLE ""EviPolygons"" (
                    ""Id"" uuid NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Polygon"" geometry NOT NULL,
                    ""FileStorageItemId"" uuid NOT NULL,
                    ""IsColored"" boolean NOT NULL,
                    CONSTRAINT ""PK_EviPolygons"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "GndviPolygons");

            migrationBuilder.Sql(@"
                CREATE TABLE ""GndviPolygons"" (
                    ""Id"" uuid NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Polygon"" geometry NOT NULL,
                    ""FileStorageItemId"" uuid NOT NULL,
                    ""IsColored"" boolean NOT NULL,
                    CONSTRAINT ""PK_GndviPolygons"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "MndwiPolygons");

            migrationBuilder.Sql(@"
                CREATE TABLE ""MndwiPolygons"" (
                    ""Id"" uuid NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Polygon"" geometry NOT NULL,
                    ""FileStorageItemId"" uuid NOT NULL,
                    ""IsColored"" boolean NOT NULL,
                    CONSTRAINT ""PK_MndwiPolygons"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "NdmiPolygons");

            migrationBuilder.Sql(@"
                CREATE TABLE ""NdmiPolygons"" (
                    ""Id"" uuid NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Polygon"" geometry NOT NULL,
                    ""FileStorageItemId"" uuid NOT NULL,
                    ""IsColored"" boolean NOT NULL,
                    CONSTRAINT ""PK_NdmiPolygons"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "NdviPolygons");

            migrationBuilder.Sql(@"
                CREATE TABLE ""NdviPolygons"" (
                    ""Id"" uuid NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Polygon"" geometry NOT NULL,
                    ""FileStorageItemId"" uuid NOT NULL,
                    ""IsColored"" boolean NOT NULL,
                    CONSTRAINT ""PK_NdviPolygons"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "OrviPolygons");

            migrationBuilder.Sql(@"
                CREATE TABLE ""OrviPolygons"" (
                    ""Id"" uuid NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Polygon"" geometry NOT NULL,
                    ""FileStorageItemId"" uuid NOT NULL,
                    ""IsColored"" boolean NOT NULL,
                    CONSTRAINT ""PK_OrviPolygons"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.DropTable(name: "OsaviPolygons");

            migrationBuilder.Sql(@"
                CREATE TABLE ""OsaviPolygons"" (
                    ""Id"" uuid NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Polygon"" geometry NOT NULL,
                    ""FileStorageItemId"" uuid NOT NULL,
                    ""IsColored"" boolean NOT NULL,
                    CONSTRAINT ""PK_OsaviPolygons"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ArviPoints");
            
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

            migrationBuilder.DropTable(name: "ArviPolygons");

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

            migrationBuilder.DropTable(name: "EviPoints");

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

            migrationBuilder.DropTable(name: "EviPolygons");

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

            migrationBuilder.DropTable(name: "GndviPoints");

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

            migrationBuilder.DropTable(name: "GndviPolygons");

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

            migrationBuilder.DropTable(name: "MndwiPoints");

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

            migrationBuilder.DropTable(name: "MndwiPolygons");

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

            migrationBuilder.DropTable(name: "NdmiPoints");

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

            migrationBuilder.DropTable(name: "NdmiPolygons");

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

            migrationBuilder.DropTable(name: "NdviPoints");

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

            migrationBuilder.DropTable(name: "NdviPolygons");

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

            migrationBuilder.DropTable(name: "OrviPoints");

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

            migrationBuilder.DropTable(name: "OrviPolygons");

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

            migrationBuilder.DropTable(name: "OsaviPoints");

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

            migrationBuilder.DropTable(name: "OsaviPolygons");

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
    }
}
