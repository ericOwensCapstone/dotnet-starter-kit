using Carter;
using FSH.WebApi.Modules.LifecycleStageCatalog.Features.LifecycleStages.LifecycleStageCreation.v1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.WebApi.Modules.LifecycleStageCatalog;

public static class LifecycleStageCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("lifecycleStagecatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var lifecycleStageGroup = app.MapGroup("lifecycleStages").WithTags("lifecycleStages");
            lifecycleStageGroup.MapLifecycleStageCreationEndpoint();

            var testGroup = app.MapGroup("test").WithTags("test");
            testGroup.MapGet("/test", () => "hi");
        }
    }
    public static WebApplicationBuilder RegisterLifecycleStageCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder;
    }
    public static WebApplication UseLifecycleStageCatalogModule(this WebApplication app)
    {
        return app;
    }
}

