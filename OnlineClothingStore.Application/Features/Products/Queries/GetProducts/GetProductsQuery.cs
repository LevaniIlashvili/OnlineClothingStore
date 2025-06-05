using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Products.Queries.GetProducts
{
    public class GetProductsQuery : IRequest<PagedProductsDTO>
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "CreatedAt";
        public string? SortDirection { get; set; } = "ASC";
    }
}
