using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Products.Queries.GetProductVariants
{
    public class GetProductVariantsQuery : IRequest<List<ProductVariantDTO>>
    {
        public long ProductId { get; set; }
    }
}
