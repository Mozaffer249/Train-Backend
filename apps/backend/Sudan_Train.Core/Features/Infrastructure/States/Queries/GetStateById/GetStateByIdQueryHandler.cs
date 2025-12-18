using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.States.Queries.GetStateById
{
    public class GetStateByIdQueryHandler : ResponseHandler, IRequestHandler<GetStateByIdQuery, Response<StateDto>>
    {
        private readonly IGeographyService _geographyService;

        public GetStateByIdQueryHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<StateDto>> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
        {
            var state = await _geographyService.GetStateByIdAsync(request.Id);
            if (state == null)
                return NotFound<StateDto>("State not found");

            return Success(null, state);
        }
    }
}

