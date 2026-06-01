using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class FareService : IFareService
    {
        private readonly IFareRepository _fareRepository;
        private readonly IRouteRepository _routeRepository;

        public FareService(
            IFareRepository fareRepository,
            IRouteRepository routeRepository)
        {
            _fareRepository = fareRepository;
            _routeRepository = routeRepository;
        }

        public async Task<FareDto> CreateFareAsync(
            int? routeId,
            int? originStationId,
            int? destinationStationId,
            int? tripId,
            CoachClass coachClass,
            decimal basePrice,
            decimal? discountPercent)
        {
            var now = DateTime.UtcNow;

            // Auto-close any active fare with the EXACT same scope tuple + class
            // so the new row cleanly supersedes the old. Keeps the "at most one
            // active fare per (scope, class)" invariant. Different scopes (e.g.
            // route-level vs segment-level) do NOT collide — strict equality on
            // all four nullables.
            var existing = await _fareRepository.GetTableAsTracking()
                .Where(f => f.CoachClass == coachClass
                         && f.RouteId == routeId
                         && f.OriginStationId == originStationId
                         && f.DestinationStationId == destinationStationId
                         && f.TripId == tripId
                         && (f.EffectiveTo == null || f.EffectiveTo > now))
                .ToListAsync();

            foreach (var old in existing)
            {
                old.EffectiveTo = now;
                await _fareRepository.UpdateAsync(old);
            }

            var fare = new Fare
            {
                RouteId = routeId,
                OriginStationId = originStationId,
                DestinationStationId = destinationStationId,
                TripId = tripId,
                CoachClass = coachClass,
                BasePrice = basePrice,
                DiscountPercent = discountPercent,
                EffectiveFrom = now
            };

            await _fareRepository.AddAsync(fare);

            return MapToDto(fare);
        }

        public async Task<FareDto?> GetFareByIdAsync(int id)
        {
            var fare = await _fareRepository.GetTableNoTracking()
                .Include(f => f.Route)
                .Include(f => f.OriginStation)
                .Include(f => f.DestinationStation)
                .FirstOrDefaultAsync(f => f.Id == id);

            return fare == null ? null : MapToDto(fare);
        }

        public async Task<List<FareDto>> GetAllFaresAsync(int? routeId = null, CoachClass? coachClass = null)
        {
            var query = _fareRepository.GetTableNoTracking()
                .Include(f => f.Route)
                .Include(f => f.OriginStation)
                .Include(f => f.DestinationStation)
                .Where(f => f.EffectiveTo == null || f.EffectiveTo > DateTime.UtcNow)
                .AsQueryable();

            if (routeId.HasValue)
                query = query.Where(f => f.RouteId == routeId);

            if (coachClass.HasValue)
                query = query.Where(f => f.CoachClass == coachClass);

            var fares = await query.OrderBy(f => f.EffectiveFrom).ToListAsync();
            return fares.Select(f => MapToDto(f)).ToList();
        }

        public async Task<FareDto> UpdateFareAsync(
            int id,
            decimal? basePrice,
            decimal? discountPercent,
            DateTime? effectiveFrom,
            DateTime? effectiveTo)
        {
            var fare = await _fareRepository.GetByIdAsync(id);
            if (fare == null)
                throw new KeyNotFoundException($"Fare with ID {id} not found");

            if (basePrice.HasValue)
                fare.BasePrice = basePrice.Value;

            if (discountPercent.HasValue)
                fare.DiscountPercent = discountPercent;

            if (effectiveFrom.HasValue)
                fare.EffectiveFrom = effectiveFrom.Value;

            if (effectiveTo.HasValue)
                fare.EffectiveTo = effectiveTo;

            await _fareRepository.UpdateAsync(fare);

            return MapToDto(fare);
        }

        public async Task<bool> DeleteFareAsync(int id)
        {
            var fare = await _fareRepository.GetByIdAsync(id);
            if (fare == null)
                return false;

            await _fareRepository.DeleteAsync(fare);
            return true;
        }

        // Simple resolver kept for callers that just want a number. Mirrors the
        // GetApplicableFareAsync priority (Segment > Route) without breakdown.
        public async Task<decimal> CalculateFareAsync(int routeId, int originStationId, int destinationStationId, CoachClass coachClass)
        {
            var segmentFare = await _fareRepository.GetTableNoTracking()
                .Where(f => f.OriginStationId == originStationId &&
                           f.DestinationStationId == destinationStationId &&
                           f.CoachClass == coachClass &&
                           (f.EffectiveTo == null || f.EffectiveTo > DateTime.UtcNow))
                .OrderByDescending(f => f.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (segmentFare != null)
                return segmentFare.FinalPrice;

            var routeFare = await _fareRepository.GetTableNoTracking()
                .Where(f => f.RouteId == routeId &&
                           f.CoachClass == coachClass &&
                           (f.EffectiveTo == null || f.EffectiveTo > DateTime.UtcNow))
                .OrderByDescending(f => f.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (routeFare != null)
                return routeFare.FinalPrice;

            // Default fallback pricing when no fare is configured for this route.
            var route = await _routeRepository.GetByIdAsync(routeId);
            if (route?.DistanceKm != null)
            {
                var baseRate = coachClass switch
                {
                    CoachClass.First => 15m,
                    CoachClass.Second => 10m,
                    CoachClass.Third => 7m,
                    _ => 10m
                };
                return baseRate * route.DistanceKm.Value;
            }

            return 0;
        }

        public async Task<FareDto?> GetApplicableFareAsync(
            int? routeId,
            int? originStationId,
            int? destinationStationId,
            int? tripId,
            CoachClass? coachClass = null)
        {
            // Priority: Trip-specific > Segment-specific > Route-level.
            // When `coachClass` is null we're in "starting price" mode (search) —
            // drop the class filter and pick the cheapest match per scope.
            // When `coachClass` is set we're in booking mode and need an exact
            // class match for pricing to be honest.
            var baseQuery = _fareRepository.GetTableNoTracking()
                .Include(f => f.Route)
                .Include(f => f.OriginStation)
                .Include(f => f.DestinationStation)
                .Where(f => f.EffectiveTo == null || f.EffectiveTo > DateTime.UtcNow);

            if (coachClass.HasValue)
                baseQuery = baseQuery.Where(f => f.CoachClass == coachClass.Value);

            // For exact-class lookups we keep "newest fare wins" (OrderByDescending
            // EffectiveFrom). For class-less search we pick the cheapest active row.
            IQueryable<Fare> Apply(IQueryable<Fare> q) =>
                coachClass.HasValue
                    ? q.OrderByDescending(f => f.EffectiveFrom)
                    : q.OrderBy(f => f.BasePrice).ThenByDescending(f => f.EffectiveFrom);

            Fare? resolved = null;

            if (tripId.HasValue)
            {
                resolved = await Apply(baseQuery.Where(f => f.TripId == tripId))
                    .FirstOrDefaultAsync();
            }

            if (resolved == null && originStationId.HasValue && destinationStationId.HasValue)
            {
                resolved = await Apply(baseQuery.Where(f => f.OriginStationId == originStationId &&
                                                            f.DestinationStationId == destinationStationId))
                    .FirstOrDefaultAsync();
            }

            if (resolved == null && routeId.HasValue)
            {
                resolved = await Apply(baseQuery.Where(f => f.RouteId == routeId))
                    .FirstOrDefaultAsync();
            }

            return resolved == null ? null : MapToDto(resolved, withBreakdown: true);
        }

        private static FareDto MapToDto(Fare fare, bool withBreakdown = false)
        {
            var dto = new FareDto
            {
                Id = fare.Id,
                RouteId = fare.RouteId,
                OriginStationId = fare.OriginStationId,
                DestinationStationId = fare.DestinationStationId,
                TripId = fare.TripId,
                CoachClass = fare.CoachClass.ToString(),
                BasePrice = fare.BasePrice,
                DiscountPercent = fare.DiscountPercent,
                Currency = fare.Currency,
                FinalPrice = fare.FinalPrice,
                EffectiveFrom = fare.EffectiveFrom,
                EffectiveTo = fare.EffectiveTo,
            };

            if (withBreakdown)
                dto.Breakdown = BuildBreakdown(fare);

            return dto;
        }

        // Single source of truth for the receipt math. Reused by BookingService.
        public static FareBreakdownDto BuildBreakdown(Fare fare)
        {
            var basePrice = fare.BasePrice;
            var discountPct = fare.DiscountPercent ?? 0m;
            var discountAmount = basePrice * discountPct / 100m;
            var total = basePrice - discountAmount;

            return new FareBreakdownDto
            {
                BasePrice = basePrice,
                DiscountPercent = discountPct,
                DiscountAmount = discountAmount,
                Total = total,
                Currency = fare.Currency,
            };
        }
    }
}
