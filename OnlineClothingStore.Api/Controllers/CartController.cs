using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Application.Features.Carts.Commands.AddToCart;
using OnlineClothingStore.Application.Features.Carts.Commands.RemoveCartItem;
using OnlineClothingStore.Application.Features.Carts.Commands.UpdateCartItem;
using OnlineClothingStore.Application.Features.Carts.Queries.GetCart;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets the cart for a specific user
        /// </summary>
        /// <param name="userId">The id of user to get cart for</param>
        /// <response code="200">Returns the cart</response>
        /// <response code="404">Resource not found</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(CartDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartDTO>> GetCart([FromRoute] int userId)
        {
            var query = new GetCartQuery() { UserId = userId };

            var cart = await _mediator.Send(query);

            return Ok(cart);
        }

        /// <summary>
        /// Adds an item to user's cart
        /// </summary>
        /// <param name="userId">The id of user whose cart is being updated</param>
        /// <param name="request">The data of cart item to add</param>
        /// <response code="201">Cart item was created</response>
        /// <response code="404">Resource not found</response>
        /// <response code="400">Validation failure</response>
        [HttpPost("{userId}/items")]
        [ProducesResponseType(typeof(CartItemDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CartItemDTO>> AddToCart(
            [FromRoute] int userId,
            [FromBody] AddToCartCommand request
        )
        {
            request.UserId = userId;
            var cartItem = await _mediator.Send(request);

            return CreatedAtAction(nameof(GetCart), new { userId }, cartItem);
        }

        /// <summary>
        /// Updates the quantity of a specific item in a user's cart
        /// </summary>
        /// <param name="userId">The id of user whose cart item is being updated</param>
        /// <param name="cartItemId">The id of cart item being updated</param>
        /// <param name="request">The updated data for cart item</param>
        /// <response code="200">Cart item was updated</response>
        /// <response code="404">Resource not found</response>
        /// /// <response code="400">Validation failure</response>
        [HttpPut("{userId}/items/{cartItemId}")]
        [ProducesResponseType(typeof(CartItemDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CartItemDTO>> UpdateCartItem(
            [FromRoute] int userId,
            [FromRoute] int cartItemId,
            [FromBody] UpdateCartItemCommand request
        )
        {
            request.UserId = userId;
            request.CartItemId = cartItemId;
            await _mediator.Send(request);

            return NoContent();
        }

        /// <summary>
        /// Removes an item from user's cart
        /// </summary>
        /// <param name="userId">The id of user whose cart item is being removed</param>
        /// <param name="cartItemId">The id of cart item being removed</param>
        /// <response code="204">Cart item was deleted</response>
        /// <response code="404">Resource not found</response>
        [HttpDelete("{userId}/items/{cartItemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveCartItem(
            [FromRoute] int userId,
            [FromRoute] int cartItemId
        )
        {
            var command = new RemoveCartItemCommand { UserId = userId, CartItemId = cartItemId };
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
