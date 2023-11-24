using Stratis.Bitcoin.Features.Wallet;

namespace AddressOwnershipTool.Common
{
    public interface IAddressOwnershipService
    {
        void BuildAndSendDistributionTransactions(string walletName, string walletPassword, string accountName, bool send = false);
        void ExportAddress(Wallet wallet, HdAddress address, string walletPassword, string destinationAddress);
        void HdAddressExport(Wallet wallet, string walletPassword, string destinationAddress, bool deepExport = false);
        void OutputToFile(string address, string destinationAddress, string signature);
        void SbfnExport(string walletName, string walletPassword, string destinationAddress, bool deepExport = false);
        void StratisXExport(string privKeyFile, string destinationAddress);
        void Validate(string sigFileFolder);
    }
}