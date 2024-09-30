using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure;
using FSH.Framework.Infrastructure.Logging.Serilog;
using FSH.Starter.WebApi.Catalog.Application.Products.Get.v1;
using FSH.Starter.WebApi.Catalog.Application.Products.Search.v1;
using FSH.Starter.WebApi.Host;
using FSH.Starter.WebApi.RationCatalog.Application.Rations.Get.v1;
using FSH.Starter.WebApi.RationCatalog.Application.Rations.Search.v1;
using MediatR;
using Serilog;

StaticLogger.EnsureInitialized();
Log.Information("server booting up..");
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureFshFramework();
    builder.RegisterModules();

    var app = builder.Build();

    //TODO this is just for debug:
    //var serviceProvider = app.Services;
    //var productHandlers = serviceProvider.GetServices(typeof(IRequestHandler<SearchProductsCommand, PagedList<ProductResponse>>));
    //var rationHandlers = serviceProvider.GetServices(typeof(IRequestHandler<SearchRationsCommand, PagedList<RationResponse>>));

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
