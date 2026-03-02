CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103083747_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103083747_Init_Database') THEN
    CREATE TABLE "BidHistories" (
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
        CONSTRAINT "PK_BidHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103083747_Init_Database') THEN
    CREATE TABLE "BidTemplateHistories" (
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
        CONSTRAINT "PK_BidTemplateHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103083747_Init_Database') THEN
    CREATE TABLE "FieldAgroOperationHistories" (
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
        CONSTRAINT "PK_FieldAgroOperationHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103083747_Init_Database') THEN
    CREATE TABLE "FieldAgroProducerHistoryHistories" (
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
        CONSTRAINT "PK_FieldAgroProducerHistoryHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103083747_Init_Database') THEN
    CREATE TABLE "FieldHistories" (
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
        CONSTRAINT "PK_FieldHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103083747_Init_Database') THEN
    CREATE TABLE "FieldInsuranceHistoryHistories" (
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
        CONSTRAINT "PK_FieldInsuranceHistoryHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103083747_Init_Database') THEN
    CREATE TABLE "SeasonHistories" (
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
        CONSTRAINT "PK_SeasonHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103083747_Init_Database') THEN
    CREATE TABLE "SoilDataHistories" (
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
        CONSTRAINT "PK_SoilDataHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251103083747_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251103083747_Init_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251201074823_Update_Database1') THEN
    CREATE TABLE "ImportedFieldHistories" (
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
        CONSTRAINT "PK_ImportedFieldHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251201074823_Update_Database1') THEN
    CREATE TABLE "ImportedFieldReportHistories" (
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
        CONSTRAINT "PK_ImportedFieldReportHistories" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251201074823_Update_Database1') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251201074823_Update_Database1', '9.0.7');
    END IF;
END $EF$;
COMMIT;

