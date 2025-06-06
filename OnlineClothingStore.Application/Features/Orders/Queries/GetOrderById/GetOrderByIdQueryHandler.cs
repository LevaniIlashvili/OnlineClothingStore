using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDTO>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandler(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<OrderDTO> Handle(GetOrderByIdQuery requst, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var role = _currentUserService.Role;

            var order = await _orderRepository.GetByIdAsync(requst.Id, cancellationToken);

            if (order is null)
            {
                throw new Exceptions.BadRequestException("Order not found");
            }

            if (order.UserId != userId && role != "Admin")
            {
                throw new Exceptions.ForbiddenException("User is not authorized to perform this action");
            }

            var orderItems = await _orderItemRepository.GetByOrderIdAsync(requst.Id, cancellationToken);

            var orderDTO = _mapper.Map<OrderDTO>(order);

            orderDTO.Items = _mapper.Map<List<OrderItemDTO>>(orderItems);

            return orderDTO;
        }
    }
}
