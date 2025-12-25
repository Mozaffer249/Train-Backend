using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Base;
using Sudan_Train.Core.Features.Authentication.Commands.TerminateSession;
using Sudan_Train.Core.Features.Authentication.Commands.TerminateAllSessions;
using Sudan_Train.Core.Features.Authentication.Queries.GetActiveSessions;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Authentication.Account
{
    /// <summary>
    /// User session management and monitoring
    /// </summary>
    public class SessionController : AppControllerBase
    {
        /// <summary>
        /// Get all active sessions for current user across all devices
        /// </summary>
        /// <param name="query">Get active sessions query</param>
        /// <returns>List of active sessions with device info, location, and timestamps</returns>
        [Authorize]
        [HttpGet(Router.AccountGetSessions)]
        public async Task<IActionResult> GetActiveSessions([FromQuery] GetActiveSessionsQuery query)
        {
            return NewResult(await Mediator.Send(query));
        }

        /// <summary>
        /// Terminate a specific session by session ID
        /// </summary>
        /// <param name="command">Session ID to terminate</param>
        /// <returns>Success message</returns>
        [Authorize]
        [HttpPost(Router.AccountTerminateSession)]
        public async Task<IActionResult> TerminateSession([FromBody] TerminateSessionCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }

        /// <summary>
        /// Terminate all sessions except the current one (logout from all devices)
        /// </summary>
        /// <param name="command">Terminate all sessions command</param>
        /// <returns>Success message with count of terminated sessions</returns>
        [Authorize]
        [HttpPost(Router.AccountTerminateAllSessions)]
        public async Task<IActionResult> TerminateAllSessions([FromBody] TerminateAllSessionsCommand command)
        {
            return NewResult(await Mediator.Send(command));
        }
    }
}
