using CommandLine;

namespace AddressOwnershipTool.Commands
{
    public class BaseInstruction
    {
        [Option('t', "testnet", Required = false, HelpText = "Specify if testnet should be used.")]
        public bool Testnet { get; set; }
    }
}
