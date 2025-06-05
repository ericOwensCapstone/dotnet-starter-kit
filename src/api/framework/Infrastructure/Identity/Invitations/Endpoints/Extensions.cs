using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Invitations.Endpoints;

public static class Extensions
{
    public static void MapInvitationEndpoints(this IEndpointRouteBuilder builder)
    {
        var apiGroup = builder.MapGroup("api/invitations");

        apiGroup.MapSearchInvitationsEndpoint();
        apiGroup.MapCreateInvitationEndpoint();
        apiGroup.MapGetInvitationEndpoint();
        apiGroup.MapGetInvitationsByTenantEndpoint();
        apiGroup.MapResendInvitationEndpoint();
        apiGroup.MapCancelInvitationEndpoint();
        apiGroup.MapExtendInvitationExpirationEndpoint();
        apiGroup.MapAcceptInvitationEndpoint();
    }
}