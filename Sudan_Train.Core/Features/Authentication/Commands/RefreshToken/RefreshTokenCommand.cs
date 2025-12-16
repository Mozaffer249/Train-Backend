using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.Results;

namespace Sudan_Train.Core.Features.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<Response<JwtAuthResult>>
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public string? DeviceId { get; set; }
    }
}
