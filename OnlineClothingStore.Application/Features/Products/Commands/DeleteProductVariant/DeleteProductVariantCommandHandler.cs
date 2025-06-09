using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Products.Commands.DeleteProductVariant
{
    public class DeleteProductVariantCommandHandler : IRequestHandler<DeleteProductVariantCommand>
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteProductVariantCommandHandler> _logger;

        public DeleteProductVariantCommandHandler(
            IProductVariantRepository productVariantRepository,
            ICurrentUserService currentUserService,
            ILogger<DeleteProductVariantCommandHandler> logger)
        {
            _productVariantRepository = productVariantRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Handle(DeleteProductVariantCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Handling DeleteProductVariantCommand for variant ID: {VariantId} by User: {UserId}", request.Id, userId);

            var productVariant = await _productVariantRepository.GetByIdAsync(request.Id, cancellationToken);

            if (productVariant is null)
            {
                _logger.LogWarning("Product variant not found with ID: {VariantId}", request.Id);
                throw new Exceptions.NotFoundException("Product variant not found");
            }

            await _productVariantRepository.DeleteAsync(productVariant, cancellationToken);

            _logger.LogInformation("Product variant with ID: {VariantId} deleted successfully by User: {UserId}", request.Id, userId);
        }
    }
}
