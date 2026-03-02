CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "AdministrativeUnits" (
        "Id" uuid NOT NULL,
        "Level" integer,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_AdministrativeUnits" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "AgroclimaticZones" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_AgroclimaticZones" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "AgroMachineTypes" (
        "Id" uuid NOT NULL,
        "IsSoilSafe" boolean,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_AgroMachineTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "AgroOperations" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_AgroOperations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "AgrotechnicalMeasures" (
        "Id" uuid NOT NULL,
        "CropId" uuid,
        "VarietyId" uuid,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_AgrotechnicalMeasures" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "AgroTerms" (
        "Id" uuid NOT NULL,
        "DescrEn" text,
        "DescrRu" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_AgroTerms" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "BidContents" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_BidContents" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "BidStates" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_BidStates" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "BidTypes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_BidTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "BusinessTypes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_BusinessTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "ClimaticStandards" (
        "Id" uuid NOT NULL,
        "RegionId" uuid,
        "Month" integer,
        "Day" integer,
        "TempAvg" numeric,
        "TempMin" numeric,
        "TempMax" numeric,
        "PrecDayAvg" numeric,
        "RadDayAvg" numeric,
        "HumAvg" numeric,
        "CoefSel" numeric,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        CONSTRAINT "PK_ClimaticStandards" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "CropPeriods" (
        "Id" uuid NOT NULL,
        "RegionId" uuid,
        "CropId" uuid,
        "VarietyId" uuid,
        "DayStart" integer,
        "DayEnd" integer,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_CropPeriods" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "CropPeriodVegetationIndexes" (
        "Id" uuid NOT NULL,
        "CropPeriodId" uuid NOT NULL,
        "VegetationIndexId" uuid NOT NULL,
        "Value" numeric,
        "Min" numeric,
        "Max" numeric,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        CONSTRAINT "PK_CropPeriodVegetationIndexes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "Crops" (
        "Id" uuid NOT NULL,
        "NameLa" text,
        "IsMonitoring" boolean,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_Crops" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "DicitonaryTypes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_DicitonaryTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "Diseases" (
        "Id" uuid NOT NULL,
        "CropId" uuid,
        "VarietyId" uuid,
        "NameLa" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_Diseases" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "EntomophageTypes" (
        "Id" uuid NOT NULL,
        "NameLa" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_EntomophageTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "ExperimentalFarmingMethods" (
        "Id" uuid NOT NULL,
        "Name" text,
        "CropId" uuid,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        CONSTRAINT "PK_ExperimentalFarmingMethods" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "Fertilizers" (
        "Id" uuid NOT NULL,
        "IsEco" boolean,
        "IsOrganic" boolean,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_Fertilizers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "FertilizerTypes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_FertilizerTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "FieldUsageStatuses" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_FieldUsageStatuses" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "Firms" (
        "Id" uuid NOT NULL,
        "Inn" text,
        "Egrpo" text,
        "Site" text,
        "Phones" text,
        "Email" text,
        "Chief" text,
        "MainRegionId" uuid,
        "PhysicalAddress" text,
        "MailingAddress" text,
        "ShortName" text NOT NULL,
        "FullName" text,
        "InternationalName" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_Firms" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "FlightTargets" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_FlightTargets" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "IrrigationMethods" (
        "Id" uuid NOT NULL,
        "IsWaterSafe" boolean,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_IrrigationMethods" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "IrrigationSources" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_IrrigationSources" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "MeasurementUnits" (
        "Id" uuid NOT NULL,
        "FullNameEn" text,
        "FullNameRu" text,
        "SiUnit" text,
        "Factor" numeric,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_MeasurementUnits" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "MeliorativeMeasureTypes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_MeliorativeMeasureTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "Persons" (
        "Id" uuid NOT NULL,
        "Pinfl" text,
        "UserId" uuid,
        "FullName" text,
        "Telegram" text,
        "Phones" text,
        "Email" text,
        "MainRegionId" uuid,
        "PhysicalAddress" text,
        "MailingAddress" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_Persons" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "Pesticides" (
        "Id" uuid NOT NULL,
        "PesticideTypeId" uuid,
        "IntCode" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_Pesticides" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "PesticideTypes" (
        "Id" uuid NOT NULL,
        "IntCode" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_PesticideTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "PestTypes" (
        "Id" uuid NOT NULL,
        "NameLa" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_PestTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "ProductQualityStandards" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_ProductQualityStandards" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "ProviderWeatherConditions" (
        "Id" uuid NOT NULL,
        "RegionId" uuid,
        "Lat" numeric,
        "Lng" numeric,
        "Radius" numeric,
        "WeatherStationTypeId" uuid,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_ProviderWeatherConditions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "RegionMaps" (
        "Id" uuid NOT NULL,
        "Lat" numeric,
        "Lng" numeric,
        "MozaikX" integer,
        "MozaikY" integer,
        "PolygonJson" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        CONSTRAINT "PK_RegionMaps" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "Regions" (
        "Id" uuid NOT NULL,
        "ParentId" uuid,
        "Level" integer,
        "NameMhobt" text,
        "ShortNameEn" text,
        "ShortNameRu" text,
        "ActiveFrom" timestamp with time zone,
        "ActiveTo" timestamp with time zone,
        "RegionSquare" numeric,
        "NameAdminUz" text,
        "NameAdminEn" text,
        "NameAdminRu" text,
        "Population" numeric,
        "AgroclimaticZoneId" uuid,
        "RegionMapId" uuid,
        "RegionTypeId" uuid,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_Regions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "RegionTypes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_RegionTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "SoilTypes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_SoilTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "SolarRadiationInfluences" (
        "Id" uuid NOT NULL,
        "RegionId" uuid,
        "CropId" uuid,
        "VarietyId" uuid,
        "DayStart" integer,
        "DayEnd" integer,
        "RadNorm" numeric,
        "VegetationPeriodId" uuid,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        CONSTRAINT "PK_SolarRadiationInfluences" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "SystemDataSources" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_SystemDataSources" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "Tags" (
        "Id" uuid NOT NULL,
        "Name" text NOT NULL,
        "Description" text,
        "OwnerId" uuid NOT NULL,
        "OwnerTypeFullName" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        CONSTRAINT "PK_Tags" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "TaskStatuses" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_TaskStatuses" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "UavCameraTypes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_UavCameraTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "UavDataTypes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_UavDataTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "Varieties" (
        "Id" uuid NOT NULL,
        "CropId" uuid NOT NULL,
        "NameLa" text,
        "RipeningPeriod" integer,
        "AverageYield" numeric,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_Varieties" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "VarietyFeatures" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_VarietyFeatures" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "VegetationIndexes" (
        "Id" uuid NOT NULL,
        "Name" text,
        "DescriptionEn" text,
        "DescriptionRu" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        CONSTRAINT "PK_VegetationIndexes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "VegetationPeriods" (
        "Id" uuid NOT NULL,
        "RegionId" uuid,
        "VarietyId" uuid,
        "ClassId" uuid,
        "DayStart" integer,
        "DayEnd" integer,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_VegetationPeriods" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "WaterResources" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_WaterResources" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "WeatherAlertTypes" (
        "Id" uuid NOT NULL,
        "Type" integer,
        "Value" double precision,
        "DescriptionEn" text,
        "DescriptionRu" text,
        "FieldName" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "NameRu" text NOT NULL,
        "NameEn" text,
        CONSTRAINT "PK_WeatherAlertTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "WeatherConditions" (
        "Id" uuid NOT NULL,
        "ProviderId" uuid,
        "ProviderCode" integer,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "NameRu" text NOT NULL,
        "NameEn" text,
        CONSTRAINT "PK_WeatherConditions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "WeatherFrequencies" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "NameRu" text NOT NULL,
        "NameEn" text,
        CONSTRAINT "PK_WeatherFrequencies" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "WeatherImageTypes" (
        "Id" uuid NOT NULL,
        "Code" integer,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "NameRu" text NOT NULL,
        "NameEn" text,
        CONSTRAINT "PK_WeatherImageTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "WeatherJobStatuses" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "NameRu" text NOT NULL,
        "NameEn" text,
        CONSTRAINT "PK_WeatherJobStatuses" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "WeatherProviders" (
        "Id" uuid NOT NULL,
        "Z" double precision,
        "FrequencyId" integer,
        "Editable" boolean,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "NameRu" text NOT NULL,
        "NameEn" text,
        CONSTRAINT "PK_WeatherProviders" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "WeatherReportTypes" (
        "Id" uuid NOT NULL,
        "Css" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "NameRu" text NOT NULL,
        "NameEn" text,
        CONSTRAINT "PK_WeatherReportTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "WeatherStations" (
        "Id" uuid NOT NULL,
        "FirmId" uuid,
        "RegionId" uuid,
        "ClassId" uuid,
        "IsAuto" boolean,
        "Lat" numeric,
        "Lng" numeric,
        "Radius" numeric,
        "WeatherStationTypeId" uuid,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_WeatherStations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "WeatherStationTypes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "NameEn" text,
        "NameRu" text,
        CONSTRAINT "PK_WeatherStationTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    CREATE TABLE "WeatherTypes" (
        "Id" uuid NOT NULL,
        "Code" integer,
        "Gis" boolean NOT NULL,
        "TiledMap" boolean NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "NameRu" text NOT NULL,
        "NameEn" text,
        CONSTRAINT "PK_WeatherTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124043_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250818124043_Init_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250819121733_Update_Database') THEN
    ALTER TABLE "Regions" DROP COLUMN "NameAdminUz";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250819121733_Update_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250819121733_Update_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250821142049_Update_Database_Add_Polygon') THEN
    ALTER TABLE "RegionMaps" ADD "Polygon" geometry;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250821142049_Update_Database_Add_Polygon') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250821142049_Update_Database_Add_Polygon', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherStationTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherStationTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherStations" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherStations" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherReportTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherReportTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherProviders" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherProviders" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherJobStatuses" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherJobStatuses" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherImageTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherImageTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherFrequencies" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherFrequencies" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherConditions" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherConditions" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherAlertTypes" DROP COLUMN "DescriptionEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherAlertTypes" DROP COLUMN "DescriptionRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherAlertTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherAlertTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WaterResources" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WaterResources" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "VegetationPeriods" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "VegetationPeriods" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "VegetationIndexes" DROP COLUMN "DescriptionEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "VegetationIndexes" DROP COLUMN "DescriptionRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "VarietyFeatures" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "VarietyFeatures" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Varieties" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Varieties" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "UavDataTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "UavDataTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "UavCameraTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "UavCameraTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "TaskStatuses" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "TaskStatuses" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "SystemDataSources" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "SystemDataSources" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "SoilTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "SoilTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "RegionTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "RegionTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Regions" DROP COLUMN "NameAdminEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Regions" DROP COLUMN "NameAdminRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Regions" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Regions" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Regions" DROP COLUMN "ShortNameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Regions" DROP COLUMN "ShortNameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "ProviderWeatherConditions" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "ProviderWeatherConditions" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "ProductQualityStandards" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "ProductQualityStandards" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "PestTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "PestTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "PesticideTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "PesticideTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Pesticides" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Pesticides" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Persons" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Persons" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "MeliorativeMeasureTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "MeliorativeMeasureTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "MeasurementUnits" DROP COLUMN "FullNameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "MeasurementUnits" DROP COLUMN "FullNameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "MeasurementUnits" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "MeasurementUnits" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "IrrigationSources" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "IrrigationSources" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "IrrigationMethods" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "IrrigationMethods" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "FlightTargets" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "FlightTargets" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Firms" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Firms" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "FieldUsageStatuses" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "FieldUsageStatuses" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "FertilizerTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "FertilizerTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Fertilizers" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Fertilizers" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "EntomophageTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "EntomophageTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Diseases" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Diseases" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "DicitonaryTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "DicitonaryTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Crops" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Crops" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "CropPeriods" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "CropPeriods" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "BusinessTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "BusinessTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "BidTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "BidTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "BidStates" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "BidStates" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "BidContents" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "BidContents" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroTerms" DROP COLUMN "DescrEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroTerms" DROP COLUMN "DescrRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroTerms" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroTerms" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgrotechnicalMeasures" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgrotechnicalMeasures" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroOperations" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroOperations" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroMachineTypes" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroMachineTypes" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroclimaticZones" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroclimaticZones" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AdministrativeUnits" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AdministrativeUnits" DROP COLUMN "NameRu";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherStationTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherStations" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherReportTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherProviders" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherJobStatuses" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherImageTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherFrequencies" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherConditions" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherAlertTypes" ADD "Descriptions" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WeatherAlertTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "WaterResources" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "VegetationPeriods" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "VegetationIndexes" ADD "Descriptions" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "VarietyFeatures" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Varieties" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "UavDataTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "UavCameraTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "TaskStatuses" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "SystemDataSources" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "SoilTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "RegionTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Regions" ADD "AdminNames" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Regions" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Regions" ADD "ShortNames" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "ProviderWeatherConditions" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "ProductQualityStandards" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "PestTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "PesticideTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Pesticides" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Persons" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "MeliorativeMeasureTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "MeasurementUnits" ADD "FullNames" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "MeasurementUnits" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "IrrigationSources" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "IrrigationMethods" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "FlightTargets" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Firms" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "FieldUsageStatuses" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "FertilizerTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Fertilizers" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "EntomophageTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Diseases" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "DicitonaryTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "Crops" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "CropPeriods" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "BusinessTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "BidTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "BidStates" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "BidContents" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroTerms" ADD "Descriptions" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroTerms" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgrotechnicalMeasures" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroOperations" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroMachineTypes" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AgroclimaticZones" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    ALTER TABLE "AdministrativeUnits" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903165141_Change_Localization') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250903165141_Change_Localization', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "Tags" RENAME COLUMN "Name" TO "Names";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "WeatherTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "WeatherStationTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "WeatherStations" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "WeatherReportTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "WeatherProviders" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "WeatherJobStatuses" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "WeatherImageTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "WeatherFrequencies" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "WeatherConditions" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "WeatherAlertTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "WeatherAlertTypes" ALTER COLUMN "Descriptions" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "WaterResources" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "VegetationPeriods" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "VegetationIndexes" ALTER COLUMN "Descriptions" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "VarietyFeatures" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "Varieties" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "UavDataTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "UavCameraTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "TaskStatuses" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "SystemDataSources" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "SoilTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "RegionTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "Regions" ALTER COLUMN "ShortNames" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "Regions" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "Regions" ALTER COLUMN "AdminNames" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "ProviderWeatherConditions" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "ProductQualityStandards" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "PestTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "PesticideTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "Pesticides" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "Persons" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "MeliorativeMeasureTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "MeasurementUnits" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "MeasurementUnits" ALTER COLUMN "FullNames" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "IrrigationSources" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "IrrigationMethods" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "FlightTargets" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "Firms" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "FieldUsageStatuses" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "FertilizerTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "Fertilizers" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "EntomophageTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "Diseases" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "DicitonaryTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "Crops" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "CropPeriods" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "BusinessTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "BidTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "BidStates" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "BidContents" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "AgroTerms" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "AgroTerms" ALTER COLUMN "Descriptions" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "AgrotechnicalMeasures" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "AgroOperations" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "AgroMachineTypes" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "AgroclimaticZones" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    ALTER TABLE "AdministrativeUnits" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141217_Update_Locale_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250904141217_Update_Locale_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251007154935_Add_Insurance') THEN
    CREATE TABLE "Insurances" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_Insurances" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251007154935_Add_Insurance') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251007154935_Add_Insurance', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251020093744_BtAndRd_Update') THEN
    ALTER TABLE "Persons" DROP COLUMN "Names";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251020093744_BtAndRd_Update') THEN
    ALTER TABLE "Firms" DROP COLUMN "Names";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251020093744_BtAndRd_Update') THEN
    CREATE TABLE "BusinessTypesFirm" (
        "Id" uuid NOT NULL,
        "FirmId" uuid NOT NULL,
        "BusinessTypeId" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_BusinessTypesFirm" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251020093744_BtAndRd_Update') THEN
    CREATE TABLE "BusinessTypesPerson" (
        "Id" uuid NOT NULL,
        "PersonId" uuid NOT NULL,
        "BusinessTypeId" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_BusinessTypesPerson" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251020093744_BtAndRd_Update') THEN
    CREATE TABLE "RegulatoryDocumentations" (
        "Id" uuid NOT NULL,
        "DocumentUrl" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_RegulatoryDocumentations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251020093744_BtAndRd_Update') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251020093744_BtAndRd_Update', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021125809_TagRelations') THEN
    ALTER TABLE "Tags" DROP COLUMN "Names";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021125809_TagRelations') THEN
    ALTER TABLE "Tags" DROP COLUMN "OwnerId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021125809_TagRelations') THEN
    ALTER TABLE "Tags" RENAME COLUMN "OwnerTypeFullName" TO "Name";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021125809_TagRelations') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251021125809_TagRelations', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021214734_Update_Database_ImageSvg') THEN
    ALTER TABLE "CropPeriods" ADD "ImageSvg" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021214734_Update_Database_ImageSvg') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251021214734_Update_Database_ImageSvg', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "WeatherTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "WeatherStationTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "WeatherStations" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "WeatherReportTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "WeatherProviders" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "WeatherJobStatuses" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "WeatherImageTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "WeatherFrequencies" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "WeatherConditions" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "WeatherAlertTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "WaterResources" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "VegetationPeriods" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "VegetationIndexes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "VarietyFeatures" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "Varieties" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "UavDataTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "UavCameraTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "TaskStatuses" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "Tags" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "SystemDataSources" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "SolarRadiationInfluences" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "SoilTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "RegulatoryDocumentations" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "RegionTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "Regions" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "RegionMaps" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "ProviderWeatherConditions" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "ProductQualityStandards" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "PestTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "PesticideTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "Pesticides" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "Persons" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "MeliorativeMeasureTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "MeasurementUnits" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "IrrigationSources" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "IrrigationMethods" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "Insurances" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "FlightTargets" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "Firms" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "FieldUsageStatuses" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "FertilizerTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "Fertilizers" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "ExperimentalFarmingMethods" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "EntomophageTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "Diseases" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "DicitonaryTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "Crops" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "CropPeriodVegetationIndexes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "CropPeriods" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "ClimaticStandards" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "BusinessTypesPerson" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "BusinessTypesFirm" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "BusinessTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "BidTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "BidStates" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "BidContents" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "AgroTerms" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "AgrotechnicalMeasures" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "AgroOperations" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "AgroMachineTypes" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "AgroclimaticZones" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    ALTER TABLE "AdministrativeUnits" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120048_Update_Database_IsPublic') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251024120048_Update_Database_IsPublic', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251028151435_UpdateRegularyDocuments') THEN
    ALTER TABLE "RegulatoryDocumentations" DROP COLUMN "DocumentUrl";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251028151435_UpdateRegularyDocuments') THEN
    ALTER TABLE "RegulatoryDocumentations" ADD "FileStorageId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251028151435_UpdateRegularyDocuments') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251028151435_UpdateRegularyDocuments', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251104124745_AddsystemDocs2Blocks') THEN
    CREATE TABLE "SystemDocumentationBlocks" (
        "Id" uuid NOT NULL,
        "ParentId" uuid,
        "OrderCode" integer,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Names" text NOT NULL,
        CONSTRAINT "PK_SystemDocumentationBlocks" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251104124745_AddsystemDocs2Blocks') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251104124745_AddsystemDocs2Blocks', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113112853_Add_New_Domain_Dictionaries') THEN
    CREATE TABLE "AnalyticalIndicators" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_AnalyticalIndicators" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113112853_Add_New_Domain_Dictionaries') THEN
    CREATE TABLE "Encumbrances" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_Encumbrances" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113112853_Add_New_Domain_Dictionaries') THEN
    CREATE TABLE "FinancialConditions" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_FinancialConditions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113112853_Add_New_Domain_Dictionaries') THEN
    CREATE TABLE "InsuranceCases" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_InsuranceCases" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113112853_Add_New_Domain_Dictionaries') THEN
    CREATE TABLE "SoilProfileAnalyses" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_SoilProfileAnalyses" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113112853_Add_New_Domain_Dictionaries') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251113112853_Add_New_Domain_Dictionaries', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251124184457_Update_Firm_Added_Coordinates') THEN
    ALTER TABLE "Firms" ADD "Latitude" double precision;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251124184457_Update_Firm_Added_Coordinates') THEN
    ALTER TABLE "Firms" ADD "Longitude" double precision;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251124184457_Update_Firm_Added_Coordinates') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251124184457_Update_Firm_Added_Coordinates', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251124191914_Update_Firm_Coordinate_Adjustment') THEN
    ALTER TABLE "Firms" DROP COLUMN "Latitude";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251124191914_Update_Firm_Coordinate_Adjustment') THEN
    ALTER TABLE "Firms" DROP COLUMN "Longitude";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251124191914_Update_Firm_Coordinate_Adjustment') THEN
    ALTER TABLE "Firms" ADD "Location" geometry;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251124191914_Update_Firm_Coordinate_Adjustment') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251124191914_Update_Firm_Coordinate_Adjustment', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251201075306_Added_Dictionary_RecommendedPlantingDate') THEN
    CREATE TABLE "RecommendedPlantingDates" (
        "Id" uuid NOT NULL,
        "CropId" uuid NOT NULL,
        "RegionId" uuid NOT NULL,
        "PeriodsJson" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        CONSTRAINT "PK_RecommendedPlantingDates" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251201075306_Added_Dictionary_RecommendedPlantingDate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251201075306_Added_Dictionary_RecommendedPlantingDate', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251202083827_RecommendedPlantingDateRenamingFields') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251202083827_RecommendedPlantingDateRenamingFields', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "AgriculturalProfessions" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_AgriculturalProfessions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "BiofuelResources" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_BiofuelResources" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "BioProtectionTechnologies" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_BioProtectionTechnologies" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "ClimateAnomalies" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_ClimateAnomalies" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "CropTypeClassifiers" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_CropTypeClassifiers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "DailyTemperatureDatas" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_DailyTemperatureDatas" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "FieldWorkTechnologies" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_FieldWorkTechnologies" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "GmoCropRegistries" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_GmoCropRegistries" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "GroundwaterSources" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_GroundwaterSources" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "HarvestProcessingTechnologies" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_HarvestProcessingTechnologies" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "LandResourceFertilities" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_LandResourceFertilities" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "MineralResources" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_MineralResources" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "NatureReserves" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_NatureReserves" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "OrganicFarmingTechnologies" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_OrganicFarmingTechnologies" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "PlantProtectionTechnologies" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_PlantProtectionTechnologies" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "PrecisionFarmingCrops" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_PrecisionFarmingCrops" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "ProductQualityStandardNews" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_ProductQualityStandardNews" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "RegionalSoilMoistures" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_RegionalSoilMoistures" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "RenewableEnergySources" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_RenewableEnergySources" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "SnowLoadDatas" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_SnowLoadDatas" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "SoilChemicalCompositions" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_SoilChemicalCompositions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "SoilIrrigationModes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_SoilIrrigationModes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "SolarActivityDatas" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_SolarActivityDatas" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "SustainableFarmingTechnologies" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_SustainableFarmingTechnologies" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    CREATE TABLE "WaterResourceQualities" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_WaterResourceQualities" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208191729_AddNewDictionaries') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251208191729_AddNewDictionaries', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251212112845_VegetationType_added') THEN
    ALTER TABLE "CropPeriodVegetationIndexes" ADD "VegetationIndexType" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251212112845_VegetationType_added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251212112845_VegetationType_added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251216175323_AddDictionaryFieldType') THEN
    ALTER TABLE "AgroclimaticZones" ADD "Polygon" geometry;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251216175323_AddDictionaryFieldType') THEN
    ALTER TABLE "AgroclimaticZones" ADD "PolygonJson" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251216175323_AddDictionaryFieldType') THEN
    CREATE TABLE "AgroclimaticZoneRegionRelations" (
        "Id" uuid NOT NULL,
        "AgroclimaticZoneId" uuid NOT NULL,
        "RegionId" uuid NOT NULL,
        CONSTRAINT "PK_AgroclimaticZoneRegionRelations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251216175323_AddDictionaryFieldType') THEN
    CREATE TABLE "FieldTypes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Visible" boolean NOT NULL,
        "OrderCode" text,
        "ExtData" text,
        "Comment" text,
        "Names" text NOT NULL,
        CONSTRAINT "PK_FieldTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251216175323_AddDictionaryFieldType') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251216175323_AddDictionaryFieldType', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251217130403_CropPeriodVegetationIndexAddCropId') THEN
    ALTER TABLE "CropPeriodVegetationIndexes" ADD "BioMass" double precision NOT NULL DEFAULT 0.0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251217130403_CropPeriodVegetationIndexAddCropId') THEN
    ALTER TABLE "CropPeriodVegetationIndexes" ADD "CropId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251217130403_CropPeriodVegetationIndexAddCropId') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251217130403_CropPeriodVegetationIndexAddCropId', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218135722_Update_CropPerIndex_Database') THEN
    ALTER TABLE "CropPeriodVegetationIndexes" DROP COLUMN "Value";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251218135722_Update_CropPerIndex_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251218135722_Update_CropPerIndex_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224143901_AddActiveTemperatureSumToCropPeriod') THEN
    ALTER TABLE "CropPeriods" ADD "ActiveTemperatureSumEnd" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224143901_AddActiveTemperatureSumToCropPeriod') THEN
    ALTER TABLE "CropPeriods" ADD "ActiveTemperatureSumStart" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224143901_AddActiveTemperatureSumToCropPeriod') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251224143901_AddActiveTemperatureSumToCropPeriod', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224151020_AddAgroMeasureFields') THEN
    ALTER TABLE "AgrotechnicalMeasures" ADD "DayEnd" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224151020_AddAgroMeasureFields') THEN
    ALTER TABLE "AgrotechnicalMeasures" ADD "DayStart" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224151020_AddAgroMeasureFields') THEN
    ALTER TABLE "AgrotechnicalMeasures" ADD "Descriptions" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224151020_AddAgroMeasureFields') THEN
    ALTER TABLE "AgrotechnicalMeasures" ADD "SoilRecommendations" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224151020_AddAgroMeasureFields') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251224151020_AddAgroMeasureFields', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224151715_AddRegionIdToAgrotechnicalMeasure') THEN
    ALTER TABLE "AgrotechnicalMeasures" ADD "RegionId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251224151715_AddRegionIdToAgrotechnicalMeasure') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251224151715_AddRegionIdToAgrotechnicalMeasure', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260113112418_RootSystemType') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260113112418_RootSystemType', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260113112534_Add_RootSystemType') THEN
    CREATE TABLE "RootSystemTypes" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "Names" text NOT NULL,
        CONSTRAINT "PK_RootSystemTypes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260113112534_Add_RootSystemType') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260113112534_Add_RootSystemType', '9.0.7');
    END IF;
END $EF$;
COMMIT;

