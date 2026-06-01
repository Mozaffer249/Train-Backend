using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Fares.Commands.CreateFare
{
    public class CreateFareCommandHandler : ResponseHandler, IRequestHandler<CreateFareCommand, Response<FareDto>>
    {
        private readonly IFareService _fareService;

        public CreateFareCommandHandler(
            IFareService fareService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _fareService = fareService;
        }

        public async Task<Response<FareDto>> Handle(CreateFareCommand request, CancellationToken cancellationToken)
        {
            var fareDto = await _fareService.CreateFareAsync(
                request.RouteId,
                request.OriginStationId,
                request.DestinationStationId,
                request.TripId,
                request.CoachClass,
                request.BasePrice,
                request.DiscountPercent);

            return Success("Fare created successfully", fareDto);
        }
    }
}
