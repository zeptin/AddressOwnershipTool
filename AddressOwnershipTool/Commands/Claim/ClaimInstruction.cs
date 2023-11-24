using CommandLine;

namespace AddressOwnershipTool.Commands.Claim;

[Verb("claim", HelpText = "Creates file for token claim request")]
public class ClaimInstruction : BaseInstruction
{
    [Option('f', "destination", Required = false, HelpText = "Please provide destination file path.")]
    public string Destination { get; set; }

    [Option('n', "walletname", Required = false, HelpText = "Please provide target wallet name.")]
    public string WalletName { get; set; }

    [Option('p', "walletpassword", Required = false, HelpText = "Please provide target wallet password.")]
    public string WalletPassword { get; set; }

    [Option('a', "walletaccount", Required = false, HelpText = "Please provide target wallet account (Default 'account 0').", Default = "account 0")]
    public string WalletAccount { get; set; }

    [Option('p', "privkeyfile", Required = false, HelpText = "Please provide private key file, if used all other settings not required.")]
    public string PrivateKeyFile { get; set; }

    [Option('i', "api", Required = false, HelpText = "Specify if claim should be done via wallet API.")]
    public bool Api { get; set; }

    [Option('d', "deep", Required = false, HelpText = "Specify Whether or not to export SFN addresses with no transactions.")]
    public bool Deep { get; set; }

    public ClaimCommand ToCommand()
    {
        return new ClaimCommand
        {
            Api = Api,
            WalletAccount = WalletAccount,
            PrivateKeyFile = PrivateKeyFile,
            WalletPassword = WalletPassword,
            WalletName = WalletName,
            Deep = Deep,
            Testnet = Testnet,
            Destination = Destination
        };
    }
}
