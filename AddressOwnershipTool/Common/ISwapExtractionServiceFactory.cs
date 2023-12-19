namespace AddressOwnershipTool.Common
{
    public interface ISwapExtractionServiceFactory
    {
        ISwapExtractionService CreateSwapExtractionServiceFactory(bool testnet, bool useCirrus = false);
    }
}