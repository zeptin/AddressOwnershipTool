using AddressOwnershipTool.Common;
using AddressOwnershipTool.Common.Models;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using NBitcoin;
using AddressOwnershipTool.Commands.Update;

namespace AddressOwnershipTool.Commands.Load;

public class LoadCommandHandler : ICommandHandler<LoadCommand, Result<List<ClaimGroup>>>
{
    public async Task<Result<List<ClaimGroup>>> Handle(LoadCommand request, CancellationToken cancellationToken)
    {
        var claimGroups = new List<ClaimGroup>();
        var claims = new List<Claim>();

        var alreadySwapped = LoadSwappedTxs(request.Path);

        var allFiles = Directory.GetFiles(request.Path);

        var csvFiles = allFiles.Where(f => f.EndsWith(".csv")).ToList();

        if (!csvFiles.Any())
        {
            return Result.Fail<List<ClaimGroup>>("Folder contains no valid csv files");
        }

        foreach (var csvFile in csvFiles.Where(c => c.Contains("swaps.csv")))
        {
            using (var reader = new StreamReader(csvFile))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                var txs = csv.GetRecords<SwapTransaction>().ToList();

                txs = txs.Where(t => t.SenderAmount > 0).ToList();

                if (!txs.Any())
                    continue;

                claims.AddRange(txs.Select(t => new Claim
                {
                    Balance = Money.Satoshis(t.SenderAmount).ToUnit(MoneyUnit.BTC),
                    Destination = t.DestinationAddress,
                    OriginNetwork = t.Network,
                    Type = "Burnt"

                }).ToList());
            }
        }

        foreach (var csvFile in csvFiles.Where(c => !c.Contains("swaps.csv")))
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
                    Type = "Manual"

                }).ToList());
            }
        }

        var grouped = claims
            .GroupBy(c => c.Destination)
            .Select(g => new ClaimGroup
                {
                    Destination = g.Key,
                    Claims = g.Select(c => c).ToList(),
                    Type = g.First().Type,
                })
            .Where(c => !alreadySwapped.Any(s => s.Destination == c.Destination))
            .ToList();

        return Result.Ok(grouped);
    }

    private List<SwappedTx> LoadSwappedTxs(string path)
    {
        var filePath = Path.Combine(path, "distribution", "distributed.csv");
        if (!File.Exists(filePath))
        {
            return new List<SwappedTx>();
        }

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            return csv.GetRecords<SwappedTx>().ToList();
        }
    }
}
