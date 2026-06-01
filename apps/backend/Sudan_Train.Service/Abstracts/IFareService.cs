using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Service.Abstracts
{
    public interface IFareService
    {
        Task<FareDto> CreateFareAsync(
            int? routeId,
            int? originStationId,
            int? destinationStationId,
            int? tripId,
            CoachClass coachClass,
            decimal basePrice,
            decimal? discountPercent);

        Task<FareDto?> GetFareByIdAsync(int id);
        Task<List<FareDto>> GetAllFaresAsync(int? routeId = null, CoachClass? coachClass = null);

        Task<FareDto> UpdateFareAsync(
            int id,
            decimal? basePrice,
            decimal? discountPercent,
            DateTime? effectiveFrom,
            DateTime? effectiveTo);

        Task<bool> DeleteFareAsync(int id);
        Task<decimal> CalculateFareAsync(int routeId, int originStationId, int destinationStationId, CoachClass coachClass);

        // Returned DTO carries the FareBreakdown for the resolved row.
        // When `coachClass` is null, the resolver drops the class filter and
        // returns the cheapest available fare for the scope (used by the
        // customer search where no class has been chosen yet).
        Task<FareDto?> GetApplicableFareAsync(
            int? routeId,
            int? originStationId,
            int? destinationStationId,
            int? tripId,
            CoachClass? coachClass = null);
    }
}
