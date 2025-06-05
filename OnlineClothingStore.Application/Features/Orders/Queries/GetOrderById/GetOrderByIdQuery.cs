using MediatR;
using OnlineClothingStore.Application.DTOs;


namespace OnlineClothingStore.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderDTO>
    {
        public long Id { get; set; }
    }
}
