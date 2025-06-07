using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Carts.Commands.UpdateCartItem
{
    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCartItemCommandHandler> _logger;

        public UpdateCartItemCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IProductVariantRepository productVariantRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<UpdateCartItemCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productVariantRepository = productVariantRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation(
                "Handling UpdateCartItemCommand for User: {UserId}, CartItem ID: {CartItemId}",
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
                _logger.LogWarning("User: {UserId} attempted to update CartItem ID: {CartItemId} not belonging to their cart", userId, request.CartItemId);
                throw new Exceptions.ForbiddenException("Access to this cart item is forbidden");
            }

            var productVariant = await _productVariantRepository.GetByIdAsync(cartItem.ProductVariantId, cancellationToken);
            if (productVariant is null)
            {
                _logger.LogWarning("Product variant not found for CartItem ID: {CartItemId}", request.CartItemId);
                throw new Exceptions.NotFoundException("Product variant not found");
            }

            if (request.Quantity > productVariant.StockQuantity)
            {
                _logger.LogWarning("Requested quantity {RequestedQuantity} exceeds stock {StockQuantity} for CartItem ID: {CartItemId}", request.Quantity, productVariant.StockQuantity, request.CartItemId);
                throw new Exceptions.BadRequestException("Quantity cannot be greater than stock");
            }

            cartItem.Quantity = request.Quantity;
            cartItem.LastUpdatedAt = DateTime.UtcNow;

            await _cartItemRepository.UpdateAsync(cartItem, cancellationToken);

            _logger.LogInformation("CartItem ID: {CartItemId} updated successfully with Quantity: {Quantity}", request.CartItemId, request.Quantity);
        }
    }
}
