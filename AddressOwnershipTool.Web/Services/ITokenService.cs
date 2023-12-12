namespace AddressOwnershipTool.Web.Services
{
    public interface ITokenService
    {
        string GenerateToken(string userId);
    }
}