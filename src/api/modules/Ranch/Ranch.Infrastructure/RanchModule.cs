using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.Rations;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.Rations;
using FSH.Starter.WebApi.Ranch.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Ranch.Infrastructure;
public static class RanchModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("ranch") { }
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
    public static WebApplicationBuilder RegisterRanchServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<RanchDbContext>();
        builder.Services.AddScoped<IDbInitializer, RanchDbInitializer>();
        builder.Services.AddKeyedScoped<IRepository<Ration>, RanchRepository<Ration>>("ranch:rations");
        builder.Services.AddKeyedScoped<IReadRepository<Ration>, RanchRepository<Ration>>("ranch:rations");
        return builder;
    }
    public static WebApplication UseRanchModule(this WebApplication app)
    {
        return app;
    }
}

