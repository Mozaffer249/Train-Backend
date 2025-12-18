using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.States.Queries.GetAllStates
{
    public class GetAllStatesQueryHandler : ResponseHandler, IRequestHandler<GetAllStatesQuery, Response<List<StateDto>>>
    {
        private readonly IGeographyService _geographyService;

        public GetAllStatesQueryHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<List<StateDto>>> Handle(GetAllStatesQuery request, CancellationToken cancellationToken)
        {
            var states = await _geographyService.GetAllStatesAsync(request.RegionId);
            return Success(null, states);
        }
    }
}

