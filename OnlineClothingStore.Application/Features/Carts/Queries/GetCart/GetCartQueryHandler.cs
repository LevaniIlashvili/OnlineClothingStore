using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Carts.Queries.GetCart
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDTO>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetCartQueryHandler(ICartRepository cartRepository, ICurrentUserService currentUserService, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<CartDTO> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);

            if (cart is null)
            {
                throw new Exceptions.NotFoundException("Cart not found");
            }

            var cartDTO = new CartDTO()
            {
                Id = cart.Id,
                Items = _mapper.Map<List<CartItemDTO>>(cart.Items)
            };

            return cartDTO;
        }
    }
}
