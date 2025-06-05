using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Brands.Queries.GetBrand
{
    public class GetBrandQueryHandler : IRequestHandler<GetBrandQuery, BrandDTO>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public GetBrandQueryHandler(IBrandRepository brandRepository, IMapper mapper)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        public async Task<BrandDTO> Handle(GetBrandQuery request, CancellationToken cancellationToken)
        {
            var brand = await _brandRepository.GetByIdAsync(request.Id, cancellationToken);

            if (brand is null)
            {
                throw new Exceptions.NotFoundException("Brand not found");
            }

            return _mapper.Map<BrandDTO>(brand);
        }
    }
}
