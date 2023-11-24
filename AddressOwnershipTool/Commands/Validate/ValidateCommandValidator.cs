using FluentValidation;

namespace AddressOwnershipTool.Commands.Validate;

public class ValidateCommandValidator : AbstractValidator<ValidateCommand>
{
    public ValidateCommandValidator()
    {
        RuleFor(x => x.Signaturefolder).NotEmpty().WithName("--sigfolder");
    }
}
