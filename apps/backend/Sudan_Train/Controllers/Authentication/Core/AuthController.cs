using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Base;
using Sudan_Train.Core.Features.Authentication.Commands.Register;
using Sudan_Train.Core.Features.Authentication.Commands.Login;
using Sudan_Train.Core.Features.Authentication.Commands.Logout;
using Sudan_Train.Core.Features.Authentication.Commands.RefreshToken;
using Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmail;
using Sudan_Train.Core.Features.Authentication.Queries.ValidateToken;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Authentication.Core
{
    /// <summary>
    /// Core authentication operations: Register, Login, Logout, Token Management
    /// </summary>
    public class AuthController : AppControllerBase
    {
        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="command">Registration details including username, email, password</param>
        /// <returns>Registration result with user information</returns>
        [HttpPost(Router.AuthenticationRegister)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Confirm user email address with verification code
        /// </summary>
        /// <param name="command">User ID and confirmation code</param>
        /// <returns>Success message</returns>
        [HttpPost(Router.AuthenticationConfirmEmail)]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Login with username and password
        /// </summary>
        /// <param name="command">Login credentials (username/email and password)</param>
        /// <returns>JWT access token and refresh token</returns>
        [HttpPost(Router.AuthenticationLogin)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Logout user and revoke all tokens
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
        /// Refresh expired access token using valid refresh token
        /// </summary>
        /// <param name="command">Current access token and refresh token</param>
        /// <returns>New JWT access token and refresh token</returns>
        [HttpPost(Router.AuthenticationRefreshToken)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Validate JWT token and check if it's still valid
        /// </summary>
        /// <param name="query">Access token to validate</param>
        /// <returns>Token validation result</returns>
        [HttpGet(Router.AuthenticationValidateToken)]
        public async Task<IActionResult> ValidateToken([FromQuery] ValidateTokenQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }
    }
}
