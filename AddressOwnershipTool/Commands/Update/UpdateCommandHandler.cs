using AddressOwnershipTool.Common;
using CsvHelper;
using System.Globalization;
namespace AddressOwnershipTool.Commands.Update;

public class UpdateCommandHandler : ICommandHandler<UpdateCommand, Result>
{
    private readonly IAddressOwnershipServiceFactory _addressOwnershipServiceFactory;

    public UpdateCommandHandler(IAddressOwnershipServiceFactory addressOwnershipServiceFactory)
    {
        _addressOwnershipServiceFactory = addressOwnershipServiceFactory;
    }

    public async Task<Result> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var distributionFolder = Path.Combine(request.Path, "distribution");
        if (!Directory.Exists(distributionFolder))
        {
            Directory.CreateDirectory(distributionFolder);
        }

        var fileExists = File.Exists(Path.Combine(distributionFolder, "distributed.csv"));

        using (FileStream stream = File.Open(Path.Combine(distributionFolder, "distributed.csv"), FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            if (!fileExists)
            {
                csv.WriteHeader<SwappedTx>();
            }
            csv.NextRecord();

            csv.WriteRecord(new SwappedTx
            {
                Amount = request.Amount,
                TxHash = request.TxHash,
                Destination = request.Destination,
                Type = request.Type
            });
        }

        return Result.Ok();
    }
}
