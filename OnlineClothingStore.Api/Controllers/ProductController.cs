using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.DTOs;
using OnlineClothingStore.Models;

namespace OnlineClothingStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public static List<Product> Products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Classic T-Shirt",
                Description = "100% cotton, breathable fabric.",
                CategoryId = 1,
                Price = 19.99m
            },
            new Product
            {
                Id = 2,
                Name = "Slim Fit Jeans",
                Description = "Denim jeans with a slim fit cut.",
                CategoryId = 2,
                Price = 49.99m
            },
            new Product
            {
                Id = 3,
                Name = "Hooded Sweatshirt",
                Description = "Cozy hoodie, great for winter.",
                CategoryId = 3,
                Price = 35.00m
            },
        };

        public static List<ProductVariant> ProductVariants = new List<ProductVariant>
        {
            // Classic T-Shirt Variants
            new ProductVariant { Id = 1, ProductId = 1, Size = "S", Color = "White", Stock = 20 },
            new ProductVariant { Id = 2, ProductId = 1, Size = "M", Color = "Black", Stock = 15 },
            new ProductVariant { Id = 3, ProductId = 1, Size = "L", Color = "Blue", Stock = 10 },

            // Slim Fit Jeans Variants
            new ProductVariant { Id = 4, ProductId = 2, Size = "30", Color = "Dark Blue", Stock = 25 },
            new ProductVariant { Id = 5, ProductId = 2, Size = "32", Color = "Light Blue", Stock = 18 },
            new ProductVariant { Id = 6, ProductId = 2, Size = "34", Color = "Black", Stock = 12 },

            // Hooded Sweatshirt Variants
            new ProductVariant { Id = 7, ProductId = 3, Size = "M", Color = "Gray", Stock = 22 },
            new ProductVariant { Id = 8, ProductId = 3, Size = "L", Color = "Red", Stock = 8 },
            new ProductVariant { Id = 9, ProductId = 3, Size = "XL", Color = "Navy", Stock = 5 }
        };

        /// <summary>
        /// Gets a list of all products with their category names
        /// </summary>
        /// <response code="200">Returns the list of product DTOs</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ProductDTO>), StatusCodes.Status200OK)]
        public ActionResult<List<ProductDTO>> GetProducts()
        {
            var producDTOs = Products.Select(p =>
            {
                var category = CategoryController.Categories.FirstOrDefault(c => c.Id == p.CategoryId);

                return new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = category.Name
                };
            }).ToList();

            return Ok(producDTOs);
        }

        /// <summary>
        /// Gets a product by id
        /// </summary>
        /// <param name="id">The id of the product to get</param>
        /// <response code="200">Returns the requested product</response>
        /// <response code="404">Product not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProductDTO> GetProduct([FromRoute] int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);

            if (product is null)
                return NotFound();

            var category = CategoryController.Categories.FirstOrDefault(c => c.Id == product.CategoryId);

            var productDTO = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = category.Name,
            };

            return Ok(productDTO);
        }

        /// <summary>
        /// Adds a new product
        /// </summary>
        /// <param name="productDTO">The data of the product to add</param>
        /// <response code="201">Product was created successfully</response>
        /// <response code="404">Category not found</response>
        [HttpPost]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Product> AddProduct([FromBody] AddProductDTO productDTO)
        {
            var category = CategoryController.Categories.FirstOrDefault(c => c.Id == productDTO.CategoryId);

            if (category is null)
                return NotFound();

            var product = new Product
            {
                Id = Products.Max(p => p.Id) + 1,
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                CategoryId = productDTO.CategoryId
            };

            Products.Add(product);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        /// <summary>
        /// Updates a product by id
        /// </summary>
        /// <param name="id">The id of the product to update</param>
        /// <param name="updatedProduct">The data of the updated product</param>
        /// <response code="204">Product was updated successfully</response>
        /// <response code="404">Product or category not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateProduct([FromRoute] int id, [FromBody] AddProductDTO updatedProduct)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);

            if (product is null)
                return NotFound();

            var category = CategoryController.Categories.FirstOrDefault(c => c.Id == updatedProduct.CategoryId);

            if (category is null)
                return NotFound();

            product.Name = updatedProduct.Name;
            product.CategoryId = updatedProduct.CategoryId;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;

            return NoContent();
        }

        /// <summary>
        /// Deletes a product by id
        /// </summary>
        /// <param name="id">The id of the product to delete</param>
        /// <response code="204">Product was deleted successfully</response>
        /// <response code="404">Product not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteProduct([FromRoute] int id)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);

            if (product is null) return NotFound();

            Products.Remove(product);

            return NoContent();
        }

        /// <summary>
        /// Gets all variants for a specific product
        /// </summary>
        /// <param name="productId">The id of the product</param>
        /// <response code="200">Returns list of product variants</response>
        /// <response code="404">Product not found</response>
        [HttpGet("{productId}/variants")]
        [ProducesResponseType(typeof(List<ProductVariant>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ProductVariant>> GetProductVariants([FromRoute] int productId)
        {
            var product = Products.FirstOrDefault(p => p.Id == productId);
            if (product is null)
                return NotFound();

            var variants = ProductVariants.Where(pv => pv.ProductId == productId).ToList();
            return Ok(variants);
        }

        /// <summary>
        /// Adds a new variant for a specific product
        /// </summary>
        /// <param name="productId">The id of the product</param>
        /// <param name="addProductVariantDTO">The data of the variant to add</param>
        /// <response code="201">Product variant was created successfully</response>
        /// <response code="404">Product not found</response>
        [HttpPost("{productId}/variants")]
        [ProducesResponseType(typeof(ProductVariant), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProductVariant> AddProductVariant([FromRoute] int productId, [FromBody] AddProductVariantDTO addProductVariantDTO)
        {
            var product = Products.FirstOrDefault(p => p.Id == productId);

            if (product is null)
                return NotFound();

            var productVariant = new ProductVariant()
            {
                Id = ProductVariants.Max(pv => pv.Id) + 1,
                ProductId = productId,
                Size = addProductVariantDTO.Size,
                Color = addProductVariantDTO.Color,
                Stock = addProductVariantDTO.Stock
            };
            ProductVariants.Add(productVariant);

            return CreatedAtAction(nameof(GetProduct), new { id = productId }, productVariant);
        }

        /// <summary>
        /// Updates a variant for a specific product
        /// </summary>
        /// <param name="productId">The id of the product</param>
        /// <param name="variantId">The id of the variant</param>
        /// <param name="updateDto">The updated variant data</param>
        /// <response code="204">Product variant was updated successfully</response>
        /// <response code="404">Product variant not found</response>
        [HttpPut("{productId}/variants/{variantId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateProductVariant(
            [FromRoute] int productId,
            [FromRoute] int variantId,
            [FromBody] UpdateProductVariantDTO updateDto)
        {
            var variant = ProductVariants
                .FirstOrDefault(pv => pv.Id == variantId && pv.ProductId == productId);

            if (variant is null)
                return NotFound();

            variant.Size = updateDto.Size;
            variant.Color = updateDto.Color;
            variant.Stock = updateDto.Stock;

            return NoContent();
        }

        /// <summary>
        /// Deletes a variant of a specific product
        /// </summary>
        /// <param name="productId">The id of the product</param>
        /// <param name="variantId">The id of the variant</param>
        /// <response code="204">Product variant was deleted successfully</response>
        /// <response code="404">Product variant not found</response>
        [HttpDelete("{productId}/variants/{variantId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteProductVariant([FromRoute] int productId, [FromRoute] int variantId)
        {
            var variant = ProductVariants
                .FirstOrDefault(pv => pv.Id == variantId && pv.ProductId == productId);

            if (variant is null)
                return NotFound();

            ProductVariants.Remove(variant);

            return NoContent();
        }
    }
}
