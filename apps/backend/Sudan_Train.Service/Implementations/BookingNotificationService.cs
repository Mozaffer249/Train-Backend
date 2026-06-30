using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sudan_Train.Data.Entity;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Service.Abstracts;
using Sudan_Train.Service.Models;

namespace Sudan_Train.Service.Implementations
{
    public class BookingNotificationService : IBookingNotificationService
    {
        private readonly ApplicationDBContext _db;
        private readonly IInAppNotificationService _inApp;
        private readonly IEmailService _email;
        private readonly ILogger<BookingNotificationService> _logger;

        public BookingNotificationService(
            ApplicationDBContext db,
            IInAppNotificationService inApp,
            IEmailService email,
            ILogger<BookingNotificationService> logger)
        {
            _db = db;
            _inApp = inApp;
            _email = email;
            _logger = logger;
        }

        public async Task NotifyBookingConfirmedAsync(int bookingId)
        {
            var ctx = await LoadContextAsync(bookingId);
            if (ctx == null || !ctx.UserId.HasValue)
                return;

            var subject = "تأكيد الحجز";
            var message = $"تم تأكيد حجزك {ctx.Reference} بمبلغ {ctx.TotalAmount:N0} {ctx.Currency}.";
            await _inApp.NotifyAsync(
                ctx.UserId.Value,
                NotificationType.BookingConfirmation,
                subject,
                message,
                bookingId);

            if (!string.IsNullOrWhiteSpace(ctx.Email))
            {
                var body = BuildConfirmationEmail(ctx);
                await SendEmailSafeAsync(ctx.Email, subject, body, bookingId, "booking confirmation");
            }
        }

        public async Task NotifyBookingCancelledAsync(int bookingId, string? reason)
        {
            var ctx = await LoadContextAsync(bookingId);
            if (ctx == null || !ctx.UserId.HasValue)
                return;

            var subject = "إلغاء الحجز";
            var reasonText = string.IsNullOrWhiteSpace(reason) ? string.Empty : $" السبب: {reason.Trim()}";
            var message = $"تم إلغاء حجزك {ctx.Reference}.{reasonText}".Trim();
            await _inApp.NotifyAsync(
                ctx.UserId.Value,
                NotificationType.BookingCancellation,
                subject,
                message,
                bookingId);

            if (!string.IsNullOrWhiteSpace(ctx.Email))
            {
                var body = BuildCancellationEmail(ctx, reason, isTripCancel: false);
                await SendEmailSafeAsync(ctx.Email, subject, body, bookingId, "booking cancellation");
            }
        }

        public async Task NotifyTripCancelledAsync(int bookingId, string? reason)
        {
            var ctx = await LoadContextAsync(bookingId);
            if (ctx == null || !ctx.UserId.HasValue)
                return;

            var subject = "إلغاء الرحلة";
            var reasonText = string.IsNullOrWhiteSpace(reason) ? string.Empty : $" السبب: {reason.Trim()}";
            var message = $"تم إلغاء رحلتك المرتبطة بالحجز {ctx.Reference}.{reasonText}".Trim();
            await _inApp.NotifyAsync(
                ctx.UserId.Value,
                NotificationType.TripCancellation,
                subject,
                message,
                bookingId);

            if (!string.IsNullOrWhiteSpace(ctx.Email))
            {
                var body = BuildCancellationEmail(ctx, reason, isTripCancel: true);
                await SendEmailSafeAsync(ctx.Email, subject, body, bookingId, "trip cancellation");
            }
        }

        private async Task SendEmailSafeAsync(string to, string subject, string body, int bookingId, string context)
        {
            try
            {
                await _email.SendEmailAsync(to, subject, body, EmailSendingStrategy.Queued);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send {Context} email for booking {BookingId} to {Email}",
                    context, bookingId, to);
            }
        }

        private async Task<BookingNotifyContext?> LoadContextAsync(int bookingId)
        {
            var booking = await _db.Bookings
                .AsNoTracking()
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.Trip).ThenInclude(t => t.Route)
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.BoardingStation)
                .Include(b => b.BookingPassengers).ThenInclude(bp => bp.AlightingStation)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || booking.BookingPassengers.Count == 0)
                return null;

            var primary = booking.BookingPassengers.First();
            var user = booking.UserId.HasValue
                ? await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == booking.UserId.Value)
                : null;

            return new BookingNotifyContext
            {
                UserId = booking.UserId,
                Email = user?.Email,
                FirstName = user?.FirstName ?? string.Empty,
                Reference = booking.Reference,
                TotalAmount = booking.TotalAmount,
                Currency = "SDG",
                RouteName = primary.Trip.Route?.NameAr ?? primary.Trip.Route?.NameEn ?? string.Empty,
                BoardingName = primary.BoardingStation?.NameAr ?? primary.BoardingStation?.NameEn ?? string.Empty,
                AlightingName = primary.AlightingStation?.NameAr ?? primary.AlightingStation?.NameEn ?? string.Empty,
                DepartureTime = primary.Trip.DepartureTime,
            };
        }

        private static string BuildConfirmationEmail(BookingNotifyContext ctx)
        {
            var departure = ctx.DepartureTime.ToString("yyyy-MM-dd HH:mm");
            return $"""
                <div dir="rtl" style="font-family:Arial,sans-serif;line-height:1.6">
                  <p>مرحباً {ctx.FirstName}،</p>
                  <p>تم تأكيد حجزك بنجاح على منصة قطارات السودان.</p>
                  <ul>
                    <li><strong>مرجع الحجز:</strong> {ctx.Reference}</li>
                    <li><strong>المسار:</strong> {ctx.RouteName}</li>
                    <li><strong>من:</strong> {ctx.BoardingName}</li>
                    <li><strong>إلى:</strong> {ctx.AlightingName}</li>
                    <li><strong>وقت المغادرة:</strong> {departure}</li>
                    <li><strong>المبلغ:</strong> {ctx.TotalAmount:N0} {ctx.Currency}</li>
                  </ul>
                  <p>يمكنك مراجعة تفاصيل الحجز من لوحة «حجوزاتي».</p>
                  <p>شكراً لاستخدامكم قطارات السودان.</p>
                </div>
                """;
        }

        private static string BuildCancellationEmail(BookingNotifyContext ctx, string? reason, bool isTripCancel)
        {
            var kind = isTripCancel ? "إلغاء الرحلة" : "إلغاء الحجز";
            var reasonLine = string.IsNullOrWhiteSpace(reason)
                ? string.Empty
                : $"<li><strong>السبب:</strong> {reason.Trim()}</li>";
            return $"""
                <div dir="rtl" style="font-family:Arial,sans-serif;line-height:1.6">
                  <p>مرحباً {ctx.FirstName}،</p>
                  <p>نود إبلاغكم بـ{kind} المرتبط بالحجز <strong>{ctx.Reference}</strong>.</p>
                  <ul>
                    <li><strong>المسار:</strong> {ctx.RouteName}</li>
                    {reasonLine}
                  </ul>
                  <p>إذا تم الدفع مسبقاً، سيتم معالجة الاسترداد وفق سياسة النظام.</p>
                </div>
                """;
        }

        private sealed class BookingNotifyContext
        {
            public int? UserId { get; init; }
            public string? Email { get; init; }
            public string FirstName { get; init; } = string.Empty;
            public string Reference { get; init; } = string.Empty;
            public decimal TotalAmount { get; init; }
            public string Currency { get; init; } = "SDG";
            public string RouteName { get; init; } = string.Empty;
            public string BoardingName { get; init; } = string.Empty;
            public string AlightingName { get; init; } = string.Empty;
            public DateTime DepartureTime { get; init; }
        }
    }
}
