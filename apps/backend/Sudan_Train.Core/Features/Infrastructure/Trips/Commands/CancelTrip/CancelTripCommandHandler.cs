using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Trips.Commands.CancelTrip
{
    public class CancelTripCommandHandler : ResponseHandler, IRequestHandler<CancelTripCommand, Response<string>>
    {
        private readonly ITripService _tripService;

        public CancelTripCommandHandler(
            ITripService tripService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _tripService = tripService;
        }

        public async Task<Response<string>> Handle(CancelTripCommand request, CancellationToken cancellationToken)
        {
            var cancelled = await _tripService.CancelTripAsync(request.Id);
            if (!cancelled)
                return BadRequest<string>("Trip not found or cannot be cancelled");

            return Success<string>("Trip cancelled successfully");
        }
    }
}

