using Blazored.LocalStorage;
using FSH.Starter.Blazor.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using System.Web;

namespace FSH.Starter.Blazor.Client.Pages.Auth;

public partial class AcceptInvitation
{
    [Inject] private IApiClient ApiClient { get; set; } = default!;
    [Inject] private ILocalStorageService localStorage { get; set; } = default!;

    private string _tenantName = string.Empty;
    private string? _invitationToken;
    private bool _isLoading = true;
    private bool _hasError = false;
    private string _errorTitle = string.Empty;
    private string _errorMessage = string.Empty;
    private string _signUpUrl = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine("AcceptInvitationStyled.OnInitializedAsync: Starting");
        
        // Extract token from URL
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        var query = uri.Query;
        
        if (!string.IsNullOrEmpty(query))
        {
            var queryString = query.StartsWith("?") ? query.Substring(1) : query;
            var queryParams = HttpUtility.ParseQueryString(queryString);
            _invitationToken = queryParams["token"];
            
            if (!string.IsNullOrEmpty(_invitationToken))
            {
                Console.WriteLine($"AcceptInvitationStyled: Token found: {_invitationToken}");
                await LoadInvitationPage();
                return;
            }
        }
        
        // If no token, show error
        _hasError = true;
        _errorTitle = "Invalid invitation";
        _errorMessage = "This invitation link is invalid or has expired.";
        _isLoading = false;
    }

    private async Task LoadInvitationPage()
    {
        try
        {
            // Validate the token with the API
            Console.WriteLine($"AcceptInvitationStyled: Validating token: {_invitationToken}");
            var validationResult = await ApiClient.ValidateInvitationTokenAsync(_invitationToken!);
            
            // Get B2C configuration
            var clientId = Config["AuthenticationOptions:AzureAdB2C:ClientId"];
            var instance = Config["AuthenticationOptions:AzureAdB2C:Instance"];
            var domain = Config["AuthenticationOptions:AzureAdB2C:Domain"];
            var apiScope = Config["AuthenticationOptions:AzureAdB2C:ApiScope"];
            
            // Use the invitation acceptance policy
            var policyId = "B2C_1A_invitation_acceptance";
            var authority = $"{instance}/{domain}/{policyId}";
            
            var redirectUri = new Uri(Navigation.BaseUri).GetLeftPart(UriPartial.Authority) + "/authentication/login-callback";
            
            // Construct the B2C URL with the invitation token as a parameter
            var signUpUrl = $"{authority}/oauth2/v2.0/authorize" +
                $"?client_id={clientId}" +
                $"&response_type=id_token token" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                $"&scope={Uri.EscapeDataString($"openid offline_access {apiScope}")}" +
                $"&response_mode=fragment" +
                $"&nonce={Guid.NewGuid()}" +
                $"&state={Uri.EscapeDataString("/")}" +
                $"&invitationToken={Uri.EscapeDataString(_invitationToken!)}";

            Console.WriteLine($"AcceptInvitationStyled: Sign-up URL: {signUpUrl}");
            
            // For security, don't show personal details before email verification
            _tenantName = "OHD Harvest Marketplace";
            _signUpUrl = signUpUrl;
            _isLoading = false;
        }
        catch (ApiException apiEx) when (apiEx.StatusCode == 404)
        {
            _hasError = true;
            _errorTitle = "Invalid invitation";
            _errorMessage = "This invitation link is not valid. Please check your invitation email.";
            _isLoading = false;
            Console.WriteLine($"AcceptInvitationStyled: 404 error - Invalid invitation");
        }
        catch (ApiException apiEx) when (apiEx.StatusCode == 400)
        {
            var message = apiEx.Message ?? "This invitation cannot be accepted.";
            _hasError = true;
            _isLoading = false;
            
            if (message.Contains("already been accepted"))
            {
                _errorTitle = "Invitation already used";
                _errorMessage = "This invitation has already been accepted.";
            }
            else if (message.Contains("expired"))
            {
                _errorTitle = "Invitation expired";
                _errorMessage = "This invitation has expired. Please request a new one.";
            }
            else
            {
                _errorTitle = "Invalid invitation";
                _errorMessage = message;
            }
            Console.WriteLine($"AcceptInvitationStyled: 400 error - {message}");
        }
        catch (Exception ex)
        {
            _hasError = true;
            _errorTitle = "Error";
            _errorMessage = "Unable to validate your invitation. Please try again later.";
            _isLoading = false;
            Console.WriteLine($"AcceptInvitationStyled: General error - {ex.Message}");
        }
    }

}