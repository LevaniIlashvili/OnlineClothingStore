using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;

namespace OnlineClothingStore.Application.Features.Brands.Commands.DeleteBrand
{
    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand>
    {
        private readonly IBrandRepository _brandRepository;

        public DeleteBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var existingBrand = await _brandRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingBrand is null)
            {
                throw new Exceptions.NotFoundException("Brand with this id doesn't exist");
            }

            var hasProducts = await _brandRepository.HasProductsAsync(existingBrand.Id, cancellationToken);

            if (hasProducts)
            {
                throw new Exceptions.ConflictException("Cannot delete brand because it has active products");
            }

            await _brandRepository.DeleteAsync(existingBrand, cancellationToken);
        }
    }
}
