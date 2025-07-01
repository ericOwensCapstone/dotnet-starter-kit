# Remove Local Authentication Plan

## Overview
This document provides a comprehensive plan to remove all local authentication code from the FSH Starter Kit, leaving only Azure B2C authentication. The system currently supports both local and B2C authentication, but we want to simplify to B2C-only.

## Current State Analysis

### Authentication Flow Decision Points
1. **AuthRedirect.razor** - Currently checks `AuthConfigService.IsLocalAuthentication()` to decide between local login and B2C
2. **AuthenticationConfigurationService** - Already returns `IsAzureB2C() => true`, effectively disabling local auth
3. **Login flows** - Both local and B2C login paths exist in the codebase

### Local Authentication Components

#### Client-Side Components
- **Pages/Auth/Login.razor** - Local login page (username/password form)
- **Pages/Personal/Security.razor** - Password change functionality
- **Components/Auth/AuthRedirect.razor** - Contains logic to choose between local and B2C auth

#### Server-Side API Endpoints
- **api/v1/identity/changePassword** - `ChangePasswordEndpoint.cs`
- **api/v1/identity/forgot-password** - `ForgotPasswordEndpoint.cs`
- **api/v1/identity/reset-password** - `ResetPasswordEndpoint.cs`
- **api/v1/identity/register** - `RegisterUserEndpoint.cs` (local user registration)
- **api/v1/identity/self-register** - `SelfRegisterEndpoint.cs`

#### Authentication Services
- **JwtAuthenticationService** - Handles local JWT authentication
- **IAuthenticationService** - Interface implemented by both JWT and B2C services
- **IdentityService** - Contains password-related methods
- **IUserService** - Has password management methods

#### Database Schema
- **Users table** - Contains PasswordHash column
- **Password reset tokens** - Stored for forgot password flow

## Removal Plan

### Phase 1: Remove Client-Side Local Auth Components

1. **Simplify AuthRedirect.razor** ✅ COMPLETED
   - Remove the `AuthConfigService` injection
   - Remove the conditional logic checking `IsLocalAuthentication()`
   - Always navigate to `/login` (which will be updated to always use B2C)
   - Remove any local auth-specific logic

2. **Remove Local Login Page** ✅ COMPLETED
   - Delete `/Pages/Auth/Login.razor` and `/Pages/Auth/Login.razor.cs`
   - Update any references to redirect to B2C login instead

3. **Remove Password Management UI** ✅ COMPLETED
   - Delete password change functionality from `/Pages/Personal/Security.razor`
   - Remove any UI for forgot password/reset password flows
   - Update navigation to remove password-related menu items

### Phase 2: Remove Server-Side Password Endpoints

1. **Delete Password Management Endpoints** ✅ COMPLETED
   ```
   - /src/api/framework/Infrastructure/Identity/Users/Endpoints/ChangePasswordEndpoint.cs
   - /src/api/framework/Infrastructure/Identity/Users/Endpoints/ForgotPasswordEndpoint.cs
   - /src/api/framework/Infrastructure/Identity/Users/Endpoints/ResetPasswordEndpoint.cs
   ```

2. **Modify Registration Endpoints** ✅ COMPLETED
   - Delete `/src/api/framework/Infrastructure/Identity/Users/Endpoints/RegisterUserEndpoint.cs` (local registration)
   - Delete `/src/api/framework/Infrastructure/Identity/Users/Validators/RegisterUserCommandValidator.cs`
   - SelfRegisterEndpoint.cs was already removed or never existed
   - Removed RegisterAsync method from UserService and IUserService
   - Removed RegisterUserCommand and RegisterUserResponse DTOs
   - Updated Users.razor to remove create functionality and Create button

### Phase 3: Clean Up Authentication Services

1. **Remove JwtAuthenticationService**
   - Delete `JwtAuthenticationService.cs` from Blazor infrastructure
   - Update DI registration to only use B2CAuthenticationService
   - Remove any references to JWT authentication

2. **Simplify IAuthenticationService**
   - Remove any methods specific to local authentication
   - Keep only methods needed for B2C

3. **Update AuthenticationConfigurationService**
   - Remove `IsLocalAuthentication()` method
   - Remove `IsTenantAuthentication()` if not needed
   - Simplify to always assume B2C

### Phase 4: Update User Service and Identity Service

1. **Clean IUserService Interface**
   - Remove password-related methods:
     - `ChangePasswordAsync`
     - `ForgotPasswordAsync`
     - `ResetPasswordAsync`
   - Keep user profile and role management methods

2. **Update IdentityService Implementation**
   - Remove all password hashing/validation logic
   - Remove password reset token generation
   - Remove email sending for password resets

### Phase 5: Database and Model Updates

1. **Update User Models**
   - Remove PasswordHash property from User entity
   - Remove any password-related validation attributes
   - Remove password reset token fields

2. **Create Database Migration**
   - Generate migration to drop PasswordHash column
   - Remove any password reset token tables/columns
   - Update any stored procedures that reference passwords

### Phase 6: Configuration and Cleanup

1. **Update Configuration**
   - Remove any local JWT signing key configuration
   - Remove password policy settings
   - Keep only B2C-related configuration

2. **Update API Client**
   - Remove password-related API methods from IApiClient
   - Remove corresponding DTOs and commands

3. **Update Documentation**
   - Remove references to local authentication
   - Update setup guides to focus on B2C only
   - Update API documentation

## Implementation Order

1. **Start with Client-Side** (Low risk, immediate UX improvement)
   - Simplify AuthRedirect.razor
   - Remove local login page
   - Update navigation flows

2. **Remove Unused Endpoints** (Medium risk)
   - Delete password management endpoints
   - Remove local registration endpoints
   - Test that B2C flows still work

3. **Consolidate Services** (Higher risk, requires careful testing)
   - Remove JWT authentication service
   - Simplify authentication interfaces
   - Update DI configuration

4. **Database Changes** (Highest risk, requires migration)
   - Create and test migration scripts
   - Update models
   - Deploy database changes

## Testing Checklist

After each phase, verify:
- [ ] B2C login still works
- [ ] B2C logout works correctly
- [ ] Protected routes redirect to B2C login
- [ ] User profile information displays correctly
- [ ] API authentication with B2C tokens works
- [ ] No references to local auth remain in UI
- [ ] No broken links or navigation issues

## Files to be Modified/Deleted

### Delete Completely:
- `/src/apps/blazor/client/Pages/Auth/Login.razor`
- `/src/apps/blazor/client/Pages/Auth/Login.razor.cs`
- `/src/apps/blazor/infrastructure/Auth/Jwt/JwtAuthenticationService.cs`
- `/src/api/server/Endpoints/Identity/ChangePasswordEndpoint.cs`
- `/src/api/server/Endpoints/Identity/ForgotPasswordEndpoint.cs`
- `/src/api/server/Endpoints/Identity/ResetPasswordEndpoint.cs`
- `/src/api/server/Endpoints/Identity/RegisterUserEndpoint.cs`
- `/src/api/server/Endpoints/Identity/SelfRegisterEndpoint.cs`

### Modify:
- `/src/apps/blazor/client/Components/Auth/AuthRedirect.razor` - Simplify to always use B2C
- `/src/apps/blazor/client/Pages/Personal/Security.razor` - Remove password section
- `/src/apps/blazor/infrastructure/Auth/IAuthenticationService.cs` - Remove local auth methods
- `/src/apps/blazor/infrastructure/Auth/AuthenticationConfigurationService.cs` - Remove local auth checks
- `/src/api/framework/Core/Identity/Users/IUserService.cs` - Remove password methods
- `/src/api/framework/Infrastructure/Identity/IdentityService.cs` - Remove password implementation
- `/src/api/framework/Core/Identity/Users/Domain/User.cs` - Remove PasswordHash property
- DI registration files - Remove JWT service registration

## Benefits of Removal

1. **Security**: No local password storage reduces attack surface
2. **Simplicity**: Single authentication method is easier to maintain
3. **Compliance**: Delegates authentication to enterprise-grade B2C service
4. **User Experience**: Consistent login experience with SSO capabilities
5. **Reduced Maintenance**: No password reset flows to maintain

## Risks and Mitigation

1. **Risk**: Existing users with local passwords
   - **Mitigation**: Ensure all users are migrated to B2C before deployment

2. **Risk**: Breaking API authentication
   - **Mitigation**: Thoroughly test API endpoints with B2C tokens

3. **Risk**: Missing edge cases
   - **Mitigation**: Comprehensive testing of all auth scenarios

## Success Criteria

- All authentication flows use Azure B2C exclusively
- No local password storage or management code remains
- Clean codebase with single authentication path
- All tests pass with B2C authentication
- No UI references to local authentication