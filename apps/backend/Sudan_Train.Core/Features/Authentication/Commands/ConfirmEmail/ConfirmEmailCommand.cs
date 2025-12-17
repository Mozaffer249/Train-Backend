using System.ComponentModel.DataAnnotations;
using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmail
{
    public class ConfirmEmailCommand : IRequest<Response<string>>
    {
        public int UserId { get; set; }

        [StringLength(4, MinimumLength = 4, ErrorMessage = "OTP code must be exactly 4 digits")]
        public string Code { get; set; } = default!;
    }
}
