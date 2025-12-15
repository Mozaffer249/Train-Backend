using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Queries.GetActiveSessions
{
    public class GetActiveSessionsQuery : IRequest<Response<List<SessionResponse>>>
    {
    }

    public class SessionResponse
    {
        public long SessionId { get; set; }
        public string DeviceInfo { get; set; } = default!;
        public string IpAddress { get; set; } = default!;
        public DateTime LoginTime { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsCurrent { get; set; }
    }
}

