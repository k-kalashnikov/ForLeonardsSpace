CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251013140145_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251013140145_Init_Database') THEN

                    CREATE TABLE IF NOT EXISTS "UgmWeatherData" (
                        "Id" uuid NOT NULL,
                        "RegionId" integer NOT NULL,
                        "Date" date,
                        "DateTime" timestamp with time zone,
                        "DayPart" integer,
                        "Icon" text,
                        "AirTMin" integer,
                        "AirTMax" integer,
                        "WindDirection" integer,
                        "WindDirectionChange" integer,
                        "WindSpeedMin" integer,
                        "WindSpeedMax" integer,
                        "WindSpeedMinAfterChange" integer,
                        "WindSpeedMaxAfterChange" integer,
                        "CloudAmount" text,
                        "TimePeriod" text,
                        "Precipitation" text,
                        "IsOccasional" integer,
                        "IsPossible" integer,
                        "Thunderstorm" integer,
                        "Location" text,
                        "WeatherCode" text,
                        CONSTRAINT "PK_UgmWeatherData" PRIMARY KEY ("Id", "Date")
                    ) PARTITION BY RANGE ("Date");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251013140145_Init_Database') THEN

                    ALTER TABLE "UgmWeatherData"
                    ADD CONSTRAINT "uq_UgmWeatherData_date_station_dayPart"
                    UNIQUE ("Date", "RegionId", "DayPart");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251013140145_Init_Database') THEN
    CREATE TABLE "UgmWeatherStations" (
        "Id" uuid NOT NULL,
        "UgmRegionId" integer NOT NULL,
        "Name" text,
        "IsRegionalCenter" boolean,
        "Latitude" double precision,
        "Longitude" double precision,
        "Title" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_UgmWeatherStations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251013140145_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251013140145_Init_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121544_Update_Database_IsPublic') THEN
    ALTER TABLE "UgmWeatherStations" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121544_Update_Database_IsPublic') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251024121544_Update_Database_IsPublic', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218082017_Add_AgroClimaticZone_Relation') THEN
    ALTER TABLE "UgmWeatherStations" ADD "AgroclimaticZoneId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218082017_Add_AgroClimaticZone_Relation') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251218082017_Add_AgroClimaticZone_Relation', '9.0.7');
    END IF;
END $EF$;
COMMIT;

