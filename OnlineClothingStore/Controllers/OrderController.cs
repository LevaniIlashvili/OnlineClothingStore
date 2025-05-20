using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.DTOs;
using OnlineClothingStore.Models;

namespace OnlineClothingStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public static List<Order> Orders = new List<Order>
        {
            new Order
            {
                Id = 1,
                UserId = 1,
                OrderAmount = 100.00m,
                OrderDate = DateTime.UtcNow.AddDays(-3),
                ShippingAddress = "123 Main St",
                Status = OrderStatus.Delivered,
                Items = new List<OrderItem>
                {
                    new OrderItem { Id = 1, OrderId = 1, ProductVariantId = 1, Quantity = 2, UnitPrice = 20.00m },
                    new OrderItem { Id = 2, OrderId = 1, ProductVariantId = 3, Quantity = 1, UnitPrice = 60.00m }
                }
            }
        };

        /// <summary>
        /// Gets all orders for a specific user
        /// </summary>
        /// <param name="userId">The id of user to get orders for</param>
        /// <response code="200">Returns the list of orders for the user</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        public ActionResult<List<Order>> GetOrdersForUser(int userId)
        {
            var userOrders = Orders.Where(o => o.UserId == userId).ToList();
            return Ok(userOrders);
        }

        /// <summary>
        /// Creates a new order for a user using items from their cart
        /// </summary>
        /// <param name="userId">The id of user to create order for</param>
        /// <param name="orderDTO">Order details</param>
        /// <response code="201">Order created successfully</response>
        /// <response code="404">User's cart not found</response>
        [HttpPost("{userId}")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Order> CreateOrder([FromRoute] int userId, [FromBody] AddOrderDTO orderDTO)
        {
            var cart = CartController.Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart is null)
                return NotFound();

            var orderId = Orders.Any() ? Orders.Max(o => o.Id) + 1 : 1;

            var order = new Order()
            {
                Id = orderId,
                UserId = userId,
                Status = OrderStatus.Processing,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = orderDTO.ShippingAddress,
                Items = cart.Items.Select(i =>
                {
                    return new OrderItem()
                    {
                        Id = i.Id,
                        OrderId = orderId,
                        ProductVariantId = i.ProductVariantId,
                        Quantity = i.Quantity,
                        UnitPrice = ProductController.Products
                            .FirstOrDefault(p => p.Id == ProductController.ProductVariants
                            .FirstOrDefault(pv => pv.Id == i.ProductVariantId)!.ProductId)!.Price
                    };
                }).ToList(),
            };

            order.OrderAmount = order.Items.Sum(item => item.Quantity * item.UnitPrice);
            Orders.Add(order);
            cart.Items.Clear();

            return CreatedAtAction(nameof(GetOrdersForUser), new { userId }, order);
        }

        /// <summary>
        /// Updates the status of an existing order
        /// </summary>
        /// <param name="orderId">The id of the order to update</param>
        /// <param name="orderStatusDTO">The updated status</param>
        /// <response code="204">Status updated successfully</response>
        /// <response code="400">Invalid order status provided</response>
        /// <response code="404">Order not found</response>
        [HttpPut("{orderId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateOrderStatus(int orderId, UpdateOrderStatusDTO orderStatusDTO)
        {
            if (!Enum.TryParse(orderStatusDTO.OrderStatus, true, out OrderStatus parsedStatus))
            {
                return BadRequest(new { Message = "Invalid order status." });
            }

            var order = Orders.FirstOrDefault(o => o.Id == orderId);
            if (order is null)
                return NotFound();

            order.Status = parsedStatus;
            return NoContent();
        }
    }
}
