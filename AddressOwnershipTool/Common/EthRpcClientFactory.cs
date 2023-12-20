namespace AddressOwnershipTool.Common;

public class EthRpcClientFactory : IEthRpcClientFactory
{
    const string TestnetGethBaseUrl = "http://88.198.48.30:8545/";

    const string MainnetGethBaseUrl = "http://144.126.200.138:8545/";

    public IEthRpcCleint Create(bool testnet)
    {
        return new GethRpcClient(testnet ? TestnetGethBaseUrl : MainnetGethBaseUrl);
    }
}
