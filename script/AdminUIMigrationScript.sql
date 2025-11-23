-- Ensure uuid_generate_v4() function is available
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Check for NULL ClaimType values
DO $$
BEGIN
    IF (SELECT COUNT(*) FROM "AspNetUserClaims" WHERE "ClaimType" IS NULL) > 0 THEN
        RAISE EXCEPTION 'All AspNetUserClaims must have a ClaimType value';
    END IF;
END $$;

-- Start transaction
BEGIN;

-- Add new columns to AspNetRoles
ALTER TABLE "AspNetRoles"
    ADD COLUMN "Description" TEXT,
    ADD COLUMN "Reserved" BOOLEAN NOT NULL DEFAULT FALSE;

-- Create AspNetClaimTypes table
CREATE TABLE "AspNetClaimTypes" (
    "Id" TEXT PRIMARY KEY,
    "ConcurrencyStamp" TEXT,
    "Description" TEXT,
    "Name" VARCHAR(256) NOT NULL,
    "NormalizedName" VARCHAR(256),
    "Required" BOOLEAN NOT NULL,
    "Reserved" BOOLEAN NOT NULL,
    "Rule" TEXT,
    "RuleValidationFailureDescription" TEXT,
    "UserEditable" BOOLEAN NOT NULL DEFAULT FALSE,
    "ValueType" INT NOT NULL,
    CONSTRAINT "AK_AspNetClaimTypes_Name" UNIQUE ("Name")
);

-- Create unique index on NormalizedName
CREATE UNIQUE INDEX "ClaimTypeNameIndex" ON "AspNetClaimTypes" ("NormalizedName")
WHERE "NormalizedName" IS NOT NULL;

-- Populate AspNetClaimTypes from distinct ClaimTypes
WITH CTE AS (
    SELECT DISTINCT "ClaimType" FROM "AspNetUserClaims"
)
INSERT INTO "AspNetClaimTypes" (
    "Id", "ConcurrencyStamp", "Name", "NormalizedName", "Required", "Reserved", "ValueType"
)
SELECT 
    uuid_generate_v4(), uuid_generate_v4(), "ClaimType", UPPER("ClaimType"), FALSE, FALSE, 0
FROM CTE;

-- Modify columns to enforce NOT NULL and data types
ALTER TABLE "AspNetUserClaims" 
    ALTER COLUMN "ClaimType" SET NOT NULL;

ALTER TABLE "AspNetUserLogins" 
    ALTER COLUMN "LoginProvider" SET DATA TYPE VARCHAR(450),
    ALTER COLUMN "ProviderKey" SET DATA TYPE VARCHAR(450);

ALTER TABLE "AspNetUserTokens" 
    ALTER COLUMN "LoginProvider" SET DATA TYPE VARCHAR(450),
    ALTER COLUMN "Name" SET DATA TYPE VARCHAR(450);

-- Add new columns to AspNetUsers
ALTER TABLE "AspNetUsers"
    ADD COLUMN "FirstName" TEXT,
    ADD COLUMN "LastName" TEXT,
    ADD COLUMN "IsBlocked" BOOLEAN NOT NULL DEFAULT FALSE,
    ADD COLUMN "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE;

-- Drop foreign key constraints (assuming constraint names match)
ALTER TABLE "AspNetUserTokens" 
    DROP CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId";

ALTER TABLE "AspNetUserRoles" 
    DROP CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId";

ALTER TABLE "AspNetUserLogins" 
    DROP CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId";

ALTER TABLE "AspNetUserClaims" 
    DROP CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId";

-- Drop and recreate primary key
ALTER TABLE "AspNetUsers" 
    DROP CONSTRAINT "PK_AspNetUsers";

ALTER TABLE "AspNetUsers" 
    ADD CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id");

-- Re-add foreign key constraints
ALTER TABLE "AspNetUserTokens" 
    ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" 
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id");

ALTER TABLE "AspNetUserRoles" 
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" 
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id");

ALTER TABLE "AspNetUserLogins" 
    ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" 
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id");

ALTER TABLE "AspNetUserClaims" 
    ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" 
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id");

-- Create EFMigrationsHistory table if not exists
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" VARCHAR(150) NOT NULL PRIMARY KEY,
    "ProductVersion" VARCHAR(32) NOT NULL
);

-- Insert migration history record
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250203160276_InitialIdentityDbMigrationForAdminUI', '8.0.12');

-- Commit transaction
COMMIT;