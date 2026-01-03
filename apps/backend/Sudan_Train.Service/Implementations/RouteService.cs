using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IRouteStationRepository _routeStationRepository;
        private readonly IStationRepository _stationRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IDistanceCalculationService _distanceCalculationService;

        public RouteService(
            IRouteRepository routeRepository,
            IRouteStationRepository routeStationRepository,
            IStationRepository stationRepository,
            ITripRepository tripRepository,
            IDistanceCalculationService distanceCalculationService)
        {
            _routeRepository = routeRepository;
            _routeStationRepository = routeStationRepository;
            _stationRepository = stationRepository;
            _tripRepository = tripRepository;
            _distanceCalculationService = distanceCalculationService;
        }

        public async Task<RouteDto> CreateRouteAsync(int originStationId, int destinationStationId, string? nameEn, string? nameAr, decimal? distanceKm)
        {
            var originStation = await _stationRepository.GetTableNoTracking()
                .Include(s => s.City)
                .FirstOrDefaultAsync(s => s.Id == originStationId);

            var destinationStation = await _stationRepository.GetTableNoTracking()
                .Include(s => s.City)
                .FirstOrDefaultAsync(s => s.Id == destinationStationId);

            // Auto-generate route name if not provided
            var routeName = !string.IsNullOrEmpty(nameEn)
                ? nameEn
                : $"{originStation?.NameEn} to {destinationStation?.NameEn} Route";

            var routeNameAr = !string.IsNullOrEmpty(nameAr)
                ? nameAr
                : $"خط {originStation?.NameAr} إلى {destinationStation?.NameAr}";

            // Auto-calculate distance if not provided
            var calculatedDistance = distanceKm;
            if (!calculatedDistance.HasValue && originStation != null && destinationStation != null)
            {
                calculatedDistance = (decimal)_distanceCalculationService.CalculateDistance(
                    originStation.Latitude, originStation.Longitude,
                    destinationStation.Latitude, destinationStation.Longitude);
            }

            var route = new Route
            {
                NameEn = routeName,
                NameAr = routeNameAr,
                OriginStationId = originStationId,
                DestinationStationId = destinationStationId,
                DistanceKm = calculatedDistance,
                CreatedAt = DateTime.UtcNow
            };

            await _routeRepository.AddAsync(route);

            return new RouteDto
            {
                Id = route.Id,
                NameEn = route.NameEn ?? "",
                NameAr = route.NameAr ?? "",
                Origin = MapStationToDto(originStation),
                Destination = MapStationToDto(destinationStation),
                DistanceKm = route.DistanceKm,
                IsActive = route.IsActive,
                MaintenanceNote = route.MaintenanceNote,
                IntermediateStops = new List<RouteStationDto>(),
                TripsCount = 0
            };
        }

        public async Task<RouteDto?> GetRouteByIdAsync(int id)
        {
            var route = await _routeRepository.GetTableNoTracking()
                .Include(r => r.OriginStation).ThenInclude(s => s.City)
                .Include(r => r.DestinationStation).ThenInclude(s => s.City)
                .Include(r => r.RouteStations).ThenInclude(rs => rs.Station)
                .Include(r => r.Trips)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                return null;

            return new RouteDto
            {
                Id = route.Id,
                NameEn = route.NameEn ?? "",
                NameAr = route.NameAr ?? "",
                Origin = MapStationToDto(route.OriginStation),
                Destination = MapStationToDto(route.DestinationStation),
                DistanceKm = route.DistanceKm,
                IsActive = route.IsActive,
                MaintenanceNote = route.MaintenanceNote,
                IntermediateStops = route.RouteStations.OrderBy(rs => rs.StopOrder).Select(rs => new RouteStationDto
                {
                    Id = rs.Id,
                    StationId = rs.StationId,
                    StationName = rs.Station.NameEn,
                    StopOrder = rs.StopOrder,
                    ArrivalOffset = rs.ArrivalOffset,
                    DepartureOffset = rs.DepartureOffset
                }).ToList(),
                TripsCount = route.Trips.Count
            };
        }

        public async Task<List<RouteDto>> GetAllRoutesAsync(int? originStationId = null, int? destinationStationId = null, bool? isActive = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _routeRepository.GetTableNoTracking()
                .Include(r => r.OriginStation).ThenInclude(s => s.City)
                .Include(r => r.DestinationStation).ThenInclude(s => s.City)
                .Include(r => r.RouteStations).ThenInclude(rs => rs.Station)
                .Include(r => r.Trips)
                .AsQueryable();

            if (originStationId.HasValue)
                query = query.Where(r => r.OriginStationId == originStationId.Value);

            if (destinationStationId.HasValue)
                query = query.Where(r => r.DestinationStationId == destinationStationId.Value);

            if (isActive.HasValue)
                query = query.Where(r => r.IsActive == isActive.Value);

            var routes = await query
                .OrderBy(r => r.NameEn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return routes.Select(r => new RouteDto
            {
                Id = r.Id,
                NameEn = r.NameEn ?? "",
                NameAr = r.NameAr ?? "",
                Origin = MapStationToDto(r.OriginStation),
                Destination = MapStationToDto(r.DestinationStation),
                DistanceKm = r.DistanceKm,
                IsActive = r.IsActive,
                MaintenanceNote = r.MaintenanceNote,
                IntermediateStops = r.RouteStations.OrderBy(rs => rs.StopOrder).Select(rs => new RouteStationDto
                {
                    Id = rs.Id,
                    StationId = rs.StationId,
                    StationName = rs.Station.NameEn,
                    StopOrder = rs.StopOrder,
                    ArrivalOffset = rs.ArrivalOffset,
                    DepartureOffset = rs.DepartureOffset
                }).ToList(),
                TripsCount = r.Trips.Count
            }).ToList();
        }

        public async Task<RouteDto> UpdateRouteAsync(int id, int? originStationId, int? destinationStationId, string? nameEn, string? nameAr, decimal? distanceKm, bool? isActive, string? maintenanceNote)
        {
            var route = await _routeRepository.GetTableNoTracking()
                .Include(r => r.OriginStation).ThenInclude(s => s.City)
                .Include(r => r.DestinationStation).ThenInclude(s => s.City)
                .Include(r => r.RouteStations).ThenInclude(rs => rs.Station)
                .Include(r => r.Trips)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                throw new KeyNotFoundException($"Route with ID {id} not found");

            // Track if origin/destination changed for distance recalculation
            bool stationsChanged = false;

            // Update origin station
            if (originStationId.HasValue && originStationId.Value != route.OriginStationId)
            {
                route.OriginStationId = originStationId.Value;
                stationsChanged = true;
            }

            // Update destination station
            if (destinationStationId.HasValue && destinationStationId.Value != route.DestinationStationId)
            {
                route.DestinationStationId = destinationStationId.Value;
                stationsChanged = true;
            }

            // Auto-recalculate distance if stations changed and distance not manually provided
            if (stationsChanged && !distanceKm.HasValue)
            {
                var originStation = await _stationRepository.GetByIdAsync(route.OriginStationId);
                var destStation = await _stationRepository.GetByIdAsync(route.DestinationStationId);

                if (originStation != null && destStation != null)
                {
                    route.DistanceKm = (decimal)_distanceCalculationService.CalculateDistance(
                        originStation.Latitude, originStation.Longitude,
                        destStation.Latitude, destStation.Longitude);
                }
            }
            else if (distanceKm.HasValue)
            {
                route.DistanceKm = distanceKm.Value;
            }

            // Update other fields
            if (!string.IsNullOrEmpty(nameEn))
                route.NameEn = nameEn;

            if (!string.IsNullOrEmpty(nameAr))
                route.NameAr = nameAr;

            if (isActive.HasValue)
                route.IsActive = isActive.Value;

            if (maintenanceNote != null)
                route.MaintenanceNote = string.IsNullOrEmpty(maintenanceNote) ? null : maintenanceNote;

            route.UpdatedAt = DateTime.UtcNow;

            await _routeRepository.UpdateAsync(route);

            // Reload with updated relationships
            var updatedRoute = await GetRouteByIdAsync(id);
            return updatedRoute!;
        }

        public async Task<bool> DeleteRouteAsync(int id)
        {
            var route = await _routeRepository.GetByIdAsync(id);
            if (route == null)
                return false;

            await _routeRepository.DeleteAsync(route);
            return true;
        }

        public async Task<RouteStationDto> AddRouteStationAsync(int routeId, int stationId, int stopOrder, int arrivalMinutesFromOrigin, int departureMinutesFromOrigin)
        {
            var routeStation = new RouteStation
            {
                RouteId = routeId,
                StationId = stationId,
                StopOrder = stopOrder,
                ArrivalOffset = TimeSpan.FromMinutes(arrivalMinutesFromOrigin),
                DepartureOffset = TimeSpan.FromMinutes(departureMinutesFromOrigin)
            };

            await _routeStationRepository.AddAsync(routeStation);

            var station = await _stationRepository.GetByIdAsync(stationId);

            return new RouteStationDto
            {
                Id = routeStation.Id,
                StationId = routeStation.StationId,
                StationName = station?.NameEn ?? "",
                StopOrder = routeStation.StopOrder,
                ArrivalOffset = routeStation.ArrivalOffset,
                DepartureOffset = routeStation.DepartureOffset
            };
        }

        public async Task<RouteStationDto?> UpdateRouteStationAsync(int routeId, int stationId, int? stopOrder, int? arrivalMinutesFromOrigin, int? departureMinutesFromOrigin)
        {
            var routeStation = await _routeStationRepository.GetTableNoTracking()
                .Include(rs => rs.Station)
                .FirstOrDefaultAsync(rs => rs.RouteId == routeId && rs.StationId == stationId);

            if (routeStation == null)
                return null;

            if (stopOrder.HasValue)
                routeStation.StopOrder = stopOrder.Value;

            if (arrivalMinutesFromOrigin.HasValue)
                routeStation.ArrivalOffset = TimeSpan.FromMinutes(arrivalMinutesFromOrigin.Value);

            if (departureMinutesFromOrigin.HasValue)
                routeStation.DepartureOffset = TimeSpan.FromMinutes(departureMinutesFromOrigin.Value);

            await _routeStationRepository.UpdateAsync(routeStation);

            return new RouteStationDto
            {
                Id = routeStation.Id,
                StationId = routeStation.StationId,
                StationName = routeStation.Station?.NameEn ?? "",
                StopOrder = routeStation.StopOrder,
                ArrivalOffset = routeStation.ArrivalOffset,
                DepartureOffset = routeStation.DepartureOffset
            };
        }

        public async Task<bool> RemoveRouteStationAsync(int routeId, int stationId)
        {
            var routeStation = await _routeStationRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(rs => rs.RouteId == routeId && rs.StationId == stationId);

            if (routeStation == null)
                return false;

            await _routeStationRepository.DeleteAsync(routeStation);

            // Auto-resequence remaining stations
            await ResequenceRouteStationsAsync(routeId);

            return true;
        }

        private async Task ResequenceRouteStationsAsync(int routeId)
        {
            var routeStations = await _routeStationRepository.GetTableNoTracking()
                .Where(rs => rs.RouteId == routeId)
                .OrderBy(rs => rs.StopOrder)
                .ToListAsync();

            for (int i = 0; i < routeStations.Count; i++)
            {
                routeStations[i].StopOrder = i + 1;
                await _routeStationRepository.UpdateAsync(routeStations[i]);
            }
        }

        public async Task<bool> RouteHasTripsAsync(int routeId)
        {
            return await _tripRepository.GetTableNoTracking()
                .AnyAsync(t => t.RouteId == routeId);
        }

        private StationDto MapStationToDto(Station? station)
        {
            if (station == null)
                return new StationDto();

            return new StationDto
            {
                Id = station.Id,
                Code = station.Code,
                NameEn = station.NameEn,
                NameAr = station.NameAr,
                CityId = station.CityId,
                CityName = station.City?.NameEn ?? "",
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                AddressEn = station.AddressEn,
                AddressAr = station.AddressAr,
                IsActive = station.IsActive,
                MaintenanceNote = station.MaintenanceNote,
                CreatedAt = station.CreatedAt
            };
        }
    }
}

