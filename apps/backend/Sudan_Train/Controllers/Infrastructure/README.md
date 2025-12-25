# Infrastructure Controllers - Refactored Structure

## Overview

The monolithic `InfrastructureController` has been refactored into **8 focused controllers** organized by domain and responsibility, following Clean Architecture principles.

## 📁 Folder Structure

```
Controllers/
└── Infrastructure/
    ├── Geography/
    │   ├── RegionsController.cs      (5 endpoints)
    │   ├── StatesController.cs       (5 endpoints)
    │   └── CitiesController.cs       (5 endpoints)
    ├── RailwayNetwork/
    │   ├── StationsController.cs     (5 endpoints)
    │   └── RoutesController.cs       (7 endpoints)
    ├── Fleet/
    │   ├── TrainsController.cs       (8 endpoints)
    │   └── CoachesController.cs      (1 endpoint)
    ├── Operations/
    │   └── TripsController.cs        (5 endpoints)
    └── README.md
```

**Total: 41 endpoints** across 8 controllers

---

## 🗺️ Domain Organization

### 1. Geography (15 endpoints)

Handles geographic hierarchy: **Regions → States → Cities**

#### RegionsController
- `GET /Infrastructure/Regions` - Get all regions
- `GET /Infrastructure/Regions/{id}` - Get region by ID
- `POST /Infrastructure/Regions` - Create region
- `PUT /Infrastructure/Regions/{id}` - Update region
- `DELETE /Infrastructure/Regions/{id}` - Delete region (SuperAdmin)

#### StatesController
- `GET /Infrastructure/States` - Get all states (optional filter by regionId)
- `GET /Infrastructure/States/{id}` - Get state by ID
- `POST /Infrastructure/States` - Create state
- `PUT /Infrastructure/States/{id}` - Update state
- `DELETE /Infrastructure/States/{id}` - Delete state (SuperAdmin)

#### CitiesController
- `GET /Infrastructure/Cities` - Get all cities (optional filter by stateId)
- `GET /Infrastructure/Cities/{id}` - Get city by ID
- `POST /Infrastructure/Cities` - Create city
- `PUT /Infrastructure/Cities/{id}` - Update city
- `DELETE /Infrastructure/Cities/{id}` - Delete city (SuperAdmin)

---

### 2. Railway Network (12 endpoints)

Manages **Stations** and **Routes** with intermediate stops

#### StationsController
- `GET /Infrastructure/Stations` - Get all stations *(Public)*
- `GET /Infrastructure/Stations/{id}` - Get station by ID *(Public)*
- `POST /Infrastructure/Stations` - Create station
- `PUT /Infrastructure/Stations/{id}` - Update station
- `DELETE /Infrastructure/Stations/{id}` - Delete station (SuperAdmin)

#### RoutesController
- `GET /Infrastructure/Routes` - Get all routes *(Public)*
- `GET /Infrastructure/Routes/{id}` - Get route with stations *(Public)*
- `POST /Infrastructure/Routes` - Create route
- `PUT /Infrastructure/Routes/{id}` - Update route
- `DELETE /Infrastructure/Routes/{id}` - Delete route (SuperAdmin)
- `POST /Infrastructure/Routes/{routeId}/Stations` - Add intermediate station
- `DELETE /Infrastructure/Routes/{routeId}/Stations/{stationId}` - Remove station

---

### 3. Fleet Management (9 endpoints)

Handles **Trains**, **Coaches**, and **Seats**

#### TrainsController
- `GET /Infrastructure/Trains` - Get all trains
- `GET /Infrastructure/Trains/{id}` - Get train by ID
- `POST /Infrastructure/Trains` - Create train
- `PUT /Infrastructure/Trains/{id}` - Update train
- `DELETE /Infrastructure/Trains/{id}` - Delete train (SuperAdmin)
- `GET /Infrastructure/Trains/{trainId}/Coaches` - Get coaches by train
- `POST /Infrastructure/Trains/{trainId}/Coaches/Bulk` - Bulk create coaches

#### CoachesController
- `GET /Infrastructure/Coaches/{coachId}/Seats` - Get seats by coach

---

### 4. Operations (5 endpoints)

Manages **Trip** scheduling and operations

#### TripsController
- `GET /Infrastructure/Trips` - Get all trips *(Public)*
- `GET /Infrastructure/Trips/{id}` - Get trip by ID with availability *(Public)*
- `POST /Infrastructure/Trips` - Create trip (auto-initializes seats)
- `PUT /Infrastructure/Trips/{id}` - Update trip
- `PUT /Infrastructure/Trips/{id}/Cancel` - Cancel trip and notify passengers

---

## 🔐 Authorization Matrix

| Controller | Public Endpoints | Admin/Staff | SuperAdmin Only |
|------------|------------------|-------------|-----------------|
| Regions | - | All | Delete |
| States | - | All | Delete |
| Cities | - | All | Delete |
| Stations | GET (all) | Create, Update | Delete |
| Routes | GET (all) | Create, Update, Manage Stations | Delete |
| Trains | - | All | Delete |
| Coaches | - | All | - |
| Trips | GET (all) | Create, Update, Cancel | - |

---

## ✨ Benefits of Refactoring

### 1. **Single Responsibility Principle**
Each controller has a single, well-defined purpose and domain

### 2. **Better Organization**
Logical folder structure by business domain:
- Geography (location hierarchy)
- RailwayNetwork (physical infrastructure)
- Fleet (trains and equipment)
- Operations (scheduling)

### 3. **Improved Maintainability**
- Smaller files (avg ~80 lines vs 406 lines)
- Easier to locate and update specific functionality
- Reduced merge conflicts

### 4. **Clearer API Documentation**
- Controllers grouped by feature
- XML comments on each action
- Swagger UI shows organized structure

### 5. **Better Testability**
- Controllers can be tested independently
- Easier to mock dependencies
- Focused unit tests per domain

### 6. **Scalability**
- Easy to add new endpoints to specific domains
- Can assign different teams to different folders
- Independent deployment (if needed)

---

## 🔄 Migration Notes

### Routes Remain Unchanged
All endpoint URLs remain **exactly the same**:
- `/Infrastructure/Regions/*`
- `/Infrastructure/States/*`
- `/Infrastructure/Cities/*`
- `/Infrastructure/Stations/*`
- `/Infrastructure/Routes/*`
- `/Infrastructure/Trains/*`
- `/Infrastructure/Coaches/*`
- `/Infrastructure/Trips/*`

### No Breaking Changes
- ✅ Same HTTP methods
- ✅ Same route paths
- ✅ Same request/response models
- ✅ Same authorization policies
- ✅ Same MediatR commands/queries

### What Changed
- ✅ File organization (multiple files instead of one)
- ✅ Namespace structure (`Sudan_Train.Controllers.Infrastructure.*`)
- ✅ XML documentation comments added
- ✅ Better separation of concerns

---

## 📊 Controller Statistics

| Controller | Lines of Code | Endpoints | Public | Admin | SuperAdmin |
|------------|---------------|-----------|--------|-------|------------|
| RegionsController | 82 | 5 | 0 | 5 | 1 |
| StatesController | 82 | 5 | 0 | 5 | 1 |
| CitiesController | 82 | 5 | 0 | 5 | 1 |
| StationsController | 74 | 5 | 2 | 3 | 1 |
| RoutesController | 102 | 7 | 2 | 5 | 1 |
| TrainsController | 106 | 8 | 0 | 8 | 1 |
| CoachesController | 31 | 1 | 0 | 1 | 0 |
| TripsController | 82 | 5 | 2 | 3 | 0 |
| **Total** | **641** | **41** | **6** | **35** | **6** |

**Previous:** 1 file, 406 lines  
**Current:** 8 files, avg 80 lines each

---

## 🚀 Next Steps

1. **Update Documentation**
   - Update API documentation to reflect new structure
   - Update Postman collection organization (already done!)

2. **Add Integration Tests**
   - Test each controller independently
   - Test cross-domain workflows

3. **Consider Further Splitting**
   - If Trains/Coaches grow, consider separate files
   - Could split TripsController if booking features are added

4. **Add Versioning**
   - Consider API versioning per controller
   - Easier to version specific domains independently

---

## 📝 Summary

The refactoring successfully transformed a **406-line monolithic controller** into **8 focused controllers** organized by business domain, improving code organization, maintainability, and scalability while maintaining complete backward compatibility with all existing API endpoints.

**Key Achievement:** ✅ Zero Breaking Changes with Improved Architecture
