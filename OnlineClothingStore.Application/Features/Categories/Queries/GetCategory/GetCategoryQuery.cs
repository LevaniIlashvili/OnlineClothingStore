using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Categories.Queries.GetCategory
{
    public class GetCategoryQuery : IRequest<CategoryDTO>
    {
        public long Id { get; set; }
    }
}
