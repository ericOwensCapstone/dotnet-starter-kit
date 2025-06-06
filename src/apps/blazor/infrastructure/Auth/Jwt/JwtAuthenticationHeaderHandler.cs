using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace FSH.Starter.Blazor.Infrastructure.Auth.Jwt;
public class JwtAuthenticationHeaderHandler : DelegatingHandler
{
    private readonly IAccessTokenProviderAccessor _tokenProviderAccessor;
    private readonly NavigationManager _navigation;
    private readonly IServiceProvider _serviceProvider;

    public JwtAuthenticationHeaderHandler(IAccessTokenProviderAccessor tokenProviderAccessor, NavigationManager navigation, IServiceProvider serviceProvider)
    {
        _tokenProviderAccessor = tokenProviderAccessor;
        _navigation = navigation;
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // skip token endpoints
        if (request.RequestUri?.AbsolutePath.Contains("/token") is not true)
        {
            Console.WriteLine($"JwtAuthenticationHeaderHandler: Processing request to {request.RequestUri?.AbsolutePath}");
            
            if (await _tokenProviderAccessor.TokenProvider.GetAccessTokenAsync() is string token)
            {
                Console.WriteLine($"JwtAuthenticationHeaderHandler: Token found, adding to request for {request.RequestUri?.AbsolutePath}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Console.WriteLine($"JwtAuthenticationHeaderHandler: NO TOKEN found for {request.RequestUri?.AbsolutePath}");
                
                // Check if we're in B2C mode and currently processing authentication
                var authConfig = _serviceProvider.GetService<IAuthenticationConfigurationService>();
                if (authConfig?.IsAzureB2C() == true)
                {
                    // For B2C, don't redirect if we're already in an auth flow
                    var currentUri = _navigation.Uri;
                    Console.WriteLine($"JwtAuthenticationHeaderHandler: B2C mode, current URI: {currentUri}");
                    
                    if (currentUri.Contains("/authentication/") || 
                        currentUri.Contains("access_token") || 
                        currentUri.Contains("id_token") ||
                        currentUri.Contains("/accept-invitation"))
                    {
                        Console.WriteLine($"JwtAuthenticationHeaderHandler: Authentication in progress, continuing without token for {request.RequestUri?.AbsolutePath}");
                        // Authentication in progress, continue without token
                        return await base.SendAsync(request, cancellationToken);
                    }
                }
                
                Console.WriteLine($"JwtAuthenticationHeaderHandler: REDIRECTING to login from {request.RequestUri?.AbsolutePath}");
                _navigation.NavigateTo("/login");
            }
        }
        else
        {
            Console.WriteLine($"JwtAuthenticationHeaderHandler: Skipping token endpoint: {request.RequestUri?.AbsolutePath}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}