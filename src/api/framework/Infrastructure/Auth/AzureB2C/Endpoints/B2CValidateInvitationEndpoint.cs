using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Infrastructure.Identity.Invitations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Auth.AzureB2C.Endpoints;

public static class B2CValidateInvitationEndpoint
{
    internal static RouteHandlerBuilder MapB2CValidateInvitationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // Get a logger for registration diagnostics
        var loggerFactory = endpoints.ServiceProvider.GetService<ILoggerFactory>();
        var regLogger = loggerFactory?.CreateLogger("B2CEndpointRegistration");
        
        regLogger?.LogInformation("=== STARTING B2C ENDPOINT REGISTRATION ===");
        regLogger?.LogInformation("Creating public group with base path: /api/public");
        
        // Create a public group for this endpoint
        var publicGroup = endpoints.MapGroup("/api/public")
            .AllowAnonymous()
            .WithMetadata(new Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute());
            
        regLogger?.LogInformation("Public group created successfully");
        
        // Add a test endpoint to verify the group works
        publicGroup.MapGet("/b2c/test", () =>
        {
            var testLogger = loggerFactory?.CreateLogger("B2CTest");
            testLogger?.LogInformation("=== B2C TEST ENDPOINT HIT ===");
            return Results.Ok("B2C Test Endpoint Working");
        })
        .WithName("B2CTestEndpoint")
        .AllowAnonymous();
        
        regLogger?.LogInformation("Test endpoint registered at: /api/public/b2c/test");
        
        // Add diagnostic endpoint to list all endpoints
        publicGroup.MapGet("/b2c/endpoints", (IEnumerable<EndpointDataSource> endpointSources) =>
        {
            var endpoints = endpointSources.SelectMany(source => source.Endpoints);
            var endpointInfo = endpoints
                .Select(endpoint =>
                {
                    var routeEndpoint = endpoint as RouteEndpoint;
                    return new
                    {
                        DisplayName = endpoint.DisplayName ?? "N/A",
                        Pattern = routeEndpoint?.RoutePattern?.RawText ?? "N/A",
                        Metadata = endpoint.Metadata.Select(m => m.GetType().Name).ToList()
                    };
                })
                .Where(e => e.Pattern.Contains("b2c", StringComparison.OrdinalIgnoreCase))
                .ToList();
                
            return Results.Ok(new { TotalB2CEndpoints = endpointInfo.Count, Endpoints = endpointInfo });
        })
        .WithName("B2CEndpointDiagnostics")
        .AllowAnonymous();
        
        regLogger?.LogInformation("Diagnostic endpoint registered at: /api/public/b2c/endpoints");
        
        var validationEndpoint = publicGroup
            .MapGet("/b2c/invitations/validate/{token}", async (
                string token,
                HttpContext context,
                ILoggerFactory loggerFactory) =>
            {
                // Multiple log points to diagnose where it might fail
                Console.WriteLine($"[CONSOLE] B2C Validation endpoint lambda invoked for token: {token}");
                
                var logger = loggerFactory.CreateLogger("B2CValidateInvitation");
                
                logger.LogInformation("=== B2C VALIDATION ENDPOINT HIT ===");
                logger.LogInformation("Received validation request for token: {Token}", token);
                logger.LogInformation("Request Path: {Path}", context.Request.Path);
                logger.LogInformation("Request Method: {Method}", context.Request.Method);
                
                try
                {
                    logger.LogInformation("Looking up invitation for token: {Token}", token);
                    
                    // Get the service from the request's service provider
                    var invitationService = context.RequestServices.GetRequiredService<AnonymousInvitationService>();
                    
                    var invitation = await invitationService.GetInvitationByTokenAsync(token);
                    
                    if (invitation == null)
                    {
                        logger.LogWarning("Invalid invitation token: {Token}", token);
                        var notFoundResponse = new B2CInvitationValidationResponse
                        {
                            isValid = false,
                            errorMessage = "Invalid invitation token."
                        };
                        logger.LogInformation("Returning invalid invitation response: {@Response}", notFoundResponse);
                        return Results.Ok(notFoundResponse);
                    }
                    
                    logger.LogInformation("Found invitation: Id={Id}, Email={Email}, Status={Status}, TenantId={TenantId}", 
                        invitation.Id, invitation.Email, invitation.Status, invitation.TargetTenantId);

                    if (invitation.Status == InvitationStatus.Accepted)
                    {
                        var acceptedResponse = new B2CInvitationValidationResponse
                        {
                            isValid = false,
                            errorMessage = "This invitation has already been accepted."
                        };
                        logger.LogInformation("Returning response for {Status} invitation: {@Response}", invitation.Status, acceptedResponse);
                        return Results.Ok(acceptedResponse);
                    }

                    if (invitation.Status == InvitationStatus.Cancelled)
                    {
                        var cancelledResponse = new B2CInvitationValidationResponse
                        {
                            isValid = false,
                            errorMessage = "This invitation has been cancelled."
                        };
                        logger.LogInformation("Returning response for {Status} invitation: {@Response}", invitation.Status, cancelledResponse);
                        return Results.Ok(cancelledResponse);
                    }

                    if (invitation.Status == InvitationStatus.Expired || invitation.IsExpired)
                    {
                        var expiredResponse = new B2CInvitationValidationResponse
                        {
                            isValid = false,
                            errorMessage = "This invitation has expired."
                        };
                        logger.LogInformation("Returning response for {Status} invitation: {@Response}", invitation.Status, expiredResponse);
                        return Results.Ok(expiredResponse);
                    }

                    if (invitation.Status != InvitationStatus.Sent)
                    {
                        var notReadyResponse = new B2CInvitationValidationResponse
                        {
                            isValid = false,
                            errorMessage = "This invitation is not ready to be accepted."
                        };
                        logger.LogInformation("Returning response for {Status} invitation: {@Response}", invitation.Status, notReadyResponse);
                        return Results.Ok(notReadyResponse);
                    }

                    var response = new B2CInvitationValidationResponse
                    {
                        isValid = true,
                        email = "test@example.com",  // Mock email
                        firstName = invitation.FirstName,
                        lastName = invitation.LastName,
                        displayName = invitation.DisplayName,
                        targetTenantId = invitation.TargetTenantId,
                        expiresAt = invitation.ExpiresAt,
                        invitedBy = invitation.InvitedBy,
                        errorMessage = null,
                        b2cUserId = invitation.B2CUserId  // Real B2C User ID
                    };

                    logger.LogInformation("Valid invitation found for email: {Email}, tenant: {TenantId}", 
                        invitation.Email, invitation.TargetTenantId);
                    
                    logger.LogInformation("Returning successful validation response: {@Response}", response);

                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during B2C invitation validation for token: {Token}", token);
                    logger.LogError("Exception type: {ExceptionType}, Message: {Message}", ex.GetType().Name, ex.Message);
                    if (ex.InnerException != null)
                    {
                        logger.LogError("Inner exception: {InnerExceptionType}, Message: {InnerMessage}", 
                            ex.InnerException.GetType().Name, ex.InnerException.Message);
                    }
                    
                    // B2C expects a 200 OK response even for errors
                    var errorResponse = new B2CInvitationValidationResponse
                    {
                        isValid = false,
                        errorMessage = "Internal server error during invitation validation"
                    };
                    logger.LogInformation("Returning error response: {@Response}", errorResponse);
                    return Results.Ok(errorResponse);
                }
            })
            .WithName("B2CValidateInvitation")
            .WithSummary("Validate invitation token for B2C integration")
            .WithDescription("B2C custom policy calls this endpoint to validate invitation tokens during signup flow")
            .ExcludeFromDescription()
            .AllowAnonymous()
            .Produces<B2CInvitationValidationResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("B2C Integration");
            
        regLogger?.LogInformation("Validation endpoint registered at: /api/public/b2c/invitations/validate/{{token}}");
        regLogger?.LogInformation("=== B2C ENDPOINT REGISTRATION COMPLETE ===");
        
        return validationEndpoint;
    }
}

public class B2CInvitationValidationResponse
{
    // B2C expects these exact property names (case-sensitive)
    public bool isValid { get; set; }
    public string? email { get; set; }
    public string? firstName { get; set; }
    public string? lastName { get; set; }
    public string? displayName { get; set; }
    public string? targetTenantId { get; set; }
    public DateTime? expiresAt { get; set; }
    public string? invitedBy { get; set; }
    public string? errorMessage { get; set; }
    public string? b2cUserId { get; set; }  // Add B2C User ID
}