using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Service.Abstracts
{
    public interface IRouteService
    {
        Task<RouteDto> CreateRouteAsync(int originStationId, int destinationStationId, string? nameEn, string? nameAr, decimal? distanceKm);
        Task<RouteDto?> GetRouteByIdAsync(int id);
        Task<List<RouteDto>> GetAllRoutesAsync(int? originStationId = null, int? destinationStationId = null);
        Task<RouteDto> UpdateRouteAsync(int id, string? nameEn, string? nameAr, decimal? distanceKm);
        Task<bool> DeleteRouteAsync(int id);
        Task<RouteStationDto> AddRouteStationAsync(int routeId, int stationId, int stopOrder, int arrivalMinutesFromOrigin, int departureMinutesFromOrigin);
        Task<bool> RemoveRouteStationAsync(int routeId, int stationId);
        Task<bool> RouteHasTripsAsync(int routeId);
    }
}

