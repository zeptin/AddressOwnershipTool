using AddressOwnershipTool.Common;

namespace AddressOwnershipTool.Commands.Validate;

public class ValidateCommandHandler : ICommandHandler<ValidateCommand, Result>
{
    private readonly IAddressOwnershipServiceFactory _addressOwnershipServiceFactory;

    public ValidateCommandHandler(IAddressOwnershipServiceFactory addressOwnershipServiceFactory)
    {
        _addressOwnershipServiceFactory = addressOwnershipServiceFactory;
    }

    public async Task<Result> Handle(ValidateCommand request, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(request.Signaturefolder))
        {
            return Result.Fail($"Could not locate directory '{request.Signaturefolder}'!");
        }

        var addressOwnershipService = _addressOwnershipServiceFactory.CreateAddressOwnershipService(request.Testnet, request.UseCirrus, outputPath: request.OutputFolder);

        addressOwnershipService.Validate(request.Signaturefolder);

        return Result.Ok();
    }
}
