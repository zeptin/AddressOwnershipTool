using AddressOwnershipTool.Common;

namespace AddressOwnershipTool.Commands.Claim;

public class ClaimCommand : ICommand<Result>
{
    public string WalletName { get; set; }

    public string Destination { get; set; }

    public string WalletPassword { get; set; }

    public string WalletAccount { get; set; }

    public string PrivateKeyFile { get; set; }

    public bool UseCirrus { get; set; }

    public bool Api { get; set; }

    public bool Deep { get; set; }

    public bool Testnet { get; set; }

    public string DataFolder { get; set; }

    public string OutputFolder { get; set; }
}
