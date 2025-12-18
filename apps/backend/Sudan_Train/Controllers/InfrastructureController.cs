using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Infrastructure.Regions.Commands.CreateRegion;
using Sudan_Train.Core.Features.Infrastructure.Regions.Commands.UpdateRegion;
using Sudan_Train.Core.Features.Infrastructure.Regions.Commands.DeleteRegion;
using Sudan_Train.Core.Features.Infrastructure.Regions.Queries.GetAllRegions;
using Sudan_Train.Core.Features.Infrastructure.Regions.Queries.GetRegionById;
using Sudan_Train.Core.Features.Infrastructure.States.Commands.CreateState;
using Sudan_Train.Core.Features.Infrastructure.States.Commands.UpdateState;
using Sudan_Train.Core.Features.Infrastructure.States.Commands.DeleteState;
using Sudan_Train.Core.Features.Infrastructure.States.Queries.GetAllStates;
using Sudan_Train.Core.Features.Infrastructure.States.Queries.GetStateById;
using Sudan_Train.Core.Features.Infrastructure.Cities.Commands.CreateCity;
using Sudan_Train.Core.Features.Infrastructure.Cities.Commands.UpdateCity;
using Sudan_Train.Core.Features.Infrastructure.Cities.Commands.DeleteCity;
using Sudan_Train.Core.Features.Infrastructure.Cities.Queries.GetAllCities;
using Sudan_Train.Core.Features.Infrastructure.Cities.Queries.GetCityById;
using Sudan_Train.Core.Features.Infrastructure.Stations.Commands.CreateStation;
using Sudan_Train.Core.Features.Infrastructure.Stations.Commands.UpdateStation;
using Sudan_Train.Core.Features.Infrastructure.Stations.Commands.DeleteStation;
using Sudan_Train.Core.Features.Infrastructure.Stations.Queries.GetAllStations;
using Sudan_Train.Core.Features.Infrastructure.Stations.Queries.GetStationById;
using Sudan_Train.Core.Features.Infrastructure.Routes.Commands.CreateRoute;
using Sudan_Train.Core.Features.Infrastructure.Routes.Commands.UpdateRoute;
using Sudan_Train.Core.Features.Infrastructure.Routes.Commands.DeleteRoute;
using Sudan_Train.Core.Features.Infrastructure.Routes.Commands.AddRouteStation;
using Sudan_Train.Core.Features.Infrastructure.Routes.Commands.RemoveRouteStation;
using Sudan_Train.Core.Features.Infrastructure.Routes.Queries.GetAllRoutes;
using Sudan_Train.Core.Features.Infrastructure.Routes.Queries.GetRouteById;
using Sudan_Train.Core.Features.Infrastructure.Trains.Commands.CreateTrain;
using Sudan_Train.Core.Features.Infrastructure.Trains.Commands.UpdateTrain;
using Sudan_Train.Core.Features.Infrastructure.Trains.Commands.DeleteTrain;
using Sudan_Train.Core.Features.Infrastructure.Trains.Queries.GetAllTrains;
using Sudan_Train.Core.Features.Infrastructure.Trains.Queries.GetTrainById;
using Sudan_Train.Core.Features.Infrastructure.Coaches.Commands.BulkCreateCoaches;
using Sudan_Train.Core.Features.Infrastructure.Coaches.Queries.GetCoachesByTrain;
using Sudan_Train.Core.Features.Infrastructure.Seats.Queries.GetSeatsByCoach;
using Sudan_Train.Core.Features.Infrastructure.Trips.Commands.CreateTrip;
using Sudan_Train.Core.Features.Infrastructure.Trips.Commands.UpdateTrip;
using Sudan_Train.Core.Features.Infrastructure.Trips.Commands.CancelTrip;
using Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetAllTrips;
using Sudan_Train.Core.Features.Infrastructure.Trips.Queries.GetTripById;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers
{
    [ApiController]
    [Route(Router.Infrastructure)]
    public class InfrastructureController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InfrastructureController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Regions
        [HttpGet("Regions")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetRegions()
        {
            var response = await _mediator.Send(new GetAllRegionsQuery());
            return Ok(response);
        }

        [HttpGet("Regions/{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetRegion(int id)
        {
            var response = await _mediator.Send(new GetRegionByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpPost("Regions")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateRegion([FromBody] CreateRegionCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("Regions/{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateRegion(int id, [FromBody] UpdateRegionCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("Regions/{id}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteRegion(int id)
        {
            var response = await _mediator.Send(new DeleteRegionCommand { Id = id });
            return Ok(response);
        }
        #endregion

        #region States
        [HttpGet("States")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetStates([FromQuery] int? regionId)
        {
            var response = await _mediator.Send(new GetAllStatesQuery { RegionId = regionId });
            return Ok(response);
        }

        [HttpGet("States/{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetState(int id)
        {
            var response = await _mediator.Send(new GetStateByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpPost("States")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateState([FromBody] CreateStateCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("States/{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateState(int id, [FromBody] UpdateStateCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("States/{id}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteState(int id)
        {
            var response = await _mediator.Send(new DeleteStateCommand { Id = id });
            return Ok(response);
        }
        #endregion

        #region Cities
        [HttpGet("Cities")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetCities([FromQuery] int? stateId)
        {
            var response = await _mediator.Send(new GetAllCitiesQuery { StateId = stateId });
            return Ok(response);
        }

        [HttpGet("Cities/{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetCity(int id)
        {
            var response = await _mediator.Send(new GetCityByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpPost("Cities")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateCity([FromBody] CreateCityCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("Cities/{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] UpdateCityCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("Cities/{id}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var response = await _mediator.Send(new DeleteCityCommand { Id = id });
            return Ok(response);
        }
        #endregion

        #region Stations
        [HttpGet("Stations")]
        [AllowAnonymous] // Public endpoint for customer search
        public async Task<IActionResult> GetStations([FromQuery] GetAllStationsQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("Stations/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStation(int id)
        {
            var response = await _mediator.Send(new GetStationByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpPost("Stations")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateStation([FromBody] CreateStationCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("Stations/{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateStation(int id, [FromBody] UpdateStationCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("Stations/{id}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var response = await _mediator.Send(new DeleteStationCommand { Id = id });
            return Ok(response);
        }
        #endregion

        #region Routes
        [HttpGet("Routes")]
        [AllowAnonymous] // Public endpoint for customer search
        public async Task<IActionResult> GetRoutes([FromQuery] GetAllRoutesQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("Routes/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoute(int id)
        {
            var response = await _mediator.Send(new GetRouteByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpPost("Routes")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("Routes/{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] UpdateRouteCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("Routes/{id}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var response = await _mediator.Send(new DeleteRouteCommand { Id = id });
            return Ok(response);
        }

        [HttpPost("Routes/{routeId}/Stations")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> AddRouteStation(int routeId, [FromBody] AddRouteStationCommand command)
        {
            command.RouteId = routeId;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("Routes/{routeId}/Stations/{stationId}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> RemoveRouteStation(int routeId, int stationId)
        {
            var response = await _mediator.Send(new RemoveRouteStationCommand { RouteId = routeId, StationId = stationId });
            return Ok(response);
        }
        #endregion

        #region Trains
        [HttpGet("Trains")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetTrains([FromQuery] GetAllTrainsQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("Trains/{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetTrain(int id)
        {
            var response = await _mediator.Send(new GetTrainByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpPost("Trains")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateTrain([FromBody] CreateTrainCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("Trains/{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateTrain(int id, [FromBody] UpdateTrainCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("Trains/{id}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteTrain(int id)
        {
            var response = await _mediator.Send(new DeleteTrainCommand { Id = id });
            return Ok(response);
        }

        [HttpGet("Trains/{trainId}/Coaches")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetCoachesByTrain(int trainId)
        {
            var response = await _mediator.Send(new GetCoachesByTrainQuery { TrainId = trainId });
            return Ok(response);
        }

        [HttpPost("Trains/{trainId}/Coaches/Bulk")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> BulkCreateCoaches(int trainId, [FromBody] BulkCreateCoachesCommand command)
        {
            command.TrainId = trainId;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        #endregion

        #region Seats
        [HttpGet("Coaches/{coachId}/Seats")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetSeatsByCoach(int coachId)
        {
            var response = await _mediator.Send(new GetSeatsByCoachQuery { CoachId = coachId });
            return Ok(response);
        }
        #endregion

        #region Trips
        [HttpGet("Trips")]
        [AllowAnonymous] // Public endpoint for customer search
        public async Task<IActionResult> GetTrips([FromQuery] GetAllTripsQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("Trips/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTrip(int id)
        {
            var response = await _mediator.Send(new GetTripByIdQuery { Id = id });
            return Ok(response);
        }

        [HttpPost("Trips")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("Trips/{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateTrip(int id, [FromBody] UpdateTripCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("Trips/{id}/Cancel")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CancelTrip(int id)
        {
            var response = await _mediator.Send(new CancelTripCommand { Id = id });
            return Ok(response);
        }
        #endregion
    }
}

