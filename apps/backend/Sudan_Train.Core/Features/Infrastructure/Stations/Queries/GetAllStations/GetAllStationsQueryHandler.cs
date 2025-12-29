using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Queries.GetAllStations
{
    public class GetAllStationsQueryHandler : ResponseHandler, IRequestHandler<GetAllStationsQuery, Response<List<StationDto>>>
    {
        private readonly IStationService _stationService;

        public GetAllStationsQueryHandler(
            IStationService stationService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _stationService = stationService;
        }

        public async Task<Response<List<StationDto>>> Handle(GetAllStationsQuery request, CancellationToken cancellationToken)
        {
            var stations = await _stationService.GetAllStationsAsync(
                request.CityId,
                request.SearchTerm,
                request.IsActive,
                request.StationType,
                request.PageNumber,
                request.PageSize);
            return Success(null, stations);
        }
    }
}

