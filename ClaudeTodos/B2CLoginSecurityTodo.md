# B2C Login Security Todo

Looking at the B2C token exchange flow after authentication, I've identified several security concerns:

## Current Flow After B2C Authentication:

1. B2C redirects to app with JWT in URL fragment
2. Client-side JavaScript extracts the JWT
3. Client calls `/api/public/b2c-token` with B2C JWT in Authorization header
4. Server exchanges B2C JWT for local JWT

## Security Issues Identified:

### 1. **Insufficient B2C Token Validation** (PublicB2CTokenEndpoint.cs)
- **Lines 71-86**: Only checks if issuer isn't the local issuer
- **No signature validation** of the B2C JWT
- **No expiration check**
- **No audience validation**
- **No nonce validation**

### 2. **Replay Attack Vulnerability**
- Same B2C token could be exchanged multiple times
- No tracking of used tokens
- No time window restrictions

### 3. **Token in URL Fragment**
- While fragments aren't sent to server, they can be:
  - Logged in browser history
  - Captured by browser extensions
  - Exposed in referrer headers to third-party resources

### 4. **Missing PKCE (Proof Key for Code Exchange)**
- The current flow doesn't appear to use PKCE
- Makes token interception attacks easier

### 5. **No State Parameter Validation**
- Current flow passes state but doesn't validate it server-side
- Vulnerable to CSRF attacks

## Recommended Security Enhancements:

### 1. **Proper B2C Token Validation**
```csharp
// Add proper JWT validation
var validationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuers = new[] { $"{b2cOptions.Instance}/{b2cOptions.Domain}/v2.0/" },
    ValidateAudience = true,
    ValidAudience = b2cOptions.ClientId,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    IssuerSigningKeys = await GetB2CSigningKeys() // Fetch from B2C metadata endpoint
};
```

### 2. **Implement Token Usage Tracking**
- Cache used B2C token JTIs for their lifetime
- Reject already-exchanged tokens
- Use distributed cache for multi-instance deployments

### 3. **Add Time Window Check**
- Only accept tokens issued within last 5 minutes
- Check `iat` (issued at) claim

### 4. **Validate State Parameter**
- Generate cryptographically secure state before redirect
- Store in session/cache with expiration
- Validate on return

### 5. **Consider Authorization Code Flow**
- Instead of implicit flow with tokens in URL
- Use authorization code flow with PKCE
- More secure for SPAs

### 6. **Add Rate Limiting**
- Limit token exchange attempts per IP
- Prevent brute force attempts

### 7. **Secure Token Transport**
- Consider using POST body instead of Authorization header
- Add request signing if possible

## Critical Implementation Gap:

The most concerning issue is the lack of B2C token signature validation. Without this, anyone could craft a fake B2C token and exchange it for a valid local token. This should be the top priority fix.