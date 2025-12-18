using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Regions.Queries.GetRegionById
{
    public class GetRegionByIdQueryHandler : ResponseHandler, IRequestHandler<GetRegionByIdQuery, Response<RegionDto>>
    {
        private readonly IGeographyService _geographyService;

        public GetRegionByIdQueryHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<RegionDto>> Handle(GetRegionByIdQuery request, CancellationToken cancellationToken)
        {
            var region = await _geographyService.GetRegionByIdAsync(request.Id);
            if (region == null)
                return NotFound<RegionDto>("Region not found");

            return Success(null, region);
        }
    }
}

