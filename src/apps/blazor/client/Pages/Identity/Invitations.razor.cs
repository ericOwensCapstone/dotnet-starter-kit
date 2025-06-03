using FSH.Starter.Blazor.Client.Components;
using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Infrastructure.Auth;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Identity;

public partial class Invitations
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;
    private string? _searchString;
    protected EntityClientTableContext<UserInvitationViewModel, Guid, CreateInvitationRequest> Context { get; set; } = default!;
    private List<UserInvitationViewModel> _invitations = new();
    public EntityTable<UserInvitationViewModel, Guid, CreateInvitationRequest> EntityTable { get; set; } = default!;
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    private bool _isRootAdmin;
    private string? _currentUserTenant;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _isRootAdmin = state.User.IsInRole(FshRoles.Admin);
        _currentUserTenant = state.User.GetTenant();

        Context = new(
            entityName: "Invitation",
            entityNamePlural: "Invitations",
            entityResource: FshResources.UserInvitations,
            searchAction: FshActions.View,
            deleteAction: string.Empty,
            updateAction: string.Empty,
            createAction: FshActions.Create,
            fields: new()
            {
                new(invitation => invitation.Email, "Email"),
                new(invitation => invitation.DisplayName, "Display Name"),
                new(invitation => invitation.Status, "Status", Type: typeof(InvitationStatus)),
                new(invitation => invitation.TenantId, "Tenant"),
                new(invitation => invitation.Role ?? "N/A", "Role"),
                new(invitation => invitation.ExpiresAt.ToString("MMM dd, yyyy HH:mm"), "Expires At"),
                new(invitation => invitation.Created.ToString("MMM dd, yyyy HH:mm"), "Created")
            },
            loadDataFunc: async () => 
            {
                var invitations = await ApiClient.GetInvitationsByTenantAsync(_isRootAdmin ? null : _currentUserTenant);
                _invitations = invitations.Adapt<List<UserInvitationViewModel>>();
                return _invitations;
            },
            searchFunc: (searchString, invitation) =>
                string.IsNullOrWhiteSpace(searchString)
                    || invitation.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    || invitation.DisplayName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    || (!string.IsNullOrEmpty(invitation.FirstName) && invitation.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    || (!string.IsNullOrEmpty(invitation.LastName) && invitation.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase)),
            createFunc: async invitation => 
            {
                var command = new CreateInvitationRequest
                {
                    Email = invitation.Email,
                    DisplayName = invitation.DisplayName,
                    FirstName = invitation.FirstName,
                    LastName = invitation.LastName,
                    TenantId = _isRootAdmin ? invitation.TenantId : _currentUserTenant!,
                    Role = invitation.Role,
                    SendInvitationEmail = true
                };
                
                await ApiClient.CreateInvitationAsync(command);
            },
            hasExtraActionsFunc: () => true,
            exportAction: string.Empty);
    }

    private void ViewInvitationDetails(Guid id)
    {
        var invitation = _invitations.First(f => f.Id == id);
        invitation.ShowDetails = !invitation.ShowDetails;
        foreach (var otherInvitation in _invitations.Except(new[] { invitation }))
        {
            otherInvitation.ShowDetails = false;
        }
    }

    private async Task ResendInvitationAsync(Guid id)
    {
        var result = await ApiHelper.ExecuteCallGuardedAsync(
            () => ApiClient.ResendInvitationAsync(id),
            Toast, Navigation,
            null,
            "Invitation resent successfully.");
        if (result)
        {
            await EntityTable.ReloadDataAsync();
        }
    }

    private async Task CancelInvitationAsync(Guid id)
    {
        var result = await ApiHelper.ExecuteCallGuardedAsync(
            () => ApiClient.CancelInvitationAsync(id),
            Toast, Navigation,
            null,
            "Invitation cancelled.");
        if (result)
        {
            await EntityTable.ReloadDataAsync();
        }
    }

    public class UserInvitationViewModel : UserInvitation
    {
        public bool ShowDetails { get; set; }
    }
}