using FSH.Starter.Blazor.Infrastructure.Auth;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Client.Layout;

public partial class NavMenu
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    protected IServiceProvider ServiceProvider { get; set; } = default!;

    private bool _canViewHangfire;
    private bool _canViewDashboard;
    private bool _canViewRoles;
    private bool _canViewUsers;
    private bool _canViewProducts;
    private bool _canViewBrands;
    private bool _canViewTodos;
    private bool _canViewTenants;
    private bool _canViewAuditTrails;
    private bool _canViewRations;
    private bool _canViewUserInvitations;
    // Start MemberPage;
    private bool _canViewMemberPages;
    // End MemberPage;
    // Start ContractStatus;
    private bool _canViewContractStatuses;
    // End ContractStatus;
    // Start Contract;
    private bool _canViewContracts;
    // End Contract;
    // Start HarvestMember;
    private bool _canViewHarvestMembers;
    // End HarvestMember;
    // Start HarvestContractStatus;
    private bool _canViewHarvestContractStatuses;
    // End HarvestContractStatus;
    // Start HarvestContract;
    private bool _canViewHarvestContracts;
    // End HarvestContract;
    //TODO ADD CAN VIEWS

    private bool CanViewAdministrationGroup => _canViewUsers || _canViewRoles || _canViewTenants || _canViewUserInvitations;

    
    protected override async Task OnParametersSetAsync()
    {
        var user = (await AuthState).User;
        
        // Only check permissions if user is fully authenticated and not in B2C auth flow
        if (user.Identity?.IsAuthenticated == true && !IsInAuthenticationFlow())
        {
            await LoadPermissionsAsync(user);
        }
        else
        {
            // Reset all permissions to false during authentication flow
            ResetPermissions();
        }
    }

    private bool IsInAuthenticationFlow()
    {
        var currentUri = Navigation.Uri;
        
        // Check if we're in any authentication-related flow
        if (currentUri.Contains("/authentication/") || 
            currentUri.Contains("access_token") || 
            currentUri.Contains("id_token") ||
            currentUri.Contains("/accept-invitation"))
        {
            return true;
        }

        // Also check if using B2C and we haven't completed authentication yet
        var authConfig = ServiceProvider.GetService<IAuthenticationConfigurationService>();
        if (authConfig?.IsAzureB2C() == true)
        {
            // For B2C, also defer if we're on a page that might be loaded during auth flow
            // and the user identity doesn't have sufficient claims yet
            var user = AuthState.Result?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                // Check if user has basic claims - if not, we might still be in auth flow
                return !user.HasClaim(c => c.Type == "sub") && 
                       !user.HasClaim(c => c.Type == "email") && 
                       !user.HasClaim(c => c.Type == "name");
            }
        }

        return false;
    }

    private async Task LoadPermissionsAsync(System.Security.Claims.ClaimsPrincipal user)
    {
        _canViewHangfire = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Hangfire);
        _canViewDashboard = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Dashboard);
        _canViewRoles = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Roles);
        _canViewUsers = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Users);
        _canViewProducts = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Products);
        _canViewBrands = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Brands);
        _canViewTodos = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Todos);
        _canViewTenants = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Tenants);
        _canViewAuditTrails = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.AuditTrails);
        _canViewRations = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Rations);
        _canViewUserInvitations = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.UserInvitations);
 
        // Start MemberPage;
        _canViewMemberPages = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.MemberPages);
        // End MemberPage;
        // Start ContractStatus;
        _canViewContractStatuses = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.ContractStatuses);
        // End ContractStatus;
        // Start Contract;
        _canViewContracts = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.Contracts);
        // End Contract;
        // Start HarvestMember;
        _canViewHarvestMembers = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.HarvestMembers);
        // End HarvestMember;
        // Start HarvestContractStatus;
        _canViewHarvestContractStatuses = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.HarvestContractStatuses);
        // End HarvestContractStatus;
        // Start HarvestContract;
        _canViewHarvestContracts = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.HarvestContracts);
        // End HarvestContract;
        //TODO ASSIGN CAN VIEWS
    }

    private void ResetPermissions()
    {
        _canViewHangfire = false;
        _canViewDashboard = false;
        _canViewRoles = false;
        _canViewUsers = false;
        _canViewProducts = false;
        _canViewBrands = false;
        _canViewTodos = false;
        _canViewTenants = false;
        _canViewAuditTrails = false;
        _canViewRations = false;
        _canViewUserInvitations = false;
        _canViewMemberPages = false;
        _canViewContractStatuses = false;
        _canViewContracts = false;
        _canViewHarvestMembers = false;
        _canViewHarvestContractStatuses = false;
        _canViewHarvestContracts = false;
    }
}
