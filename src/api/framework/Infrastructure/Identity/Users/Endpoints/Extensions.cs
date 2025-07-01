using FSH.Framework.Infrastructure.Identity.Audit.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints;
internal static class Extensions
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapUpdateUserEndpoint();
        app.MapGetUsersListEndpoint();
        app.MapDeleteUserEndpoint();
        app.MapGetMeEndpoint();
        app.MapGetUserEndpoint();
        app.MapGetCurrentUserPermissionsEndpoint();
        app.ToggleUserStatusEndpointEndpoint();
        app.MapAssignRolesToUserEndpoint();
        app.MapGetUserRolesEndpoint();
        app.MapGetUserAuditTrailEndpoint();
        app.MapConfirmEmailEndpoint();
        app.MapDeleteUserCompletelyEndpoint();
        app.MapPurgeAllDeletedB2CUsersEndpoint();
        app.MapSearchUsersAcrossTenantsEndpoint();
        app.MapGetUserRolesAcrossTenantsEndpoint();
        return app;
    }
}
