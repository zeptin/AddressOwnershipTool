namespace AddressOwnershipTool.Common.Models;

public class QuarantinedTransaction
{
    public string Destination { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; }
}
