using MediatR;
using Microsoft.Extensions.Logging;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Brands.Commands.DeleteBrand
{
    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteBrandCommandHandler> _logger;

        public DeleteBrandCommandHandler(
            IBrandRepository brandRepository,
            ICurrentUserService currentUserService,
            ILogger<DeleteBrandCommandHandler> logger)
        {
            _brandRepository = brandRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("Handling DeleteBrandCommand for Brand ID: {BrandId} by User: {UserId}", request.Id, userId);

            var existingBrand = await _brandRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingBrand is null)
            {
                _logger.LogWarning("Brand not found with ID: {BrandId}", request.Id);
                throw new Exceptions.NotFoundException("Brand with this id doesn't exist");
            }

            var hasProducts = await _brandRepository.HasProductsAsync(existingBrand.Id, cancellationToken);

            if (hasProducts)
            {
                _logger.LogWarning("Cannot delete Brand ID: {BrandId} because it has active products", existingBrand.Id);
                throw new Exceptions.ConflictException("Cannot delete brand because it has active products");
            }

            await _brandRepository.DeleteAsync(existingBrand, cancellationToken);

            _logger.LogInformation("Brand with ID: {BrandId} deleted successfully by User: {UserId}", existingBrand.Id, userId);
        }

    }
}
