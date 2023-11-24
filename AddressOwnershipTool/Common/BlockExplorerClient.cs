using Stratis.Bitcoin.Controllers.Models;

namespace AddressOwnershipTool.Common;

public class BlockExplorerClient : IBlockExplorerClient
{
    private readonly INodeApiClient _nodeApiClient;

    public BlockExplorerClient(INodeApiClient nodeApiClient)
    {
        _nodeApiClient = nodeApiClient;
    }

    public bool HasBalance(string address)
    {
        var balance = _nodeApiClient.GetAddressBalance(address);

        return balance > 0;
    }

    public bool HasActivity(string address)
    {
        List<AddressBalanceChange> changes = _nodeApiClient.GetVerboseAddressBalance(address);

        return changes.Any();
    }
}
