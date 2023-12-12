using FluentValidation;

namespace AddressOwnershipTool.Commands.Load;

public class LoadCommandValidator : AbstractValidator<LoadCommand>
{
    public LoadCommandValidator()
    {
        RuleFor(x => x.Path)
            .NotEmpty()
            .Must(x => Directory.Exists(x))
            .WithMessage("Please specify a valid distribution folder path");
    }
}
