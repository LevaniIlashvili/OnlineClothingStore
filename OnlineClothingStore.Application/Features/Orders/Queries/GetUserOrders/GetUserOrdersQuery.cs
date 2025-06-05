using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Orders.Queries.GetUserOrders
{
    public class GetUserOrdersQuery : IRequest<List<OrderDTO>>
    {
        public long UserId { get; set; }
    }
}
