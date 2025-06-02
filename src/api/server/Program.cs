using FSH.Framework.Infrastructure;
using FSH.Framework.Infrastructure.Configuration;
using FSH.Framework.Infrastructure.Logging.Serilog;
using FSH.Starter.WebApi.Host;
using Serilog;

StaticLogger.EnsureInitialized();
Log.Information("server booting up..");
try
{
    var builder = WebApplication.CreateBuilder(args);
    
    // Add Key Vault configuration
    builder.Configuration.AddKeyVaultSecrets(builder.Environment);
    
    // Add User Secrets ID for development
    builder.Configuration.AddUserSecrets<Program>(optional: true, reloadOnChange: true);
    
    builder.ConfigureFshFramework();
    builder.RegisterModules();

    var app = builder.Build();

    app.UseFshFramework();
    app.UseModules();
    await app.RunAsync();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("HostAbortedException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex.Message, "unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("server shutting down..");
    await Log.CloseAndFlushAsync();
}

public partial class Program { }
