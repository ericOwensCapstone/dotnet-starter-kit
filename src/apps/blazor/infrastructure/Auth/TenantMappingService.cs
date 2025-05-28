using System.Security.Claims;
using FSH.Starter.Blazor.Infrastructure.Api;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Infrastructure.Auth;

public interface ITenantMappingService
{
    Task<string?> GetTenantIdForUserAsync(ClaimsPrincipal user);
}

public class TenantMappingService : ITenantMappingService
{
    private readonly IApiClient _apiClient;
    
    public TenantMappingService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }
    
    public async Task<string?> GetTenantIdForUserAsync(ClaimsPrincipal user)
    {
        // Option 1: Check if tenant ID is in claims (custom claim from Entra ID)
        var tenantClaim = user.FindFirst("tenant_id") ?? user.FindFirst("tid");
        if (tenantClaim != null)
        {
            return tenantClaim.Value;
        }
        
        // Option 2: Map by email domain
        var email = user.FindFirst(ClaimTypes.Email)?.Value 
                    ?? user.FindFirst("preferred_username")?.Value
                    ?? user.FindFirst("email")?.Value;
                    
        if (!string.IsNullOrEmpty(email))
        {
            // You could implement domain-to-tenant mapping here
            // For example: @company.com -> company-tenant-id
            
            // For now, use a default tenant or prompt user to select
            // This is where you'd typically call an API to get the user's tenant
            try
            {
                // Example: Get tenant by user email
                // var tenant = await _apiClient.GetTenantByUserEmailAsync(email);
                // return tenant?.Id;
                
                // For development, return root tenant
                return "root";
            }
            catch
            {
                // Handle error
            }
        }
        
        return null;
    }
}