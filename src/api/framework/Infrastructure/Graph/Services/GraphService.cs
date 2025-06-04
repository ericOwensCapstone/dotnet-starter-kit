using System.Security.Cryptography;
using Azure.Identity;
using FSH.Framework.Infrastructure.Graph.Models;
using FSH.Framework.Infrastructure.Graph.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace FSH.Framework.Infrastructure.Graph.Services;

public class GraphService : IGraphService
{
    private readonly GraphServiceClient _graphClient;
    private readonly GraphApiOptions _options;

    private readonly ILogger<GraphService> _logger;
    private readonly string _extensionPrefix;

    public GraphService(
        IOptions<GraphApiOptions> options,
        ILogger<GraphService> logger)
    {
        _options = options.Value;
        _logger = logger;
        
        // Log the received configuration values for debugging
        _logger.LogInformation("GraphService configuration - TenantId: {TenantId}, ClientId: {ClientId}, B2CExtensionAppClientId: {B2CExtensionAppClientId}, B2CDomain: {B2CDomain}, ClientSecret: {ClientSecretPresent}",
            _options.TenantId,
            _options.ClientId,
            _options.B2CExtensionAppClientId,
            _options.B2CDomain,
            !string.IsNullOrEmpty(_options.ClientSecret) ? "Present" : "Missing");
        
        // Validate required configuration
        if (string.IsNullOrEmpty(_options.TenantId))
            throw new InvalidOperationException("GraphApi:TenantId is not configured");
        if (string.IsNullOrEmpty(_options.ClientId))
            throw new InvalidOperationException("GraphApi:ClientId is not configured");
        if (string.IsNullOrEmpty(_options.ClientSecret))
            throw new InvalidOperationException("GraphApi:ClientSecret is not configured");
        if (string.IsNullOrEmpty(_options.B2CExtensionAppClientId))
            throw new InvalidOperationException("GraphApi:B2CExtensionAppClientId is not configured");
        if (string.IsNullOrEmpty(_options.B2CDomain))
            throw new InvalidOperationException("GraphApi:B2CDomain is not configured");
        
        _extensionPrefix = $"extension_{_options.B2CExtensionAppClientId}_";

        var clientSecretCredential = new ClientSecretCredential(
            _options.TenantId,
            _options.ClientId,
            _options.ClientSecret);

        _graphClient = new GraphServiceClient(clientSecretCredential, _options.Scopes);
    }

    public async Task<GraphUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _graphClient.Users[userId]
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = GetUserSelectProperties();
                }, cancellationToken);

            return user != null ? MapToGraphUser(user) : null;
        }
        catch (ODataError ex) when (ex.ResponseStatusCode == 404)
        {
            return null;
        }
    }

    public async Task<GraphUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var filter = $"identities/any(id:id/issuer eq '{_options.B2CDomain}' and id/issuerAssignedId eq '{email}')";
        var users = await GetUsersAsync(filter, cancellationToken);
        return users.FirstOrDefault();
    }

    public async Task<List<GraphUser>> GetUsersAsync(string? filter = null, CancellationToken cancellationToken = default)
    {
        var users = new List<User>();
        
        var response = await _graphClient.Users
            .GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select = GetUserSelectProperties();
                if (!string.IsNullOrEmpty(filter))
                {
                    requestConfiguration.QueryParameters.Filter = filter;
                }
                requestConfiguration.QueryParameters.Top = 999; // Maximum allowed
            }, cancellationToken);

        if (response?.Value != null)
        {
            users.AddRange(response.Value);
            
            // Handle pagination
            var pageIterator = PageIterator<User, UserCollectionResponse>
                .CreatePageIterator(_graphClient, response, (user) =>
                {
                    users.Add(user);
                    return true;
                });
            
            await pageIterator.IterateAsync(cancellationToken);
        }

        return users.Select(MapToGraphUser).ToList();
    }

    public async Task<List<GraphUser>> GetUsersByTenantAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = $"{_extensionPrefix}TenantId eq '{tenantId}'";
        return await GetUsersAsync(filter, cancellationToken);
    }

    public async Task<GraphUser> CreateUserAsync(GraphUser graphUser, string temporaryPassword, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            AccountEnabled = false, // Disabled until invitation is accepted
            DisplayName = graphUser.DisplayName,
            GivenName = graphUser.GivenName,
            Surname = graphUser.Surname,
            UserPrincipalName = $"{graphUser.Mail.Replace("@", "_")}#EXT#@{_options.B2CDomain}",
            PasswordProfile = new PasswordProfile
            {
                ForceChangePasswordNextSignIn = false,
                Password = temporaryPassword
            },
            Identities = new List<ObjectIdentity>
            {
                new ObjectIdentity
                {
                    SignInType = "emailAddress",
                    Issuer = _options.B2CDomain,
                    IssuerAssignedId = graphUser.Mail
                }
            }
        };

        // Add custom attributes
        user.AdditionalData = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(graphUser.TenantId))
            user.AdditionalData[$"{_extensionPrefix}TenantId"] = graphUser.TenantId;
        if (!string.IsNullOrEmpty(graphUser.InvitedBy))
            user.AdditionalData[$"{_extensionPrefix}InvitedBy"] = graphUser.InvitedBy;
        if (graphUser.InvitationDate.HasValue)
            user.AdditionalData[$"{_extensionPrefix}InvitationDate"] = graphUser.InvitationDate.Value.ToString("O");
        user.AdditionalData[$"{_extensionPrefix}UserStatus"] = "Invited";

        var createdUser = await _graphClient.Users.PostAsync(user, requestConfiguration => { }, cancellationToken);
        
        if (createdUser == null)
            throw new InvalidOperationException("Failed to create user in B2C");

        return MapToGraphUser(createdUser);
    }

    public async Task<GraphUser> UpdateUserAsync(string userId, GraphUser graphUser, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            DisplayName = graphUser.DisplayName,
            GivenName = graphUser.GivenName,
            Surname = graphUser.Surname
        };

        var updatedUser = await _graphClient.Users[userId].PatchAsync(user, requestConfiguration => { }, cancellationToken);
        
        if (updatedUser == null)
            throw new InvalidOperationException("Failed to update user in B2C");

        return MapToGraphUser(updatedUser);
    }

    public async Task<bool> UpdateUserCustomAttributesAsync(string userId, Dictionary<string, object> attributes, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            AdditionalData = new Dictionary<string, object>()
        };

        foreach (var attribute in attributes)
        {
            var key = attribute.Key.StartsWith(_extensionPrefix) ? attribute.Key : $"{_extensionPrefix}{attribute.Key}";
            user.AdditionalData[key] = attribute.Value;
        }

        try
        {
            await _graphClient.Users[userId].PatchAsync(user, requestConfiguration => { }, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user custom attributes for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _graphClient.Users[userId].DeleteAsync(requestConfiguration => { }, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> EnableUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await SetUserAccountStatus(userId, true, cancellationToken);
    }

    public async Task<bool> DisableUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await SetUserAccountStatus(userId, false, cancellationToken);
    }

    public async Task<string> CreateInvitationAsync(GraphInvitation invitation, CancellationToken cancellationToken = default)
    {
        // For B2C, we create a disabled user and return the user ID
        // The actual invitation email will be sent separately
        var temporaryPassword = await GenerateTemporaryPassword();
        var graphUser = new GraphUser
        {
            DisplayName = invitation.DisplayName,
            Mail = invitation.Email,
            TenantId = invitation.TenantId,
            InvitedBy = invitation.InvitedBy,
            InvitationDate = DateTime.UtcNow,
            UserStatus = "Invited"
        };

        var createdUser = await CreateUserAsync(graphUser, temporaryPassword, cancellationToken);
        return createdUser.Id;
    }

    public async Task<bool> ResetPasswordAsync(string userId, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = new User
            {
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = false,
                    Password = newPassword
                }
            };

            await _graphClient.Users[userId].PatchAsync(user, requestConfiguration => { }, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset password for user {UserId}", userId);
            return false;
        }
    }

    public Task<string> GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%^&*";
        var password = new char[16];
        
        using (var rng = RandomNumberGenerator.Create())
        {
            var data = new byte[16];
            rng.GetBytes(data);
            
            for (int i = 0; i < password.Length; i++)
            {
                password[i] = chars[data[i] % chars.Length];
            }
        }

        return Task.FromResult(new string(password));
    }

    private async Task<bool> SetUserAccountStatus(string userId, bool enabled, CancellationToken cancellationToken)
    {
        try
        {
            var user = new User
            {
                AccountEnabled = enabled
            };

            await _graphClient.Users[userId].PatchAsync(user, requestConfiguration => { }, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set account status for user {UserId}", userId);
            return false;
        }
    }

    private GraphUser MapToGraphUser(User user)
    {
        var graphUser = new GraphUser
        {
            Id = user.Id ?? string.Empty,
            DisplayName = user.DisplayName ?? string.Empty,
            GivenName = user.GivenName,
            Surname = user.Surname,
            Mail = user.Mail ?? user.Identities?.FirstOrDefault(i => i.SignInType == "emailAddress")?.IssuerAssignedId ?? string.Empty,
            UserPrincipalName = user.UserPrincipalName ?? string.Empty,
            AccountEnabled = user.AccountEnabled ?? false
        };

        // Map custom attributes
        if (user.AdditionalData != null)
        {
            graphUser.CustomAttributes = user.AdditionalData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            if (user.AdditionalData.TryGetValue($"{_extensionPrefix}TenantId", out var tenantId))
                graphUser.TenantId = tenantId?.ToString();
            
            if (user.AdditionalData.TryGetValue($"{_extensionPrefix}InvitedBy", out var invitedBy))
                graphUser.InvitedBy = invitedBy?.ToString();
            
            if (user.AdditionalData.TryGetValue($"{_extensionPrefix}InvitationDate", out var invitationDate) && 
                DateTime.TryParse(invitationDate?.ToString(), out var date))
                graphUser.InvitationDate = date;
            
            if (user.AdditionalData.TryGetValue($"{_extensionPrefix}UserStatus", out var userStatus))
                graphUser.UserStatus = userStatus?.ToString();
        }

        return graphUser;
    }

    private string[] GetUserSelectProperties()
    {
        return new[]
        {
            "id",
            "displayName",
            "givenName",
            "surname",
            "mail",
            "userPrincipalName",
            "accountEnabled",
            "identities",
            $"{_extensionPrefix}TenantId",
            $"{_extensionPrefix}InvitedBy",
            $"{_extensionPrefix}InvitationDate",
            $"{_extensionPrefix}UserStatus"
        };
    }
}
