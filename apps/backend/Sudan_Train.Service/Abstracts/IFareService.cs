using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;

namespace Sudan_Train.Service.Abstracts
{
    public interface IFareService
    {
        Task<FareDto> CreateFareAsync(int? routeId, int? originStationId, int? destinationStationId, int? tripId, CoachClass coachClass, decimal basePrice, decimal? pricePerKm, decimal vatRate, decimal? discountPercent);
        Task<FareDto?> GetFareByIdAsync(int id);
        Task<List<FareDto>> GetAllFaresAsync(int? routeId = null, CoachClass? coachClass = null);
        Task<FareDto> UpdateFareAsync(int id, decimal? basePrice, decimal? pricePerKm, decimal? vatRate, decimal? discountPercent, DateTime? effectiveTo);
        Task<bool> DeleteFareAsync(int id);
        Task<decimal> CalculateFareAsync(int routeId, int originStationId, int destinationStationId, CoachClass coachClass);
        Task<FareDto?> GetApplicableFareAsync(int? routeId, int? originStationId, int? destinationStationId, int? tripId, CoachClass coachClass);
    }
}
