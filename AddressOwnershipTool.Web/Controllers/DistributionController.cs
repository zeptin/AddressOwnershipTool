﻿using AddressOwnershipTool.Commands.Load;
using AddressOwnershipTool.Commands.Update;
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
        var response = await _mediator.Send(new LoadCommand { Path = request.Path, Limit = request.Limit });

        if (response.Failure)
        {
            return Ok(new { response.Message });
        }

        return Ok(new { result = response.Value });
    }

    [Authorize]
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] SwapRequest request)
    {
        var response = await _mediator.Send(new UpdateCommand
        {
            Path = request.Path,
            Amount = request.Amount,
            Destination = request.Destination,
            Origin = request.Origin,
            TxHash = request.TxHash,
            Type = request.Type
        });

        return Ok(new { Result = response.Success });
    }
}
