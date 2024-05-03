namespace AddressOwnershipTool.Web.Models;

public class SwapRequest
{
    public string TxHash { get; set; }

    public string Destination { get; set; }

    public string Origin { get; set; }

    public decimal Amount { get; set; }

    public string Path { get; set; }

    public string Type { get; set; }
}
