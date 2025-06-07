using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;

namespace OnlineClothingStore.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingCategory is null)
            {
                throw new Exceptions.NotFoundException("Category with this id doesn't exist");
            }

            var hasChildren = await _categoryRepository.HasChildrenAsync(request.Id,  cancellationToken);

            if (hasChildren)
            {
                throw new Exceptions.ConflictException("Cannot delete category because it has subcategories");
            }

            var hasProducts = await _categoryRepository.HasProductsAsync(existingCategory.Id, cancellationToken);
            if (hasProducts)
            {
                throw new Exceptions.ConflictException("Cannot delete category because it has active products");
            }


            await _categoryRepository.DeleteAsync(existingCategory, cancellationToken);
        }
    }
}
