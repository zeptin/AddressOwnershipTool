
namespace AddressOwnershipTool.Common
{
    public interface ISwapExtractionService
    {
        Task RunAsync(int startBlock);
    }
}