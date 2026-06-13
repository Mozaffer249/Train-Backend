using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Users.Commands.AssignStaffStations
{
    // Replaces the user's StaffStation assignment set with the supplied list.
    // Idempotent.
    public class AssignStaffStationsCommand : IRequest<Response<List<int>>>
    {
        public int Id { get; set; }
        public List<int> StationIds { get; set; } = new();
    }
}
