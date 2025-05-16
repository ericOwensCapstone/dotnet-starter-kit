using System.Reflection;
using Asp.Versioning.Conventions;
using Carter;
using FluentValidation;
using FSH.Starter.WebApi.Catalog.Application;
using FSH.Starter.WebApi.Catalog.Infrastructure;
using FSH.Starter.WebApi.Ranch.Application;
using FSH.Starter.WebApi.Ranch.Infrastructure;
using FSH.Starter.WebApi.Todo;
// Start Module Usings
// End Module Usings
namespace FSH.Starter.WebApi.Host;

public static class Extensions
{
    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        //define module assemblies
        var assemblies = new Assembly[]
        {
            typeof(CatalogMetadata).Assembly,
            typeof(RanchMetadata).Assembly,
            // Start Assemblies
            // End Assemblies
            typeof(TodoModule).Assembly
        };

        //register validators
        builder.Services.AddValidatorsFromAssemblies(assemblies);

        //register mediatr
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });

        //register module services
        // Start Module Registrations
        // End Module Registrations
        builder.RegisterCatalogServices();
        builder.RegisterRanchServices();
        builder.RegisterTodoServices();

        //add carter endpoint modules
        builder.Services.AddCarter(configurator: config =>
        {
            // Start Carter Modules
            // End Carter Modules
            config.WithModule<CatalogModule.Endpoints>();
            config.WithModule<RanchModule.Endpoints>();
            config.WithModule<TodoModule.Endpoints>();
        });

        return builder;
    }

    public static WebApplication UseModules(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        //register modules
        // Start Use Modules
        // End Use Modules
        app.UseCatalogModule();
        app.UseRanchModule();
        app.UseTodoModule();

        //register api versions
        var versions = app.NewApiVersionSet()
                    .HasApiVersion(1)
                    .HasApiVersion(2)
                    .ReportApiVersions()
                    .Build();

        //map versioned endpoint
        var endpoints = app.MapGroup("api/v{version:apiVersion}").WithApiVersionSet(versions);

        //use carter
        endpoints.MapCarter();

        return app;
    }
}
