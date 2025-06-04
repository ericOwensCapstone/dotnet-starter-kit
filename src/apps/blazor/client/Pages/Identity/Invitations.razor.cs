using FSH.Starter.Blazor.Client.Components;
using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Client.Pages.Identity;


public partial class Invitations
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<InvitationDto, Guid, CreateInvitationRequest> Context { get; set; } = default!;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    
    private string? _searchString;
    public EntityTable<InvitationDto, Guid, CreateInvitationRequest> EntityTable { get; set; } = default!;
    private string CurrentTenantId { get; set; } = default!;
    private bool _isRootAdmin;
    private List<TenantDetail> _availableTenants = new();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        CurrentTenantId = authState.User.FindFirst("tenant")?.Value ?? string.Empty;
        _isRootAdmin = authState.User.IsInRole(FshRoles.Admin);

        Context = new(
            entityName: "Invitation",
            entityNamePlural: "Invitations",
            entityResource: FshResources.UserInvitations,
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
            enableAdvancedSearch: true,
            idFunc: invitation => invitation.Id,
            searchFunc: async filter =>
            {
                var searchQuery = filter.Adapt<SearchInvitationsQuery>();
                
                // Set tenant filter based on user permissions
                searchQuery.TenantId = _isRootAdmin ? null : CurrentTenantId;
                
                var result = await _client.SearchInvitationsEndpointAsync(searchQuery);
                return result.Adapt<PaginationResponse<InvitationDto>>();
            },
            getDefaultsFunc: async () =>
            {
                // Pre-populate TenantId for tenant admin
                return new CreateInvitationRequest
                {
                    TenantId = _isRootAdmin ? null : CurrentTenantId
                };
            },
            createFunc: async invitation =>
            {
                var command = new CreateInvitationRequest
                {
                    Email = invitation.Email,
                    DisplayName = invitation.DisplayName,
                    FirstName = invitation.FirstName,
                    LastName = invitation.LastName,
                    TenantId = _isRootAdmin ? invitation.TenantId : CurrentTenantId,
                    Role = invitation.Role,
                    SendInvitationEmail = true
                };

                await _client.CreateInvitationAsync(command);
            },
            hasExtraActionsFunc: () => true
        );

        // Load available tenants for root admin after Context initialization
        if (_isRootAdmin)
        {
            try
            {
                _availableTenants = (await _client.GetTenantsEndpointAsync()).ToList();
            }
            catch
            {
                // If API call fails, fall back to empty list
                _availableTenants = new List<TenantDetail>();
            }
        }
    }

    private async Task ResendInvitationAsync(Guid id)
    {
        var result = await ApiHelper.ExecuteCallGuardedAsync(
            () => _client.ResendInvitationAsync(id),
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
            () => _client.CancelInvitationAsync(id),
            Toast, Navigation,
            null,
            "Invitation cancelled.");
        if (result)
        {
            await EntityTable.ReloadDataAsync();
        }
    }
}