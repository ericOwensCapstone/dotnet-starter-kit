using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.Shared.Authorization;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestMembers;
public static class DeleteHarvestMemberEndpoint
{
    internal static RouteHandlerBuilder MapHarvestMemberDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator, IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor) =>
             {
                 // Validate that only root tenant can delete HarvestMembers
                 var tenantId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id;
                 if (tenantId != TenantConstants.Root.Id)
                 {
                     return Results.Forbid();
                 }
                 
                 await mediator.Send(new DeleteHarvestMemberCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteHarvestMemberEndpoint))
            .WithSummary("deletes harvestMember by id")
            .WithDescription("deletes harvestMember by id")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .RequirePermission("Permissions.HarvestMembers.Delete")
            .MapToApiVersion(1);
    }
}

