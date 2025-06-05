using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Application.Features.Orders.Commands.Checkout;
using OnlineClothingStore.Application.Features.Orders.Commands.UpdateOrderStatus;
using OnlineClothingStore.Application.Features.Orders.Queries.GetAllOrders;
using OnlineClothingStore.Application.Features.Orders.Queries.GetOrderById;
using OnlineClothingStore.Application.Features.Orders.Queries.GetUserOrders;

namespace OnlineClothingStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Process checkout for a user's cart
        /// </summary>
        /// <param name="userId">The ID of the user checking out</param>
        /// <param name="command">The checkout command containing shipping address</param>
        /// <response code="201">Returns the created order</response>
        /// <response code="400">If the cart is empty or there's insufficient stock</response>
        /// <response code="404">If the cart or any product and product variant is not found</response>
        [HttpPost("checkout/{userId}")]
        [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDTO>> Checkout(
            [FromRoute] long userId,
            [FromBody] CheckoutCommand command
        )
        {
            command.UserId = userId;
            var order = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        /// <summary>
        /// Get an order by ID
        /// </summary>
        /// <param name="id">The ID of the order to retrieve</param>
        /// <response code="200">Returns the requested order</response>
        /// <response code="404">If the order is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDTO>> GetOrder(long id)
        {
            var query = new GetOrderByIdQuery() { Id = id };
            var order = await _mediator.Send(query);

            return order;
        }

        /// <summary>
        /// Get all orders for a specific user
        /// </summary>
        /// <param name="userId">The ID of the user whose orders to retrieve</param>
        /// <response code="200">Returns the list of orders for the user</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<OrderDTO>>> GetUserOrders(long userId)
        {
            var query = new GetUserOrdersQuery { UserId = userId };
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }

        /// <summary>
        /// Get all orders in the system
        /// </summary>
        /// <response code="200">Returns the list of all orders</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<OrderDTO>>> GetAllOrders()
        {
            var query = new GetAllOrdersQuery();
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }

        /// <summary>
        /// Update the status of an order
        /// </summary>
        /// <param name="id">The ID of the order to update</param>
        /// <param name="statusId">The new status ID to set</param>
        /// <response code="204">If the order status was successfully updated</response>
        /// <response code="404">If the order is not found</response>
        [HttpPut("{id}/status/{statusId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus(long id, long statusId)
        {
            var command = new UpdateOrderStatusCommand { OrderId = id, OrderStatusId = statusId };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
