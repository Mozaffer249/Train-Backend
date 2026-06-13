using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Notifications.Commands.MarkNotificationRead
{
    public class MarkNotificationReadCommandHandler
        : ResponseHandler, IRequestHandler<MarkNotificationReadCommand, Response<string>>
    {
        private readonly ApplicationDBContext _db;
        private readonly IHttpContextAccessor _http;

        public MarkNotificationReadCommandHandler(
            ApplicationDBContext db,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _db = db;
            _http = http;
        }

        public async Task<Response<string>> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
                return Unauthorized<string>("Not authenticated.");

            var notification = await _db.Notifications
                .FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);
            if (notification == null)
                return NotFound<string>("Notification not found.");
            if (notification.UserId != userId)
                return Unauthorized<string>("Cannot modify another user's notification.");

            if (notification.IsRead)
                return Success<string>("Already read.");

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);

            return Success<string>("Marked read.");
        }
    }
}
