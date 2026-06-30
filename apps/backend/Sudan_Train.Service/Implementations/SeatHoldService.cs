using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.AppMetaData;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Data.Entity;
using Sudan_Train.Data.Helpers;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Implementations;

namespace Sudan_Train.Service.Implementations
{
    public class SeatHoldService : ISeatHoldService
    {
        private readonly ApplicationDBContext _db;

        public SeatHoldService(ApplicationDBContext db)
        {
            _db = db;
        }

        public async Task<HoldSeatsResult> HoldSeatsAsync(
            int userId,
            int tripId,
            int boardingStationId,
            int alightingStationId,
            IReadOnlyList<int> seatIds,
            Guid? holdGroupId = null)
        {
            var distinctSeatIds = seatIds.Distinct().ToList();
            var groupId = holdGroupId ?? Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expiresAt = now + SeatHoldConstants.Duration;

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                await DeleteExpiredHoldsAsync(now);

                // Replace semantics: drop caller's prior holds on this trip+segment.
                var priorHolds = await _db.SeatHolds
                    .Where(h => h.UserId == userId && h.TripId == tripId
                             && h.BoardingStationId == boardingStationId
                             && h.AlightingStationId == alightingStationId)
                    .ToListAsync();
                if (priorHolds.Count > 0)
                {
                    _db.SeatHolds.RemoveRange(priorHolds);
                    await _db.SaveChangesAsync();
                }

                if (distinctSeatIds.Count == 0)
                {
                    await tx.CommitAsync();
                    return new HoldSeatsResult
                    {
                        Success = true,
                        Data = new SeatHoldResultDto { HoldGroupId = groupId, ExpiresAt = expiresAt, HeldSeatIds = new() },
                    };
                }

                var trip = await _db.Trip
                    .Include(t => t.Route).ThenInclude(r => r.RouteStations)
                    .Include(t => t.TripSeats).ThenInclude(ts => ts.Seat)
                    .FirstOrDefaultAsync(t => t.Id == tripId);

                if (trip == null)
                    return new HoldSeatsResult { Error = "Trip not found." };

                var bOrder = TripService.StopOrderOnRoute(trip.Route, boardingStationId);
                var aOrder = TripService.StopOrderOnRoute(trip.Route, alightingStationId);
                if (bOrder == null || aOrder == null || aOrder.Value <= bOrder.Value)
                    return new HoldSeatsResult { Error = "Invalid boarding/alighting stations for this trip." };

                var tripSeatBySeatId = trip.TripSeats.ToDictionary(ts => ts.SeatId, ts => ts);

                foreach (var seatId in distinctSeatIds)
                {
                    if (!tripSeatBySeatId.TryGetValue(seatId, out var tripSeat))
                        return new HoldSeatsResult { Conflict = true, Error = $"Seat {seatId} is not on this trip." };
                    if (tripSeat.Status == SeatStatus.Maintenance)
                        return new HoldSeatsResult { Conflict = true, Error = $"Seat {tripSeat.Seat?.SeatNumber} is out of service." };

                    var booked = await _db.BookingPassengers
                        .Where(bp => bp.TripId == tripId
                                  && bp.TripSeatId == tripSeat.Id
                                  && bp.Booking.Status != BookingStatus.Cancelled)
                        .Select(bp => new { bp.BoardingStationId, bp.AlightingStationId })
                        .ToListAsync();

                    foreach (var b in booked)
                    {
                        var cB = TripService.StopOrderOnRoute(trip.Route, b.BoardingStationId);
                        var cA = TripService.StopOrderOnRoute(trip.Route, b.AlightingStationId);
                        if (cB == null || cA == null) continue;
                        if (SegmentOverlapHelper.RangesOverlap(bOrder.Value, aOrder.Value, cB.Value, cA.Value))
                        {
                            return new HoldSeatsResult
                            {
                                Conflict = true,
                                Error = $"Seat {tripSeat.Seat?.SeatNumber} is already booked for this segment.",
                            };
                        }
                    }

                    var otherHolds = await _db.SeatHolds
                        .Where(h => h.TripId == tripId
                                 && h.TripSeatId == tripSeat.Id
                                 && h.UserId != userId
                                 && h.ExpiresAt > now)
                        .ToListAsync();

                    foreach (var h in otherHolds)
                    {
                        var hB = TripService.StopOrderOnRoute(trip.Route, h.BoardingStationId);
                        var hA = TripService.StopOrderOnRoute(trip.Route, h.AlightingStationId);
                        if (hB == null || hA == null) continue;
                        if (SegmentOverlapHelper.RangesOverlap(bOrder.Value, aOrder.Value, hB.Value, hA.Value))
                        {
                            return new HoldSeatsResult
                            {
                                Conflict = true,
                                Error = $"Seat {tripSeat.Seat?.SeatNumber} is temporarily held by another user.",
                            };
                        }
                    }
                }

                foreach (var seatId in distinctSeatIds)
                {
                    var tripSeat = tripSeatBySeatId[seatId];
                    _db.SeatHolds.Add(new SeatHold
                    {
                        HoldGroupId = groupId,
                        UserId = userId,
                        TripId = tripId,
                        TripSeatId = tripSeat.Id,
                        BoardingStationId = boardingStationId,
                        AlightingStationId = alightingStationId,
                        ExpiresAt = expiresAt,
                        CreatedAt = now,
                    });
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return new HoldSeatsResult
                {
                    Success = true,
                    Data = new SeatHoldResultDto
                    {
                        HoldGroupId = groupId,
                        ExpiresAt = expiresAt,
                        HeldSeatIds = distinctSeatIds,
                    },
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task ReleaseHoldsAsync(int userId, Guid? holdGroupId = null)
        {
            await DeleteExpiredHoldsAsync(DateTime.UtcNow);
            var query = _db.SeatHolds.Where(h => h.UserId == userId);
            if (holdGroupId.HasValue)
                query = query.Where(h => h.HoldGroupId == holdGroupId.Value);

            var holds = await query.ToListAsync();
            if (holds.Count > 0)
            {
                _db.SeatHolds.RemoveRange(holds);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<(bool Valid, string? Error)> ValidateHoldsAsync(
            int userId,
            int tripId,
            int boardingStationId,
            int alightingStationId,
            IReadOnlyList<int> seatIds)
        {
            if (seatIds.Count == 0)
                return (false, "At least one seat is required.");

            var now = DateTime.UtcNow;
            await DeleteExpiredHoldsAsync(now);

            var trip = await _db.Trip
                .Include(t => t.TripSeats)
                .FirstOrDefaultAsync(t => t.Id == tripId);
            if (trip == null)
                return (false, "Trip not found.");

            var tripSeatIdBySeatId = trip.TripSeats.ToDictionary(ts => ts.SeatId, ts => ts.Id);
            var requiredTripSeatIds = new HashSet<int>();

            foreach (var seatId in seatIds.Distinct())
            {
                if (!tripSeatIdBySeatId.TryGetValue(seatId, out var tsId))
                    return (false, $"Seat {seatId} is not on this trip.");
                requiredTripSeatIds.Add(tsId);
            }

            var activeHolds = await _db.SeatHolds
                .Where(h => h.UserId == userId
                         && h.TripId == tripId
                         && h.BoardingStationId == boardingStationId
                         && h.AlightingStationId == alightingStationId
                         && h.ExpiresAt > now
                         && requiredTripSeatIds.Contains(h.TripSeatId))
                .Select(h => h.TripSeatId)
                .Distinct()
                .ToListAsync();

            if (activeHolds.Count != requiredTripSeatIds.Count)
                return (false, "Seat hold expired or missing. Please re-select your seats.");

            return (true, null);
        }

        public async Task DeleteHoldsForSeatsAsync(int userId, int tripId, IReadOnlyList<int> seatIds)
        {
            if (seatIds.Count == 0) return;

            var tripSeatIds = await _db.TripSeats
                .Where(ts => ts.TripId == tripId && seatIds.Contains(ts.SeatId))
                .Select(ts => ts.Id)
                .ToListAsync();

            var holds = await _db.SeatHolds
                .Where(h => h.UserId == userId && h.TripId == tripId && tripSeatIds.Contains(h.TripSeatId))
                .ToListAsync();

            if (holds.Count > 0)
            {
                _db.SeatHolds.RemoveRange(holds);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<ActiveSeatHold>> GetActiveHoldsForTripAsync(int tripId, int? excludeUserId = null)
        {
            var now = DateTime.UtcNow;
            await DeleteExpiredHoldsAsync(now);

            var query = _db.SeatHolds
                .Include(h => h.TripSeat)
                .Where(h => h.TripId == tripId && h.ExpiresAt > now);

            if (excludeUserId.HasValue)
                query = query.Where(h => h.UserId != excludeUserId.Value);

            return await query
                .Select(h => new ActiveSeatHold
                {
                    UserId = h.UserId,
                    TripSeatId = h.TripSeatId,
                    SeatId = h.TripSeat.SeatId,
                    BoardingStationId = h.BoardingStationId,
                    AlightingStationId = h.AlightingStationId,
                })
                .ToListAsync();
        }

        private async Task DeleteExpiredHoldsAsync(DateTime now)
        {
            var expired = await _db.SeatHolds.Where(h => h.ExpiresAt <= now).ToListAsync();
            if (expired.Count > 0)
            {
                _db.SeatHolds.RemoveRange(expired);
                await _db.SaveChangesAsync();
            }
        }
    }
}
