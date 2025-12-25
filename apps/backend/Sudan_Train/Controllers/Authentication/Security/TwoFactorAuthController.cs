using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Base;
using Sudan_Train.Core.Features.Authentication.Commands.EnableTwoFactor;
using Sudan_Train.Core.Features.Authentication.Commands.VerifyTwoFactor;
using Sudan_Train.Core.Features.Authentication.Commands.DisableTwoFactor;
using Sudan_Train.Core.Features.Authentication.Commands.GenerateRecoveryCodes;
using Sudan_Train.Core.Features.Authentication.Commands.LoginWithTwoFactor;
using Sudan_Train.Core.Features.Authentication.Queries.GetTwoFactorStatus;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Authentication.Security
{
    /// <summary>
    /// Two-Factor Authentication (2FA) management
    /// </summary>
    public class TwoFactorAuthController : AppControllerBase
    {
        /// <summary>
        /// Enable two-factor authentication and get QR code for authenticator app
        /// </summary>
        /// <param name="command">Enable 2FA command</param>
        /// <returns>QR code URL and manual entry key for Google Authenticator/Authy</returns>
        [Authorize]
        [HttpPost(Router.AuthenticationEnableTwoFactor)]
        public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactorCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Verify two-factor authentication code and activate 2FA
        /// </summary>
        /// <param name="command">6-digit verification code from authenticator app</param>
        /// <returns>Success message and recovery codes</returns>
        [Authorize]
        [HttpPost(Router.AuthenticationVerifyTwoFactor)]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Disable two-factor authentication for current user
        /// </summary>
        /// <param name="command">Disable 2FA command (may require password confirmation)</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost(Router.AuthenticationDisableTwoFactor)]
        public async Task<IActionResult> DisableTwoFactor([FromBody] DisableTwoFactorCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Generate new recovery codes for two-factor authentication
        /// </summary>
        /// <param name="command">Generate recovery codes command</param>
        /// <returns>List of single-use recovery codes (store securely!)</returns>
        [Authorize]
        [HttpPost(Router.AuthenticationGenerateRecoveryCodes)]
        public async Task<IActionResult> GenerateRecoveryCodes([FromBody] GenerateRecoveryCodesCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Login with username, password, and two-factor authentication code
        /// </summary>
        /// <param name="command">Login credentials with 2FA code or recovery code</param>
        /// <returns>JWT access token and refresh token</returns>
        [HttpPost(Router.AuthenticationLoginWithTwoFactor)]
        public async Task<IActionResult> LoginWithTwoFactor([FromBody] LoginWithTwoFactorCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Get two-factor authentication status for current user
        /// </summary>
        /// <param name="query">Get 2FA status query</param>
        /// <returns>2FA enabled status and configuration details</returns>
        [Authorize]
        [HttpGet(Router.AuthenticationGetTwoFactorStatus)]
        public async Task<IActionResult> GetTwoFactorStatus([FromQuery] GetTwoFactorStatusQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }
    }
}
