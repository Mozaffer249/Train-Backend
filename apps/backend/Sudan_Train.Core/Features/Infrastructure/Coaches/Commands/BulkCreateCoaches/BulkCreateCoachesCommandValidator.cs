using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Coaches.Commands.BulkCreateCoaches
{
    public class BulkCreateCoachesCommandValidator : AbstractValidator<BulkCreateCoachesCommand>
    {
        private readonly ITrainRepository _trainRepository;

        public BulkCreateCoachesCommandValidator(ITrainRepository trainRepository)
        {
            _trainRepository = trainRepository;

            RuleFor(x => x.TrainId)
                .GreaterThan(0).WithMessage("Train ID is required")
                .MustAsync(TrainExists).WithMessage("Train not found");

            RuleFor(x => x.NumberOfCoaches)
                .GreaterThan(0).WithMessage("Number of coaches must be greater than 0")
                .LessThanOrEqualTo(20).WithMessage("Cannot create more than 20 coaches at once");

            RuleFor(x => x.CapacityPerCoach)
                .InclusiveBetween(20, 100).WithMessage("Capacity per coach must be between 20 and 100");
        }

        private async Task<bool> TrainExists(int trainId, CancellationToken cancellationToken)
        {
            return await _trainRepository.GetTableNoTracking().AnyAsync(t => t.Id == trainId, cancellationToken);
        }
    }
}

