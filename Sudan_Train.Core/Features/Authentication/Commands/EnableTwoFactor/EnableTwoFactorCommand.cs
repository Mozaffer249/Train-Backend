using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.EnableTwoFactor
{
    public class EnableTwoFactorCommand : IRequest<Response<EnableTwoFactorResponse>>
    {
    }

    public class EnableTwoFactorResponse
    {
        public string QrCodeUrl { get; set; } = default!;
        public string ManualEntryKey { get; set; } = default!;
    }
}
