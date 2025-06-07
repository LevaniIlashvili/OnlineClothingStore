using AutoMapper;
using MediatR;
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

        public CheckoutCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IProductVariantRepository productVariantRepository,
            IProductRepository productRepository,
            IInventoryLogRepository inventoryLogRepository,
            ICurrentUserService currentUserService,
            IMapper mapper
        )
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
        }

        public async Task<OrderDTO> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var userId = _currentUserService.UserId;
            var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);

            if (cart is null)
                throw new Exceptions.NotFoundException("Cart not found");

            if (!cart.Items.Any())
                throw new Exceptions.BadRequestException("Cart is empty");

            await PopulateCartItemsWithProductVariantsAndProductsAsync(cart.Items, cancellationToken);

            var order = await CreateOrderAsync(cart.Items, userId, request.ShippingAddress, now, cancellationToken);

            await _cartItemRepository.DeleteByCartIdAsync(cart.Id, cancellationToken);

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
                    throw new Exceptions.NotFoundException($"Product variant with Id {item.ProductVariantId} not found");

                if (variant.StockQuantity < item.Quantity)
                    throw new Exceptions.BadRequestException($"Insufficient stock for variant {variant.Id}");

                item.ProductVariant = variant;

                if (!products.TryGetValue(variant.ProductId, out var product))
                    throw new Exceptions.NotFoundException($"Product with Id {variant.ProductId} not found");

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
        }
    }
}