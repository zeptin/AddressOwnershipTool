using AddressOwnershipTool.Common;

namespace AddressOwnershipTool.Commands.Load;

public class LoadCommand : ICommand<Result<List<ClaimGroup>>>
{
    public string Path { get; set; }

    public int Limit { get; set; }
}
