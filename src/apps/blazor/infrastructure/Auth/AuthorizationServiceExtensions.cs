using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FSH.Starter.Blazor.Infrastructure.Auth;

public static class AuthorizationServiceExtensions
{
    public static async Task<bool> HasPermissionAsync(this IAuthorizationService service, ClaimsPrincipal user, string action, string resource)
    {
        var name = FshPermission.NameFor(action, resource);
        var firstResult = await service.AuthorizeAsync(user, null, name);
        var finalResult = firstResult.Succeeded;
        return finalResult;
        //return (await service.AuthorizeAsync(user, null, FshPermission.NameFor(action, resource))).Succeeded;
    }
}
