using MediatR;

namespace OnlineClothingStore.Application.Features.Carts.Commands.UpdateCartItem
{
    public class UpdateCartItemCommand : IRequest
    {
        public long CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
