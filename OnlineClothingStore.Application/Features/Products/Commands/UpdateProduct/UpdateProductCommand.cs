using MediatR;

namespace OnlineClothingStore.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string SkuPrefix { get; set; }
        public long CategoryId { get; set; }
        public long BrandId { get; set; }
    }
}
