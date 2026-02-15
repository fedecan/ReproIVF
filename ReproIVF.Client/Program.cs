using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ReproIVF.Client;
using ReproIVF.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var configuredApiBaseUrl = builder.Configuration["ApiBaseUrl"]?.Trim();
var hostBaseUrl = builder.HostEnvironment.BaseAddress;
var apiBaseUrl = hostBaseUrl;

if (!string.IsNullOrWhiteSpace(configuredApiBaseUrl))
{
    if (Uri.TryCreate(configuredApiBaseUrl, UriKind.Absolute, out var absoluteConfiguredUri))
    {
        apiBaseUrl = absoluteConfiguredUri.ToString();
    }
    else if (Uri.TryCreate(hostBaseUrl, UriKind.Absolute, out var hostBaseUri)
             && Uri.TryCreate(hostBaseUri, configuredApiBaseUrl, out var resolvedConfiguredUri))
    {
        apiBaseUrl = resolvedConfiguredUri.ToString();
    }
}

if (Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var effectiveBaseUri)
    && effectiveBaseUri.Scheme.Equals(Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
{
    // Protect against opening the app as file:// (for example from local index.html)
    // where relative API calls would otherwise resolve to file:///api/* and fail.
    apiBaseUrl = "http://localhost/";
}

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl, UriKind.Absolute)
});
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<AuthService>();

var host = builder.Build();
await host.Services.GetRequiredService<AuthService>().InitializeAsync();
await host.RunAsync();
