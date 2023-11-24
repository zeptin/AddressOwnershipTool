namespace AddressOwnershipTool.Common
{
    public interface IBlockExplorerClient
    {
        bool HasActivity(string address);
        bool HasBalance(string address);
    }
}