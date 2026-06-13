using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Base;
using Sudan_Train.Core.Features.Authentication.Commands.UpdateProfile;
using Sudan_Train.Core.Features.Authentication.Commands.ChangeEmail;
using Sudan_Train.Core.Features.Authentication.Commands.ConfirmEmailChange;
using Sudan_Train.Core.Features.Authentication.Commands.DeleteAccount;
using Sudan_Train.Core.Features.Authentication.Queries.GetProfile;
using Sudan_Train.Core.Features.Authentication.Queries.GetMe;
using Sudan_Train.Core.Features.Authentication.Queries.ExportUserData;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Authentication.Account
{
    /// <summary>
    /// User profile and account management
    /// </summary>
    public class ProfileController : AppControllerBase
    {
        /// <summary>
        /// Get current user profile information
        /// </summary>
        /// <param name="query">Get profile query</param>
        /// <returns>User profile details (name, email, phone, etc.)</returns>
        [Authorize]
        [HttpGet(Router.AccountGetProfile)]
        public async Task<IActionResult> GetProfile([FromQuery] GetProfileQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }

        /// <summary>
        /// Lightweight identity summary — userId, roles, and assigned station IDs.
        /// Used by the admin shell to decide which pages/links to show.
        /// </summary>
        [Authorize]
        [HttpGet(Router.Rule + "Account/Me")]
        public async Task<IActionResult> GetMe()
        {
            return NewResult(await Mediator.Send(new GetMeQuery()));
        }

        /// <summary>
        /// Update user profile information
        /// </summary>
        /// <param name="command">Updated profile details (first name, last name, phone, etc.)</param>
        /// <returns>Success message with updated profile</returns>
        [Authorize]
        [HttpPut(Router.AccountUpdateProfile)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Request email address change (sends confirmation code to new email)
        /// </summary>
        /// <param name="command">New email address</param>
        /// <returns>Success message (confirmation code sent)</returns>
        [Authorize]
        [HttpPost(Router.AccountChangeEmail)]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Confirm email address change with verification code
        /// </summary>
        /// <param name="command">User ID, new email, and confirmation code</param>
        /// <returns>Success message</returns>
        [HttpPost(Router.AccountConfirmEmailChange)]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Export all user data in JSON format (GDPR compliance)
        /// </summary>
        /// <param name="query">Export user data query</param>
        /// <returns>Complete user data including bookings, profile, settings</returns>
        [Authorize]
        [HttpGet(Router.AccountExportData)]
        public async Task<IActionResult> ExportUserData([FromQuery] ExportUserDataQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }

        /// <summary>
        /// Permanently delete user account and all associated data
        /// </summary>
        /// <param name="command">Password confirmation for account deletion</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpDelete(Router.AccountDelete)]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }
    }
}
