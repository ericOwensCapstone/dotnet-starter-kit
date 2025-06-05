using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Core.Identity.Invitations.Features;
using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Identity.Invitations.Endpoints;

public static class CreateInvitationEndpoint
{
    public static void MapCreateInvitationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("invitations", CreateInvitation)
            .WithName(nameof(CreateInvitation))
            .WithSummary("Create a new user invitation")
            .WithDescription("Creates an invitation for a new user to join a tenant")
            .Produces<CreateInvitationResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .RequirePermission(FshPermission.NameFor(FshActions.Create, FshResources.UserInvitations))
            .WithTags("Invitations");
    }

    private static async Task<IResult> CreateInvitation(
        CreateInvitationRequest request,
        IInvitationService invitationService,
        ICurrentUser currentUser,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return Results.BadRequest("Request cannot be null");
        }
        
        var logger = loggerFactory.CreateLogger("CreateInvitationEndpoint");
        logger.LogInformation("Processing invitation request for {Email} to target tenant {TargetTenantId}", 
            request.Email, request.TargetTenantId);

        // Check permissions - user must be able to create users in the target tenant
        var currentUserTenant = currentUser.GetTenant();

        // If inviting to a different tenant, user must have root permissions
        if (request.TargetTenantId != currentUserTenant)
        {
            if (!currentUser.IsInRole(FshRoles.Admin))
            {
                return Results.Forbid();
            }
        }

        // Call the invitation service
        var response = await invitationService.CreateInvitationAsync(request, cancellationToken);
        return Results.Ok(response);
    }
}