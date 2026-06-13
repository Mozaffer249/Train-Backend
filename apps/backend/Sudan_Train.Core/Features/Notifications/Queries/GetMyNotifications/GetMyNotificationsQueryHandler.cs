using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Shared;
using Sudan_Train.Data.DTOs.Booking;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Features.Notifications.Queries.GetMyNotifications
{
    public class GetMyNotificationsQueryHandler
        : ResponseHandler, IRequestHandler<GetMyNotificationsQuery, Response<List<NotificationDto>>>
    {
        private readonly ApplicationDBContext _db;
        private readonly IHttpContextAccessor _http;

        public GetMyNotificationsQueryHandler(
            ApplicationDBContext db,
            IHttpContextAccessor http,
            IStringLocalizer<SharedResources> localizer) : base(localizer)
        {
            _db = db;
            _http = http;
        }

        public async Task<Response<List<NotificationDto>>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _http.HttpContext?.User.FindFirst("uid")?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
                return Unauthorized<List<NotificationDto>>("Not authenticated.");

            var q = _db.Notifications
                .AsNoTracking()
                .Include(n => n.Booking)
                .Where(n => n.UserId == userId);

            if (request.UnreadOnly == true)
                q = q.Where(n => !n.IsRead);

            var rows = await q
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    BookingId = n.BookingId,
                    BookingReference = n.Booking != null ? n.Booking.Reference : null,
                    Type = n.Type.ToString(),
                    Subject = n.Subject,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    ReadAt = n.ReadAt,
                    CreatedAt = n.CreatedAt,
                })
                .ToListAsync(cancellationToken);

            return Success<List<NotificationDto>>("Notifications loaded", rows);
        }
    }
}
