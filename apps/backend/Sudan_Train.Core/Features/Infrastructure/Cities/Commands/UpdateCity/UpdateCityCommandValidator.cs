using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Commands.UpdateCity
{
    public class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IGeographyService _geographyService;

        public UpdateCityCommandValidator(
            ICityRepository cityRepository,
            IGeographyService geographyService)
        {
            _cityRepository = cityRepository;
            _geographyService = geographyService;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("City ID is required")
                .MustAsync(CityExists).WithMessage("City not found");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue)
                .WithMessage("Latitude must be between -90 and 90");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue)
                .WithMessage("Longitude must be between -180 and 180");

            // Duplicate name validation globally
            RuleFor(x => x)
                .MustAsync(BeUniqueName).WithMessage("A city with this name already exists");
        }

        private async Task<bool> CityExists(int cityId, CancellationToken cancellationToken)
        {
            return await _cityRepository.GetTableNoTracking().AnyAsync(c => c.Id == cityId, cancellationToken);
        }

        private async Task<bool> BeUniqueName(UpdateCityCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(command.NameEn) && string.IsNullOrEmpty(command.NameAr))
                return true;

            return await _geographyService.IsCityNameUniqueAsync(command.NameEn, command.NameAr, command.Id);
        }
    }
}


