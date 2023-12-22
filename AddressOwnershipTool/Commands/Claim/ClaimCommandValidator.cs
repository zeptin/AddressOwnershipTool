using AddressOwnershipTool.Common;
using FluentValidation;

namespace AddressOwnershipTool.Commands.Claim;

public class ClaimCommandValidator : AbstractValidator<ClaimCommand>
{
    public ClaimCommandValidator()
    {
        When(x => string.IsNullOrEmpty(x.PrivateKeyFile), () => {
            RuleFor(x => x.WalletName).NotEmpty().WithName("--walletname");
            RuleFor(x => x.WalletPassword).NotEmpty().WithName("--walletpassword");
        });

        When(x => !string.IsNullOrEmpty(x.DataFolder), () => {
            RuleFor(x => x.DataFolder)
            .NotEmpty()
            .Must(x => Path.IsPathFullyQualified(x))
            .WithMessage("Please specify a valid data folder path")
            .WithName("--datafolder");
        });

        When(x => !string.IsNullOrEmpty(x.OutputFolder), () => {
            RuleFor(x => x.OutputFolder)
            .NotEmpty()
            .Must(x => Path.IsPathFullyQualified(x))
            .WithMessage("Please specify a valid output folder path")
            .WithName("--outputFolder");
        });

        When(x => !string.IsNullOrEmpty(x.PrivateKeyFile), () => {
            RuleFor(x => x.PrivateKeyFile)
                .Must(x => Path.IsPathFullyQualified(x))
                .WithMessage("Specified private key file path is invalid")
                .WithName("--privkeyfile");
        });

        RuleFor(x => x.Destination)
            .NotEmpty()
            .Must(x => !string.IsNullOrEmpty(x) && x.IsValidAddress())
            .WithMessage("Please specify a valid StratisEVM address")
            .WithName("--destination");
    }
}
