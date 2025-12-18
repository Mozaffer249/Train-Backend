using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Regions.Commands.CreateRegion
{
    public class CreateRegionCommandHandler : ResponseHandler, IRequestHandler<CreateRegionCommand, Response<RegionDto>>
    {
        private readonly IGeographyService _geographyService;

        public CreateRegionCommandHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<RegionDto>> Handle(CreateRegionCommand request, CancellationToken cancellationToken)
        {
            var regionDto = await _geographyService.CreateRegionAsync(
                request.NameEn,
                request.NameAr,
                request.Code);

            return Success("Region created successfully", regionDto);
        }
    }
}
