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
    private bool _canViewGrowthTreatments;
    private bool _canViewPreventiveTreatments;
    private bool _canViewLifecycleStages;
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
        _canViewGrowthTreatments = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.GrowthTreatments);
        _canViewPreventiveTreatments = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.PreventiveTreatments);
        _canViewLifecycleStages = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.LifecycleStages);
        //TODO ASSIGN CAN VIEWS
    }
}
