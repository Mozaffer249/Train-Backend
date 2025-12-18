using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trains.Commands.UpdateTrain
{
    public class UpdateTrainCommandValidator : AbstractValidator<UpdateTrainCommand>
    {
        private readonly ITrainRepository _trainRepository;
        private readonly ITrainService _trainService;

        public UpdateTrainCommandValidator(ITrainRepository trainRepository, ITrainService trainService)
        {
            _trainRepository = trainRepository;
            _trainService = trainService;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Train ID is required")
                .MustAsync(TrainExists).WithMessage("Train not found");

            RuleFor(x => x.TrainNumber)
                .NotEmpty().WithMessage("Train number is required")
                .Length(3, 50).WithMessage("Train number must be between 3 and 50 characters")
                .MustAsync(BeUniqueTrainNumber).WithMessage("Train number already exists");

            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage("English name is required")
                .Length(3, 200).WithMessage("English name must be between 3 and 200 characters");

            RuleFor(x => x.NameAr)
                .Length(3, 200).When(x => !string.IsNullOrEmpty(x.NameAr))
                .WithMessage("Arabic name must be between 3 and 200 characters");
        }

        private async Task<bool> TrainExists(int id, CancellationToken cancellationToken)
        {
            return await _trainRepository.GetTableNoTracking().AnyAsync(t => t.Id == id, cancellationToken);
        }

        private async Task<bool> BeUniqueTrainNumber(UpdateTrainCommand command, string trainNumber, CancellationToken cancellationToken)
        {
            return await _trainService.IsTrainNumberUniqueAsync(trainNumber, command.Id);
        }
    }
}

