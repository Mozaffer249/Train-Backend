using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Service.Abstracts
{
    public interface IGeographyService
    {
        // City operations
        Task<CityDto> CreateCityAsync(string nameEn, string nameAr, double latitude, double longitude, string? googlePlaceId, string? formattedAddress, string? boundaryPolygon, double? boundingBoxNorth, double? boundingBoxSouth, double? boundingBoxEast, double? boundingBoxWest);
        Task<CityDto?> GetCityByIdAsync(int id);
        Task<List<CityDto>> GetAllCitiesAsync();
        Task<CityDto> UpdateCityAsync(int id, string? nameEn, string? nameAr, double? latitude, double? longitude, string? googlePlaceId, string? formattedAddress, string? boundaryPolygon, double? boundingBoxNorth, double? boundingBoxSouth, double? boundingBoxEast, double? boundingBoxWest);
        Task<bool> DeleteCityAsync(int id);
        Task<bool> CityHasStationsAsync(int id);
    }
}
