using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Carts.Commands.UpdateCartItem
{
    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IMapper _mapper;

        public UpdateCartItemCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IProductVariantRepository productVariantRepository,
            IMapper mapper
        )
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productVariantRepository = productVariantRepository;
            _mapper = mapper;
        }

        public async Task Handle(UpdateCartItemCommand request,CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (cart is null)
            {
                throw new Exceptions.NotFoundException("Cart not found");
            }

            var cartItem = await _cartItemRepository.GetByIdAsync(
                request.CartItemId,
                cancellationToken
            );
            if (cartItem is null || cartItem.CartId != cart.Id)
            {
                throw new Exceptions.NotFoundException("Cart item not found");
            }

            var productVariant = await _productVariantRepository.GetByIdAsync(cartItem.ProductVariantId, cancellationToken);
            if (productVariant is null)
            {
                throw new Exceptions.NotFoundException("Product variant not found");
            }

            if (request.Quantity > productVariant.StockQuantity)
            {
                throw new Exceptions.BadRequestException("Quantity cannot be greater than stock");
            }

            cartItem.Quantity = request.Quantity;
            cartItem.LastUpdatedAt = DateTime.UtcNow;

            await _cartItemRepository.UpdateAsync(cartItem, cancellationToken);
        }
    }
}
