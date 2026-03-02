CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE "AgroClimaticZoneMonthNorms" (
        "Id" uuid NOT NULL,
        "ProviderId" uuid,
        "AgroClimaticZoneId" uuid,
        "M" integer,
        "TemperatureAvgNorm" double precision,
        "PrecipitationAvgNorm" double precision,
        "SolarRadiationAvgNorm" double precision,
        "TemperatureMedNorm" double precision,
        "PrecipitationMedNorm" double precision,
        "SolarRadiationMedNorm" double precision,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_AgroClimaticZoneMonthNorms" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE "AgroClimaticZoneNorms" (
        "Id" uuid NOT NULL,
        "ProviderId" uuid,
        "AgroClimaticZoneId" uuid,
        "M" integer,
        "D" integer,
        "TemperatureAvgNorm" double precision,
        "PrecipitationAvgNorm" double precision,
        "TemperatureMedNorm" double precision,
        "PrecipitationMedNorm" double precision,
        "SolarRadiationAvgNorm" double precision,
        "SolarRadiationMedNorm" double precision,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_AgroClimaticZoneNorms" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE "AgroClimaticZones" (
        "Id" uuid NOT NULL,
        "Name" text,
        "Polygon" text NOT NULL,
        "PolygonGeom" geometry,
        "Active" boolean,
        "NameEn" text,
        "NameUz" text,
        CONSTRAINT "PK_AgroClimaticZones" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "AgroClimaticZonesWeatherRates" ("Id" uuid NOT NULL, "Date" timestamp with time zone NOT NULL, "TempRate" double precision, "PrecRate" double precision, CONSTRAINT "PK_AgroclimaticZonesWeatherRates" PRIMARY KEY ("Id", "Date")) PARTITION BY RANGE ("Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "Alerts" ("Id" uuid NOT NULL, "RegionId" uuid NOT NULL, "TypeId" integer, "ProviderId" uuid NOT NULL, "Date" timestamp with time zone NOT NULL, "Value" double precision, "AgroClimaticZonesId" uuid, CONSTRAINT "PK_Alerts" PRIMARY KEY ("Id", "Date")) PARTITION BY RANGE ("Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE "ApplicationPropertys" (
        "Id" uuid NOT NULL,
        "Application" text NOT NULL,
        "Key" text NOT NULL,
        "Active" boolean,
        "Value" text,
        "Description" text,
        CONSTRAINT "PK_ApplicationPropertys" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "Jobs" ("Id" uuid NOT NULL, "Date" timestamp with time zone NOT NULL, "Action" text, "ProviderId" uuid, "JobStatusId" uuid, "Application" text, "Result" text, "Path" text, CONSTRAINT "PK_Jobs" PRIMARY KEY ("Id", "Date")) PARTITION BY RANGE ("Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "Logs" ("Id" uuid NOT NULL, "Date" timestamp with time zone NOT NULL, "JobId" uuid, "JobStatus" text, "ProviderId" uuid, "Details" text, "ContentSize" double precision, "UserInfo" text, CONSTRAINT "PK_Logs" PRIMARY KEY ("Id", "Date")) PARTITION BY RANGE ("Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "Regions" ("Id" uuid NOT NULL, "RegionName" text, "Iso" text, "RegionLevel" integer, "Lat" double precision, "Lon" double precision, "RowX" integer, "ColumnY" integer, "Polygon" text NOT NULL, "PolygonGeom" geometry, "ParentId" uuid, "Active" boolean, "RegionNameEn" text, "RegionNameUz" text, "Mhobt" text, "UpdateDate" timestamp with time zone NOT NULL, CONSTRAINT "PK_Regions" PRIMARY KEY ("Id", "UpdateDate")) PARTITION BY RANGE ("UpdateDate");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE "RegionsAgroClimaticZones" (
        "RegionId" uuid NOT NULL,
        "AgroClimaticZonesId" uuid NOT NULL,
        CONSTRAINT "PK_RegionsAgroClimaticZones" PRIMARY KEY ("RegionId")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE "RegionsDumps" (
        "Id" uuid NOT NULL,
        "RegionName" text,
        "Iso" text,
        "RegionLevel" integer,
        "Lat" double precision,
        "Lon" double precision,
        "RowX" integer,
        "ColumnY" integer,
        "Polygon" text NOT NULL,
        "PolygonGeom" geometry,
        "ParentId" uuid,
        "Active" boolean,
        "Mhobt" text,
        CONSTRAINT "PK_RegionsDumps" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "RegionsWeathers" ("Id" uuid NOT NULL, "RegionId" uuid NOT NULL, "ProviderId" uuid NOT NULL, "Date" timestamp with time zone NOT NULL, "Temp" double precision, "Precipitation" double precision, "TempDev" double precision, "PrecDev" double precision, CONSTRAINT "PK_RegionsWeathers" PRIMARY KEY ("Id", "Date")) PARTITION BY RANGE ("Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "Reports" ("Id" uuid NOT NULL, "ReportType" uuid, "Link" text, "Name" text, "UpdateDate" timestamp with time zone, "Description" text, "SourceQuery" text, CONSTRAINT "PK_Reports" PRIMARY KEY ("Id", "UpdateDate")) PARTITION BY RANGE ("UpdateDate");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE "WeatherStationAgroClimaticZones" (
        "Id" uuid NOT NULL,
        "WeatherStationId" uuid NOT NULL,
        "AgroClimaticZonesId" uuid NOT NULL,
        CONSTRAINT "PK_WeatherStationAgroClimaticZones" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE "WeatherStations" (
        "Id" uuid NOT NULL,
        "Name" text,
        "ProviderId" uuid,
        "Lat" double precision,
        "Lon" double precision,
        "X" integer,
        "Y" integer,
        "Application" text,
        "Code" text,
        CONSTRAINT "PK_WeatherStations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "WeatherStationsDataEx" ("Id" uuid NOT NULL, "TemperatureSoil" double precision, "TemperatureGroundLevel" double precision, "Temperature1mAbove" double precision, "Temperature2mUnder" double precision, "HumiditySoil50cm" double precision, "HumiditySoil2m" double precision, "Temp10cmUnder" double precision, "Temp30100cm" double precision, "Temp1030cm" double precision, CONSTRAINT "PK_WeatherStationsDataEx" PRIMARY KEY ("Id"));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE IF NOT EXISTS "WeatherStationsDatum" ("Id" uuid NOT NULL, "WeatherStationId" uuid, "Date" timestamp with time zone NOT NULL, "Temperature" double precision, "TemperatureMax" double precision, "TemperatureMin" double precision, "Precipitation" double precision, "WindSpeed" double precision, "WindSpeedMin" double precision, "WindSpeedMax" double precision, "Windchill" double precision, "CloudCover" double precision, "RelativeHumidity" double precision, "ConditionCode" integer, "SolarRadiation" double precision, "DewPoint" double precision, "HumidityMin" double precision, "HumidityMax" double precision, "WindDirection" double precision, CONSTRAINT "PK_WeatherStationsDatum" PRIMARY KEY ("Id", "Date")) PARTITION BY RANGE ("Date");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    CREATE TABLE "XslsUzUnputColumns" (
        "Id" uuid NOT NULL,
        "XlsColumnName" text,
        "DbTableName" text,
        "DbColumnName" text,
        CONSTRAINT "PK_XslsUzUnputColumns" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124512_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250818124512_Init_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250819121720_Update_Database') THEN
    ALTER TABLE "Regions" RENAME COLUMN "RegionName" TO "RegionNameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250819121720_Update_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250819121720_Update_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922131906_Update_Database1') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250922131906_Update_Database1', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121706_Update_Database_IsPublic') THEN
    ALTER TABLE "AgroClimaticZoneNorms" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121706_Update_Database_IsPublic') THEN
    ALTER TABLE "AgroClimaticZoneMonthNorms" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121706_Update_Database_IsPublic') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251024121706_Update_Database_IsPublic', '9.0.7');
    END IF;
END $EF$;
COMMIT;

