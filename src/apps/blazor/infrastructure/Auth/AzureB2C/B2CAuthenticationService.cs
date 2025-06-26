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
using Microsoft.Extensions.Configuration;

namespace FSH.Starter.Blazor.Infrastructure.Auth.AzureB2C;

public class B2CAuthenticationService : AuthenticationStateProvider, IAuthenticationService, IAccessTokenProvider
{
    private readonly NavigationManager _navigation;
    private readonly ILocalStorageService _localStorage;
    private readonly IApiClient _apiClient;
    private readonly IOptions<B2CAuthenticationOptions> _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public B2CAuthenticationService(
        NavigationManager navigation,
        ILocalStorageService localStorage,
        IApiClient apiClient,
        IOptions<B2CAuthenticationOptions> options,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _navigation = navigation;
        _localStorage = localStorage;
        _apiClient = apiClient;
        _options = options;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
        
        Console.WriteLine($"B2CAuthenticationService.GetAuthenticationStateAsync: Token: {(string.IsNullOrEmpty(token) ? "NULL/EMPTY" : $"[{token.Length} chars]")}");
        
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("B2CAuthenticationService.GetAuthenticationStateAsync: Returning unauthenticated state");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        
        Console.WriteLine($"B2CAuthenticationService.GetAuthenticationStateAsync: Returning authenticated state with {claims.Count()} claims");
        return new AuthenticationState(user);
    }


    public void NavigateToExternalLogin(string returnUrl, string? loginHint = null)
    {
        Console.WriteLine($"B2CAuthenticationService.NavigateToExternalLogin: returnUrl={returnUrl}, loginHint={loginHint}");
        
        var b2cSection = _configuration.GetSection("AuthenticationOptions:AzureAdB2C");
        var instance = b2cSection["Instance"];
        var domain = b2cSection["Domain"];
        var clientId = b2cSection["ClientId"];
        var apiScope = b2cSection["ApiScope"];
        
        var redirectUri = new Uri(_navigation.BaseUri).GetLeftPart(UriPartial.Authority) + "/authentication/login-callback";
        
        // Determine which policy to use based on whether we have a login hint (invitation scenario)
        string policyId;
        if (!string.IsNullOrEmpty(loginHint))
        {
            // Use the custom sign-up policy for invitations
            policyId = b2cSection["SignUpInvitationPolicyId"] ?? "B2C_1A_signup_invitation";
            Console.WriteLine($"B2CAuthenticationService: Using invitation policy: {policyId}");
        }
        else
        {
            // Use the regular sign-in policy
            policyId = b2cSection["SignInPolicyId"] ?? "B2C_1_signin";
            Console.WriteLine($"B2CAuthenticationService: Using regular sign-in policy: {policyId}");
        }
        
        // Construct the authority URL with the appropriate policy
        var authority = $"{instance}/{domain}/{policyId}";
        
        var loginUrl = $"{authority}/oauth2/v2.0/authorize" +
            $"?client_id={clientId}" +
            $"&response_type=id_token token" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            $"&scope={Uri.EscapeDataString($"openid offline_access {apiScope}")}" +
            $"&response_mode=fragment" +
            $"&nonce={Guid.NewGuid()}" +
            $"&state={Uri.EscapeDataString(returnUrl)}"; // Pass return URL in state

        // Add login_hint if provided (pre-populates email field)
        if (!string.IsNullOrEmpty(loginHint))
        {
            loginUrl += $"&login_hint={Uri.EscapeDataString(loginHint)}";
            // For custom policies, we don't add prompt parameter as it might interfere
            // The custom policy should handle the signup flow
            // Add domain_hint to skip home realm discovery
            loginUrl += "&domain_hint=live.com";
        }

        Console.WriteLine($"B2CAuthenticationService: Final login URL: {loginUrl}");
        _navigation.NavigateTo(loginUrl, true);
    }

    public async Task ReLoginAsync(string returnUrl)
    {
        // For B2C, logout and redirect to login
        await LogoutAsync();
        NavigateToExternalLogin(returnUrl);
    }

    public async Task<bool> ProcessAuthenticationCallbackAsync(string uri)
    {
        try
        {
            Console.WriteLine($"B2CAuthenticationService.ProcessAuthenticationCallbackAsync: Processing URI: {uri}");
            
            // Extract the fragment from the URI
            var uriObj = new Uri(uri);
            var fragment = uriObj.Fragment;
            
            Console.WriteLine($"B2CAuthenticationService.ProcessAuthenticationCallbackAsync: Fragment: {fragment}");
            
            if (string.IsNullOrEmpty(fragment) || fragment.Length <= 1)
            {
                Console.WriteLine("B2CAuthenticationService.ProcessAuthenticationCallbackAsync: No fragment found");
                return false;
            }
            
            // Parse the fragment to extract tokens
            var parameters = ParseFragment(fragment);
            
            Console.WriteLine($"B2CAuthenticationService.ProcessAuthenticationCallbackAsync: Parsed {parameters.Count} parameters");
            
            string? token = null;
            if (parameters.TryGetValue("id_token", out var idToken))
            {
                token = idToken;
                Console.WriteLine("B2CAuthenticationService.ProcessAuthenticationCallbackAsync: Found id_token");
            }
            else if (parameters.TryGetValue("access_token", out var accessToken))
            {
                token = accessToken;
                Console.WriteLine("B2CAuthenticationService.ProcessAuthenticationCallbackAsync: Found access_token");
            }
            
            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"B2CAuthenticationService.ProcessAuthenticationCallbackAsync: Token found, length: {token.Length}");
                return await ExchangeB2CTokenAsync(token);
            }
            else
            {
                Console.WriteLine("B2CAuthenticationService.ProcessAuthenticationCallbackAsync: No token found in fragment");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"B2CAuthenticationService.ProcessAuthenticationCallbackAsync: Error - {ex.Message}");
        }
        
        return false;
    }
    
    private Dictionary<string, string> ParseFragment(string fragment)
    {
        var result = new Dictionary<string, string>();
        
        if (fragment.StartsWith("#"))
            fragment = fragment.Substring(1);
            
        var pairs = fragment.Split('&');
        foreach (var pair in pairs)
        {
            var parts = pair.Split('=');
            if (parts.Length == 2)
            {
                result[parts[0]] = Uri.UnescapeDataString(parts[1]);
            }
        }
        
        return result;
    }

    public async Task<bool> ExchangeB2CTokenAsync(string b2cToken)
    {
        try
        {
            // Use the B2C-specific HttpClient that doesn't have JWT authentication handler
            using var httpClient = _httpClientFactory.CreateClient(FSH.Starter.Blazor.Infrastructure.Extensions.B2CClientName);
            
            // Exchange B2C token for local JWT using the public endpoint
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", b2cToken);
            var response = await httpClient.PostAsync("/api/public/b2c-token", null);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<FSH.Starter.Blazor.Shared.TokenResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
                {
                    Console.WriteLine("B2CAuthenticationService.ExchangeB2CTokenAsync: Storing tokens in localStorage");
                    await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, tokenResponse.Token);
                    await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, tokenResponse.RefreshToken);
                    
                    Console.WriteLine("B2CAuthenticationService.ExchangeB2CTokenAsync: Notifying authentication state change");
                    // Notify authentication state change
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                    
                    // Give a small delay to ensure the state change propagates
                    await Task.Delay(50);
                    
                    Console.WriteLine("B2CAuthenticationService.ExchangeB2CTokenAsync: Token exchange completed successfully");
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exchanging B2C token: {ex.Message}");
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
        
        Console.WriteLine($"B2CAuthenticationService.RequestAccessToken: Token from localStorage: {(string.IsNullOrEmpty(token) ? "NULL/EMPTY" : $"[{token.Length} chars]")}");
        
        if (!string.IsNullOrEmpty(token))
        {
            Console.WriteLine("B2CAuthenticationService.RequestAccessToken: Returning SUCCESS with token");
            return new AccessTokenResult(
                AccessTokenResultStatus.Success,
                new AccessToken { Value = token },
                null);
        }

        Console.WriteLine("B2CAuthenticationService.RequestAccessToken: Returning REQUIRES_REDIRECT");
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
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs is not null)
        {
            // Handle roles array (similar to JwtAuthenticationService)
            keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles);
            if (roles is not null)
            {
                string? rolesString = roles.ToString();
                if (!string.IsNullOrEmpty(rolesString))
                {
                    if (rolesString.Trim().StartsWith("["))
                    {
                        string[]? parsedRoles = JsonSerializer.Deserialize<string[]>(rolesString);
                        if (parsedRoles is not null)
                        {
                            claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rolesString));
                    }
                }
                keyValuePairs.Remove(ClaimTypes.Role);
            }

            // Handle permissions array
            keyValuePairs.TryGetValue("permission", out object? permissions);
            if (permissions is not null)
            {
                string? permissionsString = permissions.ToString();
                if (!string.IsNullOrEmpty(permissionsString))
                {
                    if (permissionsString.Trim().StartsWith("["))
                    {
                        string[]? parsedPermissions = JsonSerializer.Deserialize<string[]>(permissionsString);
                        if (parsedPermissions is not null)
                        {
                            claims.AddRange(parsedPermissions.Select(permission => new Claim("permission", permission)));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim("permission", permissionsString));
                    }
                }
                keyValuePairs.Remove("permission");
            }

            // Add remaining claims
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));
        }

        return claims;
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