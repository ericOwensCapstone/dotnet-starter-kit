using Blazored.LocalStorage;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Infrastructure.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Auth;

public partial class AcceptInvitation
{
    [Inject] private IApiClient ApiClient { get; set; } = default!;
    [Inject] private IAuthenticationService AuthenticationService { get; set; } = default!;
    [Inject] private ILocalStorageService localStorage { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;

    private bool _isLoading = true;
    private bool _hasError = false;
    private string _errorMessage = string.Empty;
    private string? _invitationToken;
    private string? _invitedUserEmail;
    private string? _invitedUserName;

    public AcceptInvitation()
    {
        Console.WriteLine("AcceptInvitation: Constructor called");
    }

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine("AcceptInvitation.OnInitializedAsync: Starting");
        try
        {
            await ValidateInvitationTokenAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AcceptInvitation.OnInitializedAsync: Exception - {ex.Message}");
            _hasError = true;
            _errorMessage = "An error occurred while loading the invitation.";
            _isLoading = false;
        }
        Console.WriteLine("AcceptInvitation.OnInitializedAsync: Completed");
    }

    private async Task ValidateInvitationTokenAsync()
    {
        try
        {
            // Extract the token from the query string
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var query = uri.Query;
            Console.WriteLine($"AcceptInvitation: Full URI: {Navigation.Uri}");
            Console.WriteLine($"AcceptInvitation: Query string: {query}");
            
            if (!string.IsNullOrEmpty(query))
            {
                // Remove the leading '?' if present
                if (query.StartsWith("?"))
                {
                    query = query.Substring(1);
                }
                
                var queryParams = query.Split('&');
                foreach (var param in queryParams)
                {
                    var parts = param.Split('=');
                    if (parts.Length == 2 && parts[0] == "token")
                    {
                        _invitationToken = Uri.UnescapeDataString(parts[1]);
                        Console.WriteLine($"AcceptInvitation: Extracted token: {_invitationToken}");
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(_invitationToken))
            {
                _hasError = true;
                _errorMessage = "No invitation token provided. Please check your invitation link.";
                return;
            }

            // Validate the token with the API
            try
            {
                Console.WriteLine($"AcceptInvitation: Validating token: {_invitationToken}");
                var validationResult = await ApiClient.ValidateInvitationTokenAsync(_invitationToken);
                _invitedUserEmail = validationResult.Email;
                _invitedUserName = validationResult.DisplayName;
                _hasError = false;
                Console.WriteLine($"AcceptInvitation: Validation successful - Email: {_invitedUserEmail}, Name: {_invitedUserName}");
            }
            catch (ApiException apiEx) when (apiEx.StatusCode == 404)
            {
                _hasError = true;
                _errorMessage = "This invitation link is not valid. Please check your invitation email.";
                Console.WriteLine($"AcceptInvitation: 404 error - {_errorMessage}");
            }
            catch (ApiException apiEx) when (apiEx.StatusCode == 400)
            {
                _hasError = true;
                _errorMessage = apiEx.Message ?? "This invitation cannot be accepted.";
                Console.WriteLine($"AcceptInvitation: 400 error - {_errorMessage}");
            }
            catch (Exception ex)
            {
                _hasError = true;
                _errorMessage = "Unable to validate your invitation. Please try again later.";
                Console.WriteLine($"AcceptInvitation: General error - {ex.Message}");
            }
        }
        catch (Exception)
        {
            _hasError = true;
            _errorMessage = "An error occurred while processing your invitation. Please try again.";
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private async Task AcceptAndSignUp()
    {
        if (string.IsNullOrEmpty(_invitationToken))
        {
            Toast.Add("Invalid invitation token.", Severity.Error);
            return;
        }

        try
        {
            Console.WriteLine($"AcceptInvitation.AcceptAndSignUp: Email={_invitedUserEmail}, Token={_invitationToken}");
            
            // Store the invitation token in localStorage so it can be retrieved after B2C authentication
            await localStorage.SetItemAsync("pendingInvitationToken", _invitationToken);
            
            // For invitation acceptance, we need to use the custom B2C invitation acceptance policy
            var clientId = Configuration["AuthenticationOptions:AzureAdB2C:ClientId"];
            var instance = Configuration["AuthenticationOptions:AzureAdB2C:Instance"];
            var domain = Configuration["AuthenticationOptions:AzureAdB2C:Domain"];
            var apiScope = Configuration["AuthenticationOptions:AzureAdB2C:ApiScope"];
            
            // Use the invitation acceptance policy
            var policyId = "B2C_1A_invitation_acceptance";
            var authority = $"{instance}/{domain}/{policyId}";
            
            var redirectUri = new Uri(Navigation.BaseUri).GetLeftPart(UriPartial.Authority) + "/authentication/login-callback";
            var returnUrl = "/";
            
            // Construct the URL with the invitation token as a parameter
            var loginUrl = $"{authority}/oauth2/v2.0/authorize" +
                $"?client_id={clientId}" +
                $"&response_type=id_token token" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                $"&scope={Uri.EscapeDataString($"openid offline_access {apiScope}")}" +
                $"&response_mode=fragment" +
                $"&nonce={Guid.NewGuid()}" +
                $"&state={Uri.EscapeDataString(returnUrl)}" +
                $"&invitationToken={Uri.EscapeDataString(_invitationToken)}";

            Console.WriteLine($"AcceptInvitation.AcceptAndSignUp: Navigating to B2C invitation acceptance policy");
            Navigation.NavigateTo(loginUrl, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AcceptInvitation.AcceptAndSignUp Error: {ex.Message}");
            Toast.Add($"Failed to redirect to sign-up: {ex.Message}", Severity.Error);
        }
    }

    private void GoToLogin()
    {
        Navigation.NavigateTo("/login");
    }
}