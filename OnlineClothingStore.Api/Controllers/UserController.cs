using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.Application.Features.Users.Commands;
using OnlineClothingStore.Application.Features.Users.Queries;

namespace OnlineClothingStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="request">The data of user being created</param>
        /// <response code="200">User created successfully</response>
        /// <response code="409">User with this email already exists</response>
        /// <response code="400">Validation failure</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<long>> RegisterUser(CreateUserCommand request)
        {
            var userId = await _mediator.Send(request);

            return Ok(userId);
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUser(LoginUserQuery request)
        {
            var token = await _mediator.Send(request);

            return Ok(token);
        }
    }
}
