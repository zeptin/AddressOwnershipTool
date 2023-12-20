using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Web3;

namespace AddressOwnershipTool.Common;

public class GethRpcClient : IEthRpcCleint
{
    private readonly string _baseUrl;

    public GethRpcClient(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public async Task SignAndSendTransaction(string destination)
    {
        var client = new RpcClient(new Uri(_baseUrl));
        var request = new EthSendTransaction(client);
        var result = await request.SendRequestAsync(new Nethereum.RPC.Eth.DTOs.TransactionInput
        {
            To = destination,
        });
    }

    public async Task<decimal> GetBalance(string destination)
    {
        try
        {
            var client = new RpcClient(new Uri(_baseUrl));
            var request = new Nethereum.RPC.Eth.EthGetBalance(client);
            var result = await request.SendRequestAsync(destination);

            var etherAmount = Nethereum.Util.UnitConversion.Convert.FromWei(result.Value);

            return etherAmount;
        }
        catch
        {
            return 0;
        }
    }
}
