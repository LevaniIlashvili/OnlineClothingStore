using MediatR;

namespace OnlineClothingStore.Application.Features.Carts.Commands.RemoveCartItem
{
    public class RemoveCartItemCommand : IRequest
    {
        public long UserId { get; set; }
        public long CartItemId { get; set; }
    }
}
