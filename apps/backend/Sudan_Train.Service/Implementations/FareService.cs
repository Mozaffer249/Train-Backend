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
        private readonly IDistanceCalculationService _distanceCalculationService;

        public FareService(
            IFareRepository fareRepository,
            IRouteRepository routeRepository,
            IDistanceCalculationService distanceCalculationService)
        {
            _fareRepository = fareRepository;
            _routeRepository = routeRepository;
            _distanceCalculationService = distanceCalculationService;
        }

        public async Task<FareDto> CreateFareAsync(int? routeId, int? originStationId, int? destinationStationId, int? tripId, CoachClass coachClass, decimal basePrice, decimal? pricePerKm, decimal vatRate, decimal? discountPercent)
        {
            var fare = new Fare
            {
                RouteId = routeId,
                OriginStationId = originStationId,
                DestinationStationId = destinationStationId,
                TripId = tripId,
                CoachClass = coachClass,
                BasePrice = basePrice,
                PricePerKm = pricePerKm,
                VatRate = vatRate,
                DiscountPercent = discountPercent,
                EffectiveFrom = DateTime.UtcNow
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
            return fares.Select(MapToDto).ToList();
        }

        public async Task<FareDto> UpdateFareAsync(int id, decimal? basePrice, decimal? pricePerKm, decimal? vatRate, decimal? discountPercent, DateTime? effectiveTo)
        {
            var fare = await _fareRepository.GetByIdAsync(id);
            if (fare == null)
                throw new KeyNotFoundException($"Fare with ID {id} not found");

            if (basePrice.HasValue)
                fare.BasePrice = basePrice.Value;

            if (pricePerKm.HasValue)
                fare.PricePerKm = pricePerKm;

            if (vatRate.HasValue)
                fare.VatRate = vatRate.Value;

            if (discountPercent.HasValue)
                fare.DiscountPercent = discountPercent;

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

        public async Task<decimal> CalculateFareAsync(int routeId, int originStationId, int destinationStationId, CoachClass coachClass)
        {
            // Try to find exact segment fare
            var segmentFare = await _fareRepository.GetTableNoTracking()
                .Where(f => f.OriginStationId == originStationId &&
                           f.DestinationStationId == destinationStationId &&
                           f.CoachClass == coachClass &&
                           (f.EffectiveTo == null || f.EffectiveTo > DateTime.UtcNow))
                .OrderByDescending(f => f.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (segmentFare != null)
                return segmentFare.TotalWithVat;

            // Try route-level fare with distance calculation
            var routeFare = await _fareRepository.GetTableNoTracking()
                .Where(f => f.RouteId == routeId &&
                           f.CoachClass == coachClass &&
                           (f.EffectiveTo == null || f.EffectiveTo > DateTime.UtcNow))
                .OrderByDescending(f => f.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (routeFare != null && routeFare.PricePerKm.HasValue)
            {
                var distance = await _distanceCalculationService.CalculateRouteDistanceAsync(originStationId, destinationStationId, new List<int>());
                var calculatedPrice = routeFare.PricePerKm.Value * distance;
                var withDiscount = calculatedPrice - (calculatedPrice * (routeFare.DiscountPercent ?? 0) / 100);
                return withDiscount + (withDiscount * routeFare.VatRate);
            }

            // Default fallback pricing
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
                return baseRate * route.DistanceKm.Value * 1.15m; // Include 15% VAT
            }

            return 0;
        }

        public async Task<FareDto?> GetApplicableFareAsync(int? routeId, int? originStationId, int? destinationStationId, int? tripId, CoachClass coachClass)
        {
            // Priority: Trip-specific > Segment-specific > Route-level
            var query = _fareRepository.GetTableNoTracking()
                .Include(f => f.Route)
                .Include(f => f.OriginStation)
                .Include(f => f.DestinationStation)
                .Where(f => f.CoachClass == coachClass &&
                           (f.EffectiveTo == null || f.EffectiveTo > DateTime.UtcNow));

            // Check trip-specific fare
            if (tripId.HasValue)
            {
                var tripFare = await query
                    .Where(f => f.TripId == tripId)
                    .OrderByDescending(f => f.EffectiveFrom)
                    .FirstOrDefaultAsync();

                if (tripFare != null)
                    return MapToDto(tripFare);
            }

            // Check segment-specific fare
            if (originStationId.HasValue && destinationStationId.HasValue)
            {
                var segmentFare = await query
                    .Where(f => f.OriginStationId == originStationId &&
                               f.DestinationStationId == destinationStationId)
                    .OrderByDescending(f => f.EffectiveFrom)
                    .FirstOrDefaultAsync();

                if (segmentFare != null)
                    return MapToDto(segmentFare);
            }

            // Check route-level fare
            if (routeId.HasValue)
            {
                var routeFare = await query
                    .Where(f => f.RouteId == routeId)
                    .OrderByDescending(f => f.EffectiveFrom)
                    .FirstOrDefaultAsync();

                if (routeFare != null)
                    return MapToDto(routeFare);
            }

            return null;
        }

        private FareDto MapToDto(Fare fare)
        {
            return new FareDto
            {
                Id = fare.Id,
                RouteId = fare.RouteId,
                OriginStationId = fare.OriginStationId,
                DestinationStationId = fare.DestinationStationId,
                TripId = fare.TripId,
                CoachClass = fare.CoachClass.ToString(),
                BasePrice = fare.BasePrice,
                PricePerKm = fare.PricePerKm,
                VatRate = fare.VatRate,
                DiscountPercent = fare.DiscountPercent,
                Currency = fare.Currency,
                FinalPrice = fare.FinalPrice,
                TotalWithVat = fare.TotalWithVat,
                EffectiveFrom = fare.EffectiveFrom,
                EffectiveTo = fare.EffectiveTo
            };
        }
    }
}
