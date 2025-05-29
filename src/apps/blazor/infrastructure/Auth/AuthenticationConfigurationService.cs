using Microsoft.Extensions.Configuration;

namespace FSH.Starter.Blazor.Infrastructure.Auth;

public interface IAuthenticationConfigurationService
{
    AuthenticationProvider GetProvider();
    bool IsAzureB2C();
}

public class AuthenticationConfigurationService : IAuthenticationConfigurationService
{
    private readonly IConfiguration _configuration;

    public AuthenticationConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AuthenticationProvider GetProvider()
    {
        var provider = _configuration["AuthenticationOptions:Provider"];
        return Enum.TryParse<AuthenticationProvider>(provider, out var result) 
            ? result 
            : AuthenticationProvider.Local;
    }

    public bool IsAzureB2C() => GetProvider() == AuthenticationProvider.AzureAdB2C;
}

public enum AuthenticationProvider
{
    Local,
    AzureAdB2C
}