using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Products.Queries.GetProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedProductsDTO>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PagedProductsDTO> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var result = await _productRepository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value, request.SortBy, request.SortDirection, cancellationToken);

            Console.WriteLine(request.PageSize.Value);

            var pagedProductsDTO = new PagedProductsDTO()
            {
                Products = _mapper.Map<List<ProductDTO>>(result.products),
                PageNumber = request.PageNumber.Value,
                PageSize = request.PageSize.Value,
                Count = result.count
            };

            return pagedProductsDTO;
        }
    }
}
