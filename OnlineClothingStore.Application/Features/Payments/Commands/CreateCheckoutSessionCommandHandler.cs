using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Domain.Entities;
using Stripe.Checkout;

namespace OnlineClothingStore.Application.Features.Payments.Commands
{
    public class CreateCheckoutSessionCommandHandler : IRequestHandler<CreateCheckoutSessionCommand, string>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CreateCheckoutSessionCommandHandler(
            ICurrentUserService currentUserService,
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _currentUserService = currentUserService;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<string> Handle(CreateCheckoutSessionCommand command, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);

            if (cart is null)
            {
                throw new Exceptions.NotFoundException("Cart not found");
            }

            foreach (var item in cart.Items)
            {
                var product = await _productRepository.GetByVariantIdAsync(item.ProductVariantId, cancellationToken);
                if (product is null)
                {
                    throw new Exceptions.NotFoundException("Product not found");
                }
                item.ProductVariant = new ProductVariant
                {
                    Product = product
                };
            }

            var lineItems = cart.Items.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.ProductVariant.Product.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.ProductVariant.Product.Name
                    },
                },
                Quantity = item.Quantity,
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:5001/success",
                CancelUrl = "https://localhost:5001/cancel",
                ClientReferenceId = userId.ToString()
            };

            var service = new SessionService();
            var session = service.Create(options);

            return session.Url;
        }
    }
}
