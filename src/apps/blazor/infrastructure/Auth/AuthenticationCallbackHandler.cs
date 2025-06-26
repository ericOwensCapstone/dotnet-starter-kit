using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.Blazor.Infrastructure.Auth;

public class AuthenticationCallbackHandler : IAuthenticationCallbackHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AuthFlowStateService _authFlowState;
    
    public AuthenticationCallbackHandler(
        IServiceProvider serviceProvider,
        AuthFlowStateService authFlowState)
    {
        _serviceProvider = serviceProvider;
        _authFlowState = authFlowState;
    }
    
    public async Task<AuthenticationCallbackResult> HandleCallbackAsync(string action, string uri)
    {
        var authConfig = _serviceProvider.GetService<IAuthenticationConfigurationService>();
        var isB2C = authConfig?.IsAzureB2C() == true;
        
        if (!isB2C)
        {
            // Non-B2C authentication not supported in this implementation
            return AuthenticationCallbackResult.FailureResult("Authentication provider not supported", "/login");
        }
        
        switch (action.ToLowerInvariant())
        {
            case "login-callback":
                return await HandleB2CLoginCallbackAsync(uri);
                
            case "logout-callback":
                return AuthenticationCallbackResult.SuccessResult("/");
                
            default:
                return AuthenticationCallbackResult.FailureResult($"Unknown authentication action: {action}", "/");
        }
    }
    
    private async Task<AuthenticationCallbackResult> HandleB2CLoginCallbackAsync(string uri)
    {
        try
        {
            Console.WriteLine($"AuthenticationCallbackHandler: Processing B2C callback with URI: {uri}");
            
            // Set state for post-B2C loading
            await _authFlowState.SetStateAsync(AuthFlowState.ProcessingB2CReturn);
            
            // Process B2C callback
            var authService = _serviceProvider.GetService<IAuthenticationService>();
            if (authService == null)
            {
                Console.WriteLine("AuthenticationCallbackHandler: Authentication service not available");
                return AuthenticationCallbackResult.FailureResult("Authentication service not available");
            }
            
            Console.WriteLine("AuthenticationCallbackHandler: Calling ProcessAuthenticationCallbackAsync");
            var success = await authService.ProcessAuthenticationCallbackAsync(uri);
            
            Console.WriteLine($"AuthenticationCallbackHandler: ProcessAuthenticationCallbackAsync returned: {success}");
            
            if (success)
            {
                await _authFlowState.ClearStateAsync();
                return AuthenticationCallbackResult.SuccessResult("/");
            }
            else
            {
                return AuthenticationCallbackResult.FailureResult("Authentication failed. Please try again.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AuthenticationCallbackHandler: Exception occurred: {ex.Message}");
            return AuthenticationCallbackResult.FailureResult($"An error occurred during authentication: {ex.Message}");
        }
    }
}