using MediatR;

namespace OnlineClothingStore.Application.Features.Payments.Commands
{
    public class CreateCheckoutSessionCommand : IRequest<string>
    {
    }
}
