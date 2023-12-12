using AddressOwnershipTool.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AddressOwnershipTool.Web.Controllers;

[ApiController]
[Route("api/directory")]
public class DirectoryController : ControllerBase
{
    [Authorize]
    [HttpPost("exists")]
    public IActionResult CheckIfDirectoryExists([FromBody] DirectoryRequest request)
    {
        bool directoryExists = Directory.Exists(request.Path);
        return Ok(new { Exists = directoryExists });
    }
}
