using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Commands.UpdateStation
{
    public class UpdateStationCommandValidator : AbstractValidator<UpdateStationCommand>
    {
        private readonly IStationRepository _stationRepository;

        public UpdateStationCommandValidator(IStationRepository stationRepository)
        {
            _stationRepository = stationRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Station ID is required")
                .MustAsync(StationExists).WithMessage("Station not found");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(8, 22).When(x => x.Latitude.HasValue)
                .WithMessage("Latitude must be within Sudan boundaries (8-22)");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(21, 39).When(x => x.Longitude.HasValue)
                .WithMessage("Longitude must be within Sudan boundaries (21-39)");
        }

        private async Task<bool> StationExists(int stationId, CancellationToken cancellationToken)
        {
            return await _stationRepository.GetTableNoTracking().AnyAsync(s => s.Id == stationId, cancellationToken);
        }
    }
}

