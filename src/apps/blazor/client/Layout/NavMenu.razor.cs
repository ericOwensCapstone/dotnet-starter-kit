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

    private bool CanViewAdministrationGroup => _canViewUsers || _canViewRoles || _canViewTenants;

    
    protected override async Task OnParametersSetAsync()
    {
        var user = (await AuthState).User;
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
}
