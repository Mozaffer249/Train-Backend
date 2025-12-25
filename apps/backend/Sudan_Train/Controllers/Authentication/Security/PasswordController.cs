using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Base;
using Sudan_Train.Core.Features.Authentication.Commands.ChangePassword;
using Sudan_Train.Core.Features.Authentication.Commands.SendResetPasswordCode;
using Sudan_Train.Core.Features.Authentication.Commands.ResetPassword;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Authentication.Security
{
    /// <summary>
    /// Password management operations
    /// </summary>
    public class PasswordController : AppControllerBase
    {
        /// <summary>
        /// Change password for authenticated user
        /// </summary>
        /// <param name="command">Current password and new password</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost(Router.AuthenticationChangePassword)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Send password reset code to user email (Forgot Password)
        /// </summary>
        /// <param name="command">User email address</param>
        /// <returns>Success message (code sent to email)</returns>
        [HttpPost(Router.AuthenticationSendResetPasswordCode)]
        public async Task<IActionResult> SendResetPasswordCode([FromBody] SendResetPasswordCodeCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Reset password using reset code from email
        /// </summary>
        /// <param name="command">Email, reset code, and new password</param>
        /// <returns>Success message</returns>
        [HttpPost(Router.AuthenticationResetPassword)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }
    }
}
