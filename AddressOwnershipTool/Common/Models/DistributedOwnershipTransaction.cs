namespace AddressOwnershipTool.Common.Models;

public class DistributedOwnershipTransaction
{
    public DistributedOwnershipTransaction() { }

    public DistributedOwnershipTransaction(OwnershipTransaction ownershipTransaction)
    {
        StraxAddress = ownershipTransaction.StraxAddress;
        SenderAmount = ownershipTransaction.SenderAmount;
        SourceAddress = ownershipTransaction.SignedAddress;
    }

    public string SourceAddress { get; set; }

    public string StraxAddress { get; set; }

    public decimal SenderAmount { get; set; }

    public bool TransactionBuilt { get; set; }

    public bool TransactionSent { get; set; }

    public string TransactionSentHash { get; set; }
}
