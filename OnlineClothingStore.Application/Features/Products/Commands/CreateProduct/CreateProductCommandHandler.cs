using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDTO>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IBrandRepository brandRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<CreateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductDTO> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Handling CreateProductCommand for User: {UserId}, Product: {Name}, SKU prefix: {SkuPrefix}",
                userId, request.Name, request.SkuPrefix);

            var normalizedName = request.Name.Trim().ToLower();
            var normalizedSkuPrefix = request.SkuPrefix.Trim().ToUpper();
            var productExists = await _productRepository.ProductExistsAsync(normalizedName, normalizedSkuPrefix, cancellationToken);

            if (productExists)
            {
                _logger.LogWarning("Product already exists with Name: {Name}, SKU Prefix: {SkuPrefix}", normalizedName, normalizedSkuPrefix);
                throw new Exceptions.ConflictException("Product with this name or sku prefix already exists");
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

            var productToAdd = _mapper.Map<Product>(request);
            productToAdd.CreatedAt = DateTime.UtcNow;
            productToAdd.CreatedBy = userId;

            var product = await _productRepository.AddAsync(productToAdd, cancellationToken);

            _logger.LogInformation("Product created successfully with ID: {ProductId} by User: {UserId}", product.Id, userId);

            return _mapper.Map<ProductDTO>(product);
        }
    }
}
