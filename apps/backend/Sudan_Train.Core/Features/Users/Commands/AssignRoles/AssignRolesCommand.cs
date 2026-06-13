using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Users.Commands.AssignRoles
{
    // Replaces the user's role set with the supplied list (add what's new,
    // remove what's gone). Idempotent.
    public class AssignRolesCommand : IRequest<Response<List<string>>>
    {
        public int Id { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
