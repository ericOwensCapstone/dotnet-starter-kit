# Authentication Architecture Documentation

## Current B2C Token Flow and Exchange Process

### Overview
The application uses Azure AD B2C for user authentication but exchanges B2C tokens for local JWT tokens to support multi-tenant authorization and provide a provider abstraction layer.

### Detailed Token Flow

#### 1. **Client-Side Authentication Initiation**
- **Automatic Redirect**: The app has no login button - authentication is triggered automatically
- **Trigger Points**:
  1. **BaseLayout Authorization** (`src/apps/blazor/client/Layout/BaseLayout.razor`):
     - Uses `<AuthorizeView>` requiring authentication for main layout
     - Unauthenticated users see limited layout
  2. **App.razor Handler** (`src/apps/blazor/client/App.razor`):
     - `<AuthorizeRouteView>` detects unauthorized access
     - Renders `<AuthRedirect />` component for unauthenticated users
  3. **AuthRedirect Component** (`src/apps/blazor/client/Components/Auth/AuthRedirect.razor`):
     - Automatically calls `AuthenticationService.NavigateToExternalLogin("/")`
     - No user interaction required
  4. **Direct Login Route** (`src/apps/blazor/client/Pages/Auth/Login.razor`):
     - `/login` page also triggers automatic redirect
     - Shows "Redirecting to Azure AD B2C..." message

- **B2C Redirect Process**:
  ```
  User accesses any page → BaseLayout requires auth → AuthRedirect component → B2C redirect
  ```
- **Location**: `src/apps/blazor/infrastructure/Auth/AzureB2C/B2CAuthenticationService.cs`
- **Method**: `NavigateToExternalLogin()`
- **URL Construction**:
  - Base: `{B2C_Instance}/{Domain}/{PolicyId}/oauth2/v2.0/authorize`
  - Parameters:
    - `client_id`: B2C application ID
    - `response_type`: "id_token token"
    - `redirect_uri`: `{BaseUri}/authentication/login-callback`
    - `scope`: "openid offline_access {ApiScope}"
    - `response_mode`: "fragment" (tokens returned in URL fragment)
    - `nonce`: Random GUID for replay protection
    - `state`: Return URL for post-login navigation

#### 2. **B2C Authentication**
- User authenticates with B2C (email/password, social login, etc.)
- B2C validates credentials and returns tokens in URL fragment
- Redirect URL: `https://app.com/authentication/login-callback#id_token=xxx&access_token=yyy&state=zzz`

#### 3. **Token Callback Processing**
- **Location**: `src/apps/blazor/infrastructure/Auth/AzureB2C/B2CAuthenticationService.cs`
- **Method**: `ProcessAuthenticationCallbackAsync()`
- **Process**:
  1. Extract tokens from URL fragment
  2. Parse fragment parameters
  3. Extract `id_token` or `access_token`
  4. Call `ExchangeB2CTokenAsync()`

#### 4. **Token Exchange**
- **Frontend**: `B2CAuthenticationService.ExchangeB2CTokenAsync()`
  - Sends B2C token to backend endpoint
  - Uses special HTTP client without JWT interceptor
  - Endpoint: `POST /api/public/b2c-token`
  - Header: `Authorization: Bearer {b2c_token}`

- **Backend**: `src/api/framework/Infrastructure/Auth/AzureB2C/Endpoints/PublicB2CTokenEndpoint.cs`
  - **No token validation** (security issue - trusts B2C implicitly)
  - Extracts claims from B2C token
  - Maps B2C user to local user
  - Generates local JWT with tenant context

#### 5. **User Mapping Process**
- **Location**: `src/api/framework/Infrastructure/Auth/AzureB2C/B2CUserMappingService.cs`
- **Logic**:
  1. Extract B2C Object ID (`oid` claim)
  2. Try to find existing user by Object ID
  3. If not found, try by email
  4. If not found, check for valid invitation
  5. Create new user if invitation exists
  6. Assign tenant and roles from invitation

#### 6. **Local Token Storage**
- **Location**: Browser's LocalStorage
- **Keys**:
  - `authToken`: Local JWT
  - `refreshToken`: Refresh token
  - `permissions`: Cached permissions (optional)

## Token Contents Comparison

### B2C Token Contents
```json
{
  // Standard OIDC Claims
  "iss": "https://{tenant}.b2clogin.com/{tenantId}/v2.0/",
  "aud": "{B2C_ClientId}",
  "exp": 1234567890,
  "iat": 1234567890,
  "nbf": 1234567890,
  "nonce": "unique-nonce-value",
  
  // User Identity Claims
  "oid": "b2c-object-id-guid",      // B2C unique user ID
  "sub": "b2c-subject-id",          // Subject identifier
  "emails": ["user@example.com"],    // User's email (array)
  "name": "John Doe",                // Display name
  "given_name": "John",              // First name
  "family_name": "Doe",              // Last name
  
  // Custom B2C Attributes (if configured)
  "extension_TenantId": "tenant-guid",
  "extension_UserStatus": "Active",
  
  // B2C Specific
  "tfp": "B2C_1_SignUpSignIn",      // Trust framework policy
  "ver": "1.0"                      // Token version
}
```

### Local JWT Token Contents
```json
{
  // Standard JWT Claims
  "iss": "https://fullstackhero.net",
  "aud": "fullstackhero",
  "exp": 1234567890,
  "iat": 1234567890,
  "nbf": 1234567890,
  
  // User Identity Claims
  "nameid": "local-user-guid",       // Local user ID
  "email": "user@example.com",
  "fullName": "John Doe",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  
  // Multi-Tenant Claims
  "FshClaims.Tenant": "tenant-guid", // Current tenant context
  "FshClaims.ImageUrl": "https://...",
  
  // Authorization Claims
  "role": ["Admin", "User"],         // User roles in tenant
  "permission": [                    // Expanded permissions
    "View Dashboard",
    "Create Users",
    "Delete Posts",
    // ... all permissions from roles
  ],
  
  // Metadata
  "ipAddress": "192.168.1.1",        // Client IP
  "oid": "b2c-object-id-guid",      // B2C Object ID (preserved)
  
  // B2C Users Only (not in regular auth)
  // All permissions included in token
  // Regular users fetch permissions per request
}
```

### How Token Contents Are Created

#### B2C Token Creation
- Created by Azure AD B2C during authentication
- Claims populated from:
  - User profile in B2C directory
  - Custom attributes via Graph API
  - Identity provider claims (social logins)
  - Custom policies (if configured)

#### Local JWT Creation
```csharp
// In TokenService.GenerateB2CTokenAsync()
var claims = new List<Claim>
{
    // User identity from B2C/local user
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(FshClaims.Fullname, user.FullName),
    
    // Tenant context (critical for multi-tenancy)
    new Claim(FshClaims.Tenant, user.TenantId),
    
    // Roles from tenant-specific database
    new Claim(ClaimTypes.Role, roles), // Multiple role claims
    
    // Permissions expanded from roles
    new Claim(FshClaims.Permission, permissions), // Multiple permission claims
    
    // Metadata
    new Claim(FshClaims.IpAddress, ipAddress),
    new Claim("oid", b2cObjectId) // Preserve B2C link
};
```

## Provider Abstraction Benefits

### 1. **Unified User Model**
- Single user record regardless of auth provider
- Consistent user IDs across the system
- Link multiple providers to same account

### 2. **Standardized Claims**
- Same claim structure for all providers
- No provider-specific logic in application
- Easy to add new providers

### 3. **Multi-Tenant Support**
- External providers don't understand tenants
- Token exchange adds tenant context
- Enables tenant-specific roles/permissions

### 4. **Provider Migration**
- Switch providers without changing app logic
- Support multiple providers simultaneously
- Gradual migration paths

### 5. **Enhanced Security**
- Centralized security policies
- Provider-specific validation
- Consistent audit trail

### 6. **Example: Adding Google Auth**
```csharp
// Frontend: New Google service implementing IAuthenticationService
public class GoogleAuthenticationService : IAuthenticationService
{
    public void NavigateToExternalLogin(string returnUrl)
    {
        // Google OAuth2 URL construction
    }
}

// Backend: New endpoint
app.MapPost("/api/public/google-token", async (context) =>
{
    var googleToken = ExtractGoogleToken(context);
    var googleClaims = ValidateGoogleToken(googleToken);
    
    // Same user mapping logic
    var user = await GetOrCreateUserFromGoogleClaims(googleClaims);
    
    // Same local JWT generation
    return await GenerateLocalJWT(user);
});
```

## MSAL Implementation Plan

### Overview
Replace the custom B2C authentication implementation with Microsoft Authentication Library (MSAL) to improve security while maintaining the token exchange pattern for multi-tenant support.

### Prerequisites
- Understanding of current auth flow (documented above)
- Access to B2C tenant configuration
- Ability to test with multiple user scenarios

### Implementation Steps

#### Phase 1: Add MSAL Package and Configuration

1. **Add NuGet Package**
   ```xml
   <!-- In src/apps/blazor/client/Client.csproj -->
   <PackageReference Include="Microsoft.Authentication.WebAssembly.Msal" Version="9.0.0" />
   ```

2. **Update Program.cs**
   ```csharp
   // src/apps/blazor/client/Program.cs
   builder.Services.AddMsalAuthentication(options =>
   {
       var b2cConfig = builder.Configuration.GetSection("AzureAdB2C");
       options.ProviderOptions.Authentication.Authority = 
           $"{b2cConfig["Instance"]}/{b2cConfig["Domain"]}/{b2cConfig["SignUpSignInPolicyId"]}";
       options.ProviderOptions.Authentication.ClientId = b2cConfig["ClientId"];
       options.ProviderOptions.Authentication.ValidateAuthority = true;
       
       // Add API scope
       options.ProviderOptions.DefaultAccessTokenScopes.Add(b2cConfig["ApiScope"]);
       
       // Optional: Use localStorage instead of sessionStorage
       // options.ProviderOptions.Cache.CacheLocation = "localStorage";
   });
   ```

3. **Create MSAL-aware Authentication Service**
   ```csharp
   // src/apps/blazor/infrastructure/Auth/AzureB2C/MsalB2CAuthenticationService.cs
   public class MsalB2CAuthenticationService : AuthenticationStateProvider, IAuthenticationService
   {
       private readonly IAccessTokenProvider _tokenProvider;
       private readonly NavigationManager _navigation;
       private readonly IApiClient _apiClient;
       
       // Use MSAL's IAccessTokenProvider for B2C tokens
       // Still exchange for local JWTs
   }
   ```

#### Phase 2: Update Authentication Flow

1. **Modify Authentication Service Registration**
   ```csharp
   // src/apps/blazor/infrastructure/Auth/Extensions.cs
   services.AddScoped<MsalB2CAuthenticationService>();
   services.AddScoped<AuthenticationStateProvider>(sp => 
       sp.GetRequiredService<MsalB2CAuthenticationService>());
   ```

2. **Update Token Exchange**
   ```csharp
   public async Task<bool> ExchangeTokenAsync()
   {
       // Get B2C token from MSAL
       var tokenResult = await _tokenProvider.RequestAccessToken();
       if (tokenResult.TryGetToken(out var token))
       {
           // Exchange with backend (same endpoint)
           return await ExchangeB2CTokenAsync(token.Value);
       }
       return false;
   }
   ```

3. **Handle MSAL Navigation**
   ```csharp
   // Update login to use MSAL
   public void NavigateToExternalLogin(string returnUrl)
   {
       _navigation.NavigateToLogin("authentication/login", returnUrl);
   }
   ```

#### Phase 3: Update Components

1. **Create Authentication Components**
   ```razor
   <!-- src/apps/blazor/client/Pages/Authentication.razor -->
   @page "/authentication/{action}"
   <RemoteAuthenticatorView Action="@Action" />
   
   @code {
       [Parameter] public string Action { get; set; } = string.Empty;
   }
   ```

2. **Update Existing Auth Components**
   - Modify `AuthenticationCallback.razor` to work with MSAL
   - Update `NavMenu.razor` to use MSAL auth state
   - Ensure `Login.razor` redirects properly

#### Phase 4: Backend Compatibility

1. **Enhanced Token Validation**
   ```csharp
   // PublicB2CTokenEndpoint.cs
   // Add proper B2C token validation using Microsoft.IdentityModel
   var validationParameters = new TokenValidationParameters
   {
       ValidateIssuer = true,
       ValidIssuers = new[] { $"{b2cInstance}/{b2cTenantId}/v2.0/" },
       ValidateAudience = true,
       ValidAudience = b2cClientId,
       ValidateLifetime = true,
       IssuerSigningKeys = await GetB2CSigningKeys()
   };
   ```

2. **Maintain Backward Compatibility**
   - Keep existing endpoints working
   - Support both old and new token formats during migration

#### Phase 5: Testing and Migration

1. **Test Scenarios**
   - New user registration
   - Existing user login
   - Token refresh
   - Logout flow
   - Multi-tenant switching

2. **Migration Strategy**
   - Deploy MSAL version alongside existing
   - Use feature flags to control rollout
   - Monitor for issues
   - Gradual user migration

### Key Files to Modify
- `src/apps/blazor/client/Program.cs`
- `src/apps/blazor/infrastructure/Auth/Extensions.cs`
- `src/apps/blazor/infrastructure/Auth/AzureB2C/B2CAuthenticationService.cs`
- `src/apps/blazor/client/Pages/Authentication.razor` (new)
- `src/api/framework/Infrastructure/Auth/AzureB2C/Endpoints/PublicB2CTokenEndpoint.cs`

### Configuration Changes
```json
// appsettings.json
{
  "AzureAdB2C": {
    "Instance": "https://{tenant}.b2clogin.com",
    "Domain": "{tenant}.onmicrosoft.com",
    "TenantId": "{tenantId}",
    "ClientId": "{clientId}",
    "SignUpSignInPolicyId": "B2C_1_SignUpSignIn",
    "ApiScope": "https://{tenant}.onmicrosoft.com/{api-app-id}/access_as_user"
  }
}
```

## Token Expiration Relationships

### Current System Expirations
- **B2C Tokens**: ~60 minutes (configurable in B2C policy)
- **Local JWT**: 60 minutes (configurable)
- **Refresh Token**: 7 days

### Recommended System Expirations
- **B2C Tokens**: Keep at 60 minutes (B2C default)
- **Local JWT**: **5 minutes** (reduced from 60)
- **Refresh Token**: **1 day** (reduced from 7)

### How They Work Together

#### 1. Initial Authentication Flow
```
User Login → B2C Token (60 min) → Exchange → Local JWT (5 min) + Refresh Token (1 day)
```

The B2C token is only used **once** during the exchange, then discarded. Its 60-minute lifetime becomes irrelevant after exchange.

#### 2. Token Refresh Scenarios

**Scenario A: Local JWT Expires (every 5 minutes)**
```
Local JWT expires → Use Refresh Token → Get new Local JWT (5 min)
```
- No B2C interaction needed
- Seamless to user
- Happens frequently (every 5 minutes)

**Scenario B: Refresh Token Expires (after 1 day)**
```
Refresh Token expires → Redirect to B2C → New B2C Token → Exchange → New tokens
```
- Requires full re-authentication
- User sees login screen
- Happens once per day

#### 3. With MSAL Integration

MSAL adds another layer with its own token cache:

```
MSAL Token Cache (B2C tokens)
    ├── Access Token (60 min)
    ├── ID Token (60 min)
    └── Refresh Token (90 days in B2C)
           ↓
    Token Exchange
           ↓
Local Token Storage
    ├── JWT (5 min)
    └── Refresh Token (1 day)
```

### Key Design Decisions

#### Why Short Local JWTs (5 min)?
1. **Security**: Limits exposure if token is stolen
2. **Revocation**: Near-instant effectiveness (max 5 min delay)
3. **Permission Updates**: Changes apply within 5 minutes

#### Why Keep B2C at 60 min?
1. **B2C Standards**: Follows OAuth2/OIDC best practices
2. **MSAL Compatibility**: Works well with MSAL's caching
3. **User Experience**: Fewer B2C round trips

#### Why 1-Day Refresh Tokens?
1. **Daily Re-auth**: Forces daily B2C validation
2. **Account Changes**: B2C account updates (disabled/deleted) take effect within 24 hours
3. **Compliance**: Many security frameworks require daily re-authentication

### Token Lifetime Coordination

```typescript
// Frontend token refresh logic
class TokenManager {
    private localJwtExpiry: Date;
    private refreshTokenExpiry: Date;
    private msalTokenExpiry: Date;
    
    async ensureValidToken(): Promise<string> {
        // Check local JWT first (expires every 5 min)
        if (this.localJwtExpiry < new Date()) {
            
            // Check if refresh token still valid
            if (this.refreshTokenExpiry > new Date()) {
                // Use local refresh token
                return await this.refreshLocalToken();
            } else {
                // Need full re-auth via B2C/MSAL
                return await this.reauthenticateWithB2C();
            }
        }
        
        return this.currentLocalJwt;
    }
    
    private async refreshLocalToken(): Promise<string> {
        // Call /api/refresh endpoint
        // Returns new 5-min JWT + same refresh token
    }
    
    private async reauthenticateWithB2C(): Promise<string> {
        // MSAL handles B2C auth
        const msalToken = await this.msalInstance.acquireTokenSilent();
        
        // Exchange for local tokens
        return await this.exchangeB2CToken(msalToken);
    }
}
```

### Edge Cases and Solutions

#### 1. B2C Token Expires During Exchange
```csharp
// In token exchange endpoint
if (IsTokenExpired(b2cToken)) {
    // Return specific error code
    return Results.Unauthorized("B2C_TOKEN_EXPIRED");
}
// Frontend handles by re-authenticating
```

#### 2. Rapid Token Refresh
```csharp
// Implement rate limiting
services.AddRateLimiter(options => {
    options.AddTokenBucketLimiter("token_refresh", opt => {
        opt.TokenLimit = 20;        // 20 refreshes
        opt.Window = TimeSpan.FromMinutes(5);  // Per 5 minutes
    });
});
```

#### 3. Clock Skew Issues
```csharp
// Allow small clock differences
TokenValidationParameters = new() {
    ClockSkew = TimeSpan.FromSeconds(30), // 30 sec tolerance
    ValidateLifetime = true
};
```

### Monitoring Token Expirations

```csharp
// Track token refresh patterns
public class TokenMetrics {
    // Alert if too many refreshes (possible attack)
    public int RefreshesPerMinute { get; set; }
    
    // Alert if many B2C re-auths (UX issue)  
    public int B2CReauthsPerHour { get; set; }
    
    // Track average session length
    public TimeSpan AverageSessionDuration { get; set; }
}
```

### Configuration Example

```json
{
  "JwtOptions": {
    "TokenExpirationInMinutes": 5,      // Short for security
    "RefreshTokenExpirationInDays": 1   // Daily re-auth
  },
  "AzureAdB2C": {
    "TokenLifetime": 3600,              // 60 minutes (B2C policy)
    "RefreshTokenLifetime": 7776000     // 90 days (B2C default)
  }
}
```

The key insight is that B2C tokens and local JWTs serve different purposes:
- **B2C tokens**: Prove initial authentication
- **Local JWTs**: Provide authorization with tenant context

By keeping local JWTs short-lived while maintaining standard B2C lifetimes, we get both security and good user experience.

## Local JWT Security Enhancement Plan

### Overview
Implement security improvements for local JWT token management, organized by priority.

### Security Enhancement Priorities

#### IMMEDIATE Priority (Do First)

##### 1. Implement Token Revocation
**Goal**: Allow immediate token invalidation on logout or security events

**Implementation**:
1. **Create Revocation Service**
   ```csharp
   // src/api/framework/Core/Identity/Tokens/ITokenRevocationService.cs
   public interface ITokenRevocationService
   {
       Task RevokeTokenAsync(string jti, DateTime expiry);
       Task<bool> IsTokenRevokedAsync(string jti);
       Task RevokeAllUserTokensAsync(string userId);
       Task CleanupExpiredRevocationsAsync(); // Housekeeping
   }
   ```

2. **Redis Implementation**
   ```csharp
   // src/api/framework/Infrastructure/Identity/Tokens/RedisTokenRevocationService.cs
   public class RedisTokenRevocationService : ITokenRevocationService
   {
       // Use Redis SET with TTL matching token expiry
       // Key: "revoked_token:{jti}"
       // Value: Revocation timestamp
       // TTL: Token expiry time
   }
   ```

3. **Update Token Generation**
   ```csharp
   // src/api/framework/Infrastructure/Identity/Tokens/TokenService.cs
   // Add JTI claim to all tokens
   claims.Add(new Claim("jti", Guid.NewGuid().ToString()));
   ```

4. **Update JWT Validation**
   ```csharp
   // src/api/framework/Infrastructure/Auth/Jwt/ConfigureJwtBearerOptions.cs
   OnTokenValidated = async context =>
   {
       var jti = context.Principal.FindFirst("jti")?.Value;
       if (await revocationService.IsTokenRevokedAsync(jti))
       {
           context.Fail("Token has been revoked");
       }
   }
   ```

##### 2. Reduce Token Lifetime
**Goal**: Minimize exposure window for compromised tokens

**Implementation**:
1. **Update Configuration**
   ```csharp
   // src/api/framework/Core/Auth/Jwt/JwtOptions.cs
   public int TokenExpirationInMinutes { get; set; } = 5; // Was 60
   public int RefreshTokenExpirationInDays { get; set; } = 1; // Was 7
   ```

2. **Add Silent Refresh in Frontend**
   ```typescript
   // src/apps/blazor/client/wwwroot/js/auth.js
   class TokenManager {
       scheduleTokenRefresh(expiresIn) {
           const refreshTime = (expiresIn - 60) * 1000; // 1 min before expiry
           setTimeout(() => this.silentRefresh(), refreshTime);
       }
   }
   ```

#### HIGH Priority

##### 3. Implement Refresh Token Rotation
**Goal**: Prevent refresh token reuse attacks

**Implementation**:
1. **Update Refresh Token Generation**
   ```csharp
   // src/api/framework/Infrastructure/Identity/Tokens/TokenService.cs
   public async Task<string> GenerateRefreshTokenAsync()
   {
       var token = GenerateSecureRandomToken();
       var hashedToken = BCrypt.Net.BCrypt.HashPassword(token);
       return token; // Return unhashed, store hashed
   }
   ```

2. **One-Time Use Refresh Tokens**
   ```csharp
   // In RefreshTokenAsync method
   // Invalidate old token immediately after validation
   user.RefreshToken = null;
   user.RefreshTokenExpiryTime = null;
   await _userManager.UpdateAsync(user);
   
   // Generate new pair
   var newTokens = await GenerateTokensAsync(user);
   ```

3. **Add Refresh Token Family Tracking**
   ```csharp
   // Track refresh token chains to detect reuse
   public class RefreshTokenFamily
   {
       public string FamilyId { get; set; }
       public List<string> UsedTokens { get; set; }
       public DateTime CreatedAt { get; set; }
   }
   ```

##### 4. Secure Refresh Token Storage
**Goal**: Protect refresh tokens in database

**Implementation**:
1. **Hash Tokens Before Storage**
   ```csharp
   // Update FshUser entity
   public string RefreshTokenHash { get; set; } // Instead of RefreshToken
   
   // Storage
   user.RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
   
   // Validation
   var isValid = BCrypt.Net.BCrypt.Verify(providedToken, user.RefreshTokenHash);
   ```

2. **Add Encryption Layer**
   ```csharp
   // For extra sensitive environments
   var encryptedHash = _dataProtector.Protect(hashedToken);
   ```

#### MEDIUM Priority

##### 5. Implement Token Binding
**Goal**: Bind tokens to specific devices/browsers

**Implementation**:
1. **Generate Device Fingerprint**
   ```csharp
   // src/api/framework/Infrastructure/Auth/DeviceFingerprintService.cs
   public string GenerateFingerprint(HttpContext context)
   {
       var components = new[]
       {
           context.Request.Headers["User-Agent"].ToString(),
           context.Connection.RemoteIpAddress?.ToString(),
           context.Request.Headers["Accept-Language"].ToString()
       };
       
       var fingerprint = string.Join("|", components);
       return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(fingerprint)));
   }
   ```

2. **Add to Token Claims**
   ```csharp
   claims.Add(new Claim("device_fingerprint", fingerprint));
   ```

3. **Validate on Each Request**
   ```csharp
   // In JWT middleware
   var tokenFingerprint = principal.FindFirst("device_fingerprint")?.Value;
   var currentFingerprint = _fingerprintService.Generate(context);
   if (tokenFingerprint != currentFingerprint)
   {
       context.Fail("Device fingerprint mismatch");
   }
   ```

##### 6. Move from LocalStorage to Secure Storage
**Goal**: Protect tokens from XSS attacks

**Implementation**:
1. **Use SessionStorage for Access Tokens**
   ```typescript
   // src/apps/blazor/infrastructure/Storage/SecureTokenStorage.ts
   class SecureTokenStorage {
       private memoryStorage = new Map<string, string>();
       
       setAccessToken(token: string) {
           // Memory only - most secure
           this.memoryStorage.set('access_token', token);
           
           // Or sessionStorage - clears on tab close
           sessionStorage.setItem('access_token', token);
       }
   }
   ```

2. **HttpOnly Cookies for Refresh Tokens**
   ```csharp
   // Backend: Set refresh token as httpOnly cookie
   context.Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
   {
       HttpOnly = true,
       Secure = true,
       SameSite = SameSiteMode.Strict,
       Expires = refreshTokenExpiry
   });
   ```

#### LOW Priority

##### 7. Enhanced Security Monitoring
**Goal**: Detect and respond to suspicious activity

**Implementation**:
1. **Token Usage Analytics**
   ```csharp
   // src/api/framework/Infrastructure/Auth/TokenUsageMonitor.cs
   public class TokenUsageMonitor
   {
       public async Task RecordUsageAsync(string userId, string jti, string ip)
       {
           // Track: Location changes, concurrent sessions, usage patterns
           await _redis.SetAddAsync($"user_sessions:{userId}", jti);
           await _redis.GeoAddAsync("user_locations", ip, userId);
       }
   }
   ```

2. **Anomaly Detection**
   ```csharp
   public async Task<bool> IsAnomalousAsync(string userId, string ip)
   {
       // Check for: Impossible travel, too many sessions, unusual times
       var recentLocations = await _redis.GeoRadiusAsync($"user:{userId}:locations");
       // Calculate distance/time and flag suspicious activity
   }
   ```

3. **Automated Response**
   ```csharp
   if (await IsAnomalousAsync(userId, ip))
   {
       await _revocationService.RevokeAllUserTokensAsync(userId);
       await _notificationService.AlertUserAsync(userId, "Suspicious login detected");
   }
   ```

### Implementation Order and Dependencies

1. **Week 1**: Token Revocation + Reduced Lifetime (IMMEDIATE)
   - Required for all other security features
   - Biggest immediate impact

2. **Week 2**: Refresh Token Security (HIGH)
   - Rotation + Hashing
   - Depends on revocation service

3. **Week 3**: Token Binding + Storage (MEDIUM)
   - Client and server changes
   - UX testing required

4. **Week 4**: Monitoring (LOW)
   - Requires all above features
   - Ongoing refinement

### Testing Strategy

1. **Security Test Suite**
   ```csharp
   [Test]
   public async Task RevokedToken_ShouldBeRejected()
   {
       var token = await GenerateToken();
       await _revocationService.RevokeToken(GetJti(token));
       var result = await CallApiWithToken(token);
       Assert.That(result.StatusCode, Is.EqualTo(401));
   }
   ```

2. **Load Testing**
   - Token refresh under load
   - Revocation service performance
   - Redis connection pooling

3. **Penetration Testing**
   - Token replay attacks
   - XSS token theft attempts
   - Refresh token abuse

### Monitoring and Alerts

1. **Key Metrics**
   - Failed authentications per user
   - Token refresh frequency
   - Revocation rate
   - Concurrent sessions

2. **Alert Thresholds**
   - \> 5 failed auths in 5 minutes
   - \> 10 concurrent sessions
   - Refresh from new location
   - Revoked token usage attempts

## Complete Authentication Flow with All Enhancements

### Overview
This section describes the complete authentication and authorization flow after implementing all security enhancements, including MSAL integration, token revocation, secure storage, and monitoring.

### 1. Initial Authentication Flow

#### Step 1: User Accesses Any Protected Resource
```
User navigates to https://app.com/dashboard (or any other protected page)
                    ↓
BaseLayout.razor detects no unexpired local JWT
                    ↓
App.razor renders <AuthRedirect /> component
```

#### Step 1a: Check for Valid B2C Token (Before Full Redirect)
```typescript
// AuthRedirect component first checks MSAL cache
const accounts = msalInstance.getAllAccounts();
if (accounts.length > 0) {
    try {
        // Try silent token acquisition first
        const response = await msalInstance.acquireTokenSilent({
            account: accounts[0],
            scopes: [apiScope]
        });
        
        // Valid B2C token exists - exchange for local JWT
        await exchangeB2CToken(response.accessToken);
        // User continues without seeing B2C login
        return;
    } catch (error) {
        // B2C token expired or invalid - continue to Step 2
    }
}
```

#### Step 2: MSAL-Managed B2C Redirect
```typescript
// AuthRedirect component with MSAL
if (!msalInstance.getAllAccounts().length) {
    // No cached accounts - initiate login
    await msalInstance.loginRedirect({
        scopes: ["openid", "offline_access", apiScope],
        redirectUri: window.location.origin + "/authentication/login-callback"
    });
}
```

#### Step 3: B2C Authentication
```
User redirected to: https://tenant.b2clogin.com/.../authorize
                    ↓
User enters credentials / social login
                    ↓
B2C validates and issues tokens (60 min lifetime)
                    ↓
Redirect to app with tokens in URL fragment
```

#### Step 4: MSAL Token Handling
```typescript
// MSAL automatically handles callback
const response = await msalInstance.handleRedirectPromise();
if (response) {
    // MSAL validates B2C tokens (signature, expiry, nonce)
    // Tokens stored in sessionStorage by default
    const account = response.account;
    const idToken = response.idToken;
    const accessToken = response.accessToken;
}
```

#### Step 4a: Transition to Token Exchange
```csharp
// MsalB2CAuthenticationService doesn't "continuously monitor" - it's called by the framework
public class MsalB2CAuthenticationService : AuthenticationStateProvider
{
    // This method is called by Blazor framework:
    // 1. When CascadingAuthenticationState first renders
    // 2. When NotifyAuthenticationStateChanged is called
    // 3. When any component calls GetAuthenticationStateAsync
    // 4. After navigation events
    protected override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try {
            // Check if we have B2C tokens but no local JWT
            var tokenResult = await _msalTokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out var token) && !HasValidLocalJWT())
            {
                // Trigger automatic exchange
                await ExchangeB2CTokenForLocalJWT(token.Value);
            }
            
            // Return authentication state based on local JWT
            return await GetLocalAuthenticationState();
        } catch {
            // Not authenticated
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }
    
    private async Task ExchangeB2CTokenForLocalJWT(string b2cToken)
    {
        // Automatically exchange B2C token for local JWT
        var success = await ExchangeTokenAsync(b2cToken);
        if (success) {
            // This triggers GetAuthenticationStateAsync to be called again
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}

// Detailed flow after MSAL callback:
// 1. User returns from B2C to /authentication/login-callback
// 2. Authentication.razor component handles the callback
// 3. MSAL stores B2C tokens and triggers re-render
// 4. CascadingAuthenticationState component re-evaluates
// 5. Framework calls GetAuthenticationStateAsync()
// 6. Method detects B2C tokens exist but no local JWT
// 7. Automatic exchange is triggered
// 8. NotifyAuthenticationStateChanged causes another evaluation
// 9. This time local JWT exists, auth state is updated
// 10. User is redirected to originally requested page

// Key trigger points for GetAuthenticationStateAsync:
await msalInstance.handleRedirectPromise(); // Triggers re-render
NavigationManager.NavigateTo("/"); // Triggers auth check
NotifyAuthenticationStateChanged(...); // Explicit trigger
<AuthorizeView> or [Authorize] // Component auth checks
```

#### Step 5: Token Exchange with Enhanced Security

##### Device Fingerprint Generation
```typescript
// Frontend: Generate unique device fingerprint
function generateFingerprint(): string {
    // Collect stable browser/device characteristics
    const components = {
        userAgent: navigator.userAgent,
        language: navigator.language,
        colorDepth: screen.colorDepth,
        deviceMemory: (navigator as any).deviceMemory || 'unknown',
        hardwareConcurrency: navigator.hardwareConcurrency || 'unknown',
        screenResolution: `${screen.width}x${screen.height}`,
        availableScreenResolution: `${screen.availWidth}x${screen.availHeight}`,
        timezoneOffset: new Date().getTimezoneOffset(),
        timezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
        sessionStorage: isStorageAvailable('sessionStorage'),
        localStorage: isStorageAvailable('localStorage'),
        indexedDb: !!window.indexedDB,
        addBehavior: !!(document.body as any).addBehavior,
        openDatabase: !!window.openDatabase,
        cpuClass: (navigator as any).cpuClass || 'unknown',
        platform: navigator.platform,
        plugins: getPluginsString(),
        canvas: getCanvasFingerprint(),
        webgl: getWebGLFingerprint(),
        webglVendor: getWebGLVendor(),
        adBlock: getAdBlockStatus(),
        hasLiedLanguages: getHasLiedLanguages(),
        hasLiedResolution: getHasLiedResolution(),
        hasLiedOs: getHasLiedOs(),
        hasLiedBrowser: getHasLiedBrowser(),
        touchSupport: getTouchSupport(),
        fonts: getFontsFingerprint()
    };
    
    // Combine all components into a single string
    const fingerprintString = Object.entries(components)
        .map(([key, value]) => `${key}:${value}`)
        .join('|');
    
    // Hash the fingerprint for consistency and privacy
    return sha256(fingerprintString);
}

// Simplified version for the actual implementation:
function generateFingerprint(): string {
    const components = [
        navigator.userAgent,
        navigator.language,
        screen.colorDepth,
        screen.width + 'x' + screen.height,
        new Date().getTimezoneOffset(),
        navigator.platform
    ];
    
    // Create a stable hash from components
    const fingerprintString = components.join('|');
    
    // Simple hash function (in production, use crypto.subtle.digest)
    let hash = 0;
    for (let i = 0; i < fingerprintString.length; i++) {
        const char = fingerprintString.charCodeAt(i);
        hash = ((hash << 5) - hash) + char;
        hash = hash & hash; // Convert to 32-bit integer
    }
    
    return Math.abs(hash).toString(16);
}
```

##### Token Exchange Request
```typescript
// Frontend: Exchange B2C token for local JWT
const deviceFingerprint = generateFingerprint();
const response = await fetch('/api/b2c-token', {  // No longer /api/public/
    method: 'POST',
    headers: {
        'Authorization': `Bearer ${accessToken}`,  // B2C token required
        'X-Device-Fingerprint': deviceFingerprint
    }
});
```

**Security Benefits of B2C Authorization**:
1. **No truly anonymous endpoints** - Every request requires authentication
2. **Automatic token validation** - Framework validates B2C token signature, expiry, audience
3. **Protection against malformed requests** - Invalid B2C tokens rejected before processing
4. **Consistent security model** - All endpoints require some form of authentication

The device fingerprint serves as a security measure to:
1. **Bind tokens to specific devices** - Prevents token theft and replay from different devices
2. **Reject unauthorized usage** - If a token is used from a different device, it's **rejected** (401 Unauthorized)
3. **Enable device-based policies** - Can enforce policies like "maximum devices per user"
4. **Improve audit trails** - Track which devices are accessing the system

**Important**: There are NO legitimate circumstances where a token should be used from a different device. This is a hard security boundary:
- If device fingerprint doesn't match = **Request rejected**
- User must re-authenticate on the new device
- Each device gets its own tokens bound to that specific device
- This prevents token theft even if tokens are somehow compromised

Important considerations:
- **Privacy**: Only collect necessary data, hash the result
- **Stability**: Use characteristics that don't change frequently
- **Uniqueness**: Combine multiple factors for better uniqueness
- **Performance**: Cache the fingerprint during a session

```csharp
// Backend: Configure dual authentication schemes in Program.cs
services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = "LocalJWT";
    options.DefaultChallengeScheme = "LocalJWT";
})
.AddJwtBearer("LocalJWT", options => {
    // Existing configuration for local JWTs
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "https://fullstackhero.net",
        ValidAudience = "fullstackhero",
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true
    };
})
.AddJwtBearer("B2CJWT", options => {
    // B2C token validation configuration
    options.MetadataAddress = $"{b2cInstance}/{b2cDomain}/{signUpSignInPolicyId}/v2.0/.well-known/openid-configuration";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudiences = new[] { b2cClientId, b2cApiClientId },
        ValidateLifetime = true
    };
});

// Backend: Enhanced token exchange endpoint with B2C authorization
[Authorize(AuthenticationSchemes = "B2CJWT")] // Requires valid B2C token
[HttpPost("/api/b2c-token")] // No longer "public"
public async Task<IResult> ExchangeB2CToken(HttpContext context)
{
    // 1. B2C token already validated by [Authorize] attribute
    // ClaimsPrincipal contains validated B2C claims
    var b2cClaims = context.User;
    var oid = b2cClaims.FindFirst("oid")?.Value;
    
    // 3. Map to local user with tenant context
    var user = await _userMappingService.GetOrCreateUserAsync(b2cClaims);
    
    // 4. Generate secure local JWT (5 min expiry)
    var jti = Guid.NewGuid().ToString();
    var deviceFingerprint = context.Request.Headers["X-Device-Fingerprint"];
    
    var claims = new List<Claim>
    {
        new Claim("jti", jti), // For revocation
        new Claim("device_fingerprint", deviceFingerprint), // Device binding
        new Claim("nameid", user.Id),
        new Claim("email", user.Email),
        new Claim("tenant", user.TenantId),
        new Claim("oid", oid), // Preserve B2C link
        // Roles and permissions included
    };
    
    var jwt = GenerateJWT(claims, TimeSpan.FromMinutes(5));
    
    // 5. Generate secure refresh token
    var refreshToken = GenerateSecureRefreshToken();
    var hashedRefreshToken = BCrypt.HashPassword(refreshToken);
    
    // 6. Store refresh token securely
    user.RefreshTokenHash = hashedRefreshToken;
    user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
    user.RefreshTokenFamily = Guid.NewGuid().ToString();
    await _userManager.UpdateAsync(user);
    
    // 7. Track token for monitoring
    await _tokenMonitor.RecordTokenIssuance(user.Id, jti, context.Connection.RemoteIpAddress);
    
    // 8. Set refresh token as HttpOnly cookie
    context.Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = user.RefreshTokenExpiry,
        Path = "/api/refresh"
    });
    
    return Results.Ok(new { token = jwt, expiresIn = 300 });
}
```

#### Step 6: Secure Token Storage
```typescript
// Frontend: Store JWT in memory only
class SecureTokenStore {
    private accessToken: string | null = null;
    private tokenExpiry: Date | null = null;
    
    setToken(token: string, expiresIn: number) {
        this.accessToken = token;
        this.tokenExpiry = new Date(Date.now() + (expiresIn * 1000));
        
        // Schedule refresh 1 minute before expiry
        this.scheduleRefresh(expiresIn - 60);
    }
    
    private scheduleRefresh(delaySeconds: number) {
        setTimeout(() => this.refreshToken(), delaySeconds * 1000);
    }
}
```

**UX Implications of Memory-Only Token Storage**:

**Important**: MSAL can be configured to use localStorage (cross-tab) instead of sessionStorage:
```typescript
// In MSAL configuration
builder.Services.AddMsalAuthentication(options =>
{
    options.ProviderOptions.Cache.CacheLocation = "localStorage"; // Cross-tab!
});
```

With MSAL using localStorage, the UX is significantly improved:

1. **Page Refresh = Silent Re-authentication**
   - User hits F5 → Local JWT lost → But MSAL has B2C token in localStorage
   - Silent token exchange happens automatically → User continues seamlessly
   - No login prompt needed if B2C token still valid (60 minutes)

2. **Full Multi-Tab Support**
   - New tab opened → No local JWT in memory
   - But MSAL finds B2C token in localStorage (shared across tabs)
   - Silent exchange happens automatically → New tab authenticated
   - User experience: Opens new tab and is already logged in!

**Actual UX Flow with localStorage MSAL**:
```typescript
// When new tab opens or page refreshes
async function initializeAuth() {
    // 1. Check if we have local JWT in memory
    if (!tokenStore.hasValidToken()) {
        
        // 2. Check MSAL for B2C token (in localStorage)
        try {
            const account = msalInstance.getAllAccounts()[0];
            if (account) {
                // 3. Silent token acquisition from localStorage
                const response = await msalInstance.acquireTokenSilent({
                    account,
                    scopes: [apiScope]
                });
                
                // 4. Exchange B2C token for local JWT
                await exchangeB2CToken(response.accessToken);
                // User never sees login screen!
            }
        } catch {
            // Only NOW do we need actual login
            await msalInstance.loginRedirect();
        }
    }
}
```

3. **B2C Refresh Token Behavior**
   
   **Important Clarification**: B2C issues its own refresh tokens (default 90 days):
   ```
   B2C Token Response:
   - Access Token: 60 minutes
   - ID Token: 60 minutes  
   - Refresh Token: 90 days (configurable in B2C policy)
   ```
   
   **MSAL Automatically Handles B2C Token Refresh**:
   ```typescript
   // When B2C access token expires after 60 minutes
   try {
       const response = await msalInstance.acquireTokenSilent({
           account: account,
           scopes: [apiScope],
           forceRefresh: false // MSAL will use refresh token if access token expired
       });
       // MSAL uses B2C refresh token to get new access token
       // User doesn't see login screen!
       // This works for up to 90 days (B2C refresh token lifetime)
   } catch (error) {
       // Only fails if B2C refresh token is expired (after 90 days)
       await msalInstance.loginRedirect();
   }
   ```
   
   **How the 90-Day B2C Refresh Token Actually Works**:
   
   B2C refresh tokens can be configured as either:
   1. **Rolling/Sliding Expiration** (common default):
      - Each time you use the refresh token, you get a NEW refresh token
      - The new refresh token is valid for another 90 days
      - Result: Active users never hit the 90-day limit
   
   2. **Absolute Expiration**:
      - Refresh token expires 90 days from initial issuance
      - Using it doesn't extend the lifetime
      - Result: Must re-authenticate every 90 days regardless
   
   **Practical Examples**:
   
   **Scenario 1: Daily Active User (Rolling Expiration)**
   ```
   Day 1: Login → Get 90-day refresh token
   Day 2: Use app → Refresh token used → Get NEW 90-day token
   Day 30: Use app → Refresh token used → Get NEW 90-day token
   Day 89: Use app → Refresh token used → Get NEW 90-day token
   Result: Never needs to re-authenticate as long as they use the app!
   ```
   
   **Scenario 2: Intermittent User (Rolling Expiration)**
   ```
   Day 1: Login → Get 90-day refresh token
   Day 30: Use app → Works fine, gets new 90-day token
   Day 120: Try to use app → Refresh token expired (unused for 90 days)
   Result: Must re-authenticate
   ```
   
   **Scenario 3: With 1-Day Local Refresh Token Limit**
   ```
   Day 1, 9am: Login → B2C refresh (90 days) + Local refresh (1 day)
   Day 1, 2pm: Use app → Local JWT refreshes (5 min tokens)
   Day 2, 10am: Local refresh expired → But B2C refresh still valid
              → Silent B2C token refresh → New token exchange
              → New 1-day local refresh token
   Result: Daily "check-in" with B2C, but no login prompt
   ```
   
   **B2C Policy Configuration** (in Azure Portal):
   ```xml
   <RefreshTokenLifetime>
     <Value>P90D</Value>  <!-- 90 days -->
     <Action>Rolling</Action>  <!-- or "Absolute" -->
   </RefreshTokenLifetime>
   ```
   
   **Critical Design Decision**: The 1-day local refresh token is the limiting factor:
   - Even though B2C tokens can refresh for 90+ days (with rolling)
   - Local refresh tokens expire after 1 day
   - This forces daily B2C token exchange for security
   - Ensures B2C account changes (disabled/deleted) take effect within 24 hours
   
   **Two Separate Refresh Token Systems**:
   1. **B2C Refresh Tokens** (in MSAL/localStorage):
      - Lifetime: 90 days (B2C policy)
      - Purpose: Get new B2C access tokens
      - Managed by: MSAL automatically
   
   2. **Local Refresh Tokens** (in HttpOnly cookies):
      - Lifetime: 1 day (app configuration)
      - Purpose: Get new local JWTs
      - Managed by: Your application

4. **Remaining UX Limitations**
   - **Browser closure**: Depends on MSAL configuration
     - With localStorage: B2C tokens persist, silent re-auth works
     - With sessionStorage: All tokens lost, login required
   - **Different browsers**: No sharing between Chrome/Firefox/Edge
   - **Incognito mode**: Isolated from regular browsing

4. **Optimal Configuration Summary**
   - **MSAL**: Use localStorage for B2C tokens (cross-tab)
   - **Local JWT**: Keep in memory only (security)
   - **Refresh tokens**: HttpOnly cookies (security)
   - **Result**: Seamless experience within 60-minute windows

5. **Security vs UX Trade-off**
   ```
   B2C Tokens (MSAL localStorage):
   - Less sensitive: Only proves identity
   - Cross-tab sharing acceptable
   - 60-minute lifetime limits exposure
   
   Local JWTs (memory only):
   - More sensitive: Contains permissions & tenant
   - No persistent storage for security
   - Short 5-minute lifetime further limits risk
   ```

**Alternative Storage Strategies with Trade-offs**:

```typescript
// Option 1: SessionStorage (persists across refreshes, not tabs)
class SessionTokenStore {
    setToken(token: string, expiresIn: number) {
        sessionStorage.setItem('access_token', token);
        sessionStorage.setItem('token_expiry', new Date(Date.now() + (expiresIn * 1000)).toISOString());
    }
    // Pros: Survives page refresh
    // Cons: Still vulnerable to XSS, no tab sharing
}

// Option 2: Secure memory with tab synchronization
class SynchronizedTokenStore {
    private channel = new BroadcastChannel('auth_tokens');
    
    constructor() {
        this.channel.onmessage = (event) => {
            if (event.data.type === 'token_update') {
                this.accessToken = event.data.token;
                this.tokenExpiry = new Date(event.data.expiry);
            }
        };
    }
    
    setToken(token: string, expiresIn: number) {
        this.accessToken = token;
        this.tokenExpiry = new Date(Date.now() + (expiresIn * 1000));
        
        // Broadcast to other tabs
        this.channel.postMessage({
            type: 'token_update',
            token: token,
            expiry: this.tokenExpiry
        });
    }
    // Pros: Secure + multi-tab support
    // Cons: Complex implementation
}

// Option 3: Service Worker storage
// Tokens stored in service worker memory, shared across all tabs/windows
```

### 2. API Request Flow with Enhanced Security

#### Step 1: Frontend API Call
```typescript
async function callAPI(endpoint: string) {
    // Get token from secure store
    const token = await tokenStore.getValidToken();
    
    const response = await fetch(endpoint, {
        headers: {
            'Authorization': `Bearer ${token}`,
            'X-Device-Fingerprint': generateFingerprint()
        },
        credentials: 'include' // For cookies
    });
    
    if (response.status === 401) {
        // Token expired or revoked
        await handleTokenRefresh();
    }
    
    return response;
}
```

#### Step 2: Backend JWT Validation
```csharp
// Enhanced JWT middleware
public async Task OnTokenValidated(TokenValidatedContext context)
{
    var principal = context.Principal;
    var jti = principal.FindFirst("jti")?.Value;
    
    // 1. Check token revocation
    if (await _revocationService.IsTokenRevokedAsync(jti))
    {
        context.Fail("Token has been revoked");
        return;
    }
    
    // 2. Validate device fingerprint
    var tokenFingerprint = principal.FindFirst("device_fingerprint")?.Value;
    var currentFingerprint = GenerateFingerprint(context.HttpContext);
    if (tokenFingerprint != currentFingerprint)
    {
        context.Fail("Device fingerprint mismatch");
        await _securityMonitor.RecordSuspiciousActivity(principal.GetUserId(), "Fingerprint mismatch");
        return;
    }
    
    // 3. Check for anomalies
    var userId = principal.GetUserId();
    var ipAddress = context.HttpContext.Connection.RemoteIpAddress;
    if (await _securityMonitor.IsAnomalousActivity(userId, ipAddress))
    {
        await _revocationService.RevokeAllUserTokensAsync(userId);
        context.Fail("Suspicious activity detected");
        return;
    }
    
    // 4. Track successful validation
    await _tokenMonitor.RecordTokenUsage(userId, jti, ipAddress);
}
```

#### Step 3: Authorization Check
```csharp
[Authorize]
[RequirePermission("View.Dashboard")]
public async Task<IActionResult> GetDashboard()
{
    // JWT already validated by middleware
    var userId = User.GetUserId();
    var tenantId = User.GetTenantId();
    
    // Permissions already in token (for B2C users)
    // No additional database lookup needed
    
    // Process request with tenant context
    using var scope = _tenantService.CreateScope(tenantId);
    var data = await _dashboardService.GetDataAsync();
    
    return Ok(data);
}
```

### 3. Token Refresh Flow

#### How Blazor Detects Need for Refresh

**The Complete Detection Flow**:

1. **Blazor app makes API call** with current local JWT (might be expired)
   ```typescript
   await fetch('/api/data', {
       headers: { 'Authorization': `Bearer ${localJWT}` }
   });
   ```

2. **API returns 401 Unauthorized** because JWT is expired
   ```csharp
   // JWT middleware automatically rejects expired tokens
   // Returns 401 with no body
   ```

3. **Blazor interceptor catches 401**
   ```typescript
   if (response.status === 401) {
       // Could be: expired token, revoked token, or no token
       return await handleUnauthorized();
   }
   ```

4. **Determine refresh strategy**
   ```typescript
   async function handleUnauthorized() {
       // Option A: Try local refresh token first
       if (hasRefreshTokenCookie()) {
           const refreshResult = await tryRefreshWithCookie();
           if (refreshResult.success) return retry();
       }
       
       // Option B: Try B2C token
       if (await msalInstance.getAllAccounts().length > 0) {
           try {
               const b2cToken = await msalInstance.acquireTokenSilent();
               const exchangeResult = await exchangeB2CToken(b2cToken);
               if (exchangeResult.success) return retry();
           } catch { }
       }
       
       // Option C: Full re-authentication
       await msalInstance.loginRedirect();
   }
   ```

**Key Insight**: You're absolutely right - we could use the B2C token instead of accepting expired JWTs!

**Why the Traditional Approach Uses Expired JWTs**:
1. **Historical**: Before B2C/MSAL, apps only had their own tokens
2. **Offline capability**: Refresh tokens work even if IdP is down
3. **Performance**: One hop instead of two (refresh vs B2C+exchange)
4. **Independence**: Not coupled to external IdP for every refresh

**Why B2C Tokens Could Replace Expired JWT Acceptance**:
```typescript
// Modern approach - always use B2C token for refresh
async function refreshLocalJWT() {
    try {
        // Get fresh B2C token (uses B2C refresh token if needed)
        const b2cToken = await msalInstance.acquireTokenSilent({
            scopes: [apiScope],
            forceRefresh: false
        });
        
        // Exchange for local JWT (same as initial login)
        return await exchangeB2CToken(b2cToken.accessToken);
    } catch {
        // B2C refresh failed - need full login
        await msalInstance.loginRedirect();
    }
}

// Backend - just use B2C authorization everywhere
[Authorize(AuthenticationSchemes = "B2CJWT")]
[HttpPost("/api/refresh")]  // Or just use /api/b2c-token for everything
public async Task<IResult> RefreshWithB2CToken(HttpContext context)
{
    // Identical to initial token exchange
    // No need for separate refresh logic!
}
```

**The Architectural Choice**:

**Option 1: Traditional Dual-Token with Refresh Tokens**
- ✅ Works offline (refresh tokens are self-contained)
- ✅ Faster (no B2C round-trip)
- ✅ Less load on B2C
- ❌ More complex (two refresh mechanisms)
- ❌ Requires accepting expired tokens

**Option 2: Always Use B2C Tokens (Simplified)**
- ✅ Simpler - one flow for everything
- ✅ Always validates against B2C
- ✅ No need to accept expired tokens
- ✅ B2C handles all refresh logic
- ❌ Requires B2C availability
- ❌ Slightly slower (B2C round-trip)

**Recommendation**: For maximum security and simplicity, Option 2 (always use B2C) is better unless you need offline capability.

#### Step 1: Automatic Refresh (Every 5 Minutes)
```typescript
// Frontend: Silent token refresh
async function refreshToken() {
    try {
        const response = await fetch('/api/refresh', {
            method: 'POST',
            credentials: 'include', // Sends HttpOnly refresh cookie
            headers: {
                'X-Device-Fingerprint': generateFingerprint()
            }
        });
        
        if (response.ok) {
            const { token, expiresIn } = await response.json();
            tokenStore.setToken(token, expiresIn);
        } else if (response.status === 401) {
            // Refresh token expired - need full re-auth
            await initiateReauthentication();
        }
    } catch (error) {
        console.error('Token refresh failed', error);
        await initiateReauthentication();
    }
}
```

#### Step 2: Backend Refresh Handling

```csharp
// Custom authorization policy that accepts expired tokens
services.AddAuthorization(options =>
{
    options.AddPolicy("AllowExpiredToken", policy =>
    {
        policy.AddAuthenticationSchemes("LocalJWT");
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new AllowExpiredTokenRequirement());
    });
});

// Custom requirement handler
public class AllowExpiredTokenHandler : AuthorizationHandler<AllowExpiredTokenRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AllowExpiredTokenRequirement requirement)
    {
        // Accept the token even if expired (for refresh endpoint only)
        var httpContext = context.Resource as HttpContext;
        if (httpContext?.Request.Path == "/api/refresh")
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

// Option 1: Authorize with expired local JWT (recommended)
[Authorize(Policy = "AllowExpiredToken")]
[RateLimiter("token_refresh")]
[HttpPost("/api/refresh")]
public async Task<IResult> RefreshToken(HttpContext context)
{
    // User already authenticated with expired token
    var userId = context.User.GetUserId();
    
    // 1. Get refresh token from HttpOnly cookie
    var refreshToken = context.Request.Cookies["refresh_token"];
    if (string.IsNullOrEmpty(refreshToken))
        return Results.Unauthorized();

// Option 2: Dual authorization - Accept EITHER expired local JWT OR valid B2C token
[Authorize(Policy = "AllowExpiredToken", AuthenticationSchemes = "LocalJWT,B2CJWT")]
[RateLimiter("token_refresh")]
[HttpPost("/api/refresh")]
public async Task<IResult> RefreshToken(HttpContext context)
{
    string userId;
    bool isB2CToken = context.User.HasClaim(c => c.Type == "iss" && c.Value.Contains("b2clogin.com"));
    
    if (isB2CToken)
    {
        // B2C token provided - do full token exchange
        var b2cClaims = context.User;
        var user = await _userMappingService.GetOrCreateUserAsync(b2cClaims);
        userId = user.Id;
        // Generate both access and refresh tokens
    }
    else
    {
        // Expired local JWT provided - standard refresh flow
        userId = context.User.GetUserId();
    }

/**
 * Security Benefits over [AllowAnonymous]:
 * 1. Requires SOME form of authentication (expired JWT or valid B2C token)
 * 2. Can extract user identity for audit logging
 * 3. Prevents anonymous refresh token fishing attacks
 * 4. Still allows refresh with expired tokens (the main requirement)
 * 5. Enables B2C token as alternative path for refresh
 */
    
    // 3. Validate refresh token
    var user = await _userManager.FindByIdAsync(userId);
    if (!BCrypt.Verify(refreshToken, user.RefreshTokenHash))
    {
        // Possible token theft - revoke all tokens
        await _revocationService.RevokeAllUserTokensAsync(userId);
        await _securityMonitor.AlertPossibleTokenTheft(userId);
        return Results.Unauthorized();
    }
    
    // 4. Check refresh token expiry
    if (user.RefreshTokenExpiry < DateTime.UtcNow)
        return Results.Unauthorized("Refresh token expired");
    
    // 5. Rotate refresh token (one-time use)
    var newRefreshToken = GenerateSecureRefreshToken();
    user.RefreshTokenHash = BCrypt.HashPassword(newRefreshToken);
    user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
    await _userManager.UpdateAsync(user);
    
    // 6. Revoke old JWT
    var oldJti = principal.FindFirst("jti")?.Value;
    await _revocationService.RevokeTokenAsync(oldJti, DateTime.UtcNow.AddMinutes(5));
    
    // 7. Generate new JWT
    var newJwt = await GenerateNewJWT(user, context);
    
    // 8. Set new refresh token cookie
    context.Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = user.RefreshTokenExpiry,
        Path = "/api/refresh"
    });
    
    return Results.Ok(new { token = newJwt, expiresIn = 300 });
}
```

### 4. Re-Authentication Flow (Daily)

#### Step 1: Refresh Token Expiry Detection
```typescript
// After 1 day, refresh token expires
async function handleExpiredRefreshToken() {
    // Clear any cached tokens
    tokenStore.clear();
    
    // MSAL checks for valid B2C session
    try {
        const account = msalInstance.getAllAccounts()[0];
        const response = await msalInstance.acquireTokenSilent({
            account,
            scopes: [apiScope]
        });
        
        // Exchange new B2C token
        await exchangeB2CToken(response.accessToken);
    } catch (error) {
        // B2C session also expired - full login needed
        await msalInstance.loginRedirect({
            scopes: [apiScope]
        });
    }
}
```

### 5. Logout Flow

#### Step 1: User-Initiated Logout
```typescript
async function logout() {
    // 1. Call backend to revoke tokens
    await fetch('/api/auth/logout', {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${currentToken}`
        },
        credentials: 'include'
    });
    
    // 2. Clear local token store
    tokenStore.clear();
    
    // 3. Logout from MSAL/B2C
    await msalInstance.logoutRedirect({
        postLogoutRedirectUri: window.location.origin
    });
}
```

#### Step 2: Backend Token Revocation
```csharp
[Authorize]
public async Task<IResult> Logout(HttpContext context)
{
    var userId = context.User.GetUserId();
    var jti = context.User.FindFirst("jti")?.Value;
    
    // 1. Revoke current JWT
    await _revocationService.RevokeTokenAsync(jti, DateTime.UtcNow.AddMinutes(5));
    
    // 2. Optionally revoke all user tokens
    if (context.Request.Query["all"] == "true")
    {
        await _revocationService.RevokeAllUserTokensAsync(userId);
    }
    
    // 3. Clear refresh token
    var user = await _userManager.FindByIdAsync(userId);
    user.RefreshTokenHash = null;
    user.RefreshTokenExpiry = null;
    await _userManager.UpdateAsync(user);
    
    // 4. Clear refresh token cookie
    context.Response.Cookies.Delete("refresh_token");
    
    // 5. Log security event
    await _auditService.LogLogoutAsync(userId, context.Connection.RemoteIpAddress);
    
    return Results.Ok();
}
```

### 6. Security Monitoring Throughout

#### Continuous Monitoring
```csharp
public class SecurityMonitor
{
    public async Task MonitorTokenUsage(string userId, string jti, IPAddress ip)
    {
        // Track concurrent sessions
        var activeSessions = await _redis.SetLengthAsync($"user:{userId}:sessions");
        if (activeSessions > 10)
        {
            await _alertService.SendAlert($"User {userId} has {activeSessions} active sessions");
        }
        
        // Track location changes
        var lastLocation = await _redis.GeoPositionAsync($"user:{userId}:locations", "last");
        var currentLocation = await _geoService.GetLocation(ip);
        
        if (lastLocation != null)
        {
            var distance = GeoCalculator.Distance(lastLocation, currentLocation);
            var timeDiff = DateTime.UtcNow - lastLocation.Timestamp;
            
            // Impossible travel detection
            if (distance > 1000 && timeDiff.TotalHours < 1)
            {
                await _revocationService.RevokeAllUserTokensAsync(userId);
                await _alertService.SendSecurityAlert(userId, "Impossible travel detected");
            }
        }
        
        // Update tracking
        await _redis.GeoAddAsync($"user:{userId}:locations", 
            (currentLocation.Longitude, currentLocation.Latitude, "last"));
    }
}
```

### 7. Error Handling and Recovery

#### Token Validation Failures
```typescript
// Frontend: Comprehensive error handling
async function handleAuthError(error: AuthError) {
    switch (error.code) {
        case 'TOKEN_EXPIRED':
            await refreshToken();
            break;
            
        case 'TOKEN_REVOKED':
        case 'DEVICE_MISMATCH':
            // Security issue - full re-auth
            await forceReauthentication();
            break;
            
        case 'REFRESH_TOKEN_EXPIRED':
            // Daily re-auth needed
            await msalInstance.loginRedirect();
            break;
            
        case 'ACCOUNT_LOCKED':
            // Show security message
            showSecurityAlert('Your account has been locked. Please contact support.');
            break;
            
        default:
            console.error('Auth error:', error);
            await msalInstance.loginRedirect();
    }
}
```

### Summary of Security Enhancements in the Flow

1. **MSAL Integration**: Proper B2C token validation, secure caching, automatic refresh
2. **Token Revocation**: JTI tracking, immediate invalidation, revocation checking on every request
3. **Short-Lived Tokens**: 5-minute JWTs minimize exposure window
4. **Secure Storage**: Memory-only for access tokens, HttpOnly cookies for refresh tokens
5. **Device Binding**: Fingerprint validation prevents token theft
6. **Refresh Token Rotation**: One-time use prevents replay attacks
7. **Anomaly Detection**: Impossible travel, concurrent session limits, suspicious activity alerts
8. **Rate Limiting**: Prevents brute force on refresh endpoint
9. **Comprehensive Monitoring**: All token operations tracked and analyzed
10. **Graceful Degradation**: Clear error handling and recovery paths

This enhanced flow provides defense-in-depth security while maintaining a smooth user experience with automatic token refresh and daily re-authentication.