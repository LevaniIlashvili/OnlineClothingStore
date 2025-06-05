using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;

namespace OnlineClothingStore.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (product is null)
            {
                throw new Exceptions.NotFoundException("Product not found");
            }

            await _productRepository.DeleteAsync(product, cancellationToken);
        }
    }
}
