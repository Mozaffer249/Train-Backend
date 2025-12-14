using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sudan_Train.Core.Bases;
using Sudan_Train.Core.Resources.Authentication;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.InfrastructureBases;
using System.Security.Claims;

namespace Sudan_Train.Core.Features.Authentication.Queries.GetTwoFactorStatus
{
    public class GetTwoFactorStatusQueryHandler : ResponseHandler, IRequestHandler<GetTwoFactorStatusQuery, Response<TwoFactorStatusResponse>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepositoryAsync<TwoFactorRecoveryCode> _recoveryCodeRepository;
        private readonly IStringLocalizer<AuthenticationResources> _authLocalizer;

        public GetTwoFactorStatusQueryHandler(
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepositoryAsync<TwoFactorRecoveryCode> recoveryCodeRepository,
            IStringLocalizer<AuthenticationResources> authLocalizer) : base(authLocalizer)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _recoveryCodeRepository = recoveryCodeRepository;
            _authLocalizer = authLocalizer;
        }

        public async Task<Response<TwoFactorStatusResponse>> Handle(GetTwoFactorStatusQuery request, CancellationToken cancellationToken)
        {
            // Get current user from HttpContext
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized<TwoFactorStatusResponse>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound<TwoFactorStatusResponse>(_authLocalizer[AuthenticationResourcesKeys.UserNotFound]);
            }

            // Get authenticator key
            var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);

            // Count unused recovery codes
            var recoveryCodesLeft = await _recoveryCodeRepository.GetTableNoTracking()
                .CountAsync(x => x.UserId == userId && !x.IsUsed, cancellationToken);

            var response = new TwoFactorStatusResponse
            {
                IsEnabled = user.TwoFactorEnabled,
                HasAuthenticatorKey = !string.IsNullOrEmpty(authenticatorKey),
                RecoveryCodesLeft = recoveryCodesLeft
            };

            return Success<TwoFactorStatusResponse>(entity: response);
        }
    }
}
