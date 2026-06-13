using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Features.Users.Queries.GetUserList;

namespace Sudan_Train.Core.Features.Users.Commands.CreateUser
{
    // Admin-only — creates a Staff/Admin user. Customers self-register via the
    // existing /Authentication/Register endpoint.
    public class CreateUserCommand : IRequest<Response<UserDto>>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        // Initial role set. Must be non-empty.
        public List<string> Roles { get; set; } = new();
        // Optional station scope — only meaningful when Roles contains
        // StaffCounter or StaffBoarding.
        public List<int> StationIds { get; set; } = new();
    }
}
