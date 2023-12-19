using Flurl;
using Flurl.Http;
using NBitcoin;
using Stratis.Bitcoin.Features.BlockStore.Models;

namespace AddressOwnershipTool.Common;

public abstract class ExtractionBase
{
    public const int EndHeight = 2100000;
}

public enum ExtractionType
{
    Swap,
    Vote
}
