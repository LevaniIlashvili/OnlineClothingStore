using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Carts.Commands.RemoveCartItem
{
    public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ICurrentUserService _currentUserService;

        public RemoveCartItemCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            ICurrentUserService currentUserService
        )
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _currentUserService = currentUserService;
        }

        public async Task Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
            if (cart is null)
            {
                throw new Exceptions.NotFoundException("Cart not found");
            }

            var cartItem = await _cartItemRepository.GetByIdAsync(request.CartItemId, cancellationToken);
            if (cartItem is null)
            {
                throw new Exceptions.NotFoundException("Cart item not found");
            }

            if (cartItem.CartId != cart.Id)
            {
                throw new Exceptions.ForbiddenException("Access to this cart item is forbidden");
            }

            await _cartItemRepository.DeleteAsync(cartItem, cancellationToken);
        }
    }
}
