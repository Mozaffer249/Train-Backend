using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Base;
using Sudan_Train.Core.Features.Authentication.Commands.Register;
using Sudan_Train.Core.Features.Authentication.Commands.Login;
using Sudan_Train.Core.Features.Authentication.Commands.RefreshToken;
using Sudan_Train.Core.Features.Authentication.Commands.SendResetPasswordCode;
using Sudan_Train.Core.Features.Authentication.Commands.ResetPassword;
using Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmail;
using Sudan_Train.Core.Features.Authentication.Queries.ValidateToken;
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
    }

}