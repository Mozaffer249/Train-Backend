using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.States.Commands.CreateState
{
    public class CreateStateCommandValidator : AbstractValidator<CreateStateCommand>
    {
        private readonly IRegionRepository _regionRepository;

        public CreateStateCommandValidator(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;

            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage("English name is required")
                .Length(3, 100).WithMessage("English name must be between 3 and 100 characters");

            RuleFor(x => x.NameAr)
                .Length(3, 100).When(x => !string.IsNullOrEmpty(x.NameAr))
                .WithMessage("Arabic name must be between 3 and 100 characters");

            RuleFor(x => x.RegionId)
                .GreaterThan(0).WithMessage("Region ID is required")
                .MustAsync(RegionExists).WithMessage("Region not found");
        }

        private async Task<bool> RegionExists(int regionId, CancellationToken cancellationToken)
        {
            return await _regionRepository.GetTableNoTracking().AnyAsync(r => r.Id == regionId, cancellationToken);
        }
    }
}

