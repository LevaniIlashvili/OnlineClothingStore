using MediatR;

namespace OnlineClothingStore.Application.Features.Products.Commands.DeleteProductVariant
{
    public class DeleteProductVariantCommand : IRequest
    {
        public long Id { get; set; }
    }
}
