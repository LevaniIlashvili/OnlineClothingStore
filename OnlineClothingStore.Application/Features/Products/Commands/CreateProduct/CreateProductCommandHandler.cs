using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDTO>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(
            IProductRepository productRepository, 
            ICategoryRepository categoryRepository, 
            IBrandRepository brandRepository, 
            IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        public async Task<ProductDTO> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var normalizedName = request.Name.Trim().ToLower();
            var normalizedSkuPrefix = request.SkuPrefix.Trim().ToUpper();
            var productExists = await _productRepository.ProductExistsAsync(normalizedName, normalizedSkuPrefix, cancellationToken);

            if (productExists)
            {
                throw new Exceptions.ConflictException("Product with this name or sku prefix already exists");
            }

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

            if (category is null)
            {
                throw new Exceptions.NotFoundException("Category not found");
            }

            var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);

            if (brand is null)
            {
                throw new Exceptions.NotFoundException("Brand not found");
            }

            var productToAdd = _mapper.Map<Product>(request);
            productToAdd.CreatedAt = DateTime.UtcNow;

            var product = await _productRepository.AddAsync(productToAdd, cancellationToken);

            return _mapper.Map<ProductDTO>(product);
        }
    }
}
