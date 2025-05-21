using System.Collections.ObjectModel;

namespace FSH.Starter.Shared.Authorization;

public static class FshPermissions
{
    private static readonly FshPermission[] AllPermissions =
    [     
        //tenants
        new("View Tenants", FshActions.View, FshResources.Tenants, IsRoot: true),
        new("Create Tenants", FshActions.Create, FshResources.Tenants, IsRoot: true),
        new("Update Tenants", FshActions.Update, FshResources.Tenants, IsRoot: true),
        new("Upgrade Tenant Subscription", FshActions.UpgradeSubscription, FshResources.Tenants, IsRoot: true),

        //identity
        new("View Users", FshActions.View, FshResources.Users),
        new("Search Users", FshActions.Search, FshResources.Users),
        new("Create Users", FshActions.Create, FshResources.Users),
        new("Update Users", FshActions.Update, FshResources.Users),
        new("Delete Users", FshActions.Delete, FshResources.Users),
        new("Export Users", FshActions.Export, FshResources.Users),
        new("View UserRoles", FshActions.View, FshResources.UserRoles),
        new("Update UserRoles", FshActions.Update, FshResources.UserRoles),
        new("View Roles", FshActions.View, FshResources.Roles),
        new("Create Roles", FshActions.Create, FshResources.Roles),
        new("Update Roles", FshActions.Update, FshResources.Roles),
        new("Delete Roles", FshActions.Delete, FshResources.Roles),
        new("View RoleClaims", FshActions.View, FshResources.RoleClaims),
        new("Update RoleClaims", FshActions.Update, FshResources.RoleClaims),
        
        //products
        new("View Products", FshActions.View, FshResources.Products, IsBasic: true),
        new("Search Products", FshActions.Search, FshResources.Products, IsBasic: true),
        new("Create Products", FshActions.Create, FshResources.Products),
        new("Update Products", FshActions.Update, FshResources.Products),
        new("Delete Products", FshActions.Delete, FshResources.Products),
        new("Export Products", FshActions.Export, FshResources.Products),
        //rations
        new("View Rations", FshActions.View, FshResources.Rations, IsBasic: true),
        new("Search Rations", FshActions.Search, FshResources.Rations, IsBasic: true),
        new("Create Rations", FshActions.Create, FshResources.Rations),
        new("Update Rations", FshActions.Update, FshResources.Rations),
        new("Delete Rations", FshActions.Delete, FshResources.Rations),
        new("Export Rations", FshActions.Export, FshResources.Rations),
        // Start MemberPage;
        new("View MemberPages", FshActions.View, FshResources.MemberPages, IsBasic: true),
        new("Search MemberPages", FshActions.Search, FshResources.MemberPages, IsBasic: true),
        new("Create MemberPages", FshActions.Create, FshResources.MemberPages),
        new("Update MemberPages", FshActions.Update, FshResources.MemberPages),
        new("Delete MemberPages", FshActions.Delete, FshResources.MemberPages, IsRoot: true),
        new("Export MemberPages", FshActions.Export, FshResources.MemberPages),
        // End MemberPage;
        // Start ContractStatus;
        new("View ContractStatuses", FshActions.View, FshResources.ContractStatuses, IsRoot: true),
        new("Search ContractStatuses", FshActions.Search, FshResources.ContractStatuses, IsBasic: true),
        new("Create ContractStatuses", FshActions.Create, FshResources.ContractStatuses, IsRoot: true),
        new("Update ContractStatuses", FshActions.Update, FshResources.ContractStatuses, IsRoot: true),
        new("Delete ContractStatuses", FshActions.Delete, FshResources.ContractStatuses, IsRoot: true),
        new("Export ContractStatuses", FshActions.Export, FshResources.ContractStatuses, IsRoot: true),
        // End ContractStatus;
        // Start Contract;
        new("View Contracts", FshActions.View, FshResources.Contracts, IsBasic: true),
        new("Search Contracts", FshActions.Search, FshResources.Contracts, IsBasic: true),
        new("Create Contracts", FshActions.Create, FshResources.Contracts),
        new("Update Contracts", FshActions.Update, FshResources.Contracts),
        new("Delete Contracts", FshActions.Delete, FshResources.Contracts),
        new("Export Contracts", FshActions.Export, FshResources.Contracts),
        // End Contract;
        //TODO PERMISSIONS

        //brands
        new("View Brands", FshActions.View, FshResources.Brands, IsBasic: true),
        new("Search Brands", FshActions.Search, FshResources.Brands, IsBasic: true),
        new("Create Brands", FshActions.Create, FshResources.Brands),
        new("Update Brands", FshActions.Update, FshResources.Brands),
        new("Delete Brands", FshActions.Delete, FshResources.Brands),
        new("Export Brands", FshActions.Export, FshResources.Brands),

        //todos
        new("View Todos", FshActions.View, FshResources.Todos, IsBasic: true),
        new("Search Todos", FshActions.Search, FshResources.Todos, IsBasic: true),
        new("Create Todos", FshActions.Create, FshResources.Todos),
        new("Update Todos", FshActions.Update, FshResources.Todos),
        new("Delete Todos", FshActions.Delete, FshResources.Todos),
        new("Export Todos", FshActions.Export, FshResources.Todos),

         new("View Hangfire", FshActions.View, FshResources.Hangfire),
         new("View Dashboard", FshActions.View, FshResources.Dashboard),

        //audit
        new("View Audit Trails", FshActions.View, FshResources.AuditTrails),
    ];

    public static IReadOnlyList<FshPermission> All { get; } = new ReadOnlyCollection<FshPermission>(AllPermissions);
    public static IReadOnlyList<FshPermission> Root { get; } = new ReadOnlyCollection<FshPermission>(AllPermissions.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<FshPermission> Admin { get; } = new ReadOnlyCollection<FshPermission>(AllPermissions.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<FshPermission> Basic { get; } = new ReadOnlyCollection<FshPermission>(AllPermissions.Where(p => p.IsBasic).ToArray());
}

public record FshPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource)
    {
        return $"Permissions.{resource}.{action}";
    }
}


