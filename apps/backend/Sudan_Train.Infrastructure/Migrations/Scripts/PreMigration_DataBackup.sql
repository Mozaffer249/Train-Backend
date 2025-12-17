-- ===================================================================
-- PRE-MIGRATION DATA BACKUP AND PREPARATION SCRIPT
-- Migration: ComprehensiveDatabaseImprovement
-- Date: 2025-12-11
-- ===================================================================
-- This script should be run BEFORE applying the EF Core migration
-- It backs up data that will be affected by breaking changes
-- ===================================================================

USE TrainsDb;
GO

PRINT 'Starting pre-migration backup process...';
GO

-- ===================================================================
-- STEP 1: Create backup tables for affected data
-- ===================================================================

PRINT 'Creating backup tables...';
GO

-- Backup BookingPassengers.SeatNumber before column removal
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookingPassengers_Backup')
BEGIN
    SELECT 
        Id,
        BookingId,
        PassengerId,
        TripId,
        TripSeatId,
        SeatNumber,
        Price,
        GETUTCDATE() AS BackupDate
    INTO BookingPassengers_Backup
    FROM BookingPassengers;
    
    PRINT 'Created BookingPassengers_Backup table with ' + CAST(@@ROWCOUNT AS VARCHAR) + ' rows';
END
GO

-- Backup TripSeats.CoachId before column removal
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TripSeats_Backup')
BEGIN
    SELECT 
        Id,
        TripId,
        SeatId,
        CoachId,
        Status,
        Price,
        GETUTCDATE() AS BackupDate
    INTO TripSeats_Backup
    FROM TripSeats;
    
    PRINT 'Created TripSeats_Backup table with ' + CAST(@@ROWCOUNT AS VARCHAR) + ' rows';
END
GO

-- ===================================================================
-- STEP 2: Validate data consistency before migration
-- ===================================================================

PRINT 'Validating data consistency...';
GO

-- Check for orphaned BookingPassenger records
SELECT COUNT(*) AS OrphanedBookingPassengers
FROM BookingPassengers bp
WHERE bp.TripSeatId IS NOT NULL 
  AND NOT EXISTS (SELECT 1 FROM TripSeats ts WHERE ts.Id = bp.TripSeatId);

-- Check for SeatNumber mismatches (where stored SeatNumber != actual seat number)
SELECT 
    bp.Id AS BookingPassengerId,
    bp.SeatNumber AS StoredSeatNumber,
    s.SeatNumber AS ActualSeatNumber,
    CASE 
        WHEN bp.SeatNumber = s.SeatNumber THEN 'Match'
        WHEN bp.SeatNumber IS NULL AND s.SeatNumber IS NOT NULL THEN 'Missing'
        ELSE 'Mismatch'
    END AS Status
FROM BookingPassengers bp
LEFT JOIN TripSeats ts ON ts.Id = bp.TripSeatId
LEFT JOIN Seats s ON s.Id = ts.SeatId
WHERE bp.SeatNumber IS NOT NULL OR s.SeatNumber IS NOT NULL;

-- Check for CoachId mismatches in TripSeats
SELECT 
    ts.Id AS TripSeatId,
    ts.CoachId AS StoredCoachId,
    s.CoachId AS ActualCoachId,
    CASE 
        WHEN ts.CoachId = s.CoachId THEN 'Match'
        ELSE 'Mismatch'
    END AS Status
FROM TripSeats ts
INNER JOIN Seats s ON s.Id = ts.SeatId
WHERE ts.CoachId != s.CoachId;

-- ===================================================================
-- STEP 3: Create verification query for post-migration
-- ===================================================================

PRINT 'Creating post-migration verification script...';
GO

-- This query can be used after migration to verify computed properties work
-- Save this query to run after migration completes
/*
-- POST-MIGRATION VERIFICATION QUERIES
-- Run these after the migration to verify data integrity

-- Verify TripSeats can derive CoachId from Seat
SELECT 
    ts.Id AS TripSeatId,
    ts.SeatId,
    s.CoachId AS DerivedCoachId,
    tsb.CoachId AS OriginalCoachId,
    CASE WHEN s.CoachId = tsb.CoachId THEN 'OK' ELSE 'ERROR' END AS Status
FROM TripSeats ts
INNER JOIN Seats s ON s.Id = ts.SeatId
INNER JOIN TripSeats_Backup tsb ON tsb.Id = ts.Id;

-- Verify BookingPassengers can derive SeatNumber from TripSeat
SELECT 
    bp.Id AS BookingPassengerId,
    s.SeatNumber AS DerivedSeatNumber,
    bpb.SeatNumber AS OriginalSeatNumber,
    CASE WHEN s.SeatNumber = bpb.SeatNumber OR bpb.SeatNumber IS NULL THEN 'OK' ELSE 'ERROR' END AS Status
FROM BookingPassengers bp
LEFT JOIN TripSeats ts ON ts.Id = bp.TripSeatId
LEFT JOIN Seats s ON s.Id = ts.SeatId
INNER JOIN BookingPassengers_Backup bpb ON bpb.Id = bp.Id;
*/

-- ===================================================================
-- STEP 4: Document affected records
-- ===================================================================

PRINT 'Generating impact report...';
GO

SELECT 
    'BookingPassengers with SeatNumber' AS Category,
    COUNT(*) AS AffectedRecords
FROM BookingPassengers
WHERE SeatNumber IS NOT NULL

UNION ALL

SELECT 
    'TripSeats with CoachId' AS Category,
    COUNT(*) AS AffectedRecords
FROM TripSeats
WHERE CoachId IS NOT NULL

UNION ALL

SELECT 
    'Bookings Total' AS Category,
    COUNT(*) AS AffectedRecords
FROM Bookings

UNION ALL

SELECT 
    'Payments Total' AS Category,
    COUNT(*) AS AffectedRecords
FROM Payments;

-- ===================================================================
-- STEP 5: Final checklist
-- ===================================================================

PRINT '
===================================================================
PRE-MIGRATION BACKUP COMPLETED
===================================================================

Backup Tables Created:
✓ BookingPassengers_Backup
✓ TripSeats_Backup

Next Steps:
1. Review the data validation results above
2. Fix any data inconsistencies if found
3. Run the EF Core migration: dotnet ef database update
4. Run post-migration verification queries
5. If successful, can drop backup tables after 30 days

Breaking Changes:
⚠ BookingPassengers.SeatNumber column will be REMOVED
⚠ TripSeats.CoachId column will be REMOVED
⚠ These will become computed properties

===================================================================
';
GO
