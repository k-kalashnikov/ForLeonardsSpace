CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910125815_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910125815_Init_Database') THEN

                    CREATE TABLE IF NOT EXISTS "EraWeatherData" (
                        "Id" uuid NOT NULL,
                        "OriginalDateTimeUtc" timestamp with time zone,
                        "Temperature" double precision,
                        "RelativeHumidity" double precision,
                        "DewPoint" double precision,
                        "Precipitation" double precision,
                        "CloudCover" double precision,
                        "WindSpeed" double precision,
                        "WindDirection" double precision,
                        "GroundTemperature" double precision,
                        "SoilTemperature" double precision,
                        "ConditionIds" integer,
                        "SoilHumidity50cm" double precision,
                        "SoilHumidity2m" double precision,
                        "EraWeatherStationId" uuid NOT NULL,
                        CONSTRAINT "PK_EraWeatherData" PRIMARY KEY ("Id", "OriginalDateTimeUtc")
                    ) PARTITION BY RANGE ("OriginalDateTimeUtc");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910125815_Init_Database') THEN

                    ALTER TABLE "EraWeatherData"
                    ADD CONSTRAINT "uq_EraWeatherData_datetime_station"
                    UNIQUE ("OriginalDateTimeUtc", "EraWeatherStationId");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910125815_Init_Database') THEN
    CREATE TABLE "EraWeatherStations" (
        "Id" uuid NOT NULL,
        "Point" geometry NOT NULL,
        "RegionId" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_EraWeatherStations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910125815_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250910125815_Init_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922141044_Added_Field') THEN
    ALTER TABLE "EraWeatherStations" ADD "FieldId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922141044_Added_Field') THEN
    ALTER TABLE "EraWeatherData" ADD "SolarRadiation" double precision;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922141044_Added_Field') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250922141044_Added_Field', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005162057_Update_Database_Add_Reports') THEN
    CREATE TABLE "Era5DayWeatherForecasts" (
        "Id" uuid NOT NULL,
        "Date" date NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "SolarRadiationInfluence" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_Era5DayWeatherForecasts" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005162057_Update_Database_Add_Reports') THEN
    CREATE TABLE "Era5DayWeatherReports" (
        "Id" uuid NOT NULL,
        "Date" date NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "SolarRadiationInfluence" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_Era5DayWeatherReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005162057_Update_Database_Add_Reports') THEN
    CREATE TABLE "Era5HourWeatherForecasts" (
        "Id" uuid NOT NULL,
        "Date" date NOT NULL,
        "Hour" integer NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "SolarRadiationInfluence" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_Era5HourWeatherForecasts" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005162057_Update_Database_Add_Reports') THEN
    CREATE TABLE "Era5HourWeatherReports" (
        "Id" uuid NOT NULL,
        "Date" date NOT NULL,
        "Hour" integer NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "SolarRadiationInfluence" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_Era5HourWeatherReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005162057_Update_Database_Add_Reports') THEN
    CREATE TABLE "Era5MonthWeatherReports" (
        "Id" uuid NOT NULL,
        "Year" integer NOT NULL,
        "Month" integer NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "SolarRadiationInfluence" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_Era5MonthWeatherReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005162057_Update_Database_Add_Reports') THEN
    CREATE TABLE "Era5WeekWeatherReports" (
        "Id" uuid NOT NULL,
        "WeekStart" date NOT NULL,
        "WeekEnd" date NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "SolarRadiationInfluence" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_Era5WeekWeatherReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005162057_Update_Database_Add_Reports') THEN
    CREATE TABLE "Era5YearWeatherReports" (
        "Id" uuid NOT NULL,
        "Year" integer NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "SolarRadiationInfluence" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_Era5YearWeatherReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005162057_Update_Database_Add_Reports') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251005162057_Update_Database_Add_Reports', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005162417_Update_Database_Hours') THEN
    DROP TABLE "Era5HourWeatherForecasts";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005162417_Update_Database_Hours') THEN
    DROP TABLE "Era5HourWeatherReports";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005162417_Update_Database_Hours') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251005162417_Update_Database_Hours', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021100417_Normalized_Day_added') THEN
    CREATE TABLE "Era5DayNormalizedWeather" (
        "Id" uuid NOT NULL,
        "Month" integer NOT NULL,
        "Day" integer NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "SolarRadiationInfluence" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_Era5DayNormalizedWeather" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021100417_Normalized_Day_added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251021100417_Normalized_Day_added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021102816_IsFrostDanger_Added') THEN
    ALTER TABLE "Era5DayWeatherReports" ADD "IsFrostDanger" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021102816_IsFrostDanger_Added') THEN
    ALTER TABLE "Era5DayWeatherForecasts" ADD "IsFrostDanger" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021102816_IsFrostDanger_Added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251021102816_IsFrostDanger_Added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121027_Update_Database_IsPublic') THEN
    ALTER TABLE "EraWeatherStations" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024121027_Update_Database_IsPublic') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251024121027_Update_Database_IsPublic', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218083002_Add_AgroClimaticZone_Relation') THEN
    ALTER TABLE "EraWeatherStations" ADD "AgroclimaticZoneId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218083002_Add_AgroClimaticZone_Relation') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251218083002_Add_AgroClimaticZone_Relation', '9.0.7');
    END IF;
END $EF$;
COMMIT;

