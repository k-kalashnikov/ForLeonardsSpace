CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922122614_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922122614_Init_Database') THEN
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
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922122614_Init_Database') THEN
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
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922122614_Init_Database') THEN
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
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922122614_Init_Database') THEN
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
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922122614_Init_Database') THEN
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
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922122614_Init_Database') THEN
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
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922122614_Init_Database') THEN
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
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922122614_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250922122614_Init_Database', '9.0.7');
    END IF;
END $EF$;
COMMIT;

