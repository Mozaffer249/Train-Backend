-- Migration: Add Route & Station Enhancements
-- Date: 2025-12-17
-- Description: Add IsActive, MaintenanceNote to Stations and Routes, enhance Fare entity

-- ==================================================
-- 1. Add columns to Stations table
-- ==================================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Stations]') AND name = 'IsActive')
BEGIN
    ALTER TABLE [dbo].[Stations]
    ADD [IsActive] bit NOT NULL DEFAULT 1;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Stations]') AND name = 'MaintenanceNote')
BEGIN
    ALTER TABLE [dbo].[Stations]
    ADD [MaintenanceNote] nvarchar(500) NULL;
END
GO

-- ==================================================
-- 2. Add columns to Routes table
-- ==================================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Routes]') AND name = 'IsActive')
BEGIN
    ALTER TABLE [dbo].[Routes]
    ADD [IsActive] bit NOT NULL DEFAULT 1;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Routes]') AND name = 'MaintenanceNote')
BEGIN
    ALTER TABLE [dbo].[Routes]
    ADD [MaintenanceNote] nvarchar(500) NULL;
END
GO

-- ==================================================
-- 3. Modify Fares table - Add new columns
-- ==================================================

-- Add RouteId column with FK to Routes
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Fares]') AND name = 'RouteId')
BEGIN
    ALTER TABLE [dbo].[Fares]
    ADD [RouteId] int NULL;
    
    ALTER TABLE [dbo].[Fares]
    ADD CONSTRAINT [FK_Fares_Routes_RouteId] 
    FOREIGN KEY ([RouteId]) REFERENCES [dbo].[Routes]([Id])
    ON DELETE SET NULL;
END
GO

-- Add OriginStationId column with FK to Stations
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Fares]') AND name = 'OriginStationId')
BEGIN
    ALTER TABLE [dbo].[Fares]
    ADD [OriginStationId] int NULL;
    
    ALTER TABLE [dbo].[Fares]
    ADD CONSTRAINT [FK_Fares_Stations_OriginStationId] 
    FOREIGN KEY ([OriginStationId]) REFERENCES [dbo].[Stations]([Id])
    ON DELETE NO ACTION;
END
GO

-- Add DestinationStationId column with FK to Stations
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Fares]') AND name = 'DestinationStationId')
BEGIN
    ALTER TABLE [dbo].[Fares]
    ADD [DestinationStationId] int NULL;
    
    ALTER TABLE [dbo].[Fares]
    ADD CONSTRAINT [FK_Fares_Stations_DestinationStationId] 
    FOREIGN KEY ([DestinationStationId]) REFERENCES [dbo].[Stations]([Id])
    ON DELETE NO ACTION;
END
GO

-- Rename Price column to BasePrice
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Fares]') AND name = 'Price')
   AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Fares]') AND name = 'BasePrice')
BEGIN
    EXEC sp_rename '[dbo].[Fares].[Price]', 'BasePrice', 'COLUMN';
END
GO

-- Add PricePerKm column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Fares]') AND name = 'PricePerKm')
BEGIN
    ALTER TABLE [dbo].[Fares]
    ADD [PricePerKm] decimal(18,2) NULL;
END
GO

-- Set default value for VatRate if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Fares]') AND name = 'VatRate')
BEGIN
    -- Update NULL values to default 0.15
    UPDATE [dbo].[Fares]
    SET [VatRate] = 0.15
    WHERE [VatRate] IS NULL OR [VatRate] = 0;
END
GO

PRINT 'Migration 20251217_AddRouteStationEnhancements completed successfully';
GO
