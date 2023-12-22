using CommandLine;

namespace AddressOwnershipTool.Commands.Scan;

[Verb("scan", HelpText = "Creates file for burnt tokens")]
public class ScanInstruction : BaseInstruction
{
    [Option('c', "cirrus", Required = false, HelpText = "[Optional] Specify if you want to use CIRRUS, default is STRAX.")]
    public bool UseCirrus { get; set; }

    [Option('s', "start", Required = false, HelpText = "[Required] Start block to scan.")]
    public int StartBlock { get; set; }

    [Option('e', "end", Required = false, HelpText = "[Optional] End block to scan.", Default = 2100000)]
    public int EndBlock { get; set; }

    [Option('o', "outputFolder", Required = false, HelpText = "[Optional] Path for the folder where generated CSV will go")]
    public string OutputFolder { get; set; }

    public ScanCommand ToCommand()
    {
        return new ScanCommand
        {
            Testnet = Testnet,
            UseCirrus = UseCirrus,
            StartBlock = StartBlock,
            EndBlock = EndBlock,
            OutputFolder = OutputFolder
        };
    }
}
