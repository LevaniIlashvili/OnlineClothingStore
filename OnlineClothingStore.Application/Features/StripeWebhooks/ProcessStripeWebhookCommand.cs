using MediatR;

namespace OnlineClothingStore.Application.Features.StripeWebhooks
{
    public class ProcessStripeWebhookCommand : IRequest
    {
        public string StripeEventJson { get; set; }
        public string StripeSignatureHeader { get; set; }
    }
}
