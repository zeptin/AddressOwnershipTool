using CommandLine;

namespace AddressOwnershipTool.Commands.Validate;

[Verb("validate", HelpText = "Validates token claims")]
public class ValidateInstruction : BaseInstruction
{
    [Option('s', "sigfolder", Required = true, HelpText = "Please provide signature folder.")]
    public string Signaturefolder { get; set; }

    public ValidateCommand ToCommand()
    {
        return new ValidateCommand
        {
            Signaturefolder = Signaturefolder,
            Testnet = Testnet
        };
    }
}
