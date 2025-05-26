using OnlineClothingStore.Domain.Common;

namespace OnlineClothingStore.Domain.Entities
{
    public class ProductVariant : AuditableEntity
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Size { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }

        public Product Product { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<InventoryLog> InventoryLogs { get; set; } = new List<InventoryLog>();
    }
}
