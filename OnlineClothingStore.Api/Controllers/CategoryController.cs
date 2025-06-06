using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Application.Features.Categories.Commands.CreateCategory;
using OnlineClothingStore.Application.Features.Categories.Commands.DeleteCategory;
using OnlineClothingStore.Application.Features.Categories.Commands.UpdateCategory;
using OnlineClothingStore.Application.Features.Categories.Queries.GetCategories;
using OnlineClothingStore.Application.Features.Categories.Queries.GetCategory;

namespace OnlineClothingStore.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(IMediator mediator, ICategoryRepository categoryRepository)
        {
            _mediator = mediator;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <response code="200">Categories retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryDTO>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<List<CategoryDTO>>> GetCategories()
        {
            var query = new GetCategoriesQuery();
            var categories = await _mediator.Send(query);
            return Ok(categories);
        }

        /// <summary>
        /// Gets a category by id
        /// </summary>
        /// <param name="id">The unique identifier of category</param>
        /// <response code="200">Category retrieved successfully</response>
        /// <response code="404">Category not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDTO>> GetCategory([FromRoute] long id)
        {
            var query = new GetCategoryQuery() { Id = id };
            var category = await _mediator.Send(query);

            return Ok(category);
        }

        /// <summary>
        /// Adds a new category
        /// </summary>
        /// <param name="request">The category data to add</param>
        /// <response code="201">Category created successfully</response>
        /// <response code="409">Category with same name already exists</response>
        /// <response code="404">Parent category not found</response>
        /// <response code="400">Validation failure</response>
        /// <response code="401">Authentication required</response>
        /// <response code="403">User not authorized</response>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CategoryDTO>> AddCategory([FromBody] CreateCategoryCommand request)
        {
            var category = await _mediator.Send(request);
            return CreatedAtAction(nameof(GetCategory), new { category.Id }, category);
        }

        /// <summary>
        /// Updates an existing category
        /// </summary>
        /// <param name="id">The id of category to update</param>
        /// <param name="request">The updated category data</param>
        /// <response code="204">Category updated successfully</response>
        /// <response code="404">Category or parent category not found</response>
        /// <response code="409">Category with different id and this name already exists</response>
        /// <response code="400">Validation failure</response>
        /// <response code="401">Authentication required</response>
        /// <response code="403">User not authorized</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateCategory([FromRoute] long id, [FromBody] UpdateCategoryCommand request)
        {
            request.Id = id;
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        /// <param name="id">The id of category to delete</param>
        /// <response code="204">Category deleted successfully</response>
        /// <response code="404">Category not found</response>
        /// <response code="409">Cannot delete category that has subcategories</response>
        /// <response code="401">Authentication required</response>
        /// <response code="403">User not authorized</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteCategory([FromRoute] long id)
        {
            var request = new DeleteCategoryCommand() { Id = id };
            await _mediator.Send(request);
            return NoContent();
        }
    }
}
