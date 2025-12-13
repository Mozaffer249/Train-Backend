# Regional Hierarchy - Quick Usage Guide

## Hierarchy Structure

```
Region (6)
  ├─ Khartoum (KRT) → 1 state → 8 cities
  ├─ Eastern (EST) → 3 states → 24 cities
  ├─ Northern (NTH) → 2 states → 16 cities
  ├─ Central (CNT) → 4 states → 30 cities
  ├─ Kordofan (KRD) → 3 states → 24 cities
  └─ Darfur (DRF) → 5 states → 43 cities
```

---

## Quick Queries

### Get All Regions
```csharp
var regions = await context.Regions
    .OrderBy(r => r.NameEn)
    .ToListAsync();
```

### Get States by Region Code
```csharp
var easternStates = await context.States
    .Where(s => s.Region.Code == "EST")
    .Include(s => s.Cities)
    .ToListAsync();
```

### Get Cities by Region
```csharp
var kordofanCities = await context.Cities
    .Where(c => c.State.Region.Code == "KRD")
    .Include(c => c.State)
        .ThenInclude(s => s.Region)
    .ToListAsync();
```

### Get Complete Hierarchy for a Region
```csharp
var region = await context.Regions
    .Include(r => r.States)
        .ThenInclude(s => s.Cities)
    .FirstOrDefaultAsync(r => r.Code == "DRF");

// Access: region.States[0].Cities[0].NameEn
```

---

## Cascading Dropdowns

### API Endpoints Pattern

```csharp
// 1. Get all regions
[HttpGet("api/geography/regions")]
public async Task<IActionResult> GetRegions()
{
    return Ok(await context.Regions
        .OrderBy(r => r.NameEn)
        .Select(r => new { r.Id, r.Code, r.NameEn, r.NameAr })
        .ToListAsync());
}

// 2. Get states for a region
[HttpGet("api/geography/regions/{regionId}/states")]
public async Task<IActionResult> GetStates(int regionId)
{
    return Ok(await context.States
        .Where(s => s.RegionId == regionId)
        .OrderBy(s => s.NameEn)
        .Select(s => new { s.Id, s.NameEn, s.NameAr })
        .ToListAsync());
}

// 3. Get cities for a state
[HttpGet("api/geography/states/{stateId}/cities")]
public async Task<IActionResult> GetCities(int stateId)
{
    return Ok(await context.Cities
        .Where(c => c.StateId == stateId)
        .OrderBy(c => c.NameEn)
        .Select(c => new { c.Id, c.NameEn, c.NameAr })
        .ToListAsync());
}
```

---

## Regional Analytics

### Bookings by Region
```csharp
var regionalBookingStats = await context.Bookings
    .Include(b => b.BookingPassengers)
        .ThenInclude(bp => bp.Trip)
            .ThenInclude(t => t.Route)
                .ThenInclude(r => r.OriginStation)
                    .ThenInclude(s => s.City)
                        .ThenInclude(c => c.State)
                            .ThenInclude(st => st.Region)
    .GroupBy(b => new {
        RegionCode = b.BookingPassengers.First().Trip.Route.OriginStation.City.State.Region.Code,
        RegionName = b.BookingPassengers.First().Trip.Route.OriginStation.City.State.Region.NameEn
    })
    .Select(g => new {
        g.Key.RegionCode,
        g.Key.RegionName,
        BookingCount = g.Count(),
        TotalRevenue = g.Sum(b => b.TotalAmount),
        AverageBookingValue = g.Average(b => b.TotalAmount)
    })
    .ToListAsync();
```

### Passengers by Region
```csharp
var passengersByRegion = await context.Passengers
    .Where(p => p.CityId.HasValue)
    .GroupBy(p => p.City.State.Region.NameEn)
    .Select(g => new {
        Region = g.Key,
        PassengerCount = g.Count()
    })
    .ToListAsync();
```

---

## Region Codes Reference

| Code | Region Name | Arabic Name | States Count |
|------|------------|-------------|--------------|
| KRT  | Khartoum   | الخرطوم     | 1            |
| EST  | Eastern    | الشرقية     | 3            |
| NTH  | Northern   | الشمالية    | 2            |
| CNT  | Central    | الوسطى      | 4            |
| KRD  | Kordofan   | كردفان      | 3            |
| DRF  | Darfur     | دارفور      | 5            |

---

## SQL Queries

### Get Complete Hierarchy
```sql
SELECT 
    r.Code AS RegionCode,
    r.NameEn AS Region,
    s.NameEn AS State,
    c.NameEn AS City
FROM Regions r
INNER JOIN States s ON s.RegionId = r.Id
INNER JOIN Cities c ON c.StateId = s.Id
ORDER BY r.Code, s.NameEn, c.NameEn;
```

### Regional Summary
```sql
SELECT 
    r.Code,
    r.NameEn AS Region,
    r.NameAr AS RegionArabic,
    COUNT(DISTINCT s.Id) AS TotalStates,
    COUNT(c.Id) AS TotalCities
FROM Regions r
LEFT JOIN States s ON s.RegionId = r.Id
LEFT JOIN Cities c ON c.StateId = s.Id
GROUP BY r.Code, r.NameEn, r.NameAr
ORDER BY r.Code;
```

### Find by Region Code
```sql
-- Get all cities in Eastern region
SELECT c.NameEn, c.NameAr, s.NameEn AS State
FROM Regions r
INNER JOIN States s ON s.RegionId = r.Id
INNER JOIN Cities c ON c.StateId = s.Id
WHERE r.Code = 'EST'
ORDER BY s.NameEn, c.NameEn;
```

---

## Performance Tips

### Always Use Region Code for Filtering
```csharp
// GOOD - Uses unique index on Code
var region = await context.Regions
    .FirstOrDefaultAsync(r => r.Code == "EST");

// LESS EFFICIENT - No unique index on NameEn
var region = await context.Regions
    .FirstOrDefaultAsync(r => r.NameEn == "Eastern");
```

### Include Navigation Properties
```csharp
// For accessing nested properties, include them
var cities = await context.Cities
    .Include(c => c.State)
        .ThenInclude(s => s.Region)
    .Where(c => c.State.Region.Code == "DRF")
    .ToListAsync();

// Now you can access: cities[0].State.Region.NameEn
```

---

**Version**: 2.0 (With Regional Hierarchy)  
**Last Updated**: December 11, 2025  
**Geographic Levels**: Region → State → City
