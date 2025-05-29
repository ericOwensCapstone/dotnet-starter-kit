using FSH.Starter.Blazor.Client.Components;
using FSH.Starter.Blazor.Infrastructure.Auth;
using FSH.Starter.Blazor.Infrastructure.Api;
using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Auth;

public partial class Login()
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;
    
    [Inject] private IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] private IAuthenticationService AuthenticationService { get; set; } = default!;

    private FshValidation? _customValidation;

    public bool BusySubmitting { get; set; }
    private bool _isRedirectingToB2C = false;

    private readonly TokenGenerationCommand _tokenRequest = new();
    private string TenantId { get; set; } = string.Empty;
    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        if (authState.User.Identity?.IsAuthenticated is true)
        {
            Navigation.NavigateTo("/");
            return;
        }

        // Check if we're coming from a failed authentication attempt to prevent loops
        var uri = new Uri(Navigation.Uri);
        var hasError = uri.Query.Contains("error=");
        
        if (hasError)
        {
            Console.WriteLine($"Login page loaded with error parameter in URL: {uri.Query}");
            // Don't redirect again if there was an authentication error
            return;
        }

        // If using B2C, redirect to B2C login
        var authConfig = ServiceProvider.GetService<IAuthenticationConfigurationService>();
        if (authConfig?.IsAzureB2C() == true)
        {
            Console.WriteLine("Redirecting to B2C login...");
            _isRedirectingToB2C = true;
            StateHasChanged();
            await Task.Delay(100); // Brief delay to show loading state
            await AuthenticationService.LoginAsync(string.Empty, new TokenGenerationCommand());
        }
    }

    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
    }

    private void FillAdministratorCredentials()
    {
        _tokenRequest.Email = TenantConstants.Root.EmailAddress;
        _tokenRequest.Password = TenantConstants.DefaultPassword;
        TenantId = TenantConstants.Root.Id;
    }

    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => AuthenticationService.LoginAsync(TenantId, _tokenRequest),
            Toast,
            _customValidation))
        {
            Toast.Add($"Logged in as {_tokenRequest.Email}", Severity.Info);
        }

        BusySubmitting = false;
    }
}
