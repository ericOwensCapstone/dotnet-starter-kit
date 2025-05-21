using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Ranch.Infrastructure.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.Rations;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.Rations;
// Start MemberPage;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.MemberPages;
// End MemberPage;
// Start ContractStatus;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.ContractStatuses;
// End ContractStatus;
// Start Contract;
using FSH.Starter.WebApi.Ranch.Domain.Contracts;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.Contracts;
// End Contract;
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
            // Start MemberPage;
            var memberPageGroup = app.MapGroup("memberPages").WithTags("memberPages");
            memberPageGroup.MapMemberPageCreationEndpoint();
            memberPageGroup.MapGetMemberPageEndpoint();
            memberPageGroup.MapGetMemberPageListEndpoint();
            memberPageGroup.MapMemberPageUpdateEndpoint();
            memberPageGroup.MapMemberPageDeleteEndpoint();
            // End MemberPage;
            // Start ContractStatus;
            var contractStatusGroup = app.MapGroup("contractStatuses").WithTags("contractStatuses");
            contractStatusGroup.MapContractStatusCreationEndpoint();
            contractStatusGroup.MapGetContractStatusEndpoint();
            contractStatusGroup.MapGetContractStatusListEndpoint();
            contractStatusGroup.MapContractStatusUpdateEndpoint();
            contractStatusGroup.MapContractStatusDeleteEndpoint();
            // End ContractStatus;
            // Start Contract;
            var contractGroup = app.MapGroup("contracts").WithTags("contracts");
            contractGroup.MapContractCreationEndpoint();
            contractGroup.MapGetContractEndpoint();
            contractGroup.MapGetContractListEndpoint();
            contractGroup.MapContractUpdateEndpoint();
            contractGroup.MapContractDeleteEndpoint();
            // End Contract;
        }
    }
    public static WebApplicationBuilder RegisterRanchServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<RanchDbContext>();
        builder.Services.AddScoped<IDbInitializer, RanchDbInitializer>();
        builder.Services.AddKeyedScoped<IRepository<Ration>, RanchRepository<Ration>>("ranch:rations");
        builder.Services.AddKeyedScoped<IReadRepository<Ration>, RanchRepository<Ration>>("ranch:rations");
        // Start MemberPage;
        builder.Services.AddKeyedScoped<IRepository<MemberPage>, RanchRepository<MemberPage>>("ranch:memberPages");
        builder.Services.AddKeyedScoped<IReadRepository<MemberPage>, RanchRepository<MemberPage>>("ranch:memberPages");
        // End MemberPage;
        // Start ContractStatus;
        builder.Services.AddKeyedScoped<IRepository<ContractStatus>, RanchRepository<ContractStatus>>("ranch:contractStatuses");
        builder.Services.AddKeyedScoped<IReadRepository<ContractStatus>, RanchRepository<ContractStatus>>("ranch:contractStatuses");
        // End ContractStatus;
        // Start Contract;
        builder.Services.AddKeyedScoped<IRepository<Contract>, RanchRepository<Contract>>("ranch:contracts");
        builder.Services.AddKeyedScoped<IReadRepository<Contract>, RanchRepository<Contract>>("ranch:contracts");
        // End Contract;
        return builder;
    }
    public static WebApplication UseRanchModule(this WebApplication app)
    {
        return app;
    }
}

