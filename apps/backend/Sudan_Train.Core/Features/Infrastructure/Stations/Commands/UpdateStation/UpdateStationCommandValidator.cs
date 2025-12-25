using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Infrastructure.Abstracts;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Commands.UpdateStation
{
    public class UpdateStationCommandValidator : AbstractValidator<UpdateStationCommand>
    {
        private readonly IStationRepository _stationRepository;
        private readonly IStationService _stationService;

        public UpdateStationCommandValidator(IStationRepository stationRepository, IStationService stationService)
        {
            _stationRepository = stationRepository;
            _stationService = stationService;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Station ID is required")
                .MustAsync(StationExists).WithMessage("Station not found");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(8, 22).When(x => x.Latitude.HasValue)
                .WithMessage("Latitude must be within Sudan boundaries (8-22)");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(21, 39).When(x => x.Longitude.HasValue)
                .WithMessage("Longitude must be within Sudan boundaries (21-39)");

            // Duplicate name validation within same city
            RuleFor(x => x)
                .MustAsync(BeUniqueName).WithMessage("A station with this name already exists in this city");
        }

        private async Task<bool> StationExists(int stationId, CancellationToken cancellationToken)
        {
            return await _stationRepository.GetTableNoTracking().AnyAsync(s => s.Id == stationId, cancellationToken);
        }

        private async Task<bool> BeUniqueName(UpdateStationCommand command, CancellationToken cancellationToken)
        {
            // Get current station to find its cityId
            var station = await _stationRepository.GetTableNoTracking().FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);
            if (station == null) return true;

            return await _stationService.IsStationNameUniqueInCityAsync(command.NameEn, command.NameAr, station.CityId, command.Id);
        }
    }
}


