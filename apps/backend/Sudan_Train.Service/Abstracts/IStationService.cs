using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Service.Abstracts
{
    public interface IStationService
    {
        Task<StationDto> CreateStationAsync(string code, string nameEn, string nameAr, int cityId, double latitude, double longitude, string? addressEn, string? addressAr);
        Task<StationDto?> GetStationByIdAsync(int id);
        Task<List<StationDto>> GetAllStationsAsync(int? cityId = null, string? searchTerm = null);
        Task<StationDto> UpdateStationAsync(int id, string? nameEn, string? nameAr, double? latitude, double? longitude, string? addressEn, string? addressAr);
        Task<bool> DeleteStationAsync(int id);
        Task<bool> IsStationCodeUniqueAsync(string code, int? excludeId = null);
        Task<bool> IsStationNameUniqueInCityAsync(string nameEn, string nameAr, int cityId, int? excludeId = null);
        Task<bool> CityExistsAsync(int cityId);
        Task<bool> StationIsUsedInRoutesAsync(int stationId);
    }
}


