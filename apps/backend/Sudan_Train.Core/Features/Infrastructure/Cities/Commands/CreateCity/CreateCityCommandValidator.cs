using FluentValidation;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Commands.CreateCity
{
    public class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
    {
        private readonly IGeographyService _geographyService;

        public CreateCityCommandValidator(IGeographyService geographyService)
        {
            _geographyService = geographyService;

            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage("English name is required")
                .Length(3, 100).WithMessage("English name must be between 3 and 100 characters");

            RuleFor(x => x.NameAr)
                .NotEmpty().WithMessage("Arabic name is required")
                .Length(3, 100).WithMessage("Arabic name must be between 3 and 100 characters");

            RuleFor(x => x.Latitude)
                .NotEmpty().WithMessage("Latitude is required")
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

            RuleFor(x => x.Longitude)
                .NotEmpty().WithMessage("Longitude is required")
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");

            // Duplicate name validation globally
            RuleFor(x => x)
                .MustAsync(BeUniqueName).WithMessage("A city with this name already exists");
        }

        private async Task<bool> BeUniqueName(CreateCityCommand command, CancellationToken cancellationToken)
        {
            return await _geographyService.IsCityNameUniqueAsync(command.NameEn, command.NameAr);
        }
    }
}


