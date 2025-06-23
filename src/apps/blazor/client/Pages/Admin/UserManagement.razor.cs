using FSH.Starter.Blazor.Client.Components;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Infrastructure.Auth;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Admin;

public partial class UserManagement
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    private UserDetail? _selectedUser;
    private string? _selectedUserTenantName;
    private List<string>? _userRoles;
    private OperationResult? _operationResult;
    private bool _isDeleting;
    private bool _isPurging;
    private bool _hasPermission;
    private Dictionary<string, UserWithTenantDetail> _userTenantMap = new();

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _hasPermission = await AuthService.HasPermissionAsync(state.User, FshActions.ManageAll, FshResources.Users);
        
        if (!_hasPermission)
        {
            Toast.Add("You do not have permission to manage all users.", Severity.Error);
            Navigation.NavigateTo("/");
        }
    }

    private async Task<IEnumerable<UserDetail>> SearchUsersAsync(string searchString, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchString) || searchString.Length < 2)
        {
            return new List<UserDetail>();
        }

        try
        {
            // Use the cross-tenant search endpoint
            var searchResults = await ApiClient.SearchUsersAcrossTenantsEndpointAsync(searchString, 20, cancellationToken);
            
            // Clear and rebuild the tenant map
            _userTenantMap.Clear();
            foreach (var user in searchResults)
            {
                _userTenantMap[user.Id] = user;
            }
            
            // Convert UserWithTenantDetail to UserDetail
            return searchResults.Select(u => new UserDetail
            {
                Id = Guid.Parse(u.Id),
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                EmailConfirmed = u.EmailConfirmed
            });
        }
        catch
        {
            return new List<UserDetail>();
        }
    }

    private async Task OnUserSelected(UserDetail user)
    {
        _selectedUser = user;
        
        // Get tenant information from the map
        if (user != null && _userTenantMap.TryGetValue(user.Id.ToString(), out var userWithTenant))
        {
            _selectedUserTenantName = userWithTenant.TenantName;
        }
        else
        {
            _selectedUserTenantName = null;
        }
        
        await LoadUserRoles();
    }

    private async Task LoadUserRoles()
    {
        if (_selectedUser != null)
        {
            _userRoles = null;
            _operationResult = null;
            
            try
            {
                // Get user roles across tenants
                var rolesResponse = await ApiClient.GetUserRolesAcrossTenantsEndpointAsync(_selectedUser.Id.ToString());
                _userRoles = rolesResponse.Select(r => r.RoleName).ToList();
            }
            catch (Exception ex)
            {
                Toast.Add($"Failed to load user roles: {ex.Message}", Severity.Error);
            }
        }
    }

    private async Task ShowDeleteUserDialog()
    {
        if (_selectedUser == null) return;

        bool? result = await DialogService.ShowMessageBox(
            "Delete User Completely",
            $"Are you sure you want to permanently delete the user '{_selectedUser.Email}'? This will remove them from both the database and Azure B2C.",
            yesText: "Delete Permanently", cancelText: "Cancel");
            
        if (result == true)
        {
            await DeleteUserCompletelyAsync();
        }
    }

    private async Task DeleteUserCompletelyAsync()
    {
        if (_selectedUser == null) return;

        _isDeleting = true;
        _operationResult = null;

        try
        {
            var deleteCommand = new DeleteUserCompletelyCommand
            {
                UserId = _selectedUser.Id.ToString(),
                DeleteFromB2C = true // Always try to delete from B2C
            };

            var response = await ApiClient.DeleteUserCompletelyEndpointAsync(deleteCommand);
            
            _operationResult = new OperationResult
            {
                Success = response.Success,
                Message = response.Message,
                Details = response.Details?.Select(d => new OperationDetail 
                { 
                    Success = d.Success, 
                    Message = d.Message 
                }).ToList()
            };
            
            if (response.Success)
            {
                _selectedUser = null;
                _userRoles = null;
                Toast.Add("User deleted successfully.", Severity.Success);
            }
            else
            {
                Toast.Add($"Failed to delete user: {response.Message}", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            _operationResult = new OperationResult
            {
                Success = false,
                Message = $"Error deleting user: {ex.Message}"
            };
            Toast.Add($"Error deleting user: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isDeleting = false;
        }
    }

    private async Task ShowPurgeB2CUsersDialog()
    {
        bool? result = await DialogService.ShowMessageBox(
            "Purge B2C Deleted Users",
            "This will permanently delete ALL users that have been soft-deleted in Azure B2C. This action cannot be undone. Are you sure you want to continue?",
            yesText: "Purge All Deleted Users", cancelText: "Cancel");
            
        if (result == true)
        {
            await PurgeAllDeletedB2CUsersAsync();
        }
    }

    private async Task PurgeAllDeletedB2CUsersAsync()
    {
        _isPurging = true;
        _operationResult = null;

        try
        {
            var response = await ApiClient.PurgeAllDeletedB2CUsersEndpointAsync();
            
            _operationResult = new OperationResult
            {
                Success = response.Success,
                Message = response.Success 
                    ? $"Successfully purged {response.TotalCount} users." 
                    : "Purge completed with errors.",
                Details = response.Results?.Select(r => new OperationDetail 
                { 
                    Success = r.Success, 
                    Message = r.Success ? $"Purged: {r.Email}" : $"Failed to purge {r.Email}: {r.Error}" 
                }).ToList()
            };
            
            if (response.Success)
            {
                Toast.Add($"Successfully purged {response.TotalCount} users.", Severity.Success);
            }
            else
            {
                Toast.Add($"Purge completed with errors. Check the results below.", Severity.Warning);
            }
        }
        catch (Exception ex)
        {
            _operationResult = new OperationResult
            {
                Success = false,
                Message = $"Error purging B2C users: {ex.Message}"
            };
            Toast.Add($"Error purging B2C users: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isPurging = false;
        }
    }

    private class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<OperationDetail>? Details { get; set; }
    }

    private class OperationDetail
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

}