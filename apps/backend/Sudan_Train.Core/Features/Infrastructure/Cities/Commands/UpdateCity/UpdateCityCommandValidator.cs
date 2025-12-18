using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Commands.UpdateCity
{
    public class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
    {
        private readonly IStateRepository _stateRepository;
        private readonly ICityRepository _cityRepository;

        public UpdateCityCommandValidator(IStateRepository stateRepository, ICityRepository cityRepository)
        {
            _stateRepository = stateRepository;
            _cityRepository = cityRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("City ID is required")
                .MustAsync(CityExists).WithMessage("City not found");

            RuleFor(x => x.StateId)
                .GreaterThan(0).When(x => x.StateId.HasValue).WithMessage("State ID must be greater than 0")
                .MustAsync(StateExists).When(x => x.StateId.HasValue).WithMessage("State not found");
        }

        private async Task<bool> CityExists(int cityId, CancellationToken cancellationToken)
        {
            return await _cityRepository.GetTableNoTracking().AnyAsync(c => c.Id == cityId, cancellationToken);
        }

        private async Task<bool> StateExists(int? stateId, CancellationToken cancellationToken)
        {
            if (!stateId.HasValue) return true;
            return await _stateRepository.GetTableNoTracking().AnyAsync(s => s.Id == stateId.Value, cancellationToken);
        }
    }
}

