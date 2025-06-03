using MediatR;

namespace OnlineClothingStore.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest
    { 
        public long Id { get; set; }
    }
}
