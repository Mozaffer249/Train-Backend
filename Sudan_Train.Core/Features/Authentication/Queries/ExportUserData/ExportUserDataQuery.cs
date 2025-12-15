using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Queries.ExportUserData
{
    public class ExportUserDataQuery : IRequest<Response<UserDataExport>>
    {
    }

    public class UserDataExport
    {
        public UserInfo User { get; set; } = default!;
        public List<BookingInfo> Bookings { get; set; } = new();
        public List<SessionInfo> Sessions { get; set; } = new();
        public List<AuditLogInfo> AuditLogs { get; set; } = new();
    }

    public class UserInfo
    {
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Address { get; set; }
        public string? Nationality { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? PasswordChangedAt { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }

    public class BookingInfo
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = default!;
        public decimal TotalPrice { get; set; }
    }

    public class SessionInfo
    {
        public string DeviceInfo { get; set; } = default!;
        public string IpAddress { get; set; } = default!;
        public DateTime LoginTime { get; set; }
    }

    public class AuditLogInfo
    {
        public string Action { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; } = default!;
        public bool Success { get; set; }
    }
}

