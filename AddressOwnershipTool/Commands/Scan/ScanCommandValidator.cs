using AddressOwnershipTool.Common;
using FluentValidation;

namespace AddressOwnershipTool.Commands.Scan;

public class ScanCommandValidator : AbstractValidator<ScanCommand>
{
    public ScanCommandValidator()
    {
        RuleFor(x => x.StartBlock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Please specify a valid start block")
            .WithName("--start");
    }
}
