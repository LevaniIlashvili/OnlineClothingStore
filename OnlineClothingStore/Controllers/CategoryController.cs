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
        [HttpGet]
        public ActionResult<List<Category>> GetCategories()
        {
            return Ok(Categories);
        }

        /// <summary>
        /// Gets a category by id
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<Category> GetCategory([FromRoute] int id)
        {
            var category = Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Adds a new category
        /// </summary>
        [HttpPost]
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
        [HttpPut("{id}")]
        public ActionResult UpdateCategory([FromRoute] int id, [FromBody] AddCategoryDTO updatedCategory)
        {
            var category = Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound();

            category.Name = updatedCategory.Name;

            return NoContent();
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        [HttpDelete("{id}")]
        public ActionResult DeleteCategory([FromRoute] int id)
        {
            var category = Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound();

            Categories.Remove(category);

            return NoContent();
        }
    }
}
