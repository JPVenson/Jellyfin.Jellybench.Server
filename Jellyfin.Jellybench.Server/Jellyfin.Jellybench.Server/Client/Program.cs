using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Jellyfin.Jellybench.Server.Client;
using Jellyfin.Jellybench.Server.Client.Api;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Jellyfin.Jellybench.Server.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddScoped<JellybenchApiClient>(sp =>
            new JellybenchApiClient(builder.HostEnvironment.BaseAddress, sp.GetRequiredService<HttpClient>()))
            .AddBlazorise(options =>
            {
                options.Immediate = true;
            })
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
        await builder.Build().RunAsync();
    }
}