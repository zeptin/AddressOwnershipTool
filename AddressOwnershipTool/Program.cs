using AddressOwnershipTool;
using AddressOwnershipTool.Commands;
using AddressOwnershipTool.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string ExplorerBaseUrl = "https://stratissnapshotapi.stratisplatform.com/";

using IHost host = CreateHostBuilder(args).Build();

using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    await services.GetRequiredService<App>().Run(args);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            services.AddTransient<INodeApiClient>(provider => new NodeApiClient(ExplorerBaseUrl));
            services.AddTransient<INodeApiClientFactory, NodeApiClientFactory>();
            services.AddTransient<IAddressOwnershipServiceFactory, AddressOwnershipServiceFactory>();
            services.AddTransient<ISwapExtractionServiceFactory, SwapExtractionServiceFactory>();
            services.AddScoped<IBlockExplorerClient, BlockExplorerClient>();
            services.AddScoped<IEthRpcClientFactory, EthRpcClientFactory>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(App).Assembly));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssembly(typeof(App).Assembly);
            services.AddSingleton<App>();
        });
}

