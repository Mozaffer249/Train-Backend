using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Service.Abstracts;
using System.Security.Claims;

namespace Sudan_Train.Core.Features.Authentication.Commands.EnableTwoFactor
{
    public class EnableTwoFactorCommandHandler : ResponseHandler, IRequestHandler<EnableTwoFactorCommand, Response<EnableTwoFactorResponse>>
    {
        private readonly ITwoFactorAuthenticationService _twoFactorService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;

        public EnableTwoFactorCommandHandler(
            ITwoFactorAuthenticationService twoFactorService,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<AuthenticationResources> authLocalizer) : base(authLocalizer)
        {
            _twoFactorService = twoFactorService;
            _httpContextAccessor = httpContextAccessor;
            _authLocalizer = authLocalizer;
        }

        public async Task<Response<EnableTwoFactorResponse>> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
        {
            // Two-factor authentication temporarily disabled — TwoFactorRecoveryCode table dropped (DropAdvancedSecurityTables migration).
            await Task.CompletedTask;
            return BadRequest<EnableTwoFactorResponse>("Two-factor authentication is temporarily disabled.");

            /* Original implementation preserved for restoration:
            // Get current user from HttpContext
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized<EnableTwoFactorResponse>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            try
            {
                var (qrCodeUrl, manualEntryKey) = await _twoFactorService.EnableTwoFactorAsync(userId);

                var response = new EnableTwoFactorResponse
                {
                    QrCodeUrl = qrCodeUrl,
                    ManualEntryKey = manualEntryKey
                };

                return Success<EnableTwoFactorResponse>(entity: response);
            }
            catch (Exception ex)
            {
                return BadRequest<EnableTwoFactorResponse>(ex.Message);
            }
            */
        }
    }
}
