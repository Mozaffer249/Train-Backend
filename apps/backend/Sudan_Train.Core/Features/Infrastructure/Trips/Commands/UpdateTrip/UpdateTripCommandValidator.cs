using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Commands.UpdateTrip
{
    public class UpdateTripCommandValidator : AbstractValidator<UpdateTripCommand>
    {
        private readonly ITripRepository _tripRepository;

        public UpdateTripCommandValidator(ITripRepository tripRepository)
        {
            _tripRepository = tripRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Trip ID is required")
                .MustAsync(TripExists).WithMessage("Trip not found");

            RuleFor(x => x.ArrivalTime)
                .GreaterThan(x => x.DepartureTime).WithMessage("Arrival time must be after departure time");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(status => new[] { "Scheduled", "In Transit", "Completed", "Delayed", "Cancelled" }.Contains(status))
                .WithMessage("Invalid status");
        }

        private async Task<bool> TripExists(int id, CancellationToken cancellationToken)
        {
            return await _tripRepository.GetTableNoTracking().AnyAsync(t => t.Id == id, cancellationToken);
        }
    }
}

