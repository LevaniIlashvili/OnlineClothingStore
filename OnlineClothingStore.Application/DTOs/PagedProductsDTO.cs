namespace OnlineClothingStore.Application.DTOs
{
    public class PagedProductsDTO
    {
        public List<ProductDTO> Products { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
    }
}
