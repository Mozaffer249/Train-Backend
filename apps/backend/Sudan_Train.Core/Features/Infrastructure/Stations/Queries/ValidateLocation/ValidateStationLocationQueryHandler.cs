using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Core.Services.Spatial;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Queries.ValidateLocation
{
    public class ValidateStationLocationQueryHandler : ResponseHandler,
        IRequestHandler<ValidateStationLocationQuery, Response<StationValidationDto>>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IStationRepository _stationRepository;
        private readonly SpatialUtilityService _spatialUtility;

        public ValidateStationLocationQueryHandler(
            ICityRepository cityRepository,
            IStationRepository stationRepository,
            SpatialUtilityService spatialUtility,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _cityRepository = cityRepository;
            _stationRepository = stationRepository;
            _spatialUtility = spatialUtility;
        }

        public async Task<Response<StationValidationDto>> Handle(
            ValidateStationLocationQuery request,
            CancellationToken cancellationToken)
        {
            // Step 1: Check if selected city exists and has boundary
            var city = await _cityRepository.GetByIdAsync(request.CityId);

            if (city == null)
            {
                return BadRequest<StationValidationDto>("Selected city not found");
            }

            // Step 2: Check if point is within city boundary (if boundary exists)
            if (!string.IsNullOrEmpty(city.BoundaryPolygon))
            {
                bool isInsideCityBoundary = _spatialUtility.IsPointInPolygon(
                    request.Latitude,
                    request.Longitude,
                    city.BoundaryPolygon);

                if (!isInsideCityBoundary)
                {
                    return Success<StationValidationDto>(null, new StationValidationDto
                    {
                        IsValid = false,
                        Message = $"Station location is outside '{city.NameEn}' city boundaries. Please place station within the city."
                    });
                }
            }

            // Step 3: Check for nearby stations (within 500 meters)
            var allStations = await _stationRepository.GetAllAsync();
            var nearbyStations = allStations
                .Where(s => s.Id != request.ExcludeStationId)
                .Where(s => CalculateDistance(request.Latitude, request.Longitude, s.Latitude, s.Longitude) < 0.5) // 500m
                .ToList();

            if (nearbyStations.Any())
            {
                var nearest = nearbyStations.First();
                var distance = CalculateDistance(request.Latitude, request.Longitude, nearest.Latitude, nearest.Longitude);

                return Success<StationValidationDto>(null, new StationValidationDto
                {
                    IsValid = false,
                    Message = $"Station '{nearest.NameEn}' already exists {(distance * 1000):F0}m away from this location.",
                    ExistingStation = new StationDto
                    {
                        Id = nearest.Id,
                        Code = nearest.Code,
                        NameEn = nearest.NameEn,
                        NameAr = nearest.NameAr,
                        CityId = nearest.CityId,
                        Latitude = nearest.Latitude,
                        Longitude = nearest.Longitude
                    },
                    DistanceKm = distance
                });
            }

            // Step 4: Location is valid
            return Success<StationValidationDto>(null, new StationValidationDto
            {
                IsValid = true,
                Message = "Location is valid. You can add this station.",
                SuggestedData = new StationLocationSuggestion
                {
                    NameEn = "",
                    FormattedAddress = "",
                    CityId = city.Id,
                    CityName = city.NameEn
                }
            });
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth's radius in kilometers
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees) => degrees * Math.PI / 180;
    }
}
