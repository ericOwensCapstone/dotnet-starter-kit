using FSH.Starter.Blazor.Client;
using FSH.Starter.Blazor.Infrastructure;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

try
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
    
    Console.WriteLine("Program.cs: Configuring services...");
    builder.Services.AddClientServices(builder.Configuration);
    
    Console.WriteLine("Program.cs: Building and running...");
    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Program.cs: Fatal error during startup: {ex}");
    throw;
}
