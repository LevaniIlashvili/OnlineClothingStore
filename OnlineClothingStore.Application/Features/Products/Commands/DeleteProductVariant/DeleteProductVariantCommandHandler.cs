using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;

namespace OnlineClothingStore.Application.Features.Products.Commands.DeleteProductVariant
{
    public class DeleteProductVariantCommandHandler : IRequestHandler<DeleteProductVariantCommand>
    {
        private readonly IProductVariantRepository _productVariantRepository;

        public DeleteProductVariantCommandHandler(IProductVariantRepository productVariantRepository)
        {
            _productVariantRepository = productVariantRepository;
        }

        public async Task Handle(DeleteProductVariantCommand request, CancellationToken cancellationToken)
        {
            var productVariant = await _productVariantRepository.GetByIdAsync(request.Id, cancellationToken);

            if (productVariant is null)
            {
                throw new Exceptions.NotFoundException("Product variant not found");
            }

            await _productVariantRepository.DeleteAsync(productVariant, cancellationToken);
        }
    }
}
