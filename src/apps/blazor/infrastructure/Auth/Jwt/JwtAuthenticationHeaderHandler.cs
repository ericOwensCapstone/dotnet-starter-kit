using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Net.Http.Headers;

namespace FSH.Starter.Blazor.Infrastructure.Auth.Jwt;
public class JwtAuthenticationHeaderHandler : DelegatingHandler
{
    private readonly IAccessTokenProviderAccessor _tokenProviderAccessor;
    private readonly NavigationManager _navigation;

    public JwtAuthenticationHeaderHandler(IAccessTokenProviderAccessor tokenProviderAccessor, NavigationManager navigation)
    {
        _tokenProviderAccessor = tokenProviderAccessor;
        _navigation = navigation;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // skip token endpoints
        if (request.RequestUri?.AbsolutePath.Contains("/token") is not true)
        {
            if (await _tokenProviderAccessor.TokenProvider.GetAccessTokenAsync() is string token)
            {
                Console.WriteLine($"JwtAuthenticationHeaderHandler: Adding token to request for {request.RequestUri?.AbsolutePath}");
                Console.WriteLine($"Token preview: {token[..Math.Min(50, token.Length)]}...");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Console.WriteLine($"JwtAuthenticationHeaderHandler: No token available for {request.RequestUri?.AbsolutePath}, redirecting to login");
                _navigation.NavigateTo("/login");
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}