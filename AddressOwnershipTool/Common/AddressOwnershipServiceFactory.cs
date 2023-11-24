namespace AddressOwnershipTool.Common;

public class AddressOwnershipServiceFactory : IAddressOwnershipServiceFactory
{
    private readonly INodeApiClientFactory _nodeApiClientFactory;

    public AddressOwnershipServiceFactory(INodeApiClientFactory nodeApiClientFactory)
    {
        _nodeApiClientFactory = nodeApiClientFactory;
    }

    public IAddressOwnershipService CreateAddressOwnershipService(bool testnet, bool loadFiles = true)
    {
        return new AddressOwnershipService(_nodeApiClientFactory, testnet, loadFiles);
    }
}
