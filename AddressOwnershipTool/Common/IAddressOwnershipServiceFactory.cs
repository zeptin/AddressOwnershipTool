namespace AddressOwnershipTool.Common
{
    public interface IAddressOwnershipServiceFactory
    {
        IAddressOwnershipService CreateAddressOwnershipService(bool testnet, bool loadFiles = true);
    }
}
