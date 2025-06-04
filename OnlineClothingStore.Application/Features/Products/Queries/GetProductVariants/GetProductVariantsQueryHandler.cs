using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Products.Queries.GetProductVariants
{
    public class GetProductVariantsQueryHandler : IRequestHandler<GetProductVariantsQuery, List<ProductVariantDTO>>
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductVariantsQueryHandler(
            IProductVariantRepository productVariantRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _productVariantRepository = productVariantRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductVariantDTO>> Handle(GetProductVariantsQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

            if (product is null)
            {
                throw new Exceptions.NotFoundException("Product not found");
            }

            var productVariants = await _productVariantRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
            return _mapper.Map<List<ProductVariantDTO>>(productVariants);
        }
    }
}
