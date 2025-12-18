using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Commands.CreateStation
{
    public class CreateStationCommandHandler : ResponseHandler, IRequestHandler<CreateStationCommand, Response<StationDto>>
    {
        private readonly IStationService _stationService;

        public CreateStationCommandHandler(
            IStationService stationService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _stationService = stationService;
        }

        public async Task<Response<StationDto>> Handle(CreateStationCommand request, CancellationToken cancellationToken)
        {
            var stationDto = await _stationService.CreateStationAsync(
                request.Code,
                request.NameEn,
                request.NameAr,
                request.CityId,
                request.Latitude,
                request.Longitude,
                request.AddressEn,
                request.AddressAr);
            return Success("Station created successfully", stationDto);
        }
    }
}

