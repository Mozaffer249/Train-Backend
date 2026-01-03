# Route Management System

## Overview

The Route Management system handles the configuration and management of train routes in the Sudan Train Railway Network. A route defines a path between two stations (origin and destination) and can include intermediate stops along the way.

---

## Table of Contents

1. [Core Concepts](#core-concepts)
2. [Backend Architecture](#backend-architecture)
3. [API Endpoints](#api-endpoints)
4. [Frontend Features](#frontend-features)
5. [Route Operations](#route-operations)
6. [Intermediate Stations](#intermediate-stations)
7. [Distance Calculation](#distance-calculation)
8. [Validation Rules](#validation-rules)
9. [Common Use Cases](#common-use-cases)
10. [Troubleshooting](#troubleshooting)

---

## Core Concepts

### Route Structure

A route consists of:

- **Origin Station**: The starting point of the route
- **Destination Station**: The endpoint of the route
- **Intermediate Stations** (optional): Stations along the route between origin and destination
- **Distance**: Total route distance in kilometers (auto-calculated or manual)
- **Status**: Active/Inactive with optional maintenance notes
- **Metadata**: English and Arabic names, creation timestamps

### Route vs Station Relationship

```
┌─────────────┐
│  Stations   │ ──┐
│ (Geography) │   │
└─────────────┘   │
                  │ Many-to-Many
                  │ Relationship
┌─────────────┐   │
│   Routes    │ ──┘
│ (Network)   │
└─────────────┘
```

**Key Points:**
- Stations are independent geographical entities
- Routes connect stations to define train paths
- One station can be part of multiple routes
- Stations must be created before adding them to routes

---

## Backend Architecture

### Project Structure

```
apps/backend/
├── Sudan_Train.Data/
│   └── Entity/
│       ├── Route.cs                 # Route entity
│       └── RouteStation.cs          # Route-Station junction
├── Sudan_Train.Core/
│   └── Features/Infrastructure/Routes/
│       ├── Commands/
│       │   ├── CreateRoute/         # Create new route
│       │   ├── UpdateRoute/         # Update route details
│       │   ├── DeleteRoute/         # Delete route
│       │   ├── AddRouteStation/     # Add intermediate station
│       │   ├── RemoveRouteStation/  # Remove intermediate station
│       │   └── UpdateRouteStation/  # Update station timing
│       └── Queries/
│           ├── GetAllRoutes/        # List routes with filters
│           └── GetRouteById/        # Get route details
├── Sudan_Train.Service/
│   ├── Abstracts/
│   │   └── IRouteService.cs
│   └── Implementations/
│       ├── RouteService.cs          # Business logic
│       └── DistanceCalculationService.cs  # Haversine distance
└── Sudan_Train/Controllers/
    └── Infrastructure/RailwayNetwork/
        └── RoutesController.cs      # API endpoints
```

### Database Schema

```sql
-- Routes Table
CREATE TABLE Routes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    NameEn NVARCHAR(200),
    NameAr NVARCHAR(200),
    OriginStationId INT NOT NULL,
    DestinationStationId INT NOT NULL,
    DistanceKm DECIMAL(10,2),
    IsActive BIT NOT NULL DEFAULT 1,
    MaintenanceNote NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2,
    FOREIGN KEY (OriginStationId) REFERENCES Stations(Id),
    FOREIGN KEY (DestinationStationId) REFERENCES Stations(Id)
);

-- RouteStations Table (Junction for intermediate stops)
CREATE TABLE RouteStations (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RouteId INT NOT NULL,
    StationId INT NOT NULL,
    StopOrder INT NOT NULL,
    ArrivalOffset TIME,
    DepartureOffset TIME,
    FOREIGN KEY (RouteId) REFERENCES Routes(Id) ON DELETE CASCADE,
    FOREIGN KEY (StationId) REFERENCES Stations(Id)
);
```

---

## API Endpoints

### Base URL
```
http://localhost:8080/Api/V1/Infrastructure/Routes
```

### 1. Get All Routes

**Endpoint:** `GET /Routes`

**Query Parameters:**
- `pageNumber` (int, default: 1) - Page number for pagination
- `pageSize` (int, default: 10) - Number of items per page
- `isActive` (bool, optional) - Filter by active status

**Example Request:**

```sh
curl -X GET "http://localhost:8080/Api/V1/Infrastructure/Routes?pageNumber=1&pageSize=20&isActive=true" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

```text
  % Total    % Received % Xferd  Average Speed   Time    Time     Time  Current
                                 Dload  Upload   Total   Spent    Left  Speed

  0     0    0     0    0     0      0      0 --:--:-- --:--:-- --:--:--     0
100  1086    0  1086    0     0  12051      0 --:--:-- --:--:-- --:--:-- 12066
{"statusCode":200,"succeeded":true,"message":"Operation completed successfully","data":[{"id":1,"nameEn":"KHR-ATB","nameAr":"الخرطوم-عطبرة","origin":{"id":1,"code":"KRT-CEN","nameEn":"Khartoum main station","nameAr":"محطة الخرطوم الرئيسية","cityId":7148,"cityName":"Khartoum","latitude":15.5949804001468,"longitude":32.52783533034036,"addressEn":null,"addressAr":null,"googlePlaceId":null,"formattedAddress":null,"serviceRadiusKm":null,"stationType":null,"isActive":true,"maintenanceNote":null,"createdAt":"2025-12-28T06:43:09.1524685"},"destination":{"id":6,"code":"ATB-CEN","nameEn":"Atbara Station","nameAr":"محطة عطبرة","cityId":7154,"cityName":"Atbara","latitude":17.699058533476023,"longitude":33.97856579151289,"addressEn":null,"addressAr":null,"googlePlaceId":null,"formattedAddress":null,"serviceRadiusKm":null,"stationType":null,"isActive":true,"maintenanceNote":null,"createdAt":"2025-12-28T07:34:47.4517822"},"distanceKm":280.40,"isActive":true,"maintenanceNote":null,"intermediateStops":[],"tripsCount":0}],"errors":null,"meta":null}
```



**Example Response:**

```json
{
  "statusCode": 200,
  "succeeded": true,
  "message": "Operation completed successfully",
  "data": [
    {
      "id": 1,
      "nameEn": "Khartoum - Wadi Halfa Express",
      "nameAr": "خط الخرطوم - وادي حلفا السريع",
      "origin": {
        "id": 1,
        "code": "KRT-CEN",
        "nameEn": "Khartoum main station",
        "nameAr": "محطة الخرطوم الرئيسية",
        "latitude": 15.5949804,
        "longitude": 32.5278353
      },
      "destination": {
        "id": 3,
        "code": "WHF-CEN",
        "nameEn": "Wadi Halfa Station",
        "nameAr": "محطة وادي حلفا",
        "latitude": 21.8018935,
        "longitude": 31.3539925
      },
      "distanceKm": 786.45,
      "isActive": true,
      "maintenanceNote": null,
      "intermediateStops": [
        {
          "id": 101,
          "stationId": 6,
          "stationName": "Atbara Station",
          "stopOrder": 1,
          "arrivalOffset": "04:30:00",
          "departureOffset": "04:45:00"
        }
      ],
      "tripsCount": 12
    }
  ]
}
```

### 2. Get Route By ID

**Endpoint:** `GET /Routes/{id}`

**Example Request:**

```sh
curl -X GET "http://localhost:8080/Api/V1/Infrastructure/Routes/1" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

```text
  % Total    % Received % Xferd  Average Speed   Time    Time     Time  Current
                                 Dload  Upload   Total   Spent    Left  Speed

  0     0    0     0    0     0      0      0 --:--:-- --:--:-- --:--:--     0
100  1084    0  1084    0     0  45386      0 --:--:-- --:--:-- --:--:-- 47130
{"statusCode":200,"succeeded":true,"message":"Operation completed successfully","data":{"id":1,"nameEn":"KHR-ATB","nameAr":"الخرطوم-عطبرة","origin":{"id":1,"code":"KRT-CEN","nameEn":"Khartoum main station","nameAr":"محطة الخرطوم الرئيسية","cityId":7148,"cityName":"Khartoum","latitude":15.5949804001468,"longitude":32.52783533034036,"addressEn":null,"addressAr":null,"googlePlaceId":null,"formattedAddress":null,"serviceRadiusKm":null,"stationType":null,"isActive":true,"maintenanceNote":null,"createdAt":"2025-12-28T06:43:09.1524685"},"destination":{"id":6,"code":"ATB-CEN","nameEn":"Atbara Station","nameAr":"محطة عطبرة","cityId":7154,"cityName":"Atbara","latitude":17.699058533476023,"longitude":33.97856579151289,"addressEn":null,"addressAr":null,"googlePlaceId":null,"formattedAddress":null,"serviceRadiusKm":null,"stationType":null,"isActive":true,"maintenanceNote":null,"createdAt":"2025-12-28T07:34:47.4517822"},"distanceKm":280.40,"isActive":true,"maintenanceNote":null,"intermediateStops":[],"tripsCount":0},"errors":null,"meta":null}
```

### 3. Create Route

**Endpoint:** `POST /Routes`

**Request Body:**

```json
{
  "originStationId": 1,
  "destinationStationId": 3,
  "nameEn": "Khartoum - Wadi Halfa Express",
  "nameAr": "خط الخرطوم - وادي حلفا السريع",
  "distanceKm": 786.45,  // Optional - auto-calculated if omitted
  "isActive": true,
  "maintenanceNote": null
}
```

**Auto-Naming:** If `nameEn` or `nameAr` are not provided, they are automatically generated:
- English: "{OriginName} - {DestinationName} Route"
- Arabic: "خط {OriginNameAr} - {DestinationNameAr}"

**Example Request:**

```sh
curl -X POST "http://localhost:8080/Api/V1/Infrastructure/Routes" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "originStationId": 1,
    "destinationStationId": 3
  }'
```

```text
  % Total    % Received % Xferd  Average Speed   Time    Time     Time  Current
                                 Dload  Upload   Total   Spent    Left  Speed

  0     0    0     0    0     0      0      0 --:--:-- --:--:-- --:--:--     0
100    61    0     0  100    61      0  10705 --:--:-- --:--:-- --:--:-- 12200
```

### 4. Update Route

**Endpoint:** `PUT /Routes/{id}`

**Request Body:**

```json
{
  "nameEn": "Updated Route Name",
  "nameAr": "اسم الخط المحدث",
  "distanceKm": 800.00
}
```

### 5. Delete Route

**Endpoint:** `DELETE /Routes/{id}`

**Example Request:**

```sh
curl -X DELETE "http://localhost:8080/Api/V1/Infrastructure/Routes/1" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

```text
  % Total    % Received % Xferd  Average Speed   Time    Time     Time  Current
                                 Dload  Upload   Total   Spent    Left  Speed

  0     0    0     0    0     0      0      0 --:--:-- --:--:-- --:--:--     0
  0     0    0     0    0     0      0      0 --:--:-- --:--:-- --:--:--     0
```

### 6. Add Intermediate Station

**Endpoint:** `POST /Routes/{routeId}/Stations`

**Request Body:**

```json
{
  "stationId": 6,
  "stopOrder": 1,
  "arrivalMinutesFromOrigin": 270,    // 4h 30m
  "departureMinutesFromOrigin": 285   // 4h 45m
}
```

**Example Request:**

```sh
curl -X POST "http://localhost:8080/Api/V1/Infrastructure/Routes/1/Stations" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "stationId": 6,
    "stopOrder": 1,
    "arrivalMinutesFromOrigin": 270,
    "departureMinutesFromOrigin": 285
  }'
```

```text
  % Total    % Received % Xferd  Average Speed   Time    Time     Time  Current
                                 Dload  Upload   Total   Spent    Left  Speed

  0     0    0     0    0     0      0      0 --:--:-- --:--:-- --:--:--     0
100   120    0     0  100   120      0  37301 --:--:-- --:--:-- --:--:-- 40000
```

### 7. Remove Intermediate Station

**Endpoint:** `DELETE /Routes/{routeId}/Stations/{stationId}`

**Auto-Resequencing:** When a station is removed, remaining stations are automatically resequenced (1, 2, 3, ...).

### 8. Update Station Timing

**Endpoint:** `PUT /Routes/{routeId}/Stations/{stationId}`

**Request Body:**

```json
{
  "stopOrder": 2,
  "arrivalMinutesFromOrigin": 300,
  "departureMinutesFromOrigin": 315
}
```

---

## Frontend Features

### Access
- **URL:** `http://localhost:3001/routes`
- **Permission:** Admin/Staff only

### Routes Page

The main routes management interface includes:

1. **Routes List Table**
   - Route name (English/Arabic)
   - Origin → Destination
   - Distance
   - Status badge (Active/Inactive)
   - Actions (View Details, Edit, Delete)

2. **Filters**
   - Origin Station dropdown
   - Destination Station dropdown
   - Status filter (Active/Inactive/All)

3. **Pagination**
   - Configurable page size (10, 20, 50, 100)
   - Page navigation controls
   - Total count display

### Create/Edit Route Modal

Fields:
- **Name (English)** - Optional, auto-generated if empty
- **Name (Arabic)** - Optional, auto-generated if empty
- **Origin Station*** - Required, dropdown with all active stations
- **Destination Station*** - Required, dropdown with all active stations
- **Distance (km)** - Optional, auto-calculated using Haversine formula
- **Active Status** - Checkbox
- **Maintenance Note** - Textarea (shown when inactive)

### Route Details Modal

Displays:
- Route information (name, origin, destination, distance)
- Intermediate stations table with:
  - Stop order
  - Station name
  - Arrival time offset
  - Departure time offset
  - Actions (Edit timing, Remove)

Add Station Form:
- Station dropdown (excludes origin/destination)
- Stop order input
- Arrival minutes from origin
- Departure minutes from origin

---

## Route Operations

### Creating a Route

**Step 1: Ensure Stations Exist**
```
1. Go to Geography → Stations
2. Verify origin and destination stations exist
3. Create missing stations if needed
```

**Step 2: Create Route**
```
1. Navigate to Routes page
2. Click "Create Route" button
3. Select origin station (required)
4. Select destination station (required)
5. Optionally enter custom names or leave for auto-generation
6. Optionally enter distance or leave for auto-calculation
7. Set active status
8. Click "Create Route"
```

**Backend Logic:**

```csharp
// Auto-generate names if not provided
if (string.IsNullOrEmpty(routeName))
    routeName = $"{originStation.NameEn} - {destinationStation.NameEn} Route";

if (string.IsNullOrEmpty(routeNameAr))
    routeNameAr = $"خط {originStation.NameAr} - {destinationStation.NameAr}";

// Auto-calculate distance if not provided
if (!distanceKm.HasValue)
    distanceKm = await _distanceCalculationService.CalculateDistance(
        originStation.Latitude, originStation.Longitude,
        destinationStation.Latitude, destinationStation.Longitude
    );
```

### Adding Intermediate Stations

**Step 1: Open Route Details**
```
1. Click "View Details" on a route
2. Click "Add Station" button
```

**Step 2: Configure Station**
```
1. Select station from dropdown
2. Set stop order (1 = first stop, 2 = second, etc.)
3. Enter arrival time (minutes from origin)
4. Enter departure time (minutes from origin)
5. Click "Add"
```

**Example:**
```
Route: Khartoum (0:00) → Wadi Halfa (12:00)

Intermediate Stop: Atbara
- Stop Order: 1
- Arrival: 270 minutes (4h 30m from Khartoum)
- Departure: 285 minutes (4h 45m from Khartoum)
- Stop Duration: 15 minutes
```

### Updating Station Timing

```
1. Open route details
2. Click edit icon next to station
3. Modify timing in modal
4. Save changes
```

### Removing Stations

When a station is removed:
1. Junction record is deleted
2. Remaining stations are auto-resequenced
3. Stop orders become 1, 2, 3, ... (no gaps)

**Example:**
```
Before: [Station A (order 1), Station B (order 2), Station C (order 3)]
Remove Station B
After:  [Station A (order 1), Station C (order 2)]
```

---

## Distance Calculation

### Haversine Formula

The system uses the Haversine formula to calculate great-circle distances between two points on Earth:

```csharp
public class DistanceCalculationService : IDistanceCalculationService
{
    private const double EarthRadiusKm = 6371.0;

    public double CalculateDistance(
        double lat1, double lon1, 
        double lat2, double lon2)
    {
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private double ToRadians(double degrees) => degrees * Math.PI / 180;
}
```

### When Distance is Calculated

1. **Route Creation**: If `distanceKm` is not provided
2. **Manual Override**: User can specify custom distance

### Accuracy

- **Formula**: Provides accuracy within 0.5% for most distances
- **Limitations**: Assumes spherical Earth (actual Earth is oblate spheroid)
- **Use Case**: Sufficient for railway distance estimation

---

## Validation Rules

### Route Creation Validation

1. **Station Selection**

```csharp
   RuleFor(x => x.OriginStationId)
       .MustAsync(StationExists)
       .WithMessage("Origin station does not exist");

   RuleFor(x => x.DestinationStationId)
       .MustAsync(StationExists)
       .WithMessage("Destination station does not exist");
```

2. **Different Stations**

```csharp
   RuleFor(x => x)
       .Must(x => x.OriginStationId != x.DestinationStationId)
       .WithMessage("Origin and destination must be different");
```

3. **Duplicate Route Prevention**

```csharp
   RuleFor(x => x)
       .MustAsync(BeUniqueRoute)
       .WithMessage("A route with this origin and destination already exists");
   
   // Checks both directions: A→B and B→A
   private async Task<bool> BeUniqueRoute(CreateRouteCommand command)
   {
       return !await _routeRepository.GetTableNoTracking()
           .AnyAsync(r => 
               (r.OriginStationId == command.OriginStationId && 
                r.DestinationStationId == command.DestinationStationId) ||
               (r.OriginStationId == command.DestinationStationId && 
                r.DestinationStationId == command.OriginStationId));
   }
```

### Adding Intermediate Station Validation

1. **Station Exists**
2. **Route Exists**
3. **Station Not Already in Route**
4. **Station Not Origin or Destination**
5. **Unique Stop Order**
6. **Logical Time Offsets**:
   - Arrival ≥ 0
   - Departure ≥ Arrival
   - Departure > 0

---

## Common Use Cases

### Use Case 1: Simple Direct Route

**Scenario:** Create a direct route with no intermediate stops

```
Step 1: Create Route
POST /Routes
{
  "originStationId": 1,      // Khartoum
  "destinationStationId": 2  // Khartoum North
}

Result:
- Name: "Khartoum main station - Khartoum North (Bahri) Station Route"
- Distance: ~5.2 km (auto-calculated)
- Intermediate stops: 0
```

### Use Case 2: Multi-Stop Express Route

**Scenario:** Northern line with multiple stops

```
Step 1: Create Route
POST /Routes
{
  "originStationId": 1,      // Khartoum
  "destinationStationId": 3, // Wadi Halfa
  "nameEn": "Northern Express",
  "nameAr": "خط الشمال السريع"
}

Step 2: Add Intermediate Stops
POST /Routes/1/Stations
{
  "stationId": 6,                    // Atbara
  "stopOrder": 1,
  "arrivalMinutesFromOrigin": 270,   // 4.5 hours
  "departureMinutesFromOrigin": 285  // 15 min stop
}

POST /Routes/1/Stations
{
  "stationId": 5,                    // Berber
  "stopOrder": 2,
  "arrivalMinutesFromOrigin": 180,   // 3 hours
  "departureMinutesFromOrigin": 195  // 15 min stop
}

Result:
Khartoum (0:00) → Berber (3:00-3:15) → Atbara (4:30-4:45) → Wadi Halfa (12:00)
```

### Use Case 3: Maintenance Mode

**Scenario:** Temporarily deactivate route for maintenance

```
PUT /Routes/1
{
  "isActive": false,
  "maintenanceNote": "Track repairs scheduled for December 2025"
}

Effect:
- Route marked inactive
- Not shown in active route filters
- Trips cannot be scheduled on this route
- Visible in admin panel with maintenance note
```

### Use Case 4: Reordering Stations

**Scenario:** Adjust stop order after planning changes

```
Initial: [Atbara (order 1), Berber (order 2), Dongola (order 3)]

Update Berber to be first:
PUT /Routes/1/Stations/5
{
  "stopOrder": 1,
  "arrivalMinutesFromOrigin": 180,
  "departureMinutesFromOrigin": 195
}

Result: [Berber (order 1), Atbara (order 2), Dongola (order 3)]
```

---

## Troubleshooting

### Problem: Stations Not Showing in Dropdown

**Symptom:** Only seeing first 10 stations when creating/editing routes

**Cause:** Missing `pageSize` parameter in API call

**Solution:** Updated in `RouteModal.tsx` and `RouteDetailModal.tsx`:

```ts
// ❌ Wrong - uses default pagination (10 items)
const data = await stationsApi.getAll({ isActive: true });

// ✅ Correct - loads all stations
const data = await stationsApi.getAll({ isActive: true, pageSize: 1000 });
```

### Problem: Duplicate Route Error

**Symptom:** "A route with this origin and destination already exists"

**Cause:** System prevents duplicate routes in both directions

**Explanation:**
- Route A→B prevents creating B→A
- Each station pair can have only one route
- Prevents ambiguity in fare calculation and trip scheduling

**Solution:**
- Check existing routes before creating
- Use different origin/destination pair
- Or edit existing route instead

### Problem: Cannot Add Station to Route

**Symptom:** Station doesn't appear in intermediate station dropdown

**Possible Causes:**

1. **Station is Origin or Destination**
   - Intermediate stations exclude origin/destination
   - Solution: Only add stations between endpoints

2. **Station Already Added**
   - Cannot add same station twice
   - Solution: Check existing intermediate stops

3. **Station is Inactive**
   - Only active stations shown in dropdowns
   - Solution: Activate station in Geography → Stations

4. **Pagination Issue**
   - Fixed with `pageSize: 1000` parameter

### Problem: Distance Auto-Calculation Incorrect

**Symptom:** Calculated distance differs from actual rail distance

**Cause:** Haversine calculates straight-line distance, not rail path

**Solution:**
- Manually enter accurate rail distance
- Use surveyed track measurements
- Account for curves, elevation, detours

**Example:**
```
Straight-line: 100 km
Actual rail track: 125 km (due to terrain, curves)
→ Enter distanceKm: 125.0 manually
```

### Problem: Time Offsets Not Updating

**Symptom:** Station timing doesn't save or resets

**Cause:** Arrival time > Departure time (invalid)

**Validation:**
- Arrival ≥ 0
- Departure ≥ Arrival
- Both must be logical (departure after arrival)

**Solution:**
```
❌ Wrong:
Arrival: 270 minutes
Departure: 260 minutes (before arrival!)

✅ Correct:
Arrival: 270 minutes
Departure: 285 minutes (15 min stop)
```

---

## Related Documentation

- [Station Management](./station-management.md)
- [Fare Management](./fare-management.md)
- [Distance Calculation Service](./distance-calculation.md)
- [API Authentication](../api/authentication.md)

---

## Change Log

| Version | Date       | Changes                                      |
|---------|------------|----------------------------------------------|
| 1.0.0   | 2025-12-17 | Initial route management implementation      |
| 1.1.0   | 2025-12-30 | Added status fields, pagination, filters     |
| 1.1.1   | 2025-12-30 | Fixed station dropdown pagination issue      |

---

## Support

For issues or questions:
- Check [Troubleshooting](#troubleshooting) section
- Review API endpoint examples
- Contact: dev-team@sudantrain.com
