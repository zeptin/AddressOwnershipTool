namespace AddressOwnershipTool.Common.Models;

public class AddressCheckResult
{
    public AddressCheckResult(bool hasBalance, bool hasActivity)
    {
        HasActivity = hasActivity;
        HasBalance = hasBalance;
    }

    public bool HasBalance { get; set; }

    public bool HasActivity { get; set; }
}
