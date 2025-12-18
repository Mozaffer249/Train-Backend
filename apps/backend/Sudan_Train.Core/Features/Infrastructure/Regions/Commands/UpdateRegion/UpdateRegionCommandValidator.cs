using FluentValidation;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Regions.Commands.UpdateRegion
{
    public class UpdateRegionCommandValidator : AbstractValidator<UpdateRegionCommand>
    {
        private readonly IGeographyService _geographyService;

        public UpdateRegionCommandValidator(IGeographyService geographyService)
        {
            _geographyService = geographyService;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Region ID is required");

            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage("English name is required")
                .Length(3, 100).WithMessage("English name must be between 3 and 100 characters");

            RuleFor(x => x.NameAr)
                .Length(3, 100).When(x => !string.IsNullOrEmpty(x.NameAr))
                .WithMessage("Arabic name must be between 3 and 100 characters");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required")
                .Length(2, 10).WithMessage("Code must be between 2 and 10 characters")
                .Matches("^[A-Z0-9]+$").WithMessage("Code must contain only uppercase letters and numbers")
                .MustAsync(BeUniqueCode).WithMessage("Region code already exists");
        }

        private async Task<bool> BeUniqueCode(UpdateRegionCommand command, string code, CancellationToken cancellationToken)
        {
            return await _geographyService.IsRegionCodeUniqueAsync(code, command.Id);
        }
    }
}

