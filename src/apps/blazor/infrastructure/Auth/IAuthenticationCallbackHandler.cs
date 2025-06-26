namespace FSH.Starter.Blazor.Infrastructure.Auth;

public interface IAuthenticationCallbackHandler
{
    Task<AuthenticationCallbackResult> HandleCallbackAsync(string action, string uri);
}

public class AuthenticationCallbackResult
{
    public bool Success { get; set; }
    public string NavigationTarget { get; set; } = "/";
    public string? ErrorMessage { get; set; }
    
    public static AuthenticationCallbackResult SuccessResult(string navigationTarget = "/") 
        => new() { Success = true, NavigationTarget = navigationTarget };
    
    public static AuthenticationCallbackResult FailureResult(string errorMessage, string navigationTarget = "/error/auth-failed") 
        => new() { Success = false, ErrorMessage = errorMessage, NavigationTarget = navigationTarget };
}