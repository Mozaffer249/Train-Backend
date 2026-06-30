using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Bookings.Commands.ReleaseSeatHolds
{
    public class ReleaseSeatHoldsCommandHandler : ResponseHandler, IRequestHandler<ReleaseSeatHoldsCommand, Response<string>>
    {
        private readonly ISeatHoldService _seatHoldService;
        private readonly IHttpContextAccessor _http;

        public ReleaseSeatHoldsCommandHandler(
            ISeatHoldService seatHoldService,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _seatHoldService = seatHoldService;
            _http = http;
        }

        public async Task<Response<string>> Handle(ReleaseSeatHoldsCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
                return Unauthorized<string>("Authentication required.");

            await _seatHoldService.ReleaseHoldsAsync(userId, request.HoldGroupId);
            return Success("Holds released", string.Empty);
        }
    }
}
