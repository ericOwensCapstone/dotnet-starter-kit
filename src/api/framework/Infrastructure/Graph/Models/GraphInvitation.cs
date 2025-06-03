namespace FSH.Framework.Infrastructure.Graph.Models;

public class GraphInvitation
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string InvitedBy { get; set; } = string.Empty;
    public string? RedirectUrl { get; set; }
    public bool SendInvitationMessage { get; set; } = true;
    public string? CustomizedMessageBody { get; set; }
}