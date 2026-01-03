using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Service.Abstracts
{
    public interface IRouteService
    {
        Task<RouteDto> CreateRouteAsync(int originStationId, int destinationStationId, string? nameEn, string? nameAr, decimal? distanceKm);
        Task<RouteDto?> GetRouteByIdAsync(int id);
        Task<List<RouteDto>> GetAllRoutesAsync(int? originStationId = null, int? destinationStationId = null, bool? isActive = null, int pageNumber = 1, int pageSize = 10);
        Task<RouteDto> UpdateRouteAsync(int id, int? originStationId, int? destinationStationId, string? nameEn, string? nameAr, decimal? distanceKm, bool? isActive, string? maintenanceNote);
        Task<bool> DeleteRouteAsync(int id);
        Task<RouteStationDto> AddRouteStationAsync(int routeId, int stationId, int stopOrder, int arrivalMinutesFromOrigin, int departureMinutesFromOrigin);
        Task<RouteStationDto?> UpdateRouteStationAsync(int routeId, int stationId, int? stopOrder, int? arrivalMinutesFromOrigin, int? departureMinutesFromOrigin);
        Task<bool> RemoveRouteStationAsync(int routeId, int stationId);
        Task<bool> RouteHasTripsAsync(int routeId);
    }
}

