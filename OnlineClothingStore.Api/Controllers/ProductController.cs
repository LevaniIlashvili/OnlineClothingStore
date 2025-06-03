using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Application.Features.Products.Commands.CreateProduct;
using OnlineClothingStore.Application.Features.Products.Commands.DeleteProduct;
using OnlineClothingStore.Application.Features.Products.Commands.UpdateProduct;
using OnlineClothingStore.Application.Features.Products.Queries.GetProduct;
using OnlineClothingStore.Application.Features.Products.Queries.GetProducts;

namespace OnlineClothingStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a list of all products with their category names
        /// </summary>
        /// <response code="200">Returns the list of product DTOs</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedProductsDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedProductsDTO>> GetProducts(
            [FromQuery] GetProductsQuery request
        )
        {
            var pagedProducts = await _mediator.Send(request);

            return Ok(pagedProducts);
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
        public async Task<ActionResult<ProductDTO>> GetProduct([FromRoute] long id)
        {
            var query = new GetProductQuery() { Id = id };

            var product = await _mediator.Send(query);

            return Ok(product);
        }

        /// <summary>
        /// Adds a new product
        /// </summary>
        /// <param name="request">The data of the product to add</param>
        /// <response code="201">Product was created successfully</response>
        /// <response code="404">Category or brand not found</response>
        /// <response code="400">Validation failure</response>
        /// <response code="409">Product with this name already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ProductDTO>> AddProduct(
            [FromBody] CreateProductCommand request
        )
        {
            var product = await _mediator.Send(request);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        /// <summary>
        /// Updates a product by id
        /// </summary>
        /// <param name="id">The id of the product to update</param>
        /// <param name="request">The data of the updated product</param>
        /// <response code="204">Product was updated successfully</response>
        /// <response code="404">Product, category or brand not found</response>
        /// <response code="409">Product with same name or skuprefix already exists</response>
        /// <response code="400">Validation failure</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateProduct([FromRoute] long id, [FromBody] UpdateProductCommand request)
        {
            request.Id = id;
            await _mediator.Send(request);

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
        public async Task<ActionResult> DeleteProduct([FromRoute] long id)
        {
            var command = new DeleteProductCommand() { Id = id };
            await _mediator.Send(command);

            return NoContent();
        }

        ///// <summary>
        ///// Gets all variants for a specific product
        ///// </summary>
        ///// <param name="productId">The id of the product</param>
        ///// <response code="200">Returns list of product variants</response>
        ///// <response code="404">Product not found</response>
        //[HttpGet("{productId}/variants")]
        //[ProducesResponseType(typeof(List<ProductVariant>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public ActionResult<List<ProductVariant>> GetProductVariants([FromRoute] int productId)
        //{
        //    var product = Products.FirstOrDefault(p => p.Id == productId);
        //    if (product is null)
        //        return NotFound();

        //    var variants = ProductVariants.Where(pv => pv.ProductId == productId).ToList();
        //    return Ok(variants);
        //}

        ///// <summary>
        ///// Adds a new variant for a specific product
        ///// </summary>
        ///// <param name="productId">The id of the product</param>
        ///// <param name="addProductVariantDTO">The data of the variant to add</param>
        ///// <response code="201">Product variant was created successfully</response>
        ///// <response code="404">Product not found</response>
        //[HttpPost("{productId}/variants")]
        //[ProducesResponseType(typeof(ProductVariant), StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public ActionResult<ProductVariant> AddProductVariant([FromRoute] int productId, [FromBody] AddProductVariantDTO addProductVariantDTO)
        //{
        //    var product = Products.FirstOrDefault(p => p.Id == productId);

        //    if (product is null)
        //        return NotFound();

        //    var productVariant = new ProductVariant()
        //    {
        //        Id = ProductVariants.Max(pv => pv.Id) + 1,
        //        ProductId = productId,
        //        Size = addProductVariantDTO.Size,
        //        Color = addProductVariantDTO.Color,
        //        Stock = addProductVariantDTO.Stock
        //    };
        //    ProductVariants.Add(productVariant);

        //    return CreatedAtAction(nameof(GetProduct), new { id = productId }, productVariant);
        //}

        ///// <summary>
        ///// Updates a variant for a specific product
        ///// </summary>
        ///// <param name="productId">The id of the product</param>
        ///// <param name="variantId">The id of the variant</param>
        ///// <param name="updateDto">The updated variant data</param>
        ///// <response code="204">Product variant was updated successfully</response>
        ///// <response code="404">Product variant not found</response>
        //[HttpPut("{productId}/variants/{variantId}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public ActionResult UpdateProductVariant(
        //    [FromRoute] int productId,
        //    [FromRoute] int variantId,
        //    [FromBody] UpdateProductVariantDTO updateDto)
        //{
        //    var variant = ProductVariants
        //        .FirstOrDefault(pv => pv.Id == variantId && pv.ProductId == productId);

        //    if (variant is null)
        //        return NotFound();

        //    variant.Size = updateDto.Size;
        //    variant.Color = updateDto.Color;
        //    variant.Stock = updateDto.Stock;

        //    return NoContent();
        //}

        ///// <summary>
        ///// Deletes a variant of a specific product
        ///// </summary>
        ///// <param name="productId">The id of the product</param>
        ///// <param name="variantId">The id of the variant</param>
        ///// <response code="204">Product variant was deleted successfully</response>
        ///// <response code="404">Product variant not found</response>
        //[HttpDelete("{productId}/variants/{variantId}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public ActionResult DeleteProductVariant([FromRoute] int productId, [FromRoute] int variantId)
        //{
        //    var variant = ProductVariants
        //        .FirstOrDefault(pv => pv.Id == variantId && pv.ProductId == productId);

        //    if (variant is null)
        //        return NotFound();

        //    ProductVariants.Remove(variant);

        //    return NoContent();
        //}
    }
}
