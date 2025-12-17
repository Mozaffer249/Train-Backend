# Database Migration Guide - Comprehensive Database Improvement

## Overview

This guide covers the application of the ComprehensiveDatabaseImprovement migration, which includes:
- New entities (Refund, Notification, TrainSchedule, Promotion, PromotionUsage)
- Audit fields on all core entities
- Security enhancements (encryption)
- Performance improvements (indexes)
- Breaking changes (removed redundant columns)

## Pre-Migration Checklist

### 1. Backup Current Database
```bash
# Create full database backup
sqlcmd -S localhost,1433 -U sa -P YourPassword \
  -Q "BACKUP DATABASE TrainsDb TO DISK='/var/opt/mssql/backup/TrainsDb_PreMigration_$(date +%Y%m%d).bak'"
```

### 2. Run Pre-Migration Data Backup Script
```bash
# Connect to SQL Server and run the backup script
sqlcmd -S localhost,1433 -U sa -P YourPassword \
  -d TrainsDb \
  -i Sudan_Train.Infrastructure/Migrations/Scripts/PreMigration_DataBackup.sql
```

This script will:
- Create backup tables for affected data
- Validate data consistency
- Check for constraint violations
- Generate impact report

### 3. Review Validation Results
Check the output for:
- Duplicate data that would violate unique constraints
- Data inconsistencies (mismatched seat numbers, coach IDs)
- Orphaned records

**Fix any issues before proceeding!**

## Migration Execution

### Development/Testing Environment

#### Option 1: Docker Compose (Recommended)
```bash
# Stop current containers
docker-compose down

# Update database through migrations
docker-compose up -d sqlserver
docker-compose up -d --build train-api

# The migration will run automatically on startup
```

#### Option 2: Manual EF Core Migration
```bash
# Run migration
dotnet ef database update \
  --project Sudan_Train.Infrastructure \
  --startup-project Sudan_Train \
  --context ApplicationDBContext

# Verify migration was applied
dotnet ef migrations list \
  --project Sudan_Train.Infrastructure \
  --startup-project Sudan_Train
```

### Production Environment

**⚠️ CRITICAL: Follow these steps exactly**

1. **Schedule maintenance window** (2-4 hours recommended)
2. **Notify all users** of the downtime
3. **Create full database backup**
4. **Run pre-migration backup script**
5. **Review validation results** - Fix any issues
6. **Stop application** to prevent data changes
7. **Apply migration**
8. **Run post-migration verification**
9. **Test critical workflows**
10. **Start application**
11. **Monitor for issues**

```bash
# Production migration command
dotnet ef database update \
  --project Sudan_Train.Infrastructure \
  --startup-project Sudan_Train \
  --context ApplicationDBContext \
  --connection "Server=prod-server;Database=TrainsDb;..."
```

## Post-Migration Verification

### 1. Run Verification Script
```bash
sqlcmd -S localhost,1433 -U sa -P YourPassword \
  -d TrainsDb \
  -i Sudan_Train.Infrastructure/Migrations/Scripts/PostMigration_Verification.sql
```

### 2. Check Application Logs
Monitor for:
- EF Core errors related to missing columns
- Null reference exceptions
- Navigation property issues

### 3. Test Critical Workflows

**Test 1: Booking Creation**
```http
POST /api/bookings
{
  "userId": 1,
  "tripId": 1,
  "passengers": [...]
}
```
✓ Verify SeatNumber is correctly derived
✓ Check audit fields are populated

**Test 2: Refund Processing**
```http
POST /api/refunds
{
  "bookingId": 1,
  "paymentId": 1,
  "amount": 100.00,
  "reason": "Cancellation"
}
```
✓ Verify refund creation works
✓ Check cascade relationships

**Test 3: Promotion Application**
```http
POST /api/bookings
{
  "promotionCode": "SAVE20",
  ...
}
```
✓ Verify promotion validation
✓ Check discount calculation

**Test 4: Notification Creation**
- Create a booking
- Verify notification is generated
- Check notification can be queried

### 4. Performance Verification

Check index usage:
```sql
-- Verify indexes are being used
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

-- Test booking lookup by reference
SELECT * FROM Bookings WHERE Reference = 'BK123456';

-- Test user's bookings query
SELECT * FROM Bookings WHERE UserId = 1;

-- Test available seats query
SELECT * FROM TripSeats WHERE TripId = 1 AND Status = 0;

-- Review execution plans
```

## Breaking Changes & Migration Notes

### Removed Columns

#### BookingPassengers.SeatNumber
- **Old**: Stored as database column
- **New**: Computed property `TripSeat?.Seat?.SeatNumber ?? "N/A"`
- **Impact**: Read-only, derived from navigation properties
- **Query Change**: None (EF Core handles it)

#### TripSeats.CoachId
- **Old**: Stored foreign key to Coach
- **New**: Computed property `Seat.Coach`
- **Impact**: Must load Seat to access Coach
- **Query Change**: Use `.Include(ts => ts.Seat.Coach)` if needed

### New Required Fields

#### Train.NameEn
- **Old**: Nullable
- **New**: Required
- **Impact**: Must provide name when creating trains
- **Migration**: Empty strings used as default for existing records

#### Passenger.IdNumber
- **Old**: Not encrypted
- **New**: Encrypted with `[EncryptColumn]`
- **Impact**: Data is encrypted at rest
- **Query**: EF Core handles encryption/decryption automatically

### Security Enhancements

**Encrypted Fields:**
- `User.Code` (already encrypted)
- `Passenger.IdNumber` (newly encrypted)
- `Payment.CardToken` (newly encrypted)
- `Payment.ProcessorResponse` (newly encrypted)

**⚠️ Important**: Backup your encryption key securely!
Current key: `8a4dcaaec64d412380fe4b02193cd26f` (in ApplicationDBContext)

## Rollback Procedure

### If Issues Occur Within 24 Hours

#### Option 1: EF Core Rollback (Preferred)
```bash
dotnet ef database update InitialCreate \
  --project Sudan_Train.Infrastructure \
  --startup-project Sudan_Train
```

#### Option 2: Manual Rollback
```bash
sqlcmd -S localhost,1433 -U sa -P YourPassword \
  -d TrainsDb \
  -i Sudan_Train.Infrastructure/Migrations/Scripts/RollbackPlan.sql
```

### If Issues Occur After 24+ Hours

1. **Analyze the specific issue**
2. **Create a forward-fix migration** (don't rollback)
3. **Apply the fix as a new migration**

**Reason**: Rolling back after users have created data in new tables (Refunds, Notifications, etc.) will cause data loss.

## New Features Available After Migration

### 1. Refund Management
```csharp
// Create refund
var refund = new Refund
{
    PaymentId = payment.Id,
    BookingId = booking.Id,
    RefundNumber = GenerateRefundNumber(),
    Amount = refundAmount,
    Method = RefundMethod.Original,
    Status = RefundStatus.Pending
};
await context.Refunds.AddAsync(refund);
```

### 2. Notification System
```csharp
// Send booking confirmation notification
var notification = new Notification
{
    UserId = user.Id,
    BookingId = booking.Id,
    Type = NotificationType.BookingConfirmation,
    Subject = "Booking Confirmed",
    Message = $"Your booking {booking.Reference} is confirmed",
    Channel = NotificationChannel.Email
};
await context.Notifications.AddAsync(notification);
```

### 3. Recurring Schedules
```csharp
// Create weekly schedule
var schedule = new TrainSchedule
{
    TrainId = train.Id,
    RouteId = route.Id,
    Name = "Cairo-Aswan Express - Mondays",
    RecurrenceType = RecurrenceType.Weekly,
    DaysOfWeek = "[1]", // Monday
    DepartureTime = new TimeSpan(8, 0, 0),
    ArrivalTime = new TimeSpan(16, 0, 0),
    EffectiveFrom = DateTime.UtcNow,
    IsActive = true
};
```

### 4. Promotion Codes
```csharp
// Apply promotion to booking
var promotion = await context.Promotions
    .FirstOrDefaultAsync(p => p.Code == "SAVE20" && p.IsActive);

if (promotion != null && promotion.ValidFrom <= DateTime.UtcNow && promotion.ValidTo >= DateTime.UtcNow)
{
    var usage = new PromotionUsage
    {
        PromotionId = promotion.Id,
        BookingId = booking.Id,
        UserId = user.Id,
        DiscountAmount = CalculateDiscount(booking.TotalAmount, promotion)
    };
    await context.PromotionUsages.AddAsync(usage);
    promotion.UsageCount++;
}
```

## Performance Monitoring

### Monitor These Queries Post-Migration

```sql
-- Should use IX_Bookings_Reference (unique)
SELECT * FROM Bookings WHERE Reference = 'BK123456';

-- Should use IX_Bookings_UserId
SELECT * FROM Bookings WHERE UserId = 1;

-- Should use IX_TripSeats_TripId_Status
SELECT * FROM TripSeats WHERE TripId = 1 AND Status = 0;

-- Should use IX_Notifications_UserId_IsRead
SELECT * FROM Notifications WHERE UserId = 1 AND IsRead = 0;

-- Should use IX_Promotions_Code (unique)
SELECT * FROM Promotions WHERE Code = 'SAVE20';
```

Check execution plans - all should use index seeks, not scans.

## Troubleshooting

### Issue: Migration Fails with Constraint Violation

**Cause**: Duplicate data violating unique constraints

**Solution**:
1. Run pre-migration validation queries
2. Identify duplicates
3. Clean up duplicates manually
4. Retry migration

### Issue: Application Throws NullReferenceException for SeatNumber

**Cause**: Trying to access SeatNumber without loading navigation properties

**Solution**:
```csharp
// Include TripSeat and Seat in query
var bookingPassengers = await context.BookingPassengers
    .Include(bp => bp.TripSeat)
        .ThenInclude(ts => ts.Seat)
    .Where(bp => bp.BookingId == bookingId)
    .ToListAsync();

// Now bp.SeatNumber will work correctly
```

### Issue: Encrypted Fields Show Garbled Data

**Cause**: Encryption key changed or not properly configured

**Solution**:
1. Verify encryption key in ApplicationDBContext matches original
2. Check that `builder.UseEncryption(_encryptionProvider)` is called
3. Do NOT change the encryption key once data is encrypted!

## Cleanup (After 30 Days of Successful Operation)

```sql
-- Drop backup tables
DROP TABLE IF EXISTS BookingPassengers_Backup;
DROP TABLE IF EXISTS TripSeats_Backup;
```

## Migration Success Criteria

✅ All new tables created
✅ All indexes created and functioning
✅ Audit fields populated with default values
✅ Encryption working on sensitive fields
✅ Computed properties returning correct values
✅ No application errors in logs
✅ All test workflows passing
✅ Performance acceptable (check slow query log)

## Support

If issues persist:
1. Check application logs
2. Review migration SQL in the generated migration file
3. Run verification script
4. Consult rollback plan if needed
5. Create GitHub issue with error details

---

**Migration Created**: December 11, 2025
**Migration ID**: 20251213070231_ComprehensiveDatabaseImprovement
**Estimated Downtime**: 5-15 minutes (small DB), 30-60 minutes (large DB)
