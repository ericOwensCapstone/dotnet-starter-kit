using FSH.Starter.Blazor.Infrastructure.Auth;
using System.Security.Claims;

namespace FSH.Starter.Blazor.Infrastructure.Api;

public interface IApiClientExtensions
{
    Task<UserInfo?> GetCurrentUserInfoAsync(ClaimsPrincipal principal);
}

public partial class ApiClient : IApiClientExtensions
{
    public async Task<UserInfo?> GetCurrentUserInfoAsync(ClaimsPrincipal principal)
    {
        try
        {
            // Get user ID from claims
            var userId = principal.FindFirst("sub")?.Value ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            // Get user permissions
            var permissions = await GetUserPermissionsAsync();
            
            return new UserInfo
            {
                UserId = userId,
                Name = principal.FindFirst("name")?.Value ?? principal.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown",
                Email = principal.FindFirst("email")?.Value ?? principal.FindFirst(ClaimTypes.Email)?.Value,
                TenantId = principal.FindFirst("tenant")?.Value,
                Permissions = permissions?.ToList() ?? new List<string>()
            };
        }
        catch (Exception)
        {
            return null;
        }
    }
}