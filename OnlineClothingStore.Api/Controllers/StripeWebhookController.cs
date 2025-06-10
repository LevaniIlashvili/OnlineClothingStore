using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.Application.Features.StripeWebhooks;

[ApiController]
[Route("api/stripe-webhook")]
public class StripeWebhookController : ControllerBase
{
    private readonly IMediator _mediator;

    public StripeWebhookController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Handles Stripe webhook events by verifying the signature and processing the event
    /// </summary>
    /// <response code="200">The event was processed successfully.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> HandleWebhook()
    {
        using var reader = new StreamReader(HttpContext.Request.Body);
        var json = await reader.ReadToEndAsync();

        var stripeSignature = Request.Headers["Stripe-Signature"];

        var command = new ProcessStripeWebhookCommand
        {
            StripeEventJson = json,
            StripeSignatureHeader = stripeSignature
        };

        await _mediator.Send(command);

        return Ok();
    }
}
