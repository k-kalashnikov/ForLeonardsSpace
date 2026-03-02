CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023074942_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023074942_Init_Database') THEN
    CREATE TABLE "IBMMeteoStations" (
        "Id" uuid NOT NULL,
        "RegionId" uuid,
        "Point" geometry NOT NULL,
        "City" text,
        "Country" text,
        "CountryCode" text,
        "DisplayName" text,
        "AdminDistrict" text,
        "AdminDistrictCode" text,
        "IataCode" text,
        "IcaoCode" text,
        "PwsId" text,
        "LocId" text,
        "PlaceId" text,
        "PostalKey" text,
        "Type" text,
        "IsActive" boolean NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Names" text NOT NULL,
        CONSTRAINT "PK_IBMMeteoStations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023074942_Init_Database') THEN
    CREATE TABLE "IBMWeatherAlertFloodInfos" (
        "Id" uuid NOT NULL,
        "WeatherAlertId" uuid NOT NULL,
        "FloodCrestTimeLocal" timestamp with time zone,
        "FloodCrestTimeLocalTimeZone" text,
        "FloodEndTimeLocal" timestamp with time zone,
        "FloodEndTimeLocalTimeZone" text,
        "FloodImmediateCause" text,
        "FloodImmediateCauseCode" text,
        "FloodLocationId" text,
        "FloodLocationName" text,
        "FloodRecordStatus" text,
        "FloodRecordStatusCode" text,
        "FloodSeverity" text,
        "FloodSeverityCode" text,
        "FloodStartTimeLocal" timestamp with time zone,
        "FloodStartTimeLocalTimeZone" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_IBMWeatherAlertFloodInfos" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023074942_Init_Database') THEN
    CREATE TABLE "IBMWeatherAlerts" (
        "Id" uuid NOT NULL,
        "IBMMeteoStationId" uuid NOT NULL,
        "AdminDistrict" text,
        "AdminDistrictCode" text,
        "AreaId" text NOT NULL,
        "AreaName" text NOT NULL,
        "AreaTypeCode" text NOT NULL,
        "Certainty" text NOT NULL,
        "CertaintyCode" text NOT NULL,
        "CountryCode" text NOT NULL,
        "CountryName" text NOT NULL,
        "DetailKey" text NOT NULL,
        "Disclaimer" text,
        "DisplayRank" integer NOT NULL,
        "EffectiveTimeLocal" timestamp with time zone,
        "EffectiveTimeLocalTimeZone" text,
        "EventDescription" text NOT NULL,
        "EventTrackingNumber" text NOT NULL,
        "ExpireTimeLocal" timestamp with time zone NOT NULL,
        "ExpireTimeLocalTimeZone" text NOT NULL,
        "ExpireTimeUTC" timestamp with time zone NOT NULL,
        "HeadlineText" text NOT NULL,
        "IanaTimeZone" text,
        "Identifier" text NOT NULL,
        "IssueTimeLocal" timestamp with time zone NOT NULL,
        "IssueTimeLocalTimeZone" text NOT NULL,
        "Latitude" double precision,
        "Longitude" double precision,
        "MessageType" text NOT NULL,
        "MessageTypeCode" integer NOT NULL,
        "OfficeAdminDistrict" text,
        "OfficeAdminDistrictCode" text,
        "OfficeCode" text NOT NULL,
        "OfficeCountryCode" text,
        "OfficeName" text NOT NULL,
        "OnsetTimeLocal" timestamp with time zone,
        "OnsetTimeLocalTimeZone" text,
        "Phenomena" text NOT NULL,
        "ProcessTimeUTC" timestamp with time zone NOT NULL,
        "ProductIdentifier" text NOT NULL,
        "Severity" text NOT NULL,
        "SeverityCode" integer NOT NULL,
        "Significance" text NOT NULL,
        "Source" text NOT NULL,
        "Urgency" text NOT NULL,
        "UrgencyCode" integer NOT NULL,
        "EndTimeLocal" timestamp with time zone,
        "EndTimeLocalTimeZone" text,
        "EndTimeUTC" timestamp with time zone,
        "WeatherAlertResponseType" integer NOT NULL,
        "WeatherAlertCategory" integer NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Names" text NOT NULL,
        CONSTRAINT "PK_IBMWeatherAlerts" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023074942_Init_Database') THEN

                    CREATE TABLE IF NOT EXISTS "IBMWeatherData" (
                        "Id" uuid NOT NULL,
                        "IBMMeteoStationId" uuid NOT NULL,
                        "ValidTimeUtc" timestamp with time zone NOT NULL,
                        "Temperature" integer,
                        "Humidity" integer,
                        "WindSpeed" integer,
                        "WindDirection" integer,
                        "Precipitation" double precision,
                        "UvIndex" integer,
                        "TemperatureMax" integer,
                        "TemperatureMin" integer,
                        "DayOrNight" text,
                        "PrecipChance" integer,
                        "Qpf" double precision,
                        "QpfSnow" double precision,
                        "RelativeHumidity" integer,
                        "DayOfWeek" text,
                        "RequestedLatitude" double precision,
                        "RequestedLongitude" double precision,
                        "GridpointId" text,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "Status" integer NOT NULL,
                        "LastUpdateAt" timestamp with time zone NOT NULL,
                        "CreateUser" uuid NOT NULL,
                        "LastUpdateUser" uuid NOT NULL,
                        CONSTRAINT "PK_IBMWeatherData" PRIMARY KEY ("Id", "ValidTimeUtc")
                    ) PARTITION BY RANGE ("ValidTimeUtc");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023074942_Init_Database') THEN

                    ALTER TABLE "IBMWeatherData"
                    ADD CONSTRAINT "uq_IBMWeatherData_datetime_station_dayPart"
                    UNIQUE ("ValidTimeUtc", "IBMMeteoStationId", "DayOrNight");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023074942_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251023074942_Init_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023085346_Report_added') THEN
    CREATE TABLE "IbmDayWeatherForecasts" (
        "Id" uuid NOT NULL,
        "Date" date NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_IbmDayWeatherForecasts" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023085346_Report_added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251023085346_Report_added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023123302_Frost_Danger_added') THEN
    ALTER TABLE "IbmDayWeatherForecasts" ADD "IsFrostDanger" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023123302_Frost_Danger_added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251023123302_Frost_Danger_added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251118141119_IsPublic_Update') THEN
    ALTER TABLE "IBMWeatherData" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251118141119_IsPublic_Update') THEN
    ALTER TABLE "IBMWeatherAlerts" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251118141119_IsPublic_Update') THEN
    ALTER TABLE "IBMWeatherAlertFloodInfos" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251118141119_IsPublic_Update') THEN
    ALTER TABLE "IBMMeteoStations" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251118141119_IsPublic_Update') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251118141119_IsPublic_Update', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204115915_Update_Database_Add_Reports') THEN
    CREATE TABLE "IbmDayNormalizedWeathers" (
        "Id" uuid NOT NULL,
        "Month" integer NOT NULL,
        "Day" integer NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_IbmDayNormalizedWeathers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204115915_Update_Database_Add_Reports') THEN
    CREATE TABLE "IbmDayWeatherReports" (
        "Id" uuid NOT NULL,
        "Date" date NOT NULL,
        "IsFrostDanger" boolean NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_IbmDayWeatherReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204115915_Update_Database_Add_Reports') THEN
    CREATE TABLE "IbmMonthWeatherReports" (
        "Id" uuid NOT NULL,
        "Date" date NOT NULL,
        "IsFrostDanger" boolean NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_IbmMonthWeatherReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204115915_Update_Database_Add_Reports') THEN
    CREATE TABLE "IbmWeekWeatherReports" (
        "Id" uuid NOT NULL,
        "Date" date NOT NULL,
        "IsFrostDanger" boolean NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_IbmWeekWeatherReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204115915_Update_Database_Add_Reports') THEN
    CREATE TABLE "IbmYearWeatherReports" (
        "Id" uuid NOT NULL,
        "Date" date NOT NULL,
        "IsFrostDanger" boolean NOT NULL,
        "TemperatureMin" double precision NOT NULL,
        "TemperatureMax" double precision NOT NULL,
        "TemperatureMinTotal" double precision NOT NULL,
        "TemperatureMaxTotal" double precision NOT NULL,
        "Fallout" double precision NOT NULL,
        "Humidity" double precision NOT NULL,
        "WindSpeed" double precision NOT NULL,
        "WindDerection" double precision NOT NULL,
        "WeatherStation" uuid,
        CONSTRAINT "PK_IbmYearWeatherReports" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251204115915_Update_Database_Add_Reports') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251204115915_Update_Database_Add_Reports', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218082707_Add_AgroClimaticZone_Relation') THEN
    ALTER TABLE "IBMMeteoStations" ADD "AgroclimaticZoneId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218082707_Add_AgroClimaticZone_Relation') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251218082707_Add_AgroClimaticZone_Relation', '9.0.7');
    END IF;
END $EF$;
COMMIT;

