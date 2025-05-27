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

## Root Tenant HarvestMember Deletion Permissions (2025-05-22)

### Summary
Applied the same permission restrictions from Ranch module's MemberPage to Harvest module's HarvestMember, so only the root tenant can delete HarvestMembers.

### Changes Made

#### 1. FshPermissions.cs
- Added `IsRoot: true` flag to "Delete HarvestMembers" permission
- This restricts the permission to root tenant only

#### 2. DeleteHarvestMemberEndpoint.cs
- Added tenant validation check in the endpoint
- Returns 403 Forbidden for non-root tenants
- Added required imports for tenant context access

#### 3. HarvestMember.cs (Domain)
- Updated CanBeSoftDeleted method with documentation
- Clarified that tenant validation is handled at endpoint/permission levels

#### 4. FshDbContext.cs
- Modified SaveChangesAsync to include HarvestMember in special case
- Allows root tenant to delete any HarvestMember across tenants

#### 5. HarvestMembers.razor.cs (Blazor UI)
- Updated canDeleteEntityFunc to only show delete option for root tenant
- Changed from tenant ownership check to root tenant check

### Security Layers Implemented
1. **Permission Level**: IsRoot flag restricts permission to root tenant
2. **API Endpoint**: Explicit tenant validation returns 403 for non-root
3. **Database Layer**: Special case allows root tenant cross-tenant deletion
4. **UI Layer**: Delete button only visible to root tenant

### Detailed File Changes

#### 1. `/src/Shared/Authorization/FshPermissions.cs`
**Line 82** - Modified the "Delete HarvestMembers" permission:
```csharp
// BEFORE:
new("Delete HarvestMembers", FshActions.Delete, FshResources.HarvestMembers),

// AFTER:
new("Delete HarvestMembers", FshActions.Delete, FshResources.HarvestMembers, IsRoot: true),
```

#### 2. `/src/api/modules/Harvest/Harvest.Infrastructure/Endpoints/v1/HarvestMembers/DeleteHarvestMemberEndpoint.cs`
**Added imports (lines 7-9):**
```csharp
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.Shared.Authorization;
```

**Modified the endpoint handler (lines 14-28):**
```csharp
// BEFORE:
.MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
{
    await mediator.Send(new DeleteHarvestMemberCommand(id));
    return Results.NoContent();
})

// AFTER:
.MapDelete("/{id:guid}", async (Guid id, ISender mediator, IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor) =>
{
    // Validate that only root tenant can delete HarvestMembers
    var tenantId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id;
    if (tenantId != TenantConstants.Root.Id)
    {
        return Results.Forbid();
    }
    
    await mediator.Send(new DeleteHarvestMemberCommand(id));
    return Results.NoContent();
})
```

**Added status code documentation (line 33):**
```csharp
.Produces(StatusCodes.Status403Forbidden)
```

#### 3. `/src/api/modules/Harvest/Harvest.Domain/HarvestMembers/HarvestMember.cs`
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
    // such as checking if HarvestMember is referenced by important entities
    
    bool canBeDeleted = true;
    string reason = string.Empty;
    return (canBeDeleted, reason);
}
```

#### 4. `/src/api/framework/Infrastructure/Persistence/FshDbContext.cs`
**Modified SaveChangesAsync deletion check (lines 184-189):**
```csharp
// BEFORE:
// Special case: Allow root tenant to delete any MemberPage
bool isRootTenant = tenantId == TenantConstants.Root.Id;
bool isMemberPage = entry.Entity.GetType().Name == "MemberPage";

if (tenantEntity3.TenantId != tenantId && !(isRootTenant && isMemberPage))

// AFTER:
// Special case: Allow root tenant to delete any MemberPage or HarvestMember
bool isRootTenant = tenantId == TenantConstants.Root.Id;
bool isMemberPage = entry.Entity.GetType().Name == "MemberPage";
bool isHarvestMember = entry.Entity.GetType().Name == "HarvestMember";

if (tenantEntity3.TenantId != tenantId && !(isRootTenant && (isMemberPage || isHarvestMember)))
```

#### 5. `/src/apps/blazor/client/Pages/Harvest/HarvestMembers.razor.cs`
**Modified canDeleteEntityFunc (line 80):**
```csharp
// BEFORE:
canDeleteEntityFunc: ad => ad.TenantId == CurrentTenantId

// AFTER:
canDeleteEntityFunc: ad => CurrentTenantId == "root" // Only root tenant can delete HarvestMembers
```

### Files Modified Summary
- `/src/Shared/Authorization/FshPermissions.cs` - 1 line changed
- `/src/api/modules/Harvest/Harvest.Infrastructure/Endpoints/v1/HarvestMembers/DeleteHarvestMemberEndpoint.cs` - 18 lines changed
- `/src/api/modules/Harvest/Harvest.Domain/HarvestMembers/HarvestMember.cs` - 11 lines changed
- `/src/api/framework/Infrastructure/Persistence/FshDbContext.cs` - 4 lines changed
- `/src/apps/blazor/client/Pages/Harvest/HarvestMembers.razor.cs` - 1 line changed

**Total: 35 lines modified across 5 files**

## Azure AD B2C Migration with Integration Testing Support (2025-05-27)

### Summary
Designed a migration strategy from JWT-based authentication to Azure AD B2C while maintaining the existing permission system and supporting automated integration testing.

### Key Requirements Identified
1. **Keep existing permission system** - Permissions are fetched from database, not stored in JWT
2. **Support integration testing** - Tests need authentication without Azure AD B2C dependency
3. **Multi-tenant support** - Both root tenant and regular tenant testing scenarios
4. **Security** - Production must not expose test authentication endpoints

### Architecture Decision: API Key Authentication for Tests

#### Why This Approach
- Production uses Azure AD B2C exclusively
- Test/Staging environments support both Azure AD B2C and API Key auth
- Integration tests can run without Azure dependencies
- Different API keys map to different tenant contexts

#### Implementation Plan

##### 1. API Key Authentication Handler
```csharp
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-API-Key", out var apiKey))
            return AuthenticateResult.NoResult();

        var testUser = await ValidateApiKeyAndGetTestUser(apiKey);
        if (testUser == null)
            return AuthenticateResult.Fail("Invalid API key");

        // Create claims including tenant ID
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, testUser.UserId),
            new Claim("tenant", testUser.TenantId),
            // Other required claims
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}
```

##### 2. Test User Configuration
```csharp
private async Task<TestUserConfiguration> ValidateApiKeyAndGetTestUser(string apiKey)
{
    var testUsers = new Dictionary<string, TestUserConfiguration>
    {
        ["test-key-root"] = new TestUserConfiguration
        {
            UserId = "test-root-user-id",
            TenantId = TenantConstants.Root.Id, // "root"
            Email = "root@test.local",
            Roles = new[] { FshRoles.Admin }
        },
        ["test-key-tenant1"] = new TestUserConfiguration
        {
            UserId = "test-tenant1-user-id", 
            TenantId = "tenant-123",
            Email = "user@tenant1.local",
            Roles = new[] { FshRoles.Basic }
        }
    };

    return testUsers.TryGetValue(apiKey, out var testUser) ? testUser : null;
}
```

##### 3. Environment-Based Configuration
```json
// appsettings.Production.json
{
  "Authentication": {
    "Mode": "AzureADB2C",
    "EnableTestAuth": false
  }
}

// appsettings.Staging.json  
{
  "Authentication": {
    "Mode": "AzureADB2C",
    "EnableTestAuth": true,
    "TestAuth": {
      "AllowedIPs": ["10.0.0.0/8"], // Azure internal only
      "ApiKeys": [] // From Key Vault
    }
  }
}
```

### Security Measures
1. **Production**: Only Azure AD B2C enabled
2. **Test/Staging**: API Key auth restricted by IP to Azure networks
3. **API Keys stored in Azure Key Vault**
4. **Audit logging for test authentication**

### Integration Test Usage
```csharp
[Fact]
public async Task RootTenant_CanDeleteHarvestMembers()
{
    var client = CreateAuthenticatedClient("test-key-root");
    var response = await client.DeleteAsync($"/api/v1/harvest-members/{id}");
    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
}

[Fact]
public async Task RegularTenant_CannotDeleteHarvestMembers()
{
    var client = CreateAuthenticatedClient("test-key-tenant1");
    var response = await client.DeleteAsync($"/api/v1/harvest-members/{id}");
    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
}
```

### Current Status
- Completed analysis of existing JWT implementation
- Designed secure API Key authentication for tests
- Currently setting up Azure AD B2C manually
- At Step 7 of manual setup: Collecting configuration values

### Next Steps
1. Complete Azure AD B2C setup
2. Implement API Key authentication handler
3. Modify authentication pipeline for environment-based auth
4. Update Blazor client for Azure AD B2C
5. Create integration test base classes
6. Document deployment configurations