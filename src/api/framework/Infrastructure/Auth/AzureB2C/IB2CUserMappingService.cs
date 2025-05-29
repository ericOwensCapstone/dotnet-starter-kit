using System.Security.Claims;
using FSH.Framework.Infrastructure.Identity.Users;

namespace FSH.Framework.Infrastructure.Auth.AzureB2C;

public interface IB2CUserMappingService
{
    Task<FshUser> GetOrCreateUserFromB2CClaimsAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
    Task<List<Claim>> GetUserClaimsAsync(FshUser user, CancellationToken cancellationToken = default);
}