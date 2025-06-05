namespace OnlineClothingStore.Application.DTOs
{
    public class OrderDTO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long OrderStatusId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public ICollection<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }
}
