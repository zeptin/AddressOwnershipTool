using CommandLine;

namespace AddressOwnershipTool.Commands.Claim;

[Verb("claim", HelpText = "Creates file for token claim request")]
public class ClaimInstruction : BaseInstruction
{
    [Option('f', "destination", Required = false, HelpText = "Please provide destination StratisEVM address.")]
    public string Destination { get; set; }

    [Option('n', "walletname", Required = false, HelpText = "Please provide target wallet name.")]
    public string WalletName { get; set; }

    [Option('p', "walletpassword", Required = false, HelpText = "Please provide target wallet password.")]
    public string WalletPassword { get; set; }

    [Option('a', "walletaccount", Required = false, HelpText = "Please provide target wallet account.", Default = "account 0")]
    public string WalletAccount { get; set; }

    public ClaimCommand ToCommand()
    {
        return new ClaimCommand
        {
            WalletAccount = WalletAccount,
            WalletPassword = WalletPassword,
            WalletName = WalletName,
            Testnet = Testnet,
            Destination = Destination
        };
    }
}
