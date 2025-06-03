namespace FSH.Framework.Infrastructure.Graph.Models;

public class GraphUser
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string Mail { get; set; } = string.Empty;
    public string UserPrincipalName { get; set; } = string.Empty;
    public bool AccountEnabled { get; set; }
    public Dictionary<string, object> CustomAttributes { get; set; } = new();
    
    // B2C Custom Attributes
    public string? TenantId { get; set; }
    public string? InvitedBy { get; set; }
    public DateTime? InvitationDate { get; set; }
    public string? UserStatus { get; set; }
}