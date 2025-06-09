using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Carts.Commands.RemoveCartItem
{
    public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<RemoveCartItemCommandHandler> _logger;

        public RemoveCartItemCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            ICurrentUserService currentUserService,
            ILogger<RemoveCartItemCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation(
                "Handling RemoveCartItemCommand for User: {UserId}, CartItem ID: {CartItemId}",
                userId, request.CartItemId);

            var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
            if (cart is null)
            {
                _logger.LogWarning("Cart not found for User: {UserId}", userId);
                throw new Exceptions.NotFoundException("Cart not found");
            }

            var cartItem = await _cartItemRepository.GetByIdAsync(request.CartItemId, cancellationToken);
            if (cartItem is null)
            {
                _logger.LogWarning("Cart item not found with ID: {CartItemId} for User: {UserId}", request.CartItemId, userId);
                throw new Exceptions.NotFoundException("Cart item not found");
            }

            if (cartItem.CartId != cart.Id)
            {
                _logger.LogWarning("User: {UserId} attempted to remove CartItem ID: {CartItemId} not belonging to their cart", userId, request.CartItemId);
                throw new Exceptions.ForbiddenException("Access to this cart item is forbidden");
            }

            await _cartItemRepository.DeleteAsync(cartItem, cancellationToken);

            _logger.LogInformation("CartItem ID: {CartItemId} removed successfully for User: {UserId}", request.CartItemId, userId);
        }
    }
}
