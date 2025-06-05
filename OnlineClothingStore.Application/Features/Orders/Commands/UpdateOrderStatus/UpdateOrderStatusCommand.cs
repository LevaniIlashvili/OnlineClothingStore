using MediatR;

namespace OnlineClothingStore.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommand : IRequest
    {
        public long OrderId { get; set; }
        public long OrderStatusId { get; set; }
    }
}
