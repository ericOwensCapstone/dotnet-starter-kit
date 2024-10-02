using Carter;
using FSH.WebApi.Modules.GrowthTreatmentCatalog.Features.GrowthTreatments.GrowthTreatmentCreation.v1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.WebApi.Modules.GrowthTreatmentCatalog;

public static class GrowthTreatmentCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("growthTreatmentcatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var growthTreatmentGroup = app.MapGroup("growthTreatments").WithTags("growthTreatments");
            growthTreatmentGroup.MapGrowthTreatmentCreationEndpoint();

            var testGroup = app.MapGroup("test").WithTags("test");
            testGroup.MapGet("/test", () => "hi");
        }
    }
    public static WebApplicationBuilder RegisterGrowthTreatmentCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder;
    }
    public static WebApplication UseGrowthTreatmentCatalogModule(this WebApplication app)
    {
        return app;
    }
}

