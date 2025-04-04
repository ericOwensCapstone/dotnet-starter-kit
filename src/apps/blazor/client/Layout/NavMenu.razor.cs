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
    // Start GrowthTreatment
    private bool _canViewGrowthTreatments;
    // End GrowthTreatment
    // Start PreventiveTreatment
    private bool _canViewPreventiveTreatments;
    // End PreventiveTreatment
    // Start LifecycleStage
    private bool _canViewLifecycleStages;
    // End LifecycleStage
    
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
        // Start GrowthTreatment
        _canViewGrowthTreatments = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.GrowthTreatments);
        // End GrowthTreatment
        // Start PreventiveTreatment
        _canViewPreventiveTreatments = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.PreventiveTreatments);
        // End PreventiveTreatment
        // Start LifecycleStage
        _canViewLifecycleStages = await AuthService.HasPermissionAsync(user, FshActions.View, FshResources.LifecycleStages);
        // End LifecycleStage
        
        //TODO ASSIGN CAN VIEWS
    }
}
