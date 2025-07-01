using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using FSH.Starter.Blazor.Infrastructure.Auth;

namespace FSH.Starter.Blazor.Client.Pages.Error;

public partial class AuthError : ComponentBase
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private IAuthenticationService AuthenticationService { get; set; } = default!;
    
    protected string ErrorMessage { get; private set; } = "We were unable to complete your authentication. This could be due to an expired session or a temporary issue.";
    protected string RetryButtonText { get; private set; } = "Try Again";
    protected string SupportMessage { get; private set; } = "If the problem persists, please contact our support team.";

    protected void OnRetryClicked()
    {
        // Navigate directly to B2C authentication to retry
        AuthenticationService.NavigateToExternalLogin("/", null);
    }
    
    protected async Task OnContactSupportClicked()
    {
        // Open email client with pre-filled support request
        var subject = Uri.EscapeDataString("Authentication Issue - OHD Harvest Marketplace");
        var body = Uri.EscapeDataString("Hello Support Team,\n\nI am experiencing an authentication issue with the OHD Harvest Marketplace.\n\nDetails:\n- Date/Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n- Issue: Unable to complete authentication\n\nPlease assist.\n\nThank you.");
        
        await JS.InvokeVoidAsync("eval", $"window.location.href = 'mailto:support@ohdharvest.com?subject={subject}&body={body}'");
    }
}