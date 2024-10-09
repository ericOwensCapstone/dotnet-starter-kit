using Carter;
using FSH.WebApi.Modules.LifecycleProgramCatalog.Features.LifecyclePrograms.LifecycleProgramCreation.v1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.WebApi.Modules.LifecycleProgramCatalog;

public static class LifecycleProgramCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("lifecycleProgramcatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var lifecycleProgramGroup = app.MapGroup("lifecyclePrograms").WithTags("lifecyclePrograms");
            lifecycleProgramGroup.MapLifecycleProgramCreationEndpoint();

            var testGroup = app.MapGroup("test").WithTags("test");
            testGroup.MapGet("/test", () => "hi");
        }
    }
    public static WebApplicationBuilder RegisterLifecycleProgramCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder;
    }
    public static WebApplication UseLifecycleProgramCatalogModule(this WebApplication app)
    {
        return app;
    }
}

