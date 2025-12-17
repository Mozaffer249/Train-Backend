-- ===================================================================
-- ROLLBACK PLAN FOR ComprehensiveDatabaseImprovement MIGRATION
-- Date: 2025-12-11
-- ===================================================================
-- Use this script if you need to rollback the migration
-- ===================================================================

USE TrainsDb;
GO

PRINT 'Starting rollback process...';
GO

-- ===================================================================
-- OPTION 1: Rollback using EF Core Migration (RECOMMENDED)
-- ===================================================================

/*
Run this command to rollback to the previous migration:

    dotnet ef database update InitialCreate \
      --project Sudan_Train.Infrastructure \
      --startup-project Sudan_Train \
      --context ApplicationDBContext

This will automatically:
- Drop new tables (Refunds, Notifications, TrainSchedules, Promotions, PromotionUsages)
- Remove new columns (audit fields, cancellation fields)
- Restore removed columns (SeatNumber, CoachId)
- Restore data from the migration's Down() method
*/

-- ===================================================================
-- OPTION 2: Manual Rollback (if EF Core rollback fails)
-- ===================================================================

-- STEP 1: Restore removed columns
-- ===================================================================

PRINT 'Restoring removed columns...';
GO

-- Restore SeatNumber to BookingPassengers
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('BookingPassengers') AND name = 'SeatNumber')
BEGIN
    ALTER TABLE BookingPassengers
    ADD SeatNumber NVARCHAR(10) NULL;
    
    PRINT '✓ Restored SeatNumber column to BookingPassengers';
    
    -- Restore data from backup
    UPDATE bp
    SET bp.SeatNumber = bpb.SeatNumber
    FROM BookingPassengers bp
    INNER JOIN BookingPassengers_Backup bpb ON bp.Id = bpb.Id
    WHERE bpb.SeatNumber IS NOT NULL;
    
    PRINT '✓ Restored SeatNumber data from backup';
END
GO

-- Restore CoachId to TripSeats
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TripSeats') AND name = 'CoachId')
BEGIN
    ALTER TABLE TripSeats
    ADD CoachId INT NOT NULL DEFAULT 0;
    
    PRINT '✓ Restored CoachId column to TripSeats';
    
    -- Restore data from backup
    UPDATE ts
    SET ts.CoachId = tsb.CoachId
    FROM TripSeats ts
    INNER JOIN TripSeats_Backup tsb ON ts.Id = tsb.Id;
    
    PRINT '✓ Restored CoachId data from backup';
    
    -- Recreate foreign key
    ALTER TABLE TripSeats
    ADD CONSTRAINT FK_TripSeats_Coaches_CoachId
    FOREIGN KEY (CoachId) REFERENCES Coaches(Id);
    
    PRINT '✓ Restored foreign key constraint';
END
GO

-- ===================================================================
-- STEP 2: Remove new tables (if needed)
-- ===================================================================

PRINT 'Removing new tables...';
GO

IF OBJECT_ID('PromotionUsages', 'U') IS NOT NULL
    DROP TABLE PromotionUsages;
    
IF OBJECT_ID('Promotions', 'U') IS NOT NULL
    DROP TABLE Promotions;
    
IF OBJECT_ID('TrainSchedules', 'U') IS NOT NULL
    DROP TABLE TrainSchedules;
    
IF OBJECT_ID('Notifications', 'U') IS NOT NULL
    DROP TABLE Notifications;
    
IF OBJECT_ID('Refunds', 'U') IS NOT NULL
    DROP TABLE Refunds;

PRINT '✓ New tables removed';
GO

-- ===================================================================
-- STEP 3: Remove audit columns from existing tables
-- ===================================================================

PRINT 'Removing audit columns...';
GO

DECLARE @sql NVARCHAR(MAX) = '';

-- Generate DROP COLUMN statements for all audit fields
SELECT @sql = @sql + 
    'ALTER TABLE ' + t.name + ' DROP COLUMN ' + c.name + ';' + CHAR(13)
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
WHERE c.name IN ('CreatedAt', 'UpdatedAt', 'CreatedBy', 'UpdatedBy', 'IsDeleted', 'DeletedAt', 'DeletedBy')
  AND t.name NOT IN ('Bookings', 'Payments', 'Stations') -- These already had CreatedAt
  AND t.schema_id = SCHEMA_ID('dbo');

-- Execute the DROP statements
IF LEN(@sql) > 0
BEGIN
    EXEC sp_executesql @sql;
    PRINT '✓ Audit columns removed';
END
GO

-- ===================================================================
-- STEP 4: Remove cancellation tracking columns from Bookings
-- ===================================================================

PRINT 'Removing cancellation tracking columns...';
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Bookings') AND name = 'CancelledAt')
    ALTER TABLE Bookings DROP COLUMN CancelledAt;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Bookings') AND name = 'CancellationReason')
    ALTER TABLE Bookings DROP COLUMN CancellationReason;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Bookings') AND name = 'CancelledBy')
    ALTER TABLE Bookings DROP COLUMN CancelledBy;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Bookings') AND name = 'RefundAmount')
    ALTER TABLE Bookings DROP COLUMN RefundAmount;

PRINT '✓ Cancellation tracking columns removed';
GO

-- ===================================================================
-- STEP 5: Verify rollback completion
-- ===================================================================

PRINT 'Verifying rollback...';
GO

-- Check that removed columns are back
SELECT 
    'BookingPassengers.SeatNumber' AS Column,
    CASE WHEN EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('BookingPassengers') AND name = 'SeatNumber') 
         THEN '✓ Restored' ELSE '✗ Missing' END AS Status

UNION ALL

SELECT 
    'TripSeats.CoachId' AS Column,
    CASE WHEN EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TripSeats') AND name = 'CoachId') 
         THEN '✓ Restored' ELSE '✗ Missing' END AS Status;

-- Check that new tables are gone
SELECT 
    'Refunds Table' AS Item,
    CASE WHEN OBJECT_ID('Refunds', 'U') IS NULL THEN '✓ Removed' ELSE '✗ Still Exists' END AS Status

UNION ALL

SELECT 'Notifications Table',
    CASE WHEN OBJECT_ID('Notifications', 'U') IS NULL THEN '✓ Removed' ELSE '✗ Still Exists' END

UNION ALL

SELECT 'TrainSchedules Table',
    CASE WHEN OBJECT_ID('TrainSchedules', 'U') IS NULL THEN '✓ Removed' ELSE '✗ Still Exists' END

UNION ALL

SELECT 'Promotions Table',
    CASE WHEN OBJECT_ID('Promotions', 'U') IS NULL THEN '✓ Removed' ELSE '✗ Still Exists' END

UNION ALL

SELECT 'PromotionUsages Table',
    CASE WHEN OBJECT_ID('PromotionUsages', 'U') IS NULL THEN '✓ Removed' ELSE '✗ Still Exists' END;

-- ===================================================================
-- STEP 6: Update EF Migrations History
-- ===================================================================

-- After manual rollback, you must also remove the migration record:
DELETE FROM __EFMigrationsHistory
WHERE MigrationId LIKE '%ComprehensiveDatabaseImprovement%';

PRINT '
===================================================================
ROLLBACK PLAN COMPLETE
===================================================================

If using Option 1 (EF Core): 
- Run: dotnet ef database update InitialCreate

If using Option 2 (Manual):
- Execute all steps above
- Verify restoration using the verification queries
- Delete migration record from __EFMigrationsHistory
- Remove migration files from code

Backup Tables:
- Keep BookingPassengers_Backup and TripSeats_Backup for reference
- Can be dropped after successful rollback verification

===================================================================
';
GO
