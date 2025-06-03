using MediatR;
using OnlineClothingStore.Application.DTOs;

namespace OnlineClothingStore.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommand : IRequest<CategoryDTO>
    {
        public string Name { get; set; } = null!;
        public long? ParentCategoryId { get; set; }
    }
}
