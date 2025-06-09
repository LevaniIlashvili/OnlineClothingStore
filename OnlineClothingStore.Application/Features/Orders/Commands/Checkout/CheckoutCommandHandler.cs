using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Common;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.Orders.Commands.Checkout
{
    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, OrderDTO>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryLogRepository _inventoryLogRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<CheckoutCommandHandler> _logger;

        public CheckoutCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IProductVariantRepository productVariantRepository,
            IProductRepository productRepository,
            IInventoryLogRepository inventoryLogRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<CheckoutCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productVariantRepository = productVariantRepository;
            _productRepository = productRepository;
            _inventoryLogRepository = inventoryLogRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDTO> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Starting checkout process for user {UserId}", userId);

            var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
            if (cart is null)
            {
                _logger.LogWarning("Cart not found during checkout for user {UserId}", userId);
                throw new Exceptions.NotFoundException("Cart not found");
            }

            if (!cart.Items.Any())
            {
                _logger.LogWarning("Attempt to checkout with empty cart for user {UserId}", userId);
                throw new Exceptions.BadRequestException("Cart is empty");
            }

            await PopulateCartItemsWithProductVariantsAndProductsAsync(cart.Items, cancellationToken);

            var order = await CreateOrderAsync(cart.Items, userId, request.ShippingAddress, now, cancellationToken);

            await _cartItemRepository.DeleteByCartIdAsync(cart.Id, cancellationToken);

            _logger.LogInformation("Checkout completed successfully for user {UserId}, OrderId: {OrderId}", userId, order.Id);

            return _mapper.Map<OrderDTO>(order);
        }

        private async Task PopulateCartItemsWithProductVariantsAndProductsAsync(ICollection<CartItem> cartItems, CancellationToken cancellationToken)
        {
            var variantIds = cartItems.Select(i => i.ProductVariantId).Distinct();
            var variants = (await _productVariantRepository.GetByIdsAsync(variantIds, cancellationToken)).ToDictionary(v => v.Id);

            var productIds = variants.Values.Select(v => v.ProductId).Distinct();
            var products = (await _productRepository.GetByIdsAsync(productIds, cancellationToken)).ToDictionary(p => p.Id);

            foreach (var item in cartItems)
            {
                if (!variants.TryGetValue(item.ProductVariantId, out var variant))
                {
                    _logger.LogWarning("Product variant with Id {ProductVariantId} not found", item.ProductVariantId);
                    throw new Exceptions.NotFoundException($"Product variant with Id {item.ProductVariantId} not found");
                }

                if (variant.StockQuantity < item.Quantity)
                {
                    _logger.LogWarning("Insufficient stock for variant {ProductVariantId}", variant.Id);
                    throw new Exceptions.BadRequestException($"Insufficient stock for variant {variant.Id}");
                }

                item.ProductVariant = variant;

                if (!products.TryGetValue(variant.ProductId, out var product))
                {
                    _logger.LogWarning("Product with Id {ProductId} not found", variant.ProductId);
                    throw new Exceptions.NotFoundException($"Product with Id {variant.ProductId} not found");
                }

                item.ProductVariant.Product = product;
            }
        }

        private async Task<Order> CreateOrderAsync(
            ICollection<CartItem> cartItems,
            long userId,
            string shippingAddress,
            DateTime now,
            CancellationToken cancellationToken)
        {
            var orderTotalAmount = cartItems.Sum(i => i.ProductVariant.Product.Price * i.Quantity);

            var order = new Order()
            {
                UserId = userId,
                OrderStatusId = (long)OrderStatus.Processing,
                OrderDate = now,
                ShippingAddress = shippingAddress,
                TotalAmount = orderTotalAmount,
                CreatedAt = now
            };

            order = await _orderRepository.AddAsync(order, cancellationToken);
            _logger.LogInformation("Order created with Id {OrderId} for user {UserId}", order.Id, userId);

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem()
                {
                    OrderId = order.Id,
                    ProductVariantId = item.ProductVariantId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.ProductVariant.Product.Price,
                    CreatedAt = now
                };

                orderItem = await _orderItemRepository.AddAsync(orderItem, cancellationToken);
                _logger.LogInformation("OrderItem created with Id {OrderItemId} for OrderId {OrderId}", orderItem.Id, order.Id);

                await UpdateProductVariantStockAsync(item, now, cancellationToken);

                order.Items.Add(orderItem);
            }

            return order;
        }

        private async Task UpdateProductVariantStockAsync(CartItem item, DateTime now, CancellationToken cancellationToken)
        {
            var productVariant = item.ProductVariant;
            productVariant.StockQuantity -= item.Quantity;
            productVariant.LastUpdatedAt = now;
            await _productVariantRepository.UpdateAsync(productVariant, cancellationToken);

            var log = new InventoryLog()
            {
                ProductVariantId = productVariant.Id,
                ChangeTypeId = (long)InventoryLogChangeType.Sale,
                ChangeQuantity = -item.Quantity,
                NewStockQuantity = productVariant.StockQuantity,
                Reason = "Sale",
                CreatedAt = now
            };
            await _inventoryLogRepository.AddAsync(log, cancellationToken);

            _logger.LogInformation("Updated stock for ProductVariantId {ProductVariantId}: new stock {NewStockQuantity}", productVariant.Id, productVariant.StockQuantity);
        }
    }
}
