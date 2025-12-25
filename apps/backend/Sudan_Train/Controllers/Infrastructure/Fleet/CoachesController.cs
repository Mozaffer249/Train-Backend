using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Infrastructure.Seats.Queries.GetSeatsByCoach;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Infrastructure.Fleet
{
    [ApiController]
    [Route(Router.Infrastructure + "/Coaches")]
    public class CoachesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CoachesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all seats for a specific coach
        /// </summary>
        [HttpGet("{coachId}/Seats")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetSeatsByCoach(int coachId)
        {
            var response = await _mediator.Send(new GetSeatsByCoachQuery { CoachId = coachId });
            return Ok(response);
        }
    }
}
