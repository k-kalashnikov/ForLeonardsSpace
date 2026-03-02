using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masofa.DataAccess.Migrations.Landsat
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
                name: "ImageAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpacecraftId = table.Column<string>(type: "text", nullable: false),
                    SensorId = table.Column<string>(type: "text", nullable: false),
                    WrsType = table.Column<string>(type: "text", nullable: false),
                    WrsPath = table.Column<string>(type: "text", nullable: false),
                    WrsRow = table.Column<string>(type: "text", nullable: false),
                    NadirOffnadir = table.Column<string>(type: "text", nullable: false),
                    TargetWrsPath = table.Column<string>(type: "text", nullable: false),
                    TargetWrsRow = table.Column<string>(type: "text", nullable: false),
                    DateAcquired = table.Column<string>(type: "text", nullable: false),
                    SceneCenterTime = table.Column<string>(type: "text", nullable: false),
                    StationId = table.Column<string>(type: "text", nullable: false),
                    CloudCover = table.Column<string>(type: "text", nullable: false),
                    CloudCoverLand = table.Column<string>(type: "text", nullable: false),
                    ImageQualityOli = table.Column<string>(type: "text", nullable: false),
                    ImageQualityTirs = table.Column<string>(type: "text", nullable: false),
                    SaturationBand1 = table.Column<string>(type: "text", nullable: false),
                    SaturationBand2 = table.Column<string>(type: "text", nullable: false),
                    SaturationBand3 = table.Column<string>(type: "text", nullable: false),
                    SaturationBand4 = table.Column<string>(type: "text", nullable: false),
                    SaturationBand5 = table.Column<string>(type: "text", nullable: false),
                    SaturationBand6 = table.Column<string>(type: "text", nullable: false),
                    SaturationBand7 = table.Column<string>(type: "text", nullable: false),
                    SaturationBand8 = table.Column<string>(type: "text", nullable: false),
                    SaturationBand9 = table.Column<string>(type: "text", nullable: false),
                    RollAngle = table.Column<string>(type: "text", nullable: false),
                    SunAzimuth = table.Column<string>(type: "text", nullable: false),
                    SunElevation = table.Column<string>(type: "text", nullable: false),
                    EarthSunDistance = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageAttributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LandsatProductMetadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<string>(type: "text", nullable: false),
                    SpacecraftId = table.Column<string>(type: "text", nullable: true),
                    SensorId = table.Column<string>(type: "text", nullable: true),
                    ProcessingLevel = table.Column<string>(type: "text", nullable: true),
                    AcquisitionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SceneCenterTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: true),
                    Row = table.Column<string>(type: "text", nullable: true),
                    CloudCover = table.Column<double>(type: "double precision", nullable: true),
                    DataType = table.Column<string>(type: "text", nullable: true),
                    CollectionCategory = table.Column<string>(type: "text", nullable: true),
                    WrsPathRow = table.Column<string>(type: "text", nullable: true),
                    MetadataFile = table.Column<string>(type: "text", nullable: true),
                    ProductRefId = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandsatProductMetadata", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LandsatProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SatellateProductId = table.Column<string>(type: "text", nullable: false),
                    LandsatSourceMetadataId = table.Column<Guid>(type: "uuid", nullable: true),
                    LandsatSrStacMetadataId = table.Column<Guid>(type: "uuid", nullable: true),
                    LandsatStStacMetadataId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandsatProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LandsatProductsQueue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<string>(type: "text", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    QueueStatus = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandsatProductsQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LandsatSourceMetadatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SatellateProductId = table.Column<string>(type: "text", nullable: false),
                    ProductContentsId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImageAttributesId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProjectionAttributesId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level2ProcessingRecordId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level2SurfaceReflectanceParametersId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level2SurfaceTemperatureParametersId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level1ProcessingRecordId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level1MinMaxRadianceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level1MinMaxReflectanceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level1MinMaxPixelValueId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level1RadiometricRescalingId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level1ThermalConstantsId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level1ProjectionParametersId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandsatSourceMetadatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LandsatSrStacMetadatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SatellateProductId = table.Column<string>(type: "text", nullable: false),
                    StacFeatureId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandsatSrStacMetadatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LandsatStStacMetadatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SatellateProductId = table.Column<string>(type: "text", nullable: false),
                    StacFeatureId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandsatStStacMetadatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Level1MinMaxPixelValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantizeCalMaxBand1 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand1 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand2 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand2 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand3 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand3 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand4 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand4 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand5 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand5 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand6 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand6 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand7 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand7 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand8 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand8 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand9 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand9 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand10 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand10 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand11 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand11 = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level1MinMaxPixelValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Level1MinMaxRadiances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RadianceMaximumBand1 = table.Column<string>(type: "text", nullable: false),
                    RadianceMinimumBand1 = table.Column<string>(type: "text", nullable: false),
                    RadianceMaximumBand2 = table.Column<string>(type: "text", nullable: false),
                    RadianceMinimumBand2 = table.Column<string>(type: "text", nullable: false),
                    RadianceMaximumBand3 = table.Column<string>(type: "text", nullable: false),
                    RadianceMinimumBand3 = table.Column<string>(type: "text", nullable: false),
                    RadianceMaximumBand4 = table.Column<string>(type: "text", nullable: false),
                    RadianceMinimumBand4 = table.Column<string>(type: "text", nullable: false),
                    RadianceMaximumBand5 = table.Column<string>(type: "text", nullable: false),
                    RadianceMinimumBand5 = table.Column<string>(type: "text", nullable: false),
                    RadianceMaximumBand6 = table.Column<string>(type: "text", nullable: false),
                    RadianceMinimumBand6 = table.Column<string>(type: "text", nullable: false),
                    RadianceMaximumBand7 = table.Column<string>(type: "text", nullable: false),
                    RadianceMinimumBand7 = table.Column<string>(type: "text", nullable: false),
                    RadianceMaximumBand8 = table.Column<string>(type: "text", nullable: false),
                    RadianceMinimumBand8 = table.Column<string>(type: "text", nullable: false),
                    RadianceMaximumBand9 = table.Column<string>(type: "text", nullable: false),
                    RadianceMinimumBand9 = table.Column<string>(type: "text", nullable: false),
                    RadianceMaximumBand10 = table.Column<string>(type: "text", nullable: false),
                    RadianceMinimumBand10 = table.Column<string>(type: "text", nullable: false),
                    RadianceMaximumBand11 = table.Column<string>(type: "text", nullable: false),
                    RadianceMinimumBand11 = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level1MinMaxRadiances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Level1MinMaxReflectances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReflectanceMaximumBand1 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand1 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand2 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand2 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand3 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand3 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand4 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand4 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand5 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand5 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand6 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand6 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand7 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand7 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand8 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand8 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand9 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand9 = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level1MinMaxReflectances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Level1ProcessingRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Origin = table.Column<string>(type: "text", nullable: false),
                    DigitalObjectIdentifier = table.Column<string>(type: "text", nullable: false),
                    RequestId = table.Column<string>(type: "text", nullable: false),
                    LandsatSceneId = table.Column<string>(type: "text", nullable: false),
                    LandsatProductId = table.Column<string>(type: "text", nullable: false),
                    ProcessingLevel = table.Column<string>(type: "text", nullable: false),
                    CollectionCategory = table.Column<string>(type: "text", nullable: false),
                    OutputFormat = table.Column<string>(type: "text", nullable: false),
                    DateProductGenerated = table.Column<string>(type: "text", nullable: false),
                    ProcessingSoftwareVersion = table.Column<string>(type: "text", nullable: false),
                    FileNameBand1 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand2 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand3 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand4 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand5 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand6 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand7 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand8 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand9 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand10 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand11 = table.Column<string>(type: "text", nullable: false),
                    FileNameQualityL1Pixel = table.Column<string>(type: "text", nullable: false),
                    FileNameQualityL1RadiometricSaturation = table.Column<string>(type: "text", nullable: false),
                    FileNameAngleCoefficient = table.Column<string>(type: "text", nullable: false),
                    FileNameAngleSensorAzimuthBand4 = table.Column<string>(type: "text", nullable: false),
                    FileNameAngleSensorZenithBand4 = table.Column<string>(type: "text", nullable: false),
                    FileNameAngleSolarAzimuthBand4 = table.Column<string>(type: "text", nullable: false),
                    FileNameAngleSolarZenithBand4 = table.Column<string>(type: "text", nullable: false),
                    FileNameMetadataOdl = table.Column<string>(type: "text", nullable: false),
                    FileNameMetadataXml = table.Column<string>(type: "text", nullable: false),
                    FileNameCpf = table.Column<string>(type: "text", nullable: false),
                    FileNameBpfOli = table.Column<string>(type: "text", nullable: false),
                    FileNameBpfTirs = table.Column<string>(type: "text", nullable: false),
                    FileNameRlut = table.Column<string>(type: "text", nullable: false),
                    DataSourceElevation = table.Column<string>(type: "text", nullable: false),
                    GroundControlPointsVersion = table.Column<string>(type: "text", nullable: false),
                    GroundControlPointsModel = table.Column<string>(type: "text", nullable: false),
                    GeometricRmseModel = table.Column<string>(type: "text", nullable: false),
                    GeometricRmseModelY = table.Column<string>(type: "text", nullable: false),
                    GeometricRmseModelX = table.Column<string>(type: "text", nullable: false),
                    GroundControlPointsVerify = table.Column<string>(type: "text", nullable: false),
                    GeometricRmseVerify = table.Column<string>(type: "text", nullable: false),
                    EphemerisType = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level1ProcessingRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Level1ProjectionParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MapProjection = table.Column<string>(type: "text", nullable: false),
                    Datum = table.Column<string>(type: "text", nullable: false),
                    Ellipsoid = table.Column<string>(type: "text", nullable: false),
                    UtmZone = table.Column<string>(type: "text", nullable: false),
                    GridCellSizePanchromatic = table.Column<string>(type: "text", nullable: false),
                    GridCellSizeReflective = table.Column<string>(type: "text", nullable: false),
                    GridCellSizeThermal = table.Column<string>(type: "text", nullable: false),
                    Orientation = table.Column<string>(type: "text", nullable: false),
                    ResamplingOption = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level1ProjectionParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Level1RadiometricRescalings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RadianceMultBand1 = table.Column<string>(type: "text", nullable: false),
                    RadianceMultBand2 = table.Column<string>(type: "text", nullable: false),
                    RadianceMultBand3 = table.Column<string>(type: "text", nullable: false),
                    RadianceMultBand4 = table.Column<string>(type: "text", nullable: false),
                    RadianceMultBand5 = table.Column<string>(type: "text", nullable: false),
                    RadianceMultBand6 = table.Column<string>(type: "text", nullable: false),
                    RadianceMultBand7 = table.Column<string>(type: "text", nullable: false),
                    RadianceMultBand8 = table.Column<string>(type: "text", nullable: false),
                    RadianceMultBand9 = table.Column<string>(type: "text", nullable: false),
                    RadianceMultBand10 = table.Column<string>(type: "text", nullable: false),
                    RadianceMultBand11 = table.Column<string>(type: "text", nullable: false),
                    RadianceAddBand1 = table.Column<string>(type: "text", nullable: false),
                    RadianceAddBand2 = table.Column<string>(type: "text", nullable: false),
                    RadianceAddBand3 = table.Column<string>(type: "text", nullable: false),
                    RadianceAddBand4 = table.Column<string>(type: "text", nullable: false),
                    RadianceAddBand5 = table.Column<string>(type: "text", nullable: false),
                    RadianceAddBand6 = table.Column<string>(type: "text", nullable: false),
                    RadianceAddBand7 = table.Column<string>(type: "text", nullable: false),
                    RadianceAddBand8 = table.Column<string>(type: "text", nullable: false),
                    RadianceAddBand9 = table.Column<string>(type: "text", nullable: false),
                    RadianceAddBand10 = table.Column<string>(type: "text", nullable: false),
                    RadianceAddBand11 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand1 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand2 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand3 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand4 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand5 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand6 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand7 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand8 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand9 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand1 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand2 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand3 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand4 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand5 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand6 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand7 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand8 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand9 = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level1RadiometricRescalings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Level1ThermalConstants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    K1ConstantBand10 = table.Column<string>(type: "text", nullable: false),
                    K2ConstantBand10 = table.Column<string>(type: "text", nullable: false),
                    K1ConstantBand11 = table.Column<string>(type: "text", nullable: false),
                    K2ConstantBand11 = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level1ThermalConstants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Level2ProcessingRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Origin = table.Column<string>(type: "text", nullable: false),
                    DigitalObjectIdentifier = table.Column<string>(type: "text", nullable: false),
                    RequestId = table.Column<string>(type: "text", nullable: false),
                    LandsatProductId = table.Column<string>(type: "text", nullable: false),
                    ProcessingLevel = table.Column<string>(type: "text", nullable: false),
                    OutputFormat = table.Column<string>(type: "text", nullable: false),
                    DateProductGenerated = table.Column<string>(type: "text", nullable: false),
                    ProcessingSoftwareVersion = table.Column<string>(type: "text", nullable: false),
                    AlgorithmSourceSurfaceReflectance = table.Column<string>(type: "text", nullable: false),
                    DataSourceOzone = table.Column<string>(type: "text", nullable: false),
                    DataSourcePressure = table.Column<string>(type: "text", nullable: false),
                    DataSourceWaterVapor = table.Column<string>(type: "text", nullable: false),
                    DataSourceAirTemperature = table.Column<string>(type: "text", nullable: false),
                    AlgorithmSourceSurfaceTemperature = table.Column<string>(type: "text", nullable: false),
                    DataSourceReanalysis = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level2ProcessingRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Level2SurfaceReflectanceParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReflectanceMaximumBand1 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand1 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand2 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand2 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand3 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand3 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand4 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand4 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand5 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand5 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand6 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand6 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMaximumBand7 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMinimumBand7 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand1 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand1 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand2 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand2 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand3 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand3 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand4 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand4 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand5 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand5 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand6 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand6 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaxBand7 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinBand7 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand1 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand2 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand3 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand4 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand5 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand6 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceMultBand7 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand1 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand2 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand3 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand4 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand5 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand6 = table.Column<string>(type: "text", nullable: false),
                    ReflectanceAddBand7 = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level2SurfaceReflectanceParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Level2SurfaceTemperatureParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemperatureMaximumBandStB10 = table.Column<string>(type: "text", nullable: false),
                    TemperatureMinimumBandStB10 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMaximumBandStB10 = table.Column<string>(type: "text", nullable: false),
                    QuantizeCalMinimumBandStB10 = table.Column<string>(type: "text", nullable: false),
                    TemperatureMultBandStB10 = table.Column<string>(type: "text", nullable: false),
                    TemperatureAddBandStB10 = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level2SurfaceTemperatureParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Origin = table.Column<string>(type: "text", nullable: false),
                    DigitalObjectIdentifier = table.Column<string>(type: "text", nullable: false),
                    LandsatProductId = table.Column<string>(type: "text", nullable: false),
                    ProcessingLevel = table.Column<string>(type: "text", nullable: false),
                    CollectionNumber = table.Column<string>(type: "text", nullable: false),
                    CollectionCategory = table.Column<string>(type: "text", nullable: false),
                    OutputFormat = table.Column<string>(type: "text", nullable: false),
                    FileNameBand1 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand2 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand3 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand4 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand5 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand6 = table.Column<string>(type: "text", nullable: false),
                    FileNameBand7 = table.Column<string>(type: "text", nullable: false),
                    FileNameBandStB10 = table.Column<string>(type: "text", nullable: false),
                    FileNameThermalRadiance = table.Column<string>(type: "text", nullable: false),
                    FileNameUpwellRadiance = table.Column<string>(type: "text", nullable: false),
                    FileNameDownwellRadiance = table.Column<string>(type: "text", nullable: false),
                    FileNameAtmosphericTransmittance = table.Column<string>(type: "text", nullable: false),
                    FileNameEmissivity = table.Column<string>(type: "text", nullable: false),
                    FileNameEmissivityStdev = table.Column<string>(type: "text", nullable: false),
                    FileNameCloudDistance = table.Column<string>(type: "text", nullable: false),
                    FileNameQualityL2Aerosol = table.Column<string>(type: "text", nullable: false),
                    FileNameQualityL2SurfaceTemperature = table.Column<string>(type: "text", nullable: false),
                    FileNameQualityL1Pixel = table.Column<string>(type: "text", nullable: false),
                    FileNameQualityL1RadiometricSaturation = table.Column<string>(type: "text", nullable: false),
                    FileNameAngleCoefficient = table.Column<string>(type: "text", nullable: false),
                    FileNameMetadataOdl = table.Column<string>(type: "text", nullable: false),
                    FileNameMetadataXml = table.Column<string>(type: "text", nullable: false),
                    DataTypeBand1 = table.Column<string>(type: "text", nullable: false),
                    DataTypeBand2 = table.Column<string>(type: "text", nullable: false),
                    DataTypeBand3 = table.Column<string>(type: "text", nullable: false),
                    DataTypeBand4 = table.Column<string>(type: "text", nullable: false),
                    DataTypeBand5 = table.Column<string>(type: "text", nullable: false),
                    DataTypeBand6 = table.Column<string>(type: "text", nullable: false),
                    DataTypeBand7 = table.Column<string>(type: "text", nullable: false),
                    DataTypeBandStB10 = table.Column<string>(type: "text", nullable: false),
                    DataTypeThermalRadiance = table.Column<string>(type: "text", nullable: false),
                    DataTypeUpwellRadiance = table.Column<string>(type: "text", nullable: false),
                    DataTypeDownwellRadiance = table.Column<string>(type: "text", nullable: false),
                    DataTypeAtmosphericTransmittance = table.Column<string>(type: "text", nullable: false),
                    DataTypeEmissivity = table.Column<string>(type: "text", nullable: false),
                    DataTypeEmissivityStdev = table.Column<string>(type: "text", nullable: false),
                    DataTypeCloudDistance = table.Column<string>(type: "text", nullable: false),
                    DataTypeQualityL2Aerosol = table.Column<string>(type: "text", nullable: false),
                    DataTypeQualityL2SurfaceTemperature = table.Column<string>(type: "text", nullable: false),
                    DataTypeQualityL1Pixel = table.Column<string>(type: "text", nullable: false),
                    DataTypeQualityL1RadiometricSaturation = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectionAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MapProjection = table.Column<string>(type: "text", nullable: false),
                    Datum = table.Column<string>(type: "text", nullable: false),
                    Ellipsoid = table.Column<string>(type: "text", nullable: false),
                    UtmZone = table.Column<string>(type: "text", nullable: false),
                    GridCellSizeReflective = table.Column<string>(type: "text", nullable: false),
                    GridCellSizeThermal = table.Column<string>(type: "text", nullable: false),
                    ReflectiveLines = table.Column<string>(type: "text", nullable: false),
                    ReflectiveSamples = table.Column<string>(type: "text", nullable: false),
                    ThermalLines = table.Column<string>(type: "text", nullable: false),
                    ThermalSamples = table.Column<string>(type: "text", nullable: false),
                    Orientation = table.Column<string>(type: "text", nullable: false),
                    CornerUlLatProduct = table.Column<string>(type: "text", nullable: false),
                    CornerUlLonProduct = table.Column<string>(type: "text", nullable: false),
                    CornerUrLatProduct = table.Column<string>(type: "text", nullable: false),
                    CornerUrLonProduct = table.Column<string>(type: "text", nullable: false),
                    CornerLlLatProduct = table.Column<string>(type: "text", nullable: false),
                    CornerLlLonProduct = table.Column<string>(type: "text", nullable: false),
                    CornerLrLatProduct = table.Column<string>(type: "text", nullable: false),
                    CornerLrLonProduct = table.Column<string>(type: "text", nullable: false),
                    CornerUlProjectionXProduct = table.Column<string>(type: "text", nullable: false),
                    CornerUlProjectionYProduct = table.Column<string>(type: "text", nullable: false),
                    CornerUrProjectionXProduct = table.Column<string>(type: "text", nullable: false),
                    CornerUrProjectionYProduct = table.Column<string>(type: "text", nullable: false),
                    CornerLlProjectionXProduct = table.Column<string>(type: "text", nullable: false),
                    CornerLlProjectionYProduct = table.Column<string>(type: "text", nullable: false),
                    CornerLrProjectionXProduct = table.Column<string>(type: "text", nullable: false),
                    CornerLrProjectionYProduct = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionAttributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SrBandAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BandNumber = table.Column<int>(type: "integer", nullable: false),
                    CommonName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CenterWavelength = table.Column<double>(type: "double precision", nullable: false),
                    SpatialResolution = table.Column<int>(type: "integer", nullable: false),
                    MinPixelValue = table.Column<int>(type: "integer", nullable: true),
                    MaxPixelValue = table.Column<int>(type: "integer", nullable: true),
                    ScaleFactor = table.Column<double>(type: "double precision", nullable: true),
                    Offset = table.Column<double>(type: "double precision", nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    StacAssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    StacFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalPath = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    ProcessingStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AdditionalMetadataJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SrBandAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SrQualityAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QualityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BitfieldsJson = table.Column<string>(type: "text", nullable: false),
                    SpatialResolution = table.Column<int>(type: "integer", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    StacAssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    StacFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalPath = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    ProcessingStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AdditionalMetadataJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SrQualityAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StacAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StacFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RolesJson = table.Column<string>(type: "text", nullable: false),
                    Href = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    AlternateJson = table.Column<string>(type: "text", nullable: true),
                    EoBandsJson = table.Column<string>(type: "text", nullable: true),
                    ClassificationBitfieldsJson = table.Column<string>(type: "text", nullable: true),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    Checksum = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ChecksumAlgorithm = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LocalPath = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DownloadStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StacAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StacFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StacVersion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StacExtensionsJson = table.Column<string>(type: "text", nullable: false),
                    FeatureId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    BoundingBoxJson = table.Column<string>(type: "text", nullable: false),
                    GeometryJson = table.Column<string>(type: "text", nullable: false),
                    Datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CloudCover = table.Column<double>(type: "double precision", nullable: false),
                    SunAzimuth = table.Column<double>(type: "double precision", nullable: false),
                    SunElevation = table.Column<double>(type: "double precision", nullable: false),
                    Platform = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InstrumentsJson = table.Column<string>(type: "text", nullable: false),
                    OffNadir = table.Column<int>(type: "integer", nullable: false),
                    CloudCoverLand = table.Column<double>(type: "double precision", nullable: false),
                    WrsType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    WrsPath = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    WrsRow = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SceneId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CollectionCategory = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CollectionNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Correction = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GeometricXBias = table.Column<int>(type: "integer", nullable: false),
                    GeometricYBias = table.Column<int>(type: "integer", nullable: false),
                    GeometricXStddev = table.Column<double>(type: "double precision", nullable: false),
                    GeometricYStddev = table.Column<double>(type: "double precision", nullable: false),
                    GeometricRmse = table.Column<double>(type: "double precision", nullable: false),
                    ProjEpsg = table.Column<int>(type: "integer", nullable: false),
                    ProjShapeJson = table.Column<string>(type: "text", nullable: false),
                    ProjTransformJson = table.Column<string>(type: "text", nullable: false),
                    Card4lSpecification = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Card4lSpecificationVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Collection = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ProductType = table.Column<int>(type: "integer", maxLength: 10, nullable: false),
                    MtlJsonAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    MtlTxtAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    MtlXmlAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    AngTxtAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    ThumbnailAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReducedResolutionBrowseAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    CoastalAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    BlueAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    GreenAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    RedAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    Nir08AssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    Swir16AssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    Swir22AssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    QaAerosolAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    Lwir11AssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    TradAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    UradAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    DradAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    AtranAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmisAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmsdAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    CdistAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    QaAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    QaPixelAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    QaRadsatAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StacFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StacLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StacFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Href = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StacLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StAuxiliaryAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuxiliaryType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SpatialResolution = table.Column<int>(type: "integer", nullable: false),
                    MinPixelValue = table.Column<int>(type: "integer", nullable: true),
                    MaxPixelValue = table.Column<int>(type: "integer", nullable: true),
                    ScaleFactor = table.Column<double>(type: "double precision", nullable: true),
                    Offset = table.Column<double>(type: "double precision", nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    StacAssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    StacFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalPath = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    ProcessingStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AdditionalMetadataJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StAuxiliaryAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StQualityAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QualityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BitfieldsJson = table.Column<string>(type: "text", nullable: false),
                    SpatialResolution = table.Column<int>(type: "integer", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    StacAssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    StacFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalPath = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    ProcessingStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AdditionalMetadataJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StQualityAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StTemperatureAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemperatureType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CenterWavelength = table.Column<double>(type: "double precision", nullable: false),
                    SpatialResolution = table.Column<int>(type: "integer", nullable: false),
                    MinPixelValue = table.Column<int>(type: "integer", nullable: true),
                    MaxPixelValue = table.Column<int>(type: "integer", nullable: true),
                    ScaleFactor = table.Column<double>(type: "double precision", nullable: true),
                    Offset = table.Column<double>(type: "double precision", nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateUser = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    StacAssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    StacFeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalPath = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    ProcessingStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AdditionalMetadataJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StTemperatureAssets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_landsat_product_metadata_acquisition_date",
                table: "LandsatProductMetadata",
                column: "AcquisitionDate");

            migrationBuilder.CreateIndex(
                name: "idx_landsat_product_metadata_product_id",
                table: "LandsatProductMetadata",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "idx_landsat_products_create_at",
                table: "LandsatProducts",
                column: "CreateAt");

            migrationBuilder.CreateIndex(
                name: "idx_landsat_products_satellite_id",
                table: "LandsatProducts",
                column: "SatellateProductId");

            migrationBuilder.CreateIndex(
                name: "idx_landsat_product_queue_product_id",
                table: "LandsatProductsQueue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "idx_landsat_product_queue_status",
                table: "LandsatProductsQueue",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "idx_stac_features_datetime",
                table: "StacFeatures",
                column: "Datetime");

            migrationBuilder.CreateIndex(
                name: "idx_stac_features_datetime_geometry",
                table: "StacFeatures",
                columns: new[] { "Datetime", "GeometryJson" },
                filter: "\"GeometryJson\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_stac_features_feature_id",
                table: "StacFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "idx_stac_features_geometry_json",
                table: "StacFeatures",
                column: "GeometryJson",
                filter: "\"GeometryJson\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageAttributes");

            migrationBuilder.DropTable(
                name: "LandsatProductMetadata");

            migrationBuilder.DropTable(
                name: "LandsatProducts");

            migrationBuilder.DropTable(
                name: "LandsatProductsQueue");

            migrationBuilder.DropTable(
                name: "LandsatSourceMetadatas");

            migrationBuilder.DropTable(
                name: "LandsatSrStacMetadatas");

            migrationBuilder.DropTable(
                name: "LandsatStStacMetadatas");

            migrationBuilder.DropTable(
                name: "Level1MinMaxPixelValues");

            migrationBuilder.DropTable(
                name: "Level1MinMaxRadiances");

            migrationBuilder.DropTable(
                name: "Level1MinMaxReflectances");

            migrationBuilder.DropTable(
                name: "Level1ProcessingRecords");

            migrationBuilder.DropTable(
                name: "Level1ProjectionParameters");

            migrationBuilder.DropTable(
                name: "Level1RadiometricRescalings");

            migrationBuilder.DropTable(
                name: "Level1ThermalConstants");

            migrationBuilder.DropTable(
                name: "Level2ProcessingRecords");

            migrationBuilder.DropTable(
                name: "Level2SurfaceReflectanceParameters");

            migrationBuilder.DropTable(
                name: "Level2SurfaceTemperatureParameters");

            migrationBuilder.DropTable(
                name: "ProductContents");

            migrationBuilder.DropTable(
                name: "ProjectionAttributes");

            migrationBuilder.DropTable(
                name: "SrBandAssets");

            migrationBuilder.DropTable(
                name: "SrQualityAssets");

            migrationBuilder.DropTable(
                name: "StacAssets");

            migrationBuilder.DropTable(
                name: "StacFeatures");

            migrationBuilder.DropTable(
                name: "StacLinks");

            migrationBuilder.DropTable(
                name: "StAuxiliaryAssets");

            migrationBuilder.DropTable(
                name: "StQualityAssets");

            migrationBuilder.DropTable(
                name: "StTemperatureAssets");
        }
    }
}
