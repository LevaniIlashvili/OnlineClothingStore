using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Brands.Commands.UpdateBrand
{
    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateBrandCommandHandler(IBrandRepository brandRepository, ICurrentUserService currentUserService)
        {
            _brandRepository = brandRepository;
            _currentUserService = currentUserService;
        }

        public async Task Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var existingBrand = await _brandRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingBrand is null)
            {
                throw new Exceptions.NotFoundException("Brand with this id not found");
            }

            var normalizedName = request.Name.Trim().ToLower();
            var brandWithThisName = await _brandRepository.GetByNameAsync(normalizedName,cancellationToken);

            if (brandWithThisName is not null && request.Id != brandWithThisName.Id)
            {
                throw new Exceptions.ConflictException("Brand with this name already exists");
            }

            existingBrand.Name = normalizedName;
            existingBrand.LastUpdatedAt = DateTime.UtcNow;
            existingBrand.LastUpdatedBy = userId;

            await _brandRepository.UpdateAsync(existingBrand, cancellationToken);
        }
    }
}
