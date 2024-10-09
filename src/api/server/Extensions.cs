using System.Reflection;
using Asp.Versioning.Conventions;
using Carter;
using FluentValidation;
using FSH.Starter.WebApi.Catalog.Application;
using FSH.Starter.WebApi.Catalog.Infrastructure;
using FSH.Starter.WebApi.Todo;
using FSH.Starter.WebApi.RationCatalog.Infrastructure;
using FSH.Starter.WebApi.RationCatalog.Application;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Application;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Infrastructure;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Infrastructure;
using FSH.Starter.WebApi.LifecycleStageCatalog.Application;
using FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Application;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Infrastructure;

namespace FSH.Starter.WebApi.Host;

public static class Extensions
{
    public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        //TODO NEWMODULE register new modules
        //define module assemblies
        var assemblies = new Assembly[]
        {
            typeof(CatalogMetadata).Assembly,
            typeof(RationCatalogMetadata).Assembly,
            typeof(GrowthTreatmentCatalogMetadata).Assembly,
            typeof(PreventativeTreatmentCatalogMetadata).Assembly,
            typeof(LifecycleStageCatalogMetadata).Assembly,
            typeof(LifecycleProgramCatalogMetadata).Assembly,
            typeof(TodoModule).Assembly
        };

        //register validators
        builder.Services.AddValidatorsFromAssemblies(assemblies);



        // TODO MORENEWMODULE register new module
        //register module services
        builder.RegisterCatalogServices();
        builder.RegisterTodoServices();
        builder.RegisterRationCatalogServices();
        builder.RegisterGrowthTreatmentCatalogServices();
        builder.RegisterPreventativeTreatmentCatalogServices();
        builder.RegisterLifecycleStageCatalogServices();
        builder.RegisterLifecycleProgramCatalogServices();




        //register mediatr
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });


        // TODO EVENMORENEWMODULE Add Carter module
        //add carter endpoint modules
        builder.Services.AddCarter(configurator: config =>
        {
            config.WithModule<CatalogModule.Endpoints>();
            config.WithModule<TodoModule.Endpoints>();
            config.WithModule<RationCatalogModule.Endpoints>();
            config.WithModule<GrowthTreatmentCatalogModule.Endpoints>();
            config.WithModule<PreventativeTreatmentCatalogModule.Endpoints>();
            config.WithModule<LifecycleStageCatalogModule.Endpoints>();
            config.WithModule<LifecycleProgramCatalogModule.Endpoints>();
        });

        return builder;
    }

    public static WebApplication UseModules(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        //register modules
        app.UseCatalogModule();
        app.UseTodoModule();

        //TODO REGISTERMODULES
        app.UseRationCatalogModule();
        app.UseGrowthTreatmentCatalogModule();
        app.UsePreventativeTreatmentCatalogModule();
        app.UseLifecycleStageCatalogModule();
        app.UseLifecycleProgramCatalogModule();

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
