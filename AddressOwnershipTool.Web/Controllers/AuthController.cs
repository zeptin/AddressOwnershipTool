using AddressOwnershipTool.Web.Models;
using AddressOwnershipTool.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Signer;

namespace AddressOwnershipTool.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly INonceService _nonceService;
    private readonly ITokenService _tokenService;

    public AuthController(INonceService nonceService, ITokenService tokenService)
    {
        _nonceService = nonceService;
        _tokenService = tokenService;
    }

    [HttpGet("nonce")]
    public IActionResult GetNonce([FromQuery] string account)
    {
        var nonce = _nonceService.GenerateNonce(account);
        return Ok(new { nonce });
    }

    [HttpPost("verify")]
    public IActionResult VerifySignature([FromBody] AuthRequest request)
    {
        if (!_nonceService.ValidateNonce(request.Nonce, request.Address))
        {
            return BadRequest("Invalid or expired nonce.");
        }

        string recoveredAddress = new EthereumMessageSigner().EncodeUTF8AndEcRecover(request.Nonce, request.SignedMessage);

        if (recoveredAddress.Equals(request.Address, StringComparison.InvariantCultureIgnoreCase))
        {
            _nonceService.MarkNonceAsUsed(request.Nonce);

            // Create JWT token
            var token = _tokenService.GenerateToken(request.Address);

            return Ok(new { Token = token });
        }

        return Unauthorized("Failed to authenticate user.");
    }
}
