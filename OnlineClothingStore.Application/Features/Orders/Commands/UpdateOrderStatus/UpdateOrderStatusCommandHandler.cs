using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;

namespace OnlineClothingStore.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler
        : IRequestHandler<UpdateOrderStatusCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

        public UpdateOrderStatusCommandHandler(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IMapper mapper,
            ILogger<UpdateOrderStatusCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateOrderStatusCommand for OrderId: {OrderId}, NewStatusId: {OrderStatusId}",
                request.OrderId, request.OrderStatusId);

            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order == null)
            {
                _logger.LogWarning("Order not found with ID: {OrderId}", request.OrderId);
                throw new Exceptions.NotFoundException("Order not found");
            }

            order.OrderStatusId = request.OrderStatusId;
            order.LastUpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order, cancellationToken);

            _logger.LogInformation("Order status updated successfully for OrderId: {OrderId} to StatusId: {OrderStatusId}",
                request.OrderId, request.OrderStatusId);
        }
    }
}
