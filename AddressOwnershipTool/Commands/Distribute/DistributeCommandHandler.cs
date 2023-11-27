using AddressOwnershipTool.Common;

namespace AddressOwnershipTool.Commands.Distribute;

public class DistributeCommandHandler : ICommandHandler<DistributeCommand, Result>
{
    private readonly IAddressOwnershipServiceFactory _addressOwnershipServiceFactory;

    public DistributeCommandHandler(IAddressOwnershipServiceFactory addressOwnershipServiceFactory)
    {
        _addressOwnershipServiceFactory = addressOwnershipServiceFactory;
    }

    public async Task<Result> Handle(DistributeCommand request, CancellationToken cancellationToken)
    {
        // We don't need wallet credentials to simulate the send.
        Console.WriteLine("Doing a trial run of the distribution to obtain the overall amount to be sent...");

        var addressOwnershipService = _addressOwnershipServiceFactory.CreateAddressOwnershipService(request.Testnet, request.UseCirrus);
        addressOwnershipService.BuildAndSendDistributionTransactions(string.Empty, string.Empty, string.Empty, false);

        Console.WriteLine("Proceed with sending funds (y/n)?");

        int result = Console.Read();
        if (result != 121 && result != 89)
        {
            return Result.Fail("Exiting...");
        }

        addressOwnershipService.BuildAndSendDistributionTransactions(request.WalletName, request.WalletPassword, request.WalletAccount, true);

        return Result.Ok();
    }
}
