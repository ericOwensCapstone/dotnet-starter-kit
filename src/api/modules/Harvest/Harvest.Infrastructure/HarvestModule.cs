
using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Harvest.Infrastructure.Persistence;

// Start HarvestContractStatus;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;
using FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestContractStatuses;
// End HarvestContractStatus;
// Start HarvestMember;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;
using FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestMembers;
// End HarvestMember;
// Start HarvestContract;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;
using FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestContracts;
// End HarvestContract;
namespace FSH.Starter.WebApi.Harvest.Infrastructure;
public static class HarvestModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("harvest") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {

            // Start HarvestContractStatus;
            var harvestContractStatusGroup = app.MapGroup("harvestContractStatuses").WithTags("harvestContractStatuses");
            harvestContractStatusGroup.MapHarvestContractStatusCreationEndpoint();
            harvestContractStatusGroup.MapGetHarvestContractStatusEndpoint();
            harvestContractStatusGroup.MapGetHarvestContractStatusListEndpoint();
            harvestContractStatusGroup.MapHarvestContractStatusUpdateEndpoint();
            harvestContractStatusGroup.MapHarvestContractStatusDeleteEndpoint();
            // End HarvestContractStatus;
            // Start HarvestMember;
            var harvestMemberGroup = app.MapGroup("harvestMembers").WithTags("harvestMembers");
            harvestMemberGroup.MapHarvestMemberCreationEndpoint();
            harvestMemberGroup.MapGetHarvestMemberEndpoint();
            harvestMemberGroup.MapGetHarvestMemberListEndpoint();
            harvestMemberGroup.MapHarvestMemberUpdateEndpoint();
            harvestMemberGroup.MapHarvestMemberDeleteEndpoint();
            // End HarvestMember;
            // Start HarvestContract;
            var harvestContractGroup = app.MapGroup("harvestContracts").WithTags("harvestContracts");
            harvestContractGroup.MapHarvestContractCreationEndpoint();
            harvestContractGroup.MapGetHarvestContractEndpoint();
            harvestContractGroup.MapGetHarvestContractListEndpoint();
            harvestContractGroup.MapHarvestContractUpdateEndpoint();
            harvestContractGroup.MapHarvestContractDeleteEndpoint();
            // End HarvestContract;
        }
    }
    public static WebApplicationBuilder RegisterHarvestServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<HarvestDbContext>();
        builder.Services.AddScoped<IDbInitializer, HarvestDbInitializer>();

        // Start HarvestContractStatus;
        builder.Services.AddKeyedScoped<IRepository<HarvestContractStatus>, HarvestRepository<HarvestContractStatus>>("harvest:harvestContractStatuses");
        builder.Services.AddKeyedScoped<IReadRepository<HarvestContractStatus>, HarvestRepository<HarvestContractStatus>>("harvest:harvestContractStatuses");
        // End HarvestContractStatus;
        // Start HarvestMember;
        builder.Services.AddKeyedScoped<IRepository<HarvestMember>, HarvestRepository<HarvestMember>>("harvest:harvestMembers");
        builder.Services.AddKeyedScoped<IReadRepository<HarvestMember>, HarvestRepository<HarvestMember>>("harvest:harvestMembers");
        // End HarvestMember;
        // Start HarvestContract;
        builder.Services.AddKeyedScoped<IRepository<HarvestContract>, HarvestRepository<HarvestContract>>("harvest:harvestContracts");
        builder.Services.AddKeyedScoped<IReadRepository<HarvestContract>, HarvestRepository<HarvestContract>>("harvest:harvestContracts");
        // End HarvestContract;
        return builder;
    }
    public static WebApplication UseHarvestModule(this WebApplication app)
    {
        return app;
    }
}
