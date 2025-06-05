using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.Application.DTOs;
using OnlineClothingStore.Application.Features.Brands.Commands.CreateBrand;
using OnlineClothingStore.Application.Features.Brands.Commands.UpdateBrand;
using OnlineClothingStore.Application.Features.Brands.Commands.DeleteBrand;
using OnlineClothingStore.Application.Features.Brands.Queries.GetBrand;
using OnlineClothingStore.Application.Features.Brands.Queries.GetBrands;

namespace OnlineClothingStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BrandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets all brands
        /// </summary>
        /// <response code="200">Brands retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<BrandDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BrandDTO>>> GetBrands()
        {
            var query = new GetBrandsQuery();
            var brands = await _mediator.Send(query);
            return Ok(brands);
        }

        /// <summary>
        /// Gets a brand by id
        /// </summary>
        /// <param name="id">The brand id</param>
        /// <response code="200">Brand retrieved successfully</response>
        /// <response code="404">Brand not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BrandDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BrandDTO>> GetBrand([FromRoute] long id)
        {
            var query = new GetBrandQuery { Id = id };
            var brand = await _mediator.Send(query);
            return Ok(brand);
        }

        /// <summary>
        /// Creates a new brand
        /// </summary>
        /// <param name="request">The brand to create</param>
        /// <response code="201">Brand created successfully</response>
        /// <response code="409">Brand with the same name already exists</response>
        /// <response code="400">Validation failure</response>
        [HttpPost]
        [ProducesResponseType(typeof(BrandDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BrandDTO>> CreateBrand([FromBody] CreateBrandCommand request)
        {
            var brand = await _mediator.Send(request);
            return CreatedAtAction(nameof(GetBrand), new { brand.Id }, brand);
        }

        /// <summary>
        /// Updates a brand
        /// </summary>
        /// <param name="id">Brand ID</param>
        /// <param name="request">Updated brand info</param>
        /// <response code="204">Brand updated successfully</response>
        /// <response code="404">Brand not found</response>
        /// <response code="409">Brand with different id and same name already exists</response>
        /// <response code="400">Validation failure</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateBrand([FromRoute] long id, [FromBody] UpdateBrandCommand request)
        {
            request.Id = id;
            await _mediator.Send(request);
            return NoContent();
        }

        /// <summary>
        /// Deletes a brand
        /// </summary>
        /// <param name="id">The brand id</param>
        /// <response code="204">Brand deleted successfully</response>
        /// <response code="404">Brand not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBrand([FromRoute] long id)
        {
            var request = new DeleteBrandCommand { Id = id };
            await _mediator.Send(request);
            return NoContent();
        }
    }
}
