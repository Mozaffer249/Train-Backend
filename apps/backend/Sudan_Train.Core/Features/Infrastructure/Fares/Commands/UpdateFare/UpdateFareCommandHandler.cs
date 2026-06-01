using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Infrastructure;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Infrastructure.Fares.Commands.UpdateFare
{
    public class UpdateFareCommandHandler : ResponseHandler, IRequestHandler<UpdateFareCommand, Response<FareDto>>
    {
        private readonly IFareService _fareService;

        public UpdateFareCommandHandler(
            IFareService fareService,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _fareService = fareService;
        }

        public async Task<Response<FareDto>> Handle(UpdateFareCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dto = await _fareService.UpdateFareAsync(
                    request.Id,
                    request.BasePrice,
                    request.DiscountPercent,
                    request.EffectiveFrom,
                    request.EffectiveTo);

                return Success("Fare updated successfully", dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound<FareDto>($"Fare with ID {request.Id} not found");
            }
        }
    }
}
