namespace AddressOwnershipTool.Common;

public class AddressOwnershipServiceFactory : IAddressOwnershipServiceFactory
{
    private readonly INodeApiClientFactory _nodeApiClientFactory;
    private readonly IEthRpcClientFactory _ethRpcClientFactory;

    public AddressOwnershipServiceFactory(INodeApiClientFactory nodeApiClientFactory, IEthRpcClientFactory ethRpcClientFactory)
    {
        _nodeApiClientFactory = nodeApiClientFactory;
        _ethRpcClientFactory = ethRpcClientFactory;
    }

    public IAddressOwnershipService CreateAddressOwnershipService(bool testnet, bool useCirrus = false, bool loadFiles = true, string outputPath = null)
    {
        return new AddressOwnershipService(_nodeApiClientFactory, _ethRpcClientFactory, testnet, useCirrus, loadFiles, outputPath);
    }
}
