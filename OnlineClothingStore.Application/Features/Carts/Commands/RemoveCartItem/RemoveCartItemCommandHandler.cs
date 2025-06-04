using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;

namespace OnlineClothingStore.Application.Features.Carts.Commands.RemoveCartItem
{
    public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;

        public RemoveCartItemCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository
        )
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (cart is null)
            {
                throw new Exceptions.NotFoundException("Cart not found");
            }

            var cartItem = await _cartItemRepository.GetByIdAsync(request.CartItemId, cancellationToken);
            if (cartItem is null || cartItem.CartId != cart.Id)
            {
                throw new Exceptions.NotFoundException("Cart item not found");
            }

            await _cartItemRepository.DeleteAsync(cartItem, cancellationToken);
        }
    }
}
