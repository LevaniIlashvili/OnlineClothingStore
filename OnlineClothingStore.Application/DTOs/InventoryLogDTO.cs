namespace OnlineClothingStore.Application.DTOs
{
    public class InventoryLogDTO
    {
        public long Id { get; set; }
        public string ProductVariantSku { get; set; }
        public string ChangeType { get; set; }
        public int ChangeQuantity { get; set; }
        public int NewStockQuantity { get; set; }
        public string? Reason { get; set; }
    }
}
