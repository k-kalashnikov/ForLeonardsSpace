CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818123948_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818123948_Init_Database') THEN
    CREATE TABLE "Bids" (
        "Id" uuid NOT NULL,
        "ParentId" uuid,
        "BidTypeId" uuid NOT NULL,
        "BidStateId" uuid NOT NULL,
        "ForemanId" uuid,
        "WorkerId" uuid,
        "StartDate" timestamp with time zone,
        "DeadlineDate" timestamp with time zone,
        "EndDate" timestamp with time zone,
        "FieldId" uuid,
        "RegionId" uuid,
        "CropId" uuid,
        "VarietyId" uuid,
        "Comment" text,
        "Description" text,
        "Lat" double precision,
        "Lng" double precision,
        "Number" bigint NOT NULL,
        "FieldPlantingDate" timestamp with time zone,
        "Customer" text,
        "FileResultId" uuid,
        "BidTemplateId" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_Bids" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818123948_Init_Database') THEN
    CREATE TABLE "BidTemplates" (
        "Id" uuid NOT NULL,
        "CropId" uuid NOT NULL,
        "SchemaVersion" integer NOT NULL,
        "ContentVersion" integer NOT NULL,
        "DataJson" text NOT NULL,
        "Comment" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_BidTemplates" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818123948_Init_Database') THEN
    CREATE TABLE "Fields" (
        "Id" uuid NOT NULL,
        "Name" text,
        "FieldArea" double precision,
        "RegionId" uuid,
        "ExternalId" text,
        "SoilTypeId" uuid,
        "AgroclimaticZoneId" uuid,
        "Comment" text,
        "AgricultureProducerId" uuid,
        "IrrigationTypeId" uuid,
        "IrrigationSourceId" uuid,
        "WaterSaving" boolean,
        "SoilIndex" double precision,
        "Polygon" geometry,
        "Control" boolean,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_Fields" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818123948_Init_Database') THEN
    CREATE TABLE "Seasons" (
        "Id" uuid NOT NULL,
        "StartDate" date,
        "EndDate" date,
        "Title" text,
        "PlantingDate" date,
        "HarvestingDate" date,
        "FieldId" uuid,
        "Latitude" double precision,
        "Longitude" double precision,
        "CropId" uuid,
        "VarietyId" uuid,
        "PlantingDatePlan" date,
        "FieldArea" double precision,
        "HarvestingDatePlan" date,
        "YieldHa" double precision,
        "Yield" double precision,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_Seasons" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818123948_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250818123948_Init_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250819122035_Update_Database') THEN
    CREATE SEQUENCE "BidNumberSequence" START WITH 1 INCREMENT BY 1 NO CYCLE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250819122035_Update_Database') THEN
    ALTER TABLE "Bids" ALTER COLUMN "Number" SET DEFAULT (nextval('"BidNumberSequence"'));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250819122035_Update_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250819122035_Update_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE TABLE "Field_Product_Mapping" (
        "Id" uuid NOT NULL,
        "FieldId" uuid NOT NULL,
        "ProductId" character varying(255) NOT NULL,
        "SatelliteType" integer NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_Field_Product_Mapping" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE TABLE "SatelliteSearchConfigs" (
        "Id" uuid NOT NULL,
        "SentinelPolygon" geometry,
        "LandsatLeftDown" geometry,
        "LandsatRightUp" geometry,
        "IsActive" boolean NOT NULL,
        "FieldsCount" integer NOT NULL,
        "BufferDistance" double precision NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_SatelliteSearchConfigs" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE INDEX idx_fields_create_at ON "Fields" ("CreateAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE INDEX idx_fields_external_id ON "Fields" ("ExternalId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE INDEX idx_fields_name ON "Fields" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE INDEX idx_fields_polygon ON "Fields" USING GIST ("Polygon");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE INDEX idx_fields_region_id ON "Fields" ("RegionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE INDEX idx_field_product_mapping_field_id ON "Field_Product_Mapping" ("FieldId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE INDEX idx_field_product_mapping_field_satellite ON "Field_Product_Mapping" ("FieldId", "SatelliteType");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE INDEX idx_field_product_mapping_product_id ON "Field_Product_Mapping" ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE INDEX idx_field_product_mapping_satellite_type ON "Field_Product_Mapping" ("SatelliteType");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE UNIQUE INDEX uk_field_product_mapping_field_product ON "Field_Product_Mapping" ("FieldId", "ProductId", "SatelliteType");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE INDEX "IX_SatelliteSearchConfigs_IsActive" ON "SatelliteSearchConfigs" ("IsActive");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    CREATE INDEX "IX_SatelliteSearchConfigs_IsActive_CreateAt" ON "SatelliteSearchConfigs" ("IsActive", "CreateAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250820143656_AddIndexesGeo') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250820143656_AddIndexesGeo', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250826141354_Update_Database_Bid') THEN
    ALTER TABLE "Bids" DROP COLUMN "BidStateId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250826141354_Update_Database_Bid') THEN
    UPDATE "Bids" SET "Lng" = 0.0 WHERE "Lng" IS NULL;
    ALTER TABLE "Bids" ALTER COLUMN "Lng" SET NOT NULL;
    ALTER TABLE "Bids" ALTER COLUMN "Lng" SET DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250826141354_Update_Database_Bid') THEN
    UPDATE "Bids" SET "Lat" = 0.0 WHERE "Lat" IS NULL;
    ALTER TABLE "Bids" ALTER COLUMN "Lat" SET NOT NULL;
    ALTER TABLE "Bids" ALTER COLUMN "Lat" SET DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250826141354_Update_Database_Bid') THEN
    UPDATE "Bids" SET "DeadlineDate" = TIMESTAMPTZ '-infinity' WHERE "DeadlineDate" IS NULL;
    ALTER TABLE "Bids" ALTER COLUMN "DeadlineDate" SET NOT NULL;
    ALTER TABLE "Bids" ALTER COLUMN "DeadlineDate" SET DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250826141354_Update_Database_Bid') THEN
    UPDATE "Bids" SET "CropId" = '00000000-0000-0000-0000-000000000000' WHERE "CropId" IS NULL;
    ALTER TABLE "Bids" ALTER COLUMN "CropId" SET NOT NULL;
    ALTER TABLE "Bids" ALTER COLUMN "CropId" SET DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250826141354_Update_Database_Bid') THEN
    ALTER TABLE "Bids" ADD "BidState" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250826141354_Update_Database_Bid') THEN
    ALTER TABLE "Bids" ADD "IsUnvalidBid" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250826141354_Update_Database_Bid') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250826141354_Update_Database_Bid', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250901153228_Update_Season_Add_Polygon') THEN
    ALTER TABLE "Seasons" ADD "Polygon" geometry;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250901153228_Update_Season_Add_Polygon') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250901153228_Update_Season_Add_Polygon', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903071119_Add_Polygon_In_Bid') THEN
    ALTER TABLE "Bids" ADD "Polygon" geometry;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903071119_Add_Polygon_In_Bid') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250903071119_Add_Polygon_In_Bid', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165206_Change_Localization') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250903165206_Change_Localization', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141106_Update_Locale_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250904141106_Update_Locale_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930070746_Update_Database_SoilData') THEN
    CREATE TABLE "SoilDatas" (
        "Id" uuid NOT NULL,
        "Point" geometry NOT NULL,
        "Parameter" text NOT NULL,
        "DepthRange" text NOT NULL,
        "Value" double precision,
        "Unit" text NOT NULL,
        "Source" text NOT NULL,
        "PointJson" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_SoilDatas" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930070746_Update_Database_SoilData') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250930070746_Update_Database_SoilData', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930121217_Update_Database_SoilData_1_1') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250930121217_Update_Database_SoilData_1_1', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930121622_Update_Database_SoilData_1_2') THEN
    ALTER TABLE "SoilDatas" ADD "FieldId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930121622_Update_Database_SoilData_1_2') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250930121622_Update_Database_SoilData_1_2', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930140653_Update_Database_SoilData_1_3') THEN
    ALTER TABLE "SoilDatas" DROP COLUMN "FieldId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930140653_Update_Database_SoilData_1_3') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250930140653_Update_Database_SoilData_1_3', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930142338_Update_Database_SoilData_1_4') THEN
    DROP TABLE "SoilDatas";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930142338_Update_Database_SoilData_1_4') THEN

                    CREATE TABLE IF NOT EXISTS "SoilDatas" (
                        "Id" uuid NOT NULL,
                        "Point" geometry NOT NULL,
                        "Parameter" text NOT NULL,
                        "DepthRange" text NOT NULL,
                        "Value" double precision NULL,
                        "Unit" text NOT NULL,
                        "Source" text NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "LastUpdateAt" timestamp with time zone NOT NULL,
                        "Status" integer NOT NULL,
                        "CreateUser" uuid NOT NULL,
                        "LastUpdateUser" uuid NOT NULL,
                        CONSTRAINT "PK_SoilDatas" PRIMARY KEY ("Id", "Parameter")
                    ) PARTITION BY RANGE ("Parameter");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930142338_Update_Database_SoilData_1_4') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250930142338_Update_Database_SoilData_1_4', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN
    DROP TABLE "SoilDatas";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                    CREATE TABLE "SoilDatas" (
                        "Id" uuid NOT NULL,
                        "Point" geometry NOT NULL,
                        "Parameter" text NOT NULL,
                        "DepthRange" text NOT NULL,
                        "Value" double precision NULL,
                        "Unit" text NOT NULL,
                        "Source" text NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "LastUpdateAt" timestamp with time zone NOT NULL,
                        "Status" integer NOT NULL,
                        "CreateUser" uuid NOT NULL,
                        "LastUpdateUser" uuid NOT NULL,
                        CONSTRAINT "PK_SoilDatas" PRIMARY KEY ("Id", "Parameter")
                    ) PARTITION BY LIST ("Parameter");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                        CREATE TABLE "SoilDatas_sand" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('sand');
                    
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                        CREATE TABLE "SoilDatas_silt" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('silt');
                    
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                        CREATE TABLE "SoilDatas_clay" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('clay');
                    
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                        CREATE TABLE "SoilDatas_phh2o" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('phh2o');
                    
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                        CREATE TABLE "SoilDatas_cec" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('cec');
                    
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                        CREATE TABLE "SoilDatas_soc" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('soc');
                    
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                        CREATE TABLE "SoilDatas_bdod" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('bdod');
                    
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                        CREATE TABLE "SoilDatas_cfvo" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('cfvo');
                    
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                        CREATE TABLE "SoilDatas_nitrogen" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('nitrogen');
                    
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                        CREATE TABLE "SoilDatas_humus" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('humus');
                    
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                        CREATE TABLE "SoilDatas_phosphorus" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('phosphorus');
                    
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN

                        CREATE TABLE "SoilDatas_salinity" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('salinity');
                    
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001103612_Update_Database_SoilData_1_5') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251001103612_Update_Database_SoilData_1_5', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN
    DROP TABLE "SoilDatas";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN
    CREATE TABLE "SoilDatas" (
        "Id" uuid NOT NULL,
        "Point" geometry(Point, 4326) NOT NULL,  
        "Parameter" text NOT NULL,
        "DepthRange" text NOT NULL,
        "Value" double precision NULL,
        "Unit" text NOT NULL,
        "Source" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "TileKey" text NOT NULL,
    CONSTRAINT "PK_SoilDatas" PRIMARY KEY ("Id", "TileKey")
    ) PARTITION BY LIST ("TileKey");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_37" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_37');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_37,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_37,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_37,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_37,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_37,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_37,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_38" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_38');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_38,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_38,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_38,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_38,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_38,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_38,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_39" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_39');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_39,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_39,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_39,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_39,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_39,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_39,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_40" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_40');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_40,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_40,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_40,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_40,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_40,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_40,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_41" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_41');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_41,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_41,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_41,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_41,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_41,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_41,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_42" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_42');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_42,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_42,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_42,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_42,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_42,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_42,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_43" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_43');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_43,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_43,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_43,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_43,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_43,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_43,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_44" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_44');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_44,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_44,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_44,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_44,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_44,75" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_44,75');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_45" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_45');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_45,25" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_45,25');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_56,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_56,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_57,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_57,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_58,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_58,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_59,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_59,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_60,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_60,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_61,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_61,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_62,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_62,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_63,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_63,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_64,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_64,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_65,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_65,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_66,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_66,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_67,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_67,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_68,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_68,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_69,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_69,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_70,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_70,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_71,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_71,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,25_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,25_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,5_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,5_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_72,75_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_72,75_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN

                        CREATE TABLE "SoilDatas_73_45,5" PARTITION OF "SoilDatas"
                            FOR VALUES IN ('x_73_y_45,5');
                        
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251001134932_Update_Database_SoilData_1_6') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251001134932_Update_Database_SoilData_1_6', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251002120552_Add_FieldAgroOperation') THEN
    CREATE TABLE "FieldAgroOperations" (
        "Id" uuid NOT NULL,
        "FieldId" uuid NOT NULL,
        "OperationId" uuid NOT NULL,
        "OperationTypeFullName" text NOT NULL,
        "AgroOperationParamsJson" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_FieldAgroOperations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251002120552_Add_FieldAgroOperation') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251002120552_Add_FieldAgroOperation', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251003101357_Update_FAO') THEN
    ALTER TABLE "FieldAgroOperations" ADD "OperationName" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251003101357_Update_FAO') THEN
    ALTER TABLE "FieldAgroOperations" ADD "OriginalDate" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251003101357_Update_FAO') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251003101357_Update_FAO', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251003102033_Update_FAO_1') THEN
    ALTER TABLE "FieldAgroOperations" ALTER COLUMN "OperationTypeFullName" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251003102033_Update_FAO_1') THEN
    ALTER TABLE "FieldAgroOperations" ALTER COLUMN "AgroOperationParamsJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251003102033_Update_FAO_1') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251003102033_Update_FAO_1', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251007133950_Add_Comment_to_FAO') THEN
    ALTER TABLE "FieldAgroOperations" ADD "Comment" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251007133950_Add_Comment_to_FAO') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251007133950_Add_Comment_to_FAO', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251007154849_Add_FAPH_and_FIH') THEN
    CREATE TABLE "FieldAgroProducerHistories" (
        "Id" uuid NOT NULL,
        "FieldId" uuid NOT NULL,
        "AgricultureProducerId" uuid NOT NULL,
        "PeriodStart" date NOT NULL,
        "PeriodEnd" date NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_FieldAgroProducerHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251007154849_Add_FAPH_and_FIH') THEN
    CREATE TABLE "FieldInsuranceHistories" (
        "Id" uuid NOT NULL,
        "FieldId" uuid NOT NULL,
        "AgricultureProducerId" uuid NOT NULL,
        "PeriodStart" date NOT NULL,
        "PeriodEnd" date NOT NULL,
        "SumInsured" double precision,
        "InsurancePremium" double precision,
        "Payments" double precision,
        "Comment" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_FieldInsuranceHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251007154849_Add_FAPH_and_FIH') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251007154849_Add_FAPH_and_FIH', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251014120922_Add_Bonitet_Classifier') THEN
    ALTER TABLE "Fields" ADD "BonitetScore" double precision;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251014120922_Add_Bonitet_Classifier') THEN
    ALTER TABLE "Fields" ADD "Classifier" double precision;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251014120922_Add_Bonitet_Classifier') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251014120922_Add_Bonitet_Classifier', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120327_Update_Database_IsPublic') THEN
    ALTER TABLE "SoilDatas" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120327_Update_Database_IsPublic') THEN
    ALTER TABLE "Seasons" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120327_Update_Database_IsPublic') THEN
    ALTER TABLE "SatelliteSearchConfigs" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120327_Update_Database_IsPublic') THEN
    ALTER TABLE "Fields" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120327_Update_Database_IsPublic') THEN
    ALTER TABLE "FieldInsuranceHistories" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120327_Update_Database_IsPublic') THEN
    ALTER TABLE "FieldAgroProducerHistories" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120327_Update_Database_IsPublic') THEN
    ALTER TABLE "FieldAgroOperations" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120327_Update_Database_IsPublic') THEN
    ALTER TABLE "Field_Product_Mapping" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120327_Update_Database_IsPublic') THEN
    ALTER TABLE "BidTemplates" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120327_Update_Database_IsPublic') THEN
    ALTER TABLE "Bids" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120327_Update_Database_IsPublic') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251024120327_Update_Database_IsPublic', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251105121755_Update_Yield_Field') THEN
    ALTER TABLE "Seasons" RENAME COLUMN "YieldHa" TO "YieldHaFact";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251105121755_Update_Yield_Field') THEN
    ALTER TABLE "Seasons" RENAME COLUMN "Yield" TO "YieldFact";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251105121755_Update_Yield_Field') THEN
    ALTER TABLE "Seasons" ADD "YieldPlan" double precision;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251105121755_Update_Yield_Field') THEN
    ALTER TABLE "Seasons" ADD "YieldHaPlan" double precision;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251105121755_Update_Yield_Field') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251105121755_Update_Yield_Field', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251106125616_FieldPhoto_Added') THEN
    CREATE TABLE "FieldPhotos" (
        "Id" uuid NOT NULL,
        "Title" text NOT NULL,
        "FileStorageId" uuid NOT NULL,
        "FieldId" uuid NOT NULL,
        "Point" geometry,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_FieldPhotos" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251106125616_FieldPhoto_Added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251106125616_FieldPhoto_Added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251114075507_UpdateFiledPhoto') THEN
    ALTER TABLE "FieldPhotos" ALTER COLUMN "FieldId" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251114075507_UpdateFiledPhoto') THEN
    ALTER TABLE "FieldPhotos" ADD "CaptureDateUtc" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251114075507_UpdateFiledPhoto') THEN
    ALTER TABLE "FieldPhotos" ADD "Description" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251114075507_UpdateFiledPhoto') THEN
    ALTER TABLE "FieldPhotos" ADD "ParentRegionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251114075507_UpdateFiledPhoto') THEN
    ALTER TABLE "FieldPhotos" ADD "RegionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251114075507_UpdateFiledPhoto') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251114075507_UpdateFiledPhoto', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251128135933_AddaltitudeAboveSeaLevel') THEN
    ALTER TABLE "Fields" ADD "AltitudeAboveSeaLevel" double precision;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251128135933_AddaltitudeAboveSeaLevel') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251128135933_AddaltitudeAboveSeaLevel', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251201074714_Imported_Fields_added') THEN
    CREATE TABLE "ImportedFieldReports" (
        "Id" uuid NOT NULL,
        "Comment" text,
        "FileStorageItemId" uuid NOT NULL,
        "CountValidImportedField" integer NOT NULL,
        "CountNotValidImportedField" integer NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_ImportedFieldReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251201074714_Imported_Fields_added') THEN
    CREATE TABLE "ImportedFields" (
        "Id" uuid NOT NULL,
        "FieldName" text NOT NULL,
        "Polygon" geometry,
        "ImportedFieldReportId" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_ImportedFields" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251201074714_Imported_Fields_added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251201074714_Imported_Fields_added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251201151859_Imported_Fields_couunt_added') THEN
    ALTER TABLE "ImportedFieldReports" ADD "FieldsCount" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251201151859_Imported_Fields_couunt_added') THEN
    ALTER TABLE "ImportedFieldReports" ADD "FileSize" bigint;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251201151859_Imported_Fields_couunt_added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251201151859_Imported_Fields_couunt_added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251203130932_Imported_Field_Data_Added') THEN
    ALTER TABLE "ImportedFields" ADD "DataJson" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251203130932_Imported_Field_Data_Added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251203130932_Imported_Field_Data_Added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE TABLE "FieldEncumbrances" (
        "Id" uuid NOT NULL,
        "FieldId" uuid NOT NULL,
        "StartDate" date NOT NULL,
        "EndDate" date NOT NULL,
        "EncumbranceId" uuid NOT NULL,
        "Comment" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_FieldEncumbrances" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE TABLE "FieldFinancialConditions" (
        "Id" uuid NOT NULL,
        "FieldId" uuid NOT NULL,
        "FinancialConditionId" uuid NOT NULL,
        "StartDate" date NOT NULL,
        "EndDate" date NOT NULL,
        "Amount" numeric NOT NULL,
        "Purpose" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_FieldFinancialConditions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE TABLE "FieldInsuranceCases" (
        "Id" uuid NOT NULL,
        "FieldId" uuid NOT NULL,
        "PaymentDate" date NOT NULL,
        "PaymentAmount" numeric NOT NULL,
        "InsuranceCaseId" uuid NOT NULL,
        "IsInsuranceCase" boolean NOT NULL,
        "Comment" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_FieldInsuranceCases" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE TABLE "FieldIrrigationDatas" (
        "Id" uuid NOT NULL,
        "FieldId" uuid NOT NULL,
        "AnnualLevelPerArea" double precision,
        "Requirement" double precision,
        "Actual" double precision,
        "WaterCharacteristics" double precision,
        "Year" integer,
        "Comment" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_FieldIrrigationDatas" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE TABLE "FieldSoilProfileAnalyses" (
        "Id" uuid NOT NULL,
        "FieldId" uuid NOT NULL,
        "AnalysisDate" date NOT NULL,
        "Coordinates" geometry,
        "AnalysisResults" text,
        "FertileLayerDepth" double precision,
        "Humus" double precision,
        "Ph" numeric,
        "SoilSalinity" double precision,
        "ChlorideSalinity" double precision,
        "SulfateSalinity" double precision,
        "CarbonateSalinity" double precision,
        "Macronutrients" double precision,
        "Nitrogen" double precision,
        "Phosphorus" double precision,
        "Potassium" double precision,
        "Sulfur" double precision,
        "Micronutrients" double precision,
        "SoilProfileAnalysisId" uuid,
        "SoilClassification" double precision,
        "BonitScore" double precision,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_FieldSoilProfileAnalyses" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE INDEX idx_field_encumbrance_dates ON "FieldEncumbrances" ("StartDate", "EndDate");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE INDEX idx_field_encumbrance_field_id ON "FieldEncumbrances" ("FieldId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE INDEX idx_field_financial_condition_dates ON "FieldFinancialConditions" ("StartDate", "EndDate");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE INDEX idx_field_financial_condition_field_id ON "FieldFinancialConditions" ("FieldId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE INDEX idx_field_insurance_case_field_id ON "FieldInsuranceCases" ("FieldId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE INDEX idx_field_insurance_case_payment_date ON "FieldInsuranceCases" ("PaymentDate");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE INDEX idx_field_irrigation_data_field_id ON "FieldIrrigationDatas" ("FieldId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE INDEX idx_field_irrigation_data_year ON "FieldIrrigationDatas" ("Year");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE INDEX idx_field_soil_profile_analysis_date ON "FieldSoilProfileAnalyses" ("AnalysisDate");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    CREATE INDEX idx_field_soil_profile_field_id ON "FieldSoilProfileAnalyses" ("FieldId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204134335_AddFieldAddictionalInfo') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251204134335_AddFieldAddictionalInfo', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251205081254_ImportedFieldLogs_added') THEN
    CREATE TABLE "ImportedFieldLogs" (
        "Id" uuid NOT NULL,
        "SeasonId" uuid NOT NULL,
        "IntersectedSeasons" uuid[] NOT NULL,
        "CoveredSeasons" uuid[] NOT NULL,
        "CoveredBySeasons" uuid[] NOT NULL,
        "EqualPolygonsSeasons" uuid[] NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_ImportedFieldLogs" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251205081254_ImportedFieldLogs_added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251205081254_ImportedFieldLogs_added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251210082553_Add_Qwen_Integration') THEN
    ALTER TABLE "Bids" ADD "QwenAnalysisEnd" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251210082553_Add_Qwen_Integration') THEN
    ALTER TABLE "Bids" ADD "QwenAnalysisStart" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251210082553_Add_Qwen_Integration') THEN
    ALTER TABLE "Bids" ADD "QwenResultJson" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251210082553_Add_Qwen_Integration') THEN
    ALTER TABLE "Bids" ADD "QwenTaskId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251210082553_Add_Qwen_Integration') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251210082553_Add_Qwen_Integration', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218075434_Update_Qwen_Integration') THEN
    ALTER TABLE "Bids" ALTER COLUMN "QwenResultJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218075434_Update_Qwen_Integration') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251218075434_Update_Qwen_Integration', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218141501_AddFieldTypeRelation') THEN
    ALTER TABLE "Fields" ADD "FieldTypeId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218141501_AddFieldTypeRelation') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251218141501_AddFieldTypeRelation', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251219063113_Update_Qwen_Integration_2') THEN
    ALTER TABLE "Bids" ADD "QwenExpressAnalysisEnd" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251219063113_Update_Qwen_Integration_2') THEN
    ALTER TABLE "Bids" ADD "QwenExpressAnalysisStart" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251219063113_Update_Qwen_Integration_2') THEN
    ALTER TABLE "Bids" ADD "QwenExpressResultJson" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251219063113_Update_Qwen_Integration_2') THEN
    ALTER TABLE "Bids" ADD "QwenExpressTaskId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251219063113_Update_Qwen_Integration_2') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251219063113_Update_Qwen_Integration_2', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251226132742_Update_Season') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251226132742_Update_Season', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251226132859_Update_Season_1') THEN
    ALTER TABLE "Seasons" ADD "CalculationUpdateDate" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251226132859_Update_Season_1') THEN
    ALTER TABLE "Seasons" ADD "MonitoringInterval" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251226132859_Update_Season_1') THEN
    ALTER TABLE "Seasons" ADD "MonitoringPerioidEnd" date;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251226132859_Update_Season_1') THEN
    ALTER TABLE "Seasons" ADD "MonitoringPerioidStart" date;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251226132859_Update_Season_1') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251226132859_Update_Season_1', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251229111743_RootSystemType_added') THEN
    ALTER TABLE "Seasons" ADD "RootSystemType" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251229111743_RootSystemType_added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251229111743_RootSystemType_added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260113113126_Update_Season_RootSystemTypeId') THEN
    ALTER TABLE "Seasons" RENAME COLUMN "RootSystemType" TO "GovernmentContractNumber";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260113113126_Update_Season_RootSystemTypeId') THEN
    ALTER TABLE "Seasons" ADD "GovernmentContractEndDate" date;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260113113126_Update_Season_RootSystemTypeId') THEN
    ALTER TABLE "Seasons" ADD "GovernmentContractStartDate" date;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260113113126_Update_Season_RootSystemTypeId') THEN
    ALTER TABLE "Seasons" ADD "RootSystemTypeId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260113113126_Update_Season_RootSystemTypeId') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260113113126_Update_Season_RootSystemTypeId', '9.0.7');
    END IF;
END $EF$;
COMMIT;

