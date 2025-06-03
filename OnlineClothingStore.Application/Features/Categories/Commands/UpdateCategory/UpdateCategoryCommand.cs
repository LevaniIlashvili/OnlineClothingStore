using MediatR;

namespace OnlineClothingStore.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommand : IRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentCategoryId { get; set; }
    }
}
