using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Carts.Commands.AddToCart
{
    public class AddToCartCommand : IRequest<CartItemDTO>
    {
        public long ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}
