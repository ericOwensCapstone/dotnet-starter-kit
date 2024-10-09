using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Infrastructure.Endpoints.v1;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Infrastructure.Persistence;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Infrastructure;
public static class LifecycleProgramCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("lifecycleProgramcatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var lifecycleProgramGroup = app.MapGroup("lifecyclePrograms").WithTags("lifecyclePrograms");
            lifecycleProgramGroup.MapLifecycleProgramCreationEndpoint();
            lifecycleProgramGroup.MapGetLifecycleProgramEndpoint();
            lifecycleProgramGroup.MapGetLifecycleProgramListEndpoint();
            lifecycleProgramGroup.MapLifecycleProgramUpdateEndpoint();
            lifecycleProgramGroup.MapLifecycleProgramDeleteEndpoint();
        }
    }
    public static WebApplicationBuilder RegisterLifecycleProgramCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<LifecycleProgramCatalogDbContext>();
        builder.Services.AddScoped<IDbInitializer, LifecycleProgramCatalogDbInitializer>();
        builder.Services.AddKeyedScoped<IComponentRepository<LifecycleProgram>, LifecycleProgramComponentRepository<LifecycleProgram>>("lifecycleProgramcatalog:lifecyclePrograms");
        builder.Services.AddKeyedScoped<IRepository<LifecycleProgram>, LifecycleProgramCatalogRepository<LifecycleProgram>>("lifecycleProgramcatalog:lifecyclePrograms");
        builder.Services.AddKeyedScoped<IReadRepository<LifecycleProgram>, LifecycleProgramCatalogRepository<LifecycleProgram>>("lifecycleProgramcatalog:lifecyclePrograms");
        return builder;
    }
    public static WebApplication UseLifecycleProgramCatalogModule(this WebApplication app)
    {
        return app;
    }
}

