using Microsoft.Extensions.Configuration;

namespace FSH.Starter.Blazor.Infrastructure.Auth;

public interface IAuthenticationConfigurationService
{
    bool IsAzureB2C();
}

public class AuthenticationConfigurationService : IAuthenticationConfigurationService
{
    private readonly IConfiguration _configuration;

    public AuthenticationConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsAzureB2C() => true; // Always use B2C
}