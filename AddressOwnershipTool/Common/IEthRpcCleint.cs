namespace AddressOwnershipTool.Common;

public interface IEthRpcCleint
{
    Task SignAndSendTransaction(string destination);
}
