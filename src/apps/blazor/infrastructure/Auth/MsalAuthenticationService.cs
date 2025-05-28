using System.Security.Claims;
using FSH.Starter.Blazor.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FSH.Starter.Blazor.Infrastructure.Auth;

public class MsalAuthenticationService : AuthenticationStateProvider, IAuthenticationService
{
    private readonly NavigationManager _navigation;
    private readonly SignOutSessionStateManager _signOutManager;

    public MsalAuthenticationService(NavigationManager navigation, SignOutSessionStateManager signOutManager)
    {
        _navigation = navigation;
        _signOutManager = signOutManager;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // This is handled by the MSAL library
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
    }

    public void NavigateToExternalLogin(string returnUrl)
    {
        var loginUrl = $"authentication/login";
        if (!string.IsNullOrEmpty(returnUrl))
        {
            loginUrl += $"?returnUrl={Uri.EscapeDataString(returnUrl)}";
        }
        _navigation.NavigateTo(loginUrl, forceLoad: true);
    }

    public Task<bool> LoginAsync(string tenantId, TokenGenerationCommand request)
    {
        // MSAL doesn't support direct username/password login via TokenGenerationCommand
        // Redirect to the MSAL login page instead
        NavigateToExternalLogin(string.Empty);
        return Task.FromResult(true);
    }

    public async Task LogoutAsync()
    {
        await _signOutManager.SetSignOutState();
        _navigation.NavigateTo("authentication/logout", forceLoad: true);
    }

    public Task ReLoginAsync(string returnUrl)
    {
        // For MSAL, re-login is the same as regular login
        NavigateToExternalLogin(returnUrl);
        return Task.CompletedTask;
    }
}