using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Cities.Commands.CreateCity
{
    public class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
    {
        private readonly IStateRepository _stateRepository;

        public CreateCityCommandValidator(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;

            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage("English name is required")
                .Length(3, 100).WithMessage("English name must be between 3 and 100 characters");

            RuleFor(x => x.NameAr)
                .Length(3, 100).When(x => !string.IsNullOrEmpty(x.NameAr))
                .WithMessage("Arabic name must be between 3 and 100 characters");

            RuleFor(x => x.StateId)
                .GreaterThan(0).WithMessage("State ID is required")
                .MustAsync(StateExists).WithMessage("State not found");
        }

        private async Task<bool> StateExists(int stateId, CancellationToken cancellationToken)
        {
            return await _stateRepository.GetTableNoTracking().AnyAsync(s => s.Id == stateId, cancellationToken);
        }
    }
}

