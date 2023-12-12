using Microsoft.Extensions.Caching.Memory;

namespace AddressOwnershipTool.Web.Services;

public class NonceService : INonceService
{
    private readonly IMemoryCache _cache;

    public NonceService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string GenerateNonce(string userId)
    {
        string nonce = Guid.NewGuid().ToString();
        _cache.Set(nonce, userId, TimeSpan.FromMinutes(10)); // Set nonce with 10 minutes expiry
        return nonce;
    }

    public bool ValidateNonce(string nonce, string userId)
    {
        if (_cache.TryGetValue(nonce, out string storedUserId))
        {
            return storedUserId == userId;
        }

        return false;
    }

    public void MarkNonceAsUsed(string nonce)
    {
        _cache.Remove(nonce);
    }
}
