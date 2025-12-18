using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Regions.Commands.DeleteRegion
{
    public class DeleteRegionCommandHandler : ResponseHandler, IRequestHandler<DeleteRegionCommand, Response<string>>
    {
        private readonly IGeographyService _geographyService;

        public DeleteRegionCommandHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<string>> Handle(DeleteRegionCommand request, CancellationToken cancellationToken)
        {
            var hasStates = await _geographyService.RegionHasStatesAsync(request.Id);
            if (hasStates)
                return BadRequest<string>("Cannot delete region because it has states");

            var deleted = await _geographyService.DeleteRegionAsync(request.Id);
            if (!deleted)
                return NotFound<string>("Region not found");

            return Success<string>("Region deleted successfully");
        }
    }
}

