using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Service.Abstracts;
using System.Security.Claims;

namespace Sudan_Train.Core.Features.Authentication.Commands.GenerateRecoveryCodes
{
    public class GenerateRecoveryCodesCommandHandler : ResponseHandler, IRequestHandler<GenerateRecoveryCodesCommand, Response<GenerateRecoveryCodesResponse>>
    {
        private readonly ITwoFactorAuthenticationService _twoFactorService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;

        public GenerateRecoveryCodesCommandHandler(
            ITwoFactorAuthenticationService twoFactorService,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<AuthenticationResources> authLocalizer) : base(authLocalizer)
        {
            _twoFactorService = twoFactorService;
            _httpContextAccessor = httpContextAccessor;
            _authLocalizer = authLocalizer;
        }

        public async Task<Response<GenerateRecoveryCodesResponse>> Handle(GenerateRecoveryCodesCommand request, CancellationToken cancellationToken)
        {
            // Two-factor recovery codes temporarily disabled — TwoFactorRecoveryCode table dropped (DropAdvancedSecurityTables migration).
            await Task.CompletedTask;
            return BadRequest<GenerateRecoveryCodesResponse>("Two-factor authentication is temporarily disabled.");

            /* Original implementation preserved for restoration:
            // Get current user from HttpContext
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized<GenerateRecoveryCodesResponse>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            try
            {
                var codes = await _twoFactorService.GenerateRecoveryCodesAsync(userId);

                var response = new GenerateRecoveryCodesResponse
                {
                    RecoveryCodes = codes
                };

                return Success<GenerateRecoveryCodesResponse>(entity: response);
            }
            catch (Exception ex)
            {
                return BadRequest<GenerateRecoveryCodesResponse>(ex.Message);
            }
            */
        }
    }
}
