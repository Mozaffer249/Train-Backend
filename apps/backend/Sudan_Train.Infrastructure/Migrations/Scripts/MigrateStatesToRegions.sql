-- ===================================================================
-- MIGRATE EXISTING STATES TO REGIONS
-- Migration: AddRegionEntity
-- Date: 2025-12-11
-- ===================================================================
-- This script updates existing States with RegionId after Regions are seeded
-- Run this ONLY if you have existing data in States table
-- For new databases, the seeder handles everything automatically
-- ===================================================================

USE TrainsDb;
GO

PRINT 'Starting state-to-region migration...';
GO

-- ===================================================================
-- STEP 1: Verify Regions exist
-- ===================================================================

IF NOT EXISTS (SELECT 1 FROM Regions)
BEGIN
    PRINT '❌ ERROR: No regions found. Please run the seeder first to populate Regions table.';
    RETURN;
END

PRINT '✓ Regions table populated';
GO

-- ===================================================================
-- STEP 2: Update States with RegionId
-- ===================================================================

PRINT 'Updating states with RegionId...';
GO

-- Khartoum Region (KRT)
UPDATE States 
SET RegionId = (SELECT Id FROM Regions WHERE Code = 'KRT')
WHERE NameEn = 'Khartoum';

-- Eastern Region (EST)
UPDATE States 
SET RegionId = (SELECT Id FROM Regions WHERE Code = 'EST')
WHERE NameEn IN ('Kassala', 'Red Sea', 'Gedaref');

-- Northern Region (NTH)
UPDATE States 
SET RegionId = (SELECT Id FROM Regions WHERE Code = 'NTH')
WHERE NameEn IN ('River Nile', 'Northern');

-- Central Region (CNT)
UPDATE States 
SET RegionId = (SELECT Id FROM Regions WHERE Code = 'CNT')
WHERE NameEn IN ('Gezira', 'White Nile', 'Blue Nile', 'Sennar');

-- Kordofan Region (KRD)
UPDATE States 
SET RegionId = (SELECT Id FROM Regions WHERE Code = 'KRD')
WHERE NameEn IN ('North Kordofan', 'South Kordofan', 'West Kordofan');

-- Darfur Region (DRF)
UPDATE States 
SET RegionId = (SELECT Id FROM Regions WHERE Code = 'DRF')
WHERE NameEn IN ('North Darfur', 'South Darfur', 'East Darfur', 'West Darfur', 'Central Darfur');

PRINT '✓ States updated with RegionId';
GO

-- ===================================================================
-- STEP 3: Verify all states have RegionId
-- ===================================================================

PRINT 'Verifying migration...';
GO

DECLARE @StatesWithoutRegion INT;
SELECT @StatesWithoutRegion = COUNT(*) 
FROM States 
WHERE RegionId IS NULL;

IF @StatesWithoutRegion > 0
BEGIN
    PRINT '❌ WARNING: ' + CAST(@StatesWithoutRegion AS VARCHAR) + ' states still have NULL RegionId!';
    
    -- Show which states are missing RegionId
    SELECT NameEn AS UnassignedState
    FROM States
    WHERE RegionId IS NULL;
    
    RETURN;
END

PRINT '✓ All states have been assigned to regions';
GO

-- ===================================================================
-- STEP 4: Display summary
-- ===================================================================

PRINT 'Migration summary:';
GO

SELECT 
    r.NameEn AS Region,
    r.Code AS RegionCode,
    COUNT(s.Id) AS StateCount
FROM Regions r
LEFT JOIN States s ON s.RegionId = r.Id
GROUP BY r.Id, r.NameEn, r.Code
ORDER BY r.NameEn;

-- ===================================================================
-- STEP 5: Optional - Make RegionId NOT NULL
-- ===================================================================

-- Uncomment the following if you want to make RegionId required
-- This should only be done after verifying all states have RegionId

/*
PRINT 'Making RegionId NOT NULL...';
GO

ALTER TABLE States 
ALTER COLUMN RegionId INT NOT NULL;

PRINT '✓ RegionId is now required';
GO
*/

PRINT '
===================================================================
MIGRATION COMPLETED SUCCESSFULLY
===================================================================

Next Steps:
1. Verify the region assignments are correct
2. Test queries that use Region hierarchy
3. Update application code to use regions
4. (Optional) Run the ALTER TABLE command above to make RegionId required

===================================================================
';
GO
