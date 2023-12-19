using AddressOwnershipTool.Commands;
using AddressOwnershipTool.Common;
using AddressOwnershipTool;
using AddressOwnershipTool.Web.Services;
using FASTER.core;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddControllersWithViews();

var secret = builder.Configuration["JwtConfig:Secret"];
var key = Encoding.ASCII.GetBytes(secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

const string ExplorerBaseUrl = "https://stratissnapshotapi.stratisplatform.com/";

builder.Services.AddTransient<INonceService, NonceService>();
builder.Services.AddSingleton<ITokenService>(new TokenService(secret));

builder.Services.AddTransient<INodeApiClient>(provider => new NodeApiClient(ExplorerBaseUrl));
builder.Services.AddTransient<INodeApiClientFactory, NodeApiClientFactory>();
builder.Services.AddTransient<IAddressOwnershipServiceFactory, AddressOwnershipServiceFactory>();
builder.Services.AddTransient<ISwapExtractionServiceFactory, SwapExtractionServiceFactory>();
builder.Services.AddScoped<IBlockExplorerClient, BlockExplorerClient>();
builder.Services.AddScoped<IEthRpcClientFactory, EthRpcClientFactory>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(App).Assembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(typeof(App).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();