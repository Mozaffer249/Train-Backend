using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class GeographyService : IGeographyService
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IStateRepository _stateRepository;
        private readonly ICityRepository _cityRepository;

        public GeographyService(
            IRegionRepository regionRepository,
            IStateRepository stateRepository,
            ICityRepository cityRepository)
        {
            _regionRepository = regionRepository;
            _stateRepository = stateRepository;
            _cityRepository = cityRepository;
        }

        #region Region Operations

        public async Task<RegionDto> CreateRegionAsync(string nameEn, string nameAr, string code)
        {
            var region = new Region
            {
                NameEn = nameEn,
                NameAr = nameAr,
                Code = code
            };

            await _regionRepository.AddAsync(region);

            return new RegionDto
            {
                Id = region.Id,
                NameEn = region.NameEn,
                NameAr = region.NameAr,
                Code = region.Code,
                StatesCount = 0
            };
        }

        public async Task<RegionDto?> GetRegionByIdAsync(int id)
        {
            var region = await _regionRepository.GetTableNoTracking()
                .Include(r => r.States)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (region == null)
                return null;

            return new RegionDto
            {
                Id = region.Id,
                NameEn = region.NameEn,
                NameAr = region.NameAr,
                Code = region.Code,
                StatesCount = region.States.Count
            };
        }

        public async Task<List<RegionDto>> GetAllRegionsAsync()
        {
            var regions = await _regionRepository.GetTableNoTracking()
                .Include(r => r.States)
                .OrderBy(r => r.NameEn)
                .Select(r => new RegionDto
                {
                    Id = r.Id,
                    NameEn = r.NameEn,
                    NameAr = r.NameAr,
                    Code = r.Code,
                    StatesCount = r.States.Count
                })
                .ToListAsync();

            return regions;
        }

        public async Task<RegionDto> UpdateRegionAsync(int id, string? nameEn, string? nameAr, string? code)
        {
            var region = await _regionRepository.GetTableNoTracking()
                .Include(r => r.States)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (region == null)
                throw new KeyNotFoundException($"Region with ID {id} not found");

            if (!string.IsNullOrEmpty(nameEn))
                region.NameEn = nameEn;

            if (!string.IsNullOrEmpty(nameAr))
                region.NameAr = nameAr;

            if (!string.IsNullOrEmpty(code))
                region.Code = code;

            await _regionRepository.UpdateAsync(region);

            return new RegionDto
            {
                Id = region.Id,
                NameEn = region.NameEn,
                NameAr = region.NameAr,
                Code = region.Code,
                StatesCount = region.States.Count
            };
        }

        public async Task<bool> DeleteRegionAsync(int id)
        {
            var region = await _regionRepository.GetByIdAsync(id);
            if (region == null)
                return false;

            await _regionRepository.DeleteAsync(region);
            return true;
        }

        public async Task<bool> IsRegionCodeUniqueAsync(string code, int? excludeId = null)
        {
            var query = _regionRepository.GetTableNoTracking()
                .Where(r => r.Code == code);

            if (excludeId.HasValue)
                query = query.Where(r => r.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> RegionHasStatesAsync(int id)
        {
            return await _stateRepository.GetTableNoTracking()
                .AnyAsync(s => s.RegionId == id);
        }

        #endregion

        #region State Operations

        public async Task<StateDto> CreateStateAsync(string nameEn, string nameAr, int regionId)
        {
            var state = new State
            {
                NameEn = nameEn,
                NameAr = nameAr,
                RegionId = regionId
            };

            await _stateRepository.AddAsync(state);

            var region = await _regionRepository.GetByIdAsync(regionId);

            return new StateDto
            {
                Id = state.Id,
                NameEn = state.NameEn,
                NameAr = state.NameAr,
                RegionId = state.RegionId,
                RegionName = region?.NameEn ?? "",
                CitiesCount = 0
            };
        }

        public async Task<StateDto?> GetStateByIdAsync(int id)
        {
            var state = await _stateRepository.GetTableNoTracking()
                .Include(s => s.Region)
                .Include(s => s.Cities)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (state == null)
                return null;

            return new StateDto
            {
                Id = state.Id,
                NameEn = state.NameEn,
                NameAr = state.NameAr,
                RegionId = state.RegionId,
                RegionName = state.Region.NameEn,
                CitiesCount = state.Cities.Count
            };
        }

        public async Task<List<StateDto>> GetAllStatesAsync(int? regionId = null)
        {
            var query = _stateRepository.GetTableNoTracking()
                .Include(s => s.Region)
                .Include(s => s.Cities)
                .AsQueryable();

            if (regionId.HasValue)
                query = query.Where(s => s.RegionId == regionId.Value);

            var states = await query
                .OrderBy(s => s.NameEn)
                .Select(s => new StateDto
                {
                    Id = s.Id,
                    NameEn = s.NameEn,
                    NameAr = s.NameAr,
                    RegionId = s.RegionId,
                    RegionName = s.Region.NameEn,
                    CitiesCount = s.Cities.Count
                })
                .ToListAsync();

            return states;
        }

        public async Task<StateDto> UpdateStateAsync(int id, string? nameEn, string? nameAr, int? regionId)
        {
            var state = await _stateRepository.GetTableNoTracking()
                .Include(s => s.Region)
                .Include(s => s.Cities)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (state == null)
                throw new KeyNotFoundException($"State with ID {id} not found");

            if (!string.IsNullOrEmpty(nameEn))
                state.NameEn = nameEn;

            if (!string.IsNullOrEmpty(nameAr))
                state.NameAr = nameAr;

            if (regionId.HasValue)
                state.RegionId = regionId.Value;

            await _stateRepository.UpdateAsync(state);

            // Reload region if it was changed
            if (regionId.HasValue)
            {
                state.Region = await _regionRepository.GetByIdAsync(regionId.Value) ?? state.Region;
            }

            return new StateDto
            {
                Id = state.Id,
                NameEn = state.NameEn,
                NameAr = state.NameAr,
                RegionId = state.RegionId,
                RegionName = state.Region.NameEn,
                CitiesCount = state.Cities.Count
            };
        }

        public async Task<bool> DeleteStateAsync(int id)
        {
            var state = await _stateRepository.GetByIdAsync(id);
            if (state == null)
                return false;

            await _stateRepository.DeleteAsync(state);
            return true;
        }

        public async Task<bool> StateHasCitiesAsync(int id)
        {
            return await _cityRepository.GetTableNoTracking()
                .AnyAsync(c => c.StateId == id);
        }

        #endregion

        #region City Operations

        public async Task<CityDto> CreateCityAsync(string nameEn, string nameAr, int stateId)
        {
            var city = new City
            {
                NameEn = nameEn,
                NameAr = nameAr,
                StateId = stateId
            };

            await _cityRepository.AddAsync(city);

            var state = await _stateRepository.GetTableNoTracking()
                .Include(s => s.Region)
                .FirstOrDefaultAsync(s => s.Id == stateId);

            return new CityDto
            {
                Id = city.Id,
                NameEn = city.NameEn,
                NameAr = city.NameAr,
                StateId = city.StateId,
                StateName = state?.NameEn ?? "",
                RegionName = state?.Region?.NameEn ?? "",
                StationsCount = 0
            };
        }

        public async Task<CityDto?> GetCityByIdAsync(int id)
        {
            var city = await _cityRepository.GetTableNoTracking()
                .Include(c => c.State).ThenInclude(s => s.Region)
                .Include(c => c.Stations)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
                return null;

            return new CityDto
            {
                Id = city.Id,
                NameEn = city.NameEn,
                NameAr = city.NameAr,
                StateId = city.StateId,
                StateName = city.State.NameEn,
                RegionName = city.State.Region.NameEn,
                StationsCount = city.Stations.Count
            };
        }

        public async Task<List<CityDto>> GetAllCitiesAsync(int? stateId = null)
        {
            var query = _cityRepository.GetTableNoTracking()
                .Include(c => c.State).ThenInclude(s => s.Region)
                .Include(c => c.Stations)
                .AsQueryable();

            if (stateId.HasValue)
                query = query.Where(c => c.StateId == stateId.Value);

            var cities = await query
                .OrderBy(c => c.NameEn)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    NameEn = c.NameEn,
                    NameAr = c.NameAr,
                    StateId = c.StateId,
                    StateName = c.State.NameEn,
                    RegionName = c.State.Region.NameEn,
                    StationsCount = c.Stations.Count
                })
                .ToListAsync();

            return cities;
        }

        public async Task<CityDto> UpdateCityAsync(int id, string? nameEn, string? nameAr, int? stateId)
        {
            var city = await _cityRepository.GetTableNoTracking()
                .Include(c => c.State).ThenInclude(s => s.Region)
                .Include(c => c.Stations)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
                throw new KeyNotFoundException($"City with ID {id} not found");

            if (!string.IsNullOrEmpty(nameEn))
                city.NameEn = nameEn;

            if (!string.IsNullOrEmpty(nameAr))
                city.NameAr = nameAr;

            if (stateId.HasValue)
                city.StateId = stateId.Value;

            await _cityRepository.UpdateAsync(city);

            // Reload state if it was changed
            if (stateId.HasValue)
            {
                city.State = await _stateRepository.GetTableNoTracking()
                    .Include(s => s.Region)
                    .FirstOrDefaultAsync(s => s.Id == stateId.Value) ?? city.State;
            }

            return new CityDto
            {
                Id = city.Id,
                NameEn = city.NameEn,
                NameAr = city.NameAr,
                StateId = city.StateId,
                StateName = city.State.NameEn,
                RegionName = city.State.Region.NameEn,
                StationsCount = city.Stations.Count
            };
        }

        public async Task<bool> DeleteCityAsync(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
                return false;

            await _cityRepository.DeleteAsync(city);
            return true;
        }

        public async Task<bool> CityHasStationsAsync(int id)
        {
            return await _cityRepository.GetTableNoTracking()
                .Where(c => c.Id == id)
                .SelectMany(c => c.Stations)
                .AnyAsync();
        }

        #endregion
    }
}

