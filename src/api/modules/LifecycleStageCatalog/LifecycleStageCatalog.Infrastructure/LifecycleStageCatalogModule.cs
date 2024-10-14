using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Endpoints.v1;
using FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure;
public static class LifecycleStageCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("lifecycleStagecatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var lifecycleStageGroup = app.MapGroup("lifecycleStages").WithTags("lifecycleStages");
            lifecycleStageGroup.MapLifecycleStageCreationEndpoint();
            lifecycleStageGroup.MapGetLifecycleStageEndpoint();
            lifecycleStageGroup.MapGetLifecycleStageListEndpoint();
            lifecycleStageGroup.MapLifecycleStageUpdateEndpoint();
            lifecycleStageGroup.MapLifecycleStageDeleteEndpoint();
        }
    }
    public static WebApplicationBuilder RegisterLifecycleStageCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<SharedDbContext>();
        builder.Services.AddScoped<IDbInitializer, LifecycleStageCatalogDbInitializer>();
        builder.Services.AddKeyedScoped<IComponentRepository<LifecycleStage>, LifecycleStageComponentRepository<LifecycleStage>>("lifecycleStagecatalog:lifecycleStages");
        builder.Services.AddKeyedScoped<IRepository<LifecycleStage>, LifecycleStageCatalogRepository<LifecycleStage>>("lifecycleStagecatalog:lifecycleStages");
        builder.Services.AddKeyedScoped<IReadRepository<LifecycleStage>, LifecycleStageCatalogRepository<LifecycleStage>>("lifecycleStagecatalog:lifecycleStages");
        return builder;
    }
    public static WebApplication UseLifecycleStageCatalogModule(this WebApplication app)
    {
        return app;
    }
}

