using MediatR;
using Sudan_Train.Core.Bases;
using System.Collections.Generic;

namespace Sudan_Train.Core.Features.Authentication.Commands.GenerateRecoveryCodes
{
    public class GenerateRecoveryCodesCommand : IRequest<Response<GenerateRecoveryCodesResponse>>
    {
    }

    public class GenerateRecoveryCodesResponse
    {
        public List<string> RecoveryCodes { get; set; } = new List<string>();
    }
}
