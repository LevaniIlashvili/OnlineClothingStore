using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<List<OrderDTO>> { }
}
