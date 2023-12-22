using System.Globalization;
using System.Text;
using AddressOwnershipTool.Commands.Update;
using CsvHelper;
using CsvHelper.Configuration;
using NBitcoin;
using Stratis.Bitcoin.Controllers.Models;
using Stratis.Bitcoin.Features.BlockStore.Models;
using Stratis.Bitcoin.Networks;
using Stratis.Sidechains.Networks;

namespace AddressOwnershipTool.Common;

public sealed class SwapExtractionService : ExtractionBase, ISwapExtractionService
{
    private List<SwapTransaction> swapTransactions;
    private readonly INodeApiClientFactory nodeApiClientFactory;
    private readonly string outputPath;
    private readonly Network network;
    private readonly int straxApiPort;

    public SwapExtractionService(INodeApiClientFactory nodeApiClientFactory, bool testnet, bool useCirrus, string outputPath = null)
    {
        this.swapTransactions = new List<SwapTransaction>();
        this.nodeApiClientFactory = nodeApiClientFactory;
        this.outputPath = string.IsNullOrEmpty(outputPath) ? "swaps.csv" : Path.Combine(outputPath, "swaps.csv");
        this.network = testnet
            ? (useCirrus ? new CirrusTest() : new StraxTest())
            : (useCirrus ? new CirrusMain() : new StraxMain());

        this.straxApiPort = testnet
            ? (useCirrus ? 38223 : 27103)
            : (useCirrus ? 37223 : 17103);
    }

    public async Task RunAsync(int startBlock, int endBlock)
    {
        await this.LoadSwapTransactionFileAsync();

        await ScanForSwapTransactionsAsync(startBlock, endBlock);
    }

    private async Task LoadSwapTransactionFileAsync()
    {
        Console.WriteLine($"Loading swap transaction file...");

        // First check if the swap file has been created.
        if (File.Exists(this.outputPath))
        {
            // If so populate the list from disk.
            using (var reader = new StreamReader(this.outputPath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                this.swapTransactions = await csv.GetRecordsAsync<SwapTransaction>().ToListAsync();
            }
        }
        else
        {
            Console.WriteLine("A swap distribution file has not been created, is this correct? (y/n)");
            int result = Console.Read();
            if (result != 121 && result != 89)
            {
                Console.WriteLine("Exiting...");
                return;
            }
        }
    }

    private async Task ScanForSwapTransactionsAsync(int startBlock, int endBlock)
    {
        Console.WriteLine($"Scanning for swap transactions...");

        for (int height = startBlock; height < endBlock; height++)
        {
            var client = nodeApiClientFactory.CreateNodeApiClient($"http://localhost:{this.straxApiPort}/api");
            BlockTransactionDetailsModel block = await client.RetrieveBlockAtHeightAsync(height);
            if (block == null)
            {
                break;
            }

            this.ProcessBlockForSwapTransactions(block, height);
        }

        Console.WriteLine($"{this.swapTransactions.Count} swap transactions to process.");
        Console.WriteLine($"{Money.Satoshis(this.swapTransactions.Sum(s => s.SenderAmount)).ToUnit(MoneyUnit.BTC)} STRAX swapped.");

        using (var writer = new StreamWriter(this.outputPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(this.swapTransactions);
        }
    }

    private void ProcessBlockForSwapTransactions(BlockTransactionDetailsModel block, int blockHeight)
    {
        // Inspect each transaction
        foreach (TransactionVerboseModel transaction in block.Transactions)
        {
            // Find all the OP_RETURN outputs.
            foreach (Vout output in transaction.VOut.Where(o => o.ScriptPubKey.Type == "nulldata"))
            {
                IList<Op> ops = new NBitcoin.Script(output.ScriptPubKey.Asm).ToOps();
                var destinationAddress = Encoding.ASCII.GetString(ops.Last().PushData);
                try
                {
                    if (!destinationAddress.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // Verify the sender address is a valid Strax address
                    if (!destinationAddress.IsValidAddress())
                    {
                        Console.WriteLine($"Swap address invalid: {destinationAddress}:{output.Value}");
                        continue;
                    }

                    Console.WriteLine($"Swap address found: {destinationAddress}:{output.Value}");

                    if (this.swapTransactions.Any(s => s.TransactionHash == transaction.Hash))
                    {
                        Console.WriteLine($"Swap transaction already exists: {destinationAddress}:{output.Value}");
                    }
                    else
                    {
                        var swapTransaction = new SwapTransaction()
                        {
                            BlockHeight = blockHeight,
                            DestinationAddress = destinationAddress.ToString(),
                            SenderAmount = (long)Money.Coins(output.Value),
                            TransactionHash = transaction.Hash,
                            Network = this.network.Name.Contains("Strax", StringComparison.OrdinalIgnoreCase) ? "Strax" : "Cirrus"
                        };

                        this.swapTransactions.Add(swapTransaction);

                        Console.WriteLine($"Swap address added to file: {destinationAddress}:{output.Value}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Swap address invalid: {destinationAddress}:{output.Value}");
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
