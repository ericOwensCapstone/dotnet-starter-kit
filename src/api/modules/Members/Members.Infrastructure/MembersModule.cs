
using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Members.Infrastructure.Persistence;

namespace FSH.Starter.WebApi.Members.Infrastructure;
public static class MembersModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("members") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {

        }
    }
    public static WebApplicationBuilder RegisterMembersServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<MembersDbContext>();
        builder.Services.AddScoped<IDbInitializer, MembersDbInitializer>();

        return builder;
    }
    public static WebApplication UseMembersModule(this WebApplication app)
    {
        return app;
    }
}
