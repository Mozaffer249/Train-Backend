using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Base;
using Sudan_Train.Core.Features.Authentication.Commands.Register;
using Sudan_Train.Core.Features.Authentication.Commands.Login;
using Sudan_Train.Core.Features.Authentication.Commands.Logout;
using Sudan_Train.Core.Features.Authentication.Commands.ChangePassword;
using Sudan_Train.Core.Features.Authentication.Commands.RefreshToken;
using Sudan_Train.Core.Features.Authentication.Commands.SendResetPasswordCode;
using Sudan_Train.Core.Features.Authentication.Commands.ResetPassword;
using Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmail;
using Sudan_Train.Core.Features.Authentication.Commands.EnableTwoFactor;
using Sudan_Train.Core.Features.Authentication.Commands.VerifyTwoFactor;
using Sudan_Train.Core.Features.Authentication.Commands.DisableTwoFactor;
using Sudan_Train.Core.Features.Authentication.Commands.GenerateRecoveryCodes;
using Sudan_Train.Core.Features.Authentication.Commands.LoginWithTwoFactor;
using Sudan_Train.Core.Features.Authentication.Queries.ValidateToken;
using Sudan_Train.Core.Features.Authentication.Queries.GetTwoFactorStatus;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Api.Controllers
{
    public class AuthenticationController : AppControllerBase
    {
        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="command">Registration details</param>
        /// <returns>Registration result</returns>
        [HttpPost(Router.AuthenticationRegister)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Login with username and password
        /// </summary>
        /// <param name="command">Login credentials</param>
        /// <returns>JWT authentication result</returns>
        [HttpPost(Router.AuthenticationLogin)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Logout user and revoke tokens
        /// </summary>
        /// <param name="command">Logout details</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost(Router.AuthenticationLogout)]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="command">Refresh token details</param>
        /// <returns>New JWT authentication result</returns>
        [HttpPost(Router.AuthenticationRefreshToken)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Change user password while authenticated
        /// </summary>
        /// <param name="command">Change password details</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost(Router.AuthenticationChangePassword)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Send password reset code to user email
        /// </summary>
        /// <param name="command">Email address</param>
        /// <returns>Success message</returns>
        [HttpPost(Router.AuthenticationSendResetPasswordCode)]
        public async Task<IActionResult> SendResetPasswordCode([FromBody] SendResetPasswordCodeCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Reset password using reset code
        /// </summary>
        /// <param name="command">Reset password details</param>
        /// <returns>Success message</returns>
        [HttpPost(Router.AuthenticationResetPassword)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Confirm user email address
        /// </summary>
        /// <param name="command">Confirmation details</param>
        /// <returns>Success message</returns>
        [HttpPost(Router.AuthenticationConfirmEmail)]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Validate JWT token
        /// </summary>
        /// <param name="query">Token to validate</param>
        /// <returns>Validation result</returns>
        [HttpGet(Router.AuthenticationValidateToken)]
        public async Task<IActionResult> ValidateToken([FromQuery] ValidateTokenQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }

        /// <summary>
        /// Enable two-factor authentication and get QR code
        /// </summary>
        /// <param name="command">Enable 2FA command</param>
        /// <returns>QR code URL and manual entry key</returns>
        [Authorize]
        [HttpPost(Router.AuthenticationEnableTwoFactor)]
        public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactorCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Verify two-factor authentication code and activate 2FA
        /// </summary>
        /// <param name="command">Verification command</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost(Router.AuthenticationVerifyTwoFactor)]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Disable two-factor authentication
        /// </summary>
        /// <param name="command">Disable 2FA command</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost(Router.AuthenticationDisableTwoFactor)]
        public async Task<IActionResult> DisableTwoFactor([FromBody] DisableTwoFactorCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Generate recovery codes for two-factor authentication
        /// </summary>
        /// <param name="command">Generate recovery codes command</param>
        /// <returns>List of recovery codes</returns>
        [Authorize]
        [HttpPost(Router.AuthenticationGenerateRecoveryCodes)]
        public async Task<IActionResult> GenerateRecoveryCodes([FromBody] GenerateRecoveryCodesCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Login with two-factor authentication code
        /// </summary>
        /// <param name="command">Login with 2FA command</param>
        /// <returns>JWT authentication result</returns>
        [HttpPost(Router.AuthenticationLoginWithTwoFactor)]
        public async Task<IActionResult> LoginWithTwoFactor([FromBody] LoginWithTwoFactorCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Get two-factor authentication status
        /// </summary>
        /// <param name="query">Get 2FA status query</param>
        /// <returns>2FA status information</returns>
        [Authorize]
        [HttpGet(Router.AuthenticationGetTwoFactorStatus)]
        public async Task<IActionResult> GetTwoFactorStatus([FromQuery] GetTwoFactorStatusQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }
    }

}