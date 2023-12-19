namespace AddressOwnershipTool.Common;

public interface IEthRpcClientFactory
{
    IEthRpcCleint Create(bool testnet);
}
