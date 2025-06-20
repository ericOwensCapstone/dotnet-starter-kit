# TODO List for Future Development

## Comprehensive B2C Security Enhancement Plan

### Context and Current State
- **Problem**: B2C authentication endpoints need to access database without tenant context, causing try/catch workarounds
- **Solution**: Create TenantFreeDbContext for B2C operations and root admin functions
- **Current Files Affected**:
  - `/src/api/framework/Infrastructure/Auth/AzureB2C/B2CUserMappingService.cs`
  - `/src/api/framework/Infrastructure/Auth/AzureB2C/Endpoints/B2CPostRegistrationEndpoint.cs`
  - `/src/api/framework/Infrastructure/Identity/Invitations/AnonymousInvitationRepository.cs`

### Phase 1: TenantFreeDbContext Foundation

**Step 1.1: Create TenantFreeDbContext**
- Create `/src/api/framework/Infrastructure/Persistence/TenantFreeDbContext.cs`
- Inherit directly from `DbContext` (not multi-tenant aware)
- Include these tables WITHOUT tenant filters:
  - `Users` (identity schema)
  - `UserRoles` (identity schema)
  - `UserInvitations` (identity schema)
  - `Roles` (identity schema)
  - `RoleClaims` (identity schema)
  - `Tenants` (tenant schema)
- Connection string handling:
  ```csharp
  // Use same connection string as AuthenticationDbContext
  // Get from IOptions<DatabaseOptions>
  ```
- Register in DI container in `/src/api/framework/Infrastructure/Persistence/Extensions.cs`:
  ```csharp
  services.AddDbContext<TenantFreeDbContext>(options =>
      options.UseNpgsql(databaseOptions.ConnectionString));
  ```

**🧪 Test Point 1.1:**
- Create unit test to verify no tenant filters applied
- Verify can query Users table without tenant context
- Test query: `SELECT * FROM identity."Users"` returns all users

**Step 1.2: Refactor AnonymousInvitationRepository**
- Current location: `/src/api/framework/Infrastructure/Identity/Invitations/AnonymousInvitationRepository.cs`
- Update constructor to accept TenantFreeDbContext instead of current context
- Remove any `.IgnoreQueryFilters()` calls (no longer needed)
- Remove any raw SQL workarounds

**Step 1.3: Update B2CUserMappingService**
- File: `/src/api/framework/Infrastructure/Auth/AzureB2C/B2CUserMappingService.cs`
- Replace `AuthenticationDbContext` with `TenantFreeDbContext` in constructor
- Remove try/catch blocks at these lines:
  - Lines 115-126 (querying by ObjectId)
  - Lines 132-142 (querying by email)
  - Lines 295-514 (role assignment)
  - Lines 711-716 (retrieving user claims)
- Remove raw SQL methods:
  - `GetUserTenantIdAsync` (lines 583-605) - use EF query instead
  - `CreateParameter` helper (lines 607-613) - no longer needed
- Remove `TempMultiTenantContextAccessor` class (lines 722-735)

**🧪 Test Point 1.3:**
- Test existing B2C login flow still works
- Verify no errors in logs during authentication
- Check logs for absence of "Error querying Users table" messages

### Phase 2: Enhanced B2CPostRegistrationEndpoint

**Current State**:
- File: `/src/api/framework/Infrastructure/Auth/AzureB2C/Endpoints/B2CPostRegistrationEndpoint.cs`
- Currently only creates user record, no role assignment
- Line 173-174: "Note: Role assignment will be handled during first sign-in"
- Uses raw SQL to bypass tenant filtering

**Step 2.1: Update B2CPostRegistrationEndpoint**
- Replace `AuthenticationDbContext` with `TenantFreeDbContext`
- Remove raw SQL user creation (lines 126-171)
- After creating user, add role assignment:
  ```csharp
  // Get role from invitation
  if (!string.IsNullOrEmpty(invitation.Role))
  {
      var role = await dbContext.Roles
          .FirstOrDefaultAsync(r => r.Name == invitation.Role && r.TenantId == invitation.TargetTenantId);
      
      if (role != null)
      {
          dbContext.UserRoles.Add(new IdentityUserRole<string>
          {
              UserId = user.Id,
              RoleId = role.Id,
              TenantId = invitation.TargetTenantId
          });
          await dbContext.SaveChangesAsync();
      }
  }
  ```
- Remove line 176: "User will be assigned role during first sign-in"
- Update `AcceptInvitationAsync` call to mark invitation as accepted

**🧪 Test Point 2.1:**
- SQL to clean test user: 
  ```sql
  DELETE FROM identity."UserRoles" WHERE "UserId" IN (SELECT "Id" FROM identity."Users" WHERE "Email" = 'test@example.com');
  DELETE FROM identity."Users" WHERE "Email" = 'test@example.com';
  DELETE FROM identity."UserInvitations" WHERE "Email" = 'test@example.com';
  ```
- Send new invitation with "Admin" role
- Complete B2C signup
- Verify in database:
  ```sql
  SELECT u."Email", r."Name" as "Role", ur."TenantId"
  FROM identity."Users" u
  JOIN identity."UserRoles" ur ON u."Id" = ur."UserId"
  JOIN identity."Roles" r ON ur."RoleId" = r."Id"
  WHERE u."Email" = 'test@example.com';
  ```

### Phase 3: Secure Invitation Landing Page

**Current Security Issues**:
- File: `/src/api/framework/Infrastructure/Auth/AzureB2C/Endpoints/B2CInvitationLandingEndpoint.cs`
- Currently displays (lines 290-301):
  - Recipient's full name and email
  - Inviting person's name
  - Target tenant name
- Anyone with the invitation link can see this information

**Step 3.1: Update B2CInvitationLandingEndpoint**
- Replace `GetLandingPage` method to show minimal info:
  ```html
  <h1>Email Verification Required</h1>
  <p>To accept this invitation, you must verify your email address.</p>
  <p>Click below to continue with the verification process.</p>
  ```
- Keep invitation token in B2C redirect URL (line 92)
- Remove parameters from GetLandingPage call:
  - Remove: displayName, email, tenantName, invitedBy
  - Keep only: signUpUrl
- Update error pages to be generic as well

**Step 3.2: Update B2C Custom Policy**
- File: `/B2CPolicies/Invitation_Acceptance.xml`
- Policy name: `B2C_1A_invitation_acceptance`

**B2C Policy Changes Required**:

1. **Add Email Verification Technical Profile** (after line 173, before closing `</TechnicalProfiles>`):
   ```xml
   <!-- Email verification technical profile -->
   <TechnicalProfile Id="LocalAccount-EmailVerification">
     <DisplayName>Email Verification</DisplayName>
     <Protocol Name="Proprietary" Handler="Web.TPEngine.Providers.SelfAssertedAttributeProvider, Web.TPEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
     <Metadata>
       <Item Key="ContentDefinitionReferenceId">api.selfasserted</Item>
       <Item Key="EnforceEmailVerification">true</Item>
       <Item Key="setting.showCancelButton">false</Item>
     </Metadata>
     <InputClaims>
       <InputClaim ClaimTypeReferenceId="email" />
     </InputClaims>
     <OutputClaims>
       <OutputClaim ClaimTypeReferenceId="email" PartnerClaimType="Verified.Email" Required="true" />
     </OutputClaims>
     <UseTechnicalProfileForSessionManagement ReferenceId="SM-AAD" />
   </TechnicalProfile>
   ```

2. **Update REST-ValidateInvitation Technical Profile** (lines 112-119):
   - Initially return only email and isValid (no sensitive data)
   - Remove these OutputClaims initially:
     ```xml
     <!-- Remove these from initial validation -->
     <OutputClaim ClaimTypeReferenceId="displayName" PartnerClaimType="displayName" />
     <OutputClaim ClaimTypeReferenceId="givenName" PartnerClaimType="firstName" />
     <OutputClaim ClaimTypeReferenceId="surname" PartnerClaimType="lastName" />
     <OutputClaim ClaimTypeReferenceId="extension_TenantId" PartnerClaimType="targetTenantId" />
     ```

3. **Add New REST Profile for Post-Verification** (after REST-ValidateInvitation):
   ```xml
   <TechnicalProfile Id="REST-GetInvitationDetails">
     <DisplayName>Get Full Invitation Details After Verification</DisplayName>
     <Protocol Name="Proprietary" Handler="Web.TPEngine.Providers.RestfulProvider, Web.TPEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
     <Metadata>
       <Item Key="ServiceUrl">https://eric-owens.ngrok.io/api/public/b2c/invitations/validate/{invitationToken}</Item>
       <Item Key="SendClaimsIn">Url</Item>
       <Item Key="AuthenticationType">None</Item>
       <Item Key="AllowInsecureAuthInProduction">false</Item>
     </Metadata>
     <InputClaims>
       <InputClaim ClaimTypeReferenceId="invitationToken" />
       <InputClaim ClaimTypeReferenceId="email" PartnerClaimType="verifiedEmail" />
     </InputClaims>
     <OutputClaims>
       <OutputClaim ClaimTypeReferenceId="displayName" PartnerClaimType="displayName" />
       <OutputClaim ClaimTypeReferenceId="givenName" PartnerClaimType="firstName" />
       <OutputClaim ClaimTypeReferenceId="surname" PartnerClaimType="lastName" />
       <OutputClaim ClaimTypeReferenceId="extension_TenantId" PartnerClaimType="targetTenantId" />
       <OutputClaim ClaimTypeReferenceId="objectId" PartnerClaimType="b2cUserId" />
     </OutputClaims>
     <UseTechnicalProfileForSessionManagement ReferenceId="SM-Noop" />
   </TechnicalProfile>
   ```

4. **Update LocalAccount-SetPasswordForDisabledAccount** (line 248):
   - Change: `<Item Key="EnforceEmailVerification">false</Item>`
   - To: `<Item Key="EnforceEmailVerification">true</Item>`

5. **Update User Journey** (insert new steps after Step 3):
   ```xml
   <!-- Step 4: Email verification -->
   <OrchestrationStep Order="4" Type="ClaimsExchange">
     <Preconditions>
       <Precondition Type="ClaimEquals" ExecuteActionsIf="false">
         <Value>isValidInvitation</Value>
         <Value>True</Value>
         <Action>SkipThisOrchestrationStep</Action>
       </Precondition>
     </Preconditions>
     <ClaimsExchanges>
       <ClaimsExchange Id="EmailVerification" TechnicalProfileReferenceId="LocalAccount-EmailVerification" />
     </ClaimsExchanges>
   </OrchestrationStep>

   <!-- Step 5: Get full invitation details after email verified -->
   <OrchestrationStep Order="5" Type="ClaimsExchange">
     <Preconditions>
       <Precondition Type="ClaimEquals" ExecuteActionsIf="false">
         <Value>isValidInvitation</Value>
         <Value>True</Value>
         <Action>SkipThisOrchestrationStep</Action>
       </Precondition>
     </Preconditions>
     <ClaimsExchanges>
       <ClaimsExchange Id="GetInvitationDetails" TechnicalProfileReferenceId="REST-GetInvitationDetails" />
     </ClaimsExchanges>
   </OrchestrationStep>
   ```
   - Renumber all subsequent steps (current Step 4 becomes Step 6, etc.)

6. **Update B2CValidateInvitationEndpoint.cs**:
   - Check for presence of `verifiedEmail` claim in request
   - If present, return full invitation details
   - If not present, return only email and isValid

**🧪 Test Point 3.2:**
- Access invitation URL from different browser/incognito
- Verify page shows only "Email verification required" message
- No personal information visible
- Click continue and verify B2C email verification flow
- After email verified, confirm invitation details shown in B2C
- Complete signup and verify user created correctly

### Phase 4: Root Admin User Management

**Context**: Root admin needs to see ALL users across ALL tenants and permanently delete them from both local DB and B2C

**Step 4.1: Create User Management Page**
- Create `/src/apps/blazor/client/Pages/Admin/UserManagement.razor` and `.razor.cs`
- Follow pattern from `/src/apps/blazor/client/Pages/Multitenancy/Tenants.razor.cs`
- Add to navigation in `/src/apps/blazor/client/Components/Layout/Navigation.razor`
- Required permission: Create new "Permissions.Root.ManageAllUsers"
- Components needed:
  ```csharp
  // User search autocomplete
  MudAutocomplete<UserDto> - search all users across tenants
  
  // Selected user display
  MudCard showing: Email, Name, Tenant, Roles, B2C ObjectId
  
  // Action buttons
  MudButton "Delete User Completely" - with confirmation dialog
  MudButton "Purge All B2C Deleted Users" - with strong warning
  
  // Results display
  MudAlert for operation results
  ```

**Step 4.2: Create DeleteUserCompletelyCommand**
- Create in `/src/api/framework/Core/Identity/Users/Features/DeleteUserCompletely/`
- Command properties: `UserId`, `DeleteFromB2C` (bool)
- Handler logic:
  ```csharp
  1. Begin transaction
  2. Get user details (for B2C ObjectId)
  3. Delete from UserRoles: DELETE FROM identity."UserRoles" WHERE "UserId" = @userId
  4. Delete from Users: DELETE FROM identity."Users" WHERE "Id" = @userId  
  5. Delete from UserInvitations by email
  6. Commit transaction
  7. If DeleteFromB2C && !string.IsNullOrEmpty(user.ObjectId):
     - Try: await _graphService.DeleteUserAsync(objectId)
     - Try: await _graphService.PermanentlyDeleteUserAsync(objectId)
  8. Return DeleteUserCompletelyResponse with detailed status
  ```
- Create endpoint in `/src/api/framework/Infrastructure/Identity/Users/Endpoints/`

**Step 4.3: Create PurgeAllDeletedB2CUsersCommand**
- Create in `/src/api/framework/Core/Identity/Users/Features/PurgeDeletedB2CUsers/`
- No parameters needed
- Handler logic:
  ```csharp
  var deletedUsers = await _graphService.GetDeletedUsersAsync();
  var results = new List<PurgeResult>();
  
  foreach (var user in deletedUsers)
  {
      try 
      {
          await _graphService.PermanentlyDeleteUserAsync(user.Id);
          results.Add(new PurgeResult { Success = true, Email = user.Mail });
      }
      catch (Exception ex)
      {
          results.Add(new PurgeResult { Success = false, Email = user.Mail, Error = ex.Message });
      }
  }
  
  return new PurgeAllResponse { TotalCount = deletedUsers.Count, Results = results };
  ```

**IGraphService additions needed**:
```csharp
Task<bool> DeleteUserAsync(string objectId);
Task<bool> PermanentlyDeleteUserAsync(string objectId);
Task<List<Microsoft.Graph.Models.User>> GetDeletedUsersAsync();
```

**Graph API endpoints**:
- Delete user: `DELETE /users/{id}`
- Get deleted users: `GET /directory/deletedItems/microsoft.graph.user`
- Permanently delete: `DELETE /directory/deletedItems/{id}`

**🧪 Test Point 4.3:**
- Login as root user (check Permissions.Root.ManageAllUsers)
- Navigate to /admin/user-management
- Search for test user - verify shows even if in different tenant
- Delete user with B2C option checked
- Verify results show all steps (DB ✓, B2C ✓, Permanent ✓)
- Check DB: `SELECT * FROM identity."Users" WHERE "Email" = 'test@example.com'` (should be empty)
- Test "Purge All" shows count of deleted users
- After purge, verify B2C portal shows no deleted users

### Phase 5: Security Hardening

**Context**: B2C endpoints need protection from unauthorized access while allowing legitimate B2C service calls

**Step 5.1: Add IP Restrictions Middleware**
- Create `/src/api/framework/Infrastructure/Security/B2CIPRestrictionMiddleware.cs`
- Azure AD B2C IP ranges (as of 2024):
  ```
  20.190.128.0/18 (Azure Public)
  40.126.0.0/18 (Azure Public)
  Additional ranges from: https://www.microsoft.com/download/details.aspx?id=56519
  ```
- Middleware implementation:
  ```csharp
  public class B2CIPRestrictionMiddleware
  {
      private readonly HashSet<IPNetwork> _allowedNetworks;
      
      public async Task InvokeAsync(HttpContext context, RequestDelegate next)
      {
          // Only apply to B2C server endpoints (not user-facing)
          if (context.Request.Path.StartsWithSegments("/api/public/b2c/invitations/validate") ||
              context.Request.Path.StartsWithSegments("/api/public/b2c/invitations/post-registration"))
          {
              var remoteIp = context.Connection.RemoteIpAddress;
              if (!IsAllowedIP(remoteIp))
              {
                  context.Response.StatusCode = 403;
                  return;
              }
          }
          await next(context);
      }
  }
  ```
- Register in Program.cs before authentication middleware
- Configuration in appsettings:
  ```json
  "B2CSecurity": {
    "EnableIPRestrictions": false, // false for dev, true for prod
    "AllowedIPRanges": ["20.190.128.0/18", "40.126.0.0/18"]
  }
  ```

**Step 5.2: Add API Key Validation**
- Add to B2C custom policy REST API technical profile:
  ```xml
  <InputClaim ClaimTypeReferenceId="apiKey" DefaultValue="your-secret-key" />
  <Header Id="X-API-Key" DataType="string" Value="{apiKey}" />
  ```
- Update B2C endpoints to validate header:
  ```csharp
  var apiKey = context.Request.Headers["X-API-Key"];
  if (apiKey != _configuration["B2CSecurity:ApiKey"])
  {
      return Results.Unauthorized();
  }
  ```
- Store API key in Azure Key Vault for production

**Endpoints needing protection**:
1. `/api/public/b2c/invitations/validate/{token}` - IP + API key
2. `/api/public/b2c/invitations/post-registration` - IP + API key
3. `/api/public/b2c-token` - NO IP restriction (user browsers), validate B2C token
4. `/api/public/invitation/{token}` - NO restrictions (email links)

**🧪 Test Point 5.2:**
- Deploy to test environment with IP restrictions enabled
- Test from local machine - should get 403 Forbidden
- Add test machine IP to allowed list temporarily
- Verify B2C custom policy sends X-API-Key header
- Test complete invitation flow works with security enabled
- Remove test IP and verify 403 again

### Phase 6: Documentation

**Step 6.1: Create TODO.md** ✓ COMPLETED
- Location: `/TODO.md` in project root
- Contains this comprehensive plan
- Includes rate limiting requirement

**Step 6.2: Update README**
- Add new section "B2C Integration and Security":
  ```markdown
  ## B2C Integration and Security
  
  ### TenantFreeDbContext
  Used for operations that need to bypass multi-tenant filtering:
  - B2C authentication endpoints
  - Root admin cross-tenant operations
  - Anonymous invitation validation
  
  ### Root Admin Features
  - User Management: `/admin/user-management`
  - Can view and delete users across all tenants
  - Can purge deleted B2C users permanently
  - Required permission: `Permissions.Root.ManageAllUsers`
  
  ### B2C Security
  - IP restrictions for server-to-server endpoints (configurable)
  - API key validation for B2C REST API calls
  - Email verification before revealing invitation details
  - Rate limiting on public endpoints (TODO)
  ```

**🧪 Final Integration Test:**
1. Clean test user:
   ```sql
   DELETE FROM identity."UserRoles" WHERE "UserId" IN (SELECT "Id" FROM identity."Users" WHERE "Email" = 'integrationtest@example.com');
   DELETE FROM identity."Users" WHERE "Email" = 'integrationtest@example.com';
   DELETE FROM identity."UserInvitations" WHERE "Email" = 'integrationtest@example.com';
   ```
2. Create new invitation for `integrationtest@example.com` with "Admin" role
3. Click invitation link - verify generic landing page (no personal info)
4. Complete B2C signup with email verification
5. Login with new user - verify can access admin features
6. Logout and login as root admin
7. Navigate to `/admin/user-management`
8. Search and select `integrationtest@example.com`
9. Delete with B2C option checked
10. Verify all deletion steps successful
11. Immediately create new invitation with same email
12. Verify no errors (email is reusable)

**Success Criteria**:
- No try/catch blocks in B2CUserMappingService
- User fully provisioned on first signup (including roles)
- No sensitive data exposed on invitation landing
- Root admin can manage all users
- B2C emails immediately reusable after deletion

## Additional TODO Items

### 1. Implement Rate Limiting for Invitation Acceptance Page
- **Priority**: High
- **Security Impact**: Prevents brute force attempts on invitation tokens
- **Implementation**: Add rate limiting middleware to `/api/public/invitation/{token}` endpoint
- **Suggested Limits**: 
  - 5 attempts per IP per hour
  - 20 attempts per token per day
- **Consider**: Using Azure Front Door or application-level rate limiting