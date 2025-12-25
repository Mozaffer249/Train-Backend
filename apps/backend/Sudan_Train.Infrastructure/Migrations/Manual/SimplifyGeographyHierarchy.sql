-- Migration: SimplifyGeographyHierarchy
-- Description: Remove Areas and Governorates tables, update Cities to have required coordinates
-- Date: 2024

-- WARNING: This migration will DELETE all Area and Governorate data permanently
-- Make a backup before running this migration!

BEGIN TRANSACTION;

-- Step 1: Drop foreign key constraint from Cities table
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Cities_Governorates_GovernorateId')
BEGIN
    ALTER TABLE [Cities] DROP CONSTRAINT [FK_Cities_Governorates_GovernorateId];
END
GO

-- Step 2: Drop GovernorateId column from Cities table
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Cities]') AND name = 'GovernorateId')
BEGIN
    ALTER TABLE [Cities] DROP COLUMN [GovernorateId];
END
GO

-- Step 3: Make Latitude and Longitude NOT NULL in Cities
-- First, update any NULL values to default coordinates (Khartoum center)
UPDATE [Cities] SET [Latitude] = 15.5007 WHERE [Latitude] IS NULL;
UPDATE [Cities] SET [Longitude] = 32.5599 WHERE [Longitude] IS NULL;

ALTER TABLE [Cities] ALTER COLUMN [Latitude] FLOAT NOT NULL;
ALTER TABLE [Cities] ALTER COLUMN [Longitude] FLOAT NOT NULL;
GO

-- Step 4: Make Latitude and Longitude NOT NULL in Stations
-- First, update any NULL values to default coordinates
UPDATE [Stations] SET [Latitude] = 15.5007 WHERE [Latitude] IS NULL;
UPDATE [Stations] SET [Longitude] = 32.5599 WHERE [Longitude] IS NULL;

ALTER TABLE [Stations] ALTER COLUMN [Latitude] FLOAT NOT NULL;
ALTER TABLE [Stations] ALTER COLUMN [Longitude] FLOAT NOT NULL;
GO

-- Step 5: Drop Governorates table (CASCADE will handle dependencies)
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Governorates')
BEGIN
    DROP TABLE [Governorates];
END
GO

-- Step 6: Drop Areas table
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Areas')
BEGIN
    DROP TABLE [Areas];
END
GO

COMMIT TRANSACTION;

-- Verify the changes
SELECT 'Migration completed successfully' AS Status;
SELECT COUNT(*) AS CitiesCount FROM [Cities];
SELECT COUNT(*) AS StationsCount FROM [Stations];
