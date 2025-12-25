using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Commands.UpdateCity
{
    public class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
    {
        private readonly ICityRepository _cityRepository;

        public UpdateCityCommandValidator(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("City ID is required")
                .MustAsync(CityExists).WithMessage("City not found");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue)
                .WithMessage("Latitude must be between -90 and 90");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue)
                .WithMessage("Longitude must be between -180 and 180");
        }

        private async Task<bool> CityExists(int cityId, CancellationToken cancellationToken)
        {
            return await _cityRepository.GetTableNoTracking().AnyAsync(c => c.Id == cityId, cancellationToken);
        }
    }
}


