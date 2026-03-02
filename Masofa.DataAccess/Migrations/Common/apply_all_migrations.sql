CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124016_Init_Database') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124016_Init_Database') THEN
    CREATE TABLE "FileStorageItems" (
        "Id" uuid NOT NULL,
        "OwnerId" uuid NOT NULL,
        "OwnerTypeFullName" text NOT NULL,
        "FileStoragePath" text NOT NULL,
        "FileStorageBacket" text NOT NULL,
        "FileContentType" integer NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_FileStorageItems" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124016_Init_Database') THEN
    CREATE TABLE "SatelliteProducts" (
        "Id" uuid NOT NULL,
        "ProductId" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "ProductSourceType" integer NOT NULL,
        "MediadataPath" uuid NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_SatelliteProducts" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250818124016_Init_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250818124016_Init_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250819065115_Add_EmailMessages') THEN

                    CREATE TABLE IF NOT EXISTS "EmailMessages" (
                        "Id" uuid NOT NULL,
                        "Sender" text NOT NULL,
                        "Recipients" text[] NOT NULL,
                        "CarbonCopy" text[],
                        "Body" text,
                        "Subject" text,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "Status" integer NOT NULL,
                        "LastUpdateAt" timestamp with time zone NOT NULL,
                        "CreateUser" uuid NOT NULL,
                        "LastUpdateUser" uuid NOT NULL,
                        CONSTRAINT "PK_EmailMessages" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250819065115_Add_EmailMessages') THEN

                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT FROM pg_tables
                        WHERE tablename = 'EmailMessages_2025_12_15'
                    ) THEN
                        EXECUTE '
                            CREATE TABLE "EmailMessages_2025_12_15" PARTITION OF "EmailMessages"
                            FOR VALUES FROM (''2025-12-15'') TO (''2025-12-16'')
                        ';
                        RAISE NOTICE 'Partition created %', 'EmailMessages_2025_12_15';
                    END IF;
                END $$;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250819065115_Add_EmailMessages') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250819065115_Add_EmailMessages', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250825102221_AddSystemBackgroundTask') THEN
    CREATE TABLE "SystemBackgroundTasks" (
        "Id" uuid NOT NULL,
        "TaskType" integer NOT NULL,
        "ExecuteTypeName" text NOT NULL,
        "IsActive" boolean NOT NULL,
        "MaxExecutions" integer NOT NULL,
        "ExecutionCount" integer NOT NULL,
        "IsRetryable" boolean NOT NULL,
        "MaxRetryCount" integer NOT NULL,
        "CurrentRetryCount" integer NOT NULL,
        "TaskOptionJson" text,
        "ParametrsJson" text,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "NameRu" text NOT NULL,
        "NameEn" text,
        CONSTRAINT "PK_SystemBackgroundTasks" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250825102221_AddSystemBackgroundTask') THEN
    CREATE TABLE "SystemBackgroundTaskResults" (
        "Id" uuid NOT NULL,
        "ResultType" integer NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "SystemBackgroundTaskId" uuid NOT NULL,
        "TaskResultJson" text,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_SystemBackgroundTaskResults" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_SystemBackgroundTaskResults_SystemBackgroundTasks_SystemBac~" FOREIGN KEY ("SystemBackgroundTaskId") REFERENCES "SystemBackgroundTasks" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250825102221_AddSystemBackgroundTask') THEN
    CREATE INDEX "IX_SystemBackgroundTaskResults_SystemBackgroundTaskId" ON "SystemBackgroundTaskResults" ("SystemBackgroundTaskId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250825102221_AddSystemBackgroundTask') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250825102221_AddSystemBackgroundTask', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250825143504_Add_Logs') THEN
    CREATE TABLE "CallStacks" (
        "Id" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "CreateUserId" uuid,
        "CreateUserName" text NOT NULL,
        "CreateUserFullName" text NOT NULL,
        CONSTRAINT "PK_CallStacks" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250825143504_Add_Logs') THEN

                    CREATE TABLE IF NOT EXISTS "LogMessages" (
                        "Id" uuid NOT NULL,
                        "CreateAt" timestamp with time zone NOT NULL,
                        "Message" text NOT NULL,
                        "LogMessageType" integer NOT NULL,
                        "CallStackId" uuid NOT NULL,
                        "Path" text NOT NULL,
                        "Order" integer NOT NULL,
                        CONSTRAINT "PK_LogMessages" PRIMARY KEY ("Id", "CreateAt")
                    ) PARTITION BY RANGE ("CreateAt");
                
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250825143504_Add_Logs') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250825143504_Add_Logs', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250829075400_Add_UserTicket_UserTicketMessage') THEN
    CREATE TABLE "UserTicketMessages" (
        "Id" uuid NOT NULL,
        "UserTicketId" uuid NOT NULL,
        "EmailId" uuid,
        "Message" text NOT NULL,
        "Created" timestamp with time zone NOT NULL,
        "AttachmentIdsJson" text NOT NULL,
        CONSTRAINT "PK_UserTicketMessages" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250829075400_Add_UserTicket_UserTicketMessage') THEN
    CREATE TABLE "UserTickets" (
        "Id" uuid NOT NULL,
        "CreateUserName" text,
        "CreateUserEmail" text,
        "ExceptionType" text NOT NULL,
        "ExceptionJson" text NOT NULL,
        "ModuleName" text NOT NULL,
        "UserDeviceId" uuid,
        "UserDeviceType" text,
        "UserDeviceJson" text NOT NULL,
        "Status" integer NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_UserTickets" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250829075400_Add_UserTicket_UserTicketMessage') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250829075400_Add_UserTicket_UserTicketMessage', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903164806_Update_Database') THEN
    ALTER TABLE "SystemBackgroundTasks" DROP COLUMN "NameEn";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903164806_Update_Database') THEN
    ALTER TABLE "SystemBackgroundTasks" RENAME COLUMN "NameRu" TO "ParametrsTypeName";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903164806_Update_Database') THEN
    UPDATE "SystemBackgroundTasks" SET "TaskOptionJson" = '' WHERE "TaskOptionJson" IS NULL;
    ALTER TABLE "SystemBackgroundTasks" ALTER COLUMN "TaskOptionJson" SET NOT NULL;
    ALTER TABLE "SystemBackgroundTasks" ALTER COLUMN "TaskOptionJson" SET DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903164806_Update_Database') THEN
    UPDATE "SystemBackgroundTasks" SET "ParametrsJson" = '' WHERE "ParametrsJson" IS NULL;
    ALTER TABLE "SystemBackgroundTasks" ALTER COLUMN "ParametrsJson" SET NOT NULL;
    ALTER TABLE "SystemBackgroundTasks" ALTER COLUMN "ParametrsJson" SET DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903164806_Update_Database') THEN
    ALTER TABLE "SystemBackgroundTasks" ADD "Names" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250903164806_Update_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250903164806_Update_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141051_Update_Locale_Database') THEN
    ALTER TABLE "SystemBackgroundTasks" ALTER COLUMN "Names" TYPE text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250904141051_Update_Locale_Database') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250904141051_Update_Locale_Database', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250918154422_Update_Database_2_4') THEN
    ALTER TABLE "SystemBackgroundTaskResults" DROP CONSTRAINT "FK_SystemBackgroundTaskResults_SystemBackgroundTasks_SystemBac~";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250918154422_Update_Database_2_4') THEN
    DROP INDEX "IX_SystemBackgroundTaskResults_SystemBackgroundTaskId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250918154422_Update_Database_2_4') THEN
    ALTER TABLE "SystemBackgroundTaskResults" ADD "TaskResultJsonTypeName" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250918154422_Update_Database_2_4') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250918154422_Update_Database_2_4', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922121846_Update_Database_2_5') THEN
    CREATE TABLE "HealthCheckResults" (
        "Id" uuid NOT NULL,
        "DateTime" timestamp with time zone NOT NULL,
        "ModulesJson" text NOT NULL,
        CONSTRAINT "PK_HealthCheckResults" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250922121846_Update_Database_2_5') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250922121846_Update_Database_2_5', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250923100108_Update_Database_2_6') THEN
    ALTER TABLE "SatelliteProducts" ADD "PreviewImagePath" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250923100108_Update_Database_2_6') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250923100108_Update_Database_2_6', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250924113600_Update_Database_2_7') THEN
    ALTER TABLE "UserTickets" ADD "Number" bigint GENERATED BY DEFAULT AS IDENTITY;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250924113600_Update_Database_2_7') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250924113600_Update_Database_2_7', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251003192059_Update_SatelliteProduct') THEN
    ALTER TABLE "SatelliteProducts" ADD "OriginDate" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251003192059_Update_SatelliteProduct') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251003192059_Update_SatelliteProduct', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021081610_Satellite_Polygon_Added') THEN
    ALTER TABLE "SatelliteProducts" ADD "Polygon" geometry;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021081610_Satellite_Polygon_Added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251021081610_Satellite_Polygon_Added', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021125630_TagRelations') THEN
    CREATE TABLE "TagRelations" (
        "Id" uuid NOT NULL,
        "OwnerId" uuid NOT NULL,
        "OwnerTypeFullName" text NOT NULL,
        "TagId" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        CONSTRAINT "PK_TagRelations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251021125630_TagRelations') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251021125630_TagRelations', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120627_Update_Database_IsPublic') THEN
    ALTER TABLE "UserTickets" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120627_Update_Database_IsPublic') THEN
    ALTER TABLE "TagRelations" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120627_Update_Database_IsPublic') THEN
    ALTER TABLE "SystemBackgroundTasks" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120627_Update_Database_IsPublic') THEN
    ALTER TABLE "SystemBackgroundTaskResults" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120627_Update_Database_IsPublic') THEN
    ALTER TABLE "SatelliteProducts" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120627_Update_Database_IsPublic') THEN
    ALTER TABLE "FileStorageItems" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120627_Update_Database_IsPublic') THEN
    ALTER TABLE "EmailMessages" ADD "IsPublic" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024120627_Update_Database_IsPublic') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251024120627_Update_Database_IsPublic', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251027152026_Add_UserComment') THEN
    ALTER TABLE "UserTickets" ADD "TicketStatus" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251027152026_Add_UserComment') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251027152026_Add_UserComment', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251028182759_AddFileLength') THEN
    ALTER TABLE "FileStorageItems" ADD "FileLength" bigint;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251028182759_AddFileLength') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251028182759_AddFileLength', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251104102606_AddsystemDocs') THEN
    CREATE TABLE "SystemDocumentations" (
        "Id" uuid NOT NULL,
        "FileStorageId" uuid,
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
        CONSTRAINT "PK_SystemDocumentations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251104102606_AddsystemDocs') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251104102606_AddsystemDocs', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251104124703_AddsystemDocs2Blocks') THEN
    ALTER TABLE "SystemDocumentations" ADD "BlockId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251104124703_AddsystemDocs2Blocks') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251104124703_AddsystemDocs2Blocks', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251114101234_Add_AccessMapItem') THEN
    CREATE TABLE "AccessMapItems" (
        "Id" uuid NOT NULL,
        "Url" text NOT NULL,
        "RolesJson" text NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_AccessMapItems" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251114101234_Add_AccessMapItem') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251114101234_Add_AccessMapItem', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251124101445_SystemDocsLocalizing') THEN
    ALTER TABLE "SystemDocumentations" DROP COLUMN "FileStorageId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251124101445_SystemDocsLocalizing') THEN
    ALTER TABLE "SystemDocumentations" ADD "FileStorageIds" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251124101445_SystemDocsLocalizing') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251124101445_SystemDocsLocalizing', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251212124404_Add_SatelliteRegionRelation') THEN
    CREATE TABLE "SatelliteRegionRelations" (
        "Id" uuid NOT NULL,
        "SatelliteProductId" uuid NOT NULL,
        "RegionId" uuid NOT NULL,
        CONSTRAINT "PK_SatelliteRegionRelations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251212124404_Add_SatelliteRegionRelation') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251212124404_Add_SatelliteRegionRelation', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251214203923_Ndwi_added') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251214203923_Ndwi_added', '9.0.7');
    END IF;
END $EF$;
COMMIT;

