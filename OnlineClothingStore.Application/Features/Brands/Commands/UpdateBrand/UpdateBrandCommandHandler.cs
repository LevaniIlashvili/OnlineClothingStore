using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;

namespace OnlineClothingStore.Application.Features.Brands.Commands.UpdateBrand
{
    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand>
    {
        private readonly IBrandRepository _brandRepository;

        public UpdateBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
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

            await _brandRepository.UpdateAsync(existingBrand, cancellationToken);
        }
    }
}
