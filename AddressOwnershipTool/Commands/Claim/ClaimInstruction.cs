using CommandLine;

namespace AddressOwnershipTool.Commands.Claim;

[Verb("claim", HelpText = "Creates file for token claim request")]
public class ClaimInstruction : BaseInstruction
{
    [Option('d', "destination", Required = false, HelpText = "[Required] Please provide destination StratisEVM address.")]
    public string Destination { get; set; }

    [Option('n', "walletname", Required = false, HelpText = "[Required] Please provide target wallet name.")]
    public string WalletName { get; set; }

    [Option('p', "walletpassword", Required = false, HelpText = "[Required] Please provide target wallet password.")]
    public string WalletPassword { get; set; }

    [Option('a', "walletaccount", Required = false, HelpText = "[Required] Please provide target wallet account", Default = "account 0")]
    public string WalletAccount { get; set; }

    [Option('c', "cirrus", Required = false, HelpText = "[Optional] Specify if you want to use CIRRUS, default is STRAX.")]
    public bool UseCirrus { get; set; }

    [Option('r', "datafolder", Required = false, HelpText = "[Optional] Root folder for your node")]
    public string DataFolder { get; set; }

    public ClaimCommand ToCommand()
    {
        return new ClaimCommand
        {
            WalletAccount = WalletAccount,
            WalletPassword = WalletPassword,
            WalletName = WalletName,
            Testnet = Testnet,
            UseCirrus = UseCirrus,
            Destination = Destination,
            DataFolder = DataFolder
        };
    }
}
