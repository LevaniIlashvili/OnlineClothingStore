using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Brands.Commands.UpdateBrand
{
    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateBrandCommandHandler> _logger;

        public UpdateBrandCommandHandler(
            IBrandRepository brandRepository,
            ICurrentUserService currentUserService,
            ILogger<UpdateBrandCommandHandler> logger)
        {
            _brandRepository = brandRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Handling UpdateBrandCommand for Brand ID: {BrandId} by User: {UserId}", request.Id, userId);

            var existingBrand = await _brandRepository.GetByIdAsync(request.Id, cancellationToken);
            if (existingBrand is null)
            {
                _logger.LogWarning("Brand not found with ID: {BrandId}", request.Id);
                throw new Exceptions.NotFoundException("Brand with this id not found");
            }

            var normalizedName = request.Name.Trim().ToLower();
            var brandWithThisName = await _brandRepository.GetByNameAsync(normalizedName, cancellationToken);

            if (brandWithThisName is not null && request.Id != brandWithThisName.Id)
            {
                _logger.LogWarning("Conflict: Another brand with name '{BrandName}' already exists (ID: {ConflictId})", normalizedName, brandWithThisName.Id);
                throw new Exceptions.ConflictException("Brand with this name already exists");
            }

            existingBrand.Name = normalizedName;
            existingBrand.LastUpdatedAt = DateTime.UtcNow;
            existingBrand.LastUpdatedBy = userId;

            await _brandRepository.UpdateAsync(existingBrand, cancellationToken);

            _logger.LogInformation("Brand with ID: {BrandId} updated successfully by User: {UserId}", request.Id, userId);
        }

    }
}
