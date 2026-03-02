CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103105359_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103105359_Init_Database') THEN
    CREATE TABLE "Sentinel2GenerateIndexStatusHistories" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "OwnerId" uuid NOT NULL,
        "NewModelJson" text,
        "OldModelJson" text,
        CONSTRAINT "PK_Sentinel2GenerateIndexStatusHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103105359_Init_Database') THEN
    CREATE TABLE "Sentinel2ProductHistories" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "OwnerId" uuid NOT NULL,
        "NewModelJson" text,
        "OldModelJson" text,
        CONSTRAINT "PK_Sentinel2ProductHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103105359_Init_Database') THEN
    CREATE TABLE "Sentinel2ProductsMetadataHistories" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "OwnerId" uuid NOT NULL,
        "NewModelJson" text,
        "OldModelJson" text,
        CONSTRAINT "PK_Sentinel2ProductsMetadataHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103105359_Init_Database') THEN
    CREATE TABLE "Sentinel2ProductsQueueHistories" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "OwnerId" uuid NOT NULL,
        "NewModelJson" text,
        "OldModelJson" text,
        CONSTRAINT "PK_Sentinel2ProductsQueueHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103105359_Init_Database') THEN
    CREATE TABLE "SentinelInspireMetadataHistories" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "OwnerId" uuid NOT NULL,
        "NewModelJson" text,
        "OldModelJson" text,
        CONSTRAINT "PK_SentinelInspireMetadataHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103105359_Init_Database') THEN
    CREATE TABLE "SentinelL1CProductMetadataHistories" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "OwnerId" uuid NOT NULL,
        "NewModelJson" text,
        "OldModelJson" text,
        CONSTRAINT "PK_SentinelL1CProductMetadataHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103105359_Init_Database') THEN
    CREATE TABLE "SentinelL1CTileMetadataHistories" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "OwnerId" uuid NOT NULL,
        "NewModelJson" text,
        "OldModelJson" text,
        CONSTRAINT "PK_SentinelL1CTileMetadataHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103105359_Init_Database') THEN
    CREATE TABLE "SentinelProductQualityMetadataHistories" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        "OwnerId" uuid NOT NULL,
        "NewModelJson" text,
        "OldModelJson" text,
        CONSTRAINT "PK_SentinelProductQualityMetadataHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103105359_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251103105359_Init_Database', '9.0.7');
    END IF;
END $EF$;
COMMIT;

