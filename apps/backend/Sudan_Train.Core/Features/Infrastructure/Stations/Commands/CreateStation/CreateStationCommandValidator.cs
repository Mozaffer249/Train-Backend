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
                .InclusiveBetween(8, 22).WithMessage("Latitude must be within Sudan boundaries (8-22)");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(21, 39).WithMessage("Longitude must be within Sudan boundaries (21-39)");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            return await _stationService.IsStationCodeUniqueAsync(code);
        }

        private async Task<bool> CityExists(int cityId, CancellationToken cancellationToken)
        {
            return await _cityRepository.GetTableNoTracking().AnyAsync(c => c.Id == cityId, cancellationToken);
        }
    }
}

