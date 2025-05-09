
using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Members.Infrastructure.Persistence;

// Start MemberAd
using FSH.Starter.WebApi.Members.Domain.MemberAds;
using FSH.Starter.WebApi.Members.Infrastructure.Endpoints.v1.MemberAds;
// End MemberAd
namespace FSH.Starter.WebApi.Members.Infrastructure;
public static class MembersModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("members") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {

            // Start MemberAd
            var memberAdGroup = app.MapGroup("memberAds").WithTags("memberAds");
            memberAdGroup.MapMemberAdCreationEndpoint();
            memberAdGroup.MapGetMemberAdEndpoint();
            memberAdGroup.MapGetMemberAdListEndpoint();
            memberAdGroup.MapMemberAdUpdateEndpoint();
            memberAdGroup.MapMemberAdDeleteEndpoint();
            // End MemberAd
        }
    }
    public static WebApplicationBuilder RegisterMembersServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<MembersDbContext>();
        builder.Services.AddScoped<IDbInitializer, MembersDbInitializer>();

        // Start MemberAd
        builder.Services.AddKeyedScoped<IRepository<MemberAd>, MembersRepository<MemberAd>>("members:memberAds");
        builder.Services.AddKeyedScoped<IReadRepository<MemberAd>, MembersRepository<MemberAd>>("members:memberAds");
        // End MemberAd
        return builder;
    }
    public static WebApplication UseMembersModule(this WebApplication app)
    {
        return app;
    }
}
