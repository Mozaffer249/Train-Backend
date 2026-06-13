using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Features.Users.Queries.GetUserList;

namespace Sudan_Train.Core.Features.Users.Commands.UpdateUser
{
    // PATCH-style update — only the basic profile fields. Role / station
    // / active-flag have dedicated commands so admin actions stay explicit.
    public class UpdateUserCommand : IRequest<Response<UserDto>>
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
