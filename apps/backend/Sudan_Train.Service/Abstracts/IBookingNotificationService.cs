namespace Sudan_Train.Service.Abstracts
{
    public interface IBookingNotificationService
    {
        Task NotifyBookingConfirmedAsync(int bookingId);
        Task NotifyBookingCancelledAsync(int bookingId, string? reason);
        Task NotifyTripCancelledAsync(int bookingId, string? reason);
    }
}
