using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Core.Services.Google;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Infrastructure.Abstracts;
using System.Linq;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Queries.ValidateLocation
{
    public class ValidateCityLocationQueryHandler : ResponseHandler,
        IRequestHandler<ValidateCityLocationQuery, Response<CityValidationDto>>
    {
        private readonly IGoogleGeocodingService _googleGeocodingService;
        private readonly ICityRepository _cityRepository;

        public ValidateCityLocationQueryHandler(
            IGoogleGeocodingService googleGeocodingService,
            ICityRepository cityRepository,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _googleGeocodingService = googleGeocodingService;
            _cityRepository = cityRepository;
        }

        public async Task<Response<CityValidationDto>> Handle(
            ValidateCityLocationQuery request,
            CancellationToken cancellationToken)
        {
            // Step 1: Reverse geocode to get city name from coordinates
            var geocodeResult = await _googleGeocodingService.ReverseGeocode(
                request.Latitude,
                request.Longitude);

            if (geocodeResult == null)
            {
                return Success<CityValidationDto>(null, new CityValidationDto
                {
                    IsValid = true,
                    Message = "Could not determine city from coordinates. Please enter city name manually.",
                    SuggestedData = new CityLocationSuggestion
                    {
                        NameEn = "",
                        FormattedAddress = "",
                        GooglePlaceId = null
                    }
                });
            }

            // Step 2: Extract city name from address components
            var cityName = ExtractCityName(geocodeResult);

            if (string.IsNullOrEmpty(cityName))
            {
                return Success<CityValidationDto>(null, new CityValidationDto
                {
                    IsValid = true,
                    Message = "City name not found in location data. Please enter manually.",
                    SuggestedData = new CityLocationSuggestion
                    {
                        NameEn = "",
                        FormattedAddress = geocodeResult.FormattedAddress,
                        GooglePlaceId = geocodeResult.PlaceId
                    }
                });
            }

            // Step 3: Check if city with this name already exists
            var existingCity = await _cityRepository.GetByNameAsync(cityName);

            if (existingCity == null)
            {
                // City doesn't exist, validation passed
                return Success<CityValidationDto>(null, new CityValidationDto
                {
                    IsValid = true,
                    Message = "Location is valid. You can add this city.",
                    SuggestedData = new CityLocationSuggestion
                    {
                        NameEn = cityName,
                        FormattedAddress = geocodeResult.FormattedAddress,
                        GooglePlaceId = geocodeResult.PlaceId
                    }
                });
            }

            // Step 4: City exists, calculate distance
            var distance = CalculateDistance(
                request.Latitude, request.Longitude,
                existingCity.Latitude, existingCity.Longitude);

            // Step 5: Check if within 50km radius (same city)
            if (distance < 50)
            {
                return Success<CityValidationDto>(null, new CityValidationDto
                {
                    IsValid = false,
                    Message = $"City '{cityName}' already exists {distance:F1}km away from this location.",
                    ExistingCity = new CityDto
                    {
                        Id = existingCity.Id,
                        NameEn = existingCity.NameEn,
                        NameAr = existingCity.NameAr,
                        Latitude = existingCity.Latitude,
                        Longitude = existingCity.Longitude,
                        GooglePlaceId = existingCity.GooglePlaceId,
                        FormattedAddress = existingCity.FormattedAddress,
                        StationsCount = 0
                    },
                    DistanceKm = distance
                });
            }

            // City with same name exists but far away (different city with same name)
            return Success<CityValidationDto>(null, new CityValidationDto
            {
                IsValid = true,
                Message = $"Warning: A city named '{cityName}' exists {distance:F1}km away. Confirm this is a different city.",
                ExistingCity = new CityDto
                {
                    Id = existingCity.Id,
                    NameEn = existingCity.NameEn,
                    NameAr = existingCity.NameAr,
                    Latitude = existingCity.Latitude,
                    Longitude = existingCity.Longitude,
                    GooglePlaceId = existingCity.GooglePlaceId,
                    FormattedAddress = existingCity.FormattedAddress,
                    StationsCount = 0
                },
                SuggestedData = new CityLocationSuggestion
                {
                    NameEn = cityName,
                    FormattedAddress = geocodeResult.FormattedAddress,
                    GooglePlaceId = geocodeResult.PlaceId
                },
                DistanceKm = distance
            });
        }

        private string ExtractCityName(Services.Google.Models.GoogleResult geocodeResult)
        {
            // Try to find locality (city) in address components
            var cityComponent = geocodeResult.AddressComponents
                .FirstOrDefault(c => c.Types.Contains("locality"));

            if (cityComponent != null)
                return cityComponent.LongName;

            // Fallback to administrative_area_level_2
            var adminComponent = geocodeResult.AddressComponents
                .FirstOrDefault(c => c.Types.Contains("administrative_area_level_2"));

            if (adminComponent != null)
                return adminComponent.LongName;

            // Fallback to administrative_area_level_1
            var stateComponent = geocodeResult.AddressComponents
                .FirstOrDefault(c => c.Types.Contains("administrative_area_level_1"));

            return stateComponent?.LongName ?? string.Empty;
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula to calculate distance between two points on Earth
            const double R = 6371; // Earth's radius in kilometers

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c;

            return distance;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
