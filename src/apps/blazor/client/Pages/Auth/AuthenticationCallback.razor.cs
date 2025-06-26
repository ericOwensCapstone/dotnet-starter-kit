using FSH.Starter.Blazor.Infrastructure.Auth;
using FSH.Starter.Blazor.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FSH.Starter.Blazor.Client.Pages.Auth;

public partial class AuthenticationCallback : ComponentBase
{
    [Parameter] public string Action { get; set; } = string.Empty;
    
    [Inject] private IAuthenticationCallbackHandler CallbackHandler { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private IAuthenticationFlowState AuthFlowState { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        // Build the full URL with fragment from Navigation.Uri and browser's hash
        var baseUri = Navigation.Uri;
        var fragment = await JS.InvokeAsync<string>("eval", "window.location.hash");
        
        Console.WriteLine($"AuthenticationCallback: baseUri = {baseUri}");
        Console.WriteLine($"AuthenticationCallback: fragment = {fragment}");
        
        // Combine the base URI with the fragment
        var fullUri = baseUri;
        if (!string.IsNullOrEmpty(fragment))
        {
            // Remove any existing fragment from baseUri first
            var hashIndex = baseUri.IndexOf('#');
            if (hashIndex >= 0)
            {
                baseUri = baseUri.Substring(0, hashIndex);
            }
            fullUri = baseUri + fragment;
        }
        
        Console.WriteLine($"AuthenticationCallback: fullUri = {fullUri}");
        
        // Handle the authentication callback
        var result = await CallbackHandler.HandleCallbackAsync(Action, fullUri);
        
        // Set flag to show panel on home page if we're navigating there
        if (result.NavigationTarget == "/" || result.NavigationTarget.StartsWith("/?"))
        {
            AuthFlowState.SetReturningFromAuthentication();
        }
        
        // Navigate to the target
        Navigation.NavigateTo(result.NavigationTarget, forceLoad: false);
    }
}