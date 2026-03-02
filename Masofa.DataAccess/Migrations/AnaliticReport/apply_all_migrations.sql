CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021120937_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021120937_Init_Database') THEN
    CREATE TABLE "FarmerRecomendationReports" (
        "Id" uuid NOT NULL,
        "FileStorageItemId" uuid NOT NULL,
        "FieldId" uuid NOT NULL,
        "SeasonId" uuid NOT NULL,
        "HeaderJson" text NOT NULL,
        "SoilJson" text NOT NULL,
        "CalendarJson" text NOT NULL,
        "IrrigationJson" text NOT NULL,
        "WeatherJson" text NOT NULL,
        "MonitoringJson" text NOT NULL,
        "FertilizationJson" text NOT NULL,
        "GrowthStagesJson" text NOT NULL,
        "IndicesJson" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_FarmerRecomendationReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021120937_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251021120937_Init_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021172829_Add_Database_Season_Fields') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251021172829_Add_Database_Season_Fields', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021172942_Add_Database_Season_Fields_1') THEN
    ALTER TABLE "FarmerRecomendationReports" ADD "FieldJson" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021172942_Add_Database_Season_Fields_1') THEN
    ALTER TABLE "FarmerRecomendationReports" ADD "SeasonJson" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021172942_Add_Database_Season_Fields_1') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251021172942_Add_Database_Season_Fields_1', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120519_Update_Database_IsPublic') THEN
    ALTER TABLE "FarmerRecomendationReports" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120519_Update_Database_IsPublic') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251024120519_Update_Database_IsPublic', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251124081801_Add_FarmerReport') THEN
    CREATE TABLE "FarmerReports" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "ReportBodyJson" text,
        "Point" integer,
        "Comment" text,
        CONSTRAINT "PK_FarmerReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251124081801_Add_FarmerReport') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251124081801_Add_FarmerReport', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ALTER COLUMN "WeatherJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ALTER COLUMN "SoilJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ALTER COLUMN "SeasonJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ALTER COLUMN "MonitoringJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ALTER COLUMN "IrrigationJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ALTER COLUMN "IndicesJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ALTER COLUMN "HeaderJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ALTER COLUMN "GrowthStagesJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ALTER COLUMN "FieldJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ALTER COLUMN "FertilizationJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ALTER COLUMN "CalendarJson" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ADD "BidResultsJson" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ADD "ClimaticSummJson" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ADD "FertPestJson" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    ALTER TABLE "FarmerRecomendationReports" ADD "QwenResult" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224104824_Add_NewJsons') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251224104824_Add_NewJsons', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251226104350_Update_Database1') THEN
    ALTER TABLE "FarmerRecomendationReports" ADD "LocalizationFile" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251226104350_Update_Database1') THEN
    ALTER TABLE "FarmerRecomendationReports" ADD "QwenJobId" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251226104350_Update_Database1') THEN
    ALTER TABLE "FarmerRecomendationReports" ADD "ReportState" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251226104350_Update_Database1') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251226104350_Update_Database1', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260107070415_Update_FarmerRecomendationReport') THEN
    ALTER TABLE "FarmerRecomendationReports" RENAME COLUMN "QwenResult" TO "QwenJobResultJson";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260107070415_Update_FarmerRecomendationReport') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260107070415_Update_FarmerRecomendationReport', '9.0.7');
    END IF;
END $EF$;
COMMIT;

