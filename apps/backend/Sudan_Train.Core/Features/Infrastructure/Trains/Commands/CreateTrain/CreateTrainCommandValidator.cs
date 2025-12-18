using FluentValidation;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trains.Commands.CreateTrain
{
    public class CreateTrainCommandValidator : AbstractValidator<CreateTrainCommand>
    {
        private readonly ITrainService _trainService;

        public CreateTrainCommandValidator(ITrainService trainService)
        {
            _trainService = trainService;

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

        private async Task<bool> BeUniqueTrainNumber(string trainNumber, CancellationToken cancellationToken)
        {
            return await _trainService.IsTrainNumberUniqueAsync(trainNumber);
        }
    }
}

