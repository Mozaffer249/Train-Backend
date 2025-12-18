using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Regions.Queries.GetAllRegions
{
    public class GetAllRegionsQueryHandler : ResponseHandler, IRequestHandler<GetAllRegionsQuery, Response<List<RegionDto>>>
    {
        private readonly IGeographyService _geographyService;

        public GetAllRegionsQueryHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<List<RegionDto>>> Handle(GetAllRegionsQuery request, CancellationToken cancellationToken)
        {
            var regions = await _geographyService.GetAllRegionsAsync();
            return Success(null, regions);
        }
    }
}

