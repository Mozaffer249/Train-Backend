using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sudan_Train.Core.Features.Infrastructure.Trains.Commands.CreateTrain;
using Sudan_Train.Core.Features.Infrastructure.Trains.Commands.UpdateTrain;
using Sudan_Train.Core.Features.Infrastructure.Trains.Commands.DeleteTrain;
using Sudan_Train.Core.Features.Infrastructure.Trains.Queries.GetAllTrains;
using Sudan_Train.Core.Features.Infrastructure.Trains.Queries.GetTrainById;
using Sudan_Train.Core.Features.Infrastructure.Coaches.Commands.BulkCreateCoaches;
using Sudan_Train.Core.Features.Infrastructure.Coaches.Commands.UpdateCoach;
using Sudan_Train.Core.Features.Infrastructure.Coaches.Queries.GetCoachById;
using Sudan_Train.Core.Features.Infrastructure.Coaches.Queries.GetCoachesByTrain;
using Sudan_Train.Data.AppMetaData;

namespace Sudan_Train.Controllers.Infrastructure.Fleet
{
    [ApiController]
    [Route(Router.Infrastructure + "/Trains")]
    public class TrainsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TrainsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all trains
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetTrains([FromQuery] GetAllTrainsQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        /// <summary>
        /// Get train by ID with coaches and capacity info
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetTrain(int id)
        {
            var response = await _mediator.Send(new GetTrainByIdQuery { Id = id });
            return Ok(response);
        }

        /// <summary>
        /// Create a new train
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> CreateTrain([FromBody] CreateTrainCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Update an existing train
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateTrain(int id, [FromBody] UpdateTrainCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Delete a train (SuperAdmin only, cannot delete if active trips exist)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteTrain(int id)
        {
            var response = await _mediator.Send(new DeleteTrainCommand { Id = id });
            return Ok(response);
        }

        /// <summary>
        /// Get all coaches for a specific train
        /// </summary>
        [HttpGet("{trainId}/Coaches")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetCoachesByTrain(int trainId)
        {
            var response = await _mediator.Send(new GetCoachesByTrainQuery { TrainId = trainId });
            return Ok(response);
        }

        /// <summary>
        /// Bulk create coaches for a train with optional auto seat generation
        /// </summary>
        [HttpPost("{trainId}/Coaches/Bulk")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> BulkCreateCoaches(int trainId, [FromBody] BulkCreateCoachesCommand command)
        {
            command.TrainId = trainId;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Fetch a single coach by ID — useful for pre-filling the admin edit modal.
        /// </summary>
        [HttpGet("Coaches/{coachId:int}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> GetCoachById(int coachId)
        {
            var response = await _mediator.Send(new GetCoachByIdQuery { Id = coachId });
            return Ok(response);
        }

        /// <summary>
        /// PATCH-style coach update. Capacity is locked — seats are already wired
        /// and may have bookings. Editable: CoachNumber, Class, Sequence.
        /// </summary>
        [HttpPut("Coaches/{coachId:int}")]
        [Authorize(Roles = Roles.AdminOrStaff)]
        public async Task<IActionResult> UpdateCoach(int coachId, [FromBody] UpdateCoachCommand command)
        {
            command.Id = coachId;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
