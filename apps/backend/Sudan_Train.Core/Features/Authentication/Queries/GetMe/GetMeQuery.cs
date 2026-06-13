using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Queries.GetMe
{
    // Lightweight identity summary for the admin shell — used to decide which
    // pages to show in the sidebar and which routes to guard.
    public class GetMeQuery : IRequest<Response<MeDto>> { }

    public class MeDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<int> AssignedStationIds { get; set; } = new();
    }
}
