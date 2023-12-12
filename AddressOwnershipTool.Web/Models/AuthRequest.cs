namespace AddressOwnershipTool.Web.Models;

public class AuthRequest
{
    public string Address { get; set; }
    public string SignedMessage { get; set; }
    public string Nonce { get; set; }
}
