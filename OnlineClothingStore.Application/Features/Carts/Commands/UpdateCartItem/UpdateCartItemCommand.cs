using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Carts.Commands.UpdateCartItem
{
    public class UpdateCartItemCommand : IRequest
    {
        public long UserId { get; set; }
        public long CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
