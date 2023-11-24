using AddressOwnershipTool.Common;

namespace AddressOwnershipTool.Commands.Claim;

public class ClaimCommandHandler : ICommandHandler<ClaimCommand, Result>
{
    private readonly IAddressOwnershipServiceFactory _addressOwnershipServiceFactory;
    private readonly INodeApiClientFactory _nodeApiClientFactory;

    public ClaimCommandHandler(IAddressOwnershipServiceFactory addressOwnershipServiceFactory, INodeApiClientFactory nodeApiClientFactory)
    {
        _addressOwnershipServiceFactory = addressOwnershipServiceFactory;
        _nodeApiClientFactory = nodeApiClientFactory;
    }

    public async Task<Result> Handle(ClaimCommand request, CancellationToken cancellationToken)
    {
        var fileName = $"{request.Destination}.csv";

        if (!File.Exists(fileName))
        {
            using (StreamWriter sw = File.AppendText(fileName))
            {
                sw.WriteLine("Address;Destination;Signature");
            }
        }

        // Settings related to a stratisX wallet export.
        if (!string.IsNullOrEmpty(request.PrivateKeyFile))
        {
            if (!File.Exists(request.PrivateKeyFile))
            {
                return Result.Fail($"Unable to locate private key file {request.PrivateKeyFile} for stratisX address ownership!");
            }

            Console.WriteLine("Private key file provided, assuming stratisX address ownership is required");

            var addressOwnershipService = _addressOwnershipServiceFactory.CreateAddressOwnershipService(request.Testnet, false);
            addressOwnershipService.StratisXExport(request.PrivateKeyFile, request.Destination);

            Console.WriteLine("Finished");

            return Result.Ok();
        }

        if (request.Api)
        {
            Console.WriteLine("Attempting to extract address signatures from node API. The node needs to be running.");

            var client = _nodeApiClientFactory.CreateNodeApiClient(request.Testnet ? "http://localhost:27103/api/" : "http://localhost:17103/api/");

            // Get accounts
            List<string> accounts = client.GetAccounts(request.WalletName);

            foreach (string account in accounts)
            {
                Console.WriteLine($"Querying account '{account}' from wallet '{request.WalletName}'...'");

                // Get addresses
                List<string> addresses = client.GetAddresses(request.WalletName, account);

                foreach (string address in addresses)
                {
                    Console.WriteLine($"Attempting to sign message with address '{address}...'");

                    string signature = client.SignMessage(request.WalletName, request.WalletPassword, address);

                    var addressOwnershipService = _addressOwnershipServiceFactory.CreateAddressOwnershipService(request.Testnet, false);
                    addressOwnershipService.OutputToFile(address, request.Destination, signature);
                }
            }
        }
        else
        {
            var addressOwnershipService = _addressOwnershipServiceFactory.CreateAddressOwnershipService(request.Testnet, false);
            addressOwnershipService.SbfnExport(request.WalletName, request.WalletPassword, request.Destination, request.Deep);
        }

        return Result.Ok();
    }
}
