CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "ImageAttributes" (
        "Id" uuid NOT NULL,
        "SpacecraftId" text NOT NULL,
        "SensorId" text NOT NULL,
        "WrsType" text NOT NULL,
        "WrsPath" text NOT NULL,
        "WrsRow" text NOT NULL,
        "NadirOffnadir" text NOT NULL,
        "TargetWrsPath" text NOT NULL,
        "TargetWrsRow" text NOT NULL,
        "DateAcquired" text NOT NULL,
        "SceneCenterTime" text NOT NULL,
        "StationId" text NOT NULL,
        "CloudCover" text NOT NULL,
        "CloudCoverLand" text NOT NULL,
        "ImageQualityOli" text NOT NULL,
        "ImageQualityTirs" text NOT NULL,
        "SaturationBand1" text NOT NULL,
        "SaturationBand2" text NOT NULL,
        "SaturationBand3" text NOT NULL,
        "SaturationBand4" text NOT NULL,
        "SaturationBand5" text NOT NULL,
        "SaturationBand6" text NOT NULL,
        "SaturationBand7" text NOT NULL,
        "SaturationBand8" text NOT NULL,
        "SaturationBand9" text NOT NULL,
        "RollAngle" text NOT NULL,
        "SunAzimuth" text NOT NULL,
        "SunElevation" text NOT NULL,
        "EarthSunDistance" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_ImageAttributes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "LandsatProductMetadata" (
        "Id" uuid NOT NULL,
        "ProductId" text NOT NULL,
        "SpacecraftId" text,
        "SensorId" text,
        "ProcessingLevel" text,
        "AcquisitionDate" timestamp with time zone,
        "SceneCenterTime" timestamp with time zone,
        "Path" text,
        "Row" text,
        "CloudCover" double precision,
        "DataType" text,
        "CollectionCategory" text,
        "WrsPathRow" text,
        "MetadataFile" text,
        "ProductRefId" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_LandsatProductMetadata" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "LandsatProducts" (
        "Id" uuid NOT NULL,
        "SatellateProductId" text NOT NULL,
        "LandsatSourceMetadataId" uuid,
        "LandsatSrStacMetadataId" uuid,
        "LandsatStStacMetadataId" uuid,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_LandsatProducts" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "LandsatProductsQueue" (
        "Id" uuid NOT NULL,
        "ProductId" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "QueueStatus" integer NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_LandsatProductsQueue" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "LandsatSourceMetadatas" (
        "Id" uuid NOT NULL,
        "SatellateProductId" text NOT NULL,
        "ProductContentsId" uuid,
        "ImageAttributesId" uuid,
        "ProjectionAttributesId" uuid,
        "Level2ProcessingRecordId" uuid,
        "Level2SurfaceReflectanceParametersId" uuid,
        "Level2SurfaceTemperatureParametersId" uuid,
        "Level1ProcessingRecordId" uuid,
        "Level1MinMaxRadianceId" uuid,
        "Level1MinMaxReflectanceId" uuid,
        "Level1MinMaxPixelValueId" uuid,
        "Level1RadiometricRescalingId" uuid,
        "Level1ThermalConstantsId" uuid,
        "Level1ProjectionParametersId" uuid,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_LandsatSourceMetadatas" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "LandsatSrStacMetadatas" (
        "Id" uuid NOT NULL,
        "SatellateProductId" text NOT NULL,
        "StacFeatureId" uuid,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_LandsatSrStacMetadatas" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "LandsatStStacMetadatas" (
        "Id" uuid NOT NULL,
        "SatellateProductId" text NOT NULL,
        "StacFeatureId" uuid,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_LandsatStStacMetadatas" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "Level1MinMaxPixelValues" (
        "Id" uuid NOT NULL,
        "QuantizeCalMaxBand1" text NOT NULL,
        "QuantizeCalMinBand1" text NOT NULL,
        "QuantizeCalMaxBand2" text NOT NULL,
        "QuantizeCalMinBand2" text NOT NULL,
        "QuantizeCalMaxBand3" text NOT NULL,
        "QuantizeCalMinBand3" text NOT NULL,
        "QuantizeCalMaxBand4" text NOT NULL,
        "QuantizeCalMinBand4" text NOT NULL,
        "QuantizeCalMaxBand5" text NOT NULL,
        "QuantizeCalMinBand5" text NOT NULL,
        "QuantizeCalMaxBand6" text NOT NULL,
        "QuantizeCalMinBand6" text NOT NULL,
        "QuantizeCalMaxBand7" text NOT NULL,
        "QuantizeCalMinBand7" text NOT NULL,
        "QuantizeCalMaxBand8" text NOT NULL,
        "QuantizeCalMinBand8" text NOT NULL,
        "QuantizeCalMaxBand9" text NOT NULL,
        "QuantizeCalMinBand9" text NOT NULL,
        "QuantizeCalMaxBand10" text NOT NULL,
        "QuantizeCalMinBand10" text NOT NULL,
        "QuantizeCalMaxBand11" text NOT NULL,
        "QuantizeCalMinBand11" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_Level1MinMaxPixelValues" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "Level1MinMaxRadiances" (
        "Id" uuid NOT NULL,
        "RadianceMaximumBand1" text NOT NULL,
        "RadianceMinimumBand1" text NOT NULL,
        "RadianceMaximumBand2" text NOT NULL,
        "RadianceMinimumBand2" text NOT NULL,
        "RadianceMaximumBand3" text NOT NULL,
        "RadianceMinimumBand3" text NOT NULL,
        "RadianceMaximumBand4" text NOT NULL,
        "RadianceMinimumBand4" text NOT NULL,
        "RadianceMaximumBand5" text NOT NULL,
        "RadianceMinimumBand5" text NOT NULL,
        "RadianceMaximumBand6" text NOT NULL,
        "RadianceMinimumBand6" text NOT NULL,
        "RadianceMaximumBand7" text NOT NULL,
        "RadianceMinimumBand7" text NOT NULL,
        "RadianceMaximumBand8" text NOT NULL,
        "RadianceMinimumBand8" text NOT NULL,
        "RadianceMaximumBand9" text NOT NULL,
        "RadianceMinimumBand9" text NOT NULL,
        "RadianceMaximumBand10" text NOT NULL,
        "RadianceMinimumBand10" text NOT NULL,
        "RadianceMaximumBand11" text NOT NULL,
        "RadianceMinimumBand11" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_Level1MinMaxRadiances" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "Level1MinMaxReflectances" (
        "Id" uuid NOT NULL,
        "ReflectanceMaximumBand1" text NOT NULL,
        "ReflectanceMinimumBand1" text NOT NULL,
        "ReflectanceMaximumBand2" text NOT NULL,
        "ReflectanceMinimumBand2" text NOT NULL,
        "ReflectanceMaximumBand3" text NOT NULL,
        "ReflectanceMinimumBand3" text NOT NULL,
        "ReflectanceMaximumBand4" text NOT NULL,
        "ReflectanceMinimumBand4" text NOT NULL,
        "ReflectanceMaximumBand5" text NOT NULL,
        "ReflectanceMinimumBand5" text NOT NULL,
        "ReflectanceMaximumBand6" text NOT NULL,
        "ReflectanceMinimumBand6" text NOT NULL,
        "ReflectanceMaximumBand7" text NOT NULL,
        "ReflectanceMinimumBand7" text NOT NULL,
        "ReflectanceMaximumBand8" text NOT NULL,
        "ReflectanceMinimumBand8" text NOT NULL,
        "ReflectanceMaximumBand9" text NOT NULL,
        "ReflectanceMinimumBand9" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_Level1MinMaxReflectances" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "Level1ProcessingRecords" (
        "Id" uuid NOT NULL,
        "Origin" text NOT NULL,
        "DigitalObjectIdentifier" text NOT NULL,
        "RequestId" text NOT NULL,
        "LandsatSceneId" text NOT NULL,
        "LandsatProductId" text NOT NULL,
        "ProcessingLevel" text NOT NULL,
        "CollectionCategory" text NOT NULL,
        "OutputFormat" text NOT NULL,
        "DateProductGenerated" text NOT NULL,
        "ProcessingSoftwareVersion" text NOT NULL,
        "FileNameBand1" text NOT NULL,
        "FileNameBand2" text NOT NULL,
        "FileNameBand3" text NOT NULL,
        "FileNameBand4" text NOT NULL,
        "FileNameBand5" text NOT NULL,
        "FileNameBand6" text NOT NULL,
        "FileNameBand7" text NOT NULL,
        "FileNameBand8" text NOT NULL,
        "FileNameBand9" text NOT NULL,
        "FileNameBand10" text NOT NULL,
        "FileNameBand11" text NOT NULL,
        "FileNameQualityL1Pixel" text NOT NULL,
        "FileNameQualityL1RadiometricSaturation" text NOT NULL,
        "FileNameAngleCoefficient" text NOT NULL,
        "FileNameAngleSensorAzimuthBand4" text NOT NULL,
        "FileNameAngleSensorZenithBand4" text NOT NULL,
        "FileNameAngleSolarAzimuthBand4" text NOT NULL,
        "FileNameAngleSolarZenithBand4" text NOT NULL,
        "FileNameMetadataOdl" text NOT NULL,
        "FileNameMetadataXml" text NOT NULL,
        "FileNameCpf" text NOT NULL,
        "FileNameBpfOli" text NOT NULL,
        "FileNameBpfTirs" text NOT NULL,
        "FileNameRlut" text NOT NULL,
        "DataSourceElevation" text NOT NULL,
        "GroundControlPointsVersion" text NOT NULL,
        "GroundControlPointsModel" text NOT NULL,
        "GeometricRmseModel" text NOT NULL,
        "GeometricRmseModelY" text NOT NULL,
        "GeometricRmseModelX" text NOT NULL,
        "GroundControlPointsVerify" text NOT NULL,
        "GeometricRmseVerify" text NOT NULL,
        "EphemerisType" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_Level1ProcessingRecords" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "Level1ProjectionParameters" (
        "Id" uuid NOT NULL,
        "MapProjection" text NOT NULL,
        "Datum" text NOT NULL,
        "Ellipsoid" text NOT NULL,
        "UtmZone" text NOT NULL,
        "GridCellSizePanchromatic" text NOT NULL,
        "GridCellSizeReflective" text NOT NULL,
        "GridCellSizeThermal" text NOT NULL,
        "Orientation" text NOT NULL,
        "ResamplingOption" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_Level1ProjectionParameters" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "Level1RadiometricRescalings" (
        "Id" uuid NOT NULL,
        "RadianceMultBand1" text NOT NULL,
        "RadianceMultBand2" text NOT NULL,
        "RadianceMultBand3" text NOT NULL,
        "RadianceMultBand4" text NOT NULL,
        "RadianceMultBand5" text NOT NULL,
        "RadianceMultBand6" text NOT NULL,
        "RadianceMultBand7" text NOT NULL,
        "RadianceMultBand8" text NOT NULL,
        "RadianceMultBand9" text NOT NULL,
        "RadianceMultBand10" text NOT NULL,
        "RadianceMultBand11" text NOT NULL,
        "RadianceAddBand1" text NOT NULL,
        "RadianceAddBand2" text NOT NULL,
        "RadianceAddBand3" text NOT NULL,
        "RadianceAddBand4" text NOT NULL,
        "RadianceAddBand5" text NOT NULL,
        "RadianceAddBand6" text NOT NULL,
        "RadianceAddBand7" text NOT NULL,
        "RadianceAddBand8" text NOT NULL,
        "RadianceAddBand9" text NOT NULL,
        "RadianceAddBand10" text NOT NULL,
        "RadianceAddBand11" text NOT NULL,
        "ReflectanceMultBand1" text NOT NULL,
        "ReflectanceMultBand2" text NOT NULL,
        "ReflectanceMultBand3" text NOT NULL,
        "ReflectanceMultBand4" text NOT NULL,
        "ReflectanceMultBand5" text NOT NULL,
        "ReflectanceMultBand6" text NOT NULL,
        "ReflectanceMultBand7" text NOT NULL,
        "ReflectanceMultBand8" text NOT NULL,
        "ReflectanceMultBand9" text NOT NULL,
        "ReflectanceAddBand1" text NOT NULL,
        "ReflectanceAddBand2" text NOT NULL,
        "ReflectanceAddBand3" text NOT NULL,
        "ReflectanceAddBand4" text NOT NULL,
        "ReflectanceAddBand5" text NOT NULL,
        "ReflectanceAddBand6" text NOT NULL,
        "ReflectanceAddBand7" text NOT NULL,
        "ReflectanceAddBand8" text NOT NULL,
        "ReflectanceAddBand9" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_Level1RadiometricRescalings" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "Level1ThermalConstants" (
        "Id" uuid NOT NULL,
        "K1ConstantBand10" text NOT NULL,
        "K2ConstantBand10" text NOT NULL,
        "K1ConstantBand11" text NOT NULL,
        "K2ConstantBand11" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_Level1ThermalConstants" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "Level2ProcessingRecords" (
        "Id" uuid NOT NULL,
        "Origin" text NOT NULL,
        "DigitalObjectIdentifier" text NOT NULL,
        "RequestId" text NOT NULL,
        "LandsatProductId" text NOT NULL,
        "ProcessingLevel" text NOT NULL,
        "OutputFormat" text NOT NULL,
        "DateProductGenerated" text NOT NULL,
        "ProcessingSoftwareVersion" text NOT NULL,
        "AlgorithmSourceSurfaceReflectance" text NOT NULL,
        "DataSourceOzone" text NOT NULL,
        "DataSourcePressure" text NOT NULL,
        "DataSourceWaterVapor" text NOT NULL,
        "DataSourceAirTemperature" text NOT NULL,
        "AlgorithmSourceSurfaceTemperature" text NOT NULL,
        "DataSourceReanalysis" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_Level2ProcessingRecords" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "Level2SurfaceReflectanceParameters" (
        "Id" uuid NOT NULL,
        "ReflectanceMaximumBand1" text NOT NULL,
        "ReflectanceMinimumBand1" text NOT NULL,
        "ReflectanceMaximumBand2" text NOT NULL,
        "ReflectanceMinimumBand2" text NOT NULL,
        "ReflectanceMaximumBand3" text NOT NULL,
        "ReflectanceMinimumBand3" text NOT NULL,
        "ReflectanceMaximumBand4" text NOT NULL,
        "ReflectanceMinimumBand4" text NOT NULL,
        "ReflectanceMaximumBand5" text NOT NULL,
        "ReflectanceMinimumBand5" text NOT NULL,
        "ReflectanceMaximumBand6" text NOT NULL,
        "ReflectanceMinimumBand6" text NOT NULL,
        "ReflectanceMaximumBand7" text NOT NULL,
        "ReflectanceMinimumBand7" text NOT NULL,
        "QuantizeCalMaxBand1" text NOT NULL,
        "QuantizeCalMinBand1" text NOT NULL,
        "QuantizeCalMaxBand2" text NOT NULL,
        "QuantizeCalMinBand2" text NOT NULL,
        "QuantizeCalMaxBand3" text NOT NULL,
        "QuantizeCalMinBand3" text NOT NULL,
        "QuantizeCalMaxBand4" text NOT NULL,
        "QuantizeCalMinBand4" text NOT NULL,
        "QuantizeCalMaxBand5" text NOT NULL,
        "QuantizeCalMinBand5" text NOT NULL,
        "QuantizeCalMaxBand6" text NOT NULL,
        "QuantizeCalMinBand6" text NOT NULL,
        "QuantizeCalMaxBand7" text NOT NULL,
        "QuantizeCalMinBand7" text NOT NULL,
        "ReflectanceMultBand1" text NOT NULL,
        "ReflectanceMultBand2" text NOT NULL,
        "ReflectanceMultBand3" text NOT NULL,
        "ReflectanceMultBand4" text NOT NULL,
        "ReflectanceMultBand5" text NOT NULL,
        "ReflectanceMultBand6" text NOT NULL,
        "ReflectanceMultBand7" text NOT NULL,
        "ReflectanceAddBand1" text NOT NULL,
        "ReflectanceAddBand2" text NOT NULL,
        "ReflectanceAddBand3" text NOT NULL,
        "ReflectanceAddBand4" text NOT NULL,
        "ReflectanceAddBand5" text NOT NULL,
        "ReflectanceAddBand6" text NOT NULL,
        "ReflectanceAddBand7" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_Level2SurfaceReflectanceParameters" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "Level2SurfaceTemperatureParameters" (
        "Id" uuid NOT NULL,
        "TemperatureMaximumBandStB10" text NOT NULL,
        "TemperatureMinimumBandStB10" text NOT NULL,
        "QuantizeCalMaximumBandStB10" text NOT NULL,
        "QuantizeCalMinimumBandStB10" text NOT NULL,
        "TemperatureMultBandStB10" text NOT NULL,
        "TemperatureAddBandStB10" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_Level2SurfaceTemperatureParameters" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "ProductContents" (
        "Id" uuid NOT NULL,
        "Origin" text NOT NULL,
        "DigitalObjectIdentifier" text NOT NULL,
        "LandsatProductId" text NOT NULL,
        "ProcessingLevel" text NOT NULL,
        "CollectionNumber" text NOT NULL,
        "CollectionCategory" text NOT NULL,
        "OutputFormat" text NOT NULL,
        "FileNameBand1" text NOT NULL,
        "FileNameBand2" text NOT NULL,
        "FileNameBand3" text NOT NULL,
        "FileNameBand4" text NOT NULL,
        "FileNameBand5" text NOT NULL,
        "FileNameBand6" text NOT NULL,
        "FileNameBand7" text NOT NULL,
        "FileNameBandStB10" text NOT NULL,
        "FileNameThermalRadiance" text NOT NULL,
        "FileNameUpwellRadiance" text NOT NULL,
        "FileNameDownwellRadiance" text NOT NULL,
        "FileNameAtmosphericTransmittance" text NOT NULL,
        "FileNameEmissivity" text NOT NULL,
        "FileNameEmissivityStdev" text NOT NULL,
        "FileNameCloudDistance" text NOT NULL,
        "FileNameQualityL2Aerosol" text NOT NULL,
        "FileNameQualityL2SurfaceTemperature" text NOT NULL,
        "FileNameQualityL1Pixel" text NOT NULL,
        "FileNameQualityL1RadiometricSaturation" text NOT NULL,
        "FileNameAngleCoefficient" text NOT NULL,
        "FileNameMetadataOdl" text NOT NULL,
        "FileNameMetadataXml" text NOT NULL,
        "DataTypeBand1" text NOT NULL,
        "DataTypeBand2" text NOT NULL,
        "DataTypeBand3" text NOT NULL,
        "DataTypeBand4" text NOT NULL,
        "DataTypeBand5" text NOT NULL,
        "DataTypeBand6" text NOT NULL,
        "DataTypeBand7" text NOT NULL,
        "DataTypeBandStB10" text NOT NULL,
        "DataTypeThermalRadiance" text NOT NULL,
        "DataTypeUpwellRadiance" text NOT NULL,
        "DataTypeDownwellRadiance" text NOT NULL,
        "DataTypeAtmosphericTransmittance" text NOT NULL,
        "DataTypeEmissivity" text NOT NULL,
        "DataTypeEmissivityStdev" text NOT NULL,
        "DataTypeCloudDistance" text NOT NULL,
        "DataTypeQualityL2Aerosol" text NOT NULL,
        "DataTypeQualityL2SurfaceTemperature" text NOT NULL,
        "DataTypeQualityL1Pixel" text NOT NULL,
        "DataTypeQualityL1RadiometricSaturation" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_ProductContents" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "ProjectionAttributes" (
        "Id" uuid NOT NULL,
        "MapProjection" text NOT NULL,
        "Datum" text NOT NULL,
        "Ellipsoid" text NOT NULL,
        "UtmZone" text NOT NULL,
        "GridCellSizeReflective" text NOT NULL,
        "GridCellSizeThermal" text NOT NULL,
        "ReflectiveLines" text NOT NULL,
        "ReflectiveSamples" text NOT NULL,
        "ThermalLines" text NOT NULL,
        "ThermalSamples" text NOT NULL,
        "Orientation" text NOT NULL,
        "CornerUlLatProduct" text NOT NULL,
        "CornerUlLonProduct" text NOT NULL,
        "CornerUrLatProduct" text NOT NULL,
        "CornerUrLonProduct" text NOT NULL,
        "CornerLlLatProduct" text NOT NULL,
        "CornerLlLonProduct" text NOT NULL,
        "CornerLrLatProduct" text NOT NULL,
        "CornerLrLonProduct" text NOT NULL,
        "CornerUlProjectionXProduct" text NOT NULL,
        "CornerUlProjectionYProduct" text NOT NULL,
        "CornerUrProjectionXProduct" text NOT NULL,
        "CornerUrProjectionYProduct" text NOT NULL,
        "CornerLlProjectionXProduct" text NOT NULL,
        "CornerLlProjectionYProduct" text NOT NULL,
        "CornerLrProjectionXProduct" text NOT NULL,
        "CornerLrProjectionYProduct" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_ProjectionAttributes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "SrBandAssets" (
        "Id" uuid NOT NULL,
        "BandNumber" integer NOT NULL,
        "CommonName" character varying(50) NOT NULL,
        "CenterWavelength" double precision NOT NULL,
        "SpatialResolution" integer NOT NULL,
        "MinPixelValue" integer,
        "MaxPixelValue" integer,
        "ScaleFactor" double precision,
        "Offset" double precision,
        "Unit" character varying(50),
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "StacAssetId" uuid NOT NULL,
        "StacFeatureId" uuid NOT NULL,
        "LocalPath" character varying(2000),
        "SizeInBytes" bigint,
        "ProcessingStatus" character varying(50),
        "AdditionalMetadataJson" text,
        CONSTRAINT "PK_SrBandAssets" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "SrQualityAssets" (
        "Id" uuid NOT NULL,
        "QualityType" character varying(50) NOT NULL,
        "BitfieldsJson" text NOT NULL,
        "SpatialResolution" integer NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "StacAssetId" uuid NOT NULL,
        "StacFeatureId" uuid NOT NULL,
        "LocalPath" character varying(2000),
        "SizeInBytes" bigint,
        "ProcessingStatus" character varying(50),
        "AdditionalMetadataJson" text,
        CONSTRAINT "PK_SrQualityAssets" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "StacAssets" (
        "Id" uuid NOT NULL,
        "StacFeatureId" uuid NOT NULL,
        "AssetKey" character varying(100) NOT NULL,
        "Title" character varying(255) NOT NULL,
        "Description" character varying(1000),
        "Type" character varying(100) NOT NULL,
        "RolesJson" text NOT NULL,
        "Href" character varying(2000) NOT NULL,
        "AlternateJson" text,
        "EoBandsJson" text,
        "ClassificationBitfieldsJson" text,
        "SizeInBytes" bigint,
        "Checksum" character varying(255),
        "ChecksumAlgorithm" character varying(50),
        "LocalPath" character varying(2000),
        "DownloadStatus" character varying(50),
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_StacAssets" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "StacFeatures" (
        "Id" uuid NOT NULL,
        "Type" character varying(50) NOT NULL,
        "StacVersion" character varying(20) NOT NULL,
        "StacExtensionsJson" text NOT NULL,
        "FeatureId" character varying(255) NOT NULL,
        "Description" character varying(1000) NOT NULL,
        "BoundingBoxJson" text NOT NULL,
        "GeometryJson" text NOT NULL,
        "Datetime" timestamp with time zone NOT NULL,
        "CloudCover" double precision NOT NULL,
        "SunAzimuth" double precision NOT NULL,
        "SunElevation" double precision NOT NULL,
        "Platform" character varying(50) NOT NULL,
        "InstrumentsJson" text NOT NULL,
        "OffNadir" integer NOT NULL,
        "CloudCoverLand" double precision NOT NULL,
        "WrsType" character varying(10) NOT NULL,
        "WrsPath" character varying(10) NOT NULL,
        "WrsRow" character varying(10) NOT NULL,
        "SceneId" character varying(255) NOT NULL,
        "CollectionCategory" character varying(50) NOT NULL,
        "CollectionNumber" character varying(10) NOT NULL,
        "Correction" character varying(50) NOT NULL,
        "GeometricXBias" integer NOT NULL,
        "GeometricYBias" integer NOT NULL,
        "GeometricXStddev" double precision NOT NULL,
        "GeometricYStddev" double precision NOT NULL,
        "GeometricRmse" double precision NOT NULL,
        "ProjEpsg" integer NOT NULL,
        "ProjShapeJson" text NOT NULL,
        "ProjTransformJson" text NOT NULL,
        "Card4lSpecification" character varying(50) NOT NULL,
        "Card4lSpecificationVersion" character varying(50) NOT NULL,
        "Collection" character varying(255) NOT NULL,
        "ProductType" integer NOT NULL,
        "MtlJsonAssetId" uuid,
        "MtlTxtAssetId" uuid,
        "MtlXmlAssetId" uuid,
        "AngTxtAssetId" uuid,
        "ThumbnailAssetId" uuid,
        "ReducedResolutionBrowseAssetId" uuid,
        "CoastalAssetId" uuid,
        "BlueAssetId" uuid,
        "GreenAssetId" uuid,
        "RedAssetId" uuid,
        "Nir08AssetId" uuid,
        "Swir16AssetId" uuid,
        "Swir22AssetId" uuid,
        "QaAerosolAssetId" uuid,
        "Lwir11AssetId" uuid,
        "TradAssetId" uuid,
        "UradAssetId" uuid,
        "DradAssetId" uuid,
        "AtranAssetId" uuid,
        "EmisAssetId" uuid,
        "EmsdAssetId" uuid,
        "CdistAssetId" uuid,
        "QaAssetId" uuid,
        "QaPixelAssetId" uuid,
        "QaRadsatAssetId" uuid,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_StacFeatures" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "StacLinks" (
        "Id" uuid NOT NULL,
        "StacFeatureId" uuid NOT NULL,
        "Rel" character varying(50) NOT NULL,
        "Href" character varying(2000) NOT NULL,
        "Type" character varying(100),
        "Title" character varying(255),
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_StacLinks" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "StAuxiliaryAssets" (
        "Id" uuid NOT NULL,
        "AuxiliaryType" character varying(50) NOT NULL,
        "Description" character varying(1000) NOT NULL,
        "SpatialResolution" integer NOT NULL,
        "MinPixelValue" integer,
        "MaxPixelValue" integer,
        "ScaleFactor" double precision,
        "Offset" double precision,
        "Unit" character varying(50),
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "StacAssetId" uuid NOT NULL,
        "StacFeatureId" uuid NOT NULL,
        "LocalPath" character varying(2000),
        "SizeInBytes" bigint,
        "ProcessingStatus" character varying(50),
        "AdditionalMetadataJson" text,
        CONSTRAINT "PK_StAuxiliaryAssets" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "StQualityAssets" (
        "Id" uuid NOT NULL,
        "QualityType" character varying(50) NOT NULL,
        "BitfieldsJson" text NOT NULL,
        "SpatialResolution" integer NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "StacAssetId" uuid NOT NULL,
        "StacFeatureId" uuid NOT NULL,
        "LocalPath" character varying(2000),
        "SizeInBytes" bigint,
        "ProcessingStatus" character varying(50),
        "AdditionalMetadataJson" text,
        CONSTRAINT "PK_StQualityAssets" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE TABLE "StTemperatureAssets" (
        "Id" uuid NOT NULL,
        "TemperatureType" character varying(50) NOT NULL,
        "CenterWavelength" double precision NOT NULL,
        "SpatialResolution" integer NOT NULL,
        "MinPixelValue" integer,
        "MaxPixelValue" integer,
        "ScaleFactor" double precision,
        "Offset" double precision,
        "Unit" character varying(50),
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "StacAssetId" uuid NOT NULL,
        "StacFeatureId" uuid NOT NULL,
        "LocalPath" character varying(2000),
        "SizeInBytes" bigint,
        "ProcessingStatus" character varying(50),
        "AdditionalMetadataJson" text,
        CONSTRAINT "PK_StTemperatureAssets" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE INDEX idx_landsat_product_metadata_acquisition_date ON "LandsatProductMetadata" ("AcquisitionDate");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE INDEX idx_landsat_product_metadata_product_id ON "LandsatProductMetadata" ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE INDEX idx_landsat_products_create_at ON "LandsatProducts" ("CreateAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE INDEX idx_landsat_products_satellite_id ON "LandsatProducts" ("SatellateProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE INDEX idx_landsat_product_queue_product_id ON "LandsatProductsQueue" ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE INDEX idx_landsat_product_queue_status ON "LandsatProductsQueue" ("Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE INDEX idx_stac_features_datetime ON "StacFeatures" ("Datetime");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE INDEX idx_stac_features_datetime_geometry ON "StacFeatures" ("Datetime", "GeometryJson") WHERE "GeometryJson" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE INDEX idx_stac_features_feature_id ON "StacFeatures" ("FeatureId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    CREATE INDEX idx_stac_features_geometry_json ON "StacFeatures" ("GeometryJson") WHERE "GeometryJson" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024131117_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251024131117_Init_Database', '9.0.7');
    END IF;
END $EF$;
COMMIT;

