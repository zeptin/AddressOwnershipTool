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

        RuleFor(x => x.EndBlock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Please specify a valid end block")
            .WithName("--end");

        RuleFor(x => new { x.StartBlock, x.EndBlock })
            .Must(x => x.StartBlock <= x.EndBlock)
            .WithMessage("Start block cannot be higher than end block");

        When(x => !string.IsNullOrEmpty(x.OutputFolder), () => {
            RuleFor(x => x.OutputFolder)
            .NotEmpty()
            .Must(x => Path.IsPathFullyQualified(x))
            .WithMessage("Please specify a valid output folder path")
            .WithName("--outputFolder");
        });
    }
}
