-- Migration: SimplifyGeographyHierarchy
-- Description: Remove Areas and Governorates tables, update Cities to have required coordinates
-- WARNING: This migration will DELETE all Area and Governorate data permanently

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TrainsDb')
BEGIN
    CREATE DATABASE [TrainsDb];
    PRINT 'Created TrainsDb database';
END
GO

USE [TrainsDb];

-- Step 1: Drop foreign key constraint from Cities table
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Cities_Governorates_GovernorateId')
BEGIN
    ALTER TABLE [Cities] DROP CONSTRAINT [FK_Cities_Governorates_GovernorateId];
    PRINT 'Dropped FK_Cities_Governorates_GovernorateId constraint';
END

-- Step 2: Drop indexes on GovernorateId column
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Cities_GovernorateId' AND object_id = OBJECT_ID(N'[dbo].[Cities]'))
BEGIN
    DROP INDEX [IX_Cities_GovernorateId] ON [Cities];
    PRINT 'Dropped IX_Cities_GovernorateId index';
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Cities_GovernorateId_NameEn' AND object_id = OBJECT_ID(N'[dbo].[Cities]'))
BEGIN
    DROP INDEX [IX_Cities_GovernorateId_NameEn] ON [Cities];
    PRINT 'Dropped IX_Cities_GovernorateId_NameEn index';
END

-- Step 3: Drop GovernorateId column from Cities table
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cities]') AND name = 'GovernorateId')
BEGIN
    ALTER TABLE [Cities] DROP COLUMN [GovernorateId];
    PRINT 'Dropped GovernorateId column from Cities';
END

-- Step 4: Make Latitude and Longitude NOT NULL in Cities
-- First, update any NULL values to default coordinates (Khartoum center)
UPDATE [Cities] SET [Latitude] = 15.5007 WHERE [Latitude] IS NULL;
UPDATE [Cities] SET [Longitude] = 32.5599 WHERE [Longitude] IS NULL;
PRINT 'Updated NULL coordinates in Cities';

ALTER TABLE [Cities] ALTER COLUMN [Latitude] FLOAT NOT NULL;
ALTER TABLE [Cities] ALTER COLUMN [Longitude] FLOAT NOT NULL;
PRINT 'Made Latitude and Longitude NOT NULL in Cities';

-- Step 5: Make Latitude and Longitude NOT NULL in Stations
-- First, update any NULL values to default coordinates
UPDATE [Stations] SET [Latitude] = 15.5007 WHERE [Latitude] IS NULL;
UPDATE [Stations] SET [Longitude] = 32.5599 WHERE [Longitude] IS NULL;
PRINT 'Updated NULL coordinates in Stations';

ALTER TABLE [Stations] ALTER COLUMN [Latitude] FLOAT NOT NULL;
ALTER TABLE [Stations] ALTER COLUMN [Longitude] FLOAT NOT NULL;
PRINT 'Made Latitude and Longitude NOT NULL in Stations';

-- Step 6: Drop Governorates table
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Governorates')
BEGIN
    DROP TABLE [Governorates];
    PRINT 'Dropped Governorates table';
END

-- Step 7: Drop Areas table
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Areas')
BEGIN
    DROP TABLE [Areas];
    PRINT 'Dropped Areas table';
END

-- Verify the changes
PRINT 'Migration completed successfully';
SELECT COUNT(*) AS CitiesCount FROM [Cities];
SELECT COUNT(*) AS StationsCount FROM [Stations];
