using FSH.Starter.Blazor.Client.Components;
using FSH.Starter.Blazor.Client.Components.Dialogs;
using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Identity;


public partial class Invitations
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;
    
    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    protected EntityServerTableContext<InvitationDto, Guid, CreateInvitationRequest> Context { get; set; } = default!;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    
    private string? _searchString;
    public EntityTable<InvitationDto, Guid, CreateInvitationRequest> EntityTable { get; set; } = default!;
    private string CurrentTenantId { get; set; } = default!;
    private bool _isRootAdmin;
    private List<TenantDetail> _availableTenants = new();
    private List<RoleDto> _availableRoles = new();
    private TenantDetail? _selectedTenant;

    private TenantDetail? SelectedTenant
    {
        get => _selectedTenant;
        set
        {
            _selectedTenant = value;
            StateHasChanged();
        }
    }

    // Method to get the current TenantId for the form
    private string GetCurrentTenantId()
    {
        if (_isRootAdmin)
        {
            return _selectedTenant?.Id ?? string.Empty;
        }
        return CurrentTenantId;
    }

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
                new(invitation => GetStatusDisplayName(invitation.Status), "Status"),
                new(invitation => invitation.TargetTenantId, "Target Tenant"),
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
                // Pre-populate TargetTenantId for tenant admin
                var defaults = new CreateInvitationRequest
                {
                    TargetTenantId = _isRootAdmin ? null : CurrentTenantId
                };
                
                // Clear selected tenant for new invitations
                if (_isRootAdmin)
                {
                    _selectedTenant = null;
                }
                
                return defaults;
            },
            createFunc: async invitation =>
            {
                var command = new CreateInvitationRequest
                {
                    Email = invitation.Email,
                    DisplayName = invitation.DisplayName,
                    FirstName = invitation.FirstName,
                    LastName = invitation.LastName,
                    TargetTenantId = _isRootAdmin ? (_selectedTenant?.Id ?? invitation.TargetTenantId) : CurrentTenantId,
                    Role = invitation.Role,
                    SendInvitationEmail = true
                };

                await _client.CreateInvitationAsync(command);
            },
            hasExtraActionsFunc: () => true
        );

        // Note: For autocomplete, we'll load tenants on-demand via SearchTenants method
        // No need to preload all tenants here anymore

        // Load available roles for dropdown
        try
        {
            _availableRoles = (await _client.GetRolesEndpointAsync()).ToList();
        }
        catch
        {
            // If API call fails, fall back to empty list
            _availableRoles = new List<RoleDto>();
        }
    }

    private async Task<IEnumerable<TenantDetail>> SearchTenants(string searchTerm, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Initial load: Show first 10 tenants alphabetically
                var query = new SearchTenantsQuery 
                { 
                    PageNumber = 1, 
                    PageSize = 10, 
                    OrderBy = "name" 
                };
                var result = await _client.SearchTenantsEndpointAsync(query);
                return result.Items ?? new List<TenantDetail>();
            }
            else if (searchTerm.Length >= 2)
            {
                // Search mode: Filter by term, return top 20 matches
                var query = new SearchTenantsQuery 
                { 
                    SearchTerm = searchTerm,
                    PageNumber = 1, 
                    PageSize = 20, 
                    OrderBy = "name" 
                };
                var result = await _client.SearchTenantsEndpointAsync(query);
                return result.Items ?? new List<TenantDetail>();
            }
            
            return new List<TenantDetail>();
        }
        catch
        {
            // If API call fails, fall back to legacy method
            return await SearchTenantsLegacy(searchTerm, cancellationToken);
        }
    }

    private async Task<IEnumerable<TenantDetail>> SearchTenantsLegacy(string searchTerm, CancellationToken cancellationToken)
    {
        // Fallback method using the old GetTenantsEndpointAsync
        try
        {
            var allTenants = await _client.GetTenantsEndpointAsync(cancellationToken);
            var tenantList = allTenants.ToList();
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Initial load: Show first 10 tenants alphabetically
                return tenantList.Where(t => t.IsActive).OrderBy(t => t.Name).Take(10);
            }
            else if (searchTerm.Length >= 2)
            {
                // Search mode: Filter by term
                return tenantList
                    .Where(t => t.IsActive && 
                               (t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                t.Id.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(t => t.Name)
                    .Take(20);
            }
            
            return new List<TenantDetail>();
        }
        catch
        {
            return new List<TenantDetail>();
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

    private async Task ViewInvitationDetailsAsync(Guid id)
    {
        try
        {
            var invitation = await _client.GetInvitationAsync(id);
            var parameters = new DialogParameters
            {
                { nameof(InvitationDetailsDialog.Invitation), invitation }
            };
            
            var options = new DialogOptions
            {
                MaxWidth = MaxWidth.Medium,
                FullWidth = true,
                CloseButton = true
            };

            await DialogService.ShowAsync<InvitationDetailsDialog>("Invitation Details", parameters, options);
        }
        catch (Exception ex)
        {
            Toast.Add($"Failed to load invitation details: {ex.Message}", Severity.Error);
        }
    }

    private async Task CopyInvitationLinkAsync(Guid id)
    {
        try
        {
            var invitation = await _client.GetInvitationAsync(id);
            var invitationUrl = $"{Navigation.BaseUri}accept-invitation?token={invitation.InvitationToken}";
            
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", invitationUrl);
            Toast.Add("Invitation link copied to clipboard!", Severity.Success);
        }
        catch (Exception ex)
        {
            Toast.Add($"Failed to copy invitation link: {ex.Message}", Severity.Error);
        }
    }

    private async Task ExtendInvitationAsync(Guid id)
    {
        try
        {
            var invitation = await _client.GetInvitationAsync(id);
            var parameters = new DialogParameters
            {
                { nameof(ExtendInvitationDialog.InvitationId), id },
                { nameof(ExtendInvitationDialog.CurrentExpiration), invitation.ExpiresAt }
            };
            
            var options = new DialogOptions
            {
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                CloseButton = true
            };

            var dialog = await DialogService.ShowAsync<ExtendInvitationDialog>("Extend Invitation", parameters, options);
            var result = await dialog.Result;
            
            if (!result.Canceled && result.Data is bool success && success)
            {
                await EntityTable.ReloadDataAsync();
                Toast.Add("Invitation expiration extended successfully!", Severity.Success);
            }
        }
        catch (Exception ex)
        {
            Toast.Add($"Failed to extend invitation: {ex.Message}", Severity.Error);
        }
    }
    
    private static string GetStatusDisplayName(InvitationStatus status)
    {
        return status switch
        {
            InvitationStatus._0 => "Pending",
            InvitationStatus._1 => "Sent",
            InvitationStatus._2 => "Accepted",
            InvitationStatus._3 => "Expired",
            InvitationStatus._4 => "Cancelled",
            InvitationStatus._5 => "Failed",
            _ => status.ToString()
        };
    }
}