using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;

namespace OnlineClothingStore.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler(
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

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateProductCommandValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Count > 0)
            {
                throw new FluentValidation.ValidationException("aaaaaaaa");
            }

            var existingProduct = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingProduct is null)
            {
                throw new Exceptions.NotFoundException("Product not found");
            }

            if (existingProduct.Name != request.Name)
            {
                var otherProductWithSameName = await _productRepository.GetByNameAsync(request.Name,  cancellationToken);
                if (otherProductWithSameName is not null && otherProductWithSameName.Id != request.Id)
                {
                    throw new Exceptions.ConflictException("Another product with the same name already exists.");
                }
            }

            if (existingProduct.SkuPrefix != request.SkuPrefix)
            {
                var otherProductWithSameSkuPrefix = await _productRepository.GetBySkuPrefixAsync(request.SkuPrefix, cancellationToken);
                if (otherProductWithSameSkuPrefix is not null && otherProductWithSameSkuPrefix.Id != request.Id)
                {
                    throw new Exceptions.ConflictException("Another product with the same sku prefix already exists.");
                }
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

            var updatedProduct = _mapper.Map(request, existingProduct);
            updatedProduct.LastUpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(updatedProduct, cancellationToken);
        }
    }
}
