using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.States.Commands.DeleteState
{
    public class DeleteStateCommandHandler : ResponseHandler, IRequestHandler<DeleteStateCommand, Response<string>>
    {
        private readonly IGeographyService _geographyService;

        public DeleteStateCommandHandler(
            IGeographyService geographyService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _geographyService = geographyService;
        }

        public async Task<Response<string>> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
        {
            var hasCities = await _geographyService.StateHasCitiesAsync(request.Id);
            if (hasCities)
                return BadRequest<string>("Cannot delete state because it has cities");

            var deleted = await _geographyService.DeleteStateAsync(request.Id);
            if (!deleted)
                return NotFound<string>("State not found");

            return Success<string>("State deleted successfully");
        }
    }
}

