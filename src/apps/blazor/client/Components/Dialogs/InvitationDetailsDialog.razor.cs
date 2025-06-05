using FSH.Starter.Blazor.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Components.Dialogs;

public partial class InvitationDetailsDialog
{
    [CascadingParameter] 
    IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] 
    public UserInvitation Invitation { get; set; } = default!;

    private void Close() => MudDialog.Close(DialogResult.Ok(true));

    private string GetStatusDisplayName()
    {
        return Invitation.Status switch
        {
            InvitationStatus._0 => "Pending",
            InvitationStatus._1 => "Sent",
            InvitationStatus._2 => "Accepted",
            InvitationStatus._3 => "Expired",
            InvitationStatus._4 => "Cancelled",
            InvitationStatus._5 => "Failed",
            _ => Invitation.Status.ToString()
        };
    }

    private Color GetStatusColor()
    {
        return Invitation.Status switch
        {
            InvitationStatus._0 => Color.Warning,  // Pending
            InvitationStatus._1 => Color.Info,     // Sent
            InvitationStatus._2 => Color.Success,  // Accepted
            InvitationStatus._3 => Color.Error,    // Expired
            InvitationStatus._4 => Color.Default,  // Cancelled
            InvitationStatus._5 => Color.Error,    // Failed
            _ => Color.Default
        };
    }

    private bool IsExpired() => DateTime.UtcNow > Invitation.ExpiresAt;

    private string GetExpirationStyle()
    {
        if (IsExpired())
            return "color: var(--mud-palette-error);";
        
        var timeUntilExpiry = Invitation.ExpiresAt - DateTime.UtcNow;
        if (timeUntilExpiry.TotalDays < 1)
            return "color: var(--mud-palette-warning);";
        
        return "";
    }

    private string GetTimeRemaining()
    {
        var timeUntilExpiry = Invitation.ExpiresAt - DateTime.UtcNow;
        
        if (timeUntilExpiry.TotalDays < 0)
            return "Expired";
        
        if (timeUntilExpiry.TotalDays >= 1)
            return $"{Math.Floor(timeUntilExpiry.TotalDays)} day(s)";
        
        if (timeUntilExpiry.TotalHours >= 1)
            return $"{Math.Floor(timeUntilExpiry.TotalHours)} hour(s)";
        
        return $"{Math.Floor(timeUntilExpiry.TotalMinutes)} minute(s)";
    }

    private string GetMaskedToken()
    {
        if (string.IsNullOrEmpty(Invitation.InvitationToken))
            return "N/A";
        
        // Show first 8 and last 4 characters, mask the middle
        if (Invitation.InvitationToken.Length <= 12)
            return Invitation.InvitationToken;
        
        var start = Invitation.InvitationToken[..8];
        var end = Invitation.InvitationToken[^4..];
        var masked = new string('*', Math.Min(16, Invitation.InvitationToken.Length - 12));
        
        return $"{start}{masked}{end}";
    }
}