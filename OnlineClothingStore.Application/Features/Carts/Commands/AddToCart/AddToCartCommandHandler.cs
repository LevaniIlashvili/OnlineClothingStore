using AutoMapper;
using MediatR;
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

        public AddToCartCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IProductVariantRepository productVariantRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productVariantRepository = productVariantRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<CartItemDTO> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);

            if (cart is null)
            {
                throw new Exceptions.NotFoundException("Cart not found");
            }

            var productVariant = await _productVariantRepository.GetByIdAsync(request.ProductVariantId, cancellationToken);
            
            if (productVariant is null)
            {
                throw new Exceptions.NotFoundException("Product variant not found");
            }

            if (request.Quantity > productVariant.StockQuantity)
            {
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
            } else
            {
                var cartItem = new CartItem()
                {
                    CartId = cart.Id,
                    ProductVariantId = productVariant.Id,
                    Quantity = request.Quantity
                };
                cartItem.CreatedAt = DateTime.UtcNow;
                addedCartItem = await _cartItemRepository.AddAsync(cartItem, cancellationToken);
            }

            return _mapper.Map<CartItemDTO>(addedCartItem);
        }
    }
}
