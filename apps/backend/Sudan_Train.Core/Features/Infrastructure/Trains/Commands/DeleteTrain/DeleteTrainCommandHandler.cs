using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trains.Commands.DeleteTrain
{
    public class DeleteTrainCommandHandler : ResponseHandler, IRequestHandler<DeleteTrainCommand, Response<string>>
    {
        private readonly ITrainService _trainService;

        public DeleteTrainCommandHandler(
            ITrainService trainService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _trainService = trainService;
        }

        public async Task<Response<string>> Handle(DeleteTrainCommand request, CancellationToken cancellationToken)
        {
            var hasActiveTrips = await _trainService.TrainHasActiveTripsAsync(request.Id);
            if (hasActiveTrips)
                return BadRequest<string>("Cannot delete train because it has active trips");

            var deleted = await _trainService.DeleteTrainAsync(request.Id);
            if (!deleted)
                return NotFound<string>("Train not found");

            return Success<string>("Train deleted successfully");
        }
    }
}

