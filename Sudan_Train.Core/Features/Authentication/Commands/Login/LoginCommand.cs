using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.Results;

namespace Sudan_Train.Core.Features.Authentication.Commands.Login
{
    public class LoginCommand : IRequest<Response<JwtAuthResult>>
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}

