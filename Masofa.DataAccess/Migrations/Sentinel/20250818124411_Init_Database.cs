using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Sentinel
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Sentinel2ProductsMetadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ContentType = table.Column<string>(type: "text", nullable: true),
                    ContentLength = table.Column<long>(type: "bigint", nullable: true),
                    OriginDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Online = table.Column<bool>(type: "boolean", nullable: false),
                    EvictionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    S3Path = table.Column<string>(type: "text", nullable: true),
                    Footprint = table.Column<string>(type: "text", nullable: true),
                    ChecksumMd5 = table.Column<string>(type: "text", nullable: true),
                    ContentDateStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ContentDateEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductId = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sentinel2ProductsMetadata", x => x.Id);
                });

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"Sentinel2Products\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"SatellateProductId\" text NOT NULL, " +
                "\"SentinelInspireMetadataId\" uuid, " +
                "\"SentinelL1CProductMetadataId\" uuid, " +
                "\"SentinelL1CTileMetadataId\" uuid, " +
                "\"SentinelProductQualityMetadataId\" uuid, " +
                "\"CreateAt\" timestamp with time zone NOT NULL, " +
                "\"Status\" integer NOT NULL, " +
                "\"LastUpdateAt\" timestamp with time zone NOT NULL, " +
                "\"CreateUser\" uuid NOT NULL, " +
                "\"LastUpdateUser\" uuid NOT NULL, " +
                "CONSTRAINT \"PK_Sentinel2Products\" PRIMARY KEY (\"Id\", \"CreateAt\")" +
                ") PARTITION BY RANGE (\"CreateAt\");");

            migrationBuilder.CreateTable(
                name: "Sentinel2ProductsQueue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sentinel2ProductsQueue", x => x.Id);
                });

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"SentinelInspireMetadata\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"FileIdentifier\" text NOT NULL, " +
                "\"LanguageCode\" text NOT NULL, " +
                "\"CharacterSetCode\" text NOT NULL, " +
                "\"HierarchyLevelCode\" text NOT NULL, " +
                "\"DateStamp\" timestamp with time zone NOT NULL, " +
                "\"MetadataStandardName\" text NOT NULL, " +
                "\"MetadataStandardVersion\" text NOT NULL, " +
                "\"OrganisationName\" text NOT NULL, " +
                "\"Email\" text NOT NULL, " +
                "\"RoleCode\" text NOT NULL, " +
                "\"WestBoundLongitude\" numeric NOT NULL, " +
                "\"EastBoundLongitude\" numeric NOT NULL, " +
                "\"SouthBoundLatitude\" numeric NOT NULL, " +
                "\"NorthBoundLatitude\" numeric NOT NULL, " +
                "\"ReferenceSystemCode\" text NOT NULL, " +
                "\"ReferenceSystemCodeSpace\" text NOT NULL, " +
                "\"CreateAt\" timestamp with time zone NOT NULL, " +
                "\"Status\" integer NOT NULL, " +
                "\"LastUpdateAt\" timestamp with time zone NOT NULL, " +
                "\"CreateUser\" uuid NOT NULL, " +
                "\"LastUpdateUser\" uuid NOT NULL, " +
                "CONSTRAINT \"PK_SentinelInspireMetadata\" PRIMARY KEY (\"Id\", \"CreateAt\")" +
                ") PARTITION BY RANGE (\"CreateAt\");");

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"SentinelL1CProductMetadata\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"ProductStartTime\" timestamp with time zone NOT NULL, " +
                "\"ProductStopTime\" timestamp with time zone NOT NULL, " +
                "\"ProductUri\" text, " +
                "\"ProcessingLevel\" text, " +
                "\"ProductType\" text, " +
                "\"ProcessingBaseline\" text, " +
                "\"ProductDoi\" text, " +
                "\"GenerationTime\" timestamp with time zone NOT NULL, " +
                "\"DatatakeRaw\" text NOT NULL, " +
                "\"GranulesRaw\" text NOT NULL, " +
                "\"SpecialValuesRaw\" text NOT NULL, " +
                "\"RedChannel\" integer NOT NULL, " +
                "\"GreenChannel\" integer NOT NULL, " +
                "\"BlueChannel\" integer NOT NULL, " +
                "\"QuantificationValue\" integer NOT NULL, " +
                "\"OffsetsRaw\" text NOT NULL, " +
                "\"ReflectanceU\" double precision NOT NULL, " +
                "\"SolarIrradianceListRaw\" text NOT NULL, " +
                "\"SpectralBandIds\" text, " +
                "\"SpectralPhysicalBands\" text, " +
                "\"SpectralResolutions\" text, " +
                "\"SpectralWavelengthMins\" text, " +
                "\"SpectralWavelengthMaxs\" text, " +
                "\"SpectralWavelengthCentrals\" text, " +
                "\"SpectralInformationListRaw\" text NOT NULL, " +
                "\"ReferenceBand\" integer NOT NULL, " +
                "\"ExtPosList\" text, " +
                "\"RasterCsType\" text, " +
                "\"PixelOrigin\" integer NOT NULL, " +
                "\"GeoTables\" text, " +
                "\"HorizontalCsType\" text, " +
                "\"GippFilesRaw\" text NOT NULL, " +
                "\"CreateAt\" timestamp with time zone NOT NULL, " +
                "\"Status\" integer NOT NULL, " +
                "\"LastUpdateAt\" timestamp with time zone NOT NULL, " +
                "\"CreateUser\" uuid NOT NULL, " +
                "\"LastUpdateUser\" uuid NOT NULL, " +
                "CONSTRAINT \"PK_SentinelL1CProductMetadata\" PRIMARY KEY (\"Id\", \"CreateAt\")" +
                ") PARTITION BY RANGE (\"CreateAt\");");

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"SentinelL1CTileMetadata\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"TileId\" text NOT NULL, " +
                "\"DataStripId\" text NOT NULL, " +
                "\"DownlinkPriority\" text NOT NULL, " +
                "\"SensingTime\" timestamp with time zone NOT NULL, " +
                "\"ArchivingCentre\" text NOT NULL, " +
                "\"ArchivingTime\" timestamp with time zone NOT NULL, " +
                "\"HorizontalCSName\" text NOT NULL, " +
                "\"HorizontalCSCode\" text NOT NULL, " +
                "\"SizesRaw\" text NOT NULL, " +
                "\"GeoPositionsRaw\" text NOT NULL, " +
                "\"MeanSunAngleZenithAngle\" text NOT NULL, " +
                "\"SunAnglesZenithColStepValue\" double precision NOT NULL, " +
                "\"MeanSunAngleAzimuthAngle\" text NOT NULL, " +
                "\"SunAnglesZenithRowStepValue\" double precision NOT NULL, " +
                "\"SunAnglesZenithValuesList\" text[] NOT NULL, " +
                "\"SunAnglesAzimuthColStepUnit\" text NOT NULL, " +
                "\"SunAnglesAzimuthColStepValue\" double precision NOT NULL, " +
                "\"SunAnglesAzimuthRowStepUnit\" text NOT NULL, " +
                "\"SunAnglesAzimuthRowStepValue\" double precision NOT NULL, " +
                "\"SunAnglesAzimuthValuesList\" text[] NOT NULL, " +
                "\"MeanSunAngleZenithColStepUnit\" text NOT NULL, " +
                "\"MeanSunAngleZenithColStepValue\" double precision NOT NULL, " +
                "\"MeanSunAngleZenithRowStepUnit\" text NOT NULL, " +
                "\"MeanSunAngleZenithRowStepValue\" double precision NOT NULL, " +
                "\"MeanSunAngleZenithValuesList\" text[] NOT NULL, " +
                "\"MeanSunAngleAzimuthColStepUnit\" text NOT NULL, " +
                "\"MeanSunAngleAzimuthColStepValue\" double precision NOT NULL, " +
                "\"MeanSunAngleAzimuthRowStepUnit\" text NOT NULL, " +
                "\"MeanSunAngleAzimuthRowStepValue\" double precision NOT NULL, " +
                "\"MeanSunAngleAzimuthValuesList\" text[] NOT NULL, " +
                "\"ViewingIncidenceAnglesGridsRaw\" text NOT NULL, " +
                "\"MeanViewingIncidenceAngleListRaw\" text NOT NULL, " +
                "\"CloudyPixelPercentage\" double precision NOT NULL, " +
                "\"DegradedDataPercentage\" double precision NOT NULL, " +
                "\"SnowPixelPercentage\" double precision NOT NULL, " +
                "\"PixelLevelQiRaw\" text NOT NULL, " +
                "\"CreateAt\" timestamp with time zone NOT NULL, " +
                "\"Status\" integer NOT NULL, " +
                "\"LastUpdateAt\" timestamp with time zone NOT NULL, " +
                "\"CreateUser\" uuid NOT NULL, " +
                "\"LastUpdateUser\" uuid NOT NULL, " +
                "CONSTRAINT \"PK_SentinelL1CTileMetadata\" PRIMARY KEY (\"Id\", \"CreateAt\")" +
                ") PARTITION BY RANGE (\"CreateAt\");");

            migrationBuilder.Sql("CREATE TABLE IF NOT EXISTS \"SentinelProductQualityMetadata\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"FileName\" text NOT NULL, " +
                "\"FileDescription\" text NOT NULL, " +
                "\"Notes\" text NOT NULL, " +
                "\"Mission\" text NOT NULL, " +
                "\"FileClass\" text NOT NULL, " +
                "\"FileType\" text NOT NULL, " +
                "\"ValidityStart\" text NOT NULL, " +
                "\"ValidityStop\" text NOT NULL, " +
                "\"FileVersion\" text NOT NULL, " +
                "\"System\" text NOT NULL, " +
                "\"Creator\" text NOT NULL, " +
                "\"CreatorVersion\" text NOT NULL, " +
                "\"CreationDate\" text NOT NULL, " +
                "\"Type\" text NOT NULL, " +
                "\"GippVersion\" text NOT NULL, " +
                "\"GlobalStatus\" text NOT NULL, " +
                "\"Date\" timestamp with time zone NOT NULL, " +
                "\"ParentId\" text NOT NULL, " +
                "\"Name\" text NOT NULL, " +
                "\"Version\" text NOT NULL, " +
                "\"ChecksRaw\" text NOT NULL, " +
                "\"CreateAt\" timestamp with time zone NOT NULL, " +
                "\"Status\" integer NOT NULL, " +
                "\"LastUpdateAt\" timestamp with time zone NOT NULL, " +
                "\"CreateUser\" uuid NOT NULL, " +
                "\"LastUpdateUser\" uuid NOT NULL, " +
                "CONSTRAINT \"PK_SentinelProductQualityMetadata\" PRIMARY KEY (\"Id\", \"CreateAt\")" +
                ") PARTITION BY RANGE (\"CreateAt\");");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Sentinel2ProductsMetadata");
            migrationBuilder.DropTable(name: "Sentinel2Products");
            migrationBuilder.DropTable(name: "Sentinel2ProductsQueue");
            migrationBuilder.DropTable(name: "SentinelInspireMetadata");
            migrationBuilder.DropTable(name: "SentinelL1CProductMetadata");
            migrationBuilder.DropTable(name: "SentinelL1CTileMetadata");
            migrationBuilder.DropTable(name: "PK_SentinelProductQualityMetadata");
        }
    }
}