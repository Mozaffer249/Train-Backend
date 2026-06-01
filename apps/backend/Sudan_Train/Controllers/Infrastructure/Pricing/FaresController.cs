using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Infrastructure.Fares.Commands.CreateFare;
using Sudan_Train.Core.Features.Infrastructure.Fares.Commands.UpdateFare;
using Sudan_Train.Core.Features.Infrastructure.Fares.Queries.GetAllFares;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Infrastructure.Pricing
{
    [ApiController]
    [Route(Router.Infrastructure + "/Fares")]
    public class FaresController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FaresController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all fares (Public endpoint for viewing prices)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetFares([FromQuery] GetAllFaresQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        /// <summary>
        /// Create a new fare
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateFare([FromBody] CreateFareCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// PATCH-style fare update. Scope columns (route/segment/trip/class)
        /// cannot be changed — admin retires and recreates if the scope is wrong.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateFare(int id, [FromBody] UpdateFareCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
