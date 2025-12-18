using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Commands.UpdateStation
{
    public class UpdateStationCommandHandler : ResponseHandler, IRequestHandler<UpdateStationCommand, Response<StationDto>>
    {
        private readonly IStationService _stationService;

        public UpdateStationCommandHandler(
            IStationService stationService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _stationService = stationService;
        }

        public async Task<Response<StationDto>> Handle(UpdateStationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var stationDto = await _stationService.UpdateStationAsync(
                    request.Id,
                    request.NameEn,
                    request.NameAr,
                    request.Latitude,
                    request.Longitude,
                    request.AddressEn,
                    request.AddressAr);
                return Success("Station updated successfully", stationDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound<StationDto>("Station not found");
            }
        }
    }
}

