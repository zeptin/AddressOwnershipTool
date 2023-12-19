using NBitcoin;

namespace AddressOwnershipTool.Common;

public sealed class CastVote
{
    public string Address { get; set; }
    public Money Balance { get; set; }
    public bool InFavour { get; set; }
    public int BlockHeight { get; set; }
}