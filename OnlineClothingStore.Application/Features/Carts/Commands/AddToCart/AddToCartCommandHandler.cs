using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.Carts.Commands.AddToCart
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, CartItemDTO>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddToCartCommandHandler> _logger;

        public AddToCartCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IProductVariantRepository productVariantRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<AddToCartCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productVariantRepository = productVariantRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CartItemDTO> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation(
                "Handling AddToCartCommand for User: {UserId}, ProductVariantId: {ProductVariantId}, Quantity: {Quantity}",
                userId, request.ProductVariantId, request.Quantity);

            var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);

            if (cart is null)
            {
                _logger.LogWarning("Cart not found for User: {UserId}", userId);
                throw new Exceptions.NotFoundException("Cart not found");
            }

            var productVariant = await _productVariantRepository.GetByIdAsync(request.ProductVariantId, cancellationToken);

            if (productVariant is null)
            {
                _logger.LogWarning(
                    "Product variant not found with ID: {ProductVariantId} for User: {UserId}",
                    request.ProductVariantId, userId);
                throw new Exceptions.NotFoundException("Product variant not found");
            }

            if (request.Quantity > productVariant.StockQuantity)
            {
                _logger.LogWarning(
                    "Quantity {Quantity} requested for ProductVariantId {ProductVariantId} exceeds stock {StockQuantity}",
                    request.Quantity, request.ProductVariantId, productVariant.StockQuantity);
                throw new Exceptions.BadRequestException("Quantity cannot be greater than stock");
            }

            var cartItems = await _cartItemRepository.GetByCartIdAsync(cart.Id, cancellationToken);
            var existingItem = cartItems.FirstOrDefault(ci => ci.ProductVariantId == request.ProductVariantId);

            CartItem addedCartItem;
            if (existingItem is not null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.LastUpdatedAt = DateTime.UtcNow;
                await _cartItemRepository.UpdateAsync(existingItem, cancellationToken);
                addedCartItem = existingItem;

                _logger.LogInformation(
                    "Updated CartItem ID: {CartItemId} Quantity to {Quantity} for User: {UserId}",
                    existingItem.Id, existingItem.Quantity, userId);
            }
            else
            {
                var cartItem = new CartItem()
                {
                    CartId = cart.Id,
                    ProductVariantId = productVariant.Id,
                    Quantity = request.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                addedCartItem = await _cartItemRepository.AddAsync(cartItem, cancellationToken);

                _logger.LogInformation(
                    "Added new CartItem ID: {CartItemId} with Quantity {Quantity} for User: {UserId}",
                    addedCartItem.Id, addedCartItem.Quantity, userId);
            }

            return _mapper.Map<CartItemDTO>(addedCartItem);
        }
    }
}
