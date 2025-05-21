# Claude Code Sessions

## Root Tenant MemberPage Deletion Permissions (2025-01-21)

### Summary
Implemented permission restrictions so only the root tenant can delete MemberPages, while all other tenants cannot delete their own or other MemberPages.

### Changes Made

#### 1. FshPermissions.cs
- Added `IsRoot: true` flag to "Delete MemberPages" permission
- This restricts the permission to root tenant only

#### 2. DeleteMemberPageEndpoint.cs
- Added tenant validation check in the endpoint
- Returns 403 Forbidden for non-root tenants
- Added required imports for tenant context access

#### 3. MemberPage.cs (Domain)
- Updated CanBeSoftDeleted method with documentation
- Clarified that tenant validation is handled at endpoint/permission levels

#### 4. FshDbContext.cs
- Added FSH.Starter.Shared.Authorization import
- Modified SaveChangesAsync to allow root tenant to delete any MemberPage
- Added special case handling for root tenant + MemberPage combination

#### 5. MemberPages.razor.cs (Blazor UI)
- Updated canDeleteEntityFunc to only show delete option for root tenant
- Changed from tenant ownership check to root tenant check

### Security Layers Implemented
1. **Permission Level**: IsRoot flag restricts permission to root tenant
2. **API Endpoint**: Explicit tenant validation returns 403 for non-root
3. **Database Layer**: Special case allows root tenant cross-tenant deletion
4. **UI Layer**: Delete button only visible to root tenant

### Testing
- Root tenant: Can see and use delete functionality
- Other tenants: Cannot see delete option in UI, API returns 403 if accessed directly

### Detailed File Changes

#### 1. `/src/Shared/Authorization/FshPermissions.cs`
**Line 50** - Modified the "Delete MemberPages" permission:
```csharp
// BEFORE:
new("Delete MemberPages", FshActions.Delete, FshResources.MemberPages),

// AFTER:
new("Delete MemberPages", FshActions.Delete, FshResources.MemberPages, IsRoot: true),
```

#### 2. `/src/api/modules/Ranch/Ranch.Infrastructure/Endpoints/v1/MemberPages/DeleteMemberPageEndpoint.cs`
**Added imports:**
```csharp
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.Shared.Authorization;
```

**Modified the endpoint handler (lines 17-27):**
```csharp
// BEFORE:
.MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
{
    await mediator.Send(new DeleteMemberPageCommand(id));
    return Results.NoContent();
})

// AFTER:
.MapDelete("/{id:guid}", async (Guid id, ISender mediator, IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor) =>
{
    // Validate that only root tenant can delete MemberPages
    var tenantId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id;
    if (tenantId != TenantConstants.Root.Id)
    {
        return Results.Forbid();
    }
    
    await mediator.Send(new DeleteMemberPageCommand(id));
    return Results.NoContent();
})
```

**Added status code documentation:**
```csharp
.Produces(StatusCodes.Status403Forbidden)
```

#### 3. `/src/api/modules/Ranch/Ranch.Domain/MemberPages/MemberPage.cs`
**Updated CanBeSoftDeleted method (lines 68-79):**
```csharp
// BEFORE:
public override (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
{
    bool canBeDeleted = true;
    string reason = string.Empty;
    return (canBeDeleted, reason);
}

// AFTER:
public override (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
{
    // Tenant validation is handled in the endpoint and via permissions
    // This is just a safety check for any entities that should never be deleted
    
    // Additional validation logic could be added here if needed,
    // such as checking if MemberPage is referenced by important entities
    
    bool canBeDeleted = true;
    string reason = string.Empty;
    return (canBeDeleted, reason);
}
```

#### 4. `/src/api/framework/Infrastructure/Persistence/FshDbContext.cs`
**Added import (line 9):**
```csharp
using FSH.Starter.Shared.Authorization;
```

**Modified SaveChangesAsync deletion check (lines 182-192):**
```csharp
// BEFORE:
else if (entry.Entity is ITenantEntity tenantEntity3 && entry.State == EntityState.Deleted)
{
    if (tenantEntity3.TenantId != tenantId)
    {
        var entityClrType = entry.Entity.GetType();
        throw new UnauthorizedAccessException($"You cannot delete another tenant's {entityClrType.Name} entity.");
    }
}

// AFTER:
else if (entry.Entity is ITenantEntity tenantEntity3 && entry.State == EntityState.Deleted)
{
    // Special case: Allow root tenant to delete any MemberPage
    bool isRootTenant = tenantId == TenantConstants.Root.Id;
    bool isMemberPage = entry.Entity.GetType().Name == "MemberPage";
    
    if (tenantEntity3.TenantId != tenantId && !(isRootTenant && isMemberPage))
    {
        var entityClrType = entry.Entity.GetType();
        throw new UnauthorizedAccessException($"You cannot delete another tenant's {entityClrType.Name} entity.");
    }
}
```

#### 5. `/src/apps/blazor/client/Pages/Ranch/MemberPages.razor.cs`
**Modified canDeleteEntityFunc (line 80):**
```csharp
// BEFORE:
canDeleteEntityFunc: ad => ad.TenantId == CurrentTenantId

// AFTER:
canDeleteEntityFunc: ad => CurrentTenantId == "root" // Only root tenant can delete MemberPages
```

### Files Modified Summary
- `/src/Shared/Authorization/FshPermissions.cs` - 1 line changed
- `/src/api/modules/Ranch/Ranch.Infrastructure/Endpoints/v1/MemberPages/DeleteMemberPageEndpoint.cs` - 18 lines changed
- `/src/api/modules/Ranch/Ranch.Domain/MemberPages/MemberPage.cs` - 6 lines changed  
- `/src/api/framework/Infrastructure/Persistence/FshDbContext.cs` - 6 lines changed
- `/src/apps/blazor/client/Pages/Ranch/MemberPages.razor.cs` - 1 line changed

**Total: 32 lines modified across 5 files**