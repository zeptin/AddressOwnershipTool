namespace AddressOwnershipTool.Common;

public class NodeApiClientFactory : INodeApiClientFactory
{
    public INodeApiClient CreateNodeApiClient(string baseUrl)
    {
        return new NodeApiClient(baseUrl);
    }
}
