using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.States.Commands.UpdateState
{
    public class UpdateStateCommandValidator : AbstractValidator<UpdateStateCommand>
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IStateRepository _stateRepository;

        public UpdateStateCommandValidator(IRegionRepository regionRepository, IStateRepository stateRepository)
        {
            _regionRepository = regionRepository;
            _stateRepository = stateRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("State ID is required")
                .MustAsync(StateExists).WithMessage("State not found");

            RuleFor(x => x.RegionId)
                .GreaterThan(0).When(x => x.RegionId.HasValue).WithMessage("Region ID must be greater than 0")
                .MustAsync(RegionExists).When(x => x.RegionId.HasValue).WithMessage("Region not found");
        }

        private async Task<bool> StateExists(int stateId, CancellationToken cancellationToken)
        {
            return await _stateRepository.GetTableNoTracking().AnyAsync(s => s.Id == stateId, cancellationToken);
        }

        private async Task<bool> RegionExists(int? regionId, CancellationToken cancellationToken)
        {
            if (!regionId.HasValue) return true;
            return await _regionRepository.GetTableNoTracking().AnyAsync(r => r.Id == regionId.Value, cancellationToken);
        }
    }
}

