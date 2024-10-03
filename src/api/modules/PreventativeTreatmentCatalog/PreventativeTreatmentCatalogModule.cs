using Carter;
using FSH.WebApi.Modules.PreventativeTreatmentCatalog.Features.PreventativeTreatments.PreventativeTreatmentCreation.v1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.WebApi.Modules.PreventativeTreatmentCatalog;

public static class PreventativeTreatmentCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("preventativeTreatmentcatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var preventativeTreatmentGroup = app.MapGroup("preventativeTreatments").WithTags("preventativeTreatments");
            preventativeTreatmentGroup.MapPreventativeTreatmentCreationEndpoint();

            var testGroup = app.MapGroup("test").WithTags("test");
            testGroup.MapGet("/test", () => "hi");
        }
    }
    public static WebApplicationBuilder RegisterPreventativeTreatmentCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder;
    }
    public static WebApplication UsePreventativeTreatmentCatalogModule(this WebApplication app)
    {
        return app;
    }
}

