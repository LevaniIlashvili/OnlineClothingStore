namespace OnlineClothingStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal OrderAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public OrderStatus Status { get; set; }

        public List<OrderItem> Items { get; set; }  
    }
}
