using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.Application.Features.Payments.Commands;

namespace OnlineClothingStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new Stripe checkout session and returns the session URL.
        /// </summary>
        /// <response code="200">Url retrieved succesfully</response>
        /// <response code="404">Cart or one of the products in cart not found</response>
        [HttpPost("create-checkout-session")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateCheckoutSession()
        {
            var command = new CreateCheckoutSessionCommand();
            var url = await _mediator.Send(command);
            return Ok(new { url });
        }
    }
}