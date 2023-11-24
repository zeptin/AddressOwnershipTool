using AddressOwnershipTool.Common;

namespace AddressOwnershipTool.Commands.Distribute;

public class DistributeCommand : ICommand<Result>
{
    public string WalletName { get; set; }

    public string WalletPassword { get; set; }

    public string WalletAccount { get; set; }

    public bool Testnet { get; set; }
}
