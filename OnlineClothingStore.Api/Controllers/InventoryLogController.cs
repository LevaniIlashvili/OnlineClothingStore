using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Application.Features.InventoryLogs.Commands;
using OnlineClothingStore.Application.Features.InventoryLogs.Queries.GetInventoryLogs;

namespace OnlineClothingStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all inventory logs
        /// </summary>
        /// <response code="200">Returns the list of inventory logs</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<InventoryLogDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<InventoryLogDTO>>> GetAll()
        {
            var query = new GetInventoryLogsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Create a new inventory log entry
        /// </summary>
        /// <param name="command">The inventory log details</param>
        /// <response code="201">Returns the created inventory log</response>
        /// <response code="400">If the stock change would result in negative quantity or change quantity is 0</response>
        /// <response code="404">If the product variant is not found</response>
        [HttpPost]
        [ProducesResponseType(typeof(InventoryLogDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryLogDTO>> Create(
            [FromBody] CreateInventoryLogCommand command
        )
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), result);
        }
    }
}
