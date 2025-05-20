using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.DTOs;
using OnlineClothingStore.Models;

namespace OnlineClothingStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public static List<Category> Categories = new List<Category>
        {
            new Category { Id = 1, Name = "T-Shirts" },
            new Category { Id = 2, Name = "Jeans" },
            new Category { Id = 3, Name = "Hoodies & Sweatshirts" }
        };

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <response code="200">Categories retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]
        public ActionResult<List<Category>> GetCategories()
        {
            return Ok(Categories);
        }

        /// <summary>
        /// Gets a category by id
        /// </summary>
        /// <param name="id">The unique identifier of category</param>
        /// <response code="200">Category retrieved successfully</response>
        /// <response code="404">Category not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Category> GetCategory([FromRoute] int id)
        {
            var category = Categories.FirstOrDefault(c => c.Id == id);

            if (category is null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Adds a new category
        /// </summary>
        /// <param name="categoryDTO">The category data to add</param>
        /// <response code="201">Category created successfully</response>
        [HttpPost]
        [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
        public ActionResult<Category> AddCategory([FromBody] AddCategoryDTO categoryDTO)
        {
            var category = new Category
            {
                Id = Categories.Max(c => c.Id) + 1,
                Name = categoryDTO.Name
            };

            Categories.Add(category);

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        /// <summary>
        /// Updates an existing category
        /// </summary>
        /// <param name="id">The id of category to update</param>
        /// <param name="updatedCategory">The updated category data</param>
        /// <response code="204">Category updated successfully</response>
        /// <response code="404">Category not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateCategory([FromRoute] int id, [FromBody] AddCategoryDTO updatedCategory)
        {
            var category = Categories.FirstOrDefault(c => c.Id == id);

            if (category is null)
                return NotFound();

            category.Name = updatedCategory.Name;

            return NoContent();
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        /// <param name="id">The id of category to delete</param>
        /// <response code="204">Category deleted successfully</response>
        /// <response code="404">Category not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteCategory([FromRoute] int id)
        {
            var category = Categories.FirstOrDefault(c => c.Id == id);

            if (category is null)
                return NotFound();

            Categories.Remove(category);

            return NoContent();
        }
    }
}
