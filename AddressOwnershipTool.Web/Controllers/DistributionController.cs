using AddressOwnershipTool.Commands.Load;
using AddressOwnershipTool.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AddressOwnershipTool.Web.Controllers;

[ApiController]
[Route("api/distribution")]
public class DistributionController : Controller
{
    private readonly IMediator _mediator;

    public DistributionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("load")]
    public async Task<IActionResult> LoadFiles([FromBody] LoadRequest request)
    {
        var response = await _mediator.Send(new LoadCommand { Path = request.Path });

        if (response.Failure)
        {
            return Ok(new { response.Message });
        }

        return Ok(new { result = response.Value });
    }
}
