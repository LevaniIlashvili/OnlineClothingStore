using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;

namespace OnlineClothingStore.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingCategory is null)
            {
                throw new Exceptions.NotFoundException("Category with this id not found");
            }

            var normalizedName = request.Name.Trim().ToLower();

            var categoryWithThisName = await _categoryRepository.GetByNameAsync(normalizedName, cancellationToken);
            if (categoryWithThisName is not null && request.Id != categoryWithThisName.Id)
            {
                throw new Exceptions.ConflictException("Category with this name already exists");
            }

            if (request.ParentCategoryId is not null)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
                if (parentCategory is null)
                {
                    throw new Exceptions.NotFoundException("Parent category with this id not found");
                }
            }

            existingCategory.Name = normalizedName;
            existingCategory.ParentCategoryId = request.ParentCategoryId;
            existingCategory.LastUpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(existingCategory, cancellationToken);
        }
    }
}
