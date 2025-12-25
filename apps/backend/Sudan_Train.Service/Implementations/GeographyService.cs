using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class GeographyService : IGeographyService
    {
        private readonly ICityRepository _cityRepository;

        public GeographyService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        #region City Operations

        public async Task<CityDto> CreateCityAsync(string nameEn, string nameAr, double latitude, double longitude, string? googlePlaceId, string? formattedAddress, string? boundaryPolygon, double? boundingBoxNorth, double? boundingBoxSouth, double? boundingBoxEast, double? boundingBoxWest)
        {
            var city = new City
            {
                NameEn = nameEn,
                NameAr = nameAr,
                Latitude = latitude,
                Longitude = longitude,
                GooglePlaceId = googlePlaceId,
                FormattedAddress = formattedAddress,
                BoundaryPolygon = boundaryPolygon,
                BoundingBoxNorth = boundingBoxNorth,
                BoundingBoxSouth = boundingBoxSouth,
                BoundingBoxEast = boundingBoxEast,
                BoundingBoxWest = boundingBoxWest,
                IsFromGoogle = !string.IsNullOrEmpty(googlePlaceId),
                GoogleSyncedAt = !string.IsNullOrEmpty(googlePlaceId) ? DateTime.UtcNow : null
            };

            await _cityRepository.AddAsync(city);

            return new CityDto
            {
                Id = city.Id,
                NameEn = city.NameEn,
                NameAr = city.NameAr,
                Latitude = city.Latitude,
                Longitude = city.Longitude,
                GooglePlaceId = city.GooglePlaceId,
                FormattedAddress = city.FormattedAddress,
                BoundaryPolygon = city.BoundaryPolygon,
                BoundingBoxNorth = city.BoundingBoxNorth,
                BoundingBoxSouth = city.BoundingBoxSouth,
                BoundingBoxEast = city.BoundingBoxEast,
                BoundingBoxWest = city.BoundingBoxWest,
                StationsCount = 0
            };
        }

        public async Task<CityDto?> GetCityByIdAsync(int id)
        {
            var city = await _cityRepository.GetTableNoTracking()
                .Include(c => c.Stations)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
                return null;

            return new CityDto
            {
                Id = city.Id,
                NameEn = city.NameEn,
                NameAr = city.NameAr,
                Latitude = city.Latitude,
                Longitude = city.Longitude,
                GooglePlaceId = city.GooglePlaceId,
                FormattedAddress = city.FormattedAddress,
                BoundaryPolygon = city.BoundaryPolygon,
                BoundingBoxNorth = city.BoundingBoxNorth,
                BoundingBoxSouth = city.BoundingBoxSouth,
                BoundingBoxEast = city.BoundingBoxEast,
                BoundingBoxWest = city.BoundingBoxWest,
                StationsCount = city.Stations.Count
            };
        }

        public async Task<List<CityDto>> GetAllCitiesAsync()
        {
            var cities = await _cityRepository.GetTableNoTracking()
                .Include(c => c.Stations)
                .OrderBy(c => c.Id)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    NameEn = c.NameEn,
                    NameAr = c.NameAr,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    GooglePlaceId = c.GooglePlaceId,
                    FormattedAddress = c.FormattedAddress,
                    BoundaryPolygon = c.BoundaryPolygon,
                    BoundingBoxNorth = c.BoundingBoxNorth,
                    BoundingBoxSouth = c.BoundingBoxSouth,
                    BoundingBoxEast = c.BoundingBoxEast,
                    BoundingBoxWest = c.BoundingBoxWest,
                    StationsCount = c.Stations.Count
                })
                .ToListAsync();

            return cities;
        }

        public async Task<CityDto> UpdateCityAsync(int id, string? nameEn, string? nameAr, double? latitude, double? longitude, string? googlePlaceId, string? formattedAddress, string? boundaryPolygon, double? boundingBoxNorth, double? boundingBoxSouth, double? boundingBoxEast, double? boundingBoxWest)
        {
            var city = await _cityRepository.GetTableNoTracking()
                .Include(c => c.Stations)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
                throw new KeyNotFoundException($"City with ID {id} not found");

            if (!string.IsNullOrEmpty(nameEn))
                city.NameEn = nameEn;

            if (!string.IsNullOrEmpty(nameAr))
                city.NameAr = nameAr;

            if (latitude.HasValue)
                city.Latitude = latitude.Value;

            if (longitude.HasValue)
                city.Longitude = longitude.Value;

            if (googlePlaceId != null)
            {
                city.GooglePlaceId = googlePlaceId;
                city.IsFromGoogle = !string.IsNullOrEmpty(googlePlaceId);
                city.GoogleSyncedAt = !string.IsNullOrEmpty(googlePlaceId) ? DateTime.UtcNow : null;
            }

            if (formattedAddress != null)
                city.FormattedAddress = formattedAddress;

            if (boundaryPolygon != null)
                city.BoundaryPolygon = boundaryPolygon;

            if (boundingBoxNorth.HasValue)
                city.BoundingBoxNorth = boundingBoxNorth.Value;

            if (boundingBoxSouth.HasValue)
                city.BoundingBoxSouth = boundingBoxSouth.Value;

            if (boundingBoxEast.HasValue)
                city.BoundingBoxEast = boundingBoxEast.Value;

            if (boundingBoxWest.HasValue)
                city.BoundingBoxWest = boundingBoxWest.Value;

            await _cityRepository.UpdateAsync(city);

            return new CityDto
            {
                Id = city.Id,
                NameEn = city.NameEn,
                NameAr = city.NameAr,
                Latitude = city.Latitude,
                Longitude = city.Longitude,
                GooglePlaceId = city.GooglePlaceId,
                FormattedAddress = city.FormattedAddress,
                BoundaryPolygon = city.BoundaryPolygon,
                BoundingBoxNorth = city.BoundingBoxNorth,
                BoundingBoxSouth = city.BoundingBoxSouth,
                BoundingBoxEast = city.BoundingBoxEast,
                BoundingBoxWest = city.BoundingBoxWest,
                StationsCount = city.Stations.Count
            };
        }

        public async Task<bool> DeleteCityAsync(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
                return false;

            // Check if city has stations
            if (await CityHasStationsAsync(id))
                throw new InvalidOperationException("Cannot delete city with existing stations");

            await _cityRepository.DeleteAsync(city);
            return true;
        }

        public async Task<bool> CityHasStationsAsync(int id)
        {
            return await _cityRepository.GetTableNoTracking()
                .AnyAsync(c => c.Id == id && c.Stations.Any());
        }

        #endregion
    }
}
