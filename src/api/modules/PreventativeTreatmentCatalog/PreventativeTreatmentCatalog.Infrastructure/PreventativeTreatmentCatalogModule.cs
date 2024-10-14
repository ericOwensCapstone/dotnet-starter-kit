using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Infrastructure.Endpoints.v1;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Infrastructure;
public static class PreventativeTreatmentCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("preventativeTreatmentcatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var preventativeTreatmentGroup = app.MapGroup("preventativeTreatments").WithTags("preventativeTreatments");
            preventativeTreatmentGroup.MapPreventativeTreatmentCreationEndpoint();
            preventativeTreatmentGroup.MapGetPreventativeTreatmentEndpoint();
            preventativeTreatmentGroup.MapGetPreventativeTreatmentListEndpoint();
            preventativeTreatmentGroup.MapPreventativeTreatmentUpdateEndpoint();
            preventativeTreatmentGroup.MapPreventativeTreatmentDeleteEndpoint();
        }
    }
    public static WebApplicationBuilder RegisterPreventativeTreatmentCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<SharedDbContext>();
        builder.Services.AddKeyedScoped<IRepository<PreventativeTreatment>, PreventativeTreatmentCatalogRepository<PreventativeTreatment>>("preventativeTreatmentcatalog:preventativeTreatments");
        builder.Services.AddKeyedScoped<IReadRepository<PreventativeTreatment>, PreventativeTreatmentCatalogRepository<PreventativeTreatment>>("preventativeTreatmentcatalog:preventativeTreatments");
        return builder;
    }
    public static WebApplication UsePreventativeTreatmentCatalogModule(this WebApplication app)
    {
        return app;
    }
}

