using AddressOwnershipTool.Common;

namespace AddressOwnershipTool.Commands.Validate;

public class ValidateCommand : ICommand<Result>
{
    public string Signaturefolder { get; set; }

    public bool Testnet { get; set; }

    public bool UseCirrus { get; set; }
}
