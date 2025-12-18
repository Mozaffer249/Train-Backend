using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Queries.GetStationById
{
    public class GetStationByIdQueryHandler : ResponseHandler, IRequestHandler<GetStationByIdQuery, Response<StationDto>>
    {
        private readonly IStationService _stationService;

        public GetStationByIdQueryHandler(
            IStationService stationService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _stationService = stationService;
        }

        public async Task<Response<StationDto>> Handle(GetStationByIdQuery request, CancellationToken cancellationToken)
        {
            var station = await _stationService.GetStationByIdAsync(request.Id);
            if (station == null)
                return NotFound<StationDto>("Station not found");

            return Success(null, station);
        }
    }
}

