using Blazored.LocalStorage;
using FSH.Starter.Blazor.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using System.Web;

namespace FSH.Starter.Blazor.Client.Pages.Auth;

public partial class AcceptInvitation
{
    [Inject] private IApiClient ApiClient { get; set; } = default!;
    [Inject] private ILocalStorageService localStorage { get; set; } = default!;

    private string _pageHtml = string.Empty;
    private string _tenantName = string.Empty;
    private string? _invitationToken;

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
        _pageHtml = GetErrorPage("Invalid invitation", "This invitation link is invalid or has expired.", Navigation.BaseUri);
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
            
            _pageHtml = GetLandingPage(
                null, // Don't show display name
                validationResult.Email, // Email is still needed for B2C
                null, // Don't show tenant name
                null, // Don't show inviter
                signUpUrl,
                Navigation.BaseUri
            );
        }
        catch (ApiException apiEx) when (apiEx.StatusCode == 404)
        {
            _pageHtml = GetErrorPage("Invalid invitation", "This invitation link is not valid. Please check your invitation email.", Navigation.BaseUri);
            Console.WriteLine($"AcceptInvitationStyled: 404 error - Invalid invitation");
        }
        catch (ApiException apiEx) when (apiEx.StatusCode == 400)
        {
            var message = apiEx.Message ?? "This invitation cannot be accepted.";
            if (message.Contains("already been accepted"))
            {
                _pageHtml = GetErrorPage("Invitation already used", "This invitation has already been accepted.", Navigation.BaseUri);
            }
            else if (message.Contains("expired"))
            {
                _pageHtml = GetErrorPage("Invitation expired", "This invitation has expired. Please request a new one.", Navigation.BaseUri);
            }
            else
            {
                _pageHtml = GetErrorPage("Invalid invitation", message, Navigation.BaseUri);
            }
            Console.WriteLine($"AcceptInvitationStyled: 400 error - {message}");
        }
        catch (Exception ex)
        {
            _pageHtml = GetErrorPage("Error", "Unable to validate your invitation. Please try again later.", Navigation.BaseUri);
            Console.WriteLine($"AcceptInvitationStyled: General error - {ex.Message}");
        }
    }

    private static string GetLandingPage(string? displayName, string email, string? tenantName, string? invitedBy, string signUpUrl, string clientOrigin)
    {
        // Original design with icon and blue info box (commented out for easy restoration)
        /*
        return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Accept Invitation - {tenantName}</title>
    <link href='https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap' rel='stylesheet' />
    <style>
        :root {{
            --primary-color: rgba(76,175,80,1);
            --primary-dark: rgba(56,142,60,1);
            --secondary-color: rgba(33,150,243,1);
            --background-color: #1b1f22;
            --surface-color: #202528;
            --error-color: #f44336;
            --text-primary: rgba(255,255,255, 0.70);
            --text-secondary: rgba(255,255,255, 0.50);
            --border-radius: 5px;
            --elevation-1: 0px 2px 1px -1px rgba(0,0,0,0.2), 0px 1px 1px 0px rgba(0,0,0,0.14), 0px 1px 3px 0px rgba(0,0,0,0.12);
            --elevation-25: 0px 8px 10px -5px rgba(0,0,0,0.2), 0px 16px 24px 2px rgba(0,0,0,0.14), 0px 6px 30px 5px rgba(0,0,0,0.12);
            --info-background: #1e3a5f;
            --divider-color: #e0e0e036;
        }}
        
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        
        body {{
            font-family: 'Roboto', 'Helvetica', 'Arial', sans-serif;
            font-size: 16px;
            line-height: 1.5;
            color: var(--text-primary);
            background-color: var(--background-color);
            background-image: url('https://ohdb2ctemplates.blob.core.windows.net/ohdb2c-templates/cornfield.png');
            background-size: cover;
            background-position: center top;
            background-repeat: no-repeat;
            background-attachment: fixed;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }}
        
        .container {{
            width: 100%;
            max-width: 400px;
            padding: 16px;
        }}
        
        .paper {{
            background-color: var(--surface-color);
            border-radius: var(--border-radius);
            box-shadow: var(--elevation-25);
            padding: 32px;
        }}
        
        .text-center {{
            text-align: center;
        }}
        
        .icon {{
            width: 64px;
            height: 64px;
            margin: 0 auto 16px;
            display: block;
            fill: var(--primary-color);
        }}
        
        h1 {{
            font-size: 2.125rem;
            font-weight: 400;
            line-height: 1.235;
            letter-spacing: 0.00735em;
            margin-bottom: 8px;
        }}
        
        .subtitle {{
            font-size: 1rem;
            line-height: 1.5;
            letter-spacing: 0.00938em;
            color: var(--text-secondary);
            margin-bottom: 24px;
        }}
        
        .info-section {{
            background-color: var(--info-background);
            border: 1px solid var(--divider-color);
            border-radius: var(--border-radius);
            padding: 16px;
            margin-bottom: 24px;
        }}
        
        .info-section p {{
            font-size: 0.875rem;
            line-height: 1.43;
            letter-spacing: 0.01071em;
            margin-bottom: 8px;
        }}
        
        .info-section p:last-child {{
            margin-bottom: 0;
        }}
        
        .info-section strong {{
            font-weight: 500;
        }}
        
        .button {{
            display: inline-block;
            width: 100%;
            padding: 12px 24px;
            background-color: var(--primary-color);
            color: white;
            text-decoration: none;
            border-radius: var(--border-radius);
            font-size: 0.875rem;
            font-weight: 500;
            letter-spacing: 0.02857em;
            text-transform: uppercase;
            text-align: center;
            box-shadow: var(--elevation-1);
            transition: background-color 250ms cubic-bezier(0.4, 0, 0.2, 1) 0ms,
                        box-shadow 250ms cubic-bezier(0.4, 0, 0.2, 1) 0ms;
        }}
        
        .button:hover {{
            background-color: var(--primary-dark);
            box-shadow: 0px 3px 1px -2px rgba(0,0,0,0.2), 0px 2px 2px 0px rgba(0,0,0,0.14), 0px 1px 5px 0px rgba(0,0,0,0.12);
        }}
        
        .divider {{
            margin: 24px 0;
            height: 1px;
            background-color: var(--divider-color);
        }}
        
        .text-secondary {{
            color: var(--text-secondary);
            font-size: 0.875rem;
            line-height: 1.43;
            letter-spacing: 0.01071em;
        }}
        
        .text-link {{
            color: var(--primary-color);
            text-decoration: none;
            font-weight: 500;
        }}
        
        .text-link:hover {{
            text-decoration: underline;
        }}
        
        .mb-2 {{ margin-bottom: 8px; }}
        .mb-3 {{ margin-bottom: 12px; }}
        .mb-4 {{ margin-bottom: 16px; }}
        .mb-6 {{ margin-bottom: 24px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='paper'>
            <div class='text-center mb-6'>
                <svg class='icon' viewBox='0 0 24 24'>
                    <path d='M20,8L12,13L4,8V6L12,11L20,6M20,4H4C2.89,4 2,4.89 2,6V18A2,2 0 0,0 4,20H20A2,2 0 0,0 22,18V6C22,4.89 21.1,4 20,4Z' />
                </svg>
                <h1>Accept Invitation</h1>
                
                <p class='subtitle mb-2'>
                    Welcome, <strong>{displayName}</strong>!
                </p>
                <p class='text-secondary mb-3'>
                    {email}
                </p>
                <p class='subtitle mb-4'>
                    Click the button below to create your account and accept this invitation.
                </p>
                
                <div class='info-section'>
                    <p>
                        You've been invited to join <strong>{tenantName}</strong> by <strong>{invitedBy}</strong>.
                    </p>
                    <p>
                        Your email <strong>{email}</strong> will be pre-filled.
                    </p>
                    <p>
                        Click <strong>""Sign up now""</strong> at the bottom to complete registration.
                    </p>
                </div>
                
                <a href='{signUpUrl}' class='button'>
                    Continue to Verify Email
                </a>
                
                <div class='divider'></div>
                
                <p class='text-secondary mb-2'>
                    Already have an account?
                </p>
                <a href='{clientOrigin}login' class='text-link'>
                    Sign In Instead
                </a>
            </div>
        </div>
    </div>
</body>
</html>";
        */
        
        // Simplified design without icon and info box
        return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Accept Invitation - {tenantName}</title>
    <link href='https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap' rel='stylesheet' />
    <style>
        :root {{
            --primary-color: rgba(76,175,80,1);
            --primary-dark: rgba(56,142,60,1);
            --secondary-color: rgba(33,150,243,1);
            --background-color: #1b1f22;
            --surface-color: #202528;
            --error-color: #f44336;
            --text-primary: rgba(255,255,255, 0.70);
            --text-secondary: rgba(255,255,255, 0.50);
            --border-radius: 5px;
            --elevation-1: 0px 2px 1px -1px rgba(0,0,0,0.2), 0px 1px 1px 0px rgba(0,0,0,0.14), 0px 1px 3px 0px rgba(0,0,0,0.12);
            --elevation-25: 0px 8px 10px -5px rgba(0,0,0,0.2), 0px 16px 24px 2px rgba(0,0,0,0.14), 0px 6px 30px 5px rgba(0,0,0,0.12);
            --info-background: #1e3a5f;
            --divider-color: #e0e0e036;
        }}
        
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        
        body {{
            font-family: 'Roboto', 'Helvetica', 'Arial', sans-serif;
            font-size: 16px;
            line-height: 1.5;
            color: var(--text-primary);
            background-color: var(--background-color);
            background-image: url('https://ohdb2ctemplates.blob.core.windows.net/ohdb2c-templates/cornfield.png');
            background-size: cover;
            background-position: center top;
            background-repeat: no-repeat;
            background-attachment: fixed;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }}
        
        .container {{
            width: 100%;
            max-width: 400px;
            padding: 16px;
        }}
        
        .paper {{
            background-color: var(--surface-color);
            border-radius: var(--border-radius);
            box-shadow: var(--elevation-25);
            padding: 32px;
        }}
        
        .text-center {{
            text-align: center;
        }}
        
        .image-placeholder {{
            height: 64px;
            margin-bottom: 16px;
            /* Space reserved for future image */
        }}
        
        h1 {{
            font-size: 2.125rem;
            font-weight: 400;
            line-height: 1.235;
            letter-spacing: 0.00735em;
            margin-bottom: 24px;
        }}
        
        .user-info {{
            margin-bottom: 24px;
        }}
        
        .subtitle {{
            font-size: 1rem;
            line-height: 1.5;
            letter-spacing: 0.00938em;
            color: var(--text-secondary);
            margin-bottom: 8px;
        }}
        
        .text-secondary {{
            color: var(--text-secondary);
            font-size: 0.875rem;
            line-height: 1.43;
            letter-spacing: 0.01071em;
        }}
        
        .button {{
            display: inline-block;
            width: 100%;
            padding: 12px 24px;
            background-color: var(--primary-color);
            color: white;
            text-decoration: none;
            border-radius: var(--border-radius);
            font-size: 0.875rem;
            font-weight: 500;
            letter-spacing: 0.02857em;
            text-transform: uppercase;
            text-align: center;
            box-shadow: var(--elevation-1);
            transition: background-color 250ms cubic-bezier(0.4, 0, 0.2, 1) 0ms,
                        box-shadow 250ms cubic-bezier(0.4, 0, 0.2, 1) 0ms;
            margin-top: 24px;
        }}
        
        .button:hover {{
            background-color: var(--primary-dark);
            box-shadow: 0px 3px 1px -2px rgba(0,0,0,0.2), 0px 2px 2px 0px rgba(0,0,0,0.14), 0px 1px 5px 0px rgba(0,0,0,0.12);
        }}
        
        .divider {{
            margin: 24px 0;
            height: 1px;
            background-color: var(--divider-color);
        }}
        
        .text-link {{
            color: var(--primary-color);
            text-decoration: none;
            font-weight: 500;
        }}
        
        .text-link:hover {{
            text-decoration: underline;
        }}
        
        .mb-2 {{ margin-bottom: 8px; }}
        .mb-3 {{ margin-bottom: 12px; }}
        .mb-4 {{ margin-bottom: 16px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='paper'>
            <div class='text-center'>
                <div class='image-placeholder'>
                    <!-- Space reserved for future image -->
                </div>
                
                <h1>
                    OHD Harvest<br />
                    Marketplace
                </h1>
                
                <div class='user-info'>
                    <h2 style='font-size: 1.5rem; font-weight: 400; margin-bottom: 16px;'>
                        Email Verification Required
                    </h2>
                    <p class='subtitle'>
                        To accept this invitation, you must verify your email address.
                    </p>
                    <p class='subtitle'>
                        Click below to continue with the verification process.
                    </p>
                </div>
                
                <a href='{signUpUrl}' class='button'>
                    Continue to Verify Email
                </a>
                
                <div class='divider'></div>
                
                <p class='text-secondary mb-2'>
                    Already have an account?
                </p>
                <a href='{clientOrigin}login' class='text-link'>
                    Sign In Instead
                </a>
            </div>
        </div>
    </div>
</body>
</html>";
    }
    
    private static string GetErrorPage(string title, string message, string clientOrigin)
    {
        return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{title}</title>
    <link href='https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap' rel='stylesheet' />
    <style>
        :root {{
            --primary-color: rgba(76,175,80,1);
            --error-color: #f44336;
            --background-color: #1b1f22;
            --surface-color: #202528;
            --text-primary: rgba(255,255,255, 0.70);
            --text-secondary: rgba(255,255,255, 0.50);
            --border-radius: 5px;
            --elevation-25: 0px 8px 10px -5px rgba(0,0,0,0.2), 0px 16px 24px 2px rgba(0,0,0,0.14), 0px 6px 30px 5px rgba(0,0,0,0.12);
        }}
        
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        
        body {{
            font-family: 'Roboto', 'Helvetica', 'Arial', sans-serif;
            font-size: 16px;
            line-height: 1.5;
            color: var(--text-primary);
            background-color: var(--background-color);
            background-image: url('https://ohdb2ctemplates.blob.core.windows.net/ohdb2c-templates/cornfield.png');
            background-size: cover;
            background-position: center top;
            background-repeat: no-repeat;
            background-attachment: fixed;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }}
        
        .container {{
            width: 100%;
            max-width: 400px;
            padding: 16px;
        }}
        
        .paper {{
            background-color: var(--surface-color);
            border-radius: var(--border-radius);
            box-shadow: var(--elevation-25);
            padding: 32px;
            text-align: center;
        }}
        
        .error-icon {{
            width: 64px;
            height: 64px;
            margin: 0 auto 16px;
            display: block;
            fill: var(--error-color);
        }}
        
        h1 {{
            font-size: 2.125rem;
            font-weight: 400;
            line-height: 1.235;
            letter-spacing: 0.00735em;
            margin-bottom: 16px;
        }}
        
        .error-message {{
            font-size: 1rem;
            line-height: 1.5;
            letter-spacing: 0.00938em;
            color: var(--text-secondary);
            margin-bottom: 24px;
        }}
        
        .button {{
            display: inline-block;
            padding: 8px 16px;
            color: var(--primary-color);
            text-decoration: none;
            border-radius: var(--border-radius);
            font-size: 0.875rem;
            font-weight: 500;
            letter-spacing: 0.02857em;
            text-transform: uppercase;
            border: 1px solid var(--primary-color);
            transition: background-color 250ms cubic-bezier(0.4, 0, 0.2, 1) 0ms;
        }}
        
        .button:hover {{
            background-color: rgba(76, 175, 80, 0.08);
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='paper'>
            <svg class='error-icon' viewBox='0 0 24 24'>
                <path d='M13,13H11V7H13M13,17H11V15H13M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z' />
            </svg>
            <h1>{title}</h1>
            <p class='error-message'>{message}</p>
            <a href='{clientOrigin}login' class='button'>Go to Login</a>
        </div>
    </div>
</body>
</html>";
    }
}