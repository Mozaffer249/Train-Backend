using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Booking;

namespace Sudan_Train.Core.Features.Notifications.Queries.GetMyNotifications
{
    public class GetMyNotificationsQuery : IRequest<Response<List<NotificationDto>>>
    {
        public bool? UnreadOnly { get; set; }
    }
}
