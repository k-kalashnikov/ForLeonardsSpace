CREATE EXTENSION IF NOT EXISTS postgis;
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251203194744_AddedUAVFlyPath') THEN
    CREATE EXTENSION IF NOT EXISTS postgis;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251203194744_AddedUAVFlyPath') THEN
    CREATE TABLE "UAVFlyPath" (
        "Id" uuid NOT NULL,
        "Comment" text,
        "FlyPath" geometry,
        "ProcessingStatus" integer NOT NULL,
        "DataTypeId" uuid,
        "CameraTypeId" uuid,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_UAVFlyPath" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251203194744_AddedUAVFlyPath') THEN
    CREATE TABLE "UAVPhotoCollection" (
        "Id" uuid NOT NULL,
        "UAVFlyPathId" uuid NOT NULL,
        "Point" geometry,
        "AnalysisOnly" boolean NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_UAVPhotoCollection" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_UAVPhotoCollection_UAVFlyPath_UAVFlyPathId" FOREIGN KEY ("UAVFlyPathId") REFERENCES "UAVFlyPath" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251203194744_AddedUAVFlyPath') THEN
    CREATE TABLE "UAVPhoto" (
        "Id" uuid NOT NULL,
        "Title" text NOT NULL,
        "FileStorageId" uuid NOT NULL,
        "Height" double precision NOT NULL,
        "Width" double precision NOT NULL,
        "OriginalDate" date NOT NULL,
        "Comment" text,
        "UAVPhotoCollectionId" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_UAVPhoto" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_UAVPhoto_UAVPhotoCollection_UAVPhotoCollectionId" FOREIGN KEY ("UAVPhotoCollectionId") REFERENCES "UAVPhotoCollection" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251203194744_AddedUAVFlyPath') THEN
    CREATE TABLE "UAVPhotoCollectionRelation" (
        "Id" uuid NOT NULL,
        "RegionId" uuid,
        "SeasonId" uuid,
        "FieldId" uuid,
        "CropId" uuid,
        "FirmId" uuid,
        "UAVPhotoCollectionId" uuid NOT NULL,
        "CreateAt" timestamp with time zone NOT NULL,
        "Status" integer NOT NULL,
        "LastUpdateAt" timestamp with time zone NOT NULL,
        "CreateUser" uuid NOT NULL,
        "LastUpdateUser" uuid NOT NULL,
        "IsPublic" boolean NOT NULL,
        CONSTRAINT "PK_UAVPhotoCollectionRelation" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_UAVPhotoCollectionRelation_UAVPhotoCollection_UAVPhotoColle~" FOREIGN KEY ("UAVPhotoCollectionId") REFERENCES "UAVPhotoCollection" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251203194744_AddedUAVFlyPath') THEN
    CREATE INDEX "IX_UAVPhoto_UAVPhotoCollectionId" ON "UAVPhoto" ("UAVPhotoCollectionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251203194744_AddedUAVFlyPath') THEN
    CREATE INDEX "IX_UAVPhotoCollection_UAVFlyPathId" ON "UAVPhotoCollection" ("UAVFlyPathId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251203194744_AddedUAVFlyPath') THEN
    CREATE INDEX "IX_UAVPhotoCollectionRelation_UAVPhotoCollectionId" ON "UAVPhotoCollectionRelation" ("UAVPhotoCollectionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251203194744_AddedUAVFlyPath') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251203194744_AddedUAVFlyPath', '9.0.7');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251209152441_AddPreviewToUavPhotoCollections') THEN
    ALTER TABLE "UAVPhotoCollection" ADD "PreviewFileStorageId" uuid;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251209152441_AddPreviewToUavPhotoCollections') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251209152441_AddPreviewToUavPhotoCollections', '9.0.7');
    END IF;
END $EF$;
COMMIT;

