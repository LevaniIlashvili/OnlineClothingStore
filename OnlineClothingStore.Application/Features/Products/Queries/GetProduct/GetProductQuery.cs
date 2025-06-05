using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Products.Queries.GetProduct
{
    public class GetProductQuery : IRequest<ProductDTO>
    {
        public long Id { get; set; }
    }
}
