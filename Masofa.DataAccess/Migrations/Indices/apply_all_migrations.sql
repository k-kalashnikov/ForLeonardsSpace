CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "ArviPoints" (
        "Id" uuid NOT NULL,
        "BZero8" real NOT NULL,
        "BZero4" real NOT NULL,
        "BZero2" real,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Point" geometry NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_ArviPoints" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "ArviPolygonRelations" (
        "Id" uuid NOT NULL,
        "ArviId" uuid NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_ArviPolygonRelations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "ArviPolygons" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Polygon" geometry NOT NULL,
        "FileStorageItemId" uuid NOT NULL,
        "IsColored" boolean NOT NULL,
        CONSTRAINT "PK_ArviPolygons" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "EviPoints" (
        "Id" uuid NOT NULL,
        "BZero8" real NOT NULL,
        "BZero4" real NOT NULL,
        "BZero2" real,
        "EPS" real NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Point" geometry NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_EviPoints" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "EviPolygonRelations" (
        "Id" uuid NOT NULL,
        "EviId" uuid NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_EviPolygonRelations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "EviPolygons" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Polygon" geometry NOT NULL,
        "FileStorageItemId" uuid NOT NULL,
        "IsColored" boolean NOT NULL,
        CONSTRAINT "PK_EviPolygons" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "GndviPoints" (
        "Id" uuid NOT NULL,
        "BZero8" real NOT NULL,
        "BZero3" real NOT NULL,
        "EPS" real NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Point" geometry NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_GndviPoints" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "GndviPolygonRelations" (
        "Id" uuid NOT NULL,
        "GNdviId" uuid NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_GndviPolygonRelations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "GndviPolygons" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Polygon" geometry NOT NULL,
        "FileStorageItemId" uuid NOT NULL,
        "IsColored" boolean NOT NULL,
        CONSTRAINT "PK_GndviPolygons" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "MndwiPoints" (
        "Id" uuid NOT NULL,
        "BZero3" real NOT NULL,
        "B11" real NOT NULL,
        "EPS" real NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Point" geometry NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_MndwiPoints" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "MndwiPolygonRelations" (
        "Id" uuid NOT NULL,
        "MndWiId" uuid NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_MndwiPolygonRelations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "MndwiPolygons" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Polygon" geometry NOT NULL,
        "FileStorageItemId" uuid NOT NULL,
        "IsColored" boolean NOT NULL,
        CONSTRAINT "PK_MndwiPolygons" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "NdmiPoints" (
        "Id" uuid NOT NULL,
        "BZero8" real NOT NULL,
        "B11" real NOT NULL,
        "EPS" real NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Point" geometry NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_NdmiPoints" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "NdmiPolygonRelations" (
        "Id" uuid NOT NULL,
        "NdMiId" uuid NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_NdmiPolygonRelations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "NdmiPolygons" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Polygon" geometry NOT NULL,
        "FileStorageItemId" uuid NOT NULL,
        "IsColored" boolean NOT NULL,
        CONSTRAINT "PK_NdmiPolygons" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "NdviPoints" (
        "Id" uuid NOT NULL,
        "BZero8" real NOT NULL,
        "BZero4" real NOT NULL,
        "EPS" real NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Point" geometry NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_NdviPoints" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "NdviPolygonRelations" (
        "Id" uuid NOT NULL,
        "NdViId" uuid NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_NdviPolygonRelations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "NdviPolygons" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Polygon" geometry NOT NULL,
        "FileStorageItemId" uuid NOT NULL,
        "IsColored" boolean NOT NULL,
        CONSTRAINT "PK_NdviPolygons" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "OrviPoints" (
        "Id" uuid NOT NULL,
        "BZero8" real NOT NULL,
        "BZero3" real NOT NULL,
        "EPS" real NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Point" geometry NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_OrviPoints" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "OrviPolygonRelations" (
        "Id" uuid NOT NULL,
        "OrViId" uuid NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_OrviPolygonRelations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "OrviPolygons" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Polygon" geometry NOT NULL,
        "FileStorageItemId" uuid NOT NULL,
        "IsColored" boolean NOT NULL,
        CONSTRAINT "PK_OrviPolygons" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "OsaviPoints" (
        "Id" uuid NOT NULL,
        "BZero8" real NOT NULL,
        "BZero4" real NOT NULL,
        "EPS" real NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Point" geometry NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_OsaviPoints" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "OsaviPolygonRelations" (
        "Id" uuid NOT NULL,
        "OsaViId" uuid NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_OsaviPolygonRelations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    CREATE TABLE "OsaviPolygons" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "Polygon" geometry NOT NULL,
        "FileStorageItemId" uuid NOT NULL,
        "IsColored" boolean NOT NULL,
        CONSTRAINT "PK_OsaviPolygons" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251006124304_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251006124304_Init_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "ArviSeasonReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "SeasonId" uuid NOT NULL,
        CONSTRAINT "PK_ArviSeasonReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "ArviSharedReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "CropId" uuid,
        "AverageMax" double precision NOT NULL,
        "AverageMin" double precision NOT NULL,
        CONSTRAINT "PK_ArviSharedReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "EviSeasonReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "SeasonId" uuid NOT NULL,
        CONSTRAINT "PK_EviSeasonReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "EviSharedReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "CropId" uuid,
        "AverageMax" double precision NOT NULL,
        "AverageMin" double precision NOT NULL,
        CONSTRAINT "PK_EviSharedReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "GndviSeasonReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "SeasonId" uuid NOT NULL,
        CONSTRAINT "PK_GndviSeasonReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "GndviSharedReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "CropId" uuid,
        "AverageMax" double precision NOT NULL,
        "AverageMin" double precision NOT NULL,
        CONSTRAINT "PK_GndviSharedReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "MndwiSeasonReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "SeasonId" uuid NOT NULL,
        CONSTRAINT "PK_MndwiSeasonReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "MndwiSharedReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "CropId" uuid,
        "AverageMax" double precision NOT NULL,
        "AverageMin" double precision NOT NULL,
        CONSTRAINT "PK_MndwiSharedReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "NdmiSeasonReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "SeasonId" uuid NOT NULL,
        CONSTRAINT "PK_NdmiSeasonReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "NdmiSharedReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "CropId" uuid,
        "AverageMax" double precision NOT NULL,
        "AverageMin" double precision NOT NULL,
        CONSTRAINT "PK_NdmiSharedReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "NdviSeasonReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "SeasonId" uuid NOT NULL,
        CONSTRAINT "PK_NdviSeasonReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "NdviSharedReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "CropId" uuid,
        "AverageMax" double precision NOT NULL,
        "AverageMin" double precision NOT NULL,
        CONSTRAINT "PK_NdviSharedReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "OrviSeasonReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "SeasonId" uuid NOT NULL,
        CONSTRAINT "PK_OrviSeasonReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "OrviSharedReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "CropId" uuid,
        "AverageMax" double precision NOT NULL,
        "AverageMin" double precision NOT NULL,
        CONSTRAINT "PK_OrviSharedReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "OsaviSeasonReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "SeasonId" uuid NOT NULL,
        CONSTRAINT "PK_OsaviSeasonReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    CREATE TABLE "OsaviSharedReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "CropId" uuid,
        "AverageMax" double precision NOT NULL,
        "AverageMin" double precision NOT NULL,
        CONSTRAINT "PK_OsaviSharedReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111644_Update_Database_Repots') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251015111644_Update_Database_Repots', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "ArviPoints";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "ArviPoints" (
                        "Id" uuid NOT NULL,
                        "BZero8" real NOT NULL,
                        "BZero4" real NOT NULL,
                        "BZero2" real NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Point" geometry NOT NULL,
                        "RegionId" uuid NULL,
                        "FieldId" uuid NULL,
                        "SeasonId" uuid NULL,
                        CONSTRAINT "PK_ArviPoints" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "EviPoints";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "EviPoints" (
                        "Id" uuid NOT NULL,
                        "BZero8" real NOT NULL,
                        "BZero4" real NOT NULL,
                        "BZero2" real NULL,
                        "EPS" real NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Point" geometry NOT NULL,
                        "RegionId" uuid NULL,
                        "FieldId" uuid NULL,
                        "SeasonId" uuid NULL,
                        CONSTRAINT "PK_EviPoints" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "GndviPoints";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "GndviPoints" (
                        "Id" uuid NOT NULL,
                        "BZero8" real NOT NULL,
                        "BZero3" real NOT NULL,
                        "EPS" real NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Point" geometry NOT NULL,
                        "RegionId" uuid NULL,
                        "FieldId" uuid NULL,
                        "SeasonId" uuid NULL,
                        CONSTRAINT "PK_GndviPoints" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "MndwiPoints";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "MndwiPoints" (
                        "Id" uuid NOT NULL,
                        "BZero3" real NOT NULL,
                        "B11" real NOT NULL,
                        "EPS" real NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Point" geometry NOT NULL,
                        "RegionId" uuid NULL,
                        "FieldId" uuid NULL,
                        "SeasonId" uuid NULL,
                        CONSTRAINT "PK_MndwiPoints" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "NdmiPoints";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "NdmiPoints" (
                        "Id" uuid NOT NULL,
                        "BZero8" real NOT NULL,
                        "B11" real NOT NULL,
                        "EPS" real NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Point" geometry NOT NULL,
                        "RegionId" uuid NULL,
                        "FieldId" uuid NULL,
                        "SeasonId" uuid NULL,
                        CONSTRAINT "PK_NdmiPoints" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "NdviPoints";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "NdviPoints" (
                        "Id" uuid NOT NULL,
                        "BZero8" real NOT NULL,
                        "BZero4" real NOT NULL,
                        "EPS" real NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Point" geometry NOT NULL,
                        "RegionId" uuid NULL,
                        "FieldId" uuid NULL,
                        "SeasonId" uuid NULL,
                        CONSTRAINT "PK_NdviPoints" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "OrviPoints";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "OrviPoints" (
                        "Id" uuid NOT NULL,
                        "BZero8" real NOT NULL,
                        "BZero3" real NOT NULL,
                        "EPS" real NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Point" geometry NOT NULL,
                        "RegionId" uuid NULL,
                        "FieldId" uuid NULL,
                        "SeasonId" uuid NULL,
                        CONSTRAINT "PK_OrviPoints" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "OsaviPoints";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "OsaviPoints" (
                        "Id" uuid NOT NULL,
                        "BZero8" real NOT NULL,
                        "BZero4" real NOT NULL,
                        "EPS" real NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Point" geometry NOT NULL,
                        "RegionId" uuid NULL,
                        "FieldId" uuid NULL,
                        "SeasonId" uuid NULL,
                        CONSTRAINT "PK_OsaviPoints" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "ArviPolygons";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "ArviPolygons" (
                        "Id" uuid NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Polygon" geometry NOT NULL,
                        "FileStorageItemId" uuid NOT NULL,
                        "IsColored" boolean NOT NULL,
                        CONSTRAINT "PK_ArviPolygons" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "EviPolygons";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "EviPolygons" (
                        "Id" uuid NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Polygon" geometry NOT NULL,
                        "FileStorageItemId" uuid NOT NULL,
                        "IsColored" boolean NOT NULL,
                        CONSTRAINT "PK_EviPolygons" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "GndviPolygons";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "GndviPolygons" (
                        "Id" uuid NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Polygon" geometry NOT NULL,
                        "FileStorageItemId" uuid NOT NULL,
                        "IsColored" boolean NOT NULL,
                        CONSTRAINT "PK_GndviPolygons" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "MndwiPolygons";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "MndwiPolygons" (
                        "Id" uuid NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Polygon" geometry NOT NULL,
                        "FileStorageItemId" uuid NOT NULL,
                        "IsColored" boolean NOT NULL,
                        CONSTRAINT "PK_MndwiPolygons" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "NdmiPolygons";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "NdmiPolygons" (
                        "Id" uuid NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Polygon" geometry NOT NULL,
                        "FileStorageItemId" uuid NOT NULL,
                        "IsColored" boolean NOT NULL,
                        CONSTRAINT "PK_NdmiPolygons" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "NdviPolygons";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "NdviPolygons" (
                        "Id" uuid NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Polygon" geometry NOT NULL,
                        "FileStorageItemId" uuid NOT NULL,
                        "IsColored" boolean NOT NULL,
                        CONSTRAINT "PK_NdviPolygons" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "OrviPolygons";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "OrviPolygons" (
                        "Id" uuid NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Polygon" geometry NOT NULL,
                        "FileStorageItemId" uuid NOT NULL,
                        "IsColored" boolean NOT NULL,
                        CONSTRAINT "PK_OrviPolygons" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    DROP TABLE "OsaviPolygons";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN

                    CREATE TABLE "OsaviPolygons" (
                        "Id" uuid NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Polygon" geometry NOT NULL,
                        "FileStorageItemId" uuid NOT NULL,
                        "IsColored" boolean NOT NULL,
                        CONSTRAINT "PK_OsaviPolygons" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251015111710_Update_Database_Partition') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251015111710_Update_Database_Partition', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125075232_PreviewPath_Added') THEN
    ALTER TABLE "OsaviPolygons" ADD "PreviewImagePath" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125075232_PreviewPath_Added') THEN
    ALTER TABLE "OrviPolygons" ADD "PreviewImagePath" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125075232_PreviewPath_Added') THEN
    ALTER TABLE "NdviPolygons" ADD "PreviewImagePath" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125075232_PreviewPath_Added') THEN
    ALTER TABLE "NdmiPolygons" ADD "PreviewImagePath" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125075232_PreviewPath_Added') THEN
    ALTER TABLE "MndwiPolygons" ADD "PreviewImagePath" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125075232_PreviewPath_Added') THEN
    ALTER TABLE "GndviPolygons" ADD "PreviewImagePath" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125075232_PreviewPath_Added') THEN
    ALTER TABLE "EviPolygons" ADD "PreviewImagePath" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125075232_PreviewPath_Added') THEN
    ALTER TABLE "ArviPolygons" ADD "PreviewImagePath" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125075232_PreviewPath_Added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251125075232_PreviewPath_Added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251213191113_Ndwi_and_Anomalies_added') THEN

                    CREATE TABLE IF NOT EXISTS "AnomalyPoints" (
                        "Id" uuid NOT NULL,
                        "AnomalyType" integer,
                        "Color" text,
                        "AnomalyPolygonId" uuid,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Point" geometry NOT NULL,
                        "RegionId" uuid,
                        "FieldId" uuid,
                        "SeasonId" uuid,
                        CONSTRAINT "PK_AnomalyPoints" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251213191113_Ndwi_and_Anomalies_added') THEN

                    CREATE TABLE IF NOT EXISTS "AnomalyPolygons" (
                        "Id" uuid NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "AnomalyType" integer NOT NULL,
                        "Color" text NOT NULL,
                        "RegionId" uuid,
                        "FieldId" uuid,
                        "SeasonId" uuid,
                        "Polygon" geometry NOT NULL,
                        CONSTRAINT "PK_AnomalyPolygons" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251213191113_Ndwi_and_Anomalies_added') THEN

                    CREATE TABLE IF NOT EXISTS "NdwiPoints" (
                        "Id" uuid NOT NULL,
                        "BZero3" real NOT NULL,
                        "BZero8" real NOT NULL,
                        "EPS" real NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Point" geometry NOT NULL,
                        "RegionId" uuid,
                        "FieldId" uuid,
                        "SeasonId" uuid,
                        CONSTRAINT "PK_NdwiPoints" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251213191113_Ndwi_and_Anomalies_added') THEN
    CREATE TABLE "NdwiPolygonRelations" (
        "Id" uuid NOT NULL,
        "NdwiId" uuid NOT NULL,
        "RegionId" uuid,
        "FieldId" uuid,
        "SeasonId" uuid,
        CONSTRAINT "PK_NdwiPolygonRelations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251213191113_Ndwi_and_Anomalies_added') THEN

                    CREATE TABLE IF NOT EXISTS "NdwiPolygons" (
                        "Id" uuid NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "ProductSourceType" integer NOT NULL,
                        "SatelliteProductId" uuid NOT NULL,
                        "Polygon" geometry NOT NULL,
                        "FileStorageItemId" uuid NOT NULL,
                        "IsColored" boolean NOT NULL,
                        "PreviewImagePath" uuid,
                        CONSTRAINT "PK_NdwiPolygons" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251213191113_Ndwi_and_Anomalies_added') THEN
    CREATE TABLE "NdwiSeasonReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "SeasonId" uuid NOT NULL,
        CONSTRAINT "PK_NdwiSeasonReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251213191113_Ndwi_and_Anomalies_added') THEN
    CREATE TABLE "NdwiSharedReports" (
        "Id" uuid NOT NULL,
        "DateOnly" date NOT NULL,
        "Average" double precision NOT NULL,
        "TotalMax" double precision NOT NULL,
        "TotalMin" double precision NOT NULL,
        "RegionId" uuid,
        "CropId" uuid,
        "AverageMax" double precision NOT NULL,
        "AverageMin" double precision NOT NULL,
        CONSTRAINT "PK_NdwiSharedReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251213191113_Ndwi_and_Anomalies_added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251213191113_Ndwi_and_Anomalies_added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218090721_AnomalyPolygon_To_Base') THEN
    ALTER TABLE "AnomalyPolygons" ADD "CreateUser" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218090721_AnomalyPolygon_To_Base') THEN
    ALTER TABLE "AnomalyPolygons" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218090721_AnomalyPolygon_To_Base') THEN
    ALTER TABLE "AnomalyPolygons" ADD "LastUpdateAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218090721_AnomalyPolygon_To_Base') THEN
    ALTER TABLE "AnomalyPolygons" ADD "LastUpdateUser" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218090721_AnomalyPolygon_To_Base') THEN
    ALTER TABLE "AnomalyPolygons" ADD "OriginalDate" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218090721_AnomalyPolygon_To_Base') THEN
    ALTER TABLE "AnomalyPolygons" ADD "Status" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218090721_AnomalyPolygon_To_Base') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251218090721_AnomalyPolygon_To_Base', '9.0.7');
    END IF;
END $EF$;
COMMIT;

