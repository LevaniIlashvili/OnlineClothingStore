using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IBrandRepository brandRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<UpdateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Handling UpdateProductCommand for product ID: {ProductId} by User: {UserId}", request.Id, userId);

            var existingProduct = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingProduct is null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", request.Id);
                throw new Exceptions.NotFoundException("Product not found");
            }

            if (existingProduct.Name != request.Name)
            {
                var otherProductWithSameName = await _productRepository.GetByNameAsync(request.Name, cancellationToken);
                if (otherProductWithSameName is not null && otherProductWithSameName.Id != request.Id)
                {
                    _logger.LogWarning("Conflict: Another product with the same name '{Name}' already exists (ID: {OtherProductId})", request.Name, otherProductWithSameName.Id);
                    throw new Exceptions.ConflictException("Another product with the same name already exists.");
                }
            }

            if (existingProduct.SkuPrefix != request.SkuPrefix)
            {
                var otherProductWithSameSkuPrefix = await _productRepository.GetBySkuPrefixAsync(request.SkuPrefix, cancellationToken);
                if (otherProductWithSameSkuPrefix is not null && otherProductWithSameSkuPrefix.Id != request.Id)
                {
                    _logger.LogWarning("Conflict: Another product with the same sku prefix '{SkuPrefix}' already exists (ID: {OtherProductId})", request.SkuPrefix, otherProductWithSameSkuPrefix.Id);
                    throw new Exceptions.ConflictException("Another product with the same sku prefix already exists.");
                }
            }

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

            if (category is null)
            {
                _logger.LogWarning("Category not found with ID: {CategoryId}", request.CategoryId);
                throw new Exceptions.NotFoundException("Category not found");
            }

            var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);

            if (brand is null)
            {
                _logger.LogWarning("Brand not found with ID: {BrandId}", request.BrandId);
                throw new Exceptions.NotFoundException("Brand not found");
            }

            var updatedProduct = _mapper.Map(request, existingProduct);
            updatedProduct.LastUpdatedAt = DateTime.UtcNow;
            updatedProduct.LastUpdatedBy = userId;

            await _productRepository.UpdateAsync(updatedProduct, cancellationToken);

            _logger.LogInformation("Product with ID: {ProductId} updated successfully by User: {UserId}", request.Id, userId);
        }
    }
}
