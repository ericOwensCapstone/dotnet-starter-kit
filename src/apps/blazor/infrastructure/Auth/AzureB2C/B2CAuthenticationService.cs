using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Infrastructure.Storage;
using FSH.Starter.Blazor.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;

namespace FSH.Starter.Blazor.Infrastructure.Auth.AzureB2C;

public class B2CAuthenticationService : AuthenticationStateProvider, IAuthenticationService, IAccessTokenProvider
{
    private readonly NavigationManager _navigation;
    private readonly ILocalStorageService _localStorage;
    private readonly IApiClient _apiClient;
    private readonly IOptions<B2CAuthenticationOptions> _options;
    private readonly HttpClient _httpClient;

    public B2CAuthenticationService(
        NavigationManager navigation,
        ILocalStorageService localStorage,
        IApiClient apiClient,
        IOptions<B2CAuthenticationOptions> options,
        HttpClient httpClient)
    {
        _navigation = navigation;
        _localStorage = localStorage;
        _apiClient = apiClient;
        _options = options;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
        
        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        
        return new AuthenticationState(user);
    }

    public async Task<bool> LoginAsync(string tenantId, TokenGenerationCommand request)
    {
        // B2C uses redirect-based flow, not direct login
        // Redirect to B2C login
        var b2cOptions = _options.Value;
        var redirectUri = new Uri(_navigation.BaseUri).GetLeftPart(UriPartial.Authority) + "/authentication/login-callback";
        var loginUrl = $"{b2cOptions.Authority}/oauth2/v2.0/authorize" +
            $"?client_id={b2cOptions.ClientId}" +
            $"&response_type=id_token token" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            $"&scope={Uri.EscapeDataString(string.Join(" ", b2cOptions.DefaultScopes))}" +
            $"&response_mode=fragment" +
            $"&nonce={Guid.NewGuid()}" +
            $"&state={Guid.NewGuid()}";

        _navigation.NavigateTo(loginUrl, true);
        return false; // Login will complete after redirect
    }

    public void NavigateToExternalLogin(string returnUrl)
    {
        // For B2C, navigate to login which will redirect to B2C
        _navigation.NavigateTo($"/auth/login?returnUrl={Uri.EscapeDataString(returnUrl)}", true);
    }

    public async Task ReLoginAsync(string returnUrl)
    {
        // For B2C, just navigate to login
        await LoginAsync(string.Empty, new TokenGenerationCommand());
    }

    public async Task<bool> ExchangeB2CTokenAsync(string b2cToken)
    {
        try
        {
            Console.WriteLine($"Attempting to exchange B2C token...");
            
            // Exchange B2C token for local JWT
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", b2cToken);
            var response = await _httpClient.PostAsync("/b2c/token", null);
            
            Console.WriteLine($"Token exchange response status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Token exchange response content: {content}");
                
                var tokenResponse = JsonSerializer.Deserialize<FSH.Starter.Blazor.Shared.TokenResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
                {
                    Console.WriteLine("Successfully received token response, storing in local storage");
                    await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, tokenResponse.Token);
                    await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, tokenResponse.RefreshToken);
                    
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                    return true;
                }
                else
                {
                    Console.WriteLine("Token response was null or invalid");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Token exchange failed with status {response.StatusCode}: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exchanging B2C token: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        return false;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.Permissions);
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
        
        // Redirect to B2C logout
        var b2cOptions = _options.Value;
        var logoutUrl = $"{b2cOptions.Authority}/oauth2/v2.0/logout" +
            $"?post_logout_redirect_uri={Uri.EscapeDataString(_navigation.BaseUri)}";
            
        _navigation.NavigateTo(logoutUrl, true);
    }

    public async Task<bool> RefreshTokenAsync()
    {
        var authToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
        var refreshToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(authToken))
            return false;

        try
        {
            // For B2C, we need tenant information - for now use empty string
            var command = new FSH.Starter.Blazor.Infrastructure.Api.RefreshTokenCommand 
            { 
                Token = authToken,
                RefreshToken = refreshToken 
            };
            var response = await _apiClient.RefreshTokenEndpointAsync(string.Empty, command);
            
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, response.Token);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, response.RefreshToken);
            
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async ValueTask<AccessTokenResult> RequestAccessToken()
    {
        var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
        
        if (!string.IsNullOrEmpty(token))
        {
            return new AccessTokenResult(
                AccessTokenResultStatus.Success,
                new AccessToken { Value = token },
                null);
        }

        return new AccessTokenResult(
            AccessTokenResultStatus.RequiresRedirect,
            null,
            "/authentication/login");
    }

    public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
    {
        return RequestAccessToken();
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        
        return keyValuePairs?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)) 
            ?? Enumerable.Empty<Claim>();
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}

public class B2CAuthenticationOptions
{
    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public bool ValidateAuthority { get; set; } = true;
    public List<string> DefaultScopes { get; set; } = new();
}