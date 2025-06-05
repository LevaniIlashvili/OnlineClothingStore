using AutoMapper;
using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Domain.Entities;

namespace OnlineClothingStore.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDTO>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDTO> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var normalizedName = request.Name.Trim().ToLower();
            var existingCategory = await _categoryRepository.GetByNameAsync(normalizedName, cancellationToken);

            if (existingCategory is not null)
            {
                throw new Exceptions.ConflictException("Category with same name already exists");
            }


            if (request.ParentCategoryId is not null)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);

                if (parentCategory is null)
                {
                    throw new Exceptions.NotFoundException("Parent category not found");
                }
            }

            var category = _mapper.Map<Category>(request);
            category.CreatedAt = DateTime.UtcNow;
            category.Name = normalizedName;
            var addedCategory = await _categoryRepository.AddAsync(category, cancellationToken);
            return _mapper.Map<CategoryDTO>(addedCategory);
        }
    }
}
