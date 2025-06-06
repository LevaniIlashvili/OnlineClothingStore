using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Application.Features.Carts.Commands.AddToCart;
using OnlineClothingStore.Application.Features.Carts.Commands.RemoveCartItem;
using OnlineClothingStore.Application.Features.Carts.Commands.UpdateCartItem;
using OnlineClothingStore.Application.Features.Carts.Queries.GetCart;

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
        /// <response code="200">Returns the cart</response>
        /// <response code="404">Resource not found</response>
        /// <response code="401">Authentication Required</response>
        [HttpGet("")]
        [ProducesResponseType(typeof(CartDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CartDTO>> GetCart()
        {
            var query = new GetCartQuery();

            var cart = await _mediator.Send(query);

            return Ok(cart);
        }

        /// <summary>
        /// Adds an item to user's cart
        /// </summary>
        /// <param name="request">The data of cart item to add</param>
        /// <response code="201">Cart item was created</response>
        /// <response code="404">Resource not found</response>
        /// <response code="400">Validation failure</response>
        /// <response code="401">Authentication Required</response>
        [HttpPost("items")]
        [ProducesResponseType(typeof(CartItemDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CartItemDTO>> AddToCart([FromBody] AddToCartCommand request)
        {
            var cartItem = await _mediator.Send(request);

            return CreatedAtAction(nameof(GetCart), cartItem);
        }

        /// <summary>
        /// Updates the quantity of a specific item in a user's cart
        /// </summary>
        /// <param name="cartItemId">The id of cart item being updated</param>
        /// <param name="request">The updated data for cart item</param>
        /// <response code="200">Cart item was updated</response>
        /// <response code="404">Resource not found</response>
        /// <response code="400">Validation failure</response>
        /// <response code="401">Authentication Required</response>
        /// <response code="403">User not authorized</response>
        [HttpPut("items/{cartItemId}")]
        [ProducesResponseType(typeof(CartItemDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CartItemDTO>> UpdateCartItem(
            [FromRoute] int cartItemId,
            [FromBody] UpdateCartItemCommand request
        )
        {
            request.CartItemId = cartItemId;
            await _mediator.Send(request);

            return NoContent();
        }

        /// <summary>
        /// Removes an item from user's cart
        /// </summary>
        /// <param name="cartItemId">The id of cart item being removed</param>
        /// <response code="204">Cart item was deleted</response>
        /// <response code="404">Resource not found</response>
        /// <response code="401">Authentication Required</response>
        /// <response code="403">User not authorized</response>
        [HttpDelete("items/{cartItemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> RemoveCartItem([FromRoute] int cartItemId)
        {
            var command = new RemoveCartItemCommand { CartItemId = cartItemId };
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
