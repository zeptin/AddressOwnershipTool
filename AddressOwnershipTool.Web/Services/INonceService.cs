namespace AddressOwnershipTool.Web.Services
{
    public interface INonceService
    {
        string GenerateNonce(string userId);
        void MarkNonceAsUsed(string nonce);
        bool ValidateNonce(string nonce, string userId);
    }
}