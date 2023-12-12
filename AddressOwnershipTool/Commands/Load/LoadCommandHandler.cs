using AddressOwnershipTool.Common;
using AddressOwnershipTool.Common.Models;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using NBitcoin;

namespace AddressOwnershipTool.Commands.Load;

public class LoadCommandHandler : ICommandHandler<LoadCommand, Result<List<ClaimGroup>>>
{
    private readonly IAddressOwnershipServiceFactory _addressOwnershipServiceFactory;

    public LoadCommandHandler(IAddressOwnershipServiceFactory addressOwnershipServiceFactory)
    {
        _addressOwnershipServiceFactory = addressOwnershipServiceFactory;
    }

    public async Task<Result<List<ClaimGroup>>> Handle(LoadCommand request, CancellationToken cancellationToken)
    {
        var claimGroups = new List<ClaimGroup>();
        var claims = new List<Claim>();

        var allFiles = Directory.GetFiles(request.Path);

        var csvFiles = allFiles.Where(f => f.EndsWith(".csv")).ToList();

        if (!csvFiles.Any())
        {
            return Result.Fail<List<ClaimGroup>>("Folder contains no valid csv files");
        }

        foreach (var csvFile in csvFiles)
        {
            using (var reader = new StreamReader(csvFile))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                var txs = csv.GetRecords<OwnershipTransaction>().ToList();

                txs = txs.Where(t => t.SenderAmount > 0).ToList();

                if (!txs.Any())
                    continue;

                claims.AddRange(txs.Select(t => new Claim
                {
                    Balance = Money.Satoshis(t.SenderAmount).ToUnit(MoneyUnit.BTC),
                    Destination = t.StraxAddress,
                    OriginNetwork = t.SignedAddress.StartsWith("C") ? "Cirrus" : "Strax",

                }).ToList());
            }
        }

        var grouped = claims
            .GroupBy(c => c.Destination)
            .Select(g => new ClaimGroup
                {
                    Destination = g.Key,
                    Claims = g.Select(c => c).ToList()
                })
            .ToList();

        return Result.Ok(grouped);
    }
}
