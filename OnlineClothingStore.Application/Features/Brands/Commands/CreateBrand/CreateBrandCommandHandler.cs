using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.Brands.Commands.CreateBrand
{
    public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, BrandDTO>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateBrandCommandHandler(
            IBrandRepository brandRepository,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _brandRepository = brandRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<BrandDTO> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var normalizedName = request.Name.Trim().ToLower();
            var existingBrand = await _brandRepository.GetByNameAsync(normalizedName, cancellationToken);

            if (existingBrand is not null)
            {
                throw new Exceptions.ConflictException("Brand with same name already exists");
            }

            var brand = new Brand();
            brand.Name = normalizedName;
            brand.CreatedAt = DateTime.UtcNow;
            brand.CreatedBy = userId; 

            var addedBrand = await _brandRepository.AddAsync(brand, cancellationToken);
            return _mapper.Map<BrandDTO>(addedBrand);
        }
    }
}
