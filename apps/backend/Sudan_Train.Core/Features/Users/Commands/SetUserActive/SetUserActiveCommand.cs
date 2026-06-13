using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Users.Commands.SetUserActive
{
    // Flips User.IsActive. Inactive users cannot log in (existing auth checks
    // already enforce this). Idempotent.
    public class SetUserActiveCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
