using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Commands.CreateStation
{
    public class CreateStationCommandValidator : AbstractValidator<CreateStationCommand>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IStationService _stationService;

        public CreateStationCommandValidator(ICityRepository cityRepository, IStationService stationService)
        {
            _cityRepository = cityRepository;
            _stationService = stationService;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Station code is required")
                .Length(3, 10).WithMessage("Station code must be between 3 and 10 characters")
                .Matches("^[A-Z0-9]+$").WithMessage("Station code must contain only uppercase letters and numbers")
                .MustAsync(BeUniqueCode).WithMessage("Station code already exists");

            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage("English name is required")
                .Length(3, 200).WithMessage("English name must be between 3 and 200 characters");

            RuleFor(x => x.NameAr)
                .Length(3, 200).When(x => !string.IsNullOrEmpty(x.NameAr))
                .WithMessage("Arabic name must be between 3 and 200 characters");

            RuleFor(x => x.CityId)
                .GreaterThan(0).WithMessage("City ID is required")
                .MustAsync(CityExists).WithMessage("City not found");

            RuleFor(x => x.Latitude)
                .NotEmpty().WithMessage("Latitude is required")
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

            RuleFor(x => x.Longitude)
                .NotEmpty().WithMessage("Longitude is required")
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");

            // Duplicate name validation within same city
            RuleFor(x => x)
                .MustAsync(BeUniqueName).WithMessage("A station with this name already exists in this city");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            return await _stationService.IsStationCodeUniqueAsync(code);
        }

        private async Task<bool> CityExists(int cityId, CancellationToken cancellationToken)
        {
            return await _cityRepository.GetTableNoTracking().AnyAsync(c => c.Id == cityId, cancellationToken);
        }

        private async Task<bool> BeUniqueName(CreateStationCommand command, CancellationToken cancellationToken)
        {
            return await _stationService.IsStationNameUniqueInCityAsync(command.NameEn, command.NameAr, command.CityId);
        }
    }
}

