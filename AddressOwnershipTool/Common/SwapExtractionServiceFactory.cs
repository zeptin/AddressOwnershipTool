using System.Globalization;
using System.Text;
using AddressOwnershipTool.Commands.Update;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Server.IIS.Core;
using NBitcoin;
using Stratis.Bitcoin.Controllers.Models;
using Stratis.Bitcoin.Features.BlockStore.Models;

namespace AddressOwnershipTool.Common;

public sealed class SwapExtractionServiceFactory : ISwapExtractionServiceFactory
{
    private readonly INodeApiClientFactory nodeApiClientFactory;

    public SwapExtractionServiceFactory(INodeApiClientFactory nodeApiClientFactory)
    {
        this.nodeApiClientFactory = nodeApiClientFactory;
    }

    public ISwapExtractionService CreateSwapExtractionServiceFactory(bool testnet, bool useCirrus = false)
    {
        return new SwapExtractionService(nodeApiClientFactory, testnet, useCirrus);
    }
}
