using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Authentication.Commands.DeleteAccount
{
    public class DeleteAccountCommand : IRequest<Response<string>>
    {
        public string Password { get; set; } = default!;
        public bool ConfirmDeletion { get; set; }
    }
}

