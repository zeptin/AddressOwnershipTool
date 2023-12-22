using CommandLine;

namespace AddressOwnershipTool.Commands.Validate;

[Verb("validate", HelpText = "Validates token claims")]
public class ValidateInstruction : BaseInstruction
{
    [Option('s', "sigfolder", Required = true, HelpText = "Please provide signature folder.")]
    public string Signaturefolder { get; set; }

    [Option('c', "cirrus", Required = false, HelpText = "[Optional] Specify if you want to use CIRRUS, default is STRAX.")]
    public bool UseCirrus { get; set; }

    [Option('o', "outputFolder", Required = false, HelpText = "[Optional] Path for the folder where generated CSV will go")]
    public string OutputFolder { get; set; }

    public ValidateCommand ToCommand()
    {
        return new ValidateCommand
        {
            Signaturefolder = Signaturefolder,
            UseCirrus = UseCirrus,
            Testnet = Testnet,
            OutputFolder = OutputFolder
        };
    }
}
