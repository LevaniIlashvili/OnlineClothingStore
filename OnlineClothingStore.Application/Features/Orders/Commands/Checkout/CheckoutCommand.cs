using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Orders.Commands.Checkout
{
    public class CheckoutCommand : IRequest<OrderDTO>
    {
        public long UserId { get; set; }
        public string ShippingAddress { get; set; }
    }
}
