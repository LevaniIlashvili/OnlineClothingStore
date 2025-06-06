using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.Contracts.Infrastructure.Authentication;

namespace OnlineClothingStore.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, ICurrentUserService currentUserService)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

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
            existingCategory.LastUpdatedBy = userId;

            await _categoryRepository.UpdateAsync(existingCategory, cancellationToken);
        }
    }
}
