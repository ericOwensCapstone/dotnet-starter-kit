using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Infrastructure.Auth;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Auth;

public partial class AcceptInvitation
{
    [Inject] private IApiClient ApiClient { get; set; } = default!;
    [Inject] private IAuthenticationService AuthenticationService { get; set; } = default!;

    private bool _isLoading = true;
    private bool _hasError = false;
    private string _errorMessage = string.Empty;
    private string? _invitationToken;
    private string? _invitedUserEmail;
    private string? _invitedUserName;

    protected override async Task OnInitializedAsync()
    {
        await ValidateInvitationTokenAsync();
    }

    private async Task ValidateInvitationTokenAsync()
    {
        try
        {
            // Extract the token from the query string
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var query = uri.Query;
            if (!string.IsNullOrEmpty(query) && query.Contains("token="))
            {
                var tokenParam = query.Split('&').FirstOrDefault(p => p.Contains("token="));
                if (tokenParam != null)
                {
                    _invitationToken = tokenParam.Split('=')[1];
                    _invitationToken = Uri.UnescapeDataString(_invitationToken);
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
                var validationResult = await ApiClient.ValidateInvitationTokenAsync(_invitationToken);
                _invitedUserEmail = validationResult.Email;
                _invitedUserName = validationResult.DisplayName;
                _hasError = false;
            }
            catch (ApiException apiEx) when (apiEx.StatusCode == 404)
            {
                _hasError = true;
                _errorMessage = "This invitation link is not valid. Please check your invitation email.";
            }
            catch (ApiException apiEx) when (apiEx.StatusCode == 400)
            {
                _hasError = true;
                _errorMessage = apiEx.Message ?? "This invitation cannot be accepted.";
            }
            catch (Exception)
            {
                _hasError = true;
                _errorMessage = "Unable to validate your invitation. Please try again later.";
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

    private void AcceptAndSignUp()
    {
        if (string.IsNullOrEmpty(_invitationToken))
        {
            Toast.Add("Invalid invitation token.", Severity.Error);
            return;
        }

        try
        {
            // Navigate to B2C authentication
            // The B2C auto-provisioning will automatically find and accept 
            // any valid invitation for the user's email address
            var returnUrl = "/";
            AuthenticationService.NavigateToExternalLogin(returnUrl);
        }
        catch (Exception ex)
        {
            Toast.Add($"Failed to redirect to sign-up: {ex.Message}", Severity.Error);
        }
    }

    private void GoToLogin()
    {
        Navigation.NavigateTo("/login");
    }
}