using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject.Core.Queries;
using System.Threading.Tasks;

namespace MyProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LoginController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Handle(LoginQuery request)
        {
            request.GetUserIdentity(this);
            request.Ip = HttpContext.Connection.RemoteIpAddress;
            var result = await _mediator.Send(request);
            return result.ToActionResult(this);
        }
    }
}
