using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Carts.Queries.GetCart
{
    public class GetCartQuery : IRequest<CartDTO>
    {
    }
}
