CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124411_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124411_Init_Database') THEN
    CREATE TABLE "Sentinel2ProductsMetadata" (
        "Id" uuid NOT NULL,
        "Name" text,
        "ContentType" text,
        "ContentLength" bigint,
        "OriginDate" timestamp with time zone,
        "PublicationDate" timestamp with time zone,
        "ModificationDate" timestamp with time zone,
        "Online" boolean NOT NULL,
        "EvictionDate" timestamp with time zone,
        "S3Path" text,
        "Footprint" text,
        "ChecksumMd5" text,
        "ContentDateStart" timestamp with time zone NOT NULL,
        "ContentDateEnd" timestamp with time zone NOT NULL,
        "ProductId" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_Sentinel2ProductsMetadata" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124411_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "Sentinel2Products" ("Id" uuid NOT NULL, "SatellateProductId" text NOT NULL, "SentinelInspireMetadataId" uuid, "SentinelL1CProductMetadataId" uuid, "SentinelL1CTileMetadataId" uuid, "SentinelProductQualityMetadataId" uuid, "CreateAt" timestamp with time zone NOT NULL, "Status" integer NOT NULL, "LastUpdateAt" timestamp with time zone NOT NULL, "CreateUser" uuid NOT NULL, "LastUpdateUser" uuid NOT NULL, CONSTRAINT "PK_Sentinel2Products" PRIMARY KEY ("Id", "CreateAt")) PARTITION BY RANGE ("CreateAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124411_Init_Database') THEN
    CREATE TABLE "Sentinel2ProductsQueue" (
        "Id" uuid NOT NULL,
        "ProductId" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_Sentinel2ProductsQueue" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124411_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "SentinelInspireMetadata" ("Id" uuid NOT NULL, "FileIdentifier" text NOT NULL, "LanguageCode" text NOT NULL, "CharacterSetCode" text NOT NULL, "HierarchyLevelCode" text NOT NULL, "DateStamp" timestamp with time zone NOT NULL, "MetadataStandardName" text NOT NULL, "MetadataStandardVersion" text NOT NULL, "OrganisationName" text NOT NULL, "Email" text NOT NULL, "RoleCode" text NOT NULL, "WestBoundLongitude" numeric NOT NULL, "EastBoundLongitude" numeric NOT NULL, "SouthBoundLatitude" numeric NOT NULL, "NorthBoundLatitude" numeric NOT NULL, "ReferenceSystemCode" text NOT NULL, "ReferenceSystemCodeSpace" text NOT NULL, "CreateAt" timestamp with time zone NOT NULL, "Status" integer NOT NULL, "LastUpdateAt" timestamp with time zone NOT NULL, "CreateUser" uuid NOT NULL, "LastUpdateUser" uuid NOT NULL, CONSTRAINT "PK_SentinelInspireMetadata" PRIMARY KEY ("Id", "CreateAt")) PARTITION BY RANGE ("CreateAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124411_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "SentinelL1CProductMetadata" ("Id" uuid NOT NULL, "ProductStartTime" timestamp with time zone NOT NULL, "ProductStopTime" timestamp with time zone NOT NULL, "ProductUri" text, "ProcessingLevel" text, "ProductType" text, "ProcessingBaseline" text, "ProductDoi" text, "GenerationTime" timestamp with time zone NOT NULL, "DatatakeRaw" text NOT NULL, "GranulesRaw" text NOT NULL, "SpecialValuesRaw" text NOT NULL, "RedChannel" integer NOT NULL, "GreenChannel" integer NOT NULL, "BlueChannel" integer NOT NULL, "QuantificationValue" integer NOT NULL, "OffsetsRaw" text NOT NULL, "ReflectanceU" double precision NOT NULL, "SolarIrradianceListRaw" text NOT NULL, "SpectralBandIds" text, "SpectralPhysicalBands" text, "SpectralResolutions" text, "SpectralWavelengthMins" text, "SpectralWavelengthMaxs" text, "SpectralWavelengthCentrals" text, "SpectralInformationListRaw" text NOT NULL, "ReferenceBand" integer NOT NULL, "ExtPosList" text, "RasterCsType" text, "PixelOrigin" integer NOT NULL, "GeoTables" text, "HorizontalCsType" text, "GippFilesRaw" text NOT NULL, "CreateAt" timestamp with time zone NOT NULL, "Status" integer NOT NULL, "LastUpdateAt" timestamp with time zone NOT NULL, "CreateUser" uuid NOT NULL, "LastUpdateUser" uuid NOT NULL, CONSTRAINT "PK_SentinelL1CProductMetadata" PRIMARY KEY ("Id", "CreateAt")) PARTITION BY RANGE ("CreateAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124411_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "SentinelL1CTileMetadata" ("Id" uuid NOT NULL, "TileId" text NOT NULL, "DataStripId" text NOT NULL, "DownlinkPriority" text NOT NULL, "SensingTime" timestamp with time zone NOT NULL, "ArchivingCentre" text NOT NULL, "ArchivingTime" timestamp with time zone NOT NULL, "HorizontalCSName" text NOT NULL, "HorizontalCSCode" text NOT NULL, "SizesRaw" text NOT NULL, "GeoPositionsRaw" text NOT NULL, "MeanSunAngleZenithAngle" text NOT NULL, "SunAnglesZenithColStepValue" double precision NOT NULL, "MeanSunAngleAzimuthAngle" text NOT NULL, "SunAnglesZenithRowStepValue" double precision NOT NULL, "SunAnglesZenithValuesList" text[] NOT NULL, "SunAnglesAzimuthColStepUnit" text NOT NULL, "SunAnglesAzimuthColStepValue" double precision NOT NULL, "SunAnglesAzimuthRowStepUnit" text NOT NULL, "SunAnglesAzimuthRowStepValue" double precision NOT NULL, "SunAnglesAzimuthValuesList" text[] NOT NULL, "MeanSunAngleZenithColStepUnit" text NOT NULL, "MeanSunAngleZenithColStepValue" double precision NOT NULL, "MeanSunAngleZenithRowStepUnit" text NOT NULL, "MeanSunAngleZenithRowStepValue" double precision NOT NULL, "MeanSunAngleZenithValuesList" text[] NOT NULL, "MeanSunAngleAzimuthColStepUnit" text NOT NULL, "MeanSunAngleAzimuthColStepValue" double precision NOT NULL, "MeanSunAngleAzimuthRowStepUnit" text NOT NULL, "MeanSunAngleAzimuthRowStepValue" double precision NOT NULL, "MeanSunAngleAzimuthValuesList" text[] NOT NULL, "ViewingIncidenceAnglesGridsRaw" text NOT NULL, "MeanViewingIncidenceAngleListRaw" text NOT NULL, "CloudyPixelPercentage" double precision NOT NULL, "DegradedDataPercentage" double precision NOT NULL, "SnowPixelPercentage" double precision NOT NULL, "PixelLevelQiRaw" text NOT NULL, "CreateAt" timestamp with time zone NOT NULL, "Status" integer NOT NULL, "LastUpdateAt" timestamp with time zone NOT NULL, "CreateUser" uuid NOT NULL, "LastUpdateUser" uuid NOT NULL, CONSTRAINT "PK_SentinelL1CTileMetadata" PRIMARY KEY ("Id", "CreateAt")) PARTITION BY RANGE ("CreateAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124411_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "SentinelProductQualityMetadata" ("Id" uuid NOT NULL, "FileName" text NOT NULL, "FileDescription" text NOT NULL, "Notes" text NOT NULL, "Mission" text NOT NULL, "FileClass" text NOT NULL, "FileType" text NOT NULL, "ValidityStart" text NOT NULL, "ValidityStop" text NOT NULL, "FileVersion" text NOT NULL, "System" text NOT NULL, "Creator" text NOT NULL, "CreatorVersion" text NOT NULL, "CreationDate" text NOT NULL, "Type" text NOT NULL, "GippVersion" text NOT NULL, "GlobalStatus" text NOT NULL, "Date" timestamp with time zone NOT NULL, "ParentId" text NOT NULL, "Name" text NOT NULL, "Version" text NOT NULL, "ChecksRaw" text NOT NULL, "CreateAt" timestamp with time zone NOT NULL, "Status" integer NOT NULL, "LastUpdateAt" timestamp with time zone NOT NULL, "CreateUser" uuid NOT NULL, "LastUpdateUser" uuid NOT NULL, CONSTRAINT "PK_SentinelProductQualityMetadata" PRIMARY KEY ("Id", "CreateAt")) PARTITION BY RANGE ("CreateAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124411_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250818124411_Init_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel2_products_create_at ON "Sentinel2Products" ("CreateAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel2_products_inspire_id ON "Sentinel2Products" ("SentinelInspireMetadataId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel2_products_satellite_id ON "Sentinel2Products" ("SatellateProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel2_product_metadata_content_date_end ON "Sentinel2ProductsMetadata" ("ContentDateEnd");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel2_product_metadata_content_date_start ON "Sentinel2ProductsMetadata" ("ContentDateStart");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel2_product_metadata_content_dates ON "Sentinel2ProductsMetadata" ("ContentDateStart", "ContentDateEnd");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel2_product_metadata_product_id ON "Sentinel2ProductsMetadata" ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel2_product_queue_create_at ON "Sentinel2ProductsQueue" ("CreateAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel2_product_queue_product_id ON "Sentinel2ProductsQueue" ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel2_product_queue_status ON "Sentinel2ProductsQueue" ("Status");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel_inspire_bbox ON "SentinelInspireMetadata" ("WestBoundLongitude", "EastBoundLongitude", "SouthBoundLatitude", "NorthBoundLatitude");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel_inspire_datestamp ON "SentinelInspireMetadata" ("DateStamp");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel_inspire_file_identifier ON "SentinelInspireMetadata" ("FileIdentifier");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel_l1c_product_metadata_generation_time ON "SentinelL1CProductMetadata" ("GenerationTime");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel_l1c_product_metadata_start_time ON "SentinelL1CProductMetadata" ("ProductStartTime");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    CREATE INDEX idx_sentinel_l1c_product_metadata_stop_time ON "SentinelL1CProductMetadata" ("ProductStopTime");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820141908_AddIndexesGeo') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250820141908_AddIndexesGeo', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141325_Update_Locale_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250904141325_Update_Locale_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251003192120_Update_SatelliteProduct') THEN
    ALTER TABLE "Sentinel2ProductsQueue" ADD "OriginDate" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251003192120_Update_SatelliteProduct') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251003192120_Update_SatelliteProduct', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251003194527_Update_SatelliteProductEntity') THEN
    ALTER TABLE "Sentinel2Products" ADD "OriginDate" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251003194527_Update_SatelliteProductEntity') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251003194527_Update_SatelliteProductEntity', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251012124030_Add_SentinelProductQualityMetadata') THEN
    ALTER TABLE "Sentinel2ProductsQueue" ADD "QueueStatus" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251012124030_Add_SentinelProductQualityMetadata') THEN
    CREATE TABLE "Sentinel2GenerateIndexStatus" (
        "Id" uuid NOT NULL,
        "Sentinel2ProductQueue" uuid NOT NULL,
        "ArviTiff" boolean NOT NULL,
        "EviTiff" boolean NOT NULL,
        "GndviTiff" boolean NOT NULL,
        "MndwiTiff" boolean NOT NULL,
        "NdmiTiff" boolean NOT NULL,
        "NdviTiff" boolean NOT NULL,
        "OrviTiff" boolean NOT NULL,
        "OsaviTiff" boolean NOT NULL,
        "ArviDb" boolean NOT NULL,
        "EviDb" boolean NOT NULL,
        "GndviDb" boolean NOT NULL,
        "MndwiDb" boolean NOT NULL,
        "NdmiDb" boolean NOT NULL,
        "NdviDb" boolean NOT NULL,
        "OrviDb" boolean NOT NULL,
        "OsaviDb" boolean NOT NULL,
        CONSTRAINT "PK_Sentinel2GenerateIndexStatus" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251012124030_Add_SentinelProductQualityMetadata') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251012124030_Add_SentinelProductQualityMetadata', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121329_Update_Database_IsPublic') THEN
    ALTER TABLE "SentinelProductQualityMetadata" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121329_Update_Database_IsPublic') THEN
    ALTER TABLE "SentinelL1CTileMetadata" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121329_Update_Database_IsPublic') THEN
    ALTER TABLE "SentinelL1CProductMetadata" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121329_Update_Database_IsPublic') THEN
    ALTER TABLE "SentinelInspireMetadata" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121329_Update_Database_IsPublic') THEN
    ALTER TABLE "Sentinel2ProductsQueue" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121329_Update_Database_IsPublic') THEN
    ALTER TABLE "Sentinel2ProductsMetadata" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121329_Update_Database_IsPublic') THEN
    ALTER TABLE "Sentinel2Products" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121329_Update_Database_IsPublic') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251024121329_Update_Database_IsPublic', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251215085243_Ndwi_added') THEN
    ALTER TABLE "Sentinel2GenerateIndexStatus" ADD "NdwiDb" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251215085243_Ndwi_added') THEN
    ALTER TABLE "Sentinel2GenerateIndexStatus" ADD "NdwiTiff" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251215085243_Ndwi_added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251215085243_Ndwi_added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260115130713_Fix_Migration') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260115130713_Fix_Migration', '9.0.7');
    END IF;
END $EF$;
COMMIT;

