namespace AddressOwnershipTool.Common;

public class EthRpcClientFactory : IEthRpcClientFactory
{
    const string TestnetGethBaseUrl = "http://88.198.48.30:8545/";

    const string MainnetGethBaseUrl = "https://auroria.rpc.stratisevm.com/";

    public IEthRpcCleint Create(bool testnet)
    {
        return new GethRpcClient(testnet ? TestnetGethBaseUrl : MainnetGethBaseUrl);
    }
}
