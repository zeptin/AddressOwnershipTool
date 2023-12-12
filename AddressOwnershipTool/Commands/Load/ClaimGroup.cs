namespace AddressOwnershipTool.Commands.Load;

public class ClaimGroup
{
    public string Destination { get; set; }

    public int NumberOfClaimedAddresses => this.Claims.Count();

    public decimal TotalAmountToTransfer => this.OriginalTotalBalance * 0.01M;

    public decimal OriginalTotalBalance => this.Claims.Sum(c => c.Balance);

    public List<Claim> Claims { get; set; } = new List<Claim>();
}

public class Claim
{
    public string Destination { get; set; }

    public decimal Balance { get; set; }

    public string OriginNetwork { get; set; }
}
