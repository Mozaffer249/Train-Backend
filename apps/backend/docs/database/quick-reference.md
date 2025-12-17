# Database Quick Reference Guide

## 🚀 Quick Commands

### Apply the Migration
```bash
# With Docker (Recommended)
docker-compose down
docker-compose up -d --build

# Without Docker
dotnet ef database update --project Sudan_Train.Infrastructure --startup-project Sudan_Train
```

### Rollback if Needed
```bash
dotnet ef database update InitialCreate --project Sudan_Train.Infrastructure --startup-project Sudan_Train
```

---

## 📊 New Entities - Quick Reference

### Refund
```csharp
var refund = new Refund
{
    PaymentId = payment.Id,
    BookingId = booking.Id,
    RefundNumber = $"REF{DateTime.UtcNow:yyyyMMddHHmmss}",
    Amount = 100.00m,
    Currency = "SDG",
    Method = RefundMethod.Original,
    Status = RefundStatus.Pending,
    Reason = "Customer cancellation"
};
```

### Notification
```csharp
var notification = new Notification
{
    UserId = user.Id,
    BookingId = booking.Id,
    Type = NotificationType.BookingConfirmation,
    Subject = "Booking Confirmed",
    Message = "Your booking has been confirmed",
    Channel = NotificationChannel.Email,
    IsSent = false
};
```

### TrainSchedule
```csharp
var schedule = new TrainSchedule
{
    TrainId = train.Id,
    RouteId = route.Id,
    Name = "Express Daily Service",
    RecurrenceType = RecurrenceType.Daily,
    DepartureTime = new TimeSpan(8, 0, 0),
    ArrivalTime = new TimeSpan(16, 0, 0),
    EffectiveFrom = DateTime.UtcNow,
    IsActive = true
};
```

### Promotion
```csharp
var promotion = new Promotion
{
    Code = "SAVE20",
    NameEn = "20% Off Special",
    NameAr = "خصم 20٪",
    Type = PromotionType.Percentage,
    DiscountValue = 20m,
    MaxDiscount = 50m,
    MinimumPurchase = 100m,
    ValidFrom = DateTime.UtcNow,
    ValidTo = DateTime.UtcNow.AddDays(30),
    IsActive = true
};
```

### PromotionUsage
```csharp
var usage = new PromotionUsage
{
    PromotionId = promotion.Id,
    BookingId = booking.Id,
    UserId = user.Id,
    DiscountAmount = 40.00m
};
promotion.UsageCount++;
```

---

## 🔍 Common Queries

### Get Booking with All Details
```csharp
var booking = await context.Bookings
    .Include(b => b.User)
    .Include(b => b.BookingPassengers)
        .ThenInclude(bp => bp.Passenger)
    .Include(b => b.BookingPassengers)
        .ThenInclude(bp => bp.TripSeat)
            .ThenInclude(ts => ts.Seat)
                .ThenInclude(s => s.Coach)
    .Include(b => b.BookingPassengers)
        .ThenInclude(bp => bp.Trip)
            .ThenInclude(t => t.Train)
    .Include(b => b.Payments)
    .Include(b => b.Refunds)
    .Include(b => b.PromotionUsages)
        .ThenInclude(pu => pu.Promotion)
    .FirstOrDefaultAsync(b => b.Reference == reference);
```

### Get Available Seats for Trip
```csharp
var availableSeats = await context.TripSeats
    .Include(ts => ts.Seat)
        .ThenInclude(s => s.Coach)
    .Where(ts => ts.TripId == tripId && ts.Status == SeatStatus.Available)
    .OrderBy(ts => ts.Seat.Coach.Sequence)
    .ThenBy(ts => ts.Seat.SeatNumber)
    .ToListAsync();
```

### Get User's Unread Notifications
```csharp
var notifications = await context.Notifications
    .Where(n => n.UserId == userId && !n.IsRead)
    .OrderByDescending(n => n.CreatedAt)
    .Take(20)
    .ToListAsync();
```

### Validate and Apply Promotion
```csharp
var promotion = await context.Promotions
    .Where(p => p.Code == code 
             && p.IsActive
             && p.ValidFrom <= DateTime.UtcNow
             && p.ValidTo >= DateTime.UtcNow)
    .FirstOrDefaultAsync();

if (promotion != null)
{
    // Check usage limit
    if (promotion.MaxUsageCount.HasValue && promotion.UsageCount >= promotion.MaxUsageCount)
        return "Promotion limit reached";
    
    // Check minimum purchase
    if (promotion.MinimumPurchase.HasValue && totalAmount < promotion.MinimumPurchase)
        return "Minimum purchase not met";
    
    // Calculate discount
    decimal discount = promotion.Type switch
    {
        PromotionType.Percentage => totalAmount * (promotion.DiscountValue / 100),
        PromotionType.FixedAmount => promotion.DiscountValue,
        _ => 0
    };
    
    // Apply max discount cap
    if (promotion.MaxDiscount.HasValue)
        discount = Math.Min(discount, promotion.MaxDiscount.Value);
    
    return discount;
}
```

### Process Refund
```csharp
var refund = new Refund
{
    PaymentId = payment.Id,
    BookingId = booking.Id,
    RefundNumber = GenerateRefundNumber(),
    Amount = refundAmount,
    Currency = payment.Currency,
    Method = RefundMethod.Original,
    Status = RefundStatus.Pending,
    Reason = reason
};

await context.Refunds.AddAsync(refund);

booking.Status = BookingStatus.Cancelled;
booking.CancelledAt = DateTime.UtcNow;
booking.CancelledBy = currentUserId;
booking.RefundAmount = refundAmount;

await context.SaveChangesAsync();
```

---

## ⚡ Performance Tips

### Always Use Indexes
```csharp
// GOOD - Uses IX_Bookings_Reference
var booking = await context.Bookings
    .FirstOrDefaultAsync(b => b.Reference == reference);

// GOOD - Uses IX_TripSeats_TripId_Status
var seats = await context.TripSeats
    .Where(ts => ts.TripId == tripId && ts.Status == SeatStatus.Available)
    .ToListAsync();

// BAD - Table scan
var bookings = await context.Bookings
    .Where(b => b.TotalAmount > 1000) // No index on TotalAmount
    .ToListAsync();
```

### Avoid N+1 Queries
```csharp
// BAD - N+1 query problem
var bookings = await context.Bookings.ToListAsync();
foreach (var booking in bookings)
{
    var user = await context.Users.FindAsync(booking.UserId); // N queries!
}

// GOOD - Single query with Include
var bookings = await context.Bookings
    .Include(b => b.User)
    .ToListAsync();
```

### Use Projections for Large Datasets
```csharp
// BAD - Loads entire entities
var data = await context.Bookings
    .Include(b => b.BookingPassengers)
    .ToListAsync();

// GOOD - Only select what you need
var data = await context.Bookings
    .Select(b => new {
        b.Id,
        b.Reference,
        b.TotalAmount,
        PassengerCount = b.BookingPassengers.Count
    })
    .ToListAsync();
```

---

## 🔐 Security Considerations

### Encrypted Fields
These fields are automatically encrypted/decrypted:
- `User.Code`
- `Passenger.IdNumber`
- `Payment.CardToken`
- `Payment.ProcessorResponse`

**Important**: 
- ❌ Never log encrypted fields
- ❌ Never expose in API responses
- ❌ Never change encryption key after data exists
- ✅ Encryption is transparent to application code

### Sensitive Data Access
```csharp
// Access is normal - encryption is handled by EF Core
var idNumber = passenger.IdNumber; // Automatically decrypted

// Saving is normal too
passenger.IdNumber = "123456789"; // Automatically encrypted on save
await context.SaveChangesAsync();
```

---

## 🛠️ Troubleshooting

### "Column SeatNumber does not exist"
**Cause**: Migration applied, but code still references old column

**Fix**: Update queries to include navigation properties:
```csharp
.Include(bp => bp.TripSeat).ThenInclude(ts => ts.Seat)
```

### "Cannot insert NULL into column 'CreatedAt'"
**Cause**: Trying to insert entity without setting CreatedAt

**Fix**: CreatedAt has default value, ensure it's set:
```csharp
train.CreatedAt = DateTime.UtcNow; // Or let default work
```

### Slow Queries After Migration
**Cause**: Missing index or not using indexes

**Fix**: 
1. Check query execution plan
2. Ensure you're filtering by indexed columns
3. Verify indexes were created

### Promotion Code Not Working
**Cause**: Case sensitivity or date validation

**Fix**:
```csharp
var promotion = await context.Promotions
    .Where(p => p.Code.ToUpper() == code.ToUpper() // Case insensitive
             && p.IsActive
             && p.ValidFrom <= DateTime.UtcNow
             && p.ValidTo >= DateTime.UtcNow)
    .FirstOrDefaultAsync();
```

---

## 📝 Cheat Sheet

### Enums Quick Reference
```
BookingStatus: Pending(0), Confirmed(1), Cancelled(2), Completed(3)
PaymentStatus: Pending(0), Completed(1), Failed(2), Refunded(3)
RefundStatus: Pending(0), Approved(1), Rejected(2), Completed(3)
NotificationType: BookingConfirmation(0), BookingCancellation(1), PaymentReceived(2), 
                  TripDelay(3), TripCancellation(4), PromotionalOffer(5), SystemAlert(6)
NotificationChannel: Email(0), SMS(1), Push(2), InApp(3)
PromotionType: Percentage(0), FixedAmount(1), BuyOneGetOne(2)
```

### Required Includes for Computed Properties
```csharp
// For BookingPassenger.SeatNumber
.Include(bp => bp.TripSeat).ThenInclude(ts => ts.Seat)

// For TripSeat.Coach
.Include(ts => ts.Seat).ThenInclude(s => s.Coach)
```

---

**Last Updated**: December 11, 2025  
**Version**: 1.0  
**Migration**: ComprehensiveDatabaseImprovement
