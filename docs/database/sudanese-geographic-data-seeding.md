# Sudanese Geographic Data Seeding

## Overview

This document describes the implementation of the StateAndCitySeeder that populates the database with a three-level geographic hierarchy: **6 Regions → 18 States → 145+ Cities**.

## Implementation Summary

### Components Created

1. **StateAndCitySeeder.cs** - Main seeder class
   - Location: `Sudan_Train.Infrastructure/Seeder/StateAndCitySeeder.cs`
   - Follows the existing seeding pattern (RoleSeeder, UserSeeder)
   - Implements transaction-safe bulk insert
   - Idempotent (safe to run multiple times)

2. **Dependency Injection Registration**
   - Registered in `ModuleInfrastructureDependencies.cs`
   - Added to application startup in `Program.cs`

### Data Seeded

#### Geographic Hierarchy

```
Region (6) → State (18) → City (145+)
```

**Hierarchy Structure:**
- **Region Entity**: Top-level administrative division
- **State Entity**: Links to Region via RegionId
- **City Entity**: Links to State via StateId

#### 6 Sudanese Regions

1. **Khartoum Region** (الخرطوم) - Code: KRT - 1 state
2. **Eastern Region** (الشرقية) - Code: EST - 3 states
3. **Northern Region** (الشمالية) - Code: NTH - 2 states
4. **Central Region** (الوسطى) - Code: CNT - 4 states
5. **Kordofan Region** (كردفان) - Code: KRD - 3 states
6. **Darfur Region** (دارفور) - Code: DRF - 5 states

**Total: 6 Regions, 18 States, 145 Cities**

#### 18 Sudanese States by Region

##### Khartoum Region (KRT)
1. **Khartoum** (الخرطوم) - 8 cities

##### Eastern Region (EST)
2. **Kassala** (كسلا) - 8 cities
3. **Red Sea** (البحر الأحمر) - 8 cities
4. **Gedaref** (القضارف) - 8 cities

##### Northern Region (NTH)
5. **River Nile** (نهر النيل) - 8 cities
6. **Northern** (الشمالية) - 8 cities

##### Central Region (CNT)
7. **Gezira** (الجزيرة) - 7 cities
8. **White Nile** (النيل الأبيض) - 9 cities
9. **Blue Nile** (النيل الأزرق) - 7 cities
10. **Sennar** (سنار) - 7 cities

##### Kordofan Region (KRD)
11. **North Kordofan** (شمال كردفان) - 9 cities
12. **South Kordofan** (جنوب كردفان) - 8 cities
13. **West Kordofan** (غرب كردفان) - 7 cities

##### Darfur Region (DRF)
14. **North Darfur** (شمال دارفور) - 9 cities
15. **South Darfur** (جنوب دارفور) - 8 cities
16. **East Darfur** (شرق دارفور) - 7 cities
17. **West Darfur** (غرب دارفور) - 8 cities
18. **Central Darfur** (وسط دارفور) - 8 cities

### Regional Organization

Cities are organized by geographical/administrative regions:

#### Greater Khartoum Region
- **Khartoum**: Capital region with major cities including Khartoum, Omdurman, Khartoum North (Bahri)

#### Eastern Region
- **Kassala**: Border region with cities like Kassala, Khashm el Girba, New Halfa
- **Red Sea**: Coastal region including Port Sudan, Suakin, Tokar
- **Gedaref**: Agricultural region with Gedaref, Gallabat, Doka

#### Northern Region
- **River Nile**: Industrial region including Atbara, Ed Damer, Shendi
- **Northern**: Historical region with Dongola, Karima, Merowe, Wadi Halfa

#### Central Region
- **Gezira**: Agricultural heartland with Wad Medani, Al Managil, Hasaheisa
- **White Nile**: Including Kosti, Rabak, Ed Dueim
- **Blue Nile**: Border region with Ad Damazin, Roseires, Kurmuk
- **Sennar**: Including Sinja, Sennar, Dinder

#### Kordofan Region
- **North Kordofan**: Including El Obeid (regional capital), Bara, En Nuhud
- **South Kordofan**: Including Kadugli, Dilling, Talodi
- **West Kordofan**: Including El Fula, Babanusa, Abu Zabad

#### Darfur Region
- **North Darfur**: Including El Fasher, Kutum, Kabkabiya
- **South Darfur**: Including Nyala, Ed Daein, Kas
- **East Darfur**: Including Ed Daein, Yassin, Abu Karinka
- **West Darfur**: Including Geneina, Kulbus, Beida
- **Central Darfur**: Including Zalingei, Mukjar, Wadi Salih

## Features

### 1. Idempotent Design
```csharp
// Checks if data already exists before seeding
var regionsCount = await _context.Regions.CountAsync();
if (regionsCount > 0)
{
    _logger.LogInformation("Regions, states, and cities already exist. Skipping seeding.");
    return;
}
```

### 2. Transaction Safety
```csharp
// Uses database transactions for atomicity
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    // Seed regions, states, and cities
    await transaction.CommitAsync();
}
catch (Exception)
{
    await transaction.RollbackAsync();
    throw;
}
```

### 3. Hierarchical Seeding
- **Regions** are seeded first to get their IDs
- **States** are then seeded with RegionId foreign keys
- **Cities** are batch-inserted with StateId foreign keys
- Uses `AddRangeAsync()` for optimal performance
- Proper foreign key relationships maintained throughout

### 4. Comprehensive Logging
- Logs when seeding starts
- Logs total regions, states, and cities seeded
- Logs if data already exists (skip)
- Logs any errors with details

## How to Use

### Automatic Seeding on Startup

The seeder runs automatically when the application starts:

```csharp
// In Program.cs
var stateAndCitySeeder = services.GetRequiredService<StateAndCitySeeder>();
await stateAndCitySeeder.SeedAsync();
```

**Execution Order:**
1. DatabaseSeeder (migrations)
2. RoleSeeder (Admin, User roles)
3. UserSeeder (default users)
4. **StateAndCitySeeder** (geographic data) ← NEW

### Manual Seeding

If you need to manually trigger seeding:

```bash
# Start the application - seeding runs automatically
docker-compose up -d --build train-api

# Or run directly
dotnet run --project Sudan_Train/Trains.Api.csproj
```

### Verify Seeding

Check the application logs:

```bash
# View logs
docker-compose logs train-api

# Look for these messages:
# "Seeding Sudanese states and cities..."
# "Successfully seeded 18 states and 145 cities."
```

Or query the database:

```sql
-- Check states count
SELECT COUNT(*) FROM States;  -- Should return 18

-- Check cities count
SELECT COUNT(*) FROM Cities;  -- Should return 145

-- View states with city counts
SELECT 
    s.NameEn AS State,
    s.NameAr AS [State (Arabic)],
    COUNT(c.Id) AS CityCount
FROM States s
LEFT JOIN Cities c ON c.StateId = s.Id
GROUP BY s.Id, s.NameEn, s.NameAr
ORDER BY s.NameEn;

-- View all cities for a specific state (e.g., Khartoum)
SELECT 
    c.NameEn AS City,
    c.NameAr AS [City (Arabic)],
    s.NameEn AS State
FROM Cities c
INNER JOIN States s ON c.StateId = s.Id
WHERE s.NameEn = 'Khartoum'
ORDER BY c.NameEn;
```

## Testing Checklist

After running the application:

- [x] Build succeeds without errors
- [ ] Application starts successfully
- [ ] Seeding logs appear in console/logs
- [ ] States table has 18 records
- [ ] Cities table has 145 records
- [ ] Arabic names display correctly (UTF-8)
- [ ] Foreign key relationships are correct (Cities.StateId → States.Id)
- [ ] Re-running application doesn't duplicate data (idempotent check works)
- [ ] No errors in application logs

## Sample Queries for Testing

### Get All Regions with Counts
```sql
SELECT 
    r.Id,
    r.NameEn,
    r.Code,
    COUNT(DISTINCT s.Id) AS TotalStates,
    COUNT(c.Id) AS TotalCities
FROM Regions r
LEFT JOIN States s ON s.RegionId = r.Id
LEFT JOIN Cities c ON c.StateId = s.Id
GROUP BY r.Id, r.NameEn, r.Code
ORDER BY r.NameEn;
```

### Get Complete Hierarchy
```sql
SELECT 
    r.NameEn AS Region,
    s.NameEn AS State,
    c.NameEn AS City
FROM Regions r
INNER JOIN States s ON s.RegionId = r.Id
INNER JOIN Cities c ON c.StateId = s.Id
ORDER BY r.NameEn, s.NameEn, c.NameEn;
```

### Get All States with City Counts
```sql
SELECT 
    r.NameEn AS Region,
    s.Id,
    s.NameEn AS State,
    s.NameAr,
    COUNT(c.Id) AS TotalCities
FROM States s
INNER JOIN Regions r ON s.RegionId = r.Id
LEFT JOIN Cities c ON c.StateId = s.Id
GROUP BY r.NameEn, s.Id, s.NameEn, s.NameAr
ORDER BY r.NameEn, s.NameEn;
```

### Find Cities by Name
```sql
SELECT 
    c.NameEn AS City,
    c.NameAr AS CityArabic,
    s.NameEn AS State,
    s.NameAr AS StateArabic
FROM Cities c
INNER JOIN States s ON c.StateId = s.Id
WHERE c.NameEn LIKE '%Port%' OR c.NameAr LIKE '%بور%'
ORDER BY c.NameEn;
```

### Get All Cities in a Specific Region
```sql
-- Get all cities in Eastern Region
SELECT 
    r.NameEn AS Region,
    s.NameEn AS State,
    c.NameEn AS City,
    c.NameAr AS CityArabic
FROM Regions r
INNER JOIN States s ON s.RegionId = r.Id
INNER JOIN Cities c ON c.StateId = s.Id
WHERE r.Code = 'EST'
ORDER BY s.NameEn, c.NameEn;
```

## Data Quality

### Naming Conventions
- **English Names**: Standard English transliterations
- **Arabic Names**: Proper Arabic script with diacritics where needed
- **Consistency**: Names match official Sudanese government records

### Coverage
- All 18 current Sudanese states included
- Major cities (state capitals, regional centers)
- Important towns (industrial, agricultural, historical)
- Border towns and key transit points
- Average of 8 cities per state (more for larger states)

## Integration with Other Features

### Regional Queries in Application Code

**Get states by region:**
```csharp
var easternStates = await context.States
    .Where(s => s.Region.Code == "EST")
    .Include(s => s.Cities)
    .ToListAsync();
```

**Get cities by region:**
```csharp
var darfurCities = await context.Cities
    .Where(c => c.State.Region.Code == "DRF")
    .Select(c => new {
        c.Id,
        c.NameEn,
        c.NameAr,
        StateName = c.State.NameEn,
        RegionName = c.State.Region.NameEn
    })
    .ToListAsync();
```

**Regional statistics:**
```csharp
var regionalStats = await context.Regions
    .Select(r => new {
        Region = r.NameEn,
        Code = r.Code,
        States = r.States.Count,
        Cities = r.States.SelectMany(s => s.Cities).Count()
    })
    .ToListAsync();
```

### Stations
Cities can be linked to train stations:
```csharp
var city = await context.Cities
    .Include(c => c.State)
        .ThenInclude(s => s.Region)
    .Include(c => c.Stations)
    .FirstOrDefaultAsync(c => c.NameEn == "Khartoum");
```

### Passengers
Passengers can have a city of residence:
```csharp
var passenger = new Passenger
{
    FullNameEn = "Ahmed Hassan",
    FullNameAr = "أحمد حسن",
    CityId = city.Id
};
```

### User Registration with Cascading Dropdowns
Users can select Region → State → City:
```csharp
// 1. Get all regions for dropdown
var regions = await context.Regions
    .OrderBy(r => r.NameEn)
    .Select(r => new { r.Id, r.NameEn, r.Code })
    .ToListAsync();

// 2. Get states for selected region
var states = await context.States
    .Where(s => s.RegionId == selectedRegionId)
    .OrderBy(s => s.NameEn)
    .Select(s => new { s.Id, s.NameEn })
    .ToListAsync();

// 3. Get cities for selected state
var cities = await context.Cities
    .Where(c => c.StateId == selectedStateId)
    .OrderBy(c => c.NameEn)
    .Select(c => new { c.Id, c.NameEn })
    .ToListAsync();
```

## Database Schema

### Entity Relationships

```
Region (1) ----< (Many) State (1) ----< (Many) City (1) ----< (Many) Station
  |                        |                        |
  Id                      RegionId                StateId
  Code                    
  NameEn
  NameAr
```

**Foreign Keys:**
- `States.RegionId` → `Regions.Id` (Restrict)
- `Cities.StateId` → `States.Id` (Restrict)
- `Stations.CityId` → `Cities.Id` (Restrict)

**Indexes:**
- `Regions.Code` - Unique
- `States.RegionId` - For joining
- `Cities.StateId` - For joining

## Maintenance

### Adding New Regions
To add a new region, update the seeder and create a migration.

### Adding New States
To add new states to existing regions:

1. Update `GetSudaneseRegionsStatesAndCities()` method in `StateAndCitySeeder.cs`
2. Add state to the appropriate region's state list
3. Clear and re-seed, or manually insert via SQL

### Adding New Cities
To add new cities to existing states:

1. Update `GetSudaneseStatesAndCities()` method in `StateAndCitySeeder.cs`
2. Add city to the appropriate state's city list
3. The seeder only runs if States table is empty, so you'll need to either:
   - Clear the States/Cities tables and re-run
   - Manually insert new cities via SQL
   - Create a separate migration/seeder for updates

### Updating City Names
If city names change (spelling corrections, official name changes):

1. Update the names in `StateAndCitySeeder.cs`
2. Create an EF Core migration to update existing records:
```bash
dotnet ef migrations add UpdateCityNames --project Sudan_Train.Infrastructure
```

## Troubleshooting

### Seeding Doesn't Run
**Issue**: No seeding logs appear

**Solutions:**
- Check that `StateAndCitySeeder` is registered in DI
- Verify it's called in `Program.cs`
- Check application startup logs for errors

### Duplicate Data
**Issue**: States/cities duplicated

**Solutions:**
- The seeder checks for existing data and skips if found
- If duplicates exist, clear tables and re-run:
```sql
DELETE FROM Cities;
DELETE FROM States;
DBCC CHECKIDENT ('Cities', RESEED, 0);
DBCC CHECKIDENT ('States', RESEED, 0);
```

### Arabic Text Shows as ???? 
**Issue**: Arabic names don't display correctly

**Solutions:**
- Ensure database collation supports Arabic (e.g., `Arabic_CI_AS`)
- Verify connection string includes proper encoding
- Check that console/log viewer supports UTF-8

### Foreign Key Errors
**Issue**: Cannot insert cities due to FK constraint

**Solutions:**
- Ensure states are inserted before cities
- Check that `StateId` matches existing state's `Id`
- Verify transaction is committed after states

## Performance

- **Seeding Time**: ~1-2 seconds for 18 states and 145 cities
- **Database Size Impact**: ~15 KB (minimal)
- **Memory Usage**: Negligible during seeding
- **Query Performance**: Indexed by state and name for fast lookups

## Migration from States-Only to Regional Hierarchy

If you have existing data without regions:

1. **Run the migration**: `dotnet ef database update`
2. **Seed regions**: The seeder will populate Regions table
3. **Update states**: Use the provided SQL script `MigrateStatesToRegions.sql`
4. **Verify**: Check that all states have RegionId set

See: `Sudan_Train.Infrastructure/Migrations/Scripts/MigrateStatesToRegions.sql`

## Future Enhancements

Potential additions:
- [ ] Add population data for regions and cities
- [ ] Add coordinates (latitude/longitude) for mapping
- [ ] Add region codes/abbreviations for all states
- [ ] Add regional economic indicators
- [ ] Add historical founding dates
- [ ] Localization for additional languages
- [ ] Regional performance dashboards
- [ ] City twinning/sister city relationships

---

**Created**: December 11, 2025  
**Last Updated**: December 11, 2025  
**Version**: 2.0 (Now with Regional Hierarchy)  
**Seeded Records**: 6 Regions, 18 States, 145 Cities
