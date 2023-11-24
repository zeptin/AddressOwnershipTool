using AddressOwnershipTool.Common;
using FluentValidation;

namespace AddressOwnershipTool.Commands.Claim;

public class ClaimCommandValidator : AbstractValidator<ClaimCommand>
{
    public ClaimCommandValidator()
    {
        When(x => string.IsNullOrEmpty(x.PrivateKeyFile), () => {
            RuleFor(x => x.WalletName).NotEmpty();
            RuleFor(x => x.WalletPassword).NotEmpty();
        });

        When(x => !string.IsNullOrEmpty(x.PrivateKeyFile), () => {
            RuleFor(x => x.PrivateKeyFile)
                .Must(x => Path.IsPathFullyQualified(x))
                .WithMessage("Specified private key file path is invalid");
        });

        RuleFor(x => x.Destination)
            .NotEmpty()
            .Must(x => !string.IsNullOrEmpty(x) && x.IsValidAddress())
            .WithMessage("Please specify a valid StratisEVM address");
    }
}
