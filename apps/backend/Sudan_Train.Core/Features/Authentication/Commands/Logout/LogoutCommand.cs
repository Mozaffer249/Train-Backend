using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.Logout
{
    public class LogoutCommand : IRequest<Response<string>>
    {
        public string AccessToken { get; set; } = default!;
        public string? RefreshToken { get; set; }
        public bool LogoutAllDevices { get; set; } = false;
    }
}

