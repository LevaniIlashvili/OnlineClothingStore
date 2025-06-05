using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDTO>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandler(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task<OrderDTO> Handle(GetOrderByIdQuery requst, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(requst.Id, cancellationToken);

            if (order is null)
            {
                throw new Exceptions.BadRequestException("Order not found");
            }

            var orderItems = await _orderItemRepository.GetByOrderIdAsync(requst.Id, cancellationToken);

            var orderDTO = _mapper.Map<OrderDTO>(order);

            orderDTO.Items = _mapper.Map<List<OrderItemDTO>>(orderItems);

            return orderDTO;
        }
    }
}
