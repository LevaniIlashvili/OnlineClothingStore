using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(
            IProductRepository productRepository,
            ICurrentUserService currentUserService,
            ILogger<DeleteProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Handling DeleteProductCommand for product ID: {ProductId} by User: {UserId}", request.Id, userId);

            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (product is null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", request.Id);
                throw new Exceptions.NotFoundException("Product not found");
            }

            await _productRepository.DeleteAsync(product, cancellationToken);

            _logger.LogInformation("Product with ID: {ProductId} deleted successfully by User: {UserId}", request.Id, userId);
        }
    }
}
