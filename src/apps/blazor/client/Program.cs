using FSH.Starter.Blazor.Client;
using FSH.Starter.Blazor.Infrastructure;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddClientServices(builder.Configuration);


var host = builder.Build();

//AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
//{
//    // Handle the exception (log it, show a message, etc.)
//    var wd = 40;
//};


await host.RunAsync();
