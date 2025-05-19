using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.DTOs;
using OnlineClothingStore.Models;

namespace OnlineClothingStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        public static List<Cart> Carts = new List<Cart>
        {
            new Cart
            {
                Id = 1,
                UserId = 1,
                Items = new List<CartItem>
                {
                    new CartItem { Id = 1, CartId = 1, ProductVariantId = 1, Quantity = 2 },
                    new CartItem { Id = 2, CartId = 1, ProductVariantId = 4, Quantity = 1 }
                }
            },
            new Cart
            {
                Id = 2,
                UserId = 2,
                Items = new List<CartItem>()
            }
        };

        /// <summary>
        /// Gets the cart for a specific user
        /// </summary>
        [HttpGet("{userId}")]
        public ActionResult<Cart> GetCart([FromRoute] int userId)
        {
            var cart = Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart is null)
                return NotFound();

            return Ok(cart);
        }

        /// <summary>
        /// Adds an item to user's cart
        /// </summary>
        [HttpPost("{userId}/items")]
        public ActionResult<CartItem> AddCartItem([FromRoute] int userId, [FromBody] AddCartItemDTO cartItemDTO)
        {
            var cart = Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart is null)
                return NotFound();

            var productVariant = ProductController.ProductVariants.FirstOrDefault(pv => {
                return pv.ProductId == cartItemDTO.ProductId &&
                pv.Color.Equals(cartItemDTO.Color, StringComparison.OrdinalIgnoreCase) &&
                pv.Size.Equals(cartItemDTO.Size, StringComparison.OrdinalIgnoreCase);
                });

            if (productVariant is null)
                return NotFound();

            var cartItem = new CartItem
            {
                Id = cart.Items.Any() ? cart.Items.Max(i => i.Id) + 1 : 1,
                CartId = cart.Id,
                ProductVariantId = productVariant.Id,
                Quantity = cartItemDTO.Quantity
            };

            cart.Items.Add(cartItem);

            return CreatedAtAction(nameof(GetCart), new { userId }, cartItem);
        }

        /// <summary>
        /// Updates the quantity of a specific item in a user's cart
        /// </summary>
        [HttpPut("{userId}/items/{itemId}")]
        public ActionResult UpdateCartItem([FromRoute] int userId, [FromRoute] int itemId, [FromBody] AddCartItemDTO cartItemDTO)
        {
            var cart = Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart is null)
                return NotFound();

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item is null)
                return NotFound();

            var productVariant = ProductController.ProductVariants.FirstOrDefault(pv => {
                return pv.ProductId == cartItemDTO.ProductId 
                        && pv.Color.Equals(cartItemDTO.Color, StringComparison.OrdinalIgnoreCase) 
                        && pv.Size.Equals(cartItemDTO.Size, StringComparison.OrdinalIgnoreCase);
            });

            if (productVariant is null)
                return NotFound();

            item.Quantity = cartItemDTO.Quantity;
            item.ProductVariantId = productVariant.Id;
            return NoContent();
        }

        /// <summary>
        /// Removes an item from user's cart
        /// </summary>
        [HttpDelete("{userId}/items/{itemId}")]
        public ActionResult RemoveCartItem([FromRoute] int userId, [FromRoute] int itemId)
        {
            var cart = Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart is null)
                return NotFound();

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item is null)
                return NotFound();

            cart.Items.Remove(item);
            return NoContent();
        }
    }
}
