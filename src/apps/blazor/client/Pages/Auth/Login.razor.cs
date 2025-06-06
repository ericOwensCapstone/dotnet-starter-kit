using FSH.Starter.Blazor.Infrastructure.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Client.Pages.Auth;

public partial class Login
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;
    
    [Inject] private IAuthenticationService AuthenticationService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        if (authState.User.Identity?.IsAuthenticated is true)
        {
            Navigation.NavigateTo("/");
            return;
        }

        // Always redirect to B2C
        AuthenticationService.NavigateToExternalLogin("/", null);
    }
}
