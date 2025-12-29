using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Infrastructure.Routes.Commands.CreateRoute;
using Sudan_Train.Core.Features.Infrastructure.Routes.Commands.UpdateRoute;
using Sudan_Train.Core.Features.Infrastructure.Routes.Commands.DeleteRoute;
using Sudan_Train.Core.Features.Infrastructure.Routes.Commands.AddRouteStation;
using Sudan_Train.Core.Features.Infrastructure.Routes.Commands.UpdateRouteStation;
using Sudan_Train.Core.Features.Infrastructure.Routes.Commands.RemoveRouteStation;
using Sudan_Train.Core.Features.Infrastructure.Routes.Queries.GetAllRoutes;
using Sudan_Train.Core.Features.Infrastructure.Routes.Queries.GetRouteById;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Infrastructure.RailwayNetwork
{
    [ApiController]
    [Route(Router.Infrastructure + "/Routes")]
    public class RoutesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoutesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all routes (Public endpoint for customer search)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoutes([FromQuery] GetAllRoutesQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        /// <summary>
        /// Get route by ID with all stations (Public endpoint)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoute(int id)
        {
            var response = await _mediator.Send(new GetRouteByIdQuery { Id = id });
            return Ok(response);
        }

        /// <summary>
        /// Create a new route
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Update an existing route
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] UpdateRouteCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Delete a route (SuperAdmin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var response = await _mediator.Send(new DeleteRouteCommand { Id = id });
            return Ok(response);
        }

        /// <summary>
        /// Add an intermediate station to a route
        /// </summary>
        [HttpPost("{routeId}/Stations")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> AddRouteStation(int routeId, [FromBody] AddRouteStationCommand command)
        {
            command.RouteId = routeId;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Update a station in a route (reorder or change timing)
        /// </summary>
        [HttpPut("{routeId}/Stations/{stationId}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateRouteStation(int routeId, int stationId, [FromBody] UpdateRouteStationCommand command)
        {
            command.RouteId = routeId;
            command.StationId = stationId;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Remove a station from a route
        /// </summary>
        [HttpDelete("{routeId}/Stations/{stationId}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> RemoveRouteStation(int routeId, int stationId)
        {
            var response = await _mediator.Send(new RemoveRouteStationCommand { RouteId = routeId, StationId = stationId });
            return Ok(response);
        }
    }
}
