ALTER TABLE "Games" ADD COLUMN IF NOT EXISTS "CoverPath" varchar(500) NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20261112090500_AddCoverPath', '10.0.14'
WHERE NOT EXISTS (
    SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261112090500_AddCoverPath'
);
