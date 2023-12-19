using Stratis.Bitcoin.Controllers.Models;
using Stratis.Bitcoin.Features.BlockStore.Models;
using Stratis.Bitcoin.Features.Wallet.Models;

namespace AddressOwnershipTool.Common;

public interface INodeApiClient
{
    WalletBuildTransactionModel BuildTransaction(string walletName, string walletPassword, string accountName, List<RecipientModel> recipients);

    void Dispose();

    List<string> GetAccounts(string wallet);

    decimal GetAddressBalance(string address);

    List<string> GetAddresses(string walletName, string account);

    List<AddressBalanceChange> GetVerboseAddressBalance(string address);

    TransactionModel GetTransaction(string txId);

    Task<BlockTransactionDetailsModel> RetrieveBlockAtHeightAsync(int blockHeight);

    void SendTransaction(string hex);

    string SignMessage(string walletName, string walletPassword, string address);

    bool ValidateAddress(string address);
}