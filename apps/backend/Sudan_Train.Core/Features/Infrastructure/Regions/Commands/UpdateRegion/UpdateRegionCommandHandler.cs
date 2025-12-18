using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Regions.Commands.UpdateRegion
{
    public class UpdateRegionCommandHandler : ResponseHandler, IRequestHandler<UpdateRegionCommand, Response<RegionDto>>
    {
        private readonly IGeographyService _geographyService;

        public UpdateRegionCommandHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<RegionDto>> Handle(UpdateRegionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var regionDto = await _geographyService.UpdateRegionAsync(
                    request.Id,
                    request.NameEn,
                    request.NameAr,
                    request.Code);

                return Success("Region updated successfully", regionDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound<RegionDto>("Region not found");
            }
        }
    }
}

