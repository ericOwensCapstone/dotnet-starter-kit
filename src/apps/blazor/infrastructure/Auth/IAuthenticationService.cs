namespace FSH.Starter.Blazor.Infrastructure.Auth;

public interface IAuthenticationService
{
    void NavigateToExternalLogin(string returnUrl, string? loginHint = null);

    Task LogoutAsync();

    Task ReLoginAsync(string returnUrl);
}