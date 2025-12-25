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

        public RouteService(
            IRouteRepository routeRepository,
            IRouteStationRepository routeStationRepository,
            IStationRepository stationRepository,
            ITripRepository tripRepository)
        {
            _routeRepository = routeRepository;
            _routeStationRepository = routeStationRepository;
            _stationRepository = stationRepository;
            _tripRepository = tripRepository;
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

            var route = new Route
            {
                NameEn = routeName,
                NameAr = routeNameAr,
                OriginStationId = originStationId,
                DestinationStationId = destinationStationId,
                DistanceKm = distanceKm,
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

        public async Task<List<RouteDto>> GetAllRoutesAsync(int? originStationId = null, int? destinationStationId = null)
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

            var routes = await query
                .OrderBy(r => r.NameEn)
                .ToListAsync();

            return routes.Select(r => new RouteDto
            {
                Id = r.Id,
                NameEn = r.NameEn ?? "",
                NameAr = r.NameAr ?? "",
                Origin = MapStationToDto(r.OriginStation),
                Destination = MapStationToDto(r.DestinationStation),
                DistanceKm = r.DistanceKm,
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

        public async Task<RouteDto> UpdateRouteAsync(int id, string? nameEn, string? nameAr, decimal? distanceKm)
        {
            var route = await _routeRepository.GetTableNoTracking()
                .Include(r => r.OriginStation).ThenInclude(s => s.City)
                .Include(r => r.DestinationStation).ThenInclude(s => s.City)
                .Include(r => r.RouteStations).ThenInclude(rs => rs.Station)
                .Include(r => r.Trips)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                throw new KeyNotFoundException($"Route with ID {id} not found");

            if (!string.IsNullOrEmpty(nameEn))
                route.NameEn = nameEn;

            if (!string.IsNullOrEmpty(nameAr))
                route.NameAr = nameAr;

            if (distanceKm.HasValue)
                route.DistanceKm = distanceKm;

            route.UpdatedAt = DateTime.UtcNow;

            await _routeRepository.UpdateAsync(route);

            return new RouteDto
            {
                Id = route.Id,
                NameEn = route.NameEn ?? "",
                NameAr = route.NameAr ?? "",
                Origin = MapStationToDto(route.OriginStation),
                Destination = MapStationToDto(route.DestinationStation),
                DistanceKm = route.DistanceKm,
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

        public async Task<bool> RemoveRouteStationAsync(int routeId, int stationId)
        {
            var routeStation = await _routeStationRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(rs => rs.RouteId == routeId && rs.StationId == stationId);

            if (routeStation == null)
                return false;

            await _routeStationRepository.DeleteAsync(routeStation);
            return true;
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
                CreatedAt = station.CreatedAt
            };
        }
    }
}

