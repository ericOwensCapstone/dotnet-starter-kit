using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.RationCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Infrastructure.Endpoints.v1;
using FSH.Starter.WebApi.RationCatalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.RationCatalog.Infrastructure;
public static class RationCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("rationcatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var rationGroup = app.MapGroup("rations").WithTags("rations");
            rationGroup.MapRationCreationEndpoint();
            rationGroup.MapGetRationEndpoint();
            rationGroup.MapGetRationListEndpoint();
            rationGroup.MapRationUpdateEndpoint();
            rationGroup.MapRationDeleteEndpoint();
        }
    }
    public static WebApplicationBuilder RegisterRationCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<RationCatalogDbContext>();
        builder.Services.AddScoped<IDbInitializer, RationCatalogDbInitializer>();
        builder.Services.AddKeyedScoped<IRepository<Ration>, RationCatalogRepository<Ration>>("rationcatalog:rations");
        builder.Services.AddKeyedScoped<IReadRepository<Ration>, RationCatalogRepository<Ration>>("rationcatalog:rations");
        return builder;
    }
    public static WebApplication UseRationCatalogModule(this WebApplication app)
    {
        return app;
    }
}

