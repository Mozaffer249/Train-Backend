using MediatR;
using Sudan_Train.Core.Wrappers;

namespace Sudan_Train.Core.Features.Authentication.Commands.Register
{
    public class RegisterCommand : IRequest<Response<string>>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public string? PhoneNumber { get; set; }
    }
}

