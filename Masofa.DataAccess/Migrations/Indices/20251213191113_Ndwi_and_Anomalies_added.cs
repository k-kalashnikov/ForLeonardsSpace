using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Indices
{
    /// <inheritdoc />
    public partial class Ndwi_and_Anomalies_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""AnomalyPoints"" (
                    ""Id"" uuid NOT NULL,
                    ""AnomalyType"" integer,
                    ""Color"" text,
                    ""AnomalyPolygonId"" uuid,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Point"" geometry NOT NULL,
                    ""RegionId"" uuid,
                    ""FieldId"" uuid,
                    ""SeasonId"" uuid,
                    CONSTRAINT ""PK_AnomalyPoints"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""AnomalyPolygons"" (
                    ""Id"" uuid NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""AnomalyType"" integer NOT NULL,
                    ""Color"" text NOT NULL,
                    ""RegionId"" uuid,
                    ""FieldId"" uuid,
                    ""SeasonId"" uuid,
                    ""Polygon"" geometry NOT NULL,
                    CONSTRAINT ""PK_AnomalyPolygons"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""NdwiPoints"" (
                    ""Id"" uuid NOT NULL,
                    ""BZero3"" real NOT NULL,
                    ""BZero8"" real NOT NULL,
                    ""EPS"" real NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Point"" geometry NOT NULL,
                    ""RegionId"" uuid,
                    ""FieldId"" uuid,
                    ""SeasonId"" uuid,
                    CONSTRAINT ""PK_NdwiPoints"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.CreateTable(
                name: "NdwiPolygonRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NdwiId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdwiPolygonRelations", x => x.Id);
                });

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""NdwiPolygons"" (
                    ""Id"" uuid NOT NULL,
                    ""CreateAt"" timestamp with time zone NOT NULL,
                    ""ProductSourceType"" integer NOT NULL,
                    ""SatelliteProductId"" uuid NOT NULL,
                    ""Polygon"" geometry NOT NULL,
                    ""FileStorageItemId"" uuid NOT NULL,
                    ""IsColored"" boolean NOT NULL,
                    ""PreviewImagePath"" uuid,
                    CONSTRAINT ""PK_NdwiPolygons"" PRIMARY KEY (""Id"", ""CreateAt"")
                ) PARTITION BY RANGE (""CreateAt"");
            ");

            migrationBuilder.CreateTable(
                name: "NdwiSeasonReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdwiSeasonReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NdwiSharedReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOnly = table.Column<DateOnly>(type: "date", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false),
                    TotalMax = table.Column<double>(type: "double precision", nullable: false),
                    TotalMin = table.Column<double>(type: "double precision", nullable: false),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CropId = table.Column<Guid>(type: "uuid", nullable: true),
                    AverageMax = table.Column<double>(type: "double precision", nullable: false),
                    AverageMin = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NdwiSharedReports", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnomalyPoints");

            migrationBuilder.DropTable(
                name: "AnomalyPolygons");

            migrationBuilder.DropTable(
                name: "NdwiPoints");

            migrationBuilder.DropTable(
                name: "NdwiPolygonRelations");

            migrationBuilder.DropTable(
                name: "NdwiPolygons");

            migrationBuilder.DropTable(
                name: "NdwiSeasonReports");

            migrationBuilder.DropTable(
                name: "NdwiSharedReports");
        }
    }
}
