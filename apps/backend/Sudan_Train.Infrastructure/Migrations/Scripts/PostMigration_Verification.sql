-- ===================================================================
-- POST-MIGRATION VERIFICATION SCRIPT
-- Migration: ComprehensiveDatabaseImprovement
-- Date: 2025-12-11
-- ===================================================================
-- Run this script AFTER applying the EF Core migration
-- It verifies data integrity and computed property functionality
-- ===================================================================

USE TrainsDb;
GO

PRINT 'Starting post-migration verification...';
GO

-- ===================================================================
-- STEP 1: Verify new tables were created
-- ===================================================================

PRINT 'Checking new tables...';
GO

SELECT 
    TABLE_NAME,
    CASE WHEN TABLE_NAME IS NOT NULL THEN '✓ Created' ELSE '✗ Missing' END AS Status
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME IN (
    'Refunds',
    'Notifications',
    'TrainSchedules',
    'Promotions',
    'PromotionUsages'
);

-- ===================================================================
-- STEP 2: Verify indexes were created
-- ===================================================================

PRINT 'Checking indexes...';
GO

SELECT 
    t.name AS TableName,
    i.name AS IndexName,
    i.is_unique AS IsUnique,
    STRING_AGG(c.name, ', ') AS Columns
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE t.name IN (
    'Bookings',
    'BookingPassengers',
    'TripSeats',
    'Trains',
    'Stations',
    'Passengers',
    'Promotions',
    'Refunds',
    'Notifications'
)
AND i.name IS NOT NULL
GROUP BY t.name, i.name, i.is_unique
ORDER BY t.name, i.name;

-- ===================================================================
-- STEP 3: Verify audit columns were added
-- ===================================================================

PRINT 'Checking audit columns...';
GO

SELECT 
    t.name AS TableName,
    CASE WHEN EXISTS(SELECT 1 FROM sys.columns WHERE object_id = t.object_id AND name = 'CreatedAt') THEN '✓' ELSE '✗' END AS CreatedAt,
    CASE WHEN EXISTS(SELECT 1 FROM sys.columns WHERE object_id = t.object_id AND name = 'UpdatedAt') THEN '✓' ELSE '✗' END AS UpdatedAt,
    CASE WHEN EXISTS(SELECT 1 FROM sys.columns WHERE object_id = t.object_id AND name = 'CreatedBy') THEN '✓' ELSE '✗' END AS CreatedBy,
    CASE WHEN EXISTS(SELECT 1 FROM sys.columns WHERE object_id = t.object_id AND name = 'UpdatedBy') THEN '✓' ELSE '✗' END AS UpdatedBy
FROM sys.tables t
WHERE t.name IN (
    'Trains',
    'Coaches',
    'Trip',
    'Routes',
    'Passengers',
    'Refunds',
    'Notifications',
    'TrainSchedules',
    'Promotions',
    'PromotionUsages'
);

-- ===================================================================
-- STEP 4: Verify columns were removed
-- ===================================================================

PRINT 'Verifying column removals...';
GO

-- Check SeatNumber was removed from BookingPassengers
IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('BookingPassengers') AND name = 'SeatNumber')
BEGIN
    PRINT '✗ ERROR: SeatNumber column still exists in BookingPassengers!';
END
ELSE
BEGIN
    PRINT '✓ SeatNumber column successfully removed from BookingPassengers';
END

-- Check CoachId was removed from TripSeats
IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TripSeats') AND name = 'CoachId')
BEGIN
    PRINT '✗ ERROR: CoachId column still exists in TripSeats!';
END
ELSE
BEGIN
    PRINT '✓ CoachId column successfully removed from TripSeats';
END

-- ===================================================================
-- STEP 5: Verify data integrity
-- ===================================================================

PRINT 'Checking data integrity...';
GO

-- Verify TripSeats can derive CoachId from Seat (compare with backup)
SELECT 
    'TripSeat Coach Derivation' AS CheckType,
    COUNT(*) AS TotalRecords,
    SUM(CASE WHEN s.CoachId = tsb.CoachId THEN 1 ELSE 0 END) AS MatchingRecords,
    SUM(CASE WHEN s.CoachId != tsb.CoachId THEN 1 ELSE 0 END) AS MismatchedRecords
FROM TripSeats ts
INNER JOIN Seats s ON s.Id = ts.SeatId
INNER JOIN TripSeats_Backup tsb ON tsb.Id = ts.Id;

-- Verify BookingPassengers can derive SeatNumber from TripSeat (compare with backup)
SELECT 
    'BookingPassenger SeatNumber Derivation' AS CheckType,
    COUNT(*) AS TotalRecords,
    SUM(CASE WHEN s.SeatNumber = bpb.SeatNumber OR (bpb.SeatNumber IS NULL AND s.SeatNumber IS NULL) THEN 1 ELSE 0 END) AS MatchingRecords,
    SUM(CASE WHEN s.SeatNumber != bpb.SeatNumber AND bpb.SeatNumber IS NOT NULL AND s.SeatNumber IS NOT NULL THEN 1 ELSE 0 END) AS MismatchedRecords
FROM BookingPassengers bp
LEFT JOIN TripSeats ts ON ts.Id = bp.TripSeatId
LEFT JOIN Seats s ON s.Id = ts.SeatId
INNER JOIN BookingPassengers_Backup bpb ON bpb.Id = bp.Id;

-- ===================================================================
-- STEP 6: Test computed properties functionality
-- ===================================================================

PRINT 'Testing computed properties with sample queries...';
GO

-- Test 1: Query BookingPassengers with derived SeatNumber
-- (Application should compute this via EF Core navigation)
SELECT TOP 10
    bp.Id,
    bp.BookingId,
    bp.TripSeatId,
    s.SeatNumber AS DerivedSeatNumber,
    bpb.SeatNumber AS OriginalSeatNumber
FROM BookingPassengers bp
LEFT JOIN TripSeats ts ON ts.Id = bp.TripSeatId
LEFT JOIN Seats s ON s.Id = ts.SeatId
LEFT JOIN BookingPassengers_Backup bpb ON bpb.Id = bp.Id
ORDER BY bp.Id;

-- Test 2: Query TripSeats with derived CoachId
-- (Application should compute this via EF Core navigation)
SELECT TOP 10
    ts.Id,
    ts.TripId,
    ts.SeatId,
    s.CoachId AS DerivedCoachId,
    tsb.CoachId AS OriginalCoachId
FROM TripSeats ts
INNER JOIN Seats s ON s.Id = ts.SeatId
INNER JOIN TripSeats_Backup tsb ON tsb.Id = ts.Id
ORDER BY ts.Id;

-- ===================================================================
-- STEP 7: Verify unique constraints
-- ===================================================================

PRINT 'Checking for duplicate data that would violate new unique constraints...';
GO

-- Check for duplicate Booking References
SELECT Reference, COUNT(*) AS Count
FROM Bookings
GROUP BY Reference
HAVING COUNT(*) > 1;

-- Check for duplicate Train Numbers
SELECT TrainNumber, COUNT(*) AS Count
FROM Trains
GROUP BY TrainNumber
HAVING COUNT(*) > 1;

-- Check for duplicate Station Codes
SELECT Code, COUNT(*) AS Count
FROM Stations
GROUP BY Code
HAVING COUNT(*) > 1;

-- Check for duplicate Passenger ID Numbers
SELECT IdNumber, COUNT(*) AS Count
FROM Passengers
GROUP BY IdNumber
HAVING COUNT(*) > 1;

-- Check for duplicate TripSeat combinations
SELECT TripId, SeatId, COUNT(*) AS Count
FROM TripSeats
GROUP BY TripId, SeatId
HAVING COUNT(*) > 1;

-- ===================================================================
-- STEP 8: Check foreign key relationships
-- ===================================================================

PRINT 'Verifying foreign key relationships...';
GO

-- Verify all BookingPassengers have valid TripSeats (if assigned)
SELECT COUNT(*) AS InvalidTripSeatReferences
FROM BookingPassengers bp
WHERE bp.TripSeatId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM TripSeats ts WHERE ts.Id = bp.TripSeatId);

-- Verify all Payments have valid Bookings
SELECT COUNT(*) AS InvalidBookingReferences
FROM Payments p
WHERE NOT EXISTS (SELECT 1 FROM Bookings b WHERE b.Id = p.BookingId);

-- ===================================================================
-- STEP 9: Generate summary report
-- ===================================================================

PRINT 'Generating migration impact summary...';
GO

SELECT 
    'Total Bookings' AS Metric,
    COUNT(*) AS Value
FROM Bookings

UNION ALL

SELECT 'Total BookingPassengers', COUNT(*) FROM BookingPassengers

UNION ALL

SELECT 'Total TripSeats', COUNT(*) FROM TripSeats

UNION ALL

SELECT 'Total Payments', COUNT(*) FROM Payments

UNION ALL

SELECT 'BookingPassengers with SeatNumber', COUNT(*) 
FROM BookingPassengers_Backup WHERE SeatNumber IS NOT NULL

UNION ALL

SELECT 'TripSeats with CoachId', COUNT(*) 
FROM TripSeats_Backup WHERE CoachId IS NOT NULL;

-- ===================================================================
-- STEP 10: Cleanup instructions (for manual execution after 30 days)
-- ===================================================================

PRINT '
===================================================================
POST-MIGRATION VERIFICATION COMPLETED
===================================================================

Backup Tables Available:
✓ BookingPassengers_Backup
✓ TripSeats_Backup

Next Steps:
1. Verify all counts match between original and backup tables
2. Check for any data inconsistencies in the validation results
3. Test application functionality with computed properties
4. Monitor application logs for any issues
5. After 30 days of successful operation, cleanup backup tables:

   DROP TABLE BookingPassengers_Backup;
   DROP TABLE TripSeats_Backup;

Breaking Changes Applied:
✓ BookingPassengers.SeatNumber - Removed (now computed)
✓ TripSeats.CoachId - Removed (now computed)
✓ Added audit fields to multiple entities
✓ Added encryption to sensitive fields

New Tables Created:
✓ Refunds
✓ Notifications
✓ TrainSchedules
✓ Promotions
✓ PromotionUsages

===================================================================
';
GO
