using FSH.Framework.Infrastructure;
using FSH.Framework.Infrastructure.Configuration;
using FSH.Framework.Infrastructure.Identity;
using FSH.Framework.Infrastructure.Logging.Serilog;
using FSH.Starter.WebApi.Host;
using Serilog;

StaticLogger.EnsureInitialized();
Log.Information("server booting up..");
try
{
    var builder = WebApplication.CreateBuilder(args);
    
    // Add User Secrets ID for development (before KeyVault to ensure precedence)
    builder.Configuration.AddUserSecrets<Program>(optional: true, reloadOnChange: true);
    
    // Add Key Vault configuration
    builder.Configuration.AddKeyVaultSecrets(builder.Environment);
    
    builder.ConfigureFshFramework();
    builder.RegisterModules();

    var app = builder.Build();

    app.UseFshFramework();
    app.UseModules();
    
    // Start the application first, then schedule jobs in the background
    _ = Task.Run(async () =>
    {
        // Wait a bit for the application to fully start and database to be ready
        await Task.Delay(TimeSpan.FromSeconds(10));
        
        try
        {
            app.Services.ScheduleIdentityJobs();
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogWarning(ex, "Failed to schedule identity jobs during startup. Jobs will be scheduled on first use.");
        }
    });
    
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
