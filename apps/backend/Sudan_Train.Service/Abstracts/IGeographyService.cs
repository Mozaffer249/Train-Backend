using Sudan_Train.Data.DTOs.Infrastructure;

namespace Sudan_Train.Service.Abstracts
{
    public interface IGeographyService
    {
        // Region operations
        Task<RegionDto> CreateRegionAsync(string nameEn, string nameAr, string code);
        Task<RegionDto?> GetRegionByIdAsync(int id);
        Task<List<RegionDto>> GetAllRegionsAsync();
        Task<RegionDto> UpdateRegionAsync(int id, string? nameEn, string? nameAr, string? code);
        Task<bool> DeleteRegionAsync(int id);
        Task<bool> IsRegionCodeUniqueAsync(string code, int? excludeId = null);
        Task<bool> RegionHasStatesAsync(int id);

        // State operations
        Task<StateDto> CreateStateAsync(string nameEn, string nameAr, int regionId);
        Task<StateDto?> GetStateByIdAsync(int id);
        Task<List<StateDto>> GetAllStatesAsync(int? regionId = null);
        Task<StateDto> UpdateStateAsync(int id, string? nameEn, string? nameAr, int? regionId);
        Task<bool> DeleteStateAsync(int id);
        Task<bool> StateHasCitiesAsync(int id);

        // City operations
        Task<CityDto> CreateCityAsync(string nameEn, string nameAr, int stateId);
        Task<CityDto?> GetCityByIdAsync(int id);
        Task<List<CityDto>> GetAllCitiesAsync(int? stateId = null);
        Task<CityDto> UpdateCityAsync(int id, string? nameEn, string? nameAr, int? stateId);
        Task<bool> DeleteCityAsync(int id);
        Task<bool> CityHasStationsAsync(int id);
    }
}

