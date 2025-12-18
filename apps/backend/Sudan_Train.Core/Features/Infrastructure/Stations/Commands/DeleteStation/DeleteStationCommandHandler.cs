using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Stations.Commands.DeleteStation
{
    public class DeleteStationCommandHandler : ResponseHandler, IRequestHandler<DeleteStationCommand, Response<string>>
    {
        private readonly IStationService _stationService;

        public DeleteStationCommandHandler(
            IStationService stationService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _stationService = stationService;
        }

        public async Task<Response<string>> Handle(DeleteStationCommand request, CancellationToken cancellationToken)
        {
            var isUsed = await _stationService.StationIsUsedInRoutesAsync(request.Id);
            if (isUsed)
                return BadRequest<string>("Cannot delete station because it is used in routes");

            var deleted = await _stationService.DeleteStationAsync(request.Id);
            if (!deleted)
                return NotFound<string>("Station not found");

            return Success<string>("Station deleted successfully");
        }
    }
}

