using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject.Core.Queries;
using System;
using System.Threading.Tasks;

namespace MyProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Test(TestQuery query)
        {
            Console.WriteLine("Testing");
            query.GetUserIdentity(this);
            var result = await _mediator.Send(query);
            return result.ToActionResult(this);
        }

    }
}
