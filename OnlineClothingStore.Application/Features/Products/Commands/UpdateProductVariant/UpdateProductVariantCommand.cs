using MediatR;

namespace OnlineClothingStore.Application.Features.Products.Commands.UpdateProductVariant
{
    public class UpdateProductVariantCommand : IRequest
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Sku { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
    }
}
