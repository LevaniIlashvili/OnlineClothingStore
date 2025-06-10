using MediatR;
using OnlineClothingStore.Application.Contracts.Infrastructure;
using Stripe.Checkout;
using Stripe;
using Microsoft.Extensions.Configuration;
using OnlineClothingStore.Domain.Entities;
using OnlineClothingStore.Application.Features.Orders.Commands.Checkout;

namespace OnlineClothingStore.Application.Features.StripeWebhooks
{
    public class ProcessStripeWebhookCommandHandler : IRequestHandler<ProcessStripeWebhookCommand>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public ProcessStripeWebhookCommandHandler(
            IPaymentRepository paymentRepository,
            IConfiguration configuration,
            IMediator mediator)
        {
            _paymentRepository = paymentRepository;
            _configuration = configuration;
            _mediator = mediator;
        }

        public async Task Handle(ProcessStripeWebhookCommand request, CancellationToken cancellationToken)
        {
            var webhookSecret = _configuration["Stripe:WebhookSecret"];
            var stripeEvent = EventUtility.ConstructEvent(request.StripeEventJson, request.StripeSignatureHeader, webhookSecret);

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;
                var userId = long.Parse(session.ClientReferenceId);
                var paymentIntentId = session.PaymentIntentId;

                var order = await _mediator.Send(new CheckoutCommand() { ShippingAddress = "from metadata or hardcoded for now", UserId = userId  }, cancellationToken);

                var payment = new Payment()
                {
                    OrderId = order.Id,
                    Amount = order.TotalAmount,
                    CreatedAt = order.OrderDate,
                    PaymentMethod = session.PaymentMethodCollection,
                    TransactionId = paymentIntentId,
                    PaymentDate = order.OrderDate
                };

                await _paymentRepository.AddAsync(payment, cancellationToken);
            }
        }
    }
}
