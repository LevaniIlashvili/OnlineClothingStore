using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.Products.Commands.CreateProductVariant
{
    public class CreateProductVariantCommandHandler : IRequestHandler<CreateProductVariantCommand, ProductVariantDTO>
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateProductVariantCommandHandler> _logger;

        public CreateProductVariantCommandHandler(
            IProductVariantRepository productVariantRepository,
            IProductRepository productRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<CreateProductVariantCommandHandler> logger)
        {
            _productVariantRepository = productVariantRepository;
            _productRepository = productRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductVariantDTO> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Handling CreateProductVariantCommand for User: {UserId}, ProductId: {ProductId}, SKU: {Sku}",
                userId, request.ProductId, request.Sku);

            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product is null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", request.ProductId);
                throw new Exceptions.NotFoundException("Product not found");
            }

            var variantWithThisSku = await _productVariantRepository.GetBySkuAsync(request.Sku, cancellationToken);
            if (variantWithThisSku is not null)
            {
                _logger.LogWarning("Product variant with SKU: {Sku} already exists", request.Sku);
                throw new Exceptions.ConflictException("Product variant with this sku already exists");
            }

            var variantToAdd = _mapper.Map<ProductVariant>(request);
            variantToAdd.CreatedAt = DateTime.UtcNow;
            variantToAdd.CreatedBy = userId;

            var addedVariant = await _productVariantRepository.AddAsync(variantToAdd, cancellationToken);

            _logger.LogInformation("Product variant created successfully with ID: {VariantId} for ProductId: {ProductId} by User: {UserId}", addedVariant.Id, request.ProductId, userId);

            return _mapper.Map<ProductVariantDTO>(addedVariant);
        }
    }
}
