using System.Collections.ObjectModel;

namespace FSH.Starter.WebApi.Shared.Authorization;

public static class FshAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
    public const string Generate = nameof(Generate);
    public const string Clean = nameof(Clean);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
}

public static class FshResource
{
    public const string Tenants = nameof(Tenants);
    public const string Dashboard = nameof(Dashboard);
    public const string Hangfire = nameof(Hangfire);
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);
    public const string Products = nameof(Products);
    public const string Todos = nameof(Todos);
    public const string AuditTrails = nameof(AuditTrails);
    //TODO FSHRESOURCES
    public const string Rations = nameof(Rations);
    public const string GrowthTreatments = nameof(GrowthTreatments);
    public const string PreventativeTreatments = nameof(PreventativeTreatments);
    public const string LifecycleStages = nameof(LifecycleStages);
    public const string LifecyclePrograms = nameof(LifecyclePrograms);
    public const string AnimalTypes = nameof(AnimalTypes);
}

public static class FshPermissions
{
    private static readonly FshPermission[] allPermissions =
   {     
        //tenants
        new("View Tenants", FshAction.View, FshResource.Tenants, IsRoot: true),
        new("Create Tenants", FshAction.Create, FshResource.Tenants, IsRoot: true),
        new("Update Tenants", FshAction.Update, FshResource.Tenants, IsRoot: true),
        new("Upgrade Tenant Subscription", FshAction.UpgradeSubscription, FshResource.Tenants, IsRoot: true),

        //identity
        new("View Users", FshAction.View, FshResource.Users),
        new("Search Users", FshAction.Search, FshResource.Users),
        new("Create Users", FshAction.Create, FshResource.Users),
        new("Update Users", FshAction.Update, FshResource.Users),
        new("Delete Users", FshAction.Delete, FshResource.Users),
        new("Export Users", FshAction.Export, FshResource.Users),
        new("View UserRoles", FshAction.View, FshResource.UserRoles),
        new("Update UserRoles", FshAction.Update, FshResource.UserRoles),
        new("View Roles", FshAction.View, FshResource.Roles),
        new("Create Roles", FshAction.Create, FshResource.Roles),
        new("Update Roles", FshAction.Update, FshResource.Roles),
        new("Delete Roles", FshAction.Delete, FshResource.Roles),
        new("View RoleClaims", FshAction.View, FshResource.RoleClaims),
        new("Update RoleClaims", FshAction.Update, FshResource.RoleClaims),
        
        //products
        new("View Products", FshAction.View, FshResource.Products, IsBasic: true),
        new("Search Products", FshAction.Search, FshResource.Products, IsBasic: true),
        new("Create Products", FshAction.Create, FshResource.Products),
        new("Update Products", FshAction.Update, FshResource.Products),
        new("Delete Products", FshAction.Delete, FshResource.Products),
        new("Export Products", FshAction.Export, FshResource.Products),

        // TODO FSHPERMISSIONS
        //rations
        new("View Rations", FshAction.View, FshResource.Rations, IsBasic: true),
        new("Search Rations", FshAction.Search, FshResource.Rations, IsBasic: true),
        new("Create Rations", FshAction.Create, FshResource.Rations),
        new("Update Rations", FshAction.Update, FshResource.Rations),
        new("Delete Rations", FshAction.Delete, FshResource.Rations),
        new("Export Rations", FshAction.Export, FshResource.Rations),

        //growthtreatments
        new("View GrowthTreatments", FshAction.View, FshResource.GrowthTreatments, IsBasic: true),
        new("Search GrowthTreatments", FshAction.Search, FshResource.GrowthTreatments, IsBasic: true),
        new("Create GrowthTreatments", FshAction.Create, FshResource.GrowthTreatments),
        new("Update GrowthTreatments", FshAction.Update, FshResource.GrowthTreatments),
        new("Delete GrowthTreatments", FshAction.Delete, FshResource.GrowthTreatments),
        new("Export GrowthTreatments", FshAction.Export, FshResource.GrowthTreatments),

        //preventativetreatments
        new("View PreventativeTreatments", FshAction.View, FshResource.PreventativeTreatments, IsBasic: true),
        new("Search PreventativeTreatments", FshAction.Search, FshResource.PreventativeTreatments, IsBasic: true),
        new("Create PreventativeTreatments", FshAction.Create, FshResource.PreventativeTreatments),
        new("Update PreventativeTreatments", FshAction.Update, FshResource.PreventativeTreatments),
        new("Delete PreventativeTreatments", FshAction.Delete, FshResource.PreventativeTreatments),
        new("Export PreventativeTreatments", FshAction.Export, FshResource.PreventativeTreatments),

        //lifecycleStages
        new("View LifecycleStages", FshAction.View, FshResource.LifecycleStages, IsBasic: true),
        new("Search LifecycleStages", FshAction.Search, FshResource.LifecycleStages, IsBasic: true),
        new("Create LifecycleStages", FshAction.Create, FshResource.LifecycleStages),
        new("Update LifecycleStages", FshAction.Update, FshResource.LifecycleStages),
        new("Delete LifecycleStages", FshAction.Delete, FshResource.LifecycleStages),
        new("Export LifecycleStages", FshAction.Export, FshResource.LifecycleStages),

        //lifecyclePrograms
        new("View LifecyclePrograms", FshAction.View, FshResource.LifecyclePrograms, IsBasic: true),
        new("Search LifecyclePrograms", FshAction.Search, FshResource.LifecyclePrograms, IsBasic: true),
        new("Create LifecyclePrograms", FshAction.Create, FshResource.LifecyclePrograms),
        new("Update LifecyclePrograms", FshAction.Update, FshResource.LifecyclePrograms),
        new("Delete LifecyclePrograms", FshAction.Delete, FshResource.LifecyclePrograms),
        new("Export LifecyclePrograms", FshAction.Export, FshResource.LifecyclePrograms),

        
        //AnimalTypes
        new("View AnimalTypes", FshAction.View, FshResource.AnimalTypes, IsBasic: true),
        new("Search AnimalTypes", FshAction.Search, FshResource.AnimalTypes, IsBasic: true),
        new("Create AnimalTypes", FshAction.Create, FshResource.AnimalTypes),
        new("Update AnimalTypes", FshAction.Update, FshResource.AnimalTypes),
        new("Delete AnimalTypes", FshAction.Delete, FshResource.AnimalTypes),
        new("Export AnimalTypes", FshAction.Export, FshResource.AnimalTypes),

        //todos
        new("View Todos", FshAction.View, FshResource.Todos, IsBasic: true),
        new("Search Todos", FshAction.Search, FshResource.Todos, IsBasic: true),
        new("Create Todos", FshAction.Create, FshResource.Todos),
        new("Update Todos", FshAction.Update, FshResource.Todos),
        new("Delete Todos", FshAction.Delete, FshResource.Todos),

        //audit
        new("View Audit Trails", FshAction.View, FshResource.AuditTrails),
   };

    public static IReadOnlyList<FshPermission> All { get; } = new ReadOnlyCollection<FshPermission>(allPermissions);
    public static IReadOnlyList<FshPermission> Root { get; } = new ReadOnlyCollection<FshPermission>(allPermissions.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<FshPermission> Admin { get; } = new ReadOnlyCollection<FshPermission>(allPermissions.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<FshPermission> Basic { get; } = new ReadOnlyCollection<FshPermission>(allPermissions.Where(p => p.IsBasic).ToArray());
}

public record FshPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource)
    {
        return $"Permissions.{resource}.{action}";
    }
}


