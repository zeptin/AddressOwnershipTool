using AddressOwnershipTool.Common.Models;
using CsvHelper.Configuration;
using CsvHelper;
using NBitcoin.DataEncoders;
using NBitcoin;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Consensus;
using Stratis.Bitcoin.Features.Wallet.Models;
using Stratis.Bitcoin.Features.Wallet;
using Stratis.Bitcoin.Networks;
using Stratis.Bitcoin.Utilities;
using Stratis.Features.SQLiteWalletRepository;
using System.Globalization;
using System.Reflection;
using System.Text;
using Stratis.Sidechains.Networks;
using System;
using AddressOwnershipTool.Commands.Update;

namespace AddressOwnershipTool.Common;

public class AddressOwnershipService : IAddressOwnershipService
{
    private const string distributedTransactionsFilename = "distributed.csv";
    private const string ownershipFilename = "ownership.csv";
    private const decimal splitThreshold = 10_000m * 100_000_000m; // In stratoshi
    private const decimal splitCount = 10;
    private const decimal minumumBalanceToAcceptInSats = 5460;

    private readonly Network network;
    private readonly string ownershipFilePath;
    private readonly INodeApiClientFactory nodeApiClientFactory;
    private readonly IEthRpcClientFactory ethRpcClientFactory;
    private readonly string outputPath;
    private List<SwappedTx> distributedTransactions;
    private List<OwnershipTransaction> ownershipTransactions;
    private int straxApiPort;

    public AddressOwnershipService(INodeApiClientFactory nodeApiClientFactory, IEthRpcClientFactory ethRpcClientFactory, bool testnet, bool useCirrus = false, bool loadFiles = true, string outputPath = null)
    {
        this.network = testnet 
            ? (useCirrus ? new CirrusTest() : new StraxTest())
            : (useCirrus ? new CirrusMain() : new StraxMain());

        this.straxApiPort = testnet
            ? (useCirrus ? 38223 : 27103)
            : (useCirrus ? 37223 : 17103);

        this.ownershipFilePath = string.IsNullOrEmpty(outputPath)
                ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                : outputPath;

        // Only needed for -validate and -distribute
        if (loadFiles)
        {
            this.LoadAlreadyDistributedTransactions();
            this.LoadSwapTransactionFile();
        }

        this.nodeApiClientFactory = nodeApiClientFactory;
        this.ethRpcClientFactory = ethRpcClientFactory;
        this.outputPath = outputPath;
    }

    private void LoadAlreadyDistributedTransactions()
    {
        Console.WriteLine($"Loading already distributed transactions...");

        if (File.Exists(Path.Combine(this.ownershipFilePath, distributedTransactionsFilename)))
        {
            using (var reader = new StreamReader(Path.Combine(this.ownershipFilePath, distributedTransactionsFilename)))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false }))
            {
                this.distributedTransactions = csv.GetRecords<SwappedTx>().ToList();
            }
        }
        else
        {
            using (FileStream file = File.Create(Path.Combine(this.ownershipFilePath, distributedTransactionsFilename)))
            {
                file.Close();
            }

            this.distributedTransactions = new List<SwappedTx>();
        }
    }

    private void LoadSwapTransactionFile()
    {
        Console.WriteLine($"Loading transaction file...");

        // First check if the ownership file has been created.
        if (File.Exists(Path.Combine(this.ownershipFilePath, ownershipFilename)))
        {
            // If so populate the list from disk.
            using (var reader = new StreamReader(Path.Combine(this.ownershipFilePath, ownershipFilename)))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                this.ownershipTransactions = csv.GetRecords<OwnershipTransaction>().ToList();
            }
        }
        else
        {
            Console.WriteLine("A transaction file has not been created yet, is this correct? (y/n)");
            int result = Console.Read();
            if (result != 121 && result != 89)
            {
                Console.WriteLine("Exiting...");
                return;
            }

            this.ownershipTransactions = new List<OwnershipTransaction>();
        }
    }

    public void Validate(string sigFileFolder)
    {
        var straxApiClient = nodeApiClientFactory.CreateNodeApiClient($"http://localhost:{this.network.DefaultAPIPort}/api");

        foreach (string file in Directory.GetFiles(sigFileFolder))
        {
            if (!file.EndsWith(".csv"))
                continue;

            Console.WriteLine($"Validating signature file '{file}'...");

            foreach (string line in File.ReadLines(file))
            {
                try
                {
                    // TODO: Modify this to use CsvHelper too?
                    string[] data = line.Split(";");

                    if (data.Length != 3)
                        continue;

                    if (data[0].Equals("Address"))
                        continue;

                    string address = data[0].Trim();
                    string destination = data[1].Trim();
                    string signature = data[2].Trim();

                    if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(destination) || string.IsNullOrWhiteSpace(signature))
                    {
                        Console.WriteLine($"Malformed record: {line}");
                        continue;
                    }

                    if (this.ownershipTransactions.Any(s => s.SignedAddress == address))
                    {
                        Console.WriteLine($"Ownership already proven for address: {address}. Ignoring this signature.");

                        continue;
                    }

                    // The address string is the actual message in this case.
                    var pubKey = PubKey.RecoverFromMessage(address, signature);

                    if (pubKey.Hash.ScriptPubKey.GetDestinationAddress(this.network).ToString() != address)
                    {
                        Console.WriteLine($"Invalid signature for address '{address}'!");
                    }

                    if (!destination.IsValidAddress())
                    {
                        Console.WriteLine($"The provided StratisEVM address was invalid: {destination}");

                        continue;
                    }

                    Console.WriteLine($"Validated signature for address '{address}'");

                    decimal balance = straxApiClient.GetAddressBalance(address);

                    // now check that destination doesn't already have this amount

                    if (balance == 0)
                    {
                        Console.WriteLine($"Address {address} has a zero balance, skipping it.");

                        continue;
                    }
                    else if (balance < minumumBalanceToAcceptInSats)
                    {
                        Console.WriteLine($"Address {address} has a balance of {balance} sats which is below minimum of {minumumBalanceToAcceptInSats}, skipping it.");

                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"Address {address} has a balance of {balance}.");
                    }

                    // We checked for an existing record already, so it is safe to add it now.
                    this.ownershipTransactions.Add(new OwnershipTransaction()
                    {
                        SenderAmount = balance,
                        StraxAddress = destination,
                        // We set this to the source address on the Strax chain, to ensure only one record exists per unique address.
                        SignedAddress = address,
                        Type = "Manual"
                    });
                }
                catch
                {
                    Console.WriteLine($"Error processing signature for '{line}'!");
                }
            }

            using (var writer = new StreamWriter(Path.Combine(this.ownershipFilePath, ownershipFilename)))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(this.ownershipTransactions);
            }
        }

        Console.WriteLine($"There are {this.ownershipTransactions.Count} ownership transactions so far to process.");
        Console.WriteLine($"There are {Money.Satoshis(this.ownershipTransactions.Sum(s => s.SenderAmount)).ToUnit(MoneyUnit.BTC)} STRAX with ownership proved.");
    }

    public void StratisXExport(string privKeyFile, string destinationAddress)
    {
        var lines = File.ReadLines(privKeyFile);

        foreach (var line in lines)
        {
            // Skip comments
            if (line.Trim().StartsWith("#"))
                continue;

            // If it isn't at least long enough to contain the WIF then ignore the line
            if (line.Trim().Length < 53)
                continue;

            try
            {
                string[] data = line.Trim().Split(" ");

                string privKey = data[0];

                Key privateKey = Key.Parse(privKey, this.network);

                string address = privateKey.PubKey.GetAddress(this.network).ToString();

                string message = $"{address}";

                string signature = privateKey.SignMessage(message);

                this.OutputToFile(address, destinationAddress, signature);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error creating signature with private key file line '{line}'");
            }
        }
    }

    public void SbfnExport(string walletName, string walletPassword, string destinationAddress, bool deepExport = false, string dataFolder = null)
    {
        var nodeSettings = string.IsNullOrEmpty(dataFolder)
            ? new NodeSettings(this.network)
            : new NodeSettings(this.network, args: new string[] { $"datadirroot={dataFolder}" });

        // First check if sqlite wallet is being used.
        var walletRepository = new SQLiteWalletRepository(nodeSettings.LoggerFactory, nodeSettings.DataFolder, nodeSettings.Network, new DateTimeProvider(), new ScriptAddressReader());
        walletRepository.Initialize(false);

        Wallet wallet = null;

        try
        {
            wallet = walletRepository.GetWallet(walletName);
        }
        catch
        {
        }

        if (wallet != null)
        {
            this.HdAddressExport(wallet, walletPassword, destinationAddress, deepExport);

            return;
        }

        Console.WriteLine($"No SQL wallet with name {walletName} was found in folder {nodeSettings.DataDir}! Checking for legacy JSON wallet.");

        var fileStorage = new FileStorage<Wallet>(nodeSettings.DataFolder.WalletPath);

        if (fileStorage.Exists(walletName + ".wallet.json"))
        {
            wallet = fileStorage.LoadByFileName(walletName + ".wallet.json");
        }

        if (wallet != null)
        {
            this.HdAddressExport(wallet, walletPassword, destinationAddress, deepExport);

            return;
        }

        Console.WriteLine($"No legacy wallet with name {walletName} was found in folder {nodeSettings.DataFolder.WalletPath}!");
    }

    public void HdAddressExport(Wallet wallet, string walletPassword, string destinationAddress, bool deepExport = false)
    {
        foreach (HdAddress address in wallet.GetAllAddresses())
        {
            if (address.Transactions.Count == 0 && !deepExport)
                continue;

            this.ExportAddress(wallet, address, walletPassword, destinationAddress);
        }
    }

    public void ExportAddress(Wallet wallet, HdAddress address, string walletPassword, string destinationAddress)
    {
        ISecret privateKey = wallet.GetExtendedPrivateKeyForAddress(walletPassword, address);

        string message = $"{address.Address}";

        string signature = privateKey.PrivateKey.SignMessage(message);

        OutputToFile(address.Address, destinationAddress, signature);
    }

    public void OutputToFile(string address, string destinationAddress, string signature)
    {
        string export = $"{address};{destinationAddress};{signature}";

        Console.WriteLine(export);

        var destination = string.IsNullOrEmpty(this.outputPath) ? $"{destinationAddress}.csv" : Path.Combine(this.outputPath, $"{destinationAddress}.csv");

        using (StreamWriter sw = File.AppendText(destination))
        {
            sw.WriteLine(export);
        }
    }

    private List<RecipientModel> GetRecipients(string destinationAddress, decimal amount)
    {
        if (amount < splitThreshold)
        {
            return new List<RecipientModel> { new RecipientModel { DestinationAddress = destinationAddress, Amount = Money.Satoshis(amount).ToUnit(MoneyUnit.BTC).ToString() } };
        }

        var recipientList = new List<RecipientModel>();

        for (int i = 0; i < splitCount; i++)
        {
            recipientList.Add(new RecipientModel()
            {
                DestinationAddress = destinationAddress,
                Amount = Money.Satoshis(amount / splitCount).ToUnit(MoneyUnit.BTC).ToString()
            });
        }

        return recipientList;
    }

    public void BuildAndSendDistributionTransactions(string walletName, string walletPassword, string accountName, bool send = false)
    {
        var straxApiClient = nodeApiClientFactory.CreateNodeApiClient($"http://localhost:{this.straxApiPort}/api");

        int count = 0;
        decimal total = 0;

        foreach (OwnershipTransaction ownershipTransaction in this.ownershipTransactions)
        {
            if (this.distributedTransactions.Any(d => d.Destination == ownershipTransaction.SignedAddress))
            {
                Console.WriteLine($"Already distributed: {ownershipTransaction.StraxAddress} -> {Money.Satoshis(ownershipTransaction.SenderAmount).ToUnit(MoneyUnit.BTC)} STRAT");

                continue;
            }

            if (!send)
            {
                count++;
                total += ownershipTransaction.SenderAmount;

                Console.WriteLine($"Simulate send of {Money.FromUnit(ownershipTransaction.SenderAmount, MoneyUnit.Satoshi)} to address {ownershipTransaction.StraxAddress}");

                continue;
            }

            try
            {
                var distributedSwapTransaction = new SwappedTx();

                List<RecipientModel> recipients = GetRecipients(ownershipTransaction.StraxAddress, ownershipTransaction.SenderAmount);

                WalletBuildTransactionModel builtTransaction = straxApiClient.BuildTransaction(walletName, walletPassword, accountName, recipients);

                straxApiClient.SendTransaction(builtTransaction.Hex);

                distributedSwapTransaction.Destination = ownershipTransaction.StraxAddress;
                distributedSwapTransaction.Amount = ownershipTransaction.SenderAmount;
                distributedSwapTransaction.TxHash = builtTransaction.TransactionId.ToString();
                distributedSwapTransaction.Type = "Manual";

                if (send)
                    Console.WriteLine($"Swap transaction built and sent to {distributedSwapTransaction.Destination}:{Money.Satoshis(distributedSwapTransaction.Amount).ToUnit(MoneyUnit.BTC)}");

                // Append to the file.
                using (FileStream stream = File.Open(Path.Combine(this.ownershipFilePath, distributedTransactionsFilename), FileMode.Append))
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecord(distributedSwapTransaction);
                    csv.NextRecord();
                }

                this.distributedTransactions.Add(distributedSwapTransaction);

                // Give some time for the transaction to begin relaying.
                Thread.Sleep(1000);

                count++;
                total += ownershipTransaction.SenderAmount;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                break;
            }
        }

        Console.WriteLine($"Count: {count} distribution transactions");
        Console.WriteLine($"Total: {Money.FromUnit(total, MoneyUnit.Satoshi)} STRAT");
    }
}
