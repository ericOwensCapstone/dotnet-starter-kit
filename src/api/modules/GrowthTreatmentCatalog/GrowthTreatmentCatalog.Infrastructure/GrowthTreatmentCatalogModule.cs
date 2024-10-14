using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Infrastructure.Endpoints.v1;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Infrastructure;
public static class GrowthTreatmentCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("growthTreatmentcatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var growthTreatmentGroup = app.MapGroup("growthTreatments").WithTags("growthTreatments");
            growthTreatmentGroup.MapGrowthTreatmentCreationEndpoint();
            growthTreatmentGroup.MapGetGrowthTreatmentEndpoint();
            growthTreatmentGroup.MapGetGrowthTreatmentListEndpoint();
            growthTreatmentGroup.MapGrowthTreatmentUpdateEndpoint();
            growthTreatmentGroup.MapGrowthTreatmentDeleteEndpoint();
        }
    }
    public static WebApplicationBuilder RegisterGrowthTreatmentCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<SharedDbContext>();
        builder.Services.AddKeyedScoped<IRepository<GrowthTreatment>, GrowthTreatmentCatalogRepository<GrowthTreatment>>("growthTreatmentcatalog:growthTreatments");
        builder.Services.AddKeyedScoped<IReadRepository<GrowthTreatment>, GrowthTreatmentCatalogRepository<GrowthTreatment>>("growthTreatmentcatalog:growthTreatments");
        return builder;
    }
    public static WebApplication UseGrowthTreatmentCatalogModule(this WebApplication app)
    {
        return app;
    }
}

