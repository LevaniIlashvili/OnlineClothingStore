using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<ProductDTO>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string SkuPrefix { get; set; }
        public long CategoryId { get; set; }
        public long BrandId { get; set; }
    }
}
