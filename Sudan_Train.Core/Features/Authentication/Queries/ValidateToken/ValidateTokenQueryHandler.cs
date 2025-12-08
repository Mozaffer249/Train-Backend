using MediatR;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Core.Features.Authentication.Queries.ValidateToken
{
    public class ValidateTokenQueryHandler : ResponseHandler, IRequestHandler<ValidateTokenQuery, Response<string>>
    {
        private readonly IAuthenticationService _authenticationService;

        public ValidateTokenQueryHandler(
            IStringLocalizer<SharedResources> stringLocalizer,
            IAuthenticationService authenticationService) : base(stringLocalizer)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Response<string>> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
        {
            var result = await _authenticationService.ValidateToken(request.AccessToken);

            if (result == "NotExpired")
            {
                return Success("Token is valid and not expired");
            }
            else if (result == "Expired")
            {
                return Unauthorized<string>("Token has expired");
            }
            else
            {
                return Unauthorized<string>("Invalid token");
            }
        }
    }
}
