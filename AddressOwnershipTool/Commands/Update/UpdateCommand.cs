using AddressOwnershipTool.Common;

namespace AddressOwnershipTool.Commands.Update;

public class UpdateCommand : ICommand<Result>
{
    public string TxHash { get; set; }

    public string Destination { get; set; }

    public decimal Amount { get; set; }

    public string Path { get; set; }

    public string Type { get; set; }
}
