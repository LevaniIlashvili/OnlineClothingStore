using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Orders.Queries.GetUserOrders
{
    public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, List<OrderDTO>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;

        public GetUserOrdersQueryHandler(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IMapper mapper
        )
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderDTO>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            var orderDTOs = _mapper.Map<List<OrderDTO>>(orders).ToList();

            foreach (var orderDTO in orderDTOs)
            {
                var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderDTO.Id, cancellationToken);
                orderDTO.Items = _mapper.Map<List<OrderItemDTO>>(orderItems);
            }

            return orderDTOs;
        }
    }
}
