using MediatR;

namespace OnlineClothingStore.Application.Features.Carts.Commands.RemoveCartItem
{
    public class RemoveCartItemCommand : IRequest
    {
        public long CartItemId { get; set; }
    }
}
