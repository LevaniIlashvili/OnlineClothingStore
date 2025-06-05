using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Products.Commands.CreateProductVariant
{
    public class CreateProductVariantCommand : IRequest<ProductVariantDTO>
    {
        public long ProductId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Sku { get; set; }
        public string? ImageUrl { get; set; }
    }
}
