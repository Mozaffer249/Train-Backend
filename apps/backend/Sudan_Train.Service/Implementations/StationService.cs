using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IRouteRepository _routeRepository;

        public StationService(
            IStationRepository stationRepository,
            ICityRepository cityRepository,
            IRouteRepository routeRepository)
        {
            _stationRepository = stationRepository;
            _cityRepository = cityRepository;
            _routeRepository = routeRepository;
        }

        public async Task<StationDto> CreateStationAsync(string code, string nameEn, string nameAr, int cityId, double latitude, double longitude, string? addressEn, string? addressAr)
        {
            var station = new Station
            {
                Code = code,
                NameEn = nameEn,
                NameAr = nameAr,
                CityId = cityId,
                Latitude = latitude,
                Longitude = longitude,
                AddressEn = addressEn,
                AddressAr = addressAr,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _stationRepository.AddAsync(station);

            var city = await _cityRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(c => c.Id == cityId);

            return new StationDto
            {
                Id = station.Id,
                Code = station.Code,
                NameEn = station.NameEn,
                NameAr = station.NameAr,
                CityId = station.CityId,
                CityName = city?.NameEn ?? "",
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                AddressEn = station.AddressEn,
                AddressAr = station.AddressAr,
                CreatedAt = station.CreatedAt
            };
        }

        public async Task<StationDto?> GetStationByIdAsync(int id)
        {
            var station = await _stationRepository.GetTableNoTracking()
                .Include(s => s.City)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (station == null)
                return null;

            return new StationDto
            {
                Id = station.Id,
                Code = station.Code,
                NameEn = station.NameEn,
                NameAr = station.NameAr,
                CityId = station.CityId,
                CityName = station.City.NameEn,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                AddressEn = station.AddressEn,
                AddressAr = station.AddressAr,
                CreatedAt = station.CreatedAt
            };
        }

        public async Task<List<StationDto>> GetAllStationsAsync(int? cityId = null, string? searchTerm = null)
        {
            var query = _stationRepository.GetTableNoTracking()
                .Include(s => s.City)
                .AsQueryable();

            if (cityId.HasValue)
                query = query.Where(s => s.CityId == cityId.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearch = searchTerm.ToLower();
                query = query.Where(s =>
                    s.NameEn.ToLower().Contains(lowerSearch) ||
                    s.NameAr.Contains(searchTerm) ||
                    s.Code.ToLower().Contains(lowerSearch));
            }

            var stations = await query
                .OrderBy(s => s.NameEn)
                .Select(s => new StationDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    NameEn = s.NameEn,
                    NameAr = s.NameAr,
                    CityId = s.CityId,
                    CityName = s.City.NameEn,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    AddressEn = s.AddressEn,
                    AddressAr = s.AddressAr,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return stations;
        }

        public async Task<StationDto> UpdateStationAsync(int id, string? nameEn, string? nameAr, double? latitude, double? longitude, string? addressEn, string? addressAr)
        {
            var station = await _stationRepository.GetTableNoTracking()
                .Include(s => s.City)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (station == null)
                throw new KeyNotFoundException($"Station with ID {id} not found");

            if (!string.IsNullOrEmpty(nameEn))
                station.NameEn = nameEn;

            if (!string.IsNullOrEmpty(nameAr))
                station.NameAr = nameAr;

            if (latitude.HasValue)
                station.Latitude = latitude.Value;

            if (longitude.HasValue)
                station.Longitude = longitude.Value;

            if (!string.IsNullOrEmpty(addressEn))
                station.AddressEn = addressEn;

            if (!string.IsNullOrEmpty(addressAr))
                station.AddressAr = addressAr;

            station.UpdatedAt = DateTime.UtcNow;

            await _stationRepository.UpdateAsync(station);

            return new StationDto
            {
                Id = station.Id,
                Code = station.Code,
                NameEn = station.NameEn,
                NameAr = station.NameAr,
                CityId = station.CityId,
                CityName = station.City.NameEn,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                AddressEn = station.AddressEn,
                AddressAr = station.AddressAr,
                CreatedAt = station.CreatedAt
            };
        }

        public async Task<bool> DeleteStationAsync(int id)
        {
            var station = await _stationRepository.GetByIdAsync(id);
            if (station == null)
                return false;

            await _stationRepository.DeleteAsync(station);
            return true;
        }

        public async Task<bool> IsStationCodeUniqueAsync(string code, int? excludeId = null)
        {
            var query = _stationRepository.GetTableNoTracking()
                .Where(s => s.Code == code);

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsStationNameUniqueInCityAsync(string nameEn, string nameAr, int cityId, int? excludeId = null)
        {
            var query = _stationRepository.GetTableNoTracking()
                .Where(s => s.CityId == cityId &&
                    (s.NameEn.ToLower() == nameEn.ToLower() || s.NameAr == nameAr));

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _cityRepository.GetTableNoTracking()
                .AnyAsync(c => c.Id == cityId);
        }

        public async Task<bool> StationIsUsedInRoutesAsync(int stationId)
        {
            return await _routeRepository.GetTableNoTracking()
                .AnyAsync(r => r.OriginStationId == stationId || r.DestinationStationId == stationId);
        }
    }
}


