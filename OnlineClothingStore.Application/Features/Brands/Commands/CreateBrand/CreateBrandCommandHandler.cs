using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CreateBrandCommandHandler> _logger;

        public CreateBrandCommandHandler(
            IBrandRepository brandRepository,
            IMapper mapper,
            ICurrentUserService currentUserService,
            ILogger<CreateBrandCommandHandler> logger)
        {
            _brandRepository = brandRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BrandDTO> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Handling CreateBrandCommand for brand name: {BrandName} by User: {UserId}", request.Name, userId);

            var normalizedName = request.Name.Trim().ToLower();
            var existingBrand = await _brandRepository.GetByNameAsync(normalizedName, cancellationToken);

            if (existingBrand is not null)
            {
                _logger.LogWarning("Conflict: Brand with the same name '{BrandName}' already exists (ID: {BrandId})", normalizedName, existingBrand.Id);
                throw new Exceptions.ConflictException("Brand with same name already exists");
            }

            var brand = new Brand
            {
                Name = normalizedName,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var addedBrand = await _brandRepository.AddAsync(brand, cancellationToken);

            _logger.LogInformation("Brand created successfully with ID: {BrandId} by User: {UserId}", addedBrand.Id, userId);

            return _mapper.Map<BrandDTO>(addedBrand);
        }
    }
}
