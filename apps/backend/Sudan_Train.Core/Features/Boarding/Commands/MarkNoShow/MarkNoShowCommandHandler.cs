using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Boarding.Commands.MarkNoShow
{
    public class MarkNoShowCommandHandler
        : ResponseHandler, IRequestHandler<MarkNoShowCommand, Response<string>>
    {
        private readonly ApplicationDBContext _db;
        private readonly IHttpContextAccessor _http;
        private readonly IStaffAuthorizationService _staffAuth;

        public MarkNoShowCommandHandler(
            ApplicationDBContext db,
            IHttpContextAccessor http,
            IStaffAuthorizationService staffAuth,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _db = db;
            _http = http;
            _staffAuth = staffAuth;
        }

        public async Task<Response<string>> Handle(MarkNoShowCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _db.Tickets
                .Include(t => t.BookingPassenger).ThenInclude(bp => bp.Trip)
                .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

            if (ticket == null)
                return NotFound<string>("Ticket not found.");

            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            int.TryParse(userIdClaim, out var userId);
            var roles = _http.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                ?? new List<string>();

            if (userId <= 0)
                return Unauthorized<string>("Not authenticated.");

            if (!await _staffAuth.CanOperateTripAsync(userId, roles, ticket.BookingPassenger.TripId))
                return Unauthorized<string>("Not assigned to a station on this trip.");

            if (ticket.Status == TicketStatus.NoShow)
                return Success("Already marked no-show.", ticket.TicketNumber);
            if (ticket.Status == TicketStatus.Boarded)
                return BadRequest<string>("Cannot mark a boarded ticket as no-show.");
            if (ticket.Status == TicketStatus.Cancelled)
                return BadRequest<string>("Ticket is cancelled.");

            ticket.Status = TicketStatus.NoShow;
            await _db.SaveChangesAsync(cancellationToken);

            return Success("Ticket marked no-show", ticket.TicketNumber);
        }
    }
}
