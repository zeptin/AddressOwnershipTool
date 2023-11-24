using FluentValidation;

namespace AddressOwnershipTool.Commands.Distribute;

public class DistributeCommandValidator : AbstractValidator<DistributeCommand>
{
    public DistributeCommandValidator()
    {            
        RuleFor(x => x.WalletName).NotEmpty();
        RuleFor(x => x.WalletPassword).NotEmpty();
    }
}
