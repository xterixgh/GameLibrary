ALTER TABLE "Games" ADD COLUMN IF NOT EXISTS "LaunchPath" varchar(1000) NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20261112094000_AddLaunchPath', '10.0.14'
WHERE NOT EXISTS (
    SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20261112094000_AddLaunchPath'
);
