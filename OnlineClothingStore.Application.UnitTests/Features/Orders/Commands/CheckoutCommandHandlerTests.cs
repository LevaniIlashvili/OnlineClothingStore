using AutoMapper;
using Moq;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Features.Orders.Commands.Checkout;
using OnlineClothingStore.Domain.Entities;
using OnlineClothingStore.Application.Exceptions;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.UnitTests.Features.Orders.Commands
{
    public class CheckoutCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Mock<ICartItemRepository> _cartItemRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;
        private readonly Mock<IProductVariantRepository> _productVariantRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IInventoryLogRepository> _inventoryLogRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CheckoutCommandHandler _handler;

        public CheckoutCommandHandlerTests()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _cartItemRepositoryMock = new Mock<ICartItemRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderItemRepositoryMock = new Mock<IOrderItemRepository>();
            _productVariantRepositoryMock = new Mock<IProductVariantRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _inventoryLogRepositoryMock = new Mock<IInventoryLogRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CheckoutCommandHandler(
                _cartRepositoryMock.Object,
                _cartItemRepositoryMock.Object,
                _orderRepositoryMock.Object,
                _orderItemRepositoryMock.Object,
                _productVariantRepositoryMock.Object,
                _productRepositoryMock.Object,
                _inventoryLogRepositoryMock.Object,
                _currentUserServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_EmptyCart_ThrowsBadRequestException()
        {
            // Arrange
            var command = new CheckoutCommand { ShippingAddress = "123 Street" };
            var cart = new Cart { Id = 1, Items = new List<CartItem>() };
            _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
            _cartRepositoryMock.Setup(x => x.GetByUserIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_InsufficientStock_ThrowsBadRequestException()
        {
            // Arrange
            var command = new CheckoutCommand { ShippingAddress = "123 Street" };

            var productVariant = new ProductVariant() { Id = 10, StockQuantity = 0, ProductId = 1 };
            
            var product = new Product() { Id = 100, Price = 20m };

            var cart = new Cart
            {
                Id = 1,
                Items = new List<CartItem> { new CartItem { ProductVariantId = 10, Quantity = 1 } }
            };

            _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
            _cartRepositoryMock.Setup(x => x.GetByUserIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
            _productVariantRepositoryMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync(new List<ProductVariant> { productVariant });
            _productRepositoryMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { product });

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(command, default));
        }

        [Fact]
        public async Task Handle_ValidCheckout_ReturnsOrderDTO()
        {
            // Arrange
            var command = new CheckoutCommand() { ShippingAddress = "address" };
            var variant = new ProductVariant { Id = 10, StockQuantity = 5, ProductId = 100, Product = new Product { Id = 100, Price = 25 } };
            var cartItem = new CartItem { ProductVariantId = 10, Quantity = 2, ProductVariant = variant };
            var cart = new Cart { Id = 1, Items = new List<CartItem> { cartItem } };
            var order = new Order { Id = 123, Items = new List<OrderItem>(), TotalAmount = 50 };
            var orderDTO = new OrderDTO { Id = 123, TotalAmount = 50 };

            _currentUserServiceMock.Setup(c => c.UserId).Returns(1);
            _cartRepositoryMock.Setup(r => r.GetByUserIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
            _productVariantRepositoryMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>()))
                                   .ReturnsAsync(new List<ProductVariant> { variant });
            _productRepositoryMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new List<Product> { variant.Product });
            _orderRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);
            _orderItemRepositoryMock.Setup(r => r.AddAsync(It.IsAny<OrderItem>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(new OrderItem());
            _productVariantRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<ProductVariant>(), It.IsAny<CancellationToken>()))
                                   .Returns(Task.CompletedTask);
            _inventoryLogRepositoryMock.Setup(r => r.AddAsync(It.IsAny<InventoryLog>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(new InventoryLog());
            _cartItemRepositoryMock.Setup(r => r.DeleteByCartIdAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<OrderDTO>(It.IsAny<Order>())).Returns(orderDTO);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(123, result.Id);
            Assert.Equal(50, result.TotalAmount);
        }
    }
}
