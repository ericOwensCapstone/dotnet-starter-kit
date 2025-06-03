using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Invitations.Endpoints;

public static class GetInvitationsByTenantEndpoint
{
    public static void MapGetInvitationsByTenantEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("invitations", GetInvitationsByTenant)
            .WithName(nameof(GetInvitationsByTenant))
            .WithSummary("Get invitations for a tenant")
            .WithDescription("Gets all invitations for the current tenant or a specific tenant (admin only)")
            .Produces<List<UserInvitation>>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequirePermission(FshPermission.NameFor(FshActions.View, FshResources.UserInvitations))
            .WithTags("Invitations");
    }

    private static async Task<IResult> GetInvitationsByTenant(
        IInvitationService invitationService,
        ICurrentUser currentUser,
        string? tenantId,
        CancellationToken cancellationToken)
    {
        // If no tenantId specified, use current user's tenant
        var targetTenantId = tenantId ?? currentUser.GetTenant();
        
        // Check if user has permission to view invitations for other tenants
        if (!string.IsNullOrEmpty(tenantId) && tenantId != currentUser.GetTenant())
        {
            if (!currentUser.IsInRole(FshRoles.Admin))
            {
                return Results.Forbid();
            }
        }

        var invitations = await invitationService.GetInvitationsByTenantAsync(targetTenantId!, cancellationToken);
        return Results.Ok(invitations);
    }
}