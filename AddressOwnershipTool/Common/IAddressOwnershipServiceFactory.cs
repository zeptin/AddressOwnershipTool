namespace AddressOwnershipTool.Common
{
    public interface IAddressOwnershipServiceFactory
    {
        IAddressOwnershipService CreateAddressOwnershipService(bool testnet, bool useCirrus = false, bool loadFiles = true, string outputPath = null);
    }
}
