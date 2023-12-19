using CommandLine;

namespace AddressOwnershipTool.Commands.Scan;

[Verb("scan", HelpText = "Creates file for burnt tokens")]
public class ScanInstruction : BaseInstruction
{
    [Option('c', "cirrus", Required = false, HelpText = "[Optional] Specify if you want to use CIRRUS, default is STRAX.")]
    public bool UseCirrus { get; set; }

    [Option('s', "start", Required = false, HelpText = "[Required] Start block to scan.")]
    public int StartBlock { get; set; }

    public ScanCommand ToCommand()
    {
        return new ScanCommand
        {
            Testnet = Testnet,
            UseCirrus = UseCirrus,
            StartBlock = StartBlock
        };
    }
}
