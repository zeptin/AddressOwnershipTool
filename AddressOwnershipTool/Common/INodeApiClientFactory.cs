namespace AddressOwnershipTool.Common;

public interface INodeApiClientFactory
{
    INodeApiClient CreateNodeApiClient(string baseUrl);
}
