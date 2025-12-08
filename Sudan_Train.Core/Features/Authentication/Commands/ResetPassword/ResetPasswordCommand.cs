using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<Response<string>>
    {
        public string Email { get; set; } = default!;
        public string ResetCode { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
    }
}
