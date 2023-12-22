using FluentValidation;

namespace AddressOwnershipTool.Commands.Validate;

public class ValidateCommandValidator : AbstractValidator<ValidateCommand>
{
    public ValidateCommandValidator()
    {
        RuleFor(x => x.Signaturefolder).NotEmpty().WithName("--sigfolder");

        When(x => !string.IsNullOrEmpty(x.OutputFolder), () => {
            RuleFor(x => x.OutputFolder)
            .NotEmpty()
            .Must(x => Path.IsPathFullyQualified(x))
            .WithMessage("Please specify a valid output folder path")
            .WithName("--outputFolder");
        });
    }
}
