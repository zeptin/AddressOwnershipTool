namespace AddressOwnershipTool.Common.Models;

public class OwnershipTransaction
{
    public string StraxAddress { get; set; }

    public decimal SenderAmount { get; set; }

    public string SignedAddress { get; set; }

    public string Type { get; set; }
}
