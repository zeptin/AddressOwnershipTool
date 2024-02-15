using Newtonsoft.Json;

namespace AddressOwnershipTool.Common;

public sealed class SwapTransaction
{
    public int BlockHeight { get; set; }

    public string DestinationAddress { get; set; }

    public string SenderAmount { get; set; }

    public decimal SenderAmountValue => string.IsNullOrEmpty(this.SenderAmount) ? 0 : (decimal.TryParse(this.SenderAmount, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var amount) ? amount : 0);

    public string TransactionHash { get; set; }

    public string Network { get; set; }
}

public class Amount
{
    public long satoshi { get; set; }
}

public class Fee
{
    public int satoshi { get; set; }
}

public class Amount2
{
    public long satoshi { get; set; }
}

public class In
{
    public string hash { get; set; }

    public Amount2 amount { get; set; }

    public int n { get; set; }
}

public class Amount3
{
    public long satoshi { get; set; }
}

public class Out
{
    public string hash { get; set; }

    public Amount3 amount { get; set; }

    public int n { get; set; }
}

public class TransactionModel
{
    public string hash { get; set; }

    public bool isCoinbase { get; set; }

    public bool isCoinstake { get; set; }

    public bool isSmartContract { get; set; }

    public Amount amount { get; set; }

    public Fee fee { get; set; }

    public int height { get; set; }

    public DateTime firstSeen { get; set; }

    public int time { get; set; }

    public bool spent { get; set; }

    [JsonProperty(PropertyName = "in")]
    public List<In> _in { get; set; }

    [JsonProperty(PropertyName = "out")]
    public List<Out> _out { get; set; }

    public int confirmations { get; set; }
}