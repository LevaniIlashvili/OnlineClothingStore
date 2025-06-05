using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;

namespace OnlineClothingStore.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler
        : IRequestHandler<UpdateOrderStatusCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;

        public UpdateOrderStatusCommandHandler(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IMapper mapper
        )
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order == null)
            {
                throw new Exceptions.NotFoundException($"Order not found");
            }

            order.OrderStatusId = request.OrderStatusId;
            order.LastUpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order, cancellationToken);
        }
    }
}
