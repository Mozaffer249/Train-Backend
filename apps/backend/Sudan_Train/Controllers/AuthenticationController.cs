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
using Sudan_Train.Core.Features.Authentication.Commands.UpdateProfile;
using Sudan_Train.Core.Features.Authentication.Commands.ChangeEmail;
using Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmailChange;
using Sudan_Train.Core.Features.Authentication.Commands.TerminateSession;
using Sudan_Train.Core.Features.Authentication.Commands.TerminateAllSessions;
using Sudan_Train.Core.Features.Authentication.Commands.DeleteAccount;
using Sudan_Train.Core.Features.Authentication.Commands.TrustDevice;
using Sudan_Train.Core.Features.Authentication.Commands.RemoveTrustedDevice;
using Sudan_Train.Core.Features.Authentication.Queries.ValidateToken;
using Sudan_Train.Core.Features.Authentication.Queries.GetTwoFactorStatus;
using Sudan_Train.Core.Features.Authentication.Queries.GetProfile;
using Sudan_Train.Core.Features.Authentication.Queries.GetActiveSessions;
using Sudan_Train.Core.Features.Authentication.Queries.ExportUserData;
using Sudan_Train.Core.Features.Authentication.Queries.GetTrustedDevices;
using Sudan_Train.Core.Features.Authentication.Queries.GetSecurityEvents;
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

        #region Account Management

        /// <summary>
        /// Get user profile information
        /// </summary>
        /// <param name="query">Get profile query</param>
        /// <returns>User profile details</returns>
        [Authorize]
        [HttpGet(Router.AccountGetProfile)]
        public async Task<IActionResult> GetProfile([FromQuery] GetProfileQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }

        /// <summary>
        /// Update user profile information
        /// </summary>
        /// <param name="command">Update profile command</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPut(Router.AccountUpdateProfile)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Request email address change
        /// </summary>
        /// <param name="command">Change email command</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost(Router.AccountChangeEmail)]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Confirm email address change
        /// </summary>
        /// <param name="command">Confirm email change command</param>
        /// <returns>Success message</returns>
        [HttpPost(Router.AccountConfirmEmailChange)]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Get all active sessions for current user
        /// </summary>
        /// <param name="query">Get active sessions query</param>
        /// <returns>List of active sessions</returns>
        [Authorize]
        [HttpGet(Router.AccountGetSessions)]
        public async Task<IActionResult> GetActiveSessions([FromQuery] GetActiveSessionsQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }

        /// <summary>
        /// Terminate a specific session
        /// </summary>
        /// <param name="command">Terminate session command</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost(Router.AccountTerminateSession)]
        public async Task<IActionResult> TerminateSession([FromBody] TerminateSessionCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Terminate all sessions except current
        /// </summary>
        /// <param name="command">Terminate all sessions command</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost(Router.AccountTerminateAllSessions)]
        public async Task<IActionResult> TerminateAllSessions([FromBody] TerminateAllSessionsCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Export all user data (GDPR compliance)
        /// </summary>
        /// <param name="query">Export user data query</param>
        /// <returns>User data in JSON format</returns>
        [Authorize]
        [HttpGet(Router.AccountExportData)]
        public async Task<IActionResult> ExportUserData([FromQuery] ExportUserDataQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }

        /// <summary>
        /// Delete user account permanently
        /// </summary>
        /// <param name="command">Delete account command</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpDelete(Router.AccountDelete)]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Get all trusted devices for current user
        /// </summary>
        /// <param name="query">Get trusted devices query</param>
        /// <returns>List of trusted devices</returns>
        [Authorize]
        [HttpGet(Router.AccountGetTrustedDevices)]
        public async Task<IActionResult> GetTrustedDevices([FromQuery] GetTrustedDevicesQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }

        /// <summary>
        /// Trust current device to skip 2FA on future logins
        /// </summary>
        /// <param name="command">Trust device command</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost(Router.AccountTrustDevice)]
        public async Task<IActionResult> TrustDevice([FromBody] TrustDeviceCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Remove a trusted device
        /// </summary>
        /// <param name="command">Remove trusted device command</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpDelete(Router.AccountRemoveTrustedDevice)]
        public async Task<IActionResult> RemoveTrustedDevice([FromBody] RemoveTrustedDeviceCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Get security events (login alerts, password changes, etc.)
        /// </summary>
        /// <param name="query">Get security events query</param>
        /// <returns>List of security events</returns>
        [Authorize]
        [HttpGet(Router.AccountGetSecurityEvents)]
        public async Task<IActionResult> GetSecurityEvents([FromQuery] GetSecurityEventsQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }

        #endregion
    }

}