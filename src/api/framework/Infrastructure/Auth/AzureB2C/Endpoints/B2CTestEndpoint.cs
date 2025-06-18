using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Auth.AzureB2C.Endpoints;

public static class B2CTestEndpoint
{
    internal static RouteHandlerBuilder MapB2CTestEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/api/public/test-invitation-page", async (HttpContext context) =>
            {
                // Override CSP for this endpoint to allow Google Fonts
                context.Response.Headers.ContentSecurityPolicy = "default-src 'self'; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; font-src 'self' https://fonts.gstatic.com; img-src 'self' data:; script-src 'self' 'unsafe-inline';";
                
                var html = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Test Invitation Page</title>
    <link href='https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap' rel='stylesheet' />
    <style>
        :root {
            --primary-color: #1976d2;
            --primary-dark: #115293;
            --secondary-color: #424242;
            --background-color: #f5f5f5;
            --surface-color: #ffffff;
            --error-color: #f44336;
            --text-primary: rgba(0, 0, 0, 0.87);
            --text-secondary: rgba(0, 0, 0, 0.6);
            --border-radius: 4px;
            --elevation-1: 0px 2px 1px -1px rgba(0,0,0,0.2), 0px 1px 1px 0px rgba(0,0,0,0.14), 0px 1px 3px 0px rgba(0,0,0,0.12);
            --elevation-25: 0px 8px 10px -5px rgba(0,0,0,0.2), 0px 16px 24px 2px rgba(0,0,0,0.14), 0px 6px 30px 5px rgba(0,0,0,0.12);
        }
        
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: 'Roboto', 'Helvetica', 'Arial', sans-serif;
            font-size: 16px;
            line-height: 1.5;
            color: var(--text-primary);
            background-color: var(--background-color);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .container {
            width: 100%;
            max-width: 400px;
            padding: 16px;
        }
        
        .paper {
            background-color: var(--surface-color);
            border-radius: var(--border-radius);
            box-shadow: var(--elevation-25);
            padding: 32px;
        }
        
        .text-center {
            text-align: center;
        }
        
        h1 {
            font-size: 2.125rem;
            font-weight: 400;
            line-height: 1.235;
            letter-spacing: 0.00735em;
            margin-bottom: 16px;
        }
        
        .subtitle {
            font-size: 1rem;
            line-height: 1.5;
            letter-spacing: 0.00938em;
            color: var(--text-secondary);
            margin-bottom: 24px;
        }
        
        .button {
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
        }
        
        .button:hover {
            background-color: var(--primary-dark);
            box-shadow: 0px 3px 1px -2px rgba(0,0,0,0.2), 0px 2px 2px 0px rgba(0,0,0,0.14), 0px 1px 5px 0px rgba(0,0,0,0.12);
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='paper text-center'>
            <h1>Test Invitation Page</h1>
            <p class='subtitle'>This is a test page to verify the styling is working correctly.</p>
            <p>If you can see this page with proper styling (centered content, Material-UI style, Roboto font), then the CSS is working correctly.</p>
            <br/>
            <a href='#' class='button'>Test Button</a>
        </div>
    </div>
</body>
</html>";
                
                return Results.Content(html, "text/html");
            })
            .WithName("B2CTestInvitationPage")
            .WithSummary("Test invitation page styling")
            .AllowAnonymous()
            .WithMetadata(new Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute())
            .WithTags("B2C Integration")
            .Produces(200, contentType: "text/html");
    }
}